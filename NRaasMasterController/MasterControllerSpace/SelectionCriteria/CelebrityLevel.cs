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
    public class CelebrityLevel : SelectionTestableOptionList<CelebrityLevel.Item, uint, uint>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Celebrity";
        }

        public class Item : TestableOption<uint, uint>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<uint,uint> results)
            {
                try
                {
                    results[me.CelebrityLevel] = me.CelebrityLevel;
                    return true;
                }
                catch
                {
                    // Sim may not have a valid celebrity manager
                    return false;
                }
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<uint, uint> results)
            {
                try
                {
                    results[me.CelebrityLevel] = me.CelebrityLevel;
                    return true;
                }
                catch
                {
                    // Sim may not have a valid celebrity manager
                    return false;
                }
            }

            public override void SetValue(uint value, uint storeType)
            {
                mValue = value;

                mName = Common.Localize("Level:MenuName", false, new object[] { value });
            }
        }
    }
}
