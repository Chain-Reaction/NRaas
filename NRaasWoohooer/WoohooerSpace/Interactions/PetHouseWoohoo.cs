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
    public class PetHouseWoohoo : PetHouse.WoohooPetHouse, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public PetHouseWoohoo()
        { }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<PetHouse>(SafeSingleton);
            interactions.Add<PetHouse>(RiskySingleton);
            interactions.Add<PetHouse>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, PetHouse.WoohooPetHouse.Definition, ProxyDefinition>(false);

            Tunings.Inject<Sim, ProxyDefinition, PetHouse, SafeDefinition>(true);
            Tunings.Inject<Sim, ProxyDefinition, PetHouse, RiskyDefinition>(true);
            Tunings.Inject<Sim, ProxyDefinition, PetHouse, TryForBabyDefinition>(true);
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
                if (WoohooHouse == null)
                {
                    return false;
                }
                if (!SafeToSync())
                {
                    return false;
                }
                PetHouse.WoohooPetHouseB entry = PetHouse.WoohooPetHouseB.Singleton.CreateInstance(Actor, Target, GetPriority(), Autonomous, CancellableByPlayer) as PetHouse.WoohooPetHouseB;
                entry.LinkedInteractionInstance = this;
                Target.InteractionQueue.Add(entry);
                Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                Actor.SynchronizationTarget = Target;
                Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                if (!WoohooHouse.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 10f))
                {
                    return false;
                }
                if (!Actor.RouteToSlotAndCheckInUse(WoohooHouse, Slot.RoutingSlot_3))
                {
                    if (!Actor.HasExitReason(ExitReason.RouteFailed))
                    {
                        WoohooHouse.SimLine.RemoveFromQueue(Actor);
                        WoohooHouse.PlayRouteFailAndWanderAway(Actor);
                        return false;
                    }
                    Actor.RemoveExitReason(ExitReason.RouteFailed);
                    if (!Actor.RouteToSlotAndCheckInUse(WoohooHouse, Slot.RoutingSlot_3))
                    {
                        WoohooHouse.SimLine.RemoveFromQueue(Actor);
                        WoohooHouse.PlayRouteFailAndWanderAway(Actor);
                        return false;
                    }
                }
                StandardEntry();
                WoohooHouse.AddToUseList(Actor);
                WoohooHouse.AddToUseList(Target);
                EnterStateMachine("PetHouse", "enter", "x");
                SetActor("petHouse", WoohooHouse);
                SetActor("y", Target);
                Animate("x", "getInWoohooX");
                Actor.ParentToSlot(WoohooHouse, WoohooHouse.GetContainmentSlotForActor(Actor));
                WoohooHouse.SimLine.RemoveFromQueue(Actor);
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Routed, 30f))
                {
                    Actor.UnParent();
                    Animate("x", "inHouseNeutral");
                    Animate("x", "exit");
                    StandardExit();
                    WoohooHouse.RemoveFromUseList(Actor);
                    WoohooHouse.RemoveFromUseList(Target);
                    return false;
                }
                BeginCommodityUpdates();
                (LinkedInteractionInstance as PetHouse.WoohooPetHouseB).BeginCommodityUpdates();
                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                EnterState("y", "enter");
                Animate("y", "getInWoohooY");
                AnimateJoinSims("woohoo");
                Actor.UnParent();
                AnimateSim("exitWoohoo");
                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);

                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                CommonWoohoo.RunPostWoohoo(Actor, Target, WoohooHouse, definition.GetStyle(this), definition.GetLocation(WoohooHouse), true);

                if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                {
                    CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                }

                StateMachineClient smc = StateMachineClient.Acquire(Actor, "PetHouse");
                smc.SetActor("petHouse", WoohooHouse);
                smc.SetActor("x", Actor);
                smc.EnterState("x", "inHouseNeutral");
                PetHouse.PetHousePosture posture = new PetHouse.PetHousePosture(WoohooHouse, Actor, smc);
                Actor.Posture = posture;
                ActorStayingInHouse = Actor.InteractionQueue.PushAsContinuation(PetHouse.LieDown.Singleton.CreateInstance(WoohooHouse, Actor, GetPriority(), Autonomous, CancellableByPlayer), true);
                if (!ActorStayingInHouse)
                {
                    posture.CancelPosture(Actor);
                }
                EndCommodityUpdates(true);
                (LinkedInteractionInstance as PetHouse.WoohooPetHouseB).EndCommodityUpdates(true);
                PlumbBob.Reparent();
                StandardExit();
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, PetHouseWoohoo, BaseHouseDefinition>
        {
            public ProxyDefinition(BaseHouseDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public abstract class BaseHouseDefinition : CommonWoohoo.PotentialDefinition<PetHouse>
        {
            public BaseHouseDefinition()
            { }
            public BaseHouseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.PetHouse;
            }

            public override Sim GetTarget(Sim actor, PetHouse target, InteractionInstance interaction)
            {
                PetHouse stall = target as PetHouse;

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

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                PetHouse house = obj as PetHouse;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                PetHouseWoohoo entry = new ProxyDefinition(this).CreateInstance(target, actor, priority, false, true) as PetHouseWoohoo;
                entry.WoohooHouse = house;
                actor.InteractionQueue.AddNext(entry);
            }

            protected override bool Satisfies(Sim actor, Sim target, PetHouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeDefinition : BaseHouseDefinition
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

            public override string GetInteractionName(Sim actor, PetHouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, PetHouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                try
                {
                    if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                    return CommonWoohoo.SatisfiesWoohoo(actor, target, "PetHouseWoohoo", isAutonomous, true, true, ref callback);
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

        public class RiskyDefinition : BaseHouseDefinition
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

            public override string GetInteractionName(Sim actor, PetHouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim a, Sim target, PetHouse obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesRisky(a, target, "PetHouseRisky", isAutonomous, true, ref greyedOutTooltipCallback);
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

        public class TryForBabyDefinition : BaseHouseDefinition
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

            public override string GetInteractionName(Sim actor, PetHouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, PetHouse obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "PetHouseTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
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
                get { return CommonWoohoo.WoohooLocation.PetHouse; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is PetHouse;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<PetHouse>(new Predicate<PetHouse>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<PetHouse>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if ((sim.IsHuman) || (sim.IsHorse)) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP5)) return false;
                }

                return Woohooer.Settings.mAutonomousPetHouse;
            }

            public bool TestUse(PetHouse obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (PetHouse obj in actor.LotCurrent.GetObjects<PetHouse>(new Predicate<PetHouse>(TestUse)))
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
