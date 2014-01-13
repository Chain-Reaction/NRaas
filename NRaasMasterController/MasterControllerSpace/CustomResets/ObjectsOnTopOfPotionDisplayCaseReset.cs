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
    public class ObjectsOnTopOfPotionDisplayCaseReset : GenericCustomReset<GameObject>
    {
        protected override bool PrivatePerform(GameObject obj)
        {
            MetalMineralPotionDisplayCase parent = obj.Parent as MetalMineralPotionDisplayCase;
            if (parent == null) return false;

            if (parent.Inventory == null) return false;

            List<InventoryItem> list = parent.Inventory.DestroyInventoryAndStoreInList();

            obj.SetObjectToReset();

            Inventories.RestoreInventoryFromList(parent.Inventory, list, true);

            return true;
        }
    }
}
