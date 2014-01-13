using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
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
    public class ScheduledExistingFlirtScenario : FlirtScenario
    {
        public ScheduledExistingFlirtScenario()
            : base (10)
        { }
        protected ScheduledExistingFlirtScenario(ScheduledExistingFlirtScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ScheduledExistingFlirt";
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override int ContinueChance
        {
            get { return 5; }
        }

        protected override int PregnancyChance
        {
            get { return -1; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool TestFlirtCooldown
        {
            get { return true; }
        }

        protected override ManagerRomance.AffairStory AffairStory
        {
            get { return ManagerRomance.AffairStory.All; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Flirts.FlirtPool;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Flirts.FindExistingFor(this, sim, true);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new PartneringScenario(Sim, Target, true, false), ScenarioResult.Start);

            return base.PrivateUpdate(frame);
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Residential, ManagerFlirt.FirstAction);
        }

        public override Scenario Clone()
        {
            return new ScheduledExistingFlirtScenario(this);
        }

        public class AllowExistingFlirtsOption : BooleanScenarioOptionItem<ManagerFlirt, ScheduledExistingFlirtScenario>
        {
            public AllowExistingFlirtsOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowExistingFlirts";
            }
        }
    }
}
