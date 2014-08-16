using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Scuba;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
    public class WooHooInCave : UnderwaterCave.WoohooInCave, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabyInCaveSingleton = new TryForBabyInCaveDefinition();

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<UnderwaterCave, UnderwaterCave.WoohooInCave.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<UnderwaterCave, UnderwaterCave.TryForBabyInCave.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<UnderwaterCave, UnderwaterCave.TryForBabyInCave.Definition, TryForBabyInCaveDefinition>(false);
            Woohooer.InjectAndReset<UnderwaterCave, UnderwaterCave.TryForBabyInCave.Definition, ProxyDefinition>(false);

            UnderwaterCave.WoohooInCave.Singleton = SafeSingleton;

            UnderwaterCave.TryForBabyInCave.Singleton = TryForBabyInCaveSingleton;
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<UnderwaterCave>(SafeSingleton);
            interactions.Add<UnderwaterCave>(RiskySingleton);
            interactions.Add<UnderwaterCave>(TryForBabyInCaveSingleton);
        }

        private void FirePreggoEx(StateMachineClient sender, IEvent evt)
        {
            IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

            if (CommonPregnancy.IsSuccess(mWoohooer, mWoohooee, Autonomous, definition.GetStyle(this)))
            {
                CommonPregnancy.Impregnate(mWoohooer, mWoohooee, Autonomous, definition.GetStyle(this));
            }
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                if (mWoohooer == null)
                {
                    mMaster = true;
                    mWoohooer = Actor;
                    mWoohooee = GetSelectedObject() as Sim;
                    if (mWoohooee == null)
                    {
                        return false;
                    }
                }

                if (mMaster)
                {
                    UnderwaterCave.WoohooInCave entry = definition.ProxyClone(mWoohooer).CreateInstance(Target, mWoohooee, mPriority, Autonomous, true) as UnderwaterCave.WoohooInCave;
                    entry.mWoohooer = Actor;
                    entry.mWoohooee = mWoohooee;
                    entry.LinkedInteractionInstance = this;
                    mWoohooee.InteractionQueue.AddNext(entry);

                    if (Target.mIsTentacleWaving)
                    {
                        Target.TentacleWaveUpdate();
                    }

                    if (!Actor.RouteToSlot(Target, Slot.RoutingSlot_0))
                    {
                        mWoohooee.AddExitReason(ExitReason.SynchronizationFailed);
                        return false;
                    }

                    Target.WaitForTentacle(Actor);
                    StandardEntry();
                    EnterStateMachine("UnderwaterCaveWooHoo", "enter", "x");
                    SetParameter("IsMermaidX", Actor.SimDescription.IsMatureMermaid);
                    SetActor("cave", Target);
                    AnimateSim("xEnterCave");
                    mRouteComplete = true;
                }

                UnderwaterCave.WoohooInCave linkedInteractionInstance = LinkedInteractionInstance as UnderwaterCave.WoohooInCave;
                while (((linkedInteractionInstance != null) && !linkedInteractionInstance.mRouteComplete) && !Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                {
                    SpeedTrap.Sleep();
                }

                if ((linkedInteractionInstance == null) || !linkedInteractionInstance.mRouteComplete)
                {
                    return false;
                }

                if (!mMaster)
                {
                    Route r = Actor.CreateRoute();
                    r.AddObjectToIgnoreForRoute(mWoohooer.ObjectId);
                    r.PlanToSlot(Target, Slot.RoutingSlot_0);
                    mRouteComplete = Actor.DoRoute(r);
                    if (!mRouteComplete)
                    {
                        mWoohooer.AddExitReason(ExitReason.SynchronizationFailed);
                        return false;
                    }
                    StandardEntry();
                }

                BeginCommodityUpdates();
                if (mMaster)
                {
                    if (Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                    {
                        AnimateSim("xExit");
                        return false;
                    }
                    Target.WaitForTentacle(mWoohooee);
                    SetActor("y", mWoohooee);
                    SetParameter("IsMermaidY", mWoohooee.SimDescription.IsMatureMermaid);
                    AddOneShotScriptEventHandler(0xc9, FireVFX);
                    if (definition.GetStyle(this) != CommonWoohoo.WoohooStyle.Safe)
                    {
                        // Custom
                        AddOneShotScriptEventHandler(0x3e9, FirePreggoEx);
                    }

                    CommonWoohoo.RunPostWoohoo(mWoohooer, mWoohooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                    mCurrentStateMachine.EnterState("y", "enter");
                    mCurrentStateMachine.RequestState("y", "yEnterCave");
                    AnimateJoinSims("woohoo");
                    mWoohooer.BuffManager.AddElement(BuffNames.MileLowClub, Origin.FromWooHooUnderwaterCave);
                    mWoohooee.BuffManager.AddElement(BuffNames.MileLowClub, Origin.FromWooHooUnderwaterCave);
                    mWoohooer.GetRelationship(mWoohooee, true).LTR.UpdateLiking(kLTRBump);
                    if (mVfxone != null)
                    {
                        mVfxone.Stop(VisualEffect.TransitionType.SoftTransition);
                        mVfxone = null;
                    }

                    if (mVfxtwo != null)
                    {
                        mVfxtwo.Stop(VisualEffect.TransitionType.SoftTransition);
                        mVfxtwo = null;
                    }

                    AnimateSim("xExit");
                    Actor.RouteToObjectRadius(Target, kRouteRadiusRange);
                    Actor.LoopIdle();
                    mCurrentStateMachine.RequestState("y", "yExit");
                    mWoohooee.AddExitReason(ExitReason.Finished);
                }
                else
                {
                    DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    Actor.RouteToObjectRadius(Target, kRouteRadiusRange);
                }

                EventTracker.SendEvent(EventTypeId.kWooHooInUnderwaterCave, Actor, Target);
                EndCommodityUpdates(true);
                StandardExit();
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<UnderwaterCave, WooHooInCave, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<UnderwaterCave>
        {
            public BaseDefinition()
            {}
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override bool PushSocial
            {
                get { return false; }
            }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Cave;
            }

            public override Sim GetTarget(Sim actor, UnderwaterCave target, InteractionInstance interaction)
            {
                if (target.ActorsUsingMe.Count == 1)
                {
                    return target.ActorsUsingMe[0];
                }

                return base.GetTarget(actor, target, interaction);
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                UnderwaterCave box = obj as UnderwaterCave;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                WooHooInCave instance = new ProxyDefinition(this).CreateInstance(box, actor, priority, false, true) as WooHooInCave;
                instance.mWoohooer = actor;
                instance.mWoohooee = target;
                instance.mMaster = true;
                actor.InteractionQueue.PushAsContinuation(instance, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, UnderwaterCave obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!(actor.Posture is ScubaDiving)) return false;

                if (!(target.Posture is ScubaDiving)) return false;

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

            public override string GetInteractionName(Sim actor, UnderwaterCave target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, UnderwaterCave obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "UnderwaterCaveWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, UnderwaterCave target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, UnderwaterCave obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "UnderwaterCaveRisky", isAutonomous, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
        }

        public class TryForBabyInCaveDefinition : BaseDefinition
        {
            public TryForBabyInCaveDefinition()
            { }
            public TryForBabyInCaveDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, UnderwaterCave target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, UnderwaterCave obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "UnderwaterCaveTryForBabyInCave", isAutonomous, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new TryForBabyInCaveDefinition(target));
            }
        }

        public class LocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.Cave; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is UnderwaterCave;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<UnderwaterCave>(new Predicate<UnderwaterCave>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<UnderwaterCave>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP6)) return false;
                }

                return Woohooer.Settings.mAutonomousUnderwaterCave;
            }

            public bool TestUse(UnderwaterCave obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (UnderwaterCave obj in actor.LotCurrent.GetObjects<UnderwaterCave>(new Predicate<UnderwaterCave>(TestUse)))
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
                        return new TryForBabyInCaveDefinition(target);
                }

                return null;
            }
        }
    }
}
