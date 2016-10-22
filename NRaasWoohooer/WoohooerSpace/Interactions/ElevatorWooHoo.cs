using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
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
using Sims3.Gameplay.Objects.Elevator;
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
    public class ElevatorWooHoo : ElevatorDoors.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ElevatorDoors, ElevatorDoors.WooHoo.Definition>(SafeSingleton);
            interactions.Add<ElevatorDoors>(RiskySingleton);
            interactions.Add<ElevatorDoors>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<ElevatorDoors, ElevatorDoors.WooHoo.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<ElevatorDoors, ElevatorDoors.WooHoo.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<ElevatorDoors, ElevatorDoors.WooHoo.Definition, TryForBabyDefinition>(false);

            ElevatorDoors.WooHoo.Singleton = SafeSingleton;
        }

        public override string GetInteractionName()
        {
            if ((WooHooer != null) && (WooHooee != null))
            {
                Sim sim = (Actor == WooHooer) ? WooHooee : WooHooer;

                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;
                if (definition != null)
                {
                    string name = (definition.GetStyle(this) == CommonWoohoo.WoohooStyle.TryForBaby) ? "TryForBabyWith" : "WooHooWith";
                    return ElevatorDoors.LocalizeString(name, new object[] { sim });
                }
            }

            InteractionInstanceParameters parameters = GetInteractionParameters();
            return InteractionDefinition.GetInteractionName(ref parameters);
        }

        private new void StartJealousyBroadcaster()
        {
            try
            {
                if (mReactToSocialBroadcaster == null)
                {
                    mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                    CommonWoohoo.CheckForWitnessedCheating(Actor, Actor.SynchronizationTarget, true);
                }

                if (mIsMaster)
                {
                    ElevatorWooHoo linked = LinkedInteractionInstance as ElevatorWooHoo;

                    if (linked != null)
                    {
                        linked.StartJealousyBroadcaster();
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public override bool Run()
        {
            try
            {
                ProxyDefinition definition = InteractionDefinition as ProxyDefinition;

                if (mIsMaster)
                {
                    Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                    Actor.SynchronizationTarget = WooHooee;
                }
                else
                {
                    Actor.SynchronizationRole = Sim.SyncRole.Receiver;
                    Actor.SynchronizationTarget = WooHooer;
                }

                bool success = false;
                try
                {
                    if (mIsMaster && !Actor.HasExitReason())
                    {
                        ElevatorWooHoo entry = definition.ProxyClone(WooHooer).CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as ElevatorWooHoo;
                        entry.LinkedInteractionInstance = this;
                        entry.WooHooer = WooHooer;
                        entry.WooHooee = WooHooee;

                        if (!WooHooee.InteractionQueue.AddNext(entry))
                        {
                            return false;
                        }
                    }

                    if (!SafeToSync())
                    {
                        return false;
                    }

                    Actor.LoopIdle();
                    Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                    if (!Actor.WaitForSynchronizationLevelWithSim(Actor.SynchronizationTarget, Sim.SyncLevel.NotStarted, ElevatorDoors.kWooHooSyncTime))
                    {
                        FinishLinkedInteraction(mIsMaster);
                        return false;
                    }

                    if (!Target.RouteToElevator(Actor))
                    {
                        FinishLinkedInteraction(mIsMaster);
                        return false;
                    }

                    Target.InteriorObj.AddElevatorColumnToUseList(Actor);
                    Actor.RouteTurnToFace(Target.Position);
                    Actor.LoopIdle();
                    Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                    if (!Actor.WaitForSynchronizationLevelWithSim(Actor.SynchronizationTarget, Sim.SyncLevel.Started, ElevatorDoors.kWooHooSyncTime))
                    {
                        FinishLinkedInteraction(mIsMaster);
                        return false;
                    }

                    if (!Target.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.CutToHeadOfLine, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), ElevatorDoors.kWooHooSyncTime))
                    {
                        FinishLinkedInteraction(mIsMaster);
                        return false;
                    }

                    IsDoneRouting = true;
                    CancellableByPlayer = false;
                    Slot slotName = Slot.RoutingSlot_0;
                    if (!mIsMaster)
                    {
                        slotName = Slot.RoutingSlot_1;
                    }
                    Actor.SimRoutingComponent.DisallowBeingPushed = true;
                    Actor.SimRoutingComponent.ShouldIgnoreAllObstacles = true;
                    if (!Actor.RouteToSlot(Target.InteriorObj, slotName))
                    {
                        WanderOut();
                        FinishLinkedInteraction(mIsMaster);
                        return false;
                    }
                    Target.SimLine.RemoveFromQueue(Actor);
                    Actor.LoopIdle();
                    Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                    if (!Actor.WaitForSynchronizationLevelWithSim(Actor.SynchronizationTarget, Sim.SyncLevel.Routed, ElevatorDoors.kWooHooSyncTime))
                    {
                        WanderOut();
                        FinishLinkedInteraction(mIsMaster);
                        return false;
                    }
                    IsInsideElevator = true;                    
                    StandardEntry(false);
                    Actor.LoopIdle();
                    Actor.SimDescription.Contactable = false;
                    if (!StartSync(mIsMaster))
                    {
                        WanderOut();                        
                        StandardExit(false);
                        return false;
                    }

                    success = true;
                }
                finally
                {
                    if (!success)
                    {
                        definition.Definition.Restart(mIsMaster, WooHooer, WooHooee, Autonomous, Target);
                    }
                }

                MotiveDelta[] deltaArray = new MotiveDelta[2];
                deltaArray[0] = AddMotiveDelta(CommodityKind.Fun, 1500f);
                deltaArray[1] = AddMotiveDelta(CommodityKind.Social, 50f);
                BeginCommodityUpdates();

                try
                {
                    if (mIsMaster)
                    {
                        AcquireStateMachine("Elevator");
                        SetActorAndEnter("x", Actor, "Enter");
                        SetActorAndEnter("y", WooHooee, "Enter");
                        SetActor("elevatorExterior", Target);

                        success = true;

                        if (Woohooer.Settings.UsingTraitScoring)
                        {
                            if (ScoringLookup.GetScore("ElevatorSuccess", Actor.SimDescription) < 0)
                            {
                                success = false;
                            }
                            else if (ScoringLookup.GetScore("ElevatorSuccess", WooHooee.SimDescription) < 0)
                            {
                                success = false;
                            }
                        }

                        if (success)
                        {
                            isWooHooing = true;
                            RockGemMetalBase.HandleNearbyWoohoo(Target, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                            AddOneShotScriptEventHandler(0x6e, WooHooTurnOnCensorBars);
                            AddOneShotScriptEventHandler(0x6f, WooHooTurnOffCensorBars);
                            mJealousyAlarm = AlarmManager.Global.AddAlarm(4f, TimeUnit.Minutes, StartJealousyBroadcaster, "StartJealousyBroadcaster", AlarmType.DeleteOnReset, Target);
                            AnimateJoinSims("WooHooAccept");

                            CommonWoohoo.RunPostWoohoo(Actor, WooHooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                            CommonWoohoo.TestNakedOutfit(Woohooer.Settings.mNakedOutfitElevator, WooHooer, WooHooee);

                            if (CommonPregnancy.IsSuccess(WooHooer, WooHooee, Autonomous, definition.GetStyle(this)))
                            {
                                CommonPregnancy.Impregnate(WooHooer, WooHooee, Autonomous, definition.GetStyle(this));
                            }

                            RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                            isWooHooing = false;
                        }
                        else
                        {
                            ElevatorDoors.WooHoo linkedInteractionInstance = LinkedInteractionInstance as ElevatorDoors.WooHoo;
                            if (linkedInteractionInstance != null)
                            {
                                linkedInteractionInstance.IsFail = true;
                            }
                            AddOneShotScriptEventHandler(0x65, new SacsEventHandler(WooHooRejectCallback));
                            AnimateJoinSims("WooHooReject");
                            SpeedTrap.Sleep((uint)SimClock.ConvertToTicks(RandomUtil.RandomFloatGaussianDistribution(ElevatorDoors.kWooHooFailWaitTime[0x0], ElevatorDoors.kWooHooFailWaitTime[0x1]), TimeUnit.Minutes));
                            if (WooHooer.LotHome != Target.LotCurrent)
                            {
                                WooHooer.BuffManager.AddElement(BuffNames.WalkOfShame, Origin.FromRejectedWooHooOffHome);
                            }
                            Relationship.Get(WooHooee, WooHooer, true).LTR.UpdateLiking(ElevatorDoors.kWooHooRejectRelHit);
                        }
                        AnimateNoYield("y", "Exit");
                        AnimateSim("Exit");
                    }
                    Actor.SynchronizationLevel = Sim.SyncLevel.Committed;
                    Actor.WaitForSynchronizationLevelWithSim(Actor.SynchronizationTarget, Sim.SyncLevel.Committed, ElevatorDoors.kWooHooSyncTime);
                    if (IsFail && !mIsMaster)
                    {
                        Actor.SetPosition(Target.InteriorObj.GetPositionOfSlot(Slot.RoutingSlot_0));
                        SpeedTrap.Sleep((uint)SimClock.ConvertToTicks(RandomUtil.RandomFloatGaussianDistribution(ElevatorDoors.kWooHooFailKickerOutWaitTime[0x0], ElevatorDoors.kWooHooFailKickerOutWaitTime[0x1]), TimeUnit.Minutes));
                    }
                    Target.RouteToElevator(Actor);
                    Actor.SynchronizationLevel = Sim.SyncLevel.Completed;
                    Actor.WaitForSynchronizationLevelWithSim(Actor.SynchronizationTarget, Sim.SyncLevel.Completed, ElevatorDoors.kWooHooSyncTime);
                    FinishLinkedInteraction(mIsMaster);
                    WaitForSyncComplete();
                }
                finally
                {
                    RemoveMotiveDelta(deltaArray[0x0]);
                    RemoveMotiveDelta(deltaArray[0x1]);
                    EndCommodityUpdates(true);
                }

                StandardExit(false);
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<ElevatorDoors, ElevatorWooHoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<ElevatorDoors>
        {
            public BaseDefinition()
            { }
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Elevator;
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                ElevatorDoors elevator = obj as ElevatorDoors;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                ElevatorWooHoo instance = new ProxyDefinition(this).CreateInstance(elevator, actor, priority, false, true) as ElevatorWooHoo;
                instance.WooHooer = actor;
                instance.WooHooee = target;
                instance.mIsMaster = true;
                actor.InteractionQueue.PushAsContinuation(instance, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, ElevatorDoors obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (isAutonomous)
                {
                    if (!ElevatorDoors.DoSimsMeetWooHooRequirements(actor, target)) return false;
                }

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

            public override string GetInteractionName(Sim actor, ElevatorDoors target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, ElevatorDoors obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "ElevatorWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, ElevatorDoors target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, ElevatorDoors obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "ElevatorRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, ElevatorDoors target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, ElevatorDoors obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "ElevatorTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.Elevator; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is IElevatorObject;
            }

            public bool TestUse(ElevatorDoors door)
            {
                if (!TestRepaired(door)) return false;

                return true;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<ElevatorDoors>(new Predicate<ElevatorDoors>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<ElevatorDoors>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP3)) return false;
                }

                return Woohooer.Settings.mAutonomousElevator;
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (ElevatorDoors obj in actor.LotCurrent.GetObjects<ElevatorDoors>(new Predicate<ElevatorDoors>(new Tester(actor).TestUse)))
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj);
                }

                return results;
            }

            public class Tester
            {
                Sim mActor;

                public Tester(Sim actor)
                {
                    mActor = actor;
                }

                public bool TestUse(ElevatorDoors door)
                {
                    if (!TestRepaired(door)) return false;

                    return (door.Level == mActor.Level);
                }
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
