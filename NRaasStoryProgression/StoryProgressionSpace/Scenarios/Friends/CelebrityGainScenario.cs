using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Interfaces;
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
    public class CelebrityGainScenario : SimEventScenario<CelebrityActivityEvent>, IFormattedStoryScenario
    {
        public CelebrityGainScenario()
        { }
        protected CelebrityGainScenario(CelebrityGainScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CelebrityGainEvent";
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Friends;
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Task; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimPerformedCelebrityActivity);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Friends.AllowCelebrity(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.CelebrityManager == null)
            {
                IncStat("No Manager");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if ((SimTypes.IsSelectable(Sim)) && (Sim.CreatedSim != null) && (Sim.CreatedSim.Conversation != null))
            {
                bool valid = false;

                Conversation conversation = Sim.CreatedSim.Conversation;
                foreach (Sim sim in conversation.Members)
                {
                    if (sim.CelebrityManager.Level > Sim.CelebrityLevel)
                    {
                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    IncStat("Lower Celebrity Fail");
                    return false;
                }
            }

            // Delayed to split the stack
            Main.Scenarios.Post(new DelayedScenario(Sim, Event), Manager, false);
            return false;
        }

        public class DelayedScenario : SimScenario
        {
            CelebrityActivityEvent mEvent;

            uint mOldLevel;

            public DelayedScenario(SimDescription sim, CelebrityActivityEvent e)
                : base(sim)
            {
                mEvent = e;
                mOldLevel = sim.CelebrityLevel;
            }
            public DelayedScenario(DelayedScenario scenario)
                : base(scenario)
            {
                mEvent = scenario.mEvent;
                mOldLevel = scenario.mOldLevel;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Story) return null;

                return "CelebrityGain";
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

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (sim.CelebrityLevel >= CelebrityManager.HighestLevel)
                {
                    IncStat("Max Level");
                    return false;
                }

                return base.Allow(sim);
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                mOldLevel = Sim.CelebrityLevel;

                AddStat("Old Level", mOldLevel);

                Sim.CelebrityManager.OnSimPerformedCelebrityActivity(mEvent);

                AddStat("New Level", Sim.CelebrityLevel);

                return true;
            }

            protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (SimTypes.IsSelectable(Sim)) return null;

                if (mOldLevel >= Sim.CelebrityLevel) return null;

                if ((Sim.CelebrityLevel <= 1) && (!GetValue<ShowFirstLevelOption, bool>()))
                {
                    IncStat("First Level Denied");
                    return null;
                }

                CelebrityManager celebrityManager = Sim.CelebrityManager;

                CelebrityLevelStaticData currentLevelStaticData = celebrityManager.GetCurrentLevelStaticData();
                if (currentLevelStaticData != null)
                {
                    string levelUpTnsLocalizationKey = currentLevelStaticData.LevelUpTnsLocalizationKey;
                    if (levelUpTnsLocalizationKey != null)
                    {
                        text = CelebrityManager.LocalizeSpreadsheetString(Sim.IsFemale, levelUpTnsLocalizationKey, new object[] { celebrityManager.LevelName, Sim });
                    }
                }

                if (extended == null)
                {
                    extended = new string[] { EAText.GetNumberString(Sim.CelebrityLevel) };
                }

                return base.PrintFormattedStory(manager, text, "0x635e705292bc4b00", parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new DelayedScenario(this);
            }
        }

        public override Scenario Clone()
        {
            return new CelebrityGainScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerFriendship, CelebrityGainScenario>, ManagerFriendship.ICelebrityOption, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityGainEvent";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }
        }

        public class ShowFirstLevelOption : BooleanManagerOptionItem<ManagerFriendship>, ManagerFriendship.ICelebrityOption
        {
            public ShowFirstLevelOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowFirstCelebrityLevel";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }
        }
    }
}
