using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.MinorPets;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class StockTerrariumsScenario : LotScenario
    {
        public StockTerrariumsScenario()
        { }
        protected StockTerrariumsScenario(StockTerrariumsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "StockTerrariums";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool> ()) return false;

            return base.Allow();
        }

        protected override bool Allow(Lot lot)
        {
            if (lot.IsActive)
            {
                IncStat("Active");
                return false;
            }
            else if (lot.Household == null)
            {
                IncStat("No Family");
                return false;
            }
            else if (lot.IsWorldLot)
            {
                IncStat("World Lot");
                return false;
            }
            else if (!lot.IsResidentialLot)
            {
                IncStat("Commercial");
                return false;
            }

            return base.Allow(lot);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            foreach (MinorPetTerrarium terrarium in Lot.GetObjects<MinorPetTerrarium>())
            {
                if (terrarium.HasLivingPet) continue;

                if (terrarium.Pet != null)
                {
                    terrarium.Pet.FadeOut(false, true);

                    IncStat("Dead Pet Removed");
                }

                if (terrarium.Cleanable != null)
                {
                    terrarium.Cleanable.ForceClean();

                    IncStat("Cleaned");
                }

                List<MinorPetSpecies> choices = new List<MinorPetSpecies>();

                foreach (KeyValuePair<MinorPetSpecies, MinorPetData> pair in MinorPet.sData)
                {
                    if (!pair.Value.Stockable) continue;

                    if (Lot.Household.FamilyFunds < pair.Value.StockCost) continue;

                    if (!terrarium.AllowsPetType(pair.Value.MinorPetType)) continue;

                    choices.Add(pair.Key);
                }

                AddStat("Choice", choices.Count);

                if (choices.Count > 0)
                {
                    MinorPet pet = MinorPet.Make(RandomUtil.GetRandomObjectFromList(choices), true, false) as MinorPet;
                    if (pet == null)
                    {
                        IncStat("Creation Fail");
                    }
                    else
                    {
                        pet.SetBehaviorSMCStateStopped();

                        if (pet.ParentToSlot(terrarium, terrarium.PetContainmentSlot))
                        {
                            pet.SetOpacity(0f, 0f);
                            pet.FadeIn();
                            pet.SetBehaviorSMCStateActive();
                            pet.StartBehaviorSMC(false);

                            Money.AdjustFunds(Lot.Household, "MinorPets", -pet.Data.StockCost);

                            IncStat("Pet Placed");
                        }
                        else
                        {
                            pet.FadeOut();

                            IncStat("Placement Fail");
                        }
                    }
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new StockTerrariumsScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerLot, StockTerrariumsScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "StockTerrariums";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }
        }
    }
}
