using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class SimEventScenario<TEvent> : SimScenario, IEventScenario
        where TEvent : Event
    {
        TEvent mEvent;

        bool mWasSecondCycle = false;

        protected SimEventScenario()
        { }
        protected SimEventScenario(SimDescription sim)
            : base (sim)
        { }
        protected SimEventScenario(SimEventScenario<TEvent> scenario)
            : base (scenario)
        {
            mEvent = scenario.mEvent;
            mWasSecondCycle = scenario.mWasSecondCycle;
        }

        public TEvent Event
        {
            get { return mEvent; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool ShouldReport
        {
            get { return mWasSecondCycle; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool Matches(Scenario scenario)
        {
            return false;
        }

        public abstract bool SetupListener(IEventHandler events);

        protected virtual bool Allow(TEvent e)
        {
            return true;
        }

        protected override bool Allow()
        {
            if (mEvent != null)
            {
                if (!Allow(mEvent)) return false;
            }

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected static SimDescription GetEventSim(Event e)
        {
            SimDescriptionEvent simDescEvent = e as SimDescriptionEvent;
            if (simDescEvent != null)
            {
                return simDescEvent.SimDescription;
            }
            else
            {
                MiniSimDescriptionEvent miniSimEvent = e as MiniSimDescriptionEvent;
                if (miniSimEvent != null)
                {
                    return miniSimEvent.MiniSimDescription as SimDescription;
                }
                else
                {
                    Sim sim = e.Actor as Sim;
                    if (sim == null)
                    {
                        sim = e.TargetObject as Sim;
                        if (sim == null) return null;
                    }

                    return sim.SimDescription;
                }
            }
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            mWasSecondCycle = Main.SecondCycle;

            mEvent = e as TEvent;
            if (mEvent == null)
            {
                IncStat("Event Mismatch");
                return null;
            }

            Sim = GetEventSim(e);
            if (Sim == null)
            {
                IncStat("No Event Sim");
                return null;
            }

            return Clone();
        }
    }
}
