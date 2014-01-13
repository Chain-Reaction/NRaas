using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class HelpHomeworkScenario : RelationshipScenario
    {
        WeightOption.NameOption mName = null;

        WeightScenarioHelper mSuccess = null;

        public HelpHomeworkScenario()
            : base (10)
        { }
        protected HelpHomeworkScenario(HelpHomeworkScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mSuccess = scenario.mSuccess;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;

            return text;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return mName.WeightName;
            }
            else
            {
                return mName.ToString();
            }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mSuccess.ShouldPush(base.ShouldPush);
            }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mSuccess = new WeightScenarioHelper(Origin.FromCharity);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            HomeworkScenario.Perform(this, Sim, 100);
            HomeworkScenario.Perform(this, Target, 100);

            School school = Target.CareerManager.School;
            if (school != null)
            {
                school.mPerformance += 25;
                if (school.mPerformance > 100)
                {
                    school.mPerformance = 100;
                }
            }

            mSuccess.Perform(this, frame, "Success", Sim, Target);

            return true;
        }

        public override Scenario Clone()
        {
            return new HelpHomeworkScenario(this);
        }
    }
}
