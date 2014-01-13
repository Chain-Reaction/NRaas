using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
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
    public class PromotedScenario : CareerEventScenario
    {
        // Valid only during the scope of the Event itself
        static List<ulong> sSuppressed = new List<ulong>();

        // Valid on a delayed basis once the scenario has been queued
        List<ulong> mSuppressed = new List<ulong>();

        string mText;

        public PromotedScenario()
        { }
        protected PromotedScenario(PromotedScenario scenario)
            : base (scenario)
        {
            mText = scenario.mText;
            mSuppressed = new List<ulong>(mSuppressed);
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Promoted";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public static void AddSuppressed(SimDescription sim)
        {
            sSuppressed.Add(sim.SimDescriptionId);
        }

        public static void RemoveSuppressed(SimDescription sim)
        {
            sSuppressed.Remove(sim.SimDescriptionId);
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kEventCareerPromotion);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            PromotedScenario scenario = base.Handle(e, ref result) as PromotedScenario;
            if (scenario != null)
            {
                scenario.UpdateText();

                scenario.mSuppressed.AddRange(sSuppressed);
                sSuppressed.Clear();
            }

            return scenario;
        }

        public bool IsSuppressed()
        {
            return mSuppressed.Contains(Sim.SimDescriptionId);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            return base.Allow(sim);
        }

        public static event UpdateDelegate OnCareerBranchScenario;
        public static event UpdateDelegate OnRetireAtMaxScenario;
        public static event UpdateDelegate OnJackOfAllTradesScenario;
        public static event UpdateDelegate OnOnlyOneMayLeadScenario;
        public static event UpdateDelegate OnCelebrityScenario;

        public static bool PerformRetireAtMax(Scenario scenario, ScenarioFrame frame)
        {
            if (OnRetireAtMaxScenario != null)
            {
                OnRetireAtMaxScenario(scenario, frame);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!SimTypes.IsSelectable(Sim))
            {
                ManagerSim.IncrementLifetimeHappiness(Sim, Sims3.Gameplay.Rewards.LifeEventRewards.kLifetimeHappinessRewardForPromotion);
            }

            ManagerSim.TestInactiveLTWTask.Perform(this, Sim, Event);

            PerformRetireAtMax(this, frame);

            if (OnOnlyOneMayLeadScenario != null)
            {
                OnOnlyOneMayLeadScenario(this, frame);
            }

            if (OnJackOfAllTradesScenario != null)
            {
                OnJackOfAllTradesScenario(this, frame);
            }

            if (OnCareerBranchScenario != null)
            {
                OnCareerBranchScenario(this, frame);
            }

            if (OnCelebrityScenario != null)
            {
                OnCelebrityScenario(this, frame);
            }

            if (ShouldReport)
            {
                Add(frame, new ReportScenario(Sim, Event.Career, mText), ScenarioResult.Start);
            }

            Add(frame, new CareerChangedScenario(Sim), ScenarioResult.Start);
            return true;
        }

        public override Scenario Clone()
        {
            return new PromotedScenario(this);
        }

        protected void UpdateText()
        {
            mText = GetText(Event.Career);
        }

        protected static string GetText(Occupation occupation)
        {
            Career career = occupation as Career;
            if (career != null)
            {
                if (occupation.OwnerDescription.CreatedSim != null)
                {
                    return career.GeneratePromotionText(0);
                }
                else
                {
                    // This is a copy of the GeneratePromotionText() sans the use of CreatedSim
                    string entryKey = "Gameplay/Careers/Career:PromotedTextNoBonus";
                    string str2 = Common.LocalizeEAString(occupation.OwnerDescription.IsFemale, entryKey, new object[] { occupation.OwnerDescription, career.CurLevel.GetLocalizedName(occupation.OwnerDescription), 0 }) + Common.NewLine + Common.NewLine + Common.LocalizeEAString(occupation.OwnerDescription.IsFemale, career.CurLevel.Text_Promotion, new object[] { occupation.OwnerDescription, career.CurLevel.GetLocalizedName(occupation.OwnerDescription) });
                    string startTime = SimClockUtils.GetText(career.CurLevel.StartTime);
                    string str4 = SimClockUtils.GetText(career.CurLevel.StartTime + career.CurLevel.DayLength);
                    if (!occupation.ShowRockStarUI)
                    {
                        return str2 + Common.NewLine + Common.NewLine + Common.LocalizeEAString(occupation.OwnerDescription.IsFemale, "Gameplay/Careers/Career:PromotedWageUpdate", new object[] { occupation.OwnerDescription, career.PayPerHourOrStipend, startTime, str4 });
                    }
                    else
                    {
                        return str2 + Common.NewLine + Common.NewLine + Common.LocalizeEAString(occupation.OwnerDescription.IsFemale, "Gameplay/Careers/Career:RockStarPromotedUpdate", new object[] { occupation.OwnerDescription });
                    }
                }
            }
            else
            {
                XpBasedCareer xpCareer = occupation as XpBasedCareer;
                if (xpCareer != null)
                {
                    return GenerateXPBasedLevelUp(xpCareer);
                }
            }

            return null;
        }

        public static string GenerateXPBasedLevelUp(XpBasedCareer ths)
        {
            XpBasedCareerLevelStaticData currentLevelStaticDataForXpBasedCareer = ths.GetCurrentLevelStaticDataForXpBasedCareer();
            if (currentLevelStaticDataForXpBasedCareer != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (ths.mLevel == 0x1)
                {
                    string occupationJoiningTnsTextPrefix = ths.GetOccupationJoiningTnsTextPrefix();
                    if (!string.IsNullOrEmpty(occupationJoiningTnsTextPrefix))
                    {
                        stringBuilder.Append(occupationJoiningTnsTextPrefix);
                    }
                }
                else if (ths.mLevel > 0x1)
                {
                    stringBuilder.Append(Common.LocalizeEAString(ths.OwnerDescription.IsFemale, "Gameplay/Excel/ActiveCareers/CareerLevels:PromotionTextBaseNew", new object[] { ths.OwnerDescription, ths.CurLevelJobTitle }));
                }
                if (ths.AppendLevelDescriptionInTns)
                {
                    ths.AppendTextForTns(ref stringBuilder, Common.LocalizeEAString(ths.OwnerDescription.IsFemale, currentLevelStaticDataForXpBasedCareer.DescriptionLocalizationKey, new object[] { ths.OwnerDescription, ths.CurLevelJobTitle }));
                }
                string rewardLocalizationKey = currentLevelStaticDataForXpBasedCareer.RewardLocalizationKey;
                if (!string.IsNullOrEmpty(rewardLocalizationKey))
                {
                    ths.AppendTextForTns(ref stringBuilder, Common.LocalizeEAString(ths.OwnerDescription.IsFemale, rewardLocalizationKey, new object[] { ths.OwnerDescription, ths.CurLevelJobTitle }));
                }
                if (ths.PayPerHourOrStipend > 0f)
                {
                    string str5 = "StipendUpdate";
                    if (((ths.StartTimeText == null) && (ths.FinishTimeText == null)) || ths.HasOpenHours)
                    {
                        str5 = "StipendUpdateNoTimes";
                    }
                    ths.AppendTextForTns(ref stringBuilder, XpBasedCareer.LocalizeString(ths.OwnerDescription.IsFemale, str5, new object[] { ths.OwnerDescription, ths.PayPerHourOrStipend, ths.StartTimeText, ths.FinishTimeText }));
                }

                return stringBuilder.ToString();
            }

            return null;
        }

        protected class ReportScenario : SimScenario, IFormattedStoryScenario
        {
            Occupation mCareer = null;

            string mText;

            public ReportScenario()
            { }
            public ReportScenario(SimDescription sim, Occupation career, string text)
                : base(sim)
            {
                mCareer = career;
                mText = text;
            }
            public ReportScenario(ReportScenario scenario)
                : base(scenario)
            {
                mCareer = scenario.mCareer;
                mText = scenario.mText;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Story) return null;

                return "Promotion";
            }

            public Manager GetFormattedStoryManager()
            {
                return StoryProgression.Main.Careers;
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (sim.Occupation == null)
                {
                    IncStat("No Job");
                    return false;
                }
                else if (sim.Occupation != mCareer)
                {
                    IncStat("Job Changed");
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
                if (manager == null)
                {
                    manager = Careers;
                }

                if ((extended == null) && (Sim.Occupation != null))
                {
                    extended = new string[] { Sim.Occupation.GetLocalizedCareerName(Sim.IsFemale), EAText.GetNumberString(Sim.Occupation.Level) };
                }

                ManagerStory.Story story = base.PrintFormattedStory(manager, mText, summaryKey, parameters, extended, logging);

                if (story != null)
                {
                    story.mOverrideImage = mCareer.CareerIconColored;
                }

                return story;
            }

            public override Scenario Clone()
            {
                return new ReportScenario(this);
            }
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, PromotedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Promoted";
            }
        }
    }
}
