using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class TaskCompletedScenario : SimEventScenario<OccupationTaskEvent>
    {
        public TaskCompletedScenario()
        { }
        protected TaskCompletedScenario(TaskCompletedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "TaskCompleted";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimCompletedOccupationTask);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!ManagerCareer.ValidCareer(sim.Occupation))
            {
                IncStat("No Job");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            float xp;
            int cash;
            if (Sim.Occupation.TryGetXpAndCash(Event.TaskId, out xp, out cash))
            {
                switch (Event.TaskId)
                {
                    case TaskId.TrappedSims:
                    case TaskId.Fires:
                    case TaskId.IsolatedFires:
                    case TaskId.Everyday:
                        Sim.Occupation.UpdatePerformanceOrExperience(xp * 9);

                        xp *= 10;
                        break;
                }
            }

            AddStat("Task XP", xp);

            try
            {
                Sim.Occupation.OnSimCompletedOccupationTask(Event);
            }
            catch (Exception e)
            {
                Common.DebugException(Sim, e);
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new TaskCompletedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, TaskCompletedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "TaskCompleted";
            }
        }
    }
}
