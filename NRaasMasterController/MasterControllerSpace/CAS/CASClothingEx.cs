using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASClothingEx
    {
        public static void OnSimOutfitCategoryChanged(OutfitCategories outfitCategory)
        {
            try
            {
                CASClothingState state = CASClothing.CASClothingStateFromOutfitCategory(outfitCategory);

                if (CASPuck.Instance != null)
                {
                    CASPuck.Instance.OnDynamicUpdateCurrentSimThumbnail();
                }

                CASController controller = CASController.Singleton;
                if (controller != null)
                {
                    controller.SetCurrentState(new CASState(CASTopState.CreateASim, CASMidState.Clothing, CASPhysicalState.None, state));
                    controller.Activate(true);
                }

                CASClothing ths = CASClothing.gSingleton;
                if (ths != null)
                {
                    ths.UpdateState(state);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimOutfitCategoryChanged", e);
            }
        }
    }
}
