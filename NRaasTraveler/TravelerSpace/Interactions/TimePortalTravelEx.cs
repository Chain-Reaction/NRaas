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
    public class TimePortalTravelEx : TimePortal.Travel, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<TimePortal, TimePortal.Travel.Definition, Definition>(false);
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
            interactions.Replace<TimePortal, TimePortal.Travel.Definition>(Singleton);
        }

        private static OpportunityNames CheckOpportunities(Sim actor)
        {
            for (int i = 0x0; i < TimePortal.kBannedOpportunities.Length; i++)
            {
                if ((actor.OpportunityManager != null) && actor.OpportunityManager.HasOpportunity(TimePortal.kBannedOpportunities[i]))
                {
                    return TimePortal.kBannedOpportunities[i];
                }
            }
            return OpportunityNames.Undefined;
        }

        public static bool PreTimeTravel1(InteractionInstance ths)
        {
            if (!UIUtils.IsOkayToStartModalDialog())
            {
                return false;
            }

            Sim actor = ths.InstanceActor as Sim;

            TravelUtil.PlayerMadeTravelRequest = true;
            OpportunityNames guid = CheckOpportunities(actor);

            string prompt;
            if (GameUtils.IsFutureWorld())
            {
                prompt = TimePortal.LocalizeString("ConfirmReturn", new object[0x0]);
            }
            else if (guid != OpportunityNames.Undefined)
            {
                prompt = TimePortal.LocalizeString("ConfirmWarningTravel", new object[0x0]);
            }
            else
            {
                prompt = TimePortal.LocalizeString("ConfirmTravel", new object[0x0]);
            }
            if (!TwoButtonDialog.Show(prompt, Localization.LocalizeString("Ui/Caption/Global:Accept", new object[0x0]), Localization.LocalizeString("Ui/Caption/Global:Cancel", new object[0x0])))
            {
                TravelUtil.PlayerMadeTravelRequest = false;
                return false;
            }

            //Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Ui/Caption/HUD/DisasterSaveError:Traveling");
            ths.CancellableByPlayer = false;
            if ((guid != OpportunityNames.Undefined) && (actor.OpportunityManager != null))
            {
                actor.OpportunityManager.CancelOpportunity(guid);
            }

            if (actor.OpportunityManager != null)
            {
                if (actor.OpportunityManager.HasOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler01))
                {
                    actor.OpportunityManager.CancelOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler01);
                }

                if (actor.OpportunityManager.HasOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler02))
                {
                    actor.OpportunityManager.CancelOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler02);
                }

                if (GameUtils.IsFutureWorld() && actor.OpportunityManager.HasOpportunity(OpportunityNames.EP11_RecalibrateDefenseGrid))
                {
                    actor.OpportunityManager.CancelOpportunity(OpportunityNames.EP11_RecalibrateDefenseGrid);
                }
            }

            return true;
        }

        public static bool PreTimeTravel2(InteractionInstance ths)
        {
            Sim actor = ths.InstanceActor as Sim;

            if (GameUtils.IsFutureWorld())
            {
                EventTracker.SendEvent(EventTypeId.kTravelToPresent, actor);
            }
            else
            {
                EventTracker.SendEvent(EventTypeId.kTravelToFuture, actor);
            }

            ths.StandardEntry();
            ths.BeginCommodityUpdates();
            BuffTransformation transformBuff = actor.BuffManager.TransformBuff;
            if (transformBuff != null)
            {
                actor.BuffManager.RemoveElement(transformBuff.Guid);
            }

            return true;
        }

        public static bool PostTimeTravel1(InteractionInstance ths, bool succeeded)
        {
            Sim actor = ths.InstanceActor as Sim;

            if ((succeeded) &&
                (actor.TraitManager.HasElement(TraitNames.Unstable)) &&
                (!actor.BuffManager.HasElement(BuffNames.FeelingOutOfSorts)) &&
                (!actor.BuffManager.HasElement(BuffNames.ImpendingEpisode)) &&
                (!actor.BuffManager.HasElement(BuffNames.Delusional)) &&
                (RandomUtil.RandomChance01(kUnstableTraitChance)))
            {
                actor.BuffManager.AddElement(BuffNames.FeelingOutOfSorts, Origin.FromUnstableTrait);
            }

            return true;
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

                if (Target.IsBroken)
                {
                    return false;
                }

                if (!PreTimeTravel1(this)) return false;

                int num;
                if (!Actor.RouteToSlotListAndCheckInUse(Target, TimePortal.kRoutingSlots, out num))
                {
                    return false;
                }

                if (Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                {
                    return false;
                }

                if (!PreTimeTravel2(this)) return false;

                EnterStateMachine("timeportal", "Enter", "x", "portal");
                AddPersistentScriptEventHandler(0xc9, CameraShakeEvent);

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
                    succeeded = TimePortalEx.TravelToFuture(Target, Actor, new List<Sim>(), new List<ulong>());
                    if (!succeeded)
                    {
                        SpeedTrap.Sleep(0x3c);
                        AnimateSim("Spit Out");
                        Target.SwitchActiveState();
                    }
                }

                if (!PostTimeTravel1(this, succeeded)) return false;

                AnimateSim("Exit");

                EndCommodityUpdates(succeeded);
                StandardExit();
                if (GameUtils.IsFutureWorld())
                {
                    EventTracker.SendEvent(EventTypeId.kTravelToPresent, Actor);
                    Target.StopActiveFX();

                    // Custom
                    GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
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

        public new class Definition : TimePortal.Travel.Definition
        {
            public override string GetInteractionName(Sim actor, TimePortal target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TimePortalTravelEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim actor, TimePortal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.Active)
                {
                    return false;
                }

                if (target.IsBroken)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, TimePortal.sLocalizationKey + ":MustRepairTimePortal", new object[] { actor }));
                    return false;
                }

                return PublicTest(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }

            public static bool PublicTest(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (TravelUtil.PlayerMadeTravelRequest)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("PlayerMadeTravelRequest");
                    return false;
                }

                if ((target.Repairable != null) && target.Repairable.Broken)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Broken");
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

                if (GameUtils.IsFutureWorld() && (actor.Household.Sims.Count != 0x1))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Multiple");
                    return false;
                }

                // Custom
                if (!TravelerSpace.Helpers.TravelUtilEx.CanSimTriggerTravelToFutureWorld(actor, true, ref greyedOutTooltipCallback))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
