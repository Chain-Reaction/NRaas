using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
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
    public class EducateScenario : TeachScenario
    {
        WeightOption.NameOption mName = null;

        WeightScenarioHelper mSuccess = null;

        SkillNames mSkill = SkillNames.None;

        public EducateScenario()
        { }
        protected EducateScenario(EducateScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mSuccess = scenario.mSuccess;
            mSkill = scenario.mSkill;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "Skill=" + mSkill;

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

        protected override SkillNames Skill
        {
            get { return mSkill; }
        }

        public override bool IsFriendly
        {
            get { return true; }
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

            if (row.Exists("Skill"))
            {
                mSkill = SkillManager.sSkillEnumValues.ParseEnumValue(row.GetString("Skill"));
                if (mSkill == SkillNames.None)
                {
                    error = "Skill unknown";
                    return false;
                }
            }

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

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.Age > Sim.Age)
            {
                IncStat("Too Old");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            Add(frame, new PropagateClanDelightScenario(Sim, Manager, Origin.FromCharity), ScenarioResult.Start);

            mSuccess.Perform(this, frame, "Success", Sim, Target);

            return true;
        }

        public override Scenario Clone()
        {
            return new EducateScenario(this);
        }
    }
}
