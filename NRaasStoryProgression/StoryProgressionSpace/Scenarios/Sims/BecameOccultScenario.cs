using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
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
using System;
using System.Collections.Generic;
using System.Text;
using Sims3.UI.Hud;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class BecameOccultScenario : SimEventScenario<Event>
    {
        public BecameOccultScenario()
        { }
        protected BecameOccultScenario(BecameOccultScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "BecameOccult";
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
            return events.AddListener(this, EventTypeId.kBecameOccult);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.TeenOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            List<OccultTypes> occults = OccultTypeHelper.CreateList(Sim);
            if (occults.Count == 0) return null;

            OccultTypes occult = occults[occults.Count-1];

            switch(occult)
            {
                case OccultTypes.Vampire:
                case OccultTypes.Werewolf:
                case OccultTypes.Fairy:
                case OccultTypes.Witch:
                case OccultTypes.Genie:
                //case OccultTypes.Mummy:
                    name = "Became" + occult;
                    break;
                default:
                    if (!Manager.DebuggingEnabled)
                    {
                        return null;
                    }
                    break;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new BecameOccultScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim, BecameOccultScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "BecameOccult";
            }
        }
    }
}
