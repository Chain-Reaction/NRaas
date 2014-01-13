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
using Sims3.Gameplay.Objects.ShelvesStorage;
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
    public class WardrobeWoohoo : Wardrobe.WooHooInWardrobeB, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, Wardrobe.WooHooInWardrobeB.Definition, ProxyDefinition>(false);

            InteractionTuning tuning = Tunings.GetTuning<Sim, Wardrobe.WooHooInWardrobeA.Definition>();
            if (tuning != null)
            {
                tuning.Availability.RemoveFlags(Availability.FlagField.DisallowedIfPregnant);
                tuning.Availability.Teens = true;
            }

            Woohooer.InjectAndReset<Wardrobe, ProxyDefinition, SafeDefinition>(false);
            Woohooer.InjectAndReset<Wardrobe, ProxyDefinition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<Wardrobe, ProxyDefinition, TryForBabyDefinition>(false);

            Wardrobe.PushWoohooInWardrobe.WoohooSingleton = SafeSingleton;
            Wardrobe.PushWoohooInWardrobe.TryForBabySingleton = TryForBabySingleton;
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Wardrobe, Wardrobe.PushWoohooInWardrobe.Definition>(SafeSingleton);
            interactions.Add<Wardrobe>(RiskySingleton);
            interactions.Add<Wardrobe>(TryForBabySingleton);
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                if ((mWardrobe == null) || (mOtherWooHoo == null))
                {
                    return false;
                }

                if (!mWardrobe.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 10f))
                {
                    return false;
                }

                mWardrobe.SimLine.RemoveFromQueue(Actor);
                if (!Actor.RouteToSlotAndCheckInUse(mWardrobe, Slot.RoutingSlot_0))
                {
                    return false;
                }

                OutfitCategories currentOutfitCategory = Actor.CurrentOutfitCategory;
                int currentOutfitIndex = Actor.CurrentOutfitIndex;
                StandardEntry();
                BeginCommodityUpdates();
                mWardrobe.AddToUseList(Actor);
                EnterStateMachine("wardrobewoohoo", "Enter", "y");
                SetActor("dresserWardrobe", mWardrobe);
                AddPersistentScriptEventHandler(0x65, HideSim);
                AddOneShotScriptEventHandler(0x66, ShowX);
                AddOneShotScriptEventHandler(0x67, ShowY);
                Animate("y", "GetInY");
                mIsInWardrobe = true;
                while (!mOtherWooHoo.mIsInWardrobe)
                {
                    if (Target.InteractionQueue.GetCurrentInteraction() != mOtherWooHoo)
                    {
                        Actor.SetOpacity(0f, 0f);
                        SpeedTrap.Sleep(0x1);
                        Actor.SetPosition(mWardrobe.GetPositionOfSlot(Slot.RoutingSlot_1));
                        Actor.SetForward(mWardrobe.GetForwardOfSlot(Slot.RoutingSlot_1));
                        Animate("y", "Exit");
                        mCurrentStateMachine.RemoveEventHandler(HideSim);
                        mCurrentStateMachine.RemoveEventHandler(ShowX);
                        mCurrentStateMachine.RemoveEventHandler(ShowY);
                        mWardrobe.RemoveFromUseList(Actor);
                        EndCommodityUpdates(false);
                        StandardExit();
                        return false;
                    }
                    SpeedTrap.Sleep(0xa);
                }

                if (((Actor.OccultManager != null) && !Actor.OccultManager.DisallowClothesChange()) && !Actor.DoesSimHaveTransformationBuff())
                {
                    List<OutfitCategories> randomList = new List<OutfitCategories>(Wardrobe.kPossibleOutfitsAfterWooHoo);
                    randomList.Remove(Actor.CurrentOutfitCategory);

                    if (Woohooer.Settings.mNakedOutfitWardrobe)
                    {
                        randomList.Add(OutfitCategories.Naked);
                    }

                    Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.Force, RandomUtil.GetRandomObjectFromList(randomList));
                }

                AnimateJoinSims("WooHoo");
                mCurrentStateMachine.RemoveEventHandler(HideSim);
                Target.SetPosition(mWardrobe.GetPositionOfSlot(Slot.RoutingSlot_2));
                Target.SetForward(mWardrobe.GetForwardOfSlot(Slot.RoutingSlot_2));
                Actor.SetPosition(mWardrobe.GetPositionOfSlot(Slot.RoutingSlot_1));
                Actor.SetForward(mWardrobe.GetForwardOfSlot(Slot.RoutingSlot_1));
                mCurrentStateMachine.RequestState(false, "x", "GetOutX");
                mCurrentStateMachine.RequestState(true, "y", "GetOutX");
                mCurrentStateMachine.RequestState(false, "x", "GetOutY");
                mCurrentStateMachine.RequestState(true, "y", "GetOutY");

                if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                {
                    CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                }

                VisitSituation situation = VisitSituation.FindVisitSituationInvolvingGuest(Actor);
                VisitSituation situation2 = VisitSituation.FindVisitSituationInvolvingGuest(Target);
                if ((situation != null) && (situation2 != null))
                {
                    situation.GuestStartingInappropriateAction(Actor, Wardrobe.kAppropriatenessPenalty);
                    situation2.GuestStartingInappropriateAction(Target, Wardrobe.kAppropriatenessPenalty);
                }

                Actor.BuffManager.AddElement(BuffNames.ClothesEncounters, Origin.FromWooHooInWardrobe);
                Target.BuffManager.AddElement(BuffNames.ClothesEncounters, Origin.FromWooHooInWardrobe);

                mCurrentStateMachine.RequestState(false, "x", "Exit");
                mCurrentStateMachine.RequestState(true, "y", "Exit");
                mIsInWardrobe = false;

                EventTracker.SendEvent(EventTypeId.kWoohooInWardrobe, Actor, Target);

                mWardrobe.RemoveFromUseList(Actor);
                EndCommodityUpdates(true);
                StandardExit();
                mCurrentStateMachine.RemoveEventHandler(ShowX);
                mCurrentStateMachine.RemoveEventHandler(ShowY);
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, WardrobeWoohoo, BaseWardrobeDefinition>
        {
            public ProxyDefinition(BaseWardrobeDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public abstract class BaseWardrobeDefinition : CommonWoohoo.PotentialDefinition<Wardrobe>
        {
            public BaseWardrobeDefinition()
            { }
            public BaseWardrobeDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Wardrobe;
            }

            public override Sim GetTarget(Sim actor, Wardrobe target, InteractionInstance interaction)
            {
                Wardrobe stall = target as Wardrobe;

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
                Wardrobe caravan = obj as Wardrobe;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                Wardrobe.WooHooInWardrobeA instance = Wardrobe.WooHooInWardrobeA.Singleton.CreateInstance(actor, target, priority, false, true) as Wardrobe.WooHooInWardrobeA;
                instance.mWardrobe = caravan;

                WardrobeWoohoo nb = new ProxyDefinition(this).CreateInstance(target, actor, priority, false, true) as WardrobeWoohoo;
                nb.mWardrobe = caravan;

                instance.mTryForBaby = (GetStyle(nb) != CommonWoohoo.WoohooStyle.Safe);
                instance.mOtherWooHoo = nb;
                nb.mOtherWooHoo = instance;

                target.InteractionQueue.PushAsContinuation(instance, true);
                actor.InteractionQueue.PushAsContinuation(nb, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, Wardrobe obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeDefinition : BaseWardrobeDefinition
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

            public override string GetInteractionName(Sim actor, Wardrobe target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, Wardrobe obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "WardrobeWoohoo", isAutonomous, true, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeDefinition(target));
            }
        }

        public class RiskyDefinition : BaseWardrobeDefinition
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

            public override string GetInteractionName(Sim actor, Wardrobe target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Woohooer.Settings.GetRiskyChanceText(actor);
            }

            protected override bool Satisfies(Sim a, Sim target, Wardrobe obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesRisky(a, target, "WardrobeRisky", isAutonomous, true, ref greyedOutTooltipCallback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
        }

        public class TryForBabyDefinition : BaseWardrobeDefinition
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

            public override string GetInteractionName(Sim actor, Wardrobe target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, Wardrobe obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(a, target, "WardrobeTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
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
                get { return CommonWoohoo.WoohooLocation.Wardrobe; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is Wardrobe;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<Wardrobe>(new Predicate<Wardrobe>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<Wardrobe>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP7)) return false;
                }

                return Woohooer.Settings.mAutonomousWardrobe;
            }

            public bool TestUse(Wardrobe obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (Wardrobe obj in actor.LotCurrent.GetObjects<Wardrobe>(new Predicate<Wardrobe>(TestUse)))
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
