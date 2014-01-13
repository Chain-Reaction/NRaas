using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class SimDisposedScenario : SimScenario, IAlarmScenario
    {
        public SimDisposedScenario(SimDescription sim)
            : base (sim)
        { }
        protected SimDisposedScenario(SimDisposedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SimDisposed";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarm(this, 2);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Main.RemoveSim(Sim.SimDescriptionId);
            return true;
        }

        public override Scenario Clone()
        {
            return new SimDisposedScenario(this);
        }

        public class Task : Manager.Task<ManagerDeath>
        {
            SimDescription mSim;

            protected Task(ManagerDeath manager, SimDescription sim)
                : base(manager)
            {
                mSim = sim;
            }

            public static void Perform(ManagerDeath manager, SimDescription sim)
            {
                new Task(manager, sim).AddToSimulator();
            }

            protected override void OnPerform()
            {
                Manager.AddAlarm(new SimDisposedScenario(mSim));
            }
        }
    }
}
