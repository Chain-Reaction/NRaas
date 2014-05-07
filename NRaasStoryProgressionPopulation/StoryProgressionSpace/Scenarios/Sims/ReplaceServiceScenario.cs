using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class ReplaceServiceScenario : SimEventScenario<Event>
    {
        public ReplaceServiceScenario()
        { }
        protected ReplaceServiceScenario(ReplaceServiceScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ReplaceService";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 25; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return HouseholdsEx.All(Household.NpcHousehold);
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimInstantiated);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }        

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            // Delayed to allow for Role Manager to assign a role
            Add(frame, new DelayedScenario(Sim), ScenarioResult.Start);
            return true;
        }

        public override Scenario Clone()
        {
            return new ReplaceServiceScenario(this);
        }

        public class DelayedScenario : ReplaceSimScenario
        {
            public DelayedScenario(SimDescription sim)
                : base(sim)
            { }
            protected DelayedScenario(DelayedScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "ReplaceServiceDelayed";
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected static bool IsValidSim(SimDescription sim)
            {
                if ((SimTypes.InServicePool(sim)) || (sim.AssignedRole != null))
                {
                    return true;
                }                                            

                return false;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (sim.LotHome != null)
                {
                    IncStat("Resident");
                    return false;
                }  
                if (!IsValidSim(sim))
                {
                    IncStat("Invalid");
                    return false;
                }             
                else if (GetValue<ReplacedServiceOption, bool>(sim))
                {
                    IncStat("Already Replaced");
                    return false;
                }

                return base.Allow(sim);
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                if (base.PrivateUpdate(frame))
                {
                    SetValue<ReplacedServiceOption, bool>(Sim, true);
                    return true;
                }

                return false;
            }

            public override Scenario Clone()
            {
                return new DelayedScenario(this);
            }
        }        

        public class Option : BooleanScenarioOptionItem<ManagerSim, ReplaceServiceScenario>, ManagerSim.IImmigrationEmigrationOption
        {
            public Option()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ReplaceService";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool Install(ManagerSim main, bool initial)
            {
                if (!base.Install(main, initial)) return false;            
                
                return true;
            }
        }        

        public class EventOption : BooleanEventOptionItem<ManagerSim, ReplaceServiceScenario>, ManagerSim.IImmigrationEmigrationOption, IDebuggingOption
        {
            public EventOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ReplaceServiceEvent";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class ReplacedServiceOption : SimBooleanOption, IDebuggingOption, INotCasteLevelOption
        {
            public ReplacedServiceOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ReplacedService";
            }
        }        
    }
}
