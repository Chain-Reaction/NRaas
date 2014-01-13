using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class GiveTattooEx : TattooChair.GiveTattoo, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<TattooChair, TattooChair.GiveTattoo.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        private bool TryDeductFunds(TattooChair ths, Sim giver, Sim receiver)
        {
            if (giver == receiver)
            {
                if (giver.FamilyFunds >= Tattooing.kCostTattooSelf)
                {
                    receiver.ModifyFunds(-Tattooing.kCostTattooSelf);
                    return true;
                }
                return false;
            }
            if (giver == ths.GetTattooArtist())
            {
                if (!CelebrityManager.TryModifyFundsWithCelebrityDiscount(receiver, giver, Tattooing.kCostTattooFromTattooArtist, true))
                {
                    return false;
                }
                return true;
            }
            if (!CelebrityManager.TryModifyFundsWithCelebrityDiscount(receiver, giver, Tattooing.kCostTattooFromSim, true))
            {
                return false;
            }
            return true;
        }

        private new void OnDisplayCas()
        {
            try
            {
                Sim instanceActor = LinkedInteractionInstance.InstanceActor;
                if (instanceActor == null) return;

                bool flag = !Target.Repairable.Broken && (Target.Upgradable.Inkinization || !RandomUtil.RandomChance01(Tattooing.GetChanceOfFailure(Actor, instanceActor)));
                if (!flag)
                {
                    Sim actor = instanceActor;
                    ObjectGuid choice = ObjectGuid.InvalidObjectGuid;
                    if (Actor.IsSelectable)
                    {
                        actor = Actor;
                        choice = instanceActor.ObjectId;
                    }

                    if (instanceActor.IsSelectable || Actor.IsSelectable)
                    {
                        TattooChair.AddFailureTattoo(instanceActor);
                    }

                    actor.ShowTNSIfSelectable(TattooChair.LocalizeString("FailureTns" + RandomUtil.GetInt(0x2), new object[0x0]), StyledNotification.NotificationStyle.kSimTalking, Actor.ObjectId, choice);
                }
                else if ((Actor.IsSelectable || instanceActor.IsSelectable) && TryDeductFunds(Target, Actor, instanceActor))
                {
                    bool tookSemaphore = mTookSemaphore;
                    DisplayCAS(instanceActor, ref tookSemaphore);
                    mTookSemaphore = tookSemaphore;
                    EventTracker.SendEvent(EventTypeId.kGotTattoo, instanceActor);
                }

                ActiveTopic.AddToSim(instanceActor, "Got a Tattoo");
                
                (LinkedInteractionInstance as TattooChair.GetTattoo).IsSuccess = flag;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public override bool Run()
        {
            try
            {
                AlarmHandle handle;
                float kTimeToRemoveTattoos;
                if (!Target.RouteToChairControls(Actor))
                {
                    return false;
                }
                if (!StartSync(true))
                {
                    return false;
                }
                Actor.LoopIdle();
                Actor.SkillManager.AddElement(SkillNames.Tattooing);
                StandardEntry();

                BeginCommodityUpdates();

                Sim instanceActor = LinkedInteractionInstance.InstanceActor;
                Target.OnStartTattooing(instanceActor);
                AcquireStateMachine(Target.StateMachineName, AnimationPriority.kAPNormalPlus);
                SetActorAndEnter(Target.ActorForTattooer, Actor, "Enter");
                SetActor("tattooChair", Target);
                SetActor(Target.ActorForTattooed, instanceActor);
                SetParameter("isScared", TattooChair.ShouldPlayScaredReaction(instanceActor));
                AddPersistentScriptEventHandler(0x0, new SacsEventHandler(Target.OnSound));
                EnterState(Target.ActorForTattooed, "Enter");
                AnimateSim("Tattoo");

                kTimeToRemoveTattoos = Tattooing.GetTimeToTattoo(Actor);
                handle = Target.AddAlarm(kTimeToRemoveTattoos * TattooChair.kTimeAtWhichToPerformAction, TimeUnit.Minutes, OnDisplayCas, "Tattoo Chair - Display Cas", AlarmType.DeleteOnReset);

                bool succeeded = DoTimedLoop(kTimeToRemoveTattoos);
                if (!succeeded)
                {
                    Target.RemoveAlarm(handle);
                }

                AnimateSim("Exit");
                FinishLinkedInteraction(true);
                EndCommodityUpdates(succeeded);
                StandardExit();
                WaitForSyncComplete();
                Target.Repairable.UpdateBreakage(Actor.IsSelectable ? Actor : instanceActor);
                return succeeded;
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

        public static void DisplayCAS(Sim simInCAS, ref bool tookSemaphore)
        {
            if (!Responder.Instance.OptionsModel.SaveGameInProgress)
            {
                bool flag = GameStates.WaitForInteractionStateChangeSemaphore(simInCAS, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                tookSemaphore = flag;
                if (tookSemaphore)
                {
                    while ((Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause) || MoveDialog.InProgress())
                    {
                        SpeedTrap.Sleep();
                    }

                    new Sims.Basic.Tattoo().Perform(new GameHitParameters<GameObject>(simInCAS, simInCAS, GameObjectHit.NoHit));

                    while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                    {
                        SpeedTrap.Sleep();
                    }
                }
            }
        }

        public new class Definition : TattooChair.GiveTattoo.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GiveTattooEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
