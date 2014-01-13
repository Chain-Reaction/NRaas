using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class GoHomePushScenario : SimScenario, IAlarmScenario
    {
        public GoHomePushScenario(SimDescription sim)
            : base(sim)
        { }
        protected GoHomePushScenario(GoHomePushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "GoHomePush";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarm(this, 1);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim.InteractionQueue == null)
            {
                IncStat("No Queue");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool bWorkFound = false;
            foreach (InteractionInstance instance in Sim.CreatedSim.InteractionQueue.InteractionList)
            {
                if (instance is ICountsAsWorking)
                {
                    bWorkFound = true;
                }
                else if (bWorkFound)
                {
                    if (instance.GetPriority().Level != InteractionPriorityLevel.UserDirected) continue;

                    IncStat("Denied: " + instance.GetType().Name);
                    return false;
                }
            }

            if (Sim.CreatedSim.LotCurrent == Sim.LotHome)
            {
                Manager.AddAlarm(new GoHomePushScenario(Sim));
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override bool Push()
        {
            return Situations.PushGoHome(this, Sim);
        }

        public override Scenario Clone()
        {
            return new GoHomePushScenario(this);
        }
    }
}
