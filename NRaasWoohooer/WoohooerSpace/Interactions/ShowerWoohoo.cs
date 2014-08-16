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
    public class ShowerWoohoo : Shower.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition SafeSingleton = new SafeDefinition();
        public static InteractionDefinition RiskySingleton = new RiskyDefinition();
        public static InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public ShowerWoohoo()
        { }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<IShowerable, Shower.WooHooInShower.Definition>(SafeSingleton);
            interactions.Add<IShowerable>(RiskySingleton);
            interactions.Add<IShowerable>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, Shower.WooHoo.Definition, ProxyDefinition>(false);

            Woohooer.InjectAndReset<IShowerable, Shower.WooHooInShower.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<IShowerable, Shower.WooHooInShower.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<IShowerable, Shower.WooHooInShower.Definition, TryForBabyDefinition>(false);

            Sims3.Gameplay.Objects.Plumbing.Shower.WooHooInShower.Singleton = SafeSingleton;
            Sims3.Gameplay.Objects.Plumbing.Shower.WooHooInShower.TryForBabySingleton = TryForBabySingleton;
        }

        private void OnPregnancyEvent(StateMachineClient smc, IEvent evt)
        {
            IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

            Pregnancy pregnancy = CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
            if (pregnancy != null)
            {
                if (RandomUtil.RandomChance(Sims3.Gameplay.Objects.Plumbing.Shower.kChanceOfHydrophobic))
                {
                    pregnancy.SetForcedBabyTrait(TraitNames.Hydrophobic);
                }
            }
        }

        private void OnJealousyEvent(StateMachineClient smc, IEvent evt)
        {
            if (mReactToSocialBroadcasterActor == null)
            {
                mReactToSocialBroadcasterActor = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                CommonWoohoo.CheckForWitnessedCheating(Actor, Target, !Rejected);
            }
            if (mReactToSocialBroadcasterTarget == null)
            {
                mReactToSocialBroadcasterTarget = new ReactionBroadcaster(Target, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                CommonWoohoo.CheckForWitnessedCheating(Target, Actor, !Rejected);
            }
        }

        public override bool Run()
        {
            Common.StringBuilder msg = new Common.StringBuilder("ShowerWoohoo:Run" + Common.NewLine);

            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                Actor.BuffManager.RemoveElement(BuffNames.RobotForm);

                mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GoingToBathe);
                mSwitchOutfitHelper.Start();
                if (WaitForBToEnterShower)
                {
                    //Actor.RouteToObjectRadialRange(Shower, 1.5f, 4f);
                    while (Shower.SimInShower != Target)
                    {
                        if ((Target.InteractionQueue.GetCurrentInteraction() != TakeShowerInst) || Target.HasExitReason(ExitReason.Canceled))
                        {
                            mSwitchOutfitHelper.Dispose();
                            return false;
                        }
                        SpeedTrap.Sleep(0xa);
                    }
                }

                msg += "A";

                if (!Actor.RouteToSlot(Shower, Slot.RoutingSlot_0))
                {
                    mSwitchOutfitHelper.Dispose();
                    return false;
                }

                msg += "B";

                mSwitchOutfitHelper.Wait(true);
                bool flag = Actor.GetCurrentOutfitCategoryFromOutfitInGameObject() == OutfitCategories.Singed;
                Shower.AddToUseList(Actor);
                mInUseList = true;
                mCurrentStateMachine = TakeShowerInst.mCurrentStateMachine;
                SetActorAndEnter("y", Actor, "Enter");
                if (!WaitForBToEnterShower)
                {
                    mCurrentStateMachine.RequestState("y", "Ask");
                }

                msg += "C";

                string socialName = CommonWoohoo.GetSocialName(definition.GetStyle(this), Actor);
                StartSocial(socialName);

                if (WaitForBToEnterShower)
                {
                    Rejected = false;
                }

                InitiateSocialUI(Actor, Target);

                msg += "D";

                bool succeeded = true;
                if (Rejected)
                {
                    succeeded = false;
                    mCurrentStateMachine.RequestState("y", "Reject");
                    mCurrentStateMachine.RemoveActor(Actor);
                    FinishSocial(socialName, true);
                    Actor.BuffManager.AddElement(BuffNames.WalkOfShame, Origin.FromRejectedWooHooOffHome);
                }
                else
                {
                    mCurrentStateMachine.RequestState("y", "ShooSims");
                    SetParameter("SimShouldClothesChange", !flag && !Actor.OccultManager.DisallowClothesChange());
                    mSwitchOutfitHelper.AddScriptEventHandler(this);
                    Actor.LoopIdle();

                    msg += "E";

                    if (CommonWoohoo.NeedPrivacy(Shower.IsOutside, Actor, Target))
                    {
                        mSituation = new BedWoohoo.WooHooPrivacySituation(this);
                        if (!mSituation.Start())
                        {
                            mSwitchOutfitHelper.Dispose();
                            succeeded = false;
                        }
                    }

                    msg += "F";

                    if ((succeeded) && (Actor.RouteToSlot(Shower, Slot.RoutingSlot_0)))
                    {
                        MotiveDelta[] deltaArray = new MotiveDelta[6];
                        deltaArray[0] = AddMotiveDelta(CommodityKind.Fun, 1500f);
                        deltaArray[1] = TakeShowerInst.AddMotiveDelta(CommodityKind.Fun, 1500f);
                        deltaArray[2] = AddMotiveDelta(CommodityKind.Social, 50f);
                        deltaArray[3] = TakeShowerInst.AddMotiveDelta(CommodityKind.Social, 50f);
                        if (Actor.SimDescription.IsPlantSim)
                        {
                            deltaArray[4] = AddMotiveDelta(CommodityKind.Hygiene, 800f * Sims3.Gameplay.Objects.Plumbing.Shower.kPlantSimHygieneModifier);
                        }
                        else
                        {
                            deltaArray[4] = AddMotiveDelta(CommodityKind.Hygiene, 800f);
                        }

                        if (Actor.SimDescription.IsMermaid)
                        {
                            deltaArray[5] = AddMotiveDelta(CommodityKind.MermaidDermalHydration, 800f);
                        }

                        Target.EnableCensor(Sim.CensorType.FullHeight);
                        RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                        mCurrentStateMachine.AddOneShotScriptEventHandler(0x65, OnJealousyEvent);
                        mCurrentStateMachine.AddOneShotScriptEventHandler(0x66, OnAnimationEvent);

                        if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                        {
                            mCurrentStateMachine.AddOneShotScriptEventHandler(0x67, OnPregnancyEvent);
                        }

                        msg += "G";

                        mSteamVfx = VisualEffect.Create(Shower.SteamVfxName);
                        mSteamVfx.ParentTo(Shower, Shower.IsShowerTub ? Slot.FXJoint_2 : Slot.FXJoint_0);
                        mSteamVfx.Start();
                        mCurrentStateMachine.RequestState(null, "BreatheIdle");
                        RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);

                        CommonWoohoo.RunPostWoohoo(Actor, Target, Shower, definition.GetStyle(this), definition.GetLocation(Shower), true);

                        msg += "H";

                        Sims3.Gameplay.Objects.Plumbing.Shower.WaitToLeaveShower(Actor, Shower);
                        
                        // Custom
                        ShowerEx.ApplyPostShowerEffects(Actor, Shower);
                        if (flag)
                        {
                            mSwitchOutfitHelper.Dispose();
                            mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GoingToBathe);
                            mSwitchOutfitHelper.Start();
                            mSwitchOutfitHelper.Wait(false);
                            mSwitchOutfitHelper.ChangeOutfit();
                        }

                        msg += "I";

                        mSwitchOutfitHelper.Dispose();
                        mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GettingOutOfBath);
                        mSwitchOutfitHelper.Start();
                        mSwitchOutfitHelper.AddScriptEventHandler(this);
                        mSwitchOutfitHelper.Wait(false);
                        RemoveMotiveDelta(deltaArray[0x0]);
                        TakeShowerInst.RemoveMotiveDelta(deltaArray[0x1]);
                        RemoveMotiveDelta(deltaArray[0x2]);
                        TakeShowerInst.RemoveMotiveDelta(deltaArray[0x3]);
                        RemoveMotiveDelta(deltaArray[0x4]);

                        if (Actor.SimDescription.IsMermaid)
                        {
                            RemoveMotiveDelta(deltaArray[0x5]);
                        }

                        mCurrentStateMachine.RequestState("y", "Exit Working Y");
                        Target.AutoEnableCensor();

                        msg += "J";
                    }

                    mCurrentStateMachine.RemoveActor(Actor);
                    FinishSocial(socialName, true);
                }

                msg += "K";

                Shower.RemoveFromUseList(Actor);
                mInUseList = false;
                Actor.RouteAway(Sims3.Gameplay.Objects.Plumbing.Shower.kMinDistanceToMoveAwayFromShower, Sims3.Gameplay.Objects.Plumbing.Shower.kMaxDistanceToMoveAwayFromShower, false, GetPriority(), true, true, true, RouteDistancePreference.PreferFurthestFromRouteOrigin);
                if (mSteamVfx != null)
                {
                    mSteamVfx.Stop();
                    mSteamVfx = null;
                }

                msg += "L";

                EndCommodityUpdates(succeeded);
                return succeeded;
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
            finally
            {
                if (TakeShowerInst != null)
                {
                    TakeShowerInst.HavingWooHoo = false;
                }
            }
        }

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<IShowerable, ShowerWoohoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base (definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<IShowerable>
        {
            public BaseDefinition()
            { }
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                if (obj.GetType().ToString().Contains("ShowerOutdoor"))
                {
                    return CommonWoohoo.WoohooLocation.OutdoorShower;
                }
                else
                {
                    return CommonWoohoo.WoohooLocation.Shower;
                }
            }

            public override Sim GetTarget(Sim actor, IShowerable target, InteractionInstance interaction)
            {
                if ((target != null) && (target.SimInShower != null))
                {
                    return target.SimInShower;
                }
                else
                {
                    return base.GetTarget(actor, target, interaction);
                }
            }

            public override bool JoinInProgress(Sim actor, Sim target, IShowerable obj, InteractionInstance interaction)
            {
                if (obj.SimInShower == null) return false;

                ShowerWoohoo hoo = new ProxyDefinition(this).CreateInstance(obj.SimInShower, actor, interaction.GetPriority(), interaction.Autonomous, interaction.CancellableByPlayer) as ShowerWoohoo;

                if (actor.InteractionQueue.Add(hoo))
                {
                    Shower.TakeShower currentInteraction = obj.SimInShower.InteractionQueue.GetCurrentInteraction() as Shower.TakeShower;
                    if (currentInteraction != null)
                    {
                        currentInteraction.HavingWooHoo = true;
                        hoo.TakeShowerInst = currentInteraction;
                        hoo.Shower = obj;
                    }
                }
                return true;
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                IShowerable shower = obj as IShowerable;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                // Use custom interaction to bypass privacy
                Shower.TakeShower.Definition takeShower = null;
                foreach (InteractionObjectPair pair in shower.Interactions)
                {
                    takeShower = pair.InteractionDefinition as Shower.TakeShower.Definition;
                    if (takeShower != null) break;
                }

                if (takeShower == null)
                {
                    Common.DebugNotify("PushWooHoo Take Shower Fail");
                    return;
                }

                Shower.TakeShower instance = takeShower.CreateInstance(shower, target, priority, false, true) as Shower.TakeShower;
                instance.HavingWooHoo = true;

                ShowerWoohoo hoo = new ProxyDefinition(this).CreateInstance(target, actor, priority, false, true) as ShowerWoohoo;
                hoo.WaitForBToEnterShower = true;
                hoo.Shower = shower;
                hoo.TakeShowerInst = instance;
                if (actor.InteractionQueue.PushAsContinuation(hoo, true))
                {
                    target.InteractionQueue.PushAsContinuation(instance, true);
                    hoo.LinkedInteractionInstance = instance;
                }
            }

            protected override bool Satisfies(Sim actor, Sim target, IShowerable obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP4))
                {
                    callback = Common.DebugTooltip("EP4 Fail");
                    return false;
                }

                if ((obj.Repairable == null) && (!obj.GetType().ToString().Contains("ShowerOutdoor")))
                {
                    // Take Shower requires a Repairable component
                    callback = Common.DebugTooltip("Not Repairable");
                    return false;
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

            public override string GetInteractionName(Sim actor, IShowerable target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, IShowerable obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "ShowerWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, IShowerable target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, IShowerable obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "ShowerRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, IShowerable target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, IShowerable obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "ShowerTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.Shower; }
            }

            public override bool Matches(IGameObject obj)
            {
                // Exclude Outdoor showers
                if (obj.GetType().ToString().Contains("ShowerOutdoor")) return false;

                return obj is IShowerable;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<IShowerable>(new Predicate<IShowerable>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<IShowerable>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                //if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP4)) return false;
                }

                return Woohooer.Settings.mAutonomousShower;
            }

            public bool TestUse(IShowerable obj)
            {
                if (!TestRepaired(obj)) return false;

                if (obj.Repairable == null)
                {
                    // Take Shower requires a Repairable component
                    return false;
                }

                // Exclude Outdoor showers
                if (obj.GetType().ToString().Contains("ShowerOutdoor")) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                if (GameUtils.IsInstalled(ProductVersion.EP4))
                {
                    foreach (IShowerable obj in actor.LotCurrent.GetObjects<IShowerable>(new Predicate<IShowerable>(TestUse)))
                    {
                        if ((testFunc != null) && (!testFunc(obj, null))) continue;

                        results.Add(obj as GameObject);
                    }
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
