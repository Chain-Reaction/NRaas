using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HotAirBalloonSit : HotairBalloon.SitInBalloon, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        static readonly InteractionDefinition WithSingleton = new Definition(true);

        public bool mIsMaster;

        public void OnPreLoad()
        {
            Tunings.Inject<HotairBalloon, HotairBalloon.SitInBalloon.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<HotairBalloon, HotairBalloon.SitInBalloon.Definition>(Singleton);
            interactions.AddNoDupTest<HotairBalloon>(WithSingleton);
        }

        public override bool Run()
        {
            try
            {
                Sim linkedActor = GetSelectedObject() as Sim;
                if (linkedActor == Actor)
                {
                    linkedActor = null;
                }

                if (linkedActor != null)
                {
                    HotAirBalloonSit entry = Singleton.CreateInstance(Target, linkedActor, GetPriority(), Autonomous, CancellableByPlayer) as HotAirBalloonSit;
                    entry.LinkedInteractionInstance = this;
                    linkedActor.InteractionQueue.Add(entry);
                }

                HotAirBalloonSit linked = LinkedInteractionInstance as HotAirBalloonSit;
                if (linked != null)
                {
                    linkedActor = linked.Actor;
                }

                bool isXActor = Target.mLeftActorId == 0x0L;
                if (!isXActor && (Target.mRightActorId != 0x0L))
                {
                    return false;
                }

                if (isXActor)
                {
                    Target.mLeftActorId = Actor.SimDescription.SimDescriptionId;
                }
                else
                {
                    Target.mRightActorId = Actor.SimDescription.SimDescriptionId;
                }

                Route r = Actor.CreateRoute();

                if (linkedActor != null)
                {
                    r.AddObjectToIgnoreForRoute(linkedActor.ObjectId);
                }

                if (!r.PlanToSlot(Target, Slot.RoutingSlot_7).Succeeded())
                {
                    if (isXActor)
                    {
                        Target.mLeftActorId = 0x0L;
                    }
                    else
                    {
                        Target.mRightActorId = 0x0L;
                    }
                    return false;
                }

                if ((Target.mTargetHeight != HotairBalloon.BalloonHeight.OnGround) || (Target.mCurrentHeight != HotairBalloon.BalloonHeight.OnGround))
                {
                    Actor.PlayRouteFailure();

                    if (isXActor)
                    {
                        Target.mLeftActorId = 0x0L;
                    }
                    else
                    {
                        Target.mRightActorId = 0x0L;
                    }
                    return false;
                }

                r = Actor.CreateRoute();
                if (linkedActor != null)
                {
                    r.AddObjectToIgnoreForRoute(linkedActor.ObjectId);
                }

                r.AddObjectToIgnoreForRoute(Target.ObjectId);
                if (!r.PlanToSlot(Target, isXActor ? Slot.RoutingSlot_5 : Slot.RoutingSlot_2).Succeeded())
                {
                    Target.ResetParentingHierarchy(true);
                    r.PlanToSlot(Target, isXActor ? Slot.RoutingSlot_5 : Slot.RoutingSlot_2);
                }

                if (!Actor.DoRoute(r))
                {
                    if (isXActor)
                    {
                        Target.mLeftActorId = 0x0L;
                    }
                    else
                    {
                        Target.mRightActorId = 0x0L;
                    }
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();
                Animation.ForceAnimation(Actor.ObjectId, true);
                if (Target.mFlyStateMachine == null)
                {
                    Target.mFlyStateMachine = StateMachineClient.Acquire(Target, "hotairballoon_store", AnimationPriority.kAPDefault);
                    Target.mFlyStateMachine.SetActor("hotairBalloonX", Target);
                    Target.mFlyStateMachine.EnterState("hotairBalloonX", "BalloonLiftOff");
                }

                AcquireStateMachine("hotairballoon_store");
                mCurrentStateMachine.SetActor("x", Actor);
                mCurrentStateMachine.SetActor("hotairBalloonX", Target);
                mCurrentStateMachine.SetParameter("XSimR", isXActor ? YesOrNo.no : YesOrNo.yes);
                mCurrentStateMachine.EnterState("x", "EnterBalloon");
                Slot slotName = isXActor ? ((Slot)(0xa820f8a5)) : ((Slot)(0xa820f8a2));
                float f = (float)Math.Acos((double)(Target.ForwardVector * Actor.ForwardVector));
                if (float.IsNaN(f))
                {
                    f = 3.141593f;
                }

                Actor.ParentToSlot(Target, slotName, f, false);
                mCurrentStateMachine.RequestState("x", "IdleStand");
                EndCommodityUpdates(true);

                HotairBalloon.InBalloonPosture posture = new HotairBalloon.InBalloonPosture(Actor, Target, isXActor, mCurrentStateMachine);
                Actor.Posture = posture;

                if ((Autonomous) && (RandomUtil.RandomChance(HotairBalloon.kChanceToCallOverRomanticInterest)))
                {
                    Sim actor = Target.FindRomanticSimToRideWith(Actor);
                    if ((actor != null) && !actor.InteractionQueue.HasInteractionOfType(Singleton))
                    {
                        InteractionInstance entry = Singleton.CreateInstance(Target, actor, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true);
                        actor.InteractionQueue.Add(entry);
                        mCurrentStateMachine.RequestState("x", "CallOver");
                        mCurrentStateMachine.RequestState("x", "IdleStand");
                        mCurrentStateMachine.RequestState("x", "CallOver");
                        mCurrentStateMachine.RequestState("x", "IdleStand");
                    }
                }

                StandardExit();
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

        public new class Definition : HotairBalloon.SitInBalloon.Definition
        {
            bool mWith;

            public Definition()
            { }
            public Definition(bool with)
            {
                mWith = with;
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new HotAirBalloonSit();
                na.Init(ref parameters);
                return na;
            }

            protected bool OnTest(Sim sim)
            {
                if (!sim.IsHuman) return false;

                return true;
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                if (!mWith)
                {
                    base.PopulatePieMenuPicker(ref parameters, out listObjs, out headers, out NumSelectableRows);
                }
                else
                {
                    List<Sim> sims = parameters.Actor.LotCurrent.GetSims(OnTest);
                    
                    NumSelectableRows = 1;
                    PopulateSimPicker(ref parameters, out listObjs, out headers, sims, false);
                }
            }

            public override string GetInteractionName(Sim actor, HotairBalloon target, InteractionObjectPair iop)
            {
                if (mWith)
                {
                    return Common.LocalizeEAString(actor.IsFemale, "NRaas.Woohooer.GetInWith:MenuName");
                }
                else
                {
                    return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
                }
            }
        }
    }
}


