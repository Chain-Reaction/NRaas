using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
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
using Sims3.Gameplay.Objects.Entertainment;
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
    public class PhotoBoothWooHoo : PhotoBooth.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition MakeOutSingleton = new MakeOutDefinition();
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            //interactions.Add<PhotoBooth>(MakeOutSingleton);
            interactions.Replace<PhotoBooth, PhotoBooth.WooHoo.Definition>(SafeSingleton);
            interactions.Add<PhotoBooth>(RiskySingleton);
            interactions.Add<PhotoBooth>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<PhotoBooth, PhotoBooth.WooHoo.Definition, MakeOutDefinition>(true);
            Woohooer.InjectAndReset<PhotoBooth, PhotoBooth.WooHoo.Definition, SafeDefinition>(false);
            Woohooer.InjectAndReset<PhotoBooth, PhotoBooth.WooHoo.Definition, RiskyDefinition>(true);
            Woohooer.InjectAndReset<PhotoBooth, PhotoBooth.WooHoo.Definition, TryForBabyDefinition>(true);

            PhotoBooth.WooHoo.Singleton = SafeSingleton;
        }

        public override bool Run()
        {
            try
            {
                ProxyDefinition definition = InteractionDefinition as ProxyDefinition;

                isMaster = (WooHooer == Actor);
                bool flag = false;

                if (Target.LockDoor)
                {
                    return false;
                }

                if (isMaster && !Actor.HasExitReason())
                {
                    if (!Target.mEnterLine.WaitForTurn(this, Actor, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 30f))
                    {
                        return false;
                    }

                    PhotoBooth.WooHoo entry = definition.ProxyClone(WooHooee).CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as PhotoBooth.WooHoo;
                    entry.LinkedInteractionInstance = this;
                    entry.WooHooer = WooHooer;
                    entry.WooHooee = WooHooee;
                    WooHooee.InteractionQueue.AddNext(entry);
                }

                if (!SafeToSync())
                {
                    return false;
                }

                if (!Target.RouteToAndEnterPhotoBooth(Actor, this, false, isMaster))
                {
                    if (isMaster)
                    {
                        Target.RemoveSims();
                    }
                    return false;
                }

                StandardEntry(false);
                Actor.LoopIdle();
                if (!StartSync(isMaster))
                {
                    Target.RouteOutOfPhotoBooth(Actor, this, PhotoBooth.PhotoBoothExitType.Regular);
                    StandardExit(false, false);
                    return false;
                }

                BeginCommodityUpdates();
                Target.LockDoor = true;
                if (isMaster)
                {
                    Target.EnableFootprint(PhotoBooth.FootprintPathingHash);
                    AcquireStateMachine("photobooth");
                    SetActorAndEnter("x", Actor, "EnterPhotoBooth");
                    SetActorAndEnter("y", WooHooee, "EnterPhotoBooth");
                    SetActor("PhotoBooth", Target);

                    if (definition.Makeout)
                    {
                        AnimateJoinSims("MakeOut");
                        EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Actor, WooHooee, "Make Out", false, true, false, CommodityTypes.Undefined));
                        EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, WooHooee, Actor, "Make Out", true, true, false, CommodityTypes.Undefined));
                    }
                    else
                    {
                        isWooHooing = true;

                        CommonWoohoo.TestNakedOutfit(Woohooer.Settings.mNakedOutfitPhotoBooth, WooHooer, WooHooee);

                        AnimateJoinSims("WooHoo");

                        EventTracker.SendEvent(EventTypeId.kWoohooInPhotoBooth, Actor, Target);
                        EventTracker.SendEvent(EventTypeId.kWoohooInPhotoBooth, WooHooee, Target);

                        CommonWoohoo.RunPostWoohoo(Actor, WooHooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                        if (CommonPregnancy.IsSuccess(WooHooer, WooHooee, Autonomous, definition.GetStyle(this)))
                        {
                            CommonPregnancy.Impregnate(WooHooer, WooHooee, Autonomous, definition.GetStyle(this));
                        }

                        if (RandomUtil.RandomChance01(PhotoBooth.kChanceWooHooPicture))
                        {
                            flag = true;
                        }
                    }

                    isWooHooing = false;
                    AnimateSim("ExitPhotoBooth");
                    AnimateNoYield("y", "ExitPhotoBooth");
                }

                FinishLinkedInteraction(isMaster);
                WaitForSyncComplete();
                if (isMaster)
                {
                    Target.DisableFootprint(PhotoBooth.FootprintPathingHash);
                    if (!definition.Makeout)
                    {
                        Relationship.Get(WooHooer, WooHooee, true).LTR.UpdateLiking(PhotoBooth.kLTRIncreaseOnWoohoo);
                    }

                    if (flag)
                    {
                        Target.RouteOutOfPhotoBooth(WooHooer, this, PhotoBooth.PhotoBoothExitType.ExitAndTear);
                    }
                    else
                    {
                        Target.RouteOutOfPhotoBooth(WooHooer, this, PhotoBooth.PhotoBoothExitType.Regular);
                    }
                }
                else
                {
                    Target.RouteOutOfPhotoBooth(Actor, this, PhotoBooth.PhotoBoothExitType.Regular);
                }

                EndCommodityUpdates(true);
                StandardExit(false, false);

                if (isMaster && flag)
                {
                    WooHooer.BuffManager.AddElement(BuffNames.PublicWooHoo, Origin.FromWooHooInPublic);
                    WooHooee.BuffManager.AddElement(BuffNames.PublicWooHoo, Origin.FromWooHooInPublic);

                    WooHooee.ShowTNSIfSelectable(Localization.LocalizeString(WooHooee.IsFemale, "Gameplay/Objects/Entertainment/PhotoBooth:ToreUpPhotoEvidenceTNS", new object[] { WooHooee.FullName }), StyledNotification.NotificationStyle.kGameMessagePositive);
                    WooHooer.ShowTNSIfSelectable(Localization.LocalizeString(WooHooer.IsFemale, "Gameplay/Objects/Entertainment/PhotoBooth:ToreUpPhotoEvidenceTNS", new object[] { WooHooer.FullName }), StyledNotification.NotificationStyle.kGameMessagePositive);
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<PhotoBooth, PhotoBoothWooHoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }

            public bool Makeout
            {
                get
                {
                    return Definition.Makeout;
                }
            }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<PhotoBooth>
        {
            public BaseDefinition()
            { }
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.PhotoBooth;
            }

            public virtual bool Makeout
            {
                get { return false; }
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                PhotoBooth booth = obj as PhotoBooth;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                PhotoBoothWooHoo instance = new ProxyDefinition(this).CreateInstance(booth, actor, priority, false, true) as PhotoBoothWooHoo;
                instance.WooHooer = actor;
                instance.WooHooee = target;
                actor.InteractionQueue.PushAsContinuation(instance, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, PhotoBooth obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class MakeOutDefinition : BaseDefinition
        {
            public MakeOutDefinition()
            { }
            public MakeOutDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override bool Makeout
            {
                get { return true; }
            }

            public override string GetInteractionName(Sim actor, PhotoBooth target, InteractionObjectPair iop)
            {
                return PhotoBooth.LocalizeString("MakeOutInteractionName", new object[0x0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, PhotoBooth obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonSocials.SatisfiesRomance(actor, target, "PhotoBoothMakeOut ", isAutonomous, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new MakeOutDefinition(target));
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

            public override string GetInteractionName(Sim actor, PhotoBooth target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, PhotoBooth obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "PhotoBoothWooHoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, PhotoBooth target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, PhotoBooth obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "PhotoBoothRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, PhotoBooth target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, PhotoBooth obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "PhotoBoothTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.PhotoBooth; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is PhotoBooth;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<PhotoBooth>(new Predicate<PhotoBooth>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<PhotoBooth>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if ((!GameUtils.IsInstalled(ProductVersion.EP6)) &&
                        (!GameUtils.IsInstalled(ProductVersion.EP9)))
                    {
                        return false;
                    }
                }

                return Woohooer.Settings.mAutonomousPhotoBooth;
            }

            public bool TestUse(PhotoBooth obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (PhotoBooth obj in actor.LotCurrent.GetObjects<PhotoBooth>(new Predicate<PhotoBooth>(TestUse)))
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
