using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
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
    public class ScheduledFlirtScenario : ScheduledSoloScenario
    {
        public ScheduledFlirtScenario()
        { }
        protected ScheduledFlirtScenario(ScheduledFlirtScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ScheduledFlirt";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override bool Allow()
        {
            if (!GetValue<AllowFlirtsOption,bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new ScheduledRegularFlirtScenario(), ScenarioResult.Start);
            Add(frame, new ScheduledFailSafeFlirtScenario(), ScenarioResult.Failure);
            return true;
        }

        public override Scenario Clone()
        {
            return new ScheduledFlirtScenario(this);
        }

        public class AllowFlirtsOption : BooleanScenarioOptionItem<ManagerFlirt, ScheduledFlirtScenario>
        {
            public AllowFlirtsOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowFlirts";
            }
        }

        public class MaximumFlirtsOption : IntegerManagerOptionItem<ManagerFlirt>
        {
            public MaximumFlirtsOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "MaximumAllowedPerSim";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<AllowFlirtsOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
