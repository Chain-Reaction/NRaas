using NRaas.CommonSpace.Interactions;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Pools;
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
    public class AncientPortalWooHoo : Interaction<Sim, AncientPortal>, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        ReactionBroadcaster mReactToSocialBroadcaster;

        Sim mLinkedActor;

        bool mNowWaiting;
        bool mWasSuccess;

        public bool mIsMaster;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<AncientPortal>(SafeSingleton);
            interactions.Add<AncientPortal>(RiskySingleton);
            interactions.Add<AncientPortal>(TryForBabySingleton);
        }

        private void HideSim(StateMachineClient sender, IEvent evt)
        {
            Actor.SetHiddenFlags(HiddenFlags.Model);
        }

        private void ShowSim(StateMachineClient sender, IEvent evt)
        {
            Actor.SetHiddenFlags(HiddenFlags.Nothing);
        }

        public override void Cleanup()
        {
            if (mReactToSocialBroadcaster != null)
            {
                mReactToSocialBroadcaster.Dispose();
                mReactToSocialBroadcaster = null;
            }

            base.Cleanup();
        }

        public override bool Run()
        {
            try
            {
                mLinkedActor = LinkedInteractionInstance.InstanceActor;

                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                bool succeeded = true;
                if (mIsMaster)
                {
                    Target.mRoutingSims.Add(Actor);
                    Target.mRoutingSims.Add(mLinkedActor);
                }

                if (!mIsMaster)
                {
                    bool success = false;

                    Actor.RouteToObjectRadialRange(Target, 1.5f, 4f);
                    while (Target.mRoutingSims.Contains(mLinkedActor))
                    {
                        AncientPortalWooHoo interaction = LinkedInteractionInstance as AncientPortalWooHoo;
                        if (interaction == null) break;

                        if (mLinkedActor.HasExitReason(ExitReason.Canceled)) break;

                        if (interaction.mNowWaiting)
                        {
                            success = true;
                            break;
                        }

                        SpeedTrap.Sleep(0xa);
                    }

                    if (!success)
                    {
                        Actor.AddExitReason(ExitReason.RouteFailed);
                        return false;
                    }
                }

                bool routeSuccess = false;

                Route r = Actor.CreateRoute();
                r.AddObjectToIgnoreForRoute(mLinkedActor.ObjectId);
                r.PlanToSlot(Target, Target.GetRoutingSlots());
                routeSuccess = Actor.DoRoute(r);

                if ((mIsMaster) && (routeSuccess))
                {
                    routeSuccess = !Actor.InUse;
                }

                if (!routeSuccess)
                {
                    Actor.AddExitReason(ExitReason.RouteFailed);
                    return false;
                }

                CommonWoohoo.TestNakedOutfit(Woohooer.Settings.mNakedOutfitAncientPortal, Actor);

                StandardEntry();

                EnterStateMachine("AncientPortal", "Enter", "x", "portal");

                AddOneShotScriptEventHandler(0x65, HideSim);
                AddOneShotScriptEventHandler(0x66, ShowSim);
                AnimateSim("InsidePortal");

                if (mIsMaster)
                {
                    Target.RemoveFromUseList(Actor);

                    mNowWaiting = true;

                    RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                }

                BeginCommodityUpdates();

                if (mReactToSocialBroadcaster == null)
                {
                    mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                    CommonWoohoo.CheckForWitnessedCheating(Actor, mLinkedActor, !Rejected);
                }

                if ((Target.BoobyTrapComponent != null) ? Target.BoobyTrapComponent.CanTriggerTrap(Actor.SimDescription) : false)
                {
                    Target.TriggerTrap(Actor);
                }

                int count = 0;

                while (Target.mRoutingSims.Contains(mLinkedActor))
                {
                    AncientPortalWooHoo interaction = LinkedInteractionInstance as AncientPortalWooHoo;
                    if (interaction == null) break;

                    if (mLinkedActor.HasExitReason(ExitReason.Canceled)) break;

                    if (!mIsMaster)
                    {
                        interaction.mWasSuccess = true;

                        if (count > 30) break;
                        count++;

                        Target.SetOpacity(RandomUtil.GetFloat(0f, 1f), 0.25f);
                    }

                    SpeedTrap.Sleep(10);
                }

                if (!mIsMaster)
                {
                    Target.SetOpacity(1f, 1f);
                }

                AnimateSim("Exit");

                for (int i = 0; i < AncientPortal.CatchABeam.kPotentialTravelBuffs.Length; i++)
                {
                    if (RandomUtil.RandomChance(AncientPortal.CatchABeam.kChanceForEachBuff))
                    {
                        Actor.BuffManager.AddElement(AncientPortal.CatchABeam.kPotentialTravelBuffs[i], Origin.FromAncientPortal);
                    }
                }

                if (mIsMaster)
                {
                    RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                }

                EndCommodityUpdates(succeeded);
                StandardExit();

                if ((mWasSuccess) && (mIsMaster))
                {
                    CommonWoohoo.RunPostWoohoo(Actor, mLinkedActor, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                    if (CommonPregnancy.IsSuccess(Actor, mLinkedActor, Autonomous, definition.GetStyle(this)))
                    {
                        CommonPregnancy.Impregnate(Actor, mLinkedActor, Autonomous, definition.GetStyle(this));
                    }
                }

                Actor.Wander(1f, 2f, false, RouteDistancePreference.PreferNearestToRouteOrigin, false);
                return succeeded;
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
            finally
            {
                Target.mRoutingSims.Remove(Actor);
            }
        }

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<AncientPortal, AncientPortalWooHoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<AncientPortal>
        {
            public BaseDefinition()
            {}
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.AncientPortal;
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                AncientPortal portal = obj as AncientPortal;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                AncientPortalWooHoo instance = new ProxyDefinition(this).CreateInstance(portal, actor, priority, false, true) as AncientPortalWooHoo;
                instance.mIsMaster = true;

                AncientPortalWooHoo hoo2 = new ProxyDefinition(this).CreateInstance(portal, target, priority, false, true) as AncientPortalWooHoo;
                if (actor.InteractionQueue.PushAsContinuation(instance, true))
                {
                    target.InteractionQueue.PushAsContinuation(hoo2, true);
                    instance.LinkedInteractionInstance = hoo2;
                }
            }

            protected override bool Satisfies(Sim actor, Sim target, AncientPortal obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeDefinition : BaseDefinition
        {
            public SafeDefinition()
            { }
            public SafeDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string GetInteractionName(Sim actor, AncientPortal target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, AncientPortal obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "AncientPortalWoohoo", isAutonomous, true, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeDefinition(target));
            }
        }

        public class RiskyDefinition : BaseDefinition
        {
            public RiskyDefinition()
            { }
            public RiskyDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, AncientPortal target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, AncientPortal obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "AncientPortalRisky", isAutonomous, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
        }

        public class TryForBabyDefinition : BaseDefinition
        {
            public TryForBabyDefinition()
            { }
            public TryForBabyDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, AncientPortal target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, AncientPortal obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "AncientPortalTryForBaby", isAutonomous, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new TryForBabyDefinition(target));
            }
        }

        public class LocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.AncientPortal; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is AncientPortal;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<AncientPortal>(new Predicate<AncientPortal>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<AncientPortal>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP7)) return false;
                }

                return Woohooer.Settings.mAutonomousAncientPortal;
            }

            public bool TestUse(AncientPortal obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.mRoutingSims.Count == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (AncientPortal obj in actor.LotCurrent.GetObjects<AncientPortal>(new Predicate<AncientPortal>(TestUse)))
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj);
                }

                return results;
            }

            public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                switch (style)
                {
                    case CommonWoohoo.WoohooStyle.Safe:
                        return new SafeDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new RiskyDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new TryForBabyDefinition(target);
                }

                return null;
            }
        }
    }
}
