using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.CupcakeSpace.Helpers;
using NRaas.CupcakeSpace.Options.Displays;
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
    public class CopySettingsToDisplay : OperationSettingOption<GameObject>, ICaseOption
    {
        IGameObject mTarget;

        public CopySettingsToDisplay()
        { }

        public override string GetTitlePrefix()
        {
            return "CopySettingsToDisplay";
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

            if (!Cupcake.Settings.HasSettings(Cupcake.activeDisplay.ObjectId))
            {
                Common.Notify(Common.Localize("SlotCopy:NoSettings"));
                return OptionResult.Failure;
            }

            if (Cupcake.Settings.HasSettings(mTarget.ObjectId))
            {
                Cupcake.Settings.RemoveDisplaySettings(mTarget.ObjectId);                
            }

            Cupcake.Settings.SetDisplaySettings(mTarget.ObjectId, Cupcake.Settings.GetDisplaySettings(Cupcake.activeDisplay.ObjectId));

            Common.Notify(Common.Localize("General:Success"));

            return OptionResult.SuccessClose;
        }             
    }
}