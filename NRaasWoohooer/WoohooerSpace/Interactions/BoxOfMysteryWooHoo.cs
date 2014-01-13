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
using Sims3.Gameplay.Objects.Misc;
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
    public class BoxOfMysteryWooHoo : BoxOfMystery.WooHooInBoxOfMystery, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        bool mIsMaster;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<BoxOfMystery, BoxOfMystery.WooHooInBoxOfMystery.Definition>(SafeSingleton);
            interactions.Add<BoxOfMystery>(RiskySingleton);
            interactions.Replace<BoxOfMystery, BoxOfMystery.TryForBaby.Definition>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<BoxOfMystery, BoxOfMystery.WooHooInBoxOfMystery.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<BoxOfMystery, BoxOfMystery.TryForBaby.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<BoxOfMystery, BoxOfMystery.TryForBaby.Definition, TryForBabyDefinition>(false);

            BoxOfMystery.WooHooInBoxOfMystery.Singleton = SafeSingleton;

            BoxOfMystery.TryForBaby.SingletonPregnency = TryForBabySingleton;
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                bool flag2 = false;
                if (mIsMaster)
                {
                    flag2 = Actor.RouteToSlotAndCheckInUse(Target, Slot.RoutingSlot_1);
                    mFail = RandomUtil.RandomChance(BoxOfMystery.kChanceToFailWooHoo);
                }
                else
                {
                    Route r = Actor.CreateRoute();
                    r.AddObjectToIgnoreForRoute(mWooHooer.ObjectId);
                    r.PlanToSlot(Target, Slot.RoutingSlot_1);
                    flag2 = Actor.DoRoute(r);
                }
                if (!flag2)
                {
                    Actor.AddExitReason(ExitReason.RouteFailed);
                    return false;
                }

                mRouteComplete = flag2;
                if (mIsMaster && Target.ActorsUsingMe.Contains(mWooHooee))
                {
                    return false;
                }

                if (mIsMaster && !Actor.HasExitReason())
                {
                    mLinkedInstance = definition.ProxyClone(mWooHooer).CreateInstance(Target, mWooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as BoxOfMystery.WooHooInBoxOfMystery;
                    mLinkedInstance.LinkedInteractionInstance = this;
                    mLinkedInstance.mWooHooer = mWooHooer;
                    mLinkedInstance.mWooHooee = mWooHooee;
                    mLinkedInstance.mFail = mFail;
                    mWooHooee.InteractionQueue.AddNext(mLinkedInstance);
                }

                StandardEntry();
                EnterStateMachine("BoxOfMystery", "Enter", "x");
                SetActor("box", Target);
                AnimateSim("GetInBox");
                if (mIsMaster)
                {
                    Actor.SetOpacity(0f, 0f);
                }

                while ((mIsMaster && !mLinkedInstance.mRouteComplete) && (!mWooHooee.HasExitReason(ExitReason.RouteFailed) && mWooHooee.InteractionQueue.HasInteraction(mLinkedInstance)))
                {
                    SpeedTrap.Sleep(0x0);
                }

                if (mIsMaster && (mWooHooee.HasExitReason(ExitReason.RouteFailed) || !mWooHooee.InteractionQueue.HasInteraction(mLinkedInstance)))
                {
                    Actor.SetOpacity(1f, 0f);
                    AnimateSim("Exit");
                    StandardExit();
                    return false;
                }

                if (mIsMaster)
                {
                    List<Sim> exceptions = new List<Sim>();
                    exceptions.Add(mWooHooee);
                    Target.EnableFootprintAndPushSims(BoxOfMystery.sFootprintPathingHash, Actor, exceptions, false);
                }

                if (!StartSync(mIsMaster))
                {
                    if (mIsMaster)
                    {
                        if (LinkedInteractionInstance != null)
                        {
                            LinkedInteractionInstance.InstanceActor.AddExitReason(ExitReason.CanceledByScript);
                            Actor.AddExitReason(ExitReason.CanceledByScript);
                        }

                        do
                        {
                            SpeedTrap.Sleep(0xa);
                        }
                        while (Target.UseCount > 0x1);
                    }
                }
                else
                {
                    if (mIsMaster)
                    {
                        mLinkedInstance.mAnimating = true;
                    }
                    Actor.SetOpacity(0f, 0f);
                    BeginCommodityUpdates();
                    if (!mIsMaster)
                    {
                        while (mAnimating)
                        {
                            SpeedTrap.Sleep(0x1);
                        }
                    }
                    else
                    {
                        AnimateSim("TryForWooHoo");
                        mLinkedInstance.mAnimating = false;
                    }
                    EndCommodityUpdates(true);
                    if (mIsMaster)
                    {
                        CommonWoohoo.RunPostWoohoo(Actor, mWooHooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                        if (CommonPregnancy.IsSuccess(Actor, mWooHooee, Autonomous, definition.GetStyle(this)))
                        {
                            CommonPregnancy.Impregnate(Actor, mWooHooee, Autonomous, definition.GetStyle(this));
                        }

                        if (mFail)
                        {
                            Actor.SetOpacity(1f, 0f);
                            AnimateSim("FailExit");
                            Route route2 = Actor.CreateRouteTurnToFace(Target.Position);
                            route2.AddObjectToIgnoreForRoute(mWooHooee.ObjectId);
                            Actor.DoRoute(route2);
                            AnimateSim("Panic");
                            Actor.RouteAway(1f, 3f, false, GetPriority(), Autonomous, false, true, RouteDistancePreference.PreferNearestToRouteOrigin);
                        }
                        else
                        {
                            do
                            {
                                SpeedTrap.Sleep(0xa);
                            }
                            while (Target.UseCount > 0x1);
                        }
                    }
                    else if (mFail)
                    {
                        do
                        {
                            SpeedTrap.Sleep(0xa);
                        }
                        while (Target.UseCount > 0x1);

                        Actor.SetOpacity(1f, 0f);
                        AnimateSim("SuprisedExit");
                    }
                }

                Actor.SetOpacity(1f, 0f);
                AnimateSim("Exit");
                StandardExit();
                if (!mIsMaster)
                {
                    Actor.RouteAway(1f, 3f, false, GetPriority(), Autonomous, false, true, RouteDistancePreference.PreferNearestToRouteOrigin);
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<BoxOfMystery, BoxOfMysteryWooHoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<BoxOfMystery>
        {
            public BaseDefinition()
            {}
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.BoxOfMystery;
            }

            public override Sim GetTarget(Sim actor, BoxOfMystery target, InteractionInstance interaction)
            {
                if (target.ActorsUsingMe.Count == 1)
                {
                    return target.ActorsUsingMe[0];
                }

                return base.GetTarget(actor, target, interaction);
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                BoxOfMystery box = obj as BoxOfMystery;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                BoxOfMysteryWooHoo instance = new ProxyDefinition(this).CreateInstance(box, actor, priority, false, true) as BoxOfMysteryWooHoo;
                instance.mWooHooer = actor;
                instance.mWooHooee = target;
                instance.mIsMaster = true;
                actor.InteractionQueue.PushAsContinuation(instance, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, BoxOfMystery obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
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

            public override string GetInteractionName(Sim actor, BoxOfMystery target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, BoxOfMystery obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "BoxOfMysteryWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, BoxOfMystery target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, BoxOfMystery obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "BoxOfMysteryRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, BoxOfMystery target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, BoxOfMystery obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "BoxOfMysteryTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.BoxOfMystery; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is BoxOfMystery;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<BoxOfMystery>(new Predicate<BoxOfMystery>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<BoxOfMystery>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP6)) return false;
                }

                return Woohooer.Settings.mAutonomousBoxOfMystery;
            }

            public bool TestUse(BoxOfMystery obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (BoxOfMystery obj in actor.LotCurrent.GetObjects<BoxOfMystery>(new Predicate<BoxOfMystery>(TestUse)))
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
