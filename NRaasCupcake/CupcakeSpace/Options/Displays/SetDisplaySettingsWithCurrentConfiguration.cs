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
    public class SetDisplaySettingsWithCurrentConfiguration : OperationSettingOption<GameObject>, ICaseOption
    {
        IGameObject mTarget;

        public SetDisplaySettingsWithCurrentConfiguration()
        { }

        public override string GetTitlePrefix()
        {
            return "SetDisplaySettingsWithCurrentConfiguration";
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
            Cupcake.Settings.RemoveDisplaySettings(mTarget.ObjectId);

            DisplayHelper.DisplayTypes displayType;
            Dictionary<int, Slot> containmentSlots = DisplayHelper.GetEmptyOrFoodSlots(mTarget as CraftersConsignmentDisplay, out displayType);

            foreach (KeyValuePair<int, Slot> displaySlots in containmentSlots)
            {
                Quality quality;
                string recipeKey = DisplayHelper.ExtractRecipeKeyFromSlot(mTarget as CraftersConsignmentDisplay, displaySlots.Key, displaySlots.Value, out quality);

                if (recipeKey != null)
                {
                    Cupcake.Settings.AddDisplayRecipe(mTarget.ObjectId, new List<int> { displaySlots.Key }, recipeKey, false);
                    Cupcake.Settings.SetRecipeQuality(mTarget.ObjectId, new List<int> { displaySlots.Key }, quality, false);
                }
            }

            Common.Notify(Common.Localize("General:Success"));

            return OptionResult.SuccessClose;
        }        
    }
}