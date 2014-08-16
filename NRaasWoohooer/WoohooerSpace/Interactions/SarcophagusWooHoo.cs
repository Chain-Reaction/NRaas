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
using Sims3.Gameplay.Objects.TombObjects;
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
    public class SarcophagusWooHoo : Sarcophagus.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        bool mIsMaster;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sarcophagus, Sarcophagus.WooHoo.Definition>(SafeSingleton);
            interactions.Add<Sarcophagus>(RiskySingleton);
            interactions.Replace<Sarcophagus, Sarcophagus.TryForBaby.Definition>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sarcophagus, Sarcophagus.WooHoo.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<Sarcophagus, Sarcophagus.TryForBaby.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<Sarcophagus, Sarcophagus.TryForBaby.Definition, TryForBabyDefinition>(false);

            Sarcophagus.WooHoo.Singleton = SafeSingleton;

            Sarcophagus.TryForBaby.Singleton = TryForBabySingleton;
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                bool flag2 = false;
                if (mIsMaster)
                {
                    WooHooee.InteractionQueue.AddNext(new RouteToObject.Definition(WooHooer).CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));

                    flag2 = Target.RouteToSarcaphagus(Actor, null);
                }
                else
                {
                    flag2 = Target.RouteToSarcaphagus(Actor, WooHooer);
                }

                if (!flag2)
                {
                    Actor.AddExitReason(ExitReason.Finished);
                    return false;
                }

                if (mIsMaster && Target.ActorsUsingMe.Contains(WooHooee))
                {
                    return false;
                }

                StandardEntry();
                EnterStateMachine("Sarcophagus", "Enter", "x", "sarcophagus");
                SetParameter("isMummy", Actor.SimDescription.IsMummy);
                RegisterForHidingEvents();
                AnimateSim("Open");
                if (mIsMaster && !Actor.HasExitReason())
                {
                    SarcophagusWooHoo entry = definition.ProxyClone(WooHooer).CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as SarcophagusWooHoo;
                    entry.LinkedInteractionInstance = this;
                    entry.WooHooer = WooHooer;
                    entry.WooHooee = WooHooee;
                    WooHooee.InteractionQueue.AddNext(entry);
                }

                AnimateSim("WooHooWait");
                if (StartSync(mIsMaster))
                {
                    BeginCommodityUpdates();

                    try
                    {
                        if (mIsMaster)
                        {
                            CommonWoohoo.TestNakedOutfit(Woohooer.Settings.mNakedOutfitSarcophagus, WooHooer, WooHooee);

                            mWooHooFX = VisualEffect.Create("sarcophagusCairoWoohooPetals");
                            mWooHooFX.ParentTo(Target, Slot.FXJoint_0);
                            mWooHooFX.Start();
                            Audio.StartObjectSound(Target.ObjectId, "sarcoph_woohoo", false);
                        }
                        isWooHooing = true;
                        RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                        AnimateSim("WooHoo");
                        RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                        isWooHooing = false;
                    }
                    finally
                    {
                        EndCommodityUpdates(true);
                    }

                    if (mIsMaster)
                    {
                        mWooHooFX.Stop();

                        CommonWoohoo.RunPostWoohoo(Actor, WooHooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                        do
                        {
                            SpeedTrap.Sleep(0xa);
                        }
                        while (Target.UseCount > 0x1);
                    }
                }
                else if (mIsMaster)
                {
                    if (LinkedInteractionInstance != null)
                    {
                        LinkedInteractionInstance.InstanceActor.AddExitReason(ExitReason.CanceledByScript);
                    }

                    do
                    {
                        SpeedTrap.Sleep(0xa);
                    }
                    while (Target.UseCount > 0x1);
                }

                PrepSimForExit(true);
                Target.EnableRoutingFootprint(Actor);
                AnimateSim("Exit");
                StandardExit();
                if (mIsMaster)
                {
                    if (CommonPregnancy.IsSuccess(WooHooer, WooHooee, Autonomous, definition.GetStyle(this)))
                    {
                        CommonPregnancy.Impregnate(WooHooer, WooHooee, Autonomous, definition.GetStyle(this));
                    }
                }
                else
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sarcophagus, SarcophagusWooHoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<Sarcophagus>
        {
            public BaseDefinition()
            {}
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Sarcophagus;
            }

            public override Sim GetTarget(Sim actor, Sarcophagus target, InteractionInstance interaction)
            {
                if (target.ActorsUsingMe.Count == 1)
                {
                    return target.ActorsUsingMe[0];
                }

                return base.GetTarget(actor, target, interaction);
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                Sarcophagus sarcophagus = obj as Sarcophagus;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                SarcophagusWooHoo instance = new ProxyDefinition(this).CreateInstance(sarcophagus, actor, priority, false, true) as SarcophagusWooHoo;
                instance.WooHooer = actor;
                instance.WooHooee = target;
                instance.mIsMaster = true;
                actor.InteractionQueue.PushAsContinuation(instance, true);
            }

            public override bool Test(Sim a, Sarcophagus target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((!target.mLookedIn) && (TombRoomManager.IsObjectInATombRoom(target)))
                {
                    return false;
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }

            protected override bool Satisfies(Sim actor, Sim target, Sarcophagus obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (isAutonomous)
                {
                    if (!Sarcophagus.DoSimsMeetWooHooRequirements(actor, target)) return false;
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

            public override string GetInteractionName(Sim actor, Sarcophagus target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, Sarcophagus obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "SarcophagusWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, Sarcophagus target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, Sarcophagus obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "SarcophagusRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, Sarcophagus target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, Sarcophagus obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "SarcophagusTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.Sarcophagus; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is Sarcophagus;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<Sarcophagus>(new Predicate<Sarcophagus>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<ISarcophagus>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP1)) return false;
                }

                return Woohooer.Settings.mAutonomousSarcophagus;
            }

            public bool TestUse(Sarcophagus obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (Sarcophagus obj in actor.LotCurrent.GetObjects<Sarcophagus>(new Predicate<Sarcophagus>(TestUse)))
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
