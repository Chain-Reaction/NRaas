using NRaas.CommonSpace.Booters;
using NRaas.OverwatchSpace.Helpers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;

namespace NRaas.OverwatchSpace.Startup
{
    public class LoadStoreData : StartupOption
    {
        public LoadStoreData()
        { }

        public override void OnStartupApp()
        {
            Overwatch.Log("LoadStoreData");

            XmlDbData data = XmlDbData.ReadData(new ResourceKey(ResourceUtils.HashString64("buffs_store"), 0xdd3223a7, 0), false);
            if (data != null)
            {
                try
                {
                    BuffManager.ParseBuffData(data, true);

                    Overwatch.Log(" buffs_store");
                }
                catch (Exception e)
                {
                    Common.DebugException("buffs_store", e);
                }
            }

            data = XmlDbData.ReadData(new ResourceKey(ResourceUtils.HashString64("skills_store"), 0xa8d58be5, 0), false);
            if (data != null)
            {
                try
                {
                    SkillManager.ParseSkillData(data, true);

                    Overwatch.Log(" skills_store");
                }
                catch (Exception e)
                {
                    Common.DebugException("skills_store", e);
                }
            }

            data = XmlDbData.ReadData(new ResourceKey(ResourceUtils.HashString64("Ingredients_store"), 0xe5105066, 0), false);
            if (data != null)
            {
                try
                {
                    IngredientData.LoadIngredientData(data, true);

                    Overwatch.Log(" Ingredients_store");
                }
                catch (Exception e)
                {
                    Common.DebugException("Ingredients_store", e);
                }
            }

            data = XmlDbData.ReadData(new ResourceKey(ResourceUtils.HashString64("Plants_store"), 0xe5105068, 0), false);
            if (data != null)
            {
                try
                {
                    PlantDefinition.ParsePlantDefinitionData(data, true);

                    Overwatch.Log(" Plants_store");
                }
                catch (Exception e)
                {
                    Common.DebugException("Plants_store", e);
                }
            }

            data = XmlDbData.ReadData(new ResourceKey(ResourceUtils.HashString64("RecipeMasterList_store"), 0xe5105067, 0), false);
            if (data != null)
            {
                try
                {
                    Recipe.LoadRecipeData(data, true);

                    Overwatch.Log(" RecipeMasterList_store");
                }
                catch (Exception e)
                {
                    Common.DebugException("RecipeMasterList_store", e);
                }
            }
        }
    }
}
