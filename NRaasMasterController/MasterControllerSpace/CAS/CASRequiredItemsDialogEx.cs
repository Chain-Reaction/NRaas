using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.Sims;
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
using Sims3.UI.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASRequiredItemsDialogEx
    {
        public static void OnRandomizeNameClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASRequiredItemsDialog dialog = CASRequiredItemsDialog.sDialog;
                if (dialog != null)
                {
                    dialog.OnRandomizeNameClick(sender, eventArgs);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnRandomizeNameClick", e);
            }
        }

        public static void OnNameTextEditChange(WindowBase sender, UITextChangeEventArgs eventArgs)
        {
            try
            {
                CASRequiredItemsDialog dialog = CASRequiredItemsDialog.sDialog;
                if (dialog != null)
                {
                    dialog.OnNameTextEditChange(sender, eventArgs);
                }

                ICASModel cASModel = Responder.Instance.CASModel;
                if (cASModel != null)
                {
                    CASBase.ChangeName(cASModel.FirstName, cASModel.LastName);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnNameTextEditChange", e);
            }
        }
    }
}
