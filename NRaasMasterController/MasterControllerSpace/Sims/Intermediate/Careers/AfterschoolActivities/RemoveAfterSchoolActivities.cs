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
    public class RemoveAfterSchoolActivities : SimFromList, IAfterschoolActivitiesOption
    {
        IEnumerable<AfterSchoolActivityCriteria.Item> mSelection = null;

        public override string GetTitlePrefix()
        {
            return "AfterSchoolActivityRemove";
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

            if (me.CareerManager == null) return false;

            School school = me.CareerManager.School;
            if (school == null) return false;

            if (school.AfterschoolActivities == null) return false;

            return (school.AfterschoolActivities.Count > 0);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            School school = me.CareerManager.School;
            if (school == null) return false;

            if (!ApplyAll)
            {
                List<AfterSchoolActivityCriteria.Item> choices = new List<AfterSchoolActivityCriteria.Item>();

                foreach (AfterschoolActivity activity in school.AfterschoolActivities)
                {
                    choices.Add(new AfterSchoolActivityCriteria.Item(activity.CurrentActivityType, 1));
                }

                CommonSelection<AfterSchoolActivityCriteria.Item>.Results selection = new CommonSelection<AfterSchoolActivityCriteria.Item>(Name, choices).SelectMultiple();
                if ((selection == null) || (selection.Count == 0)) return false;

                mSelection = selection;
            }

            bool changed = false;
            foreach (AfterSchoolActivityCriteria.Item item in mSelection)
            {
                for(int i=school.AfterschoolActivities.Count-1; i>=0; i--)
                {
                    if (school.AfterschoolActivities[i].CurrentActivityType == item.Value)
                    {
                        school.AfterschoolActivities.RemoveAt (i);

                        changed = true;
                        break;
                    }
                }
            }

            if (changed)
            {
                me.CareerManager.UpdateCareerUI();
            }

            return true;
        }
    }
}
