using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Telemetry;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.TravelerSpace.Interactions
{
    public interface ITravelWith
    {
        bool AddFollower(Sim sim);
    }

    public class TimePortalTravelWithEx : TimePortal.TravelWith, ITravelWith, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<TimePortal, TimePortal.TravelWith.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
                tuning.SetFlags(InteractionTuning.FlagField.DisallowAutonomous, true);
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<TimePortal, TimePortal.TravelWith.Definition>(Singleton);
        }

        private static Dictionary<Sim, OpportunityNames> CheckOpportunities(Sim actor, List<Sim> sims)
        {
            Dictionary<Sim, OpportunityNames> dictionary = new Dictionary<Sim, OpportunityNames>();
            sims.Add(actor);
            foreach (Sim sim in sims)
            {
                for (int i = 0x0; i < TimePortal.kBannedOpportunities.Length; i++)
                {
                    if ((sim.OpportunityManager != null) && sim.OpportunityManager.HasOpportunity(TimePortal.kBannedOpportunities[i]))
                    {
                        dictionary.Add(sim, TimePortal.kBannedOpportunities[i]);
                    }
                }
            }
            sims.Remove(actor);
            return dictionary;
        }

        public static bool PreTimeTravel1(InteractionInstance ths, ITravelWith travelWith, List<Sim> travelers)
        {
            Sim actor = ths.InstanceActor as Sim;

            if (!UIUtils.IsOkayToStartModalDialog())
            {
                return false;
            }

            if (GameUtils.IsFutureWorld())
            {
                foreach (Sim sim in actor.Household.Sims)
                {
                    if (sim != actor)
                    {
                        GreyedOutTooltipCallback callback = null;
                        if (TravelerSpace.Helpers.TravelUtilEx.CanSimTriggerTravelToFutureWorld(sim, true, ref callback))
                        {
                            Lazy.Add<List<Sim>, Sim>(ref travelers, sim);
                        }
                    }
                }
            }
            else
            {
                travelers.AddRange(ths.GetSelectedObjectsAsSims());
            }

            if (travelers.Count == 0)
            {
                return false;
            }

            if ((actor.OpportunityManager != null) && actor.OpportunityManager.HasOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler01))
            {
                return false;
            }

            string portal;

            TravelUtil.PlayerMadeTravelRequest = true;
            Dictionary<Sim, OpportunityNames> dictionary = CheckOpportunities(actor, travelers);
            if (GameUtils.IsFutureWorld())
            {
                portal = TimePortal.LocalizeString("ConfirmReturnWith", new object[0x0]);
            }
            else if (dictionary.Count > 0x0)
            {
                portal = TimePortal.LocalizeString("ConfirmTravelWith", new object[0x0]) + TimePortal.LocalizeString("ConfirmWarningTravelWith", new object[0x0]);
            }
            else
            {
                portal = TimePortal.LocalizeString("ConfirmTravelWith", new object[0x0]);
            }

            if (!TwoButtonDialog.Show(portal, Localization.LocalizeString("Ui/Caption/Global:Accept", new object[0x0]), Localization.LocalizeString("Ui/Caption/Global:Cancel", new object[0x0])))
            {
                return false;
            }

            ths.CancellableByPlayer = false;
            //Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Ui/Caption/HUD/DisasterSaveError:Traveling");
            if (dictionary.Count > 0x0)
            {
                foreach (Sim sim2 in dictionary.Keys)
                {
                    if (sim2.OpportunityManager != null)
                    {
                        sim2.OpportunityManager.CancelOpportunity(dictionary[sim2]);
                    }
                }
            }

            if ((GameUtils.IsFutureWorld() && (actor.OpportunityManager != null)) && actor.OpportunityManager.HasOpportunity(OpportunityNames.EP11_RecalibrateDefenseGrid))
            {
                actor.OpportunityManager.CancelOpportunity(OpportunityNames.EP11_RecalibrateDefenseGrid);
            }

            foreach (Sim sim3 in travelers)
            {
                travelWith.AddFollower(sim3);
            }

            return true;
        }

        public static bool PreTimeTravel2(InteractionInstance ths, List<Sim> travelers)
        {
            Sim actor = ths.InstanceActor as Sim;

            foreach (Sim sim5 in travelers)
            {
                sim5.SkillManager.AddElement(SkillNames.Future);
            }

            BuffTransformation transformBuff = actor.BuffManager.TransformBuff;
            if (transformBuff != null)
            {
                actor.BuffManager.RemoveElement(transformBuff.Guid);
            }

            foreach (Sim sim6 in travelers)
            {
                transformBuff = sim6.BuffManager.TransformBuff;
                if (transformBuff != null)
                {
                    sim6.BuffManager.RemoveElement(transformBuff.Guid);
                }
            }

            if (GameUtils.IsFutureWorld())
            {
                SendEventForActorAndFollowers(actor, travelers, EventTypeId.kTravelToPresent);
            }
            else
            {
                SendEventForActorAndFollowers(actor, travelers, EventTypeId.kTravelToFuture);
            }

            ths.StandardEntry();
            ths.BeginCommodityUpdates();

            return true;
        }

        public new static void SendEventForActorAndFollowers(Sim actor, List<Sim> followers, EventTypeId eventType)
        {
            EventTracker.SendEvent(eventType, actor);
            foreach (Sim sim in followers)
            {
                EventTracker.SendEvent(eventType, sim);
            }
        }

        public override bool Run()
        {
            try
            {
                if (Responder.Instance.TutorialModel.IsTutorialRunning())
                {
                    if (!Target.CancelTutorial())
                    {
                        return false;
                    }
                    Responder.Instance.TutorialModel.ForceExitTutorial();
                }

                List<Sim> lazyList = new List<Sim>();
                if (!PreTimeTravel1(this, this, lazyList)) return false;

                foreach (Sim sim4 in lazyList)
                {
                    if (!sim4.SimDescription.Baby)
                    {
                        TimePortal.GatherAround entry = TimePortal.GatherAround.Singleton.CreateInstance(Target, sim4, GetPriority(), Autonomous, CancellableByPlayer) as TimePortal.GatherAround;
                        entry.mMasterInteraction = this;
                        sim4.InteractionQueue.AddNext(entry);
                    }
                }

                int num;
                if (!Actor.RouteToSlotListAndCheckInUse(Target, TimePortal.kRoutingSlots, out num))
                {
                    CancelFollowers(lazyList);
                    return false;
                }

                if (Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                {
                    CancelFollowers(lazyList);
                    return false;
                }

                if (!PreTimeTravel2(this, lazyList)) return false;

                EnterStateMachine("timeportal", "Enter", "x", "portal");
                AddPersistentScriptEventHandler(0xc9, CameraShakeEvent);
                AnimateSim("Call Over");

                Skill futureSkill = Actor.SkillManager.AddElement(SkillNames.Future);
                if (futureSkill.SkillLevel >= 0x3)
                {
                    AnimateSim("Jump In");
                }
                else
                {
                    AnimateSim("Apprehensive");
                }

                bool succeeded = true;
                if (!GameUtils.IsFutureWorld())
                {
                    // Custom
                    succeeded = TimePortalEx.TravelToFuture(Target, Actor, new List<Sim>(mFollowerGroup), new List<ulong>(mFollowerGroupIDs));
                }

                if (succeeded)
                {
                    foreach (Sim sim7 in lazyList)
                    {
                        futureSkill = sim7.SkillManager.GetElement(SkillNames.Future);
                        if (futureSkill != null)
                        {
                            futureSkill.AddPoints(kFollowerFutureSkillPointGain, true, false);
                        }
                    }
                }
                else
                {
                    CancelFollowers(lazyList);
                    SpeedTrap.Sleep(0x3c);
                    AnimateSim("Spit Out");
                    Target.SwitchActiveState();
                }

                AnimateSim("Exit");
                EndCommodityUpdates(succeeded);
                StandardExit();
                if (GameUtils.IsFutureWorld())
                {
                    Target.StopActiveFX();

                    // Custom
                    GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();

                    SendEventForActorAndFollowers(Actor, lazyList, EventTypeId.kTravelToPresent);
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
            }
            finally
            {
                TravelUtil.PlayerMadeTravelRequest = false;
                //Sims3.Gameplay.Gameflow.Singleton.EnableSave(this);
            }

            return false;
        }

        public new class Definition : TimePortal.TravelWith.Definition
        {
            public override string GetInteractionName(Sim actor, TimePortal target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                if (GameUtils.IsFutureWorld())
                {
                    base.PopulatePieMenuPicker(ref parameters, out listObjs, out headers, out NumSelectableRows);
                }
                else
                {
                    listObjs = null;
                    headers = null;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();
                    foreach (Sim sim2 in Households.AllSims(actor.Household))
                    {
                        GreyedOutTooltipCallback callback = null;
                        if (TravelerSpace.Helpers.TravelUtilEx.CanSimTriggerTravelToFutureWorld(sim2, true, ref callback))
                        {
                            sims.Add(sim2);
                        }
                    }

                    NumSelectableRows = sims.Count - 0x1;
                    if (NumSelectableRows > 0x0)
                    {
                        base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, false);
                    }
                }
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TimePortalTravelWithEx();
                na.Init(ref parameters);
                return na;
            }

            public static bool PublicTest(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((target.Repairable != null) && target.Repairable.Broken)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Broken");
                    return false;
                }

                if (TravelUtil.PlayerMadeTravelRequest)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("PlayerMadeTravelRequest");
                    return false;
                }

                if (target.InUse && !target.IsActorUsingMe(actor))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("InUse");
                    return false;
                }

                CauseEffectService instance = CauseEffectService.GetInstance();
                if ((instance != null) && !instance.ShouldShowTimeAlmanacButton())
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("ShouldShowTimeAlmanacButton");
                    return false;
                }

                if (actor.BuffManager.HasElement(BuffNames.Ensorcelled))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Ensorcelled");
                    return false;
                }

                if (actor.Household.Sims.Count <= 0x1)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Single");
                    return false;
                }

                // Custom
                if (!TravelerSpace.Helpers.TravelUtilEx.CanSimTriggerTravelToFutureWorld(actor, true, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                if (GameUtils.IsFutureWorld())
                {
                    foreach (Sim sim in Households.AllSims(actor.Household))
                    {
                        // Custom
                        if ((sim != actor) && !TravelerSpace.Helpers.TravelUtilEx.CanSimTriggerTravelToFutureWorld(sim, true, ref greyedOutTooltipCallback))
                        {
                            //greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, TimePortal.sLocalizationKey + ":UnfitForTravel", new object[] { actor }));
                            return false;
                        }
                    }
                }
                else
                {
                    Common.StringBuilder result = new Common.StringBuilder();

                    List<Sim> list = new List<Sim>();
                    foreach (Sim sim2 in Households.AllSims(actor.Household))
                    {
                        // Custom
                        if ((sim2 != actor) && TravelerSpace.Helpers.TravelUtilEx.CanSimTriggerTravelToFutureWorld(sim2, true, ref greyedOutTooltipCallback))
                        {
                            if (greyedOutTooltipCallback != null)
                            {
                                result += Common.NewLine + greyedOutTooltipCallback();
                            }

                            list.Add(sim2);
                        }
                    }

                    if (list.Count == 0x0)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, TimePortal.sLocalizationKey + ":UnfitForTravel", new object[] { actor }) + Common.NewLine + result);
                        return false;
                    }
                }
                return true;
            }

            public override bool Test(Sim actor, TimePortal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (TravelUtil.PlayerMadeTravelRequest)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("PlayerMadeTravelRequest");
                    return false;
                }

                if (!TimePortal.sTimeTravelerHasBeenSummoned)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("sTimeTravelerHasBeenSummoned");
                    return false;
                }

                if (target.InUse && !target.IsActorUsingMe(actor))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("InUse");
                    return false;
                }

                if (!target.Active)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Active Fail");
                    return false;
                }

                if (target.IsBroken)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, TimePortal.sLocalizationKey + ":MustRepairTimePortal", new object[] { actor }));
                    return false;
                }

                return PublicTest(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
