using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers.AfterschoolActivities
{
    public class AddAfterSchoolActivities : SimFromList, IAfterschoolActivitiesOption
    {
        static Common.MethodStore sCareerAfterschoolActivityList = new Common.MethodStore("NRaasCareer", "NRaas.Careers", "GetAfterSchoolActivityList", new Type[] { typeof (SimDescription) });

        IEnumerable<AfterSchoolActivityCriteria.Item> mSelection = null;

        public override string GetTitlePrefix()
        {
            return "AfterSchoolActivityAdd";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.LotCurrent == null) return false;

            if (me.CreatedSim.LotCurrent.IsWorldLot) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<AfterSchoolActivityCriteria.Item> choices = new List<AfterSchoolActivityCriteria.Item>();

                List<AfterschoolActivity> customTypes = sCareerAfterschoolActivityList.Invoke<List<AfterschoolActivity>>(new object[] { me });

                if (customTypes != null)
                {
                    foreach (AfterschoolActivity type in customTypes)
                    {
                        choices.Add(new AfterSchoolActivityCriteria.Item(type, AfterSchoolActivityCriteria.HasActivity(me, type.CurrentActivityType) ? 1 : 0));
                    }
                }
                else
                {
                    foreach (AfterschoolActivityType type in Enum.GetValues(typeof(AfterschoolActivityType)))
                    {
                        if ((me.Child) && (AfterschoolActivity.IsChildActivity(type)))
                        {
                            choices.Add(new AfterSchoolActivityCriteria.Item(type, AfterSchoolActivityCriteria.HasActivity(me, type) ? 1 : 0));
                        }
                        else if (me.Teen && AfterschoolActivity.IsTeenActivity(type))
                        {
                            choices.Add(new AfterSchoolActivityCriteria.Item(type, AfterSchoolActivityCriteria.HasActivity(me, type) ? 1 : 0));
                        }
                    }
                }

                if (choices.Count == 0)
                {
                    SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":NoChoices", me.IsFemale, new object[] { me }));
                    return false;
                }

                CommonSelection<AfterSchoolActivityCriteria.Item>.Results selection = new CommonSelection<AfterSchoolActivityCriteria.Item>(Name, choices).SelectMultiple();
                if ((selection == null) || (selection.Count == 0)) return false;

                mSelection = selection;
            }

            foreach (AfterSchoolActivityCriteria.Item item in mSelection)
            {
                School school = me.CareerManager.School;
                if (school.AfterschoolActivities == null)
                {
                    school.AfterschoolActivities = new List<AfterschoolActivity>();
                }

                if (AfterSchoolActivityCriteria.HasActivity(me, item.Value))
                {
                    for (int i = school.AfterschoolActivities.Count - 1; i >= 0; i--)
                    {
                        if (school.AfterschoolActivities[i].CurrentActivityType == item.Value)
                        {
                            school.AfterschoolActivities.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    AfterschoolActivity newActivity = new AfterschoolActivity(item.Value);

                    AfterschoolActivity oldActivity = item.Activity;
                    if (oldActivity != null)
                    {
                        newActivity.ActivitySkillNameList = new List<Sims3.Gameplay.Skills.SkillNames>(oldActivity.ActivitySkillNameList);
                        newActivity.mDaysForActivity = oldActivity.mDaysForActivity;
                    }

                    school.AfterschoolActivities.Add(newActivity);
                }

                me.CareerManager.UpdateCareerUI();
            }

            return true;
        }
    }
}
