using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.Sims.Basic;
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
    public class CASDresserSheetEx
    {
        /*
        public static void OnNavigationButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASDresserSheet ths = CASDresserSheet.gSingleton;
                if (ths == null) return;

                Window newWindow, oldWindow;
                CASDresserSheetEx.GetWindows(out newWindow, out oldWindow);

                CASDresserSheetEx.UpdateGlideEffects(newWindow);

                CASDresserSheetEx.UpdatePanelGlide(newWindow);

                ths.OnNavigationButtonClick(sender, eventArgs);
            }
            catch (Exception e)
            {
                Common.Exception("OnNavigationButtonClick", e);
            }
        }
        */
        public static void UpdatePanelGlide(Window newWindow)
        {
            CASDresserSheet ths = CASDresserSheet.gSingleton;
            if (ths == null) return;

            newWindow.EffectFinished -= ths.OnPanelGlideFinished;
            newWindow.EffectFinished += ths.OnPanelGlideFinished;
        }

        public static void UpdateGlideEffects(Window newWindow)
        {
            CASDresserSheet ths = CASDresserSheet.gSingleton;
            if (ths == null) return;

            foreach (object effect in newWindow.EffectList)
            {
                ths.mGlideEffect = effect as GlideEffect;
                if (ths.mGlideEffect != null)
                {
                    ths.mGlideTime = ths.mGlideEffect.Duration;
                    break;
                }
            }

            foreach (object effect in ths.EffectList)
            {
                ths.mMainGlideEffect = effect as GlideEffect;
                if (ths.mMainGlideEffect != null)
                {
                    ths.mMainGlideOffset = ths.mMainGlideEffect.Offset.x;
                    break;
                }
            }
        }

        public static bool GetWindows(out Window newWindow, out Window oldWindow)
        {
            CASDresserSheet ths = CASDresserSheet.gSingleton;
            if (ths == null)
            {
                newWindow = null;
                oldWindow = null;
                return false;
            }

            CASDresserSheet.ControlIDs newDS, oldDS;
            if (StringTable.GetLocale() != "en-us")
            {
                newDS = CASDresserSheet.ControlIDs.BackPanelBigLong;
                oldDS = CASDresserSheet.ControlIDs.BackPanelBig;
            }
            else
            {
                newDS = CASDresserSheet.ControlIDs.BackPanelSmallLong;
                oldDS = CASDresserSheet.ControlIDs.BackPanelSmall;
            }

            newWindow = ths.GetChildByID((uint)newDS, true) as Window;
            oldWindow = ths.GetChildByID((uint)oldDS, true) as Window;
            return true;
        }
    }
}
