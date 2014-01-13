using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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
    public abstract class CareerHiredBaseScenario : CareerEventScenario, IFormattedStoryScenario
    {
        bool mReport = true;

        string mText;

        public CareerHiredBaseScenario()
        { }
        protected CareerHiredBaseScenario(CareerHiredBaseScenario scenario)
            : base (scenario)
        {
            mReport = scenario.mReport;
            mText = scenario.mText;
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Careers;
        }

        protected override bool ShouldReport
        {
            get 
            {
                if (!base.ShouldReport) return false;

                return mReport; 
            }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            CareerHiredBaseScenario scenario = base.Handle(e, ref result) as CareerHiredBaseScenario;
            if (scenario != null)
            {
                scenario.UpdateText();
            }

            return scenario;
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kEventCareerHired);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Event.Career is Retired)
            {
                IncStat("Retired");
                mReport = false;
            }

            Add(frame,new CareerChangedScenario(Sim), ScenarioResult.Start);
            return true;
        }

        protected void UpdateText()
        {
            mText = GetText(Event.Career);
        }

        protected static string GetText(Occupation occupation)
        {
            if (occupation is Retired) return null;

            Career career = occupation as Career;
            if (career != null)
            {
                if (career.CurLevel == null) return null;

                if (occupation.CareerLoc == null) return null;

                if (occupation.CareerLoc.Owner == null) return null;

                DaysOfTheWeek nextWorkDay = career.CurLevel.GetNextWorkDay(SimClock.CurrentDayOfWeek);
                string entryKey = (occupation is School) ? "Gameplay/Careers/Career:FirstSchoolDayDetails" : "Gameplay/Careers/Career:FirstDayDetails";

                string text = Common.LocalizeEAString(occupation.OwnerDescription.IsFemale, entryKey, new object[] { occupation.OwnerDescription, occupation.CareerLoc.Owner.CatalogName, SimClockUtils.GetText(career.CurLevel.StartTime), SimClockUtils.GetDayAsText(nextWorkDay) });
                if (occupation.Boss != null)
                {
                    text += Common.NewLine + Common.NewLine + Common.LocalizeEAString(occupation.OwnerDescription.IsFemale, "Gameplay/Careers:MetBoss", new object[] { occupation.OwnerDescription, occupation.Boss });
                }

                return text;
            }
            else
            {
                XpBasedCareer xpCareer = occupation as XpBasedCareer;
                if (xpCareer != null)
                {
                    return PromotedScenario.GenerateXPBasedLevelUp(xpCareer);
                }
            }

            return null;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Careers;
            }

            if (extended == null)
            {
                if (Event.Career is School)
                {
                    extended = new string[] { Event.Career.GetLocalizedCareerName(Sim.IsFemale) };
                }
                else
                {
                    extended = new string[] { Event.Career.GetLocalizedCareerName(Sim.IsFemale), EAText.GetNumberString(Event.Career.Level) };
                }
            }

            ManagerStory.Story story = base.PrintFormattedStory(manager, mText, summaryKey, parameters, extended, logging);

            if (story != null)
            {
                story.mOverrideImage = Event.Career.CareerIconColored;
            }

            return story;
        }
    }
}
