using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using Sims3.Store.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class MassageTableGetMassage : MassageTable.GetMassage, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<MassageTable, MassageTable.GetMassage.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public static void ChangeSimToTowelOutfit(Sim s, bool romantic)
        {
            if ((s.CurrentOutfitCategory != OutfitCategories.Singed) && !(s.Service is GrimReaper))
            {
                if ((romantic) && (Woohooer.Settings.mNakedOutfitMassage))
                {
                    s.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToBathe);
                }
                else
                {
                    SimOutfit outfit = null;
                    SimDescription simDescription = s.SimDescription;
                    ResourceKey key = ResourceKey.CreateOutfitKey(OutfitUtils.GetAgePrefix(simDescription.Age, true) + OutfitUtils.GetGenderPrefix(simDescription.Gender) + "MassageTowel", 0x2000000);
                    outfit = OutfitUtils.CreateOutfitForSim(simDescription, key, OutfitCategories.SkinnyDippingTowel, true);
                    if (outfit != null)
                    {
                        s.SwitchToOutfitWithSpin(outfit.Key);
                    }
                    else
                    {
                        // Custom
                        s.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.Force, OutfitCategories.Swimwear);
                    }
                }
            }
        }

        public override bool Run()
        {
            try
            {
                if (!Actor.RouteToSlot(Target, Slot.RoutingSlot_1))
                {
                    return false;
                }
                Actor.ClearExitReasons();
                if (!StartSync(false))
                {
                    return false;
                }

                // Custom
                ChangeSimToTowelOutfit(Actor, ((mMassInfo.mType == MassageTable.MassageType.Romantic) || (mMassInfo.mType == MassageTable.MassageType.AmazingRomantic)));

                Actor.LoopIdle();
                Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                Actor.ClearExitReasons();
                if (!Actor.WaitForSynchronizationLevelWithSim(mMasseuse, Sim.SyncLevel.Routed, 60f))
                {
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();
                WaitForMasterInteractionToFinish();
                EndCommodityUpdates(true);
                StandardExit();
                WaitForSyncComplete();
                MassageTable.ChangeSimToEverydayOutfit(Actor);
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : MassageTable.GetMassage.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new MassageTableGetMassage();
                result.Init(ref parameters);
                return result;
            }
        }
    }
}
