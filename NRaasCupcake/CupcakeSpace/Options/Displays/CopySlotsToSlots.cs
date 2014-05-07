using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.CupcakeSpace.Helpers;
using NRaas.CupcakeSpace.Options.Displays;
using NRaas.CupcakeSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace.Options.Displays
{
    public class CopySlotsToSlots : OperationSettingOption<GameObject>, ICaseOption
    {
        IGameObject mTarget;

        public CopySlotsToSlots()
        { }

        public override string GetTitlePrefix()
        {
            return "CopySlotsToSlots";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            if (mTarget == null) return false;

            if (!(mTarget is CraftersConsignmentDisplay)) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (Cupcake.activeDisplay == null)
            {
                Common.Notify(Common.Localize("SlotCopy:NoSelection"));
                return OptionResult.Failure;
            }

            if (Cupcake.activeDisplay == mTarget)
            {
                Common.Notify(Common.Localize("SlotCopy:SameDisplay"));
                return OptionResult.Failure;
            }

            DisplayHelper.DisplayTypes targetType = DisplayHelper.GetDisplayType(mTarget as CraftersConsignmentDisplay);
            DisplayHelper.DisplayTypes activeType = DisplayHelper.GetDisplayType(Cupcake.activeDisplay as CraftersConsignmentDisplay);

            if (targetType != activeType)
            {
                Common.Notify(Common.Localize("SlotCopy:TypeMismatch"));
                return OptionResult.Failure;
            }

            if(!Cupcake.Settings.HasSettings(Cupcake.activeDisplay.ObjectId))
            {
                Common.Notify(Common.Localize("SlotCopy:NoSettings"));
                return OptionResult.Failure;
            }

            SimpleMessageDialog.Show(Common.Localize("SlotCopy:MessageTitle"), Common.Localize("SlotCopy:MessageBody"));

            SlotSelection activeSlotSelection = SlotSelection.Create(Cupcake.activeDisplay as CraftersConsignmentDisplay);
            OptionResult result = activeSlotSelection.Perform();

            if (result != OptionResult.SuccessClose)
            {
                return OptionResult.Failure;
            }

            SlotSelection targetSlotSelection = null;            
            if (activeSlotSelection.selectedItems.Count == 1 && !activeSlotSelection.all)
            {
                targetSlotSelection = SlotSelection.Create(mTarget as CraftersConsignmentDisplay);
                OptionResult targetResult = targetSlotSelection.Perform();

                if (targetResult != OptionResult.SuccessClose)
                {
                    return OptionResult.Failure;
                }                
            }

            if (targetSlotSelection == null)
            {
                // selected multiple from active display, we are doing carbon copying
                foreach (int slot in activeSlotSelection.selectedItems)
                {
                    if (Cupcake.Settings.SlotHasSettings(Cupcake.activeDisplay.ObjectId, slot))
                    {
                        Cupcake.Settings.RemoveDisplaySettingsForSlot(mTarget.ObjectId, slot);
                        Cupcake.Settings.SetDisplaySettingsForSlot(mTarget.ObjectId, Cupcake.Settings.GetDisplaySettingsForSlot(Cupcake.activeDisplay.ObjectId, slot), slot);
                    }
                }
            }
            else
            {
                // selected single from active display, we are doing multiple copy to target display
                foreach (int slot in targetSlotSelection.selectedItems)
                {
                    if (Cupcake.Settings.SlotHasSettings(Cupcake.activeDisplay.ObjectId, activeSlotSelection.selectedItems[0]))
                    {
                        Cupcake.Settings.RemoveDisplaySettingsForSlot(mTarget.ObjectId, slot);
                        Cupcake.Settings.SetDisplaySettingsForSlot(mTarget.ObjectId, Cupcake.Settings.GetDisplaySettingsForSlot(Cupcake.activeDisplay.ObjectId, activeSlotSelection.selectedItems[0]), slot);
                    }
                }
            }

            Common.Notify(Common.Localize("General:Success"));

            return OptionResult.SuccessClose;
        }        
    }
}