using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class CareerLevelCriteria : SelectionTestableOptionList<CareerLevelCriteria.Item,int,int>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.CareerLevel";
        }

        public class Item : TestableOption<int,int>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<int,int> results)
            {
                int level = 0;

                if (me.Occupation != null)
                {
                    level = me.Occupation.CareerLevel;
                }

                results[level] = level;
                return true;
            }

            public override void SetValue(int value, int storeType)
            {
                mValue = value;

                mName = Common.Localize("Level:MenuName", false, new object[] { value });
            }
        }
    }
}
