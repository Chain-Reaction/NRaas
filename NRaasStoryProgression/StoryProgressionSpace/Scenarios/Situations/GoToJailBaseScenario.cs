using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public abstract class GoToJailBaseScenario : SimScenario, IAlarmScenario
    {
        public GoToJailBaseScenario()
        { }
        public GoToJailBaseScenario(SimDescription sim)
            : base(sim)
        { }
        protected GoToJailBaseScenario(GoToJailBaseScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 60; }
        }

        protected override int MaximumReschedules
        {
            get { return 12; }
        }

        protected abstract string StoryName
        {
            get;
        }

        protected abstract int Chance
        {
            get;
        }

        protected virtual int Bail
        {
            get { return Manager.GetValue<GotArrestedScenario.BailOption, int>(); }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarm(this, 1);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        public delegate InteractionDefinition PushInteraction();

        public static event PushInteraction OnPushInteraction;

        public static bool HasPushInteraction()
        {
            return (OnPushInteraction != null);
        }

        protected override bool Allow()
        {
            if (OnPushInteraction == null)
            {
                IncStat("Not Installed");
                return false;
            }

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (RandomUtil.RandomChance(Chance))
            {
                GotArrestedScenario.AddToRepository(Sim, StoryName, Bail);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool Push()
        {
            if (Sim.CreatedSim == null) return false;

            if (!Situations.Allow(this, Sim)) return false;

            if (Sim.CreatedSim.InteractionQueue != null)
            {
                Sim.CreatedSim.InteractionQueue.CancelAllInteractions();
            }

            PoliceStation target = ManagerSituation.FindRabbitHole(RabbitHoleType.PoliceStation) as PoliceStation;
            if (target == null) return false;
            
            if (OnPushInteraction == null) return false;

            InteractionDefinition pushInteraction = OnPushInteraction();
            if (pushInteraction == null) return false;

            InteractionInstance entry = pushInteraction.CreateInstance(target, Sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.High), false, false);
            Sim.CreatedSim.AddExitReason(ExitReason.CanceledByScript);
            Sim.CreatedSim.InteractionQueue.AddNext(entry);

            return true;
        }
    }
}
