using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ImmigrantPressureScenario : ScheduledSoloScenario, IFormattedStoryScenario
    {
        ManagerLot.ImmigrationRequirement mRequirement;

        string mStory;

        bool mReport;

        public ImmigrantPressureScenario(ManagerLot.ImmigrationRequirement requirement, bool report)
        {
            mRequirement = requirement;
            mReport = report;
        }
        protected ImmigrantPressureScenario(ImmigrantPressureScenario scenario)
            : base (scenario)
        {
            mRequirement = scenario.mRequirement;
            mStory = scenario.mStory;
            mReport = scenario.mReport;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            return "ImmigrantPressure";
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Lots;
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool ShouldReport
        {
            get 
            {
                if (!Common.kDebugging)
                {
                    if ((GameUtils.IsUniversityWorld()) && (GameStates.IsOnVacation)) return false;
                }

                return mReport; 
            }
        }

        protected int ImmigrationGauge
        {
            get { return GetValue<ScheduledImmigrationScenario.GaugeOption, int>(); }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<Manager> managers = new List<Manager>();

            string body = null;

            int previousPressure = 0;
            foreach (KeyValuePair<Manager, int> value in Lots.ImmigrationPressure)
            {
                if (value.Value == 0) continue;

                AddStat("Pressure: " + value.Key.UnlocalizedName, value.Value);

                body += Localize("PressureElement", false, new object[] { value.Key.GetLocalizedName(), value.Value });

                managers.Add(value.Key);

                previousPressure += value.Value;
            }

            AddStat("Total", previousPressure);

            Lots.PreviousPressure = previousPressure;

            if (ImmigrationGauge <= 0) return false;

            mStory = Localize("PressureSummary", false, new object[] { body, previousPressure });

            if (previousPressure <= ImmigrationGauge)
            {
                mStory += Localize("PressureInsufficient", false, new object[] { ImmigrationGauge });
                return true;
            }

            if (Sims.HasEnough(this, CASAgeGenderFlags.Human))
            {
                mStory += Localize("PressureMaxResidents");
                return true;
            }

            foreach (Manager manager in managers)
            {
                Add(frame, manager.GetImmigrantRequirement(mRequirement), ScenarioResult.Start);
            }

            mStory = null;

            mRequirement.mRequired = true;
            return false;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (!ShouldReport) return null;

            if (manager == null)
            {
                manager = Lots;
            }

            ManagerStory.Story story = base.PrintFormattedStory(manager, mStory, summaryKey, parameters, extended, logging);

            if (story != null)
            {
                story.mShowNoImage = true;
            }

            return story;
        }

        public override Scenario Clone()
        {
            return new ImmigrantPressureScenario(this);
        }
    }
}
