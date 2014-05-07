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

namespace NRaas.CupcakeSpace.Options
{
    public class SetGlobalRandomRestockSettings : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public SetGlobalRandomRestockSettings()
        { }

        public override string GetTitlePrefix()
        {
            return "SetGlobalRandomRestockSettings";
        }        

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            RecipeSelection recipeSelection = RecipeSelection.Create(new List<string>(Cupcake.Settings.mRandomRestockSettings.Keys));
            OptionResult recipeResult = recipeSelection.Perform();

            if (recipeResult != OptionResult.SuccessClose)
            {
                return OptionResult.Failure;
            }
            
            QualitySelection qualitySelection = QualitySelection.Create(new List<Quality>(Cupcake.Settings.RandomRestockQualitiesAsList()));
            OptionResult qualityResult = qualitySelection.Perform();

            if (qualityResult != OptionResult.SuccessClose)
            {
                return OptionResult.Failure;
            }

            foreach (Recipe recipe in recipeSelection.selectedItems)
            {
                Cupcake.Settings.AddRandomRestockRecipe(recipe.Key, qualitySelection.selectedItems);
            }           

            Common.Notify(Common.Localize("General:Success"));

            return OptionResult.SuccessClose;
        }
    }
}