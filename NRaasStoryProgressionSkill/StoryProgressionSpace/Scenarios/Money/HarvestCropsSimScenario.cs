using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace.CAS;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class HarvestCropsSimScenario : HarvestScenario
    {
        readonly Household mHouse;

        List<HarvestPlant> mPlants = null;

        public HarvestCropsSimScenario(Household house)
            : base()
        {
            mHouse = house;
        }
        protected HarvestCropsSimScenario(HarvestCropsSimScenario scenario)
            : base (scenario)
        {
            mHouse = scenario.mHouse;
            mPlants = scenario.mPlants;
        }

        protected override int ContinueChance
        {
            get { return 50; }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "HarvestCrops";
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int InitialReportChance
        {
            get { return 25; }
        }

        protected override int ContinueReportChance
        {
            get { return 10; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if ((mPlants == null) || (mPlants.Count == 0))
            {
                IncStat("No Plants");
                return false;
            }
            else if (AddScoring("Gardening", Sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }
            else if (sim.CreatedSim.LotCurrent != sim.LotHome)
            {
                IncStat("Not Home");
                return false;
            }
            else if ((sim.IsEP11Bot) && (!sim.HasTrait(TraitNames.GardenerChip)))
            {
                IncStat("Chip Denied");
                return false;
            }

            return true;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            List<SimDescription> allSims = ManagerSim.Matching(HouseholdsEx.Humans(mHouse), CASAgeGenderFlags.Teen | CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder);
            List<SimDescription> sims = new List<SimDescription>();

            foreach(SimDescription sim in allSims)
            {
                if (!ManagerCareer.HasSkillCareer(sim, SkillNames.Gardening)) continue;

                sims.Add(sim);
            }

            if (sims.Count == 0)
            {
                return allSims;
            }
            else
            {
                return sims;
            }
        }

        protected override GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            if (mHouse.Sims.Count == 0) return GatherResult.Failure;

            if (mPlants == null)
            {
                mPlants = new List<HarvestPlant>();

                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot.CanSimTreatAsHome(mHouse.Sims[0]))
                    {
                        mPlants.AddRange(lot.GetObjects<HarvestPlant>());
                    }
                }

                if ((mPlants == null) || (mPlants.Count <= 0))
                {
                    IncStat("No Plants");
                    return GatherResult.Failure;
                }

                RandomUtil.RandomizeListOfObjects(mPlants);
            }

            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected override List<HarvestPlant> GatherPlants()
        {
            return mPlants;
        }

        public override Scenario Clone()
        {
            return new HarvestCropsSimScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, HarvestCropsHouseholdScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HarvestCrops";
            }
        }
    }
}
