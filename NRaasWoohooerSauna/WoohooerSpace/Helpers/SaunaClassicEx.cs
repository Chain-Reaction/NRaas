using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Situations;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Helpers
{
    public class SaunaClassicEx
    {
        public static bool StateMachineEnterAndSit(SaunaClassic ths, bool forWoohoo, StateMachineClient smc, SittingPosture sitPosture, Slot routingSlot, object sitContext)
        {
            /*
            if (sitPosture.Sim.SimDescription.IsVisuallyPregnant)
            {
                ThoughtBalloonManager.BalloonData bd = null;
                bd = new ThoughtBalloonManager.BalloonData("balloon_moodlet_pregnant");
                if (bd != null)
                {
                    sitPosture.Sim.ThoughtBalloonManager.ShowBalloon(bd);
                }
                return false;
            }
            */
            if ((sitPosture.Sim.CarryingChildPosture != null) || (sitPosture.Sim.CarryingPetPosture != null))
            {
                return false;
            }

            if (!sitPosture.Sim.HasTrait(TraitNames.NeverNude))
            {
                bool change = false;
                if ((Woohooer.Settings.mNakedOutfitSaunaGeneral) || ((forWoohoo) && (Woohooer.Settings.mNakedOutfitSaunaWoohoo)))
                {
                    if (sitPosture.Sim.SimDescription.Teen)
                    {
                        if (Woohooer.Settings.mAllowTeenWoohoo)
                        {
                            change = true;
                        }
                    }
                    else if (sitPosture.Sim.SimDescription.YoungAdultOrAbove)
                    {
                        change = true;
                    }
                }

                if (change)
                {
                    sitPosture.Sim.SwitchToOutfitWithSpin(OutfitCategories.Naked, 0);

                    Woohooer.Settings.AddChange(sitPosture.Sim);
                }
                else
                {
                    sitPosture.Sim.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToSwim);
                }
            }

            if (!MultiSeatObjectEx.StateMachineEnterAndSit(ths, smc, sitPosture, routingSlot, sitContext))
            {
                return false;
            }

            sitPosture.Interactions.Remove(new InteractionObjectPair(StartSeatedCuddleA.Singleton, sitPosture.Sim));
            sitPosture.Sim.RemoveInteractionByType(StartSeatedCuddleA.Singleton);

            List<InteractionDefinition> list = sitPosture.mSocialInteractionDefinitions as List<InteractionDefinition>;
            if (list != null)
            {
                list.Remove(StartSeatedCuddleA.Singleton);
            }

            sitPosture.AddInteraction(SaunaClassic.CuddleSeatedWooHooSauna.Singleton, sitPosture.Sim);
            sitPosture.AddInteraction(SaunaClassic.CuddleSeatedWooHooSauna.TryForBoySingleton, sitPosture.Sim);
            sitPosture.AddInteraction(SaunaClassic.CuddleSeatedWooHooSauna.TryForGirlSingleton, sitPosture.Sim);
            sitPosture.AddInteraction(SaunaClassic.StartSaunaSeatedCuddleA.Singleton, sitPosture.Sim);
            sitPosture.AddSocialInteraction(SaunaClassic.StartSaunaSeatedCuddleA.Singleton);

            sitPosture.Sim.BuffManager.AddElementPaused((BuffNames)(0x9a7f5f1919df0018L), Origin.None);
            return true;
        }
    }
}


