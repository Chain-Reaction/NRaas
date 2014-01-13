using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckPlantLargeTree : Check<PlantLargeTree>
    {
        protected override bool PrePerform(PlantLargeTree plant, bool postLoad)
        {
            return true;
        }

        protected override bool PostPerform(PlantLargeTree plant, bool postLoad)
        {
            if (!ErrorTrap.Loading) return true;

            if (!plant.IsDataValid())
            {
                if (plant.Seed == null)
                {
                    Ingredient ingredient;
                    if (CheckIngredient.sIngredients.TryGetValue(plant.Position, out ingredient))
                    {
                        CheckIngredient.sIngredients.Remove(plant.Position);

                        CheckIngredient.MergePlantSeed(plant, ingredient);
                    }
                    else
                    {
                        CheckIngredient.sMissingPlants[plant.Position] = plant;

                       // DebugLogCorrection("Potential Plant: " + ErrorTrap.GetName(plant));
                    }
                }
            }

            return true;
        }
    }
}
