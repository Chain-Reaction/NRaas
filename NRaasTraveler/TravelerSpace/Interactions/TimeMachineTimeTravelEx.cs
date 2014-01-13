using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Interactions
{
    public class TimeMachineTimeTravelEx : TimeMachine.TimeTravel, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<TimeMachine, TimeMachine.TimeTravel.Definition, Definition>(true);
            if (tuning != null)
            {
                tuning.SetFlags(InteractionTuning.FlagField.DisallowAutonomous, true);
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<TimeMachine>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                string str;
                mTimeTravelDef = InteractionDefinition as Definition;
                if (!Target.RouteToMachine(Actor, false, null))
                {
                    return false;
                }

                if (!TimePortalTravelEx.PreTimeTravel1(this)) return false;

                if (!TimePortalTravelEx.PreTimeTravel2(this)) return false;

                Actor.SimDescription.Contactable = false;
                EnterStateMachine("TimeMachine", "Enter", "x");
                SetActor("timeMachine", Target);
                SetParameter("isFuture", mTimeTravelDef.TimePeriod == TimeMachine.TravelTimePeriod.Future);
                AddOneShotScriptEventHandler(0x3ee, OnEnterAnimationEvent);
                AddOneShotScriptEventHandler(0x66, ToggleHiddenAnimationEvent);
                AnimateSim("GetIn");
                Target.EnableRoutingFootprint(Actor);

                mTimeTravelAlarm = AlarmManager.Global.AddAlarmRepeating(RandomUtil.GetFloat(TimeMachine.kMinutesBetweenAdventureTNSMin, TimeMachine.kMinutesBetweenAdventureTNSMax), TimeUnit.Minutes, new AlarmTimerCallback(TimeTravelCallback), RandomUtil.GetFloat(TimeMachine.kMinutesBetweenAdventureTNSMin, TimeMachine.kMinutesBetweenAdventureTNSMax), TimeUnit.Minutes, "Time Travel Alarm For:" + Actor.SimDescription.FullName, AlarmType.AlwaysPersisted, Actor);
                Target.SetMaterial("InUse");

                bool succeeded = true;
                if (!GameUtils.IsFutureWorld())
                {
                    // Custom
                    succeeded = TimePortalEx.TravelToFuture(null, Actor, new List<Sim>(), new List<ulong>());
                }

                succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));

                EndCommodityUpdates(succeeded);
                Target.PickExitStateAndSound(mTimeTravelDef.TimePeriod, out str, out mExitSound);
                AddOneShotScriptEventHandler(0x3e9, OnExitAnimationEvent);
                AddOneShotScriptEventHandler(0x3ef, OnExitAnimationEvent);
                AddOneShotScriptEventHandler(0x67, ToggleHiddenAnimationEvent);
                AnimateSim(str);

                if (!TimePortalTravelEx.PostTimeTravel1(this, succeeded)) return false;

                Target.SetMaterial("default");
                AnimateSim("Exit");
                if (!string.IsNullOrEmpty(mTravelSummary))
                {
                    Actor.ShowTNSIfSelectable(mTravelSummary, StyledNotification.NotificationStyle.kGameMessagePositive, Target.ObjectId, Actor.ObjectId);
                }

                StandardExit();
                Target.Repairable.UpdateBreakage(Actor);
                InventingSkill skill = Actor.SkillManager.GetSkill<InventingSkill>(SkillNames.Inventing);
                if (succeeded && (skill != null))
                {
                    skill.RegisterTimeTravelDone();
                }

                if (GameUtils.IsFutureWorld())
                {
                    EventTracker.SendEvent(EventTypeId.kTravelToPresent, Actor);

                    // Custom
                    GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
                }

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
            finally
            {
                TravelUtil.PlayerMadeTravelRequest = false;
            }
        }

        public new class Definition : TimeMachine.TimeTravel.Definition
        {
            public Definition()
                : base(null, new string[0], GameUtils.IsFutureWorld() ? TimeMachine.TravelTimePeriod.Past : TimeMachine.TravelTimePeriod.Future)
            { }

            public override string GetInteractionName(Sim actor, TimeMachine target, InteractionObjectPair iop)
            {
                if (GameUtils.IsFutureWorld())
                {
                    return TimePortal.LocalizeString("TravelHome", new object[0x0]);
                }
                return TimePortal.LocalizeString("Travel", new object[0x0]);
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TimeMachineTimeTravelEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, TimeMachine target, List<InteractionObjectPair> results)
            {
                // Override base class
                if (iop.CheckIfInteractionValid())
                {
                    results.Add(iop);
                }
            }

            public override bool Test(Sim actor, TimeMachine target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP11))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Pack Fail");
                    return false;
                }

                if (actor.BuffManager.HasElement(BuffNames.TimeTraveled))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(TimeMachine.LocalizeString("TimeTravelGreyedOutTooltip", new object[] { actor }));
                    return false;
                }

                return TimePortalTravelEx.Definition.PublicTest(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
