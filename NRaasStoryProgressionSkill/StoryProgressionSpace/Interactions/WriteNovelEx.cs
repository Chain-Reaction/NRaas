using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class WriteNovelEx : Computer.WriteNovel, Common.IPreLoad
    {
        public static new Definition Singleton = new Definition();

        DateAndTime mStart;

        public void OnPreLoad()
        {
            Tunings.Inject<Computer, Computer.WriteNovel.Definition, Definition>(false);
        }

        private new void LoopDel(StateMachineClient smc, Interaction<Sim, Computer>.LoopData loopData)
        {
            try
            {
                base.LoopDel(smc, loopData);

                if (SimClock.CurrentTime() > mStart)
                {
                    Actor.AddExitReason(ExitReason.Finished);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);

                Actor.AddExitReason(ExitReason.Finished);
            }
        }

        private static bool Exists(Writing skill, string key)
        {
            if (skill.WrittenBookDataList.ContainsKey(key)) return true;

            if (BookData.BookWrittenDataList.ContainsKey(key + skill.SkillOwner.FullName)) return true;

            return false;
        }

        private static bool StartNewWriting(Computer ths, Writing writingSkill, BookData.BookGenres genre)
        {
            List<string> randomList = BookData.WrittenBookTitles[genre];

            if (writingSkill.WrittenBookDataList == null)
            {
                writingSkill.WrittenBookDataList = new Dictionary<string, WrittenBookData>();
            }

            int count = 0;

            string key = null;

            while (count < 25)
            {
                key = Common.LocalizeEAString("Gameplay/Excel/Books/WrittenBookTitles:" + RandomUtil.GetRandomObjectFromList(randomList));
                count++;

                if (!Exists(writingSkill, key))
                {
                    break;
                }
            }

            if (Exists(writingSkill, key))
            {
                string oldKey = key;

                int num = 0x1;
                do
                {
                    num++;
                    key = oldKey + " " + num;
                }
                while (Exists(writingSkill, key));
            }

            if ((SimTypes.IsSelectable(writingSkill.SkillOwner)) || (StoryProgression.Main.GetValue<PromptToTitleOption, bool>()))
            {
                string title = StringInputDialog.Show(Common.Localize("TitleBook:Header", writingSkill.SkillOwner.IsFemale), Common.Localize("TitleBook:Prompt", writingSkill.SkillOwner.IsFemale, new object[] { writingSkill.SkillOwner }), key);
                if (!string.IsNullOrEmpty(title))
                {
                    key = title;
                }
            }

            writingSkill.AddWritingToDataList(key, writingSkill.GetNumPagesForWriting(genre), genre, true, null);
            return true;
        }

        public override bool Run()
        {
            string msg = null;

            try
            {
                mStart = SimClock.Add(SimClock.CurrentTime(), TimeUnit.Hours, 3);

                StandardEntry();
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    StandardExit();

                    return false;
                }

                msg += "A";

                mWritingSkill = Actor.SkillManager.AddElement(SkillNames.Writing) as Writing;

                Target.StartVideo(Computer.VideoType.WordProcessor);
                mStartWritingTime = SimClock.CurrentTime();
                BeginCommodityUpdates();

                bool playFilledEffect = false;
                try
                {
                    if (mWritingSkill.CurrentWriting == null)
                    {
                        msg += "B";

                        BookData.BookGenres choice = mWritingSkill.mNovelistGenre;

                        if (choice == BookData.BookGenres.None)
                        {
                            List<BookData.BookGenres> genres = new List<BookData.BookGenres>();

                            foreach (BookData.BookGenres genre in Enum.GetValues(typeof(BookData.BookGenres)))
                            {
                                if (genre == BookData.BookGenres.None) continue;

                                if (genre == BookData.BookGenres.Biography) continue;

                                if (genre == BookData.BookGenres.Autobiography) continue;

                                if (!Writing.CanWriteGenre(Actor, genre)) continue;

                                switch(genre)
                                {
                                    case BookData.BookGenres.Horror:
                                    case BookData.BookGenres.Poetry:
                                        if (!GameUtils.IsInstalled(ProductVersion.EP7)) continue;

                                        break;
                                }

                                genres.Add(genre);
                            }

                            if (genres.Count == 0)
                            {
                                return false;
                            }

                            msg += "C";

                            if ((SimTypes.IsSelectable(mWritingSkill.SkillOwner)) || (StoryProgression.Main.GetValue<PromptForGenreOption, bool>()))
                            {
                                List<GenreOption> choices = new List<GenreOption>();
                                foreach (BookData.BookGenres genre in genres)
                                {
                                    int count = 0;

                                    if (mWritingSkill.WrittenBookGenreCount != null)
                                    {
                                        if (!mWritingSkill.WrittenBookGenreCount.TryGetValue(genre, out count))
                                        {
                                            count = 0;
                                        }
                                    }

                                    choices.Add(new GenreOption(genre, count));
                                }

                                GenreOption option = new CommonSelection<GenreOption>(Common.Localize("ChooseBookGenre:Header", mWritingSkill.SkillOwner.IsFemale), mWritingSkill.SkillOwner.FullName, choices).SelectSingle();
                                if (option != null)
                                {
                                    choice = option.Value;
                                }
                            }

                            msg += "D";

                            if (choice == BookData.BookGenres.None)
                            {
                                choice = RandomUtil.GetRandomObjectFromList(genres);
                            }
                        }

                        msg += "E";

                        if (!StartNewWriting(Target, mWritingSkill, choice))
                        {
                            AnimateSim("WorkTyping");
                            EndCommodityUpdates(false);
                            Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                            StandardExit();

                            return false;
                        }
                    }

                    msg += "F";

                    AnimateSim("WorkTyping");
                    ProgressMeter.ShowProgressMeter(Actor, 0f, ProgressMeter.GlowType.Weak);

                    playFilledEffect = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), LoopDel, null);
                    ProgressMeter.HideProgressMeter(Actor, playFilledEffect);
                    float points = SimClock.ElapsedTime(TimeUnit.Hours, mStartWritingTime) * Computer.kWritingNovelPointsPerHour;
                    mWritingSkill.AddPoints(points);
                }
                finally
                {
                    EndCommodityUpdates(playFilledEffect);
                }

                msg += "G";

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                if (mWritingSkill.IsWritingComplete())
                {
                    WrittenBookData currentWriting = mWritingSkill.CurrentWriting;

                    if (!BookData.BookWrittenDataList.ContainsKey(currentWriting.Title + mWritingSkill.SkillOwner.FullName))
                    {
                        List<Lot> lots = new List<Lot>();

                        if ((!SimTypes.IsSelectable(Actor)) && (!StoryProgression.Main.GetValue<AddBooksToLibraryOption, bool>()))
                        {
                            foreach (Lot lot in LotManager.AllLots)
                            {
                                if (lot.GetMetaAutonomyType == Lot.MetaAutonomyType.Library)
                                {
                                    lots.Add(lot);

                                    lot.mMetaAutonomyType = Lot.MetaAutonomyType.None;
                                }
                            }
                        }

                        try
                        {
                            msg += "H";

                            Target.FinalizeWriting(mWritingSkill);
                        }
                        finally
                        {
                            foreach (Lot lot in lots)
                            {
                                lot.mMetaAutonomyType = Lot.MetaAutonomyType.Library;
                            }
                        }
                    }
                }

                msg += "I";

                StandardExit();

                return playFilledEffect;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, msg, e);
                return false;
            }
        }

        public new class Definition : InteractionDefinition<Sim, Computer, WriteNovelEx>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return Common.Localize("WriteNovelPush:MenuName");
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return target.IsComputerUsable(a, true, false, isAutonomous);
            }
        }

        public class GenreOption : ValueSettingOption<BookData.BookGenres>
        {
            public GenreOption(BookData.BookGenres genre, int count)
                : base(genre, BookData.GetGenreLocalizedString(genre), count)
            { }
        }

        public class AddBooksToLibraryOption : BooleanManagerOptionItem<ManagerSkill>
        {
            public AddBooksToLibraryOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AddWrittenBooksToLibrary";
            }
        }

        public class PromptForGenreOption : BooleanManagerOptionItem<ManagerSkill>
        {
            public PromptForGenreOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "PromptForBookGenre";
            }
        }

        public class PromptToTitleOption : BooleanManagerOptionItem<ManagerSkill>
        {
            public PromptToTitleOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "PromptToTitleBooks";
            }
        }
    }
}

