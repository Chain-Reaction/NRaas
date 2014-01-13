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
    public class CASLoadSimEx
    {
        public static void RequestLoadSim(ResourceKey sim)
        {
            CASLogic.CASOperationStack.Instance.Push(new SetSimDescOperationEx(sim));
        }
        public static void RequestLoadSim(ISimDescription simDesc, bool inHousehold)
        {
            SimDescription newSimDesc = simDesc as SimDescription;
            if (newSimDesc.FavoriteMusic == FavoriteMusicType.Custom)
            {
                Array installedFavoriteMusicList = CASCharacter.GetInstalledFavoriteMusicList();
                while (newSimDesc.mFavouriteMusicType == FavoriteMusicType.Custom)
                {
                    newSimDesc.mFavouriteMusicType = (FavoriteMusicType)installedFavoriteMusicList.GetValue(RandomUtil.GetInt(0x1, installedFavoriteMusicList.Length - 0x1));
                }
            }
            CASLogic.CASOperationStack.Instance.Push(new SetSimDescOperationEx(newSimDesc, inHousehold));
        }

        public static void OnSimGridClicked(ItemGrid sender, ItemGridCellClickEvent itemClicked)
        {
            try
            {
                CASLoadSim ths = CASLoadSim.gSingleton;
                if (ths == null) return;

                ICASModel cASModel = Responder.Instance.CASModel;
                if (ths.mLoading)
                {
                    ths.mSimGrid.SelectedItem = ths.mPreviousSelection;
                }
                else
                {
                    if (itemClicked.mTag is CASLoadSim.SimDescriptionKeyPair)
                    {
                        CASLoadSim.SimDescriptionKeyPair tag = (CASLoadSim.SimDescriptionKeyPair)itemClicked.mTag;
                        if (tag.mSimDesc.IsValid)
                        {
                            ths.mCurrSimDescKeyPair = tag;
                            
                            // Custom
                            RequestLoadSim(tag.mSimDesc, false);

                            ths.SetCustomButtonStates();
                            ths.UpdateTooltips(tag.mSimDesc.Species);
                        }
                    }

                    ths.mPreviousSelection = sender.SelectedItem;
                    ths.mLoading = true;
                    UIManager.SetOverrideCursor(0x1003);
                    Audio.StartSound("ui_secondary_button");
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimGridClicked", e);
            }
        }
    }
}
