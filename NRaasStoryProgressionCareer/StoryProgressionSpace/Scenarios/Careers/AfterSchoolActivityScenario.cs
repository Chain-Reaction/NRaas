using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class AfterSchoolActivityScenario : SimScenario
    {
        static Common.MethodStore sCareerGetAfterSchoolActivityList = new Common.MethodStore("NRaasCareer", "Careers", "GetAfterSchoolActivityList", new Type[] { typeof(SimDescription) });

        public AfterSchoolActivityScenario()
        { }
        protected AfterSchoolActivityScenario(AfterSchoolActivityScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AfterSchoolActivity";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected static bool HasActivity(SimDescription sim)
        {
            List<IAfterschoolActivity> list = sim.GetAfterschoolActivities;
            if (list == null) return false;

            return (list.Count > 0);
        }

        protected override bool Allow(SimDescription sim)
        {
            if ((!sim.Child) && (!sim.Teen))
            {
                IncStat("Wrong Age");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }
            else if (!GetValue<AllowFindJobOption, bool>(sim))
            {
                IncStat("Find Job Denied");
                return false;
            }
            else if (HasActivity(sim))
            {
                IncStat("Has Activity");
                return false;
            }

            return base.Allow(sim);
        }

        public class WeightedChoice : IWeightable
        {
            public readonly AfterschoolActivityType mType;
            public readonly float mWeight;

            public WeightedChoice(AfterschoolActivityType type, float weight)
            {
                mType = type;
                mWeight = weight;
            }

            public float Weight
            {
                get
                {
                    return mWeight;
                }
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<AfterschoolActivity> allChoices = sCareerGetAfterSchoolActivityList.Invoke<List<AfterschoolActivity>>(new object[] { Sim });
            if (allChoices == null)
            {
                allChoices = new List<AfterschoolActivity>();

                foreach (AfterschoolActivityType type in Enum.GetValues(typeof(AfterschoolActivityType)))
                {
                    if ((Sim.Child) && (!AfterschoolActivity.IsChildActivity(type))) continue;

                    if ((Sim.Teen) && (!AfterschoolActivity.IsTeenActivity(type))) continue;

                    GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                    if (!AfterschoolActivity.MeetsCommonAfterschoolActivityRequirements(Sim.CreatedSim, type, ref greyedOutTooltipCallback)) continue;

                    allChoices.Add(new AfterschoolActivity(type));
                }
            }

            List<WeightedChoice> choices = new List<WeightedChoice>();

            foreach(AfterschoolActivity activity in allChoices)
            {
                float weight = 0;
                int count = 0;

                foreach(SkillNames actualSkill in activity.ActivitySkillNameList)
                {
                    SkillNames skill = actualSkill;
                    switch (skill)
                    {
                        case SkillNames.BassGuitar:
                        case SkillNames.Piano:
                        case SkillNames.Drums:
                            skill = SkillNames.Guitar;
                            break;
                    }

                    if (ScoringLookup.HasScoring(skill.ToString()))
                    {
                        weight += AddScoring(skill.ToString(), Sim);
                    }
                    else
                    {
                        Common.DebugNotify("AfterschoolActivity Missing " + skill.ToString());

                        weight += AddScoring("AfterschoolActivity", Sim);
                    }

                    count++;
                }

                if (weight < 0) continue;

                if (count == 0) 
                {
                    count = 1;
                }

                choices.Add(new WeightedChoice(activity.CurrentActivityType, weight / count));
            }

            if (choices.Count == 0)
            {
                IncStat("No Choices");
                return false;
            }

            WeightedChoice choice = RandomUtil.GetWeightedRandomObjectFromList(choices);

            AfterschoolActivity.AddNewActivity(Sim.CreatedSim, choice.mType);
            return true;
        }

        public override Scenario Clone()
        {
            return new AfterSchoolActivityScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerCareer, AfterSchoolActivityScenario>, ManagerCareer.ISchoolOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AfterSchoolActivity";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }
        }
    }
}
