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
    public class SetDisplaySettings : OperationSettingOption<GameObject>, ICaseOption
    {
        IGameObject mTarget;

        public SetDisplaySettings()
        { }

        public override string GetTitlePrefix()
        {
            return "SetDisplaySettings";
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
            SlotSelection slotSelection = SlotSelection.Create(mTarget as CraftersConsignmentDisplay);
            OptionResult result = slotSelection.Perform();

            if (result != OptionResult.SuccessClose)
            {
                return OptionResult.Failure;
            }

            RecipeSelection recipeSelection = RecipeSelection.Create(Cupcake.Settings.BuildSlotsWithRecipes(Cupcake.Settings.GetDisplaySettings(mTarget.ObjectId)));
            OptionResult recipeResult = recipeSelection.Perform();

            if (recipeResult != OptionResult.SuccessClose)
            {
                return OptionResult.Failure;
            }

            QualitySelection qualitySelection = QualitySelection.Create(Cupcake.Settings.BuildSlotsWithQualities(Cupcake.Settings.GetDisplaySettings(mTarget.ObjectId)));
            OptionResult qualityResult = qualitySelection.Perform();

            if (qualityResult != OptionResult.SuccessClose)
            {
                return OptionResult.Failure;
            }

            foreach (Recipe recipe in recipeSelection.selectedItems)
            {               
                Cupcake.Settings.AddDisplayRecipe(mTarget.ObjectId, slotSelection.selectedItems, recipe.Key, false);
            }           

            foreach (Quality quality in qualitySelection.selectedItems)
            {
                Cupcake.Settings.SetRecipeQuality(mTarget.ObjectId, slotSelection.selectedItems, quality, false);
            }

            Common.Notify(Common.Localize("General:Success"));
            
            return OptionResult.SuccessClose;
        }       
    }
}
