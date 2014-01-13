using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;

namespace NRaas.CommonSpace.Helpers
{
    public class OccultPlantSimEx
    {
        public static void OnRemoval(OccultPlantSim ths, SimDescription simDes, bool alterOutfit)
        {
            Sim createdSim = simDes.CreatedSim;
            int index = simDes.HasOutfit(OutfitCategories.Everyday, ths.mOccultOutfitKey);
            if (index >= 0x0)
            {
                simDes.RemoveOutfit(OutfitCategories.Everyday, index, true);
            }
            if (createdSim != null)
            {
                createdSim.Motives.RemoveMotive(CommodityKind.BePlantSim);
                createdSim.Motives.EnableMotive(CommodityKind.Bladder);
                createdSim.Motives.EnableMotive(CommodityKind.Hunger);
                createdSim.BuffManager.RemoveElement(BuffNames.PlantSimDehydrated);
                createdSim.BuffManager.RemoveElement(BuffNames.PlantSimHydrated);
                createdSim.TraitManager.RemoveElement(TraitNames.PlantSim);
                ActiveTopic.RemoveTopicFromSim(createdSim, "PlantSim Trait Topic");
                createdSim.Motives.UpdateMotives(1f, createdSim);
                (createdSim.RoutingComponent as SimRoutingComponent).RemoveOccultWalkingEffects(OccultTypes.PlantSim);
                ResourceKey skinToneKey = new ResourceKey(0xe8a7d5f0fec3d90bL, 0x354796a, 0x0);
                if (ths.mOldSkinToneKey != ResourceKey.kInvalidResourceKey)
                {
                    skinToneKey = ths.mOldSkinToneKey;
                }

                if (alterOutfit)
                {
                    simDes.SetSkinAndHairForAllOutfits(simDes.Age, skinToneKey, ths.mOldSkinIndex, ResourceKey.kInvalidResourceKey, true, ths.mOldHairColor);
                    simDes.RemoveBodyTypeFromAllOutfits(BodyTypes.BirthMark);
                    simDes.RemoveBodyTypeFromAllOutfits(BodyTypes.Beard);
                }

                HudModel hudModel = Responder.Instance.HudModel as HudModel;
                if ((hudModel != null) && (hudModel.GetSimInfo(createdSim.ObjectId) != null))
                {
                    if (alterOutfit)
                    {
                        SimBuilder builder = new SimBuilder();
                        builder.Clear();
                        SimOutfit outfit = simDes.GetOutfit(OutfitCategories.Everyday, 0x0);
                        OutfitUtils.SetOutfit(builder, outfit, null);
                        OutfitUtils.ExtractOutfitHairColor(simDes, builder);
                        ths.mOutfitIsReady = false;
                        createdSim.SwitchToOutfitDelayed(outfit, new CASUtils.SwapReadyCallback(ths.PlayConcealEffectAndSwapOutfit));
                        for (int i = 0x0; !ths.mOutfitIsReady && (i < 0xc350); i++)
                        {
                            SpeedTrap.Sleep(0xa);
                        }
                        ths.SwapOutfit();
                    }

                    hudModel.OnSimAppearanceChanged(createdSim.ObjectId);
                    hudModel.NotifyNameChanged(createdSim.ObjectId);
                }
                else
                {
                    createdSim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.Force, createdSim.CurrentOutfitCategory, true);
                }

                createdSim.Motives.RemoveMotive(CommodityKind.BePlantSim);
                createdSim.Motives.EnableMotive(CommodityKind.Bladder);
                createdSim.Motives.EnableMotive(CommodityKind.Hunger);
                createdSim.BuffManager.RemoveElement(BuffNames.PlantSimDehydrated);
                createdSim.BuffManager.RemoveElement(BuffNames.PlantSimHydrated);
                createdSim.TraitManager.RemoveElement(TraitNames.PlantSim);
                ActiveTopic.RemoveTopicFromSim(createdSim, "PlantSim Trait Topic");
                hudModel.NotifyNameChanged(createdSim.ObjectId);
                createdSim.Motives.UpdateMotives(1f, createdSim);
            }
        }
    }
}
