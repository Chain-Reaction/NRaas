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
using Sims3.Gameplay.Objects.Environment;
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
    public class TreehouseWoohoo : Treehouse.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public TreehouseWoohoo()
        { }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Treehouse, Treehouse.WooHooInitiator.Definition>(SafeSingleton);
            interactions.Add<Treehouse>(RiskySingleton);
            interactions.Add<Treehouse>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Treehouse, Treehouse.WooHoo.Definition, ProxyDefinition>(false);

            Woohooer.InjectAndReset<Treehouse, Treehouse.WooHooInitiator.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<Treehouse, Treehouse.WooHooInitiator.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<Treehouse, Treehouse.WooHooInitiator.Definition, TryForBabyDefinition>(false);

            Treehouse.WooHooInitiator.SingletonWooHoo = SafeSingleton;
            Treehouse.WooHooInitiator.SingletonTryForBaby = TryForBabySingleton;
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
                mLinkedActor = LinkedInteractionInstance.InstanceActor;
                if (!StartSync(mIsMaster))
                {
                    Treehouse.PushGetOutAsContinuation(Actor);
                    return false;
                }

                StandardEntry(false);
                BeginCommodityUpdates();
                if (mIsMaster)
                {
                    RockGemMetalBase.HandleNearbyWoohoo(Target, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                    mCurrentStateMachine = Actor.Posture.CurrentStateMachine;
                    SetActor("y", mLinkedActor);
                    AnimateSim("WooHoo");
                    AnimateSim("Idle");
                    RemoveActor("y");

                    IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                    CommonWoohoo.RunPostWoohoo(Actor, mLinkedActor, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                    if (CommonPregnancy.IsSuccess(Actor, mLinkedActor, Autonomous, definition.GetStyle(this)))
                    {
                        Pregnancy pregnancy = CommonPregnancy.Impregnate(Actor, mLinkedActor, Autonomous, definition.GetStyle(this));
                        if (pregnancy != null)
                        {
                            pregnancy.SetForcedBabyTrait(RandomUtil.GetRandomObjectFromList(Target.RandomPregnancyTraits));
                        }
                    }

                    RockGemMetalBase.HandleNearbyWoohoo(Target, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                }
                else
                {
                    DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }

                float splinterChance = Treehouse.WooHoo.kSplinterChance;
                if (Actor.TraitManager.HasAnyElement(kSplinterChanceTraitsIncrease))
                {
                    splinterChance *= kSplinterChanceMultiplierIncrease;
                }
                if (Actor.TraitManager.HasAnyElement(kSplinterChanceTraitsDecrease) || Actor.BuffManager.HasElement(BuffNames.LuckyLime))
                {
                    splinterChance *= kSplinterChanceMultiplierDecrease;
                }
                if (RandomUtil.RandomChance01(splinterChance))
                {
                    Actor.BuffManager.AddElement(BuffNames.Splinter, Origin.None);
                }

                EndCommodityUpdates(true);
                StandardExit(false, false);
                FinishLinkedInteraction(mIsMaster);
                WaitForSyncComplete();
                Treehouse.PushGetOutAsContinuation(Actor);
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Treehouse, TreehouseWoohoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<Treehouse>
        {
            public BaseDefinition()
            { }
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Treehouse;
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                Treehouse house = obj as Treehouse;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                TreehouseWoohoo instance = new ProxyDefinition(this).CreateInstance(house, actor, priority, false, true) as TreehouseWoohoo;
                instance.IsMaster = true;

                TreehouseWoohoo hoo2 = new ProxyDefinition(this).CreateInstance(house, target, priority, false, true) as TreehouseWoohoo;
                if (actor.InteractionQueue.PushAsContinuation(instance, true))
                {
                    target.InteractionQueue.PushAsContinuation(hoo2, true);
                    instance.LinkedInteractionInstance = hoo2;
                }
            }

            protected override bool Satisfies(Sim actor, Sim target, Treehouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
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

            public override string GetInteractionName(Sim actor, Treehouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, Treehouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "TreehouseWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, Treehouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, Treehouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "TreehouseRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, Treehouse target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, Treehouse obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "TreehouseTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.Treehouse; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is Treehouse;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<Treehouse>(new Predicate<Treehouse>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<Treehouse>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP4)) return false;
                }

                return Woohooer.Settings.mAutonomousTreehouse;
            }

            public bool TestUse(ITreehouse obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (ITreehouse obj in actor.LotCurrent.GetObjects<ITreehouse>(new Predicate<ITreehouse>(TestUse)))
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
