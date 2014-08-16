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
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
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
    public class TimeMachineWooHoo : TimeMachine.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        bool mIsMaster;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<TimeMachine, TimeMachine.WooHoo.Definition>(SafeSingleton);
            interactions.Add<TimeMachine>(RiskySingleton);
            interactions.Add<TimeMachine>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<TimeMachine, TimeMachine.WooHoo.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<TimeMachine, TimeMachine.WooHoo.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<TimeMachine, TimeMachine.WooHoo.Definition, TryForBabyDefinition>(false);

            TimeMachine.WooHoo.Singleton = SafeSingleton;
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
                    return TimeMachine.LocalizeString(name, new object[] { sim });
                }
            }

            InteractionInstanceParameters parameters = GetInteractionParameters();
            return InteractionDefinition.GetInteractionName(ref parameters);
        }

        public override bool Run()
        {
            try
            {
                string str2;
                ProxyDefinition definition = InteractionDefinition as ProxyDefinition;

                bool flag2 = false;
                if (mIsMaster)
                {
                    WooHooee.InteractionQueue.AddNext(new RouteToObject.Definition(WooHooer).CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));

                    flag2 = Target.RouteToMachine(Actor, false, null);
                }
                else
                {
                    flag2 = Target.RouteToMachine(Actor, false, WooHooer);
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

                Actor.SimDescription.Contactable = false;
                StandardEntry();
                EnterStateMachine("TimeMachine", "Enter", "x");
                SetActor("timeMachine", Target);
                SetParameter("isFuture", definition.Definition.TimePeriod == TimeMachine.TravelTimePeriod.Future);
                Target.SetMaterial("InUse");
                AddOneShotScriptEventHandler(0x66, ToggleHiddenAnimationEvent);
                if (mIsMaster)
                {
                    AddOneShotScriptEventHandler(0x3ee, OnEnterAnimationEvent);
                }
                else
                {
                    Target.EnableRoutingFootprint(Actor);
                }

                AnimateSim("GetIn");
                if ((WooHooee == null) || WooHooee.HasBeenDestroyed)
                {
                    AnimateSim("WooHoo");
                    Target.PushSimsFromFootprint(TimeMachine.sFootprintPathingHash, Actor, null, true);
                    SpeedTrap.Sleep(0x64);

                    string str;
                    Target.PickExitStateAndSound(definition.Definition.TimePeriod, out str, out mExitSound);
                    if (mIsMaster)
                    {
                        AddOneShotScriptEventHandler(0x3e9, OnExitAnimationEvent);
                        AddOneShotScriptEventHandler(0x3ef, OnExitAnimationEvent);
                    }
                    AddOneShotScriptEventHandler(0x67, ToggleHiddenAnimationEvent);
                    AnimateSim(str);
                    Target.SetMaterial("default");
                    AnimateSim("Exit");
                    StandardExit();
                    return true;
                }
                if (mIsMaster && !Actor.HasExitReason())
                {
                    TimeMachineWooHoo entry = definition.ProxyClone(WooHooer).CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as TimeMachineWooHoo;
                    entry.LinkedInteractionInstance = this;
                    entry.WooHooer = WooHooer;
                    entry.WooHooee = WooHooee;
                    WooHooee.InteractionQueue.AddNext(entry);
                }
                isWooHooing = true;
                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                AnimateSim("WooHoo");
                if (StartSync(mIsMaster))
                {
                    BeginCommodityUpdates();

                    try
                    {
                        DoTimedLoop(RandomUtil.GetFloat(TimeMachine.kWooHooMinutesMin, TimeMachine.kWooHooMinutesMax));
                    }
                    finally
                    {
                        EndCommodityUpdates(true);
                    }

                    if (mIsMaster)
                    {
                        CommonWoohoo.TestNakedOutfit(Woohooer.Settings.mNakedOutfitTimeMachine, WooHooer, WooHooee);

                        CommonWoohoo.RunPostWoohoo(Actor, WooHooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                        do
                        {
                            SpeedTrap.Sleep(0xa);
                        }
                        while (Target.UseCount > 0x1);

                        if (CommonPregnancy.IsSuccess(WooHooee, WooHooer, Autonomous, definition.GetStyle(this)))
                        {
                            if (WooHooer.IsMale)
                            {
                                Target.AddTryForBabyAlarm(WooHooer.SimDescription.SimDescriptionId, WooHooee.SimDescription.SimDescriptionId, definition.Definition.TimePeriod);
                            }
                            else
                            {
                                Target.AddTryForBabyAlarm(WooHooee.SimDescription.SimDescriptionId, WooHooer.SimDescription.SimDescriptionId, definition.Definition.TimePeriod);
                            }
                        }
                    }
                }
                else if (mIsMaster)
                {
                    Target.PushSimsFromFootprint(TimeMachine.sFootprintPathingHash, Actor, null, true);
                    SpeedTrap.Sleep(0x64);
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
                Target.PickExitStateAndSound(definition.Definition.TimePeriod, out str2, out mExitSound);
                if (mIsMaster)
                {
                    AddOneShotScriptEventHandler(0x3e9, new SacsEventHandler(OnExitAnimationEvent));
                    AddOneShotScriptEventHandler(0x3ef, new SacsEventHandler(OnExitAnimationEvent));
                }
                AddOneShotScriptEventHandler(0x67, new SacsEventHandler(ToggleHiddenAnimationEvent));
                AnimateSim(str2);
                Target.SetMaterial("default");
                AnimateSim("Exit");
                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                isWooHooing = false;
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<TimeMachine, TimeMachineWooHoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<TimeMachine>
        {
            public TimeMachine.TravelTimePeriod TimePeriod;

            public BaseDefinition()
            { }
            public BaseDefinition(Sim target)
                : this(GetRandomPeriod(), target)
            { }
            public BaseDefinition(TimeMachine.TravelTimePeriod timePeriod)
                : this(timePeriod, null)
            { }
            protected BaseDefinition(TimeMachine.TravelTimePeriod timePeriod, Sim target)
                : base(target)
            {
                TimePeriod = timePeriod;
            }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.TimeMachine;
            }

            protected static TimeMachine.TravelTimePeriod GetRandomPeriod()
            {
                List<TimeMachine.TravelTimePeriod> periods = new List<TimeMachine.TravelTimePeriod>();
                foreach (TimeMachine.TravelTimePeriod period in Enum.GetValues(typeof(TimeMachine.TravelTimePeriod)))
                {
                    periods.Add(period);
                }

                return RandomUtil.GetRandomObjectFromList(periods);
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                TimeMachine timeMachine = obj as TimeMachine;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                TimeMachineWooHoo instance = new ProxyDefinition(this).CreateInstance(timeMachine, actor, priority, false, true) as TimeMachineWooHoo;
                instance.WooHooer = actor;
                instance.WooHooee = target;
                instance.mIsMaster = true;
                actor.InteractionQueue.PushAsContinuation(instance, true);
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, TimeMachine target, List<InteractionObjectPair> results)
            {
                foreach (TimeMachine.TravelTimePeriod period in Enum.GetValues(typeof(TimeMachine.TravelTimePeriod)))
                {
                    if (period == TimeMachine.TravelTimePeriod.All) continue;

                    results.Add(new InteractionObjectPair(Clone(period), iop.Target));
                }
            }

            public override string GetInteractionName(Sim actor, TimeMachine target, InteractionObjectPair iop)
            {
                return TimeMachine.LocalizeString(TimePeriod.ToString(), new object[0x0]); ;
            }

            protected override bool Satisfies(Sim actor, Sim target, TimeMachine obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }

            public override bool Test(Sim a, TimeMachine target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((target.Repairable != null) && (target.Repairable.Broken)) return false;

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }

            public abstract InteractionDefinition Clone(TimeMachine.TravelTimePeriod period);
        }

        public class SafeDefinition : BaseDefinition
        {
            public SafeDefinition()
            { }
            public SafeDefinition(Sim target)
                : base(target)
            { }
            public SafeDefinition(TimeMachine.TravelTimePeriod timePeriod)
                : base(timePeriod)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.LocalizeEAString(isFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]) };
            }

            protected override bool Satisfies(Sim actor, Sim target, TimeMachine obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "TimeMachineWoohoo", isAutonomous, true, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeDefinition(target));
            }
            public override InteractionDefinition Clone(TimeMachine.TravelTimePeriod period)
            {
                return new SafeDefinition(period);
            }
        }

        public class RiskyDefinition : BaseDefinition
        {
            public RiskyDefinition()
            { }
            public RiskyDefinition(Sim target)
                : base(target)
            { }
            public RiskyDefinition(TimeMachine.TravelTimePeriod timePeriod)
                : base(timePeriod)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.LocalizeEAString(isFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(CASAgeGenderFlags.Human)] }) };
            }

            protected override bool Satisfies(Sim actor, Sim target, TimeMachine obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "TimeMachineRisky", isAutonomous, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
            public override InteractionDefinition Clone(TimeMachine.TravelTimePeriod period)
            {
                return new RiskyDefinition(period);
            }
        }

        public class TryForBabyDefinition : BaseDefinition
        {
            public TryForBabyDefinition()
            { }
            public TryForBabyDefinition(Sim target)
                : base(target)
            { }
            public TryForBabyDefinition(TimeMachine.TravelTimePeriod timePeriod)
                : base(timePeriod)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.LocalizeEAString(isFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]) };
            }

            protected override bool Satisfies(Sim actor, Sim target, TimeMachine obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "TimeMachineTryForBaby", isAutonomous, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new TryForBabyDefinition(target));
            }
            public override InteractionDefinition Clone(TimeMachine.TravelTimePeriod period)
            {
                return new TryForBabyDefinition(period);
            }
        }

        public class LocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.TimeMachine; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is TimeMachine;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<TimeMachine>(new Predicate<TimeMachine>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<TimeMachine>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP2)) return false;
                }

                return Woohooer.Settings.mAutonomousTimeMachine;
            }

            public bool TestUse(TimeMachine obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (TimeMachine obj in actor.LotCurrent.GetObjects<TimeMachine>(new Predicate<TimeMachine>(TestUse)))
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
