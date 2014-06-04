using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    public class CommonSocials
    {
        public static void OnTrickOrTreatAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            IGameObject trickOrTreatItem = null;
            if (actor.OccultManager != null)
            {
                if ((actor.OccultManager.HasAnyOccultType() || actor.SimDescription.IsAlien) && RandomUtil.RandomChance(Tempest.Settings.mChanceOccultItemTrickOrTreat))
                {
                    List<OccultTypes> types = OccultTypeHelper.CreateList(actor.SimDescription);
                    OccultTypes pick = OccultTypes.None;
                    bool useAlien = true;
                    if (types.Count > 0)
                    {
                        pick = RandomUtil.GetRandomObjectFromList<OccultTypes>(types);
                        useAlien = actor.SimDescription.IsAlien && RandomUtil.CoinFlip();
                    }
                    trickOrTreatItem = FetchRandomOccultTreat(pick, useAlien);

                    if (trickOrTreatItem is FailureObject)
                    {
                        trickOrTreatItem = null;
                    }
                    else
                    {
                        if (RandomUtil.CoinFlip())
                        {
                            if (actor.OccultManager.HasOccultType(OccultTypes.PlantSim) && GameUtils.IsInstalled(ProductVersion.EP10) && Sim.MosquitoBeBitten.kChanceToKillMosquito != 100 && !target.BuffManager.HasAnyElement(new BuffNames[] { BuffNames.MosquitoBiteLow, BuffNames.MosquitoBiteMid, BuffNames.MosquitoBiteHigh }))
                            {
                                target.BuffManager.AddElement(BuffNames.MosquitoBiteLow, Origin.FromFallHoliday);
                            }
                            else if (actor.OccultManager.HasOccultType(OccultTypes.Werewolf) && GameUtils.IsInstalled(ProductVersion.EP5) && BuffGotFleasHuman.kPetToHumanSpreadFleasChance != 0 && !target.BuffManager.HasElement(BuffNames.GotFleasHuman))
                            {
                                target.BuffManager.AddElement(BuffNames.GotFleasHuman, Origin.FromFallHoliday);
                            }
                        }

                    }
                }                
            }

            if (trickOrTreatItem == null)
            {
                trickOrTreatItem = TrickOrTreatSituation.GetTrickOrTreatItem();
            }

            if (trickOrTreatItem != null)
            {
                if (!target.Inventory.TryToAdd(trickOrTreatItem))
                {
                    trickOrTreatItem.Destroy();
                }
                else if (!(trickOrTreatItem is Candy))
                {
                    target.ShowTNSIfSelectable(Localization.LocalizeString(target.IsFemale, "Gameplay/Situation/TrickorTreat:SpecialObjectTNS", new object[] { trickOrTreatItem }), StyledNotification.NotificationStyle.kSimTalking, target.ObjectId, trickOrTreatItem.ObjectId);
                    if (trickOrTreatItem is IMagicGnomeFall)
                    {
                        EventTracker.SendEvent(EventTypeId.kReceivedSeasonalGnome);
                    }
                }
            }
            EventTracker.SendEvent(EventTypeId.kWentTrickOrTreating, target);
        }

        public static IGameObject FetchRandomOccultTreat(OccultTypes type, bool isAlien)
        {
            if (isAlien)
            {

                InteractionDefinition instance = RabbitHole.StealSpaceRocks.Singleton;
                RabbitHole.StealSpaceRocks instance2 = instance as RabbitHole.StealSpaceRocks;
                if (instance2 != null)
                {
                    RabbitHole.StealSpaceRocks.SpaceRockSize weightedIndex = (RabbitHole.StealSpaceRocks.SpaceRockSize)RandomUtil.GetWeightedIndex(instance2.SSRTuning.SpaceRockWeights);
                    RockGemMetal spaceRockSize = RockGemMetal.SpaceRockSmall;
                    switch (weightedIndex)
                    {
                        case RabbitHole.StealSpaceRocks.SpaceRockSize.Small:
                            spaceRockSize = RockGemMetal.SpaceRockSmall;
                            break;
                        case RabbitHole.StealSpaceRocks.SpaceRockSize.Medium:
                            spaceRockSize = RockGemMetal.SpaceRockMedium;
                            break;
                        case RabbitHole.StealSpaceRocks.SpaceRockSize.Large:
                            spaceRockSize = RockGemMetal.SpaceRockLarge;
                            break;
                    }

                    return RockGemMetalBase.Make(spaceRockSize, false);
                }
            }            

            switch(type)
            {                
                case OccultTypes.Fairy:
                    PlantRarity rarity = (PlantRarity) RandomUtil.GetInt(0, 3);
                    Quality quality = (Quality) RandomUtil.GetInt(4, 9);
                    PlayerDisclosure playerKnowledgeOfPlantableType = (rarity == PlantRarity.Common) ? PlayerDisclosure.Exposed : PlayerDisclosure.Concealed;
                    IGameObject seed = PlantHelper.CreateRandomPlantable(rarity, quality, true, playerKnowledgeOfPlantableType);

                    return seed;                   
                case OccultTypes.Frankenstein:
                    ScrapInitParameters initData = new ScrapInitParameters(1);
                    IGameObject scrap = GlobalFunctions.CreateObjectOutOfWorld("scrapPile", ProductVersion.EP2, null, initData);
                    return scrap;
                case OccultTypes.Genie:
                    IGameObject lamp = GlobalFunctions.CreateObjectOutOfWorld("GenieLamp", ProductVersion.EP6);
                    return lamp;
                case OccultTypes.Mermaid:                    
                    IGameObject kelp = MermadicKelp.MakeMermadicKelp(RandomUtil.CoinFlip());
                    return kelp;
                case OccultTypes.PlantSim:
                    string randomHerb = RandomUtil.GetRandomObjectFromList<string>(new List<string>(Herb.sIngredientToHerbDataMap.Keys));
                    Ingredient herb = Ingredient.Create(IngredientData.NameToDataMap[randomHerb]);
                    return herb as IGameObject;
                case OccultTypes.Robot:
                    TraitChipStaticData data = RandomUtil.GetRandomObjectFromDictionary<ulong, TraitChipStaticData>(GenericManager<TraitChipName, TraitChipStaticData, TraitChip>.sDictionary);
                    if (data != null)
                    {
                        TraitChip chip = TraitChipManager.CreateTraitChip(data.Guid);
                        return chip;
                    }
                    return null;
                case OccultTypes.Vampire:
                    IGameObject vampireTreat = null;
                    if (!GameUtils.IsInstalled(ProductVersion.EP3))
                    {
                        vampireTreat = Recipe.NameToRecipeHash["VampireJuiceEP7"].CreateFinishedFood(Recipe.MealQuantity.Single, Quality.Perfect);
                    }
                    else
                    {
                        bool coinToss = RandomUtil.CoinFlip();
                        if (coinToss)
                        {
                            vampireTreat = Recipe.NameToRecipeHash["VampireJuice"].CreateFinishedFood(Recipe.MealQuantity.Single, Quality.Perfect);
                        }
                        else
                        {
                            vampireTreat = Ingredient.Create(IngredientData.NameToDataMap["VampireFruit"]);
                        }
                    }
                    return vampireTreat;
                case OccultTypes.Werewolf:
                    List<FishType> fish = new List<FishType>();
                    List<float> weights = new List<float>(fish.Count);
                    foreach (FishType fishType in Fish.sFishData.Keys)
                    {
                        FishData data2 = Fish.sFishData[fishType];
                        if (((data2.IngredientData != null) && data2.IngredientData.CanBuyFromStore) && (data2.Level >= 0))
                        {
                            fish.Add(fishType);
                            weights.Add(1f);
                        }
                    }
                    int weightedIndexFish = RandomUtil.GetWeightedIndex(weights.ToArray());
                    Fish obj2 = Fish.CreateFishOfRandomWeight(fish[weightedIndexFish]);
                    return obj2 as IGameObject;
                case OccultTypes.Witch:
                    if (RandomUtil.CoinFlip())
                    {
                        ISoloInteractionDefinition cdef = TraitFunctions.ConjureApple.Singleton;
                        TraitFunctions.ConjureApple appleDef = cdef as TraitFunctions.ConjureApple;
                        if (appleDef != null)
                        {
                            return appleDef.CreateAppleForInventory(RandomUtil.CoinFlip(), Quality.Perfect);
                        }
                    }
                    else
                    {
                        AlchemyPotion potion = AlchemyPotion.CreateARandomPotion(RandomUtil.CoinFlip(), 0);
                        return potion as IGameObject;
                    }
                    return null;
                case OccultTypes.None:
                default:
                    return null;                    
            }           
        }
    }
}
