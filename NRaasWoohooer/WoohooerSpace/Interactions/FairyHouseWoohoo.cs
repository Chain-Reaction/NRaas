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
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class FairyHouseWoohoo : FairyHouse.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public FairyHouseWoohoo()
        { }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<FairyHouse, FairyHouse.WooHooInitiator.Definition>(SafeSingleton);
            interactions.Add<FairyHouse>(RiskySingleton);
            interactions.Add<FairyHouse>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<FairyHouse, FairyHouse.WooHoo.Definition, ProxyDefinition>(false);

            Woohooer.InjectAndReset<FairyHouse, FairyHouse.WooHooInitiator.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<FairyHouse, FairyHouse.WooHooInitiator.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<FairyHouse, FairyHouse.WooHooInitiator.Definition, TryForBabyDefinition>(false);

            FairyHouse.WooHooInitiator.SingletonWooHoo = SafeSingleton;
            FairyHouse.WooHooInitiator.SingletonTryForBaby = TryForBabySingleton;
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
                if (!(Actor.Posture is FairyHouse.FairyHousePosture))
                {
                    return false;
                }

                mLinkedActor = LinkedInteractionInstance.InstanceActor;
                if (!StartSync(mIsMaster))
                {
                    if (!Target.IsNextInteractionAFairyHouseInteraction(Actor))
                    {
                        Target.PushGetOutAsContinuation(Actor);
                    }
                    return false;
                }

                StandardEntry(false);
                BeginCommodityUpdates();
                mCurrentStateMachine = Actor.Posture.CurrentStateMachine;
                if (mIsMaster)
                {
                    SetActorAndEnter("y", Actor, "WooHoo");
                    SetActorAndEnter("fairyHouse", Target, "Enter");
                    RockGemMetalBase.HandleNearbyWoohoo(Target, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                    Animate("fairyHouse", "WooHoo");
                    Animate("fairyHouse", "Exit");

                    IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                    CommonWoohoo.RunPostWoohoo(Actor, mLinkedActor, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                    if (CommonPregnancy.IsSuccess(Actor, mLinkedActor, Autonomous, definition.GetStyle(this)))
                    {
                        CommonPregnancy.Impregnate(Actor, mLinkedActor, Autonomous, definition.GetStyle(this));
                    }
                }
                else
                {
                    DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }

                EventTracker.SendEvent(EventTypeId.kWooHooedInTheFairyHouse, Actor, mLinkedActor);
                EndCommodityUpdates(true);
                StandardExit(false, false);
                FinishLinkedInteraction(mIsMaster);
                WaitForSyncComplete();

                LongTermRelationshipTypes longTermRelationship = Relationship.GetLongTermRelationship(Actor, mLinkedActor);
                Relationship relationship = Relationship.Get(Actor, mLinkedActor, true);
                if (mIsMaster && (relationship != null))
                {
                    relationship.LTR.UpdateLiking(FairyHouse.kLTRIncreaseOnWoohoo);
                    LongTermRelationshipTypes currentLTR = relationship.CurrentLTR;
                    SocialComponent.SetSocialFeedback(CommodityTypes.Friendly, Actor, true, 0x0, longTermRelationship, currentLTR);
                    SocialComponent.SetSocialFeedback(CommodityTypes.Friendly, mLinkedActor, true, 0x0, longTermRelationship, currentLTR);
                }

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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<FairyHouse, FairyHouseWoohoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<FairyHouse>
        {
            public BaseDefinition()
            { }
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.FairyHouse;
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                FairyHouse house = obj as FairyHouse;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                FairyHouseWoohoo instance = new ProxyDefinition(this).CreateInstance(house, actor, priority, false, true) as FairyHouseWoohoo;
                instance.IsMaster = true;

                FairyHouseWoohoo hoo2 = new ProxyDefinition(this).CreateInstance(house, target, priority, false, true) as FairyHouseWoohoo;
                if (actor.InteractionQueue.PushAsContinuation(instance, true))
                {
                    target.InteractionQueue.PushAsContinuation(hoo2, true);
                    instance.LinkedInteractionInstance = hoo2;
                }
            }

            protected override bool Satisfies(Sim actor, Sim target, FairyHouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return ((actor.SimDescription.IsFairy) || (target.SimDescription.IsFairy));
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

            public override string GetInteractionName(Sim actor, FairyHouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, FairyHouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "FairyHouseWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, FairyHouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, FairyHouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "FairyHouseRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, FairyHouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, FairyHouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "FairyHouseTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.FairyHouse; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is FairyHouse;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<FairyHouse>(new Predicate<FairyHouse>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<FairyHouse>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP7)) return false;
                }

                return Woohooer.Settings.mAutonomousFairyHouse;
            }

            public bool TestUse(FairyHouse obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (FairyHouse obj in actor.LotCurrent.GetObjects<FairyHouse>(new Predicate<FairyHouse>(TestUse)))
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
                        return new FairyHouseWoohoo.SafeDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new FairyHouseWoohoo.RiskyDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new FairyHouseWoohoo.TryForBabyDefinition(target);
                }

                return null;
            }
        }
    }
}
