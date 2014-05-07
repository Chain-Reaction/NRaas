using NRaas.MasterControllerSpace.Demographics;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.RealEstate;
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
    public class BotQuality : SelectionTestableOptionList<BotQuality.Item, int, int>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.BotQuality";
        }

        public class Item : TestableOption<int, int>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<int, int> results)
            {
                if (me.IsEP11Bot)
                {
                    int quality = 0;
                    CASRobotData supernaturalData = me.SupernaturalData as CASRobotData;
                    if (supernaturalData != null)
                    {
                        quality = supernaturalData.BotQualityLevel;
                    }

                    results[quality] = quality;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override void SetValue(int value, int storeType)
            {
                mValue = value;

                mName = Common.Localize("Level:MenuName", false, new object[] { value }); ;
            }
        }
    }
}