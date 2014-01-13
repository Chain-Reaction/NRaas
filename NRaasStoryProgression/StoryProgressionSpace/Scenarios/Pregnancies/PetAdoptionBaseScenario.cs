using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Pets;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public abstract class PetAdoptionBaseScenario : AdoptionBaseScenario
    {
        bool mForceInspection;

        public PetAdoptionBaseScenario()
        { }
        protected PetAdoptionBaseScenario(bool forceInspection)
        {
            mForceInspection = forceInspection;
        }
        protected PetAdoptionBaseScenario(CASAgeGenderFlags species)
            : base(species)
        { }
        protected PetAdoptionBaseScenario(SimDescription newSim, bool forceInspection)
            : base(newSim)
        {
            mForceInspection = forceInspection;
        }
        protected PetAdoptionBaseScenario(PetAdoptionBaseScenario scenario)
            : base (scenario)
        {
            mForceInspection = scenario.mForceInspection;
        }

        public static void GetPossibleSpecies(Scenario scenario, Lot lot, SimDescription head, bool forceInspection, List<CASAgeGenderFlags> species)
        {
            bool success = true;
            if ((lot != null) && ((forceInspection) || (scenario.GetValue<RequireInspectionOption, bool>())))
            {
                success = ((lot.CountObjects<IPetBed>() + lot.CountObjects<IPetHouse>()) > 0);
            }

            if (success)
            {
                if (scenario.AddScoring("LikesCats", head) > 0)
                {
                    species.Add(CASAgeGenderFlags.Cat);
                }

                if (scenario.AddScoring("LikesDogs", head) > 0)
                {
                    species.Add(CASAgeGenderFlags.Dog);
                    species.Add(CASAgeGenderFlags.LittleDog);
                }
            }
            else
            {
                scenario.IncStat("No Pet House");
            }

            if (scenario.AddScoring("LikesHorses", head) > 0)
            {
                if ((lot != null) && (lot.CountObjects<IBoxStall>() > 0))
                {
                    species.Add(CASAgeGenderFlags.Horse);
                }
                else
                {
                    scenario.IncStat("No Horse Stalls");
                }
            }
        }

        protected override void GetPossibleSpecies(Household house, List<CASAgeGenderFlags> species)
        {
            GetPossibleSpecies(this, house.LotHome, SimTypes.HeadOfFamily(house), mForceInspection, species);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (!GetValue<AllowAdoptionOption, bool>(sim))
            {
                IncStat("Adoption Denied");
                return false;
            }
            else if (!Households.Allow(this, sim, 0))
            {
                IncStat("User Denied");
                return false;
            }
            else if (AddScoring("Adoption Cooldown", TestElapsedTime<DayOfLastPetOption, MinTimeBetweenAdoptionOption>(sim)) < 0)
            {
                AddStat("Too Soon After Adoption", GetElapsedTime<DayOfLastPetOption>(sim));
                return false;
            }
            else if (Situation.GetSituations<SocialWorkerPetAdoptionSituation>().Count > 0)
            {
                IncStat("Adoption Situation");
                return false;
            }
            else if ((sim.IsHorse) && (sim.Child))
            {
                IncStat("Foal");
                return false;
            }

            return true;
        }

        protected override void UpdateDayOfLastOption(SimDescription sim)
        {
            foreach (SimDescription member in HouseholdsEx.All(sim.Household))
            {
                SetElapsedTime<DayOfLastPetOption>(sim);
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (mNewSim != null)
            {
                return base.PrivateUpdate(frame);
            }

            if (GetValue<PetAdoptionScenario.UsePetPoolOption, bool>())
            {
                PetPoolType poolType = PetPoolType.None;
                switch (Species)
                {
                    case CASAgeGenderFlags.Dog:
                    case CASAgeGenderFlags.LittleDog:
                        poolType = PetPoolType.AdoptDog;
                        break;
                    case CASAgeGenderFlags.Cat:
                        poolType = PetPoolType.AdoptCat;
                        break;
                    case CASAgeGenderFlags.Horse:
                        poolType = PetPoolType.AdoptHorse;
                        break;
                }

                List<SimDescription> choices = PetPoolManager.GetPetsByType(poolType);
                if ((choices != null) && (choices.Count > 0))
                {
                    CASAgeGenderFlags ages = Ages;
                    for (int i = choices.Count - 1; i >= 0; i--)
                    {
                        if ((ages & choices[i].Age) != choices[i].Age)
                        {
                            choices.RemoveAt(i);
                        }
                        else if (choices[i].Species != Species)
                        {
                            choices.RemoveAt(i);
                        }
                    }

                    if (choices.Count == 0)
                    {
                        IncStat("No Matching In Pool");
                    }
                    else
                    {
                        mNewSim = RandomUtil.GetRandomObjectFromList(choices);

                        PetPoolManager.RemovePet(poolType, mNewSim, true);
                    }

                    return base.PrivateUpdate(frame);
                }
                else
                {
                    IncStat("Pool Empty: " + poolType);
                }
            }

            if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() > 0)
            {
                return base.PrivateUpdate(frame);
            }
            else
            {
                IncStat("Immigration Disabled");
                return false;
            }
        }

        public class MinTimeBetweenAdoptionOption : Manager.CooldownOptionItem<ManagerPregnancy>, ManagerPregnancy.IAdoptionOption
        {
            public MinTimeBetweenAdoptionOption()
                : base(3)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownBetweenPetAdoption";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }
        }

        public class RequireInspectionOption : BooleanManagerOptionItem<ManagerPregnancy>, ManagerPregnancy.IAdoptionOption
        {
            public RequireInspectionOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "RequirePetInspection";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }
        }

        public class ChanceOfAnotherOptionV2 : IntegerManagerOptionItem<ManagerPregnancy>, ManagerPregnancy.IAdoptionOption
        {
            public ChanceOfAnotherOptionV2()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceOfAnotherPetAdoption";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }
        }
    }
}
