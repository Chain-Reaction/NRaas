using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
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
    public class GiveTattooToSelfEx : TattooChair.GiveTattooToSelf, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<TattooChair, TattooChair.GiveTattooToSelf.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<TattooChair, TattooChair.GiveTattooToSelf.Definition>(Singleton);
        }

        private new void OnDisplayCas()
        {
            try
            {
                if (!Target.Repairable.Broken && (Target.Upgradable.Inkinization || !RandomUtil.RandomChance01(Tattooing.GetChanceOfFailure(Actor, Actor))))
                {
                    if (Target.TryDeductFunds(Actor, Actor))
                    {
                        bool tookSemaphore = mTookSemaphore;
                        GiveTattooEx.DisplayCAS(Actor, ref tookSemaphore);
                        mTookSemaphore = tookSemaphore;
                        EventTracker.SendEvent(EventTypeId.kGotTattoo, Actor);
                    }
                }
                else
                {
                    TattooChair.AddFailureTattoo(Actor);
                    EventTracker.SendEvent(EventTypeId.kGotTattoo, Actor);
                    Actor.ShowTNSIfSelectable(TattooChair.LocalizeString("FailureTnsSelf", new object[] { Actor }), StyledNotification.NotificationStyle.kGameMessageNegative, Target.ObjectId, Actor.ObjectId);
                }
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
                if (!Target.RouteToChairControls(Actor))
                {
                    return false;
                }
                EnterStateMachine(Target.StateMachineName, "Enter", "x");
                SetActor("tattooChair", Target);
                SetActor("y", Actor);
                AnimateSim("Tattoo Self Stand");
                AddPersistentScriptEventHandler(0x0, new SacsEventHandler(Target.OnSound));
                bool succeeded = DoTimedLoop(TattooChair.kTimeToPrepareSelfTattoo);
                AnimateSim("Exit");
                if (!succeeded || !Target.RouteToChair(Actor))
                {
                    return false;
                }
                EnterState("x", "Enter");
                EnterState("tattooChair", "Enter");
                Actor.SkillManager.AddElement(SkillNames.Tattooing);
                StandardEntry();
                BeginCommodityUpdates();
                SetParameter("isScared", TattooChair.ShouldPlayScaredReaction(Actor));
                AnimateSim("Tattoo Self");
                float timeToTattoo = Tattooing.GetTimeToTattoo(Actor);
                AlarmHandle handle = Target.AddAlarm(timeToTattoo * TattooChair.kTimeAtWhichToPerformAction, TimeUnit.Minutes, new AlarmTimerCallback(OnDisplayCas), "Tattoo Chair - Display Cas", AlarmType.DeleteOnReset);
                succeeded = DoTimedLoop(timeToTattoo);
                if (!succeeded)
                {
                    Target.RemoveAlarm(handle);
                }
                AnimateSim("Exit");
                Target.Repairable.UpdateBreakage(Actor);
                EndCommodityUpdates(succeeded);
                StandardExit();
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
                }
            }
        }
        
        public new class Definition : TattooChair.GiveTattooToSelf.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GiveTattooToSelfEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
