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
using Sims3.Gameplay.Objects.Pets;
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
    public class BoxStallWoohoo : BoxStall.WooHooInBoxStall, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public BoxStallWoohoo()
        { }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<BoxStall>(SafeSingleton);
            interactions.Add<BoxStall>(RiskySingleton);
            interactions.Add<BoxStall>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            InteractionTuning tuning = Woohooer.InjectAndReset<Sim, BoxStall.WooHooInBoxStall.Definition, ProxyDefinition>(false);
            if (tuning != null)
            {
                tuning.Availability.AgeSpeciesAvailabilityFlags |= CASAGSAvailabilityFlags.HumanTeen | CASAGSAvailabilityFlags.HumanYoungAdult | CASAGSAvailabilityFlags.HumanAdult | CASAGSAvailabilityFlags.HorseElder;
            }

            Tunings.Inject<Sim, ProxyDefinition, BoxStall, SafeDefinition>(true);
            Tunings.Inject<Sim, ProxyDefinition, BoxStall, RiskyDefinition>(true);
            Tunings.Inject<Sim, ProxyDefinition, BoxStall, TryForBabyDefinition>(true);
        }

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

        public override bool Run()
        {
            try
            {
                if (!SafeToSync())
                {
                    return false;
                }
                else if (BoxStallToUse == null)
                {
                    return false;
                }
                else if (!StartSync(IsMaster))
                {
                    return false;
                }

                if (!IsMaster)
                {
                    SpeedTrap.Sleep();
                }

                try
                {
                    Slot destinationSlot = IsMaster ? BoxStall.kRoutingSlot_SnuggleA : BoxStall.kRoutingSlot_SnuggleB;
                    if (Actor.IsHuman)
                    {
                        if (IsMaster)
                        {
                            (BoxStallToUse.PortalComponent as BoxStall.BoxStallPortalComponent).AddPortals();
                        }

                        if (!BoxStallToUse.TryRouteToSlot(Actor, BoxStall.kRoutingSlot_EnterA, BoxStall.RouteOptions.IgnoreOrientation | BoxStall.RouteOptions.IgnoreExitReasons))
                        {
                            return false;
                        }

                        if (!BoxStallToUse.TryRouteToSlot(Actor, destinationSlot))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!BoxStallToUse.RouteToAndEnter(Actor, destinationSlot, this))
                        {
                            return false;
                        }
                    }

                    Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                    if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Routed, 30f))
                    {
                        return false;
                    }

                    BeginCommodityUpdates();

                    IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                    if (IsMaster)
                    {
                        try
                        {
                            RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                            Actor.EnableCensor(Sim.CensorType.FullBody);
                            Target.EnableCensor(Sim.CensorType.FullBody);
                            EnterStateMachine("BoxStallWooHoo", "Enter", "x", "y");
                            AddPersistentSynchronousScriptEventHandler(0x65, StartEffects);
                            AddPersistentSynchronousScriptEventHandler(0x66, StopEffects);
                            mCurrentStateMachine.RequestState(false, "x", "WooHoo");
                            mCurrentStateMachine.RequestState(true, "y", "WooHoo");
                            mCurrentStateMachine.RequestState(false, "x", "Exit");
                            mCurrentStateMachine.RequestState(true, "y", "Exit");
                        }
                        finally
                        {
                            Actor.AutoEnableCensor();
                            Target.AutoEnableCensor();
                            RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                        }

                        CommonWoohoo.RunPostWoohoo(Actor, Target, TargetBoxStall(), definition.GetStyle(this), definition.GetLocation(TargetBoxStall()), true);

                        if (RandomUtil.RandomChance(HayStack.kChanceOfRolledInHayMoodlet))
                        {
                            Actor.BuffManager.AddElement(BuffNames.RolledInTheHay, WoohooBuffs.sWoohooOrigin);
                            Target.BuffManager.AddElement(BuffNames.RolledInTheHay, WoohooBuffs.sWoohooOrigin);
                        }

                        if (Actor.IsHuman)
                        {
                            BoxStallToUse.TryRouteToSlot(Actor, BoxStall.kRoutingSlot_EnterB, BoxStall.RouteOptions.IgnoreOrientation | BoxStall.RouteOptions.IgnoreExitReasons);
                            BoxStallToUse.TryRouteToSlot(Actor, BoxStall.kRoutingSlot_Exit, BoxStall.RouteOptions.IgnoreExitReasons);
                        }
                    }

                    if (IsMaster)
                    {
                        if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                        {
                            CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                        }
                    }

                    if (Actor.IsHuman)
                    {
                        BoxStallToUse.TryRouteToSlot(Actor, BoxStall.kRoutingSlot_EnterB, BoxStall.RouteOptions.IgnoreOrientation | BoxStall.RouteOptions.IgnoreExitReasons);
                        BoxStallToUse.TryRouteToSlot(Actor, BoxStall.kRoutingSlot_Exit, BoxStall.RouteOptions.IgnoreExitReasons);
                    }

                    Actor.SynchronizationLevel = Sim.SyncLevel.Completed;
                    if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Completed, ExitReason.None, 30f))
                    {
                        EndCommodityUpdates(false);
                        return false;
                    }

                    if ((!IsMaster) && (!Actor.IsHuman))
                    {
                        TryPushAsContinuation(BoxStall.Exit.Singleton, BoxStallToUse);
                    }

                    EndCommodityUpdates(true);
                    FinishLinkedInteraction();
                    WaitForSyncComplete();
                    return true;
                }
                finally
                {
                    if ((Actor.IsHuman) && (IsMaster))
                    {
                        (BoxStallToUse.PortalComponent as BoxStall.BoxStallPortalComponent).RemovePortals();
                    }
                }
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, BoxStallWoohoo, BaseStallDefinition>
        {
            public ProxyDefinition(BaseStallDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public abstract class BaseStallDefinition : CommonWoohoo.PotentialDefinition<BoxStall>
        {
            public BaseStallDefinition()
            { }
            public BaseStallDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.BoxStall;
            }

            public override Sim GetTarget(Sim actor, BoxStall target, InteractionInstance interaction)
            {
                BoxStall stall = target as BoxStall;

                List<Sim> sims = new List<Sim>(stall.ActorsUsingMe);
                sims.Remove(actor);

                if (sims.Count == 1)
                {
                    return sims[0];
                }
                else
                {
                    return base.GetTarget(actor, target, interaction);
                }
            }

            public override bool JoinInProgress(Sim actor, Sim target, BoxStall obj, InteractionInstance interaction)
            {
                if ((obj.ActorsUsingMe.Contains(actor)) || (obj.ActorsUsingMe.Contains(target)))
                {
                    Common.DebugNotify("JoinInProgress");

                    PushWooHoo(actor, target, obj);
                    return true;
                }

                return base.JoinInProgress(actor, target, obj, interaction);
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                BoxStall stall = obj as BoxStall;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                BoxStallWoohoo entry = new ProxyDefinition(this).CreateInstance(actor, target, priority, false, true) as BoxStallWoohoo;
                entry.IsMaster = false;
                entry.BoxStallToUse = stall;
                target.InteractionQueue.PushAsContinuation(entry, true);

                BoxStallWoohoo entry2 = new ProxyDefinition(this).CreateInstance(target, actor, priority, false, true) as BoxStallWoohoo;
                entry2.IsMaster = true;
                entry2.LinkedInteractionInstance = entry;
                entry2.BoxStallToUse = stall;
                actor.InteractionQueue.PushAsContinuation(entry2, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, BoxStall obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeDefinition : BaseStallDefinition
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

            public override string GetInteractionName(Sim actor, BoxStall target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, BoxStall obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                try
                {
                    if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                    return CommonWoohoo.SatisfiesWoohoo(actor, target, "StallWoohoo", isAutonomous, true, true, ref callback);
                }
                catch (Exception exception)
                {
                    Common.Exception(actor, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeDefinition(target));
            }
        }

        public class RiskyDefinition : BaseStallDefinition
        {
            public RiskyDefinition()
            { }
            public RiskyDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, BoxStall target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim a, Sim target, BoxStall obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesRisky(a, target, "StallRisky", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                    return false;
                }
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
        }

        public class TryForBabyDefinition : BaseStallDefinition
        {
            public TryForBabyDefinition()
            { }
            public TryForBabyDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, BoxStall target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, BoxStall obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "StallTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                    return false;
                }
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
                get { return CommonWoohoo.WoohooLocation.BoxStall; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is BoxStall;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<BoxStall>(new Predicate<BoxStall>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<BoxStall>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP5)) return false;
                }

                if (sim.IsHuman)
                {
                    return false;//return Woohooer.Settings.mAutonomousBoxStallHuman;
                }
                else if (sim.IsHorse)
                {
                    return Woohooer.Settings.mAutonomousBoxStallHorse;
                }
                else
                {
                    return false;
                }
            }

            public bool TestUse(BoxStall obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (BoxStall obj in actor.LotCurrent.GetObjects<BoxStall>(new Predicate<BoxStall>(TestUse)))
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
