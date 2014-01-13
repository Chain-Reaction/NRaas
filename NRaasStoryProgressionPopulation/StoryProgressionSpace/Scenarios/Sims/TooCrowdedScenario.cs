using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class TooCrowdedScenario : EmigrateScenario
    {
        public TooCrowdedScenario()
        { }
        protected TooCrowdedScenario(TooCrowdedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "TooCrowded";
        }

        protected override bool Allow()
        {
            if (GetValue<ThresholdOption,int>() <= 0) return false;

            if (!Sims.HasEnough(this, CASAgeGenderFlags.Human))
            {
                IncStat("Not Full");
                return false;
            }

            return base.Allow();
        }

        protected int GetTotalFamily(Household house)
        {
            int total = 0;
            foreach (Household other in Household.GetHouseholdsLivingInWorld())
            {
                if (other == house) continue;

                foreach (SimDescription otherSim in HouseholdsEx.All(other))
                {
                    bool found = false;
                    foreach (SimDescription sim in HouseholdsEx.All(house))
                    {
                        if (Relationships.IsCloselyRelated(sim, otherSim, false))
                        {
                            total++;
                            found = true;
                            //break;
                        }
                    }
                    if (found)
                    {
                        if (other == Household.ActiveHousehold)
                        {
                            return int.MaxValue;
                        }
                        break;
                    }
                }
            }
            
            return total;
        }

        protected override bool Allow(Household house)
        {
            if (!base.Allow(house)) return false;

            int totalFamily = GetTotalFamily(House);
            if (totalFamily >= GetValue<ThresholdOption, int>())
            {
                AddStat("Enough Family", totalFamily);
                return false;
            }

            bool deadParent = false;
            foreach (SimDescription sim in HouseholdsEx.All(house))
            {
                foreach (SimDescription parent in Relationships.GetParents(sim))
                {
                    if (parent.IsDead)
                    {
                        deadParent = true;
                        break;
                    }
                }
            }

            if (!deadParent)
            {
                IncStat("No Ancestors");
                return false;
            }


            return true;
        }

        public override Scenario Clone()
        {
            return new TooCrowdedScenario(this);
        }

        public class ThresholdOption : IntegerScenarioOptionItem<ManagerSim, TooCrowdedScenario>
        {
            public ThresholdOption()
                : base(1)
            { }

            public override string GetTitlePrefix()
            {
                return "TooCrowdedThreshold";
            }

            public override int Value
            {
                get
                {
                    if (!ShouldDisplay()) return 0;

                    return base.Value;
                }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<EmigrationScenario.Option, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
