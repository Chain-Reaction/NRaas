using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class HomelessMergeScenario : HouseholdScenario
    {
        public HomelessMergeScenario(Household house)
            : base (house)
        { }
        protected HomelessMergeScenario(HomelessMergeScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HomelessMerge";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>())
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (house.LotHome != null)
            {
                IncStat("Resident");
                return false;
            }
            else if (Sims.HasEnough(house.SimDescriptions))
            {
                IncStat("Town Full");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<Household> homeless = new List<Household>();
            foreach (Household house in Household.sHouseholdList)
            {
                if (house.LotHome != null) continue;

                if (house == House) continue;

                homeless.Add(house);
            }

            if (homeless.Count == 0) return false;

            int count = 0;
            do
            {
                count++;

                int total = RandomUtil.GetInt(2, 4);

                Dictionary<Household, bool> homes = new Dictionary<Household, bool>();
                homes.Add(House, true);

                List<SimDescription> sims = new List<SimDescription>();

                int humanCount = 0, petCount = 0;
                for (int i = 0; i < total; i++)
                {
                    Household home = RandomUtil.GetRandomObjectFromList<Household>(homeless);

                    if (homes.ContainsKey(home)) continue;

                    if (HouseholdsEx.IsFull(this, home, CASAgeGenderFlags.Human, humanCount, false)) continue;
                    if (HouseholdsEx.IsFull(this, home, CASAgeGenderFlags.Dog, petCount, false)) continue;

                    HouseholdsEx.NumSimsIncludingPregnancy(home, ref humanCount, ref petCount);

                    homes.Add(home, true);

                    sims.AddRange(home.AllSimDescriptions);
                }

                Add(frame, new HomelessMoveInLotScenario(sims), ScenarioResult.Failure);
            }
            while (count < 10);

            return false;
        }

        public override Scenario Clone()
        {
            return new HomelessMergeScenario(this);
        }
    }
}
