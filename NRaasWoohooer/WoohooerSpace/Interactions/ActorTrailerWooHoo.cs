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
    public class ActorTrailerWooHoo : ActorTrailer.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        bool mIsMaster;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ActorTrailer, ActorTrailer.WooHoo.Definition>(SafeSingleton);
            interactions.Add<ActorTrailer>(RiskySingleton);
            interactions.Add<ActorTrailer>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<ActorTrailer, ActorTrailer.WooHoo.Definition, SafeDefinition>(true);
            Woohooer.InjectAndReset<ActorTrailer, ActorTrailer.WooHoo.Definition, RiskyDefinition>(true);
            Woohooer.InjectAndReset<ActorTrailer, ActorTrailer.WooHoo.Definition, TryForBabyDefinition>(true);
            Woohooer.InjectAndReset<ActorTrailer, ActorTrailer.WooHoo.Definition, ProxyDefinition>(true);

            ActorTrailer.WooHoo.Singleton = SafeSingleton;
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
                    return ActorTrailer.LocalizeString(name, new object[] { sim });
                }
            }

            InteractionInstanceParameters parameters = GetInteractionParameters();
            return InteractionDefinition.GetInteractionName(ref parameters);
        }

        public override void Cleanup()
        {
            base.Cleanup();

            if (mWooHooReactionBroadcast != null)
            {
                mWooHooReactionBroadcast.Dispose();
                mWooHooReactionBroadcast = null;
            }
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                if (mIsMaster && !Actor.HasExitReason())
                {
                    if (!Target.mEnterLine.WaitForTurn(this, Actor, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 30f))
                    {
                        return false;
                    }

                    List<Sim> exceptionsList = new List<Sim>();
                    exceptionsList.Add(WooHooer);
                    exceptionsList.Add(WooHooee);
                    Target.RemoveSimsExceptFor(exceptionsList);

                    ActorTrailerWooHoo entry = definition.ProxyClone(WooHooer).CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as ActorTrailerWooHoo;
                    entry.LinkedInteractionInstance = this;
                    entry.WooHooer = WooHooer;
                    entry.WooHooee = WooHooee;

                    WooHooee.InteractionQueue.AddNext(entry);
                }

                if (!SafeToSync())
                {
                    return false;
                }

                if (!Target.RouteToAndEnterActorTrailer(Actor, this, false))
                {
                    return false;
                }

                StandardEntry(false);
                Actor.LoopIdle();
                if (!StartSync(mIsMaster))
                {
                    StandardExit(false, false);
                    return false;
                }

                BeginCommodityUpdates();

                try
                {
                    if (mIsMaster)
                    {
                        AcquireStateMachine("ActorTrailerSocials");
                        SetActorAndEnter("x", Actor, "FromRestOrSleep");
                        SetActorAndEnter("y", WooHooee, "FromRestOrSleep");
                        SetActor("Trailer", Target);
                        isWooHooing = true;
                        mWooHooReactionBroadcast = new ReactionBroadcaster(Target, ActorTrailer.kWooHooReactionBroadcastParams, PublicWooHooReactionCallback);
                        RockGemMetalBase.HandleNearbyWoohoo(Target, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                        AnimateJoinSims("WooHoo");

                        CommonWoohoo.TestNakedOutfit(Woohooer.Settings.mNakedOutfitActorTrailer, WooHooer, WooHooee);

                        CommonWoohoo.RunPostWoohoo(WooHooer, WooHooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                        if (CommonPregnancy.IsSuccess(WooHooer, WooHooee, Autonomous, definition.GetStyle(this)))
                        {
                            CommonPregnancy.Impregnate(WooHooer, WooHooee, Autonomous, definition.GetStyle(this));
                        }
                        RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                        isWooHooing = false;

                        AnimateNoYield("y", "ToRestOrSleep");
                        AnimateSim("ToRestOrSleep");
                    }
                    FinishLinkedInteraction(mIsMaster);
                    WaitForSyncComplete();
                }
                finally
                {
                    EndCommodityUpdates(true);
                }

                StandardExit(false, false);

                if (mIsMaster)
                {
                    //WooHooer.InteractionQueue.PushAsContinuation(ActorTrailer.Relax.Singleton.CreateInstance(Target, WooHooer, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true), true);
                    //WooHooee.InteractionQueue.PushAsContinuation(ActorTrailer.Relax.Singleton.CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true), true); 

                    if (mWooHooReactionBroadcast != null)
                    {
                        mWooHooReactionBroadcast.Dispose();
                        mWooHooReactionBroadcast = null;
                    }

                    foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>(Target.Position, ActorTrailer.kWooHooReactionBroadcastParams.PulseRadius))
                    {
                        if (sim.RoomId == Target.RoomId)
                        {
                            sim.PlayReaction(ReactionTypes.Cheer, Target, ReactionSpeed.NowOrLater);
                        }
                    }
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<ActorTrailer, ActorTrailerWooHoo, BaseDefinition>
        {
            public ProxyDefinition(BaseDefinition definition)
                : base(definition)
            { }
        }

        public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<ActorTrailer>
        {
            public BaseDefinition()
            { }
            public BaseDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.ActorTrailer; 
            }

            public override Sim GetTarget(Sim actor, ActorTrailer target, InteractionInstance interaction)
            {
                List<Sim> sims = new List<Sim>(target.ActorsUsingMe);
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

            public override bool JoinInProgress(Sim actor, Sim target, ActorTrailer obj, InteractionInstance interaction)
            {
                if ((obj.ActorsUsingMe.Contains(actor)) || (obj.ActorsUsingMe.Contains(target)))
                {
                    PushWooHoo(actor, target, obj);
                    return true;
                }
                else
                {
                    return base.JoinInProgress(actor, target, obj, interaction);
                }
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                ActorTrailer trailer = obj as ActorTrailer;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                ActorTrailerWooHoo instance = new ProxyDefinition(this).CreateInstance(trailer, actor, priority, false, true) as ActorTrailerWooHoo;
                instance.WooHooer = actor;
                instance.WooHooee = target;
                instance.mIsMaster = true;
                actor.InteractionQueue.PushAsContinuation(instance, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, ActorTrailer obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (isAutonomous)
                {
                    if (!ActorTrailer.DoSimsMeetWooHooRequirements(actor, target)) return false;
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

            public override string GetInteractionName(Sim actor, ActorTrailer target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, ActorTrailer obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "HotTubWoohoo", isAutonomous, true, true, ref callback);
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

            public override string GetInteractionName(Sim actor, ActorTrailer target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim actor, Sim target, ActorTrailer obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesRisky(actor, target, "HotTubRisky", isAutonomous, true, ref callback);
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

            public override string GetInteractionName(Sim actor, ActorTrailer target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, ActorTrailer obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(actor, target, "HotTubTryForBaby", isAutonomous, true, ref callback);
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
                get { return CommonWoohoo.WoohooLocation.ActorTrailer; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is ActorTrailer;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<ActorTrailer>(new Predicate<ActorTrailer>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<ActorTrailer>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP3)) return false;
                }

                return Woohooer.Settings.mAutonomousActorTrailer;
            }

            public bool TestUse(ActorTrailer obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (ActorTrailer obj in actor.LotCurrent.GetObjects<ActorTrailer>(new Predicate<ActorTrailer>(TestUse)))
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
                        return new ActorTrailerWooHoo.SafeDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new ActorTrailerWooHoo.RiskyDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new ActorTrailerWooHoo.TryForBabyDefinition(target);
                }

                return null;
            }
        }
    }
}
