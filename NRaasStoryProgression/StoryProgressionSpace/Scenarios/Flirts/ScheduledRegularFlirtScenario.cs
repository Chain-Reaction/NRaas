using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Flirts
{
    public class ScheduledRegularFlirtScenario : RegularFlirtBaseScenario
    {
        public ScheduledRegularFlirtScenario()
            : base(10)
        { }
        protected ScheduledRegularFlirtScenario(ScheduledRegularFlirtScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ScheduledRegularFlirt";
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Flirts.FindAnyFor(this, sim, true, Force);
        }

        protected override GatherResult TargetGather(List<Scenario> list, ref bool random)
        {
            GatherResult result = base.TargetGather(list, ref random);
            if ((Target == null) && (result == GatherResult.Failure))
            {
                if (SimTypes.IsSpecial (Sim))
                {
                    IncStat("Special Failure");
                }
                else if (!Sim.YoungAdultOrAbove)
                {
                    IncStat("Teen Love-Loss");
                }
                else if (Sim.Elder)
                {
                    IncStat("Elder Love-Loss");
                }
                else if (Sim.Partner != null)
                {
                    IncStat("Partnered Love-Loss");
                }
                else
                {
                    list.Add(new CannotFindLoveScenario(Sim));
                }
            }

            return result;
        }

        public override Scenario Clone()
        {
            return new ScheduledRegularFlirtScenario(this);
        }
    }
}
