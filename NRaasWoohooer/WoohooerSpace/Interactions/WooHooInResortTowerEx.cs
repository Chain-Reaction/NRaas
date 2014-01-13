using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
    public class WooHooInResortTowerEx : ResortTower.WooHooInResortTower, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<ResortTower, ResortTower.WooHooInResortTower.Definition, SafeDefinition>(true);
            Woohooer.InjectAndReset<ResortTower, ResortTower.WooHooInResortTower.Definition, RiskyDefinition>(true);
            Woohooer.InjectAndReset<ResortTower, ResortTower.WooHooInResortTower.Definition, TryForBabyDefinition>(true);
            Woohooer.InjectAndReset<ResortTower, ResortTower.WooHooInResortTower.Definition, ProxyDefinition>(true);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<ResortTower>(SafeSingleton);
            interactions.Add<ResortTower>(RiskySingleton);
            interactions.Add<ResortTower>(TryForBabySingleton);
        }

        public override bool InRabbitHole()
        {
            try
            {
                IWooHooDefinition woohooDefinition = InteractionDefinition as IWooHooDefinition;

                Definition interactionDefinition = InteractionDefinition as Definition;
                bool shouldBeMaster = false;
                if (Actor == mWooHooer)
                {
                    shouldBeMaster = true;
                }

                if (Actor.IsActiveSim)
                {
                    PlumbBob.HidePlumbBob();
                }

                if (StartSync(shouldBeMaster, false, null, 0f, false))
                {
                    BeginCommodityUpdates();
                    StartStages();
                    if (shouldBeMaster)
                    {
                        Target.TurnOnWooHooEffect();
                    }

                    mStartedWooHooing = true;
                    RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                    bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    EndCommodityUpdates(succeeded);
                    FinishLinkedInteraction(shouldBeMaster);
                    WaitForSyncComplete();
                    if (shouldBeMaster)
                    {
                        Target.TurnOffWooHooEffect();
                        if (Actor.HasExitReason(ExitReason.StageComplete))
                        {
                            CommonWoohoo.RunPostWoohoo(Actor, mWooHooee, Target, woohooDefinition.GetStyle(this), woohooDefinition.GetLocation(Target), true);

                            if (CommonPregnancy.IsSuccess(Actor, mWooHooee, Autonomous, woohooDefinition.GetStyle(this)))
                            {
                                CommonPregnancy.Impregnate(Actor, mWooHooee, Autonomous, woohooDefinition.GetStyle(this));
                            }
                        }
                    }
                }
                else if (shouldBeMaster && (LinkedInteractionInstance != null))
                {
                    LinkedInteractionInstance.InstanceActor.AddExitReason(ExitReason.CanceledByScript);
                }

                if (Actor.IsActiveSim)
                {
                    PlumbBob.ShowPlumbBob();
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<ResortTower, WooHooInResortTowerEx, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<ResortTower>
        {
            public BaseDefinition()
            { }
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Resort;
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                ResortTower tower = obj as ResortTower;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                WooHooInResortTowerEx instance = new ProxyDefinition(this).CreateInstance(tower, actor, priority, false, true) as WooHooInResortTowerEx;
                instance.mWooHooer = actor;
                instance.mWooHooee = target;
                actor.InteractionQueue.PushAsContinuation(instance, true);

                WooHooInResortTowerEx second = new ProxyDefinition(this).CreateInstance(tower, target, priority, false, true) as WooHooInResortTowerEx;
                second.LinkedInteractionInstance = instance;
                second.mWooHooer = actor;
                second.mWooHooee = target;
                target.InteractionQueue.PushAsContinuation(second, true);
            }

            public override bool Test(Sim a, ResortTower target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((target.LotCurrent == null) || (!target.LotCurrent.IsCommunityLotOfType(CommercialLotSubType.kEP10_Resort)))
                {
                    return false;
                }

                if (!target.LotCurrent.ResortManager.IsCheckedIn(a) && (a.Household.RealEstateManager.FindProperty(target.LotCurrent) == null))
                {
                    return false;
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }

            protected override bool Satisfies(Sim actor, Sim target, ResortTower obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
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

            public override string GetInteractionName(Sim actor, ResortTower target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, ResortTower obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "ResortTowerWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, ResortTower target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance(actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, ResortTower obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "ResortTowerRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, ResortTower target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, ResortTower obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "ResortTowerTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.Resort; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is ResortTower;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<ResortTower>(new Predicate<ResortTower>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<ResortTower>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP10)) return false;
                }

                return Woohooer.Settings.mAutonomousResort;
            }

            public bool TestUse(ResortTower obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (ResortTower obj in actor.LotCurrent.GetObjects<ResortTower>(new Predicate<ResortTower>(TestUse)))
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
