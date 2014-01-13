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
    public class CASDresserClothingEx
    {
        public static void OnSimOutfitIndexChanged(int index)
        {
            try
            {
                CASDresserClothing ths = CASDresserClothing.gSingleton;
                if (ths == null) return;

                foreach (Button button in ths.mOutfitButtons)
                {
                    button.Selected = false;
                }

                /*
                ICASModel cASModel = Responder.Instance.CASModel;
                if (ths.mDefaultText != null)
                {
                    if ((index == 0x0) && (cASModel.OutfitCategory == OutfitCategories.Career))
                    {
                        ths.mDefaultText.Visible = true;
                    }
                    else
                    {
                        ths.mDefaultText.Visible = false;
                    }
                }
                */

                // Custom
                if (index < ths.mOutfitButtons.Length)
                {
                    ths.mOutfitButtons[index].Selected = true;
                }

                CASController.Singleton.Activate(true);
                ths.UpdateOutfitButtons(Responder.Instance.CASModel.OutfitCategory);
                ths.UpdateOutfitDeleteButtons(index);
                if (CASPuck.Instance != null)
                {
                    CASPuck.Instance.OnDynamicUpdateCurrentSimThumbnail();
                }
                }
            catch (Exception e)
            {
                Common.Exception("OnOutfitButtonMouseDown", e);
            }
        }

        public static void OnOutfitButtonMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                CASDresserClothing dresser = CASDresserClothing.gSingleton;
                if (dresser == null) return;

                if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                {
                    Task.Perform();
                }
                else
                {
                    dresser.OnOutfitButtonClick(sender, new UIButtonClickEventArgs());
                }

                eventArgs.Handled = true;
            }
            catch (Exception e)
            {
                Common.Exception("OnOutfitButtonMouseDown", e);
            }
        }

        public class Task : Common.FunctionTask
        {
            protected Task()
            { }

            public static void Perform()
            {
                new Task().AddToSimulator();
            }

            protected override void OnPerform()
            {
                List<OutfitBase.Item> allOptions = new List<OutfitBase.Item>();

                ICASModel casModel = Responder.Instance.CASModel;

                ArrayList outfits = casModel.GetOutfits(casModel.OutfitCategory);

                for (int i = 0; i < outfits.Count; i++)
                {
                    allOptions.Add(new OutfitBase.Item(new CASParts.Key(casModel.OutfitCategory, i), casModel.CurrentSimDescription as SimDescription));
                }

                OutfitBase.Item choice = new CommonSelection<OutfitBase.Item>(Common.Localize("ChangeOutfit:MenuName"), allOptions).SelectSingle();
                if (choice == null) return;

                casModel.OutfitIndex = choice.Index;
            }
        }
    }
}
