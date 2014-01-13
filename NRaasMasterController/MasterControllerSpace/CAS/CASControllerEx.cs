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
using Sims3.UI.CAS.CAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASControllerEx
    {
        public static void OnExitFullEditMode()
        {
            try
            {
                CASController ths = CASController.gSingleton;
                if (ths == null) return;

                ICASModel casModel = Responder.Instance.CASModel;
                if (casModel == null) return;

                bool reprocess = false;

                switch (casModel.CASMode)
                {
                    case CASMode.Full:
                        if ((casModel.CurrentSimDescription != null) && (casModel.CurrentSimDescription.IsHuman))
                        {
                            reprocess = true;
                        }
                        break;

                    case CASMode.Dresser:
                        reprocess = true;
                        break;

                    case CASMode.Mirror:
                        reprocess = true;
                        break;

                    case CASMode.Stylist:
                        reprocess = true;
                        break;
                }
                /*
                // Corrects for a bounce issue in CASDresserSheet:Init() with foreign language users
                if ((ths.mCurrUINode == null) && (casModel.CASMode == CASMode.Stylist) && (casModel.SelectionIndex != 0x1))
                {
                    if (CASDresserSheet.gSingleton == null)
                    {
                        CASDresserSheet.Load();
                        CASDresserSheet.gSingleton.UINodeShutdown += ths.SetState;

                        Window newWindow, oldWindow;
                        CASDresserSheetEx.GetWindows(out newWindow, out oldWindow);

                        CASDresserSheetEx.UpdateGlideEffects(newWindow);

                        CASDresserSheetEx.UpdatePanelGlide(newWindow);
                    }
                }
                */
                ths.OnExitFullEditMode();

                if (reprocess)
                {
                    CASClothingCategoryEx.DelayedCategoryUpdate.Perform();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnExitFullEditMode", e);
            }
        }
    }
}
