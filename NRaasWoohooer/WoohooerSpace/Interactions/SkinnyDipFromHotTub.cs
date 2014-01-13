using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class SkinnyDipFromHotTub : HotTubBase.SkinnyDipFromHotTub, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Woohooer.InjectAndReset<HotTubBase, HotTubBase.SkinnyDipFromHotTub.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.SetFlags(Availability.FlagField.DisallowedIfPregnant, false);
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<HotTubBase, HotTubBase.SkinnyDipFromHotTub.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                StandardEntry(false);
                mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GoingToSwim, OutfitCategories.Naked);
                mSwitchOutfitHelper.Start();
                mSwitchOutfitHelper.Wait(false);

                HotTubSeat partSimIsIn = Target.PartComponent.GetPartSimIsIn(Actor) as HotTubSeat;
                mCurrentStateMachine = Actor.Posture.CurrentStateMachine;
                SetParameter("IkSuffix", partSimIsIn.IKSuffix);

                BeginCommodityUpdates();
                AnimateSim("Change To Naked");
                mSwitchOutfitHelper.ChangeOutfit();
                
                // Custom
                HotTubBaseEx.StartSkinnyDipBroadcastersAndSendWishEvents(Target, Actor);

                partSimIsIn.CreateClothingPile();
                Actor.BridgeOrigin = Actor.Posture.Idle();
                EndCommodityUpdates(true);

                Target.PushRelaxInteraction(Actor, Autonomous);
                StandardExit(false, false);
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

        public new class Definition : HotTubBase.SkinnyDipFromHotTub.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new SkinnyDipFromHotTub();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim actor, HotTubBase target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                using (WoohooTuningControl control = new WoohooTuningControl(parameters.InteractionObjectPair.Tuning, Woohooer.Settings.mAllowTeenSkinnyDip))
                {
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
            }

            public override bool Test(Sim a, HotTubBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    HotTubPosture posture = a.Posture as HotTubPosture;
                    if (posture == null) return false;

                    if (posture.Container != target) return false;

                    if (a.IsSkinnyDipping()) return false;

                    // Custom
                    return CommonSkinnyDip.CanSkinnyDipAtLocation(a, target.Position, ref greyedOutTooltipCallback, false, true);
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
