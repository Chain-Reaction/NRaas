using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class SimRecruitFilter
    {
        SimScenarioFilter mActorRecruit = null;
        SimScenarioFilter mTargetRecruit = null;

        bool mAllowSteal = false;

        public SimRecruitFilter()
        { }

        public override string ToString()
        {
            return "-- SimRecruitFilter --" + Common.NewLine + "Actor" + Common.NewLine + mActorRecruit + Common.NewLine + "Target" + Common.NewLine + mTargetRecruit + Common.NewLine + "Steal=" + mAllowSteal + Common.NewLine + "-- End SimRecruitFilter --";
        }

        public void Perform(Scenario scenario, SimDescription actor, SimDescription target)
        {
            ICollection<SimDescription> recruits = mTargetRecruit.Filter(new SimScenarioFilter.Parameters(scenario, mActorRecruit.Enabled), "TargetRecruit", target);

            recruits = mActorRecruit.Filter(new SimScenarioFilter.Parameters(scenario, false), "ActorRecruit", actor, recruits);
            if (recruits == null) return;

            SimPersonality clan = scenario.Manager as SimPersonality;

            foreach (SimDescription recruit in recruits)
            {
                clan.AddToClan(scenario, recruit, mAllowSteal);
            }
        }

        public bool Parse(XmlDbRow row, StoryProgressionObject manager, IUpdateManager updater, string prefix, ref string error)
        {
            if ((!string.IsNullOrEmpty(prefix)) && (!Parse(row, manager, updater, null, ref error)))
            {
                return false;
            }

            if (row.Exists(prefix + "RecruitAllowSteal"))
            {
                mAllowSteal = row.GetBool(prefix + "RecruitAllowSteal");
            }

            if (mActorRecruit == null)
            {
                mActorRecruit = new SimScenarioFilter();
            }

            if (!mActorRecruit.Parse(row, manager, updater, prefix + "RecruitActor", false, ref error))
            {
                return false;
            }

            if (mTargetRecruit == null)
            {
                mTargetRecruit = new SimScenarioFilter();
            }

            if (!mTargetRecruit.Parse(row, manager, updater, prefix + "RecruitTarget", false, ref error))
            {
                return false;
            }

            return true;
        }
    }
}
