using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class AfterSchoolActivityCriteria : SelectionTestableOptionList<AfterSchoolActivityCriteria.Item, AfterschoolActivityType, AfterschoolActivityType>
    {
        public override string GetTitlePrefix()
        {
            return "AfterSchoolActivity";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (me.CareerManager == null) return false;

            School school = me.CareerManager.School;
            if (school == null) return false;

            return base.Allow(me, actor);
        }

        public static bool HasActivity(SimDescription sim, AfterschoolActivityType type)
        {
            if (sim.CareerManager != null)
            {
                School school = sim.CareerManager.School;
                if (school == null)
                {
                    return false;
                }
                if (school.AfterschoolActivities == null)
                {
                    return false;
                }
                foreach (AfterschoolActivity activity in school.AfterschoolActivities)
                {
                    if (activity.CurrentActivityType == type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public class Item : TestableOption<AfterschoolActivityType, AfterschoolActivityType>
        {
            [Persistable(false)]
            AfterschoolActivity mActivity;

            public Item()
            { }
            public Item(AfterschoolActivity value, int count)
            {
                mActivity = value;

                SetValue(value.CurrentActivityType, value.CurrentActivityType);

                mCount = count;
            }
            public Item(AfterschoolActivityType value, int count)
            {
                SetValue(value, value);

                mCount = count;
            }

            public AfterschoolActivity Activity
            {
                get { return mActivity; }
            }

            public override void SetValue(AfterschoolActivityType value, AfterschoolActivityType storeType)
            {
                mValue = value;

                mName = AfterschoolActivity.LocalizeString(false, value.ToString(), new object[0]);
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<AfterschoolActivityType, AfterschoolActivityType> results)
            {
                if (me.CareerManager == null) return false;

                School school = me.CareerManager.School;
                if (school == null) return false;

                if (school.AfterschoolActivities == null) return false;

                foreach (AfterschoolActivity activity in school.AfterschoolActivities)
                {
                    results[activity.CurrentActivityType] = activity.CurrentActivityType;
                }

                return true;
            }
        }
    }
}
