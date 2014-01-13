using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MoverSpace.Helpers
{
    public class EditTownMergeToolEx : EditTownMergeTool
    {
        private new void OnSelection(UIBinInfo info)
        {
            if (((mFrom != null) && (mFrom != info)) && (info.HouseholdId != ulong.MaxValue))
            {
                mModel.CenterCamera(info.LotId);
                GameEntryMovingModelEx.MergeHouseholds(EditTownController.Instance, mFrom, info);
            }
            else
            {
                Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(WarnCantMergeHouseholds), info));
            }
        }

        public override void OnMaptagSelection(ulong id, MaptagTypes maptagType, Vector2 mouseClickScreen)
        {
            try
            {
                UIBinInfo info = FindBinInfoForLot(id, maptagType);
                if (info != null)
                {
                    OnSelection(info);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMaptagSelection", e);
            }
        }

        public override bool OnLotPick(ulong id)
        {
            try
            {
                UIBinInfo info = FindBinInfoForLot(id, MaptagTypes.None);
                if (info != null)
                {
                    OnSelection(info);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Common.Exception("OnLotPick", e);
                return false;
            }
        }
    }
}
