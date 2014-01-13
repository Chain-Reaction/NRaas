using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
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
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HotTubGetIn : HotTubBase.GetIn, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;
        static InteractionDefinition sOldSkinnyDipSingleton;

        public bool mIsMaster;

        public bool mCompleted;

        public void OnPreLoad()
        {
            Tunings.Inject<HotTubBase, HotTubBase.GetIn.Definition, Definition>(false);

            InteractionTuning tuning = Tunings.Inject<HotTubBase, HotTubBase.GetIn.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.SetFlags(Availability.FlagField.DisallowedIfPregnant, false);
            }

            sOldSingleton = Singleton;
            Singleton = new Definition(false);

            sOldSkinnyDipSingleton = SkinnyDipSingleton;
            SkinnyDipSingleton = new Definition(true);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<HotTubBase, HotTubBase.GetIn.Definition>(Singleton);
            interactions.Replace<HotTubBase, HotTubBase.GetIn.SkinnyDipDefinition>(SkinnyDipSingleton);
        }

        public override void Cleanup()
        {
            mCompleted = true;

            base.Cleanup();
        }

        public override bool Run()
        {
            try
            {
                Definition interactionDefinition = InteractionDefinition as Definition;
                bool isSkinnyDipping = interactionDefinition.IsSkinnyDipping || (Actor.CurrentOutfitCategory == OutfitCategories.SkinnyDippingTowel);

                pickSeat();
                if (isSkinnyDipping)
                {
                    mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GoingToSwim, OutfitCategories.Naked, false);
                }
                else
                {
                    mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GoingToSwim);
                }

                mSwitchOutfitHelper.Start();
                Slot none = Slot.None;
                if (!RouteToHottub(out none))
                {
                    return false;
                }

                HotTubSeat part = Target.PartComponent.GetPart(none) as HotTubSeat;
                if (part.InUse && !MoveSimToDifferentSeat(part))
                {
                    return false;
                }

                if (Target.Repairable.Broken)
                {
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();
                if (Actor.HasTrait(TraitNames.Hydrophobic))
                {
                    Actor.PlayReaction(ReactionTypes.WhyMe, Target, ReactionSpeed.ImmediateWithoutOverlay);
                }

                // Custom
                bool succeeded = HotTubBaseEx.SitDown(Target, Actor, none, part, mSwitchOutfitHelper, isSkinnyDipping, InvitedBy, Autonomous);
                if (succeeded)
                {
                    if (Actor.HasExitReason(ExitReason.CancelledByPosture))
                    {
                        InteractionInstance cancelTransition = Actor.Posture.GetCancelTransition();
                        Actor.InteractionQueue.PushAsContinuation(cancelTransition, true);
                    }
                    else
                    {
                        if ((!Target.Repairable.Broken && !Target.Repairable.UpdateBreakage(Actor)) && Actor.HasTrait(TraitNames.Hydrophobic))
                        {
                            Actor.BuffManager.AddElementPaused(BuffNames.Hydrophobic, Origin.FromHotTub);
                        }
                        Target.PushRelaxInteraction(Actor, Autonomous);
                    }
                }

                StandardExit(!succeeded, !succeeded);
                EndCommodityUpdates(succeeded);

                if (mIsMaster)
                {
                    HotTubGetIn linked = LinkedInteractionInstance as HotTubGetIn;
                    if (linked != null)
                    {
                        Sim linkedActor = linked.Actor;

                        while (!Cancelled)
                        {
                            if (!linkedActor.InteractionQueue.HasInteraction(linked)) break;

                            if (linked.mCompleted) break;

                            SpeedTrap.Sleep(10);
                        }
                    }
                }

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

        public new class Definition : HotTubBase.GetIn.Definition
        {
            public Definition()
            { }
            public Definition(bool isSkinnyDipping)
                : base(isSkinnyDipping)
            { }

            public override string GetInteractionName(Sim actor, HotTubBase target, InteractionObjectPair iop)
            {
                if (IsSkinnyDipping)
                {
                    return Localization.LocalizeString(HotTubBase.sLocalizationKey + "/GetIn:SkinnyDipInteractionName", new object[0x0]);
                }
                else
                {
                    return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
                }
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new HotTubGetIn();
                result.Init(ref parameters);
                return result;
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                using (WoohooTuningControl control = new WoohooTuningControl(parameters.InteractionObjectPair.Tuning, (!IsSkinnyDipping) || (Woohooer.Settings.mAllowTeenSkinnyDip)))
                {
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
            }

            public override bool Test(Sim a, HotTubBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if ((target.Repairable != null) && (target.Repairable.Broken))
                    {
                        return false;
                    }

                    HotTubPosture posture = a.Posture as HotTubPosture;
                    if ((posture != null) && (posture.Container == target))
                    {
                        return false;
                    }

                    if (target.mSimsAreWooHooing)
                    {
                        return false;
                    }

                    if (isAutonomous && a.HasTrait(TraitNames.Hydrophobic))
                    {
                        return false;
                    }

                    if (target.SeatingGroup.Count == target.UseCount)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(HotTubBase.LocalizeString(a.IsFemale, "AllSeatsTaken", new object[0x0]));
                        return false;
                    }

                    if (a.CurrentOutfitCategory == OutfitCategories.SkinnyDippingTowel)
                    {
                        greyedOutTooltipCallback = new GrayedOutTooltipHelper(a.IsFemale, "ClothesStolenTooltip", null).GetTooltip;
                        return false;
                    }

                    if (HotTubBase.StressExitFromHeat(a, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }

                    if (IsSkinnyDipping)
                    {
                        // Custom
                        return CommonSkinnyDip.CanSkinnyDipAtLocation(a, target.Position, ref greyedOutTooltipCallback, false, true);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
