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
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class GypsyCaravanWoohoo : GypsyCaravan.WooHooCaravanB, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public GypsyCaravanWoohoo()
        { }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GypsyCaravan, GypsyCaravan.PushWoohooInCaravan.Definition>(SafeSingleton);
            interactions.Add<GypsyCaravan>(RiskySingleton);
            interactions.Add<GypsyCaravan>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, GypsyCaravan.WooHooCaravanB.Definition, ProxyDefinition>(false);

            Woohooer.InjectAndReset<GypsyCaravan, ProxyDefinition, SafeDefinition>(false);
            Woohooer.InjectAndReset<GypsyCaravan, ProxyDefinition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<GypsyCaravan, ProxyDefinition, TryForBabyDefinition>(false);

            GypsyCaravan.PushWoohooInCaravan.WoohooSingleton = SafeSingleton;
            GypsyCaravan.PushWoohooInCaravan.TryForBabySingleton = TryForBabySingleton;
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                if ((mGypsyCaravan == null) || (mOtherWooHoo == null))
                {
                    return false;
                }
                if (!mGypsyCaravan.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 10f))
                {
                    return false;
                }

                mGypsyCaravan.SimLine.RemoveFromQueue(Actor);
                if (!mGypsyCaravan.RouteNearEntranceAndEnterRabbitHole(Actor, null, null, false, Route.RouteMetaType.GoRabbitHole, true))
                {
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();
                EnterStateMachine("GypsyCaravanWoohoo", "WoohooEnter", "y");
                SetActor("caravan", mGypsyCaravan);
                AddPersistentScriptEventHandler(0x65, HideSim);
                AddOneShotScriptEventHandler(0x66, ShowX);
                AddOneShotScriptEventHandler(0x67, ShowY);
                Animate("y", "GetInY");
                mIsInCaravan = true;

                while (!mOtherWooHoo.mIsInCaravan)
                {
                    if ((Target.InteractionQueue.GetCurrentInteraction() != mOtherWooHoo) || Target.HasExitReason(ExitReason.Canceled))
                    {
                        Actor.SetOpacity(1f, 0f);
                        Animate("y", "WoohooExit");
                        mCurrentStateMachine.RemoveEventHandler(HideSim);
                        mCurrentStateMachine.RemoveEventHandler(ShowX);
                        mCurrentStateMachine.RemoveEventHandler(ShowY);
                        mGypsyCaravan.RemoveFromUseList(Actor);
                        EndCommodityUpdates(false);
                        StandardExit();
                        return false;
                    }
                    SpeedTrap.Sleep(0xa);
                }

                mGypsyCaravan.TurnOnWooHooEffect();
                AnimateJoinSims("WooHoo");
                CommonWoohoo.RunPostWoohoo(Actor, Target, mGypsyCaravan, definition.GetStyle(this), definition.GetLocation(mGypsyCaravan), true);

                if (RandomUtil.RandomChance(GypsyCaravan.kFortuneToldSleepWear))
                {
                    if (Woohooer.Settings.mNakedOutfitGypsyCaravan)
                    {
                        if (!Actor.OccultManager.DisallowClothesChange())
                        {
                            Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Naked);
                        }

                        if (!Target.OccultManager.DisallowClothesChange())
                        {
                            Target.SwitchToOutfitWithoutSpin(OutfitCategories.Naked);
                        }
                    }
                    else 
                    {
                        if (!Actor.OccultManager.DisallowClothesChange())
                        {
                            Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Sleepwear);
                        }

                        if (!Target.OccultManager.DisallowClothesChange())
                        {
                            Target.SwitchToOutfitWithoutSpin(OutfitCategories.Sleepwear);
                        }
                    }
                }
                else if (RandomUtil.RandomChance(GypsyCaravan.kFortuneToldSinged))
                {
                    if (!Actor.OccultManager.DisallowClothesChange())
                    {
                        Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Singed);
                    }

                    if (!Target.OccultManager.DisallowClothesChange())
                    {
                        Target.SwitchToOutfitWithoutSpin(OutfitCategories.Singed);
                    }
                }

                mCurrentStateMachine.RemoveEventHandler(HideSim);
                mGypsyCaravan.TurnOffWooHooEffect();
                mGypsyCaravan.ExitRabbitHoleAndRouteAway(Actor);

                if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                {
                    CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                }

                VisitSituation situation = VisitSituation.FindVisitSituationInvolvingGuest(Actor);
                VisitSituation situation2 = VisitSituation.FindVisitSituationInvolvingGuest(Target);
                if ((situation != null) && (situation2 != null))
                {
                    situation.GuestStartingInappropriateAction(Actor, GypsyCaravan.kAppropriatenessPenalty);
                    situation2.GuestStartingInappropriateAction(Target, GypsyCaravan.kAppropriatenessPenalty);
                }

                EventTracker.SendEvent(EventTypeId.kWooHooedInTheVardo, Actor, mOtherWooHoo.Actor);

                mIsInCaravan = false;
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, GypsyCaravanWoohoo, BaseGypsyCaravanDefinition>
        {
            public ProxyDefinition(BaseGypsyCaravanDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public abstract class BaseGypsyCaravanDefinition : CommonWoohoo.PotentialDefinition<GypsyCaravan>
        {
            public BaseGypsyCaravanDefinition()
            { }
            public BaseGypsyCaravanDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.GypsyCaravan;
            }

            public override Sim GetTarget(Sim actor, GypsyCaravan target, InteractionInstance interaction)
            {
                GypsyCaravan stall = target as GypsyCaravan;

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
                GypsyCaravan caravan = obj as GypsyCaravan;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                GypsyCaravan.WooHooCaravanA instance = GypsyCaravan.WooHooCaravanA.Singleton.CreateInstance(actor, target, priority, false, true) as GypsyCaravan.WooHooCaravanA;
                instance.mGypsyCaravan = caravan;

                GypsyCaravanWoohoo nb = new ProxyDefinition(this).CreateInstance(target, actor, priority, false, true) as GypsyCaravanWoohoo;
                nb.mGypsyCaravan = caravan;

                instance.mTryForBaby = (GetStyle(nb) != CommonWoohoo.WoohooStyle.Safe);
                instance.mOtherWooHoo = nb;
                nb.mOtherWooHoo = instance;

                target.InteractionQueue.PushAsContinuation(instance, true);
                actor.InteractionQueue.PushAsContinuation(nb, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, GypsyCaravan obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeDefinition : BaseGypsyCaravanDefinition
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

            public override string GetInteractionName(Sim actor, GypsyCaravan target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, GypsyCaravan obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "GypsyCaravanWoohoo", isAutonomous, true, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeDefinition(target));
            }
        }

        public class RiskyDefinition : BaseGypsyCaravanDefinition
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

            public override string GetInteractionName(Sim actor, GypsyCaravan target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Woohooer.Settings.GetRiskyChanceText(actor);
            }

            protected override bool Satisfies(Sim a, Sim target, GypsyCaravan obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesRisky(a, target, "GypsyCaravanRisky", isAutonomous, true, ref greyedOutTooltipCallback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
        }

        public class TryForBabyDefinition : BaseGypsyCaravanDefinition
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

            public override string GetInteractionName(Sim actor, GypsyCaravan target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, GypsyCaravan obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(a, target, "GypsyCaravanTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
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
                get { return CommonWoohoo.WoohooLocation.GypsyCaravan; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is GypsyCaravan;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<GypsyCaravan>(new Predicate<GypsyCaravan>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<GypsyCaravan>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP7)) return false;
                }

                return Woohooer.Settings.mAutonomousGypsyCaravan;
            }

            public bool TestUse(IGypsyCaravan obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (IGypsyCaravan obj in actor.LotCurrent.GetObjects<IGypsyCaravan>(new Predicate<IGypsyCaravan>(TestUse)))
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj as GameObject);
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
