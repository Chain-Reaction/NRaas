using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
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
    public class WentToWorkScenario : CareerEventScenario
    {
        public WentToWorkScenario()
        { }
        protected WentToWorkScenario(WentToWorkScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "WentToWork";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kWentToWork);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            WentToWorkScenario scenario = base.Handle(e, ref result) as WentToWorkScenario;
            if (scenario != null)
            {
                // Do this now, rather than waiting the 10 sim-minute Scenario manager delay
                Careers.VerifyTone(scenario.Sim);
            }

            return scenario;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Sim.CreatedSim != null)
            {
                // Clear Missing School/Work punishments if a sim goes back to school
                Punishment manager = Punishment.GetPunishmentManagerFromSim(Sim.CreatedSim);
                if (manager != null)
                {
                    if (manager.HasUnpunishedDeviantBehaviors())
                    {
                        bool valid = false;
                        foreach(Punishment.DeviantBehaviorInstance instance in manager.mUnpunishedDeviantBehaviors)
                        {
                            switch(instance.Type)
                            {
                                case Punishment.DeviantBehaviorType.MissingSchool:
                                case Punishment.DeviantBehaviorType.MissingWork:
                                    break;
                                default:
                                    valid = true;
                                    break;
                            }

                            if (valid) break;
                        }

                        if (!valid)
                        {
                            manager.mUnpunishedDeviantBehaviors = null;
                        }
                    }
                }
            }

            Add(frame, new ActiveCareerToneScenario(Sim), ScenarioResult.Start);
            Add(frame, new InactiveCareerToneScenario(Sim), ScenarioResult.Failure);

            return Careers.ResetBoss(this, Event.Career);
        }

        public override Scenario Clone()
        {
            return new WentToWorkScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, WentToWorkScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "WentToWork";
            }
        }
    }
}
