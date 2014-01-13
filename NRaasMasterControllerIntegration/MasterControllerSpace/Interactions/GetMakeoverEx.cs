using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
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
    public class GetMakeoverEx : StylingStation.GetMakeover, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<StylingStation, StylingStation.GetMakeover.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        /*
        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<StylingStation, StylingStation.GetMakeover.Definition>(Singleton);
        }
        */

        public override bool Run()
        {
            try
            {
                bool succeeded = false;
                if (Styler == null)
                {
                    return false;
                }
                else if ((Actor.LotCurrent != Styler.LotCurrent) && !RouteActorToTargetLot())
                {
                    return false;
                }
                else if (ShouldPushOtherHalf && !PushOtherHalf(StylingStation.GiveMakeover.Singleton, Actor, Styler, null))
                {
                    return false;
                }
                else if (!Actor.RouteToSlot(Target, StylingStation.kPlatformRoutingSlot))
                {
                    return false;
                }
                else if (!Styling.CanSimReceiveMakeover(Actor, Styler, Autonomous))
                {
                    return false;
                }
                else if (Target.UseCount > 0x1)
                {
                    return false;
                }
                else if (Target.UseCount == 0x1)
                {
                    if (!Target.IsActorUsingMe(Styler))
                    {
                        return false;
                    }
                    if (Styler.InteractionQueue.GetHeadInteraction() != LinkedInteractionInstance)
                    {
                        return false;
                    }
                }

                StandardEntry();
                Styling.PreMakeover(Actor, Styler);
                AddSynchronousOneShotScriptEventHandler(0x384, SnapSimToPlatform);
                AddSynchronousOneShotScriptEventHandler(0x385, SnapSimToGround);
                Animate("x", "Get On Platform");
                Actor.LoopIdle();
                if (!StartSync(true))
                {
                    Animate("x", "Get Off Platform");
                    Animate("x", "Exit");
                    StandardExit(true, false);
                    return false;
                }
                BeginCommodityUpdates();
                AddPersistentScriptEventHandler(0x7a6a, DisplayThoughtBalloon);
                AddPersistentScriptEventHandler(0x7a69, DisplayThoughtBalloon);
                bool flag2 = false;
                int num = 0x0;
                do
                {
                    num++;
                    flag2 = false;
                    AnimateJoinSims("Customer and Stylist Loop");
                    float duration = (Styler.IsNPC && Actor.IsNPC) ? StylingStation.kNPCMakeoverDurationInMinutes : StylingStation.kMakeoverDurationInMinutes;
                    succeeded = DoTimedLoop(duration);
                    AnimateNoYield("y", "Stylist Idle");
                    StylingStation.GiveMakeover linkedInteractionInstance = LinkedInteractionInstance as StylingStation.GiveMakeover;
                    linkedInteractionInstance.DoStylistIdle = true;
                    if (succeeded)
                    {
                        bool enteringCAS = linkedInteractionInstance.SimInCAS != null;
                        Styling.MakeoverOutcome makeoverOutcome = Styling.GetMakeoverOutcome(Actor, Styler, enteringCAS);
                        bool epicMakeoverFailure = makeoverOutcome == Styling.MakeoverOutcome.EpicFailure;
                        if (!enteringCAS)
                        {
                            Sim.SwitchOutfitHelper switchOutfitHelper = null;
                            Styling.LoadMakeoverOutfitForClothesSpin(Actor, epicMakeoverFailure, Autonomous, ref switchOutfitHelper);
                            mSwitchOutfitHelper = switchOutfitHelper;
                        }
                        else if (epicMakeoverFailure)
                        {
                            Styling.LoadMakeoverEpicFailureOutfitForCasOverride(Actor);
                        }
                        Animate("x", "Gussy Anims Done");
                        if (!enteringCAS && (mSwitchOutfitHelper != null))
                        {
                            mSwitchOutfitHelper.Wait(true);
                            mSwitchOutfitHelper.AddScriptEventHandler(this);
                        }

                        if (enteringCAS)
                        {
                            bool tookSemaphore = mTookSemaphore;
                            enteringCAS = DisplayCAS(linkedInteractionInstance.SimInCAS, Styler, ref tookSemaphore, epicMakeoverFailure);
                            mTookSemaphore = tookSemaphore;
                            ReleaseSemaphore();
                            succeeded = CASChangeReporter.Instance.GetPropertyChanged(CASChangeReporter.ChangeFlags.Any);
                        }

                        if (succeeded)
                        {
                            float[] reactionWeightsBonus = Target.UpgradableComponent.LookGoodMirrors ? StylingStation.kUpgradeReactionWeightsBonus : null;
                            SkillLevel paramValue = Styling.GetCustomerReactionType(Actor, Styler, makeoverOutcome, num >= 0x2, reactionWeightsBonus);
                            SetParameter("doClothesSpin", !enteringCAS);
                            SetParameter("customerReactionType", paramValue);
                            Animate("x", "Customer Reaction");
                            Styling.PostMakeover(Actor, Styler, makeoverOutcome, false, paramValue, enteringCAS, enteringCAS, OnMakeoverCompleted);
                            SkillLevel stylerReactionType = Styling.GetStylerReactionType(paramValue);
                            SetParameter("stylistReactionType", stylerReactionType);
                            AnimateNoYield("y", "Stylist Reaction");
                            (LinkedInteractionInstance as StylingStation.GiveMakeover).DoStylistIdle = true;
                            Actor.LoopIdle();
                            if (((Actor.IsNPC && Styler.IsNPC) && (paramValue == SkillLevel.poor)) && ((num <= 0x1) && Actor.HasNoExitReason()))
                            {
                                flag2 = true;
                            }
                        }
                        else
                        {
                            (LinkedInteractionInstance as StylingStation.GiveMakeover).MakeoverCancelled = true;
                        }
                    }
                }
                while (flag2);
                Animate("x", "Get Off Platform");
                AnimateNoYield("y", "Exit Stylist");
                Animate("x", "Exit");
                FinishLinkedInteraction(true);
                EndCommodityUpdates(succeeded);
                StandardExit(true, false);
                WaitForSyncComplete();
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

        public static bool DisplayCAS(Sim simInCAS, Sim stylerSim, ref bool tookSemaphore, bool forceFailureOutfit)
        {
            if (!Responder.Instance.OptionsModel.SaveGameInProgress)
            {
                tookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(simInCAS, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                if (!tookSemaphore)
                {
                    return false;
                }
                Sim sim = stylerSim ?? simInCAS;
                if (sim.Household == Household.ActiveHousehold)
                {
                    while ((Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause) || MoveDialog.InProgress())
                    {
                        SpeedTrap.Sleep();
                    }

                    StyledNotification notification = null;
                    if (stylerSim != null)
                    {
                        Stylist occupation = stylerSim.Occupation as Stylist;
                        if (occupation != null)
                        {
                            Stylist.Makeover currentJob = occupation.CurrentJob as Stylist.Makeover;
                            if ((currentJob != null) && (currentJob.MakeoverTarget == simInCAS))
                            {
                                string titleText = currentJob.JobTitle + ":" + Common.NewLine + Common.NewLine;
                                foreach (TaskInfo info in occupation.GetTaskNames().Values)
                                {
                                    titleText = titleText + "- " + info.TaskDescription + Common.NewLine;
                                }
                                StyledNotification.Format format = new StyledNotification.Format(titleText, StyledNotification.NotificationStyle.kGameMessagePositive);
                                notification = StyledNotification.Show(format);
                            }
                        }
                    }

                    new Sims.Stylist().Perform(new GameHitParameters<GameObject>(simInCAS, simInCAS, GameObjectHit.NoHit));

                    if (forceFailureOutfit)
                    {
                        CASLogic.GetSingleton().SetOverrideExitOutfit(OutfitCategories.Makeover, 0x0);
                    }

                    while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                    {
                        SpeedTrap.Sleep();
                    }

                    if (notification != null)
                    {
                        notification.CloseNow();
                    }

                    Styling.UpdateJobTrackerIfNecessary(simInCAS, stylerSim, forceFailureOutfit);
                    return true;
                }
            }
            return false;
        }

        public new class Definition : StylingStation.GetMakeover.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GetMakeoverEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, StylingStation target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
