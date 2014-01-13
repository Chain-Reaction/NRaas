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
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public abstract class LeafPileBaseWoohoo : LeafPile.WoohooInPileOrStack, Common.IPreLoad, Common.IAddInteraction
    {
        public LeafPileBaseWoohoo()
        { }

        public abstract void AddInteraction(Common.InteractionInjectorList interactions);

        public abstract void OnPreLoad();

        public override void Cleanup()
        {
            try
            {
                if (Actor.SimDescription.Teen)
                {
                    Actor.FadeIn();
                }

                base.Cleanup();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
        }

        protected abstract bool UsingNakedOutfit
        {
            get;
        }

        public override bool Run()
        {
            try
            {
                int num;
                if ((WoohooObject == null) || !WoohooObject.CanWooHooIn())
                {
                    return false;
                }
                if (!SafeToSync())
                {
                    return false;
                }

                LeafPile.WoohooInPileOrStackB entry = LeafPile.WoohooInPileOrStackB.Singleton.CreateInstance(Actor, Target, GetPriority(), Autonomous, CancellableByPlayer) as LeafPile.WoohooInPileOrStackB;
                entry.LinkedInteractionInstance = this;
                if (!Target.InteractionQueue.Add(entry))
                {
                    Common.DebugNotify("LeafPileBaseWoohoo Add Fail");
                    return false;
                }

                Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                Actor.SynchronizationTarget = Target;
                Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                if (!WoohooObject.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 10f))
                {
                    return false;
                }

                Slot[] routingSlots = WoohooObject.GetRoutingSlots();
                if (!Actor.RouteToSlotListAndCheckInUse(WoohooObject, routingSlots, out num))
                {
                    return false;
                }

                if (!WoohooObject.CanWooHooIn())
                {
                    return false;
                }

                CommonWoohoo.TestNakedOutfit(UsingNakedOutfit, Actor, Target);

                mActorXRoutingSlot = routingSlots[num];
                StandardEntry();
                WoohooObject.AddToUseList(Actor);
                WoohooObject.AddToUseList(Target);
                BeginCommodityUpdates();
                EnterStateMachine(WoohooObject.JazzFileName, "Enter", "x");
                AddOneShotScriptEventHandler(0x65, PlayEffectsHandlerX);
                AddOneShotScriptEventHandler(0x78, AnimCallbackSimX);
                SetActor(WoohooObject.JazzObjectName, WoohooObject);
                Animate("x", "GetInStackX");
                WoohooObject.SimLine.RemoveFromQueue(Actor);
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Routed, 30f))
                {
                    AddOneShotScriptEventHandler(0x65, PlayEffectsHandlerX);
                    AddOneShotScriptEventHandler(0x66, PlayEffectsHandlerX);
                    AddOneShotScriptEventHandler(0x6e, AnimCallbackSimX);
                    AddOneShotScriptEventHandler(0x79, AnimCallbackSimX);
                    Animate("x", "GetOut");
                    Animate("x", "Exit");
                    WoohooObject.RemoveFromUseList(Actor);
                    WoohooObject.RemoveFromUseList(Target);
                    EndCommodityUpdates(false);
                    StandardExit();
                    return false;
                }

                mActorYRoutingSlot = (Slot)(LinkedInteractionInstance as LeafPile.WoohooInPileOrStackB).RoutedSlot;

                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                SetActor("y", Target);
                EnterState("y", "Enter");
                AddOneShotScriptEventHandler(0x65, PlayEffectsHandlerY);
                AddOneShotScriptEventHandler(0x78, AnimCallbackSimY);
                Animate("y", "GetInStackY");
                if (mReactToSocialBroadcasterActor == null)
                {
                    mReactToSocialBroadcasterActor = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                    CommonWoohoo.CheckForWitnessedCheating(Actor, Target, true);
                }

                if (mReactToSocialBroadcasterTarget == null)
                {
                    mReactToSocialBroadcasterTarget = new ReactionBroadcaster(Target, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                    CommonWoohoo.CheckForWitnessedCheating(Target, Actor, true);
                }

                AnimateJoinSims("Woohoo");
                (WoohooObject as GameObject).PushSimsFromFootprint((uint)mActorXRoutingSlot, Actor, null, true);
                AddOneShotScriptEventHandler(0x65, PlayEffectsHandlerX);
                AddOneShotScriptEventHandler(0x66, PlayEffectsHandlerX);
                AddOneShotScriptEventHandler(0x6e, AnimCallbackSimX);
                AddOneShotScriptEventHandler(0x79, AnimCallbackSimX);
                AnimateJoinSims("GetOutX");
                List<Sim> exceptions = new List<Sim>();
                exceptions.Add(Actor);
                (WoohooObject as GameObject).PushSimsFromFootprint((uint)mActorYRoutingSlot, Target, exceptions, true);
                if (mActorYRoutingSlot == mActorXRoutingSlot)
                {
                    Actor.Wander(1f, 2f, false, RouteDistancePreference.PreferFurthestFromRouteOrigin, true);
                }
                AddOneShotScriptEventHandler(0x65, PlayEffectsHandlerY);
                AddOneShotScriptEventHandler(0x66, PlayEffectsHandlerY);
                AddOneShotScriptEventHandler(0x6e, AnimCallbackSimY);
                AddOneShotScriptEventHandler(0x79, AnimCallbackSimY);
                AnimateJoinSims("GetOutY");
                AnimateJoinSims("Exit");
                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);

                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                CommonWoohoo.RunPostWoohoo(Actor, Target, WoohooObject, definition.GetStyle(this), definition.GetLocation(WoohooObject), true);

                if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                {
                    Pregnancy pregnancy = CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                    if (pregnancy != null)
                    {
                        if (RandomUtil.RandomChance(WoohooObject.ChanceBabyGetsLovesOutdoorsTrait))
                        {
                            pregnancy.SetForcedBabyTrait(TraitNames.LovesTheOutdoors);
                        }
                    }
                }

                WoohooObject.RemoveFromUseList(Actor);
                WoohooObject.RemoveFromUseList(Target);
                EndCommodityUpdates(true);
                StandardExit();

                VisitSituation situation = VisitSituation.FindVisitSituationInvolvingGuest(Actor);
                VisitSituation situation2 = VisitSituation.FindVisitSituationInvolvingGuest(Target);
                if ((situation != null) && (situation2 != null))
                {
                    situation.GuestStartingInappropriateAction(Actor, 3.5f);
                    situation2.GuestStartingInappropriateAction(Target, 3.5f);
                }

                if (RandomUtil.RandomChance(WoohooObject.ChanceGetRollInHayBuff))
                {
                    Actor.BuffManager.AddElement(BuffNames.RolledInTheHay, Origin.FromWooHooInHayStack);
                    Target.BuffManager.AddElement(BuffNames.RolledInTheHay, Origin.FromWooHooInHayStack);
                }

                Relationship.Get(Actor, Target, true).LTR.UpdateLiking(WoohooObject.LTRGain);
                EventTracker.SendEvent(WoohooObject.WooHooEventID, Actor, Target);
                EventTracker.SendEvent(WoohooObject.WooHooEventID, Target, Actor);
                return true;

            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
                return false;
            }
        }
    }
}
