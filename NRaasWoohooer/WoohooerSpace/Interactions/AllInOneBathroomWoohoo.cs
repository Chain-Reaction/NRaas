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
using Sims3.Gameplay.Objects.Plumbing;
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
    public class AllInOneBathroomWoohoo : AllInOneBathroom.WoohooInAllInOneBathroom, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<AllInOneBathroom, AllInOneBathroom.PushWoohooInAllInOneBathroom.Definition>(SafeSingleton);
            interactions.Add<AllInOneBathroom>(RiskySingleton);
            interactions.Add<AllInOneBathroom>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, AllInOneBathroom.WoohooInAllInOneBathroom.Definition, ProxyDefinition>(false);

            InteractionTuning tuning = Tunings.GetTuning<Sim, AllInOneBathroom.WoohooInAllInOneBathroom.Definition>();
            if (tuning != null)
            {
                tuning.Availability.RemoveFlags(Availability.FlagField.DisallowedIfPregnant);
            }

            tuning = Tunings.GetTuning<Sim, AllInOneBathroom.WoohooInAllInOneBathroomB.Definition>();
            if (tuning != null)
            {
                tuning.Availability.RemoveFlags(Availability.FlagField.DisallowedIfPregnant);
                tuning.Availability.Teens = true;
                tuning.Availability.AddFlags(Availability.FlagField.AllowGreetedSims);
                tuning.Availability.AddFlags(Availability.FlagField.AllowNonGreetedSimsIfObjectOutsideAutonomous);
                tuning.Availability.AddFlags(Availability.FlagField.AllowNonGreetedSimsIfObjectOutsideUserDirected);
                tuning.Availability.AddFlags(Availability.FlagField.AllowOnCommunityLots);
                tuning.Availability.AddFlags(Availability.FlagField.AllowOnAllLots);
            }

            Woohooer.InjectAndReset<AllInOneBathroom, ProxyDefinition, SafeDefinition>(false);
            Woohooer.InjectAndReset<AllInOneBathroom, ProxyDefinition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<AllInOneBathroom, ProxyDefinition, TryForBabyDefinition>(false);

            AllInOneBathroom.PushWoohooInAllInOneBathroom.WoohooSingleton = SafeSingleton;
            AllInOneBathroom.PushWoohooInAllInOneBathroom.TryForBabySingleton = TryForBabySingleton;
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                if (WoohooObject == null)
                {
                    return false;
                }

                if (!SafeToSync())
                {
                    return false;
                }

                AllInOneBathroom.WoohooInAllInOneBathroomB entry = AllInOneBathroom.WoohooInAllInOneBathroomB.Singleton.CreateInstance(Actor, Target, GetPriority(), Autonomous, CancellableByPlayer) as AllInOneBathroom.WoohooInAllInOneBathroomB;
                entry.TryForBaby = TryForBaby;
                entry.LinkedInteractionInstance = this;
                Target.InteractionQueue.Add(entry);
                Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                Actor.SynchronizationTarget = Target;
                Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                if (!WoohooObject.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 10f))
                {
                    return false;
                }

                if (!Actor.RouteToSlotAndCheckInUse(WoohooObject, Slot.RoutingSlot_1))
                {
                    return false;
                }

                StandardEntry();
                WoohooObject.AddToUseList(Actor);
                WoohooObject.AddToUseList(Target);
                BeginCommodityUpdates();

                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Routed, 30f))
                {
                    WoohooObject.RemoveFromUseList(Actor);
                    WoohooObject.RemoveFromUseList(Target);
                    EndCommodityUpdates(false);
                    StandardExit();
                    return false;
                }

                EnterStateMachine("AllInOneBathroom", "Enter", "x");
                SetActor("bathroom", WoohooObject);
                SetActor("y", Target);
                AddOneShotScriptEventHandler(0x64, AnimationCallback);
                AddOneShotScriptEventHandler(0x65, AnimationCallback);
                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);

                if (mReactToSocialBroadcasterActor == null)
                {
                    mReactToSocialBroadcasterActor = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                }

                if (mReactToSocialBroadcasterTarget == null)
                {
                    mReactToSocialBroadcasterTarget = new ReactionBroadcaster(Target, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                }

                Animate("x", "WooHoo");

                List<Sim> exceptions = new List<Sim>();
                exceptions.Add(Target);
                WoohooObject.PushSimsFromFootprint(0x31229a4d, Actor, exceptions, true);
                WoohooObject.PushSimsFromFootprint(0x31229a4e, Actor, exceptions, true);
                Animate("x", "Exit");

                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);

                CommonWoohoo.RunPostWoohoo(Actor, Target, WoohooObject, definition.GetStyle(this), definition.GetLocation(WoohooObject), true);

                if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                {
                    Pregnancy pregnancy = CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                    if (pregnancy != null)
                    {
                        if (RandomUtil.RandomChance(AllInOneBathroom.kChanceOfHydrophobicTrait))
                        {
                            pregnancy.SetForcedBabyTrait(TraitNames.Hydrophobic);
                        }
                    }
                }

                WoohooObject.RemoveFromUseList(Actor);
                WoohooObject.RemoveFromUseList(Target);
                WoohooObject.SimLine.RemoveFromQueue(Actor);
                EndCommodityUpdates(true);
                StandardExit();

                EventTracker.SendEvent(EventTypeId.kWooHooInAllInOneBathroom, Actor, Target);

                VisitSituation situation = VisitSituation.FindVisitSituationInvolvingGuest(Actor);
                VisitSituation situation2 = VisitSituation.FindVisitSituationInvolvingGuest(Target);
                if ((situation != null) && (situation2 != null))
                {
                    situation.GuestStartingInappropriateAction(Actor, 3.5f);
                    situation2.GuestStartingInappropriateAction(Target, 3.5f);
                }

                Relationship.Get(Actor, Target, true).LTR.UpdateLiking(AllInOneBathroom.kLTRGainFromWoohooInAllInOneBathroom);
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, AllInOneBathroomWoohoo, BaseAllInOneBathroomDefinition>
        {
            public ProxyDefinition(BaseAllInOneBathroomDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public abstract class BaseAllInOneBathroomDefinition : CommonWoohoo.PotentialDefinition<AllInOneBathroom>
        {
            public BaseAllInOneBathroomDefinition()
            { }
            public BaseAllInOneBathroomDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.AllInOneBathroom;
            }

            public override Sim GetTarget(Sim actor, AllInOneBathroom target, InteractionInstance interaction)
            {
                AllInOneBathroom stall = target as AllInOneBathroom;

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
                AllInOneBathroom house = obj as AllInOneBathroom;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                AllInOneBathroomWoohoo entry = new ProxyDefinition(this).CreateInstance(target, actor, priority, false, true) as AllInOneBathroomWoohoo;
                entry.WoohooObject = house;
                actor.InteractionQueue.PushAsContinuation(entry, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, AllInOneBathroom obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeDefinition : BaseAllInOneBathroomDefinition
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

            public override string GetInteractionName(Sim actor, AllInOneBathroom target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, AllInOneBathroom obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "AllInOneBathroomWoohoo", isAutonomous, true, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeDefinition(target));
            }
        }

        public class RiskyDefinition : BaseAllInOneBathroomDefinition
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

            public override string GetInteractionName(Sim actor, AllInOneBathroom target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Woohooer.Settings.GetRiskyChanceText(actor);
            }

            protected override bool Satisfies(Sim a, Sim target, AllInOneBathroom obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesRisky(a, target, "AllInOneBathroomRisky", isAutonomous, true, ref greyedOutTooltipCallback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
        }

        public class TryForBabyDefinition : BaseAllInOneBathroomDefinition
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

            public override string GetInteractionName(Sim actor, AllInOneBathroom target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, AllInOneBathroom obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(a, target, "AllInOneBathroomTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
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
                get { return CommonWoohoo.WoohooLocation.AllInOneBathroom; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is AllInOneBathroom;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<AllInOneBathroom>(new Predicate<AllInOneBathroom>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<AllInOneBathroom>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if ((!GameUtils.IsInstalled(ProductVersion.EP10)) &&
                        (!GameUtils.IsInstalled(ProductVersion.EP11)))
                    {                        
                        return false;
                    }
                }

                return Woohooer.Settings.mAutonomousAllInOneBathroom;
            }

            public bool TestUse(AllInOneBathroom obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (AllInOneBathroom obj in actor.LotCurrent.GetObjects<AllInOneBathroom>(new Predicate<AllInOneBathroom>(TestUse)))
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
