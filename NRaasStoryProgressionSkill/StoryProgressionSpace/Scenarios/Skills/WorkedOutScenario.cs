using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class WorkedOutScenario : SimEventScenario<TimePassedEvent>
    {
        static Dictionary<ReferenceWrapper, float> sTimedPassed = new Dictionary<ReferenceWrapper, float>();

        public WorkedOutScenario()
        { }
        protected WorkedOutScenario(WorkedOutScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "WorkedOut";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Task; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kWorkedOut);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CreatedSim.Autonomy == null)
            {
                IncStat("No Autonomy");
                return false;
            }

            return base.Allow(sim);
        }

        protected static bool SetWorkoutTone(SimDescription sim, AthleticGameObject.WorkOut interaction)
        {
            List<InteractionToneDisplay> tones = interaction.AvailableTonesForDisplay();
            if ((tones == null) || (tones.Count == 0)) return false;

            interaction.CurrentITone = RandomUtil.GetRandomObjectFromList(tones).InteractionTone;
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            try
            {
                float increment = Event.mIncrement * 60;

                AthleticGameObject.WorkOut interaction = Sim.CreatedSim.CurrentInteraction as AthleticGameObject.WorkOut;
                if (interaction != null) 
                {
                    if (interaction.CurrentITone == null)
                    {
                        if (SetWorkoutTone(Sim, interaction))
                        {
                            IncStat("Tone " + interaction.CurrentITone.Name());
                        }
                    }

                    ReferenceWrapper reference = new ReferenceWrapper(interaction);

                    float value = 0;
                    if (sTimedPassed.TryGetValue(reference, out value))
                    {
                        if (value > 180)
                        {
                            Sim.CreatedSim.InteractionQueue.CancelInteraction(Sim.CreatedSim.CurrentInteraction, true);

                            IncStat("Auto Cancelled");
                        }
                    }

                    sTimedPassed[reference] = value + increment;
                }

                if (!Sim.CreatedSim.Autonomy.IsRunningHighLODSimulation)
                {
                    Sim.UpdateBodyShape(increment, Sim.CreatedSim.ObjectId);
                }

                return true;
            }
            catch (Exception e)
            {
                IncStat("Exception Fail");

                Common.DebugException(Sim, e);
                return false;
            }
        }

        public override Scenario Clone()
        {
            return new WorkedOutScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSkill, WorkedOutScenario>, IDebuggingOption
        {
            public Option()
                :base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "WorkedOut";
            }
        }
    }
}
