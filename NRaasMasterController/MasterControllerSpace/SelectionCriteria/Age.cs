using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class Age : SelectionTestableOptionList<Age.Item, CASAgeGenderFlags, CASAgeGenderFlags>
    {
        public override string GetTitlePrefix()
        {
 	        return "Criteria.Age";
        }

        public class Item : TestableOption<CASAgeGenderFlags, CASAgeGenderFlags>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<CASAgeGenderFlags, CASAgeGenderFlags> results)
            {
                results[me.Age] = me.Age;
                return true;
            }

            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<CASAgeGenderFlags, CASAgeGenderFlags> results)
            {
                results[me.Age] = me.Age;
                return true;
            }

            public override void SetValue(CASAgeGenderFlags value, CASAgeGenderFlags storeType)
            {
                mValue = value;

                mName = GetName(value);
            }

            public static string GetName(CASAgeGenderFlags value)
            {
                return Common.LocalizeEAString("UI/Feedback/CAS:" + value);
            }
        }
    }
}
