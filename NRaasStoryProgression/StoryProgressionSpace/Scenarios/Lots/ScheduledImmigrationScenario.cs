using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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
    public class ScheduledImmigrationScenario : ScheduledSoloScenario
    {
        public ScheduledImmigrationScenario()
        { }
        protected ScheduledImmigrationScenario(ScheduledImmigrationScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Immigration";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (GetValue<GaugeOption,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new ImmigrantNoHomeScenario(), ScenarioResult.Start);

            ManagerLot.ImmigrationRequirement requirement = new ManagerLot.ImmigrationRequirement();

            Add(frame, new ImmigrantPressureScenario(requirement, true), ScenarioResult.Failure);
            Add(frame, new ImmigrateScenario(requirement, true), ScenarioResult.Success);
            return false;
        }

        public override Scenario Clone()
        {
            return new ScheduledImmigrationScenario(this);
        }

        public class GaugeOption : IntegerScenarioOptionItem<ManagerLot, ScheduledImmigrationScenario>, IAdjustForVacationOption, ManagerLot.IImmigrationEmigrationOption
        {
            public GaugeOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrationGauge";
            }

            public bool AdjustForVacationTown()
            {
                SetValue (50);
                return true;
            }

            protected override string GetPrompt()
            {
                return Localize("Prompt", new object[] { Manager.PreviousPressure });
            }

            public override bool Install(ManagerLot main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                SimFromBin<ManagerLot>.Install(new Controller(Manager), main, initial);
                return true;
            }
        }

        public class Controller : SimFromBinController
        {
            public Controller(Manager manager)
                : base(manager)
            { }

            public override bool ShouldDisplayImmigrantOptions()
            {
                return (Manager.GetValue<GaugeOption, int>() > 0);
            }
        }
    }
}
