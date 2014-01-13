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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class RapidImmigrationScenario : ScheduledSoloScenario
    {
        public RapidImmigrationScenario()
        { }
        protected RapidImmigrationScenario(RapidImmigrationScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RapidImmigration";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            return true;
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            GetOption<Option>().SetValue(GetValue<Option,int>() - 1);

            Add(frame, new ImmigrantNoHomeScenario(), ScenarioResult.Start);

            ManagerLot.ImmigrationRequirement requirement = new ManagerLot.ImmigrationRequirement();
            Add(frame, new ImmigrantPressureScenario(requirement, false), ScenarioResult.Start);
            Add(frame, new ImmigrateScenario(requirement, false), ScenarioResult.Start);
            return true;
        }

        public override Scenario Clone()
        {
            return new RapidImmigrationScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerLot, RapidImmigrationScenario>, ManagerLot.IImmigrationEmigrationOption
        {
            public Option()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "RapidImmigration";
            }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                if (Value <= 0) return;

                if (!ShouldDisplay()) return;

                base.PrivateUpdate(fullUpdate, initialPass);
            }

            protected override string GetPrompt()
            {
                return Localize("Prompt", new object[] { Manager.PreviousPressure });
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
