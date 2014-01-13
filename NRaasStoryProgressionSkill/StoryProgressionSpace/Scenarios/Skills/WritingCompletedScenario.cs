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
using Sims3.Gameplay.Situations;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class WritingCompletedScenario : SimEventScenario<WroteGenreBookEvent>, IFormattedStoryScenario
    {
        public WritingCompletedScenario()
        { }
        protected WritingCompletedScenario(WritingCompletedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            if (type == PrefixType.Summary)
            {
                return "Writing";
            }
            else
            {
                return "WritingComplete";
            }
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Skills;
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kWroteBookOfGenre);
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

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            Writing skill = Sim.SkillManager.GetSkill<Writing>(SkillNames.Writing);

            WrittenBookData writing = skill.LastCompletedWriting;

            List<BookGeneralData> randomList = new List<BookGeneralData>(BookData.BookGeneralDataList.Values);
            BookGeneralData randomObjectFromList = RandomUtil.GetRandomObjectFromList(randomList);

            int genre = RandomUtil.GetInt(1, Writing.kDialogsPerGenre);
            int quality = RandomUtil.GetInt(1, Writing.kDialogsPerQuality);

            text = Writing.LocalizeString(Sim.IsFemale, "Finished" + writing.Genre.ToString() + genre, new object[] { Sim, writing.NumPages, writing.Title, randomObjectFromList.Title });
            if (writing.Quality != Writing.WrittenBookQuality.Normal)
            {
                string str2 = Writing.LocalizeString(Sim.IsFemale, "Finished" + writing.Quality.ToString() + quality, new object[] { Sim, writing.NumPages, writing.Title });
                text = text + " " + str2;
            }

            string str3 = Writing.LocalizeString(Sim.IsFemale, "RoyaltyDetails", new object[] { Sim, Writing.kRoyaltyLength, writing.Royalty, SimClockUtils.GetText(Writing.kRoyaltyPayHour) });
            text = text + Common.NewLine + Common.NewLine + str3;

            if (extended == null)
            {
                extended = new string[] { writing.Title, EAText.GetNumberString(writing.Royalty) };
            }

            return base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new WritingCompletedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSkill, WritingCompletedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "WritingCompleted";
            }
        }
    }
}
