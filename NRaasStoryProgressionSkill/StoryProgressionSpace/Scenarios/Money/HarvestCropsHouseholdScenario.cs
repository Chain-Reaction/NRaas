using Sims3.Gameplay.CAS;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class HarvestCropsHouseholdScenario : HouseholdScenario
    {
        public HarvestCropsHouseholdScenario()
            : base()
        { }
        protected HarvestCropsHouseholdScenario(HarvestCropsHouseholdScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HarvestHouseholdCrops";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 240; }
        }

        protected override bool Allow()
        {
            if (!GetValue<HarvestCropsSimScenario.Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new HarvestCropsSimScenario(House), ScenarioResult.Start);
            return false;
        }

        protected override GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected override List<Household> GetHouses()
        {
            return Household.GetHouseholdsLivingInWorld();
        }

        protected override bool Allow(Household house)
        {
            if (!base.Allow(house)) return false;

            if (house == Household.ActiveHousehold) return false;

            if (house.LotHome == null) return false;

            return true;
        }

        public override Scenario Clone()
        {
            return new HarvestCropsHouseholdScenario(this);
        }
    }
}
