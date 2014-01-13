using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Flirts
{
    public abstract class FlirtBaseScenario : DualSimScenario
    {
        bool mReport = true;

        string mStoryPrefix;

        public FlirtBaseScenario(SimDescription sim, SimDescription target, string storyPrefix, bool report)
            : base (sim, target)
        {
            mReport = report;
            mStoryPrefix = storyPrefix;
        }
        protected FlirtBaseScenario(FlirtBaseScenario scenario)
            : base (scenario)
        {
            mReport = scenario.mReport;
            mStoryPrefix = scenario.mStoryPrefix;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return mStoryPrefix;
        }

        protected override bool ShouldReport
        {
            get { return mReport; }
        }

        protected override int PushChance
        {
            get
            {
                if (!mReport) return 0;

                return base.PushChance;
            }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledFlirtScenario.AllowFlirtsOption,bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Flirts.FlirtPool;
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Commercial, ManagerFlirt.FirstAction);
        }
    }
}
