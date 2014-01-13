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
using Sims3.Gameplay.TimeTravel;
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
    public class TimePortalWooHoo : TimePortal.Travel, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        Sim mWoohooee;

        bool mIsMaster;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<TimePortal>(SafeSingleton);
            interactions.Add<TimePortal>(RiskySingleton);
            interactions.Add<TimePortal>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<TimePortal, TimePortal.Travel.Definition, SafeDefinition>(true);
            Woohooer.InjectAndReset<TimePortal, TimePortal.Travel.Definition, RiskyDefinition>(true);
            Woohooer.InjectAndReset<TimePortal, TimePortal.Travel.Definition, TryForBabyDefinition>(true);
        }

        private void CameraShakeEventEx(StateMachineClient smc, IEvent evt)
        {
            Target.ShakeCamera();
            List<Sim> simsThatShouldNotReact = new List<Sim>(0x2);
            simsThatShouldNotReact.Add(Actor);
            simsThatShouldNotReact.Add(mWoohooee);
            Target.MakeNearbySimsReact(simsThatShouldNotReact);
        }

        private void SwitchActiveState(StateMachineClient sender, IEvent evt)
        {
            Target.SwitchActiveState();
        }

        public override void Cleanup()
        {
            base.Cleanup();

            Actor.FadeIn();
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                if (Target.IsBroken)
                {
                    return false;
                }

                int num;
                if (mIsMaster)
                {
                    if (!Actor.RouteToSlotListAndCheckInUse(Target, TimePortal.kRoutingSlots, out num))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Actor.RouteToSlotList(Target, TimePortal.kRoutingSlots, out num))
                    {
                        return false;
                    }
                }

                if (Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                {
                    return false;
                }

                StandardEntry();

                EnterStateMachine("timeportal", "Enter", "x", "portal");
                AddPersistentScriptEventHandler(0xc9, CameraShakeEventEx);

                if (mIsMaster && !Actor.HasExitReason())
                {
                    TimePortalWooHoo entry = definition.ProxyClone(Actor).CreateInstance(Target, mWoohooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as TimePortalWooHoo;
                    entry.LinkedInteractionInstance = this;
                    entry.mWoohooee = mWoohooee;
                    mWoohooee.InteractionQueue.AddNext(entry);
                }

                Skill futureSkill = Actor.SkillManager.AddElement(SkillNames.Future);
                if (futureSkill.SkillLevel >= 0x3)
                {
                    AnimateSim("Jump In");
                }
                else
                {
                    AnimateSim("Apprehensive");
                }

                bool succeeded = true;

                if (StartSync(mIsMaster))
                {
                    BeginCommodityUpdates();

                    RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);

                    if (mIsMaster)
                    {
                        CommonWoohoo.TestNakedOutfit(Woohooer.Settings.mNakedOutfitTimePortal, Actor, mWoohooee);

                        TimePortal.PortalState previousState = Target.State;

                        List<TimePortal.PortalState> choices = new List<TimePortal.PortalState>();
                        foreach (TimePortal.PortalState state in Enum.GetValues(typeof(TimePortal.PortalState)))
                        {
                            if (state == TimePortal.PortalState.Invalid_State) continue;

                            choices.Add(state);
                        }

                        int count = 0;
                        while (Target.ActorsUsingMe.Contains(mWoohooee))
                        {
                            TimePortalWooHoo interaction = LinkedInteractionInstance as TimePortalWooHoo;
                            if (interaction == null) break;

                            if (mWoohooee.HasExitReason(ExitReason.Canceled)) break;

                            if (mIsMaster)
                            {
                                if (count > 30) break;
                                count++;

                                Target.UpdateState(RandomUtil.GetRandomObjectFromList(choices));

                                if (RandomUtil.RandomChance(5))
                                {
                                    CameraShakeEventEx(null, null);
                                }
                            }

                            SpeedTrap.Sleep(10);
                        }

                        CommonWoohoo.RunPostWoohoo(Actor, mWoohooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                        Target.UpdateState(previousState);
                    }
                    else
                    {
                        do
                        {
                            SpeedTrap.Sleep(0xa);
                        }
                        while (Target.UseCount > 0x1);
                    }

                    RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                }
                else if (mIsMaster)
                {
                    if (LinkedInteractionInstance != null)
                    {
                        LinkedInteractionInstance.InstanceActor.AddExitReason(ExitReason.CanceledByScript);
                    }
                }

                if (((succeeded && Actor.TraitManager.HasElement(TraitNames.Unstable)) && (!Actor.BuffManager.HasElement(BuffNames.FeelingOutOfSorts) && !Actor.BuffManager.HasElement(BuffNames.ImpendingEpisode))) && (!Actor.BuffManager.HasElement(BuffNames.Delusional) && RandomUtil.RandomChance01(kUnstableTraitChance)))
                {
                    Actor.BuffManager.AddElement(BuffNames.FeelingOutOfSorts, Origin.FromUnstableTrait);
                }

                if (futureSkill.SkillLevel >= 0x3)
                {
                    AnimateSim("Exit");
                }
                else
                {
                    AnimateSim("Spit Out");
                }

                EndCommodityUpdates(succeeded);
                StandardExit();

                if (!mIsMaster)
                {
                    if (CommonPregnancy.IsSuccess(Actor, mWoohooee, Autonomous, definition.GetStyle(this)))
                    {
                        CommonPregnancy.Impregnate(Actor, mWoohooee, Autonomous, definition.GetStyle(this));
                    }

                    Actor.RouteAway(1f, 3f, false, GetPriority(), Autonomous, false, true, RouteDistancePreference.PreferNearestToRouteOrigin);
                }
                else
                {
                    Target.StopActiveFX();
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<TimePortal, TimePortalWooHoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<TimePortal>
        {
            public BaseDefinition()
            {}
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.TimePortal;
            }

            public override Sim GetTarget(Sim actor, TimePortal target, InteractionInstance interaction)
            {
                if (target.ActorsUsingMe.Count == 1)
                {
                    return target.ActorsUsingMe[0];
                }

                return base.GetTarget(actor, target, interaction);
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                TimePortal station = obj as TimePortal;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                TimePortalWooHoo instance = new ProxyDefinition(this).CreateInstance(station, actor, priority, false, true) as TimePortalWooHoo;
                instance.mWoohooee = target;
                instance.mIsMaster = true;
                actor.InteractionQueue.PushAsContinuation(instance, true);
            }

            public override bool Test(Sim a, TimePortal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!TimePortal.sTimeTravelerHasBeenSummoned)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("sTimeTravelerHasBeenSummoned");
                    return false;
                }

                if (!target.Active)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not Active");
                    return false;
                }

                if (target.IsBroken)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Broken");
                    return false;
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }

            protected override bool Satisfies(Sim actor, Sim target, TimePortal obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
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

            public override string GetInteractionName(Sim actor, TimePortal target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, TimePortal obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "TimePortalWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, TimePortal target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, TimePortal obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "TimePortalRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, TimePortal target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, TimePortal obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "TimePortalTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.TimePortal; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is TimePortal;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<TimePortal>(new Predicate<TimePortal>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<ITimePortal>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP11)) return false;
                }

                return Woohooer.Settings.mAutonomousTimePortal;
            }

            public bool TestUse(TimePortal obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (TimePortal obj in actor.LotCurrent.GetObjects<TimePortal>(new Predicate<TimePortal>(TestUse)))
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
