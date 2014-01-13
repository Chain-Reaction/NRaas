using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CustomResets
{
    public class MetalMineralPotionDisplayCaseReset : GenericCustomReset<MetalMineralPotionDisplayCase>
    {
        protected override bool PrivatePerform(MetalMineralPotionDisplayCase obj)
        {
            if (obj.Inventory != null)
            {
                List<InventoryItem> list = obj.Inventory.DestroyInventoryAndStoreInList();

                obj.SetObjectToReset();

                Inventories.RestoreInventoryFromList(obj.Inventory, list, true);
            }

            return true;
        }
    }
}
