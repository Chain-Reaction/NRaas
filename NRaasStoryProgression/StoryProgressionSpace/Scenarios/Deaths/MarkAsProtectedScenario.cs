using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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
    public class MarkAsProtectedScenario : SimEventScenario<Event>
    {
        public MarkAsProtectedScenario()
        { }
        protected MarkAsProtectedScenario(MarkAsProtectedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "MarkAsProtected";
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
            return events.AddListener(this, EventTypeId.kSimInstantiated);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            return fullUpdate;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            MiniSimDescription miniSim = Sim.GetMiniSimForProtection();
            if (miniSim != null)
            {
                miniSim.AddProtection(MiniSimDescription.ProtectionFlag.PartialFromPlayer);
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new MarkAsProtectedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerDeath, MarkAsProtectedScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "MarkedasEAProtected";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                new MarkAsProtectedScenario().Post(Manager, fullUpdate, initialPass);
            }

            protected override bool PrivatePerform()
            {
                if (Value)
                {
                    SimpleMessageDialog.Show(Name, Localize("Warning"));
                }
                else if (!AcceptCancelDialog.Show(Localize("Prompt")))
                {
                    return false;
                }

                if (!base.PrivatePerform()) return false;

                new MarkAsProtectedScenario().Post(Manager, true, false);
                return true;
            }
        }
    }
}
