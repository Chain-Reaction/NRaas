using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class InventoryEx : SelectionTestableOptionList<InventoryEx.Item, IGameObject, string>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Inventory";
        }

        public class Item : TestableOption<IGameObject, string>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<string,IGameObject> results)
            {
                if (me.CreatedSim == null) return false;

                if (me.CreatedSim.Inventory == null) return false;

                foreach (GameObject obj in Inventories.QuickFind<GameObject>(me.CreatedSim.Inventory))
                {
                    results[obj.GetLocalizedName()] = obj;
                }

                return true;
            }

            public override void SetValue(IGameObject dataType, string storeType)
            {
                mValue = storeType;

                mName = storeType;

                mThumbnail = dataType.GetThumbnailKey();
            }
        }
    }
}
