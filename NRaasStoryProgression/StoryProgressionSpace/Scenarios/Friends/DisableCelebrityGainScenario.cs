using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class DisableCelebrityGainScenario : SimEventScenario<Event>
    {
        public DisableCelebrityGainScenario()
        { }
        protected DisableCelebrityGainScenario(DisableCelebrityGainScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "DisableCelebrityGain";
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
            return events.AddListener(this, EventTypeId.kSimInstantiated);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CelebrityManager == null)
            {
                IncStat("No Manager");
                return false;
            }

            return base.Allow(sim);
        }

        public static bool Perform(SimDescription sim)
        {
            if (sim.CelebrityManager.mCelebrityActivityEventListener != null)
            {
                EventTracker.RemoveListener(sim.CelebrityManager.mCelebrityActivityEventListener);
                sim.CelebrityManager.mCelebrityActivityEventListener = null;
                return true;
            }

            return false;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Perform(Sim))
            {
                IncStat("Removed Disgrace Listener");
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new DisableCelebrityGainScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerFriendship, DisableCelebrityGainScenario>, ManagerFriendship.ICelebrityOption, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "DisableCelebrityGain";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            public override bool Install(ManagerFriendship main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim == null) continue;

                    if (DisableCelebrityGainScenario.Perform(sim.SimDescription))
                    {
                        Manager.IncStat("Removed Celebrity Gain Listener");
                    }
                }

                return true;
            }
        }
    }
}
