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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public abstract class AdoptionBaseScenario : SimScenario
    {
        protected SimDescription mNewSim;

        CASAgeGenderFlags mSpecies;

        public AdoptionBaseScenario()
        { }
        protected AdoptionBaseScenario(CASAgeGenderFlags species)
        {
            mSpecies = species;
        }
        protected AdoptionBaseScenario(SimDescription newSim)
        {
            mNewSim = newSim;
            mSpecies = newSim.Species;
        }
        protected AdoptionBaseScenario(AdoptionBaseScenario scenario)
            : base (scenario)
        {
            mNewSim = scenario.mNewSim;
            mSpecies = scenario.mSpecies;
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected virtual int Delta
        {
            get { return 0; }
        }

        protected virtual CASAgeGenderFlags Ages
        {
            get { return CASAgeGenderFlags.Child; }
        }

        protected CASAgeGenderFlags Species
        {
            get { return mSpecies; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected abstract void GetPossibleSpecies(Household house, List<CASAgeGenderFlags> species);

        protected CASAgeGenderFlags GetSpecies(SimDescription sim)
        {
            List<CASAgeGenderFlags> species = new List<CASAgeGenderFlags>();
            if (mSpecies != CASAgeGenderFlags.None)
            {
                species.Add(mSpecies);
            }
            else
            {
                GetPossibleSpecies(sim.Household, species);
            }

            AddStat("Choices", species.Count);

            bool allowAnother = RandomUtil.RandomChance(GetValue<PetAdoptionBaseScenario.ChanceOfAnotherOptionV2, int>());

            for (int i = species.Count - 1; i >= 0; i--)
            {
                CASAgeGenderFlags choice = species[i];

                if (Sims.HasEnough(this, choice))
                {
                    IncStat("Maximum " + species);
                    species.RemoveAt(i);
                }
                else if (HouseholdsEx.IsFull(this, sim.Household, choice, 0, true, allowAnother))
                {
                    species.RemoveAt(i);
                }
            }

            if (species.Count > 0)
            {
                if (mNewSim != null)
                {
                    if (!species.Contains(mNewSim.Species))
                    {
                        IncStat("House Wrong Species");
                        return CASAgeGenderFlags.None;
                    }
                    else
                    {
                        return mNewSim.Species;
                    }
                }

                return RandomUtil.GetRandomObjectFromList(species);
            }
            else
            {
                return CASAgeGenderFlags.None;
            }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }
            else if ((mNewSim != null) && (mNewSim.Household == sim.Household))
            {
                IncStat("Same House");
                return false;
            }
            else if (!Households.AllowGuardian(sim))
            {
                IncStat("Too Young");
                return false;
            }
            else if (!GetValue<AllowAdoptionOption, bool>(sim))
            {
                IncStat("Adoption Denied");
                return false;
            }
            else
            {
                mSpecies = GetSpecies(sim);
                if (mSpecies == CASAgeGenderFlags.None)
                {
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected abstract void UpdateDayOfLastOption(SimDescription sim);

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (mNewSim != null)
            {
                mSpecies = mNewSim.Species;
            }

            if (HouseholdsEx.IsFull(this, Sim.Household, mSpecies, 0, true, true))
            {
                IncStat("Full");
                return false;
            }

            bool created = false;
            if (mNewSim == null)
            {
                using (SimFromBin<ManagerLot> simBin = new SimFromBin<ManagerLot>(this, Lots))
                {
                    CASAgeGenderFlags gender = BabyGenderScenario.GetGenderByFirstBorn(Manager, GetValue<ImmigrateScenario.GenderOption, BabyGenderScenario.FirstBornGender>(), false);

                    mNewSim = simBin.CreateNewSim(Ages, gender, mSpecies);
                    created = true;
                }
            }

            if (mNewSim != null)
            {
                if (Households.MoveSim(mNewSim, Sim.Household))
                {
                    mNewSim.LastName = Sim.LastName;
                    mNewSim.WasAdopted = true;

                    Relationship relation = ManagerSim.GetRelationship(Sim, mNewSim);
                    if (relation != null)
                    {
                        relation.LTR.SetLiking(Delta);
                    }

                    Relationships.CheckAddHumanParentFlagOnAdoption(Sim, mNewSim);

                    if (Sim.Partner != null)
                    {
                        Relationships.CheckAddHumanParentFlagOnAdoption(Sim.Partner, mNewSim);
                    }

                    if (SimTypes.IsEquivalentSpecies(mNewSim, Sim))
                    {
                        Sim.Genealogy.AddChild(mNewSim.Genealogy);
                    }

                    UpdateDayOfLastOption(Sim);

                    if ((Sim.Partner != null) && (Sim.IsMarried))
                    {
                        if (SimTypes.IsEquivalentSpecies(mNewSim, Sim))
                        {
                            Sim.Partner.Genealogy.AddChild(mNewSim.Genealogy);
                        }

                        UpdateDayOfLastOption(Sim.Partner);
                    }
                    return true;
                }
                else if (created)
                {
                    mNewSim.Dispose();
                    mNewSim = null;
                }
            }

            IncStat("No New Sim");
            return false;
        }

        protected override bool Push()
        {
            if (mNewSim != null)
            {
                Situations.PushGoHome(this, mNewSim);
            }

            return base.Push();
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mNewSim == null) return null;

            if (parameters == null)
            {
                parameters = new object[] { Sim, mNewSim };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
