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
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
    public class SaunaSit : Sit, Common.IPreLoad, Common.IAddInteraction
    {
        public static new readonly InteractionDefinition Singleton = new Definition();
        public static readonly InteractionDefinition WoohooSingleton = new Definition(true);

        public bool mIsMaster;

        public bool mCompleted;

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, Sit.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<SaunaClassic, Sit.Definition>(Singleton);
        }

        public override void Cleanup()
        {
            mCompleted = true;

            base.Cleanup();
        }

        public override bool Run()
        {
            try
            {
                SitData data2;
                Slot slot2;
                object obj2;
                SittingPosture posture2;
                ISittable sittable = SittingHelpers.CastToSittable(Target);
                if (sittable == null)
                {
                    Actor.AddExitReason(ExitReason.FailedToStart);
                    return false;
                }

                Slot containmentSlotClosestToHit = GetContainmentSlotClosestToHit();
                if (Actor.Posture.Container == Target)
                {
                    SittingPosture posture = Actor.Posture as SittingPosture;
                    if (posture != null)
                    {
                        SitData target = posture.Part.Target;
                        if (containmentSlotClosestToHit == target.ContainmentSlot)
                        {
                            return true;
                        }
                        if (!Stand.Singleton.CreateInstance(Target, Actor, GetPriority(), Autonomous, CancellableByPlayer).RunInteraction())
                        {
                            return false;
                        }
                    }
                }

                SimQueue simLine = Target.SimLine;
                if ((simLine != null) && !simLine.WaitForTurn(this, SimQueue.WaitBehavior.DontPlayRouteFail | SimQueue.WaitBehavior.NeverWait, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 0f))
                {
                    Sim firstSim = simLine.FirstSim;
                    if ((firstSim != null) && (firstSim.InteractionQueue.TransitionInteraction is Stand))
                    {
                        Actor.RemoveExitReason(ExitReason.ObjectInUse);
                        simLine.WaitForTurn(this, SimQueue.WaitBehavior.DontPlayRouteFail | SimQueue.WaitBehavior.OnlyWaitAtHeadOfLine, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), kTimeToWait);
                    }
                }

                if (!sittable.RouteToForSitting(Actor, containmentSlotClosestToHit, true, out data2, out slot2, out obj2))
                {
                    return false;
                }

                sittable = SittingHelpers.CastToSittable(data2.Container);
                if (!SittingHelpers.ReserveSittable(this, Actor, sittable, data2))
                {
                    return false;
                }

                StateMachineClient smc = sittable.StateMachineAcquireAndInit(Actor);
                if (smc == null)
                {
                    Actor.AddExitReason(ExitReason.NullValueFound);
                    SittingHelpers.UnreserveSittable(this, sittable, data2);
                    return false;
                }

                ISittingPostureCreator parent = data2.Container.Parent as ISittingPostureCreator;
                if (parent != null)
                {
                    posture2 = parent.CreatePosture(data2.Container, Actor, smc, data2);
                }
                else
                {
                    posture2 = new SittingPosture(data2.Container, Actor, smc, data2);
                }

                if (smc.HasActorDefinition("surface"))
                {
                    smc.SetActor("surface", data2.Container);
                }

                BeginCommodityUpdates();
                Actor.LookAtManager.DisableLookAts();
                bool flag = (Actor.CarryStateMachine != null) && (Actor.GetObjectInRightHand() is IUseCarrySitTransitions);
                if (flag)
                {
                    Actor.CarryStateMachine.RequestState(false, "x", "CarrySitting");
                }

                Definition definition = InteractionDefinition as Definition;
                if (!SaunaClassicEx.StateMachineEnterAndSit(sittable as SaunaClassic, definition.mForWoohoo, smc, posture2, slot2, obj2))
                {
                    if (flag)
                    {
                        Actor.CarryStateMachine.RequestState(false, "x", "Carry");
                    }
                    Actor.LookAtManager.EnableLookAts();
                    Actor.AddExitReason(ExitReason.NullValueFound);
                    SittingHelpers.UnreserveSittable(this, sittable, data2);
                    EndCommodityUpdates(false);
                    return false;
                }

                Actor.LookAtManager.EnableLookAts();
                Actor.Posture = posture2;
                if (sittable.ComfyScore > 0x0)
                {
                    Actor.BuffManager.AddElement(BuffNames.Comfy, sittable.ComfyScore, Origin.FromComfyObject);
                }

                EndCommodityUpdates(true);
                StandardExit(false, false);
                if (Actor.HasExitReason(ExitReason.UserCanceled))
                {
                    Actor.AddExitReason(ExitReason.CancelledByPosture);
                }

                if (mIsMaster)
                {
                    SaunaSit linked = LinkedInteractionInstance as SaunaSit;
                    if (linked != null)
                    {
                        Sim linkedActor = linked.Actor;

                        while (!Cancelled)
                        {
                            if (!linkedActor.InteractionQueue.HasInteraction(linked)) break;

                            if (linked.mCompleted) break;

                            SpeedTrap.Sleep(10);
                        }
                    }
                }

                return !Actor.HasExitReason();
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

        public new class Definition : Sit.Definition
        {
            public readonly bool mForWoohoo;

            public Definition()
            { }
            public Definition(bool forWoohoo)
            {
                mForWoohoo = forWoohoo;
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SaunaSit();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(Sit.Singleton, target));
            }
        }
    }
}


