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
    public class TimeMachineTimeTravelWithEx : TimeMachine.TimeTravel, ITravelWith, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        private List<Sim> mFollowerGroup = new List<Sim>();
        private List<ulong> mFollowerGroupIDs = new List<ulong>();

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

        public bool AddFollower(Sim sim)
        {
            if (sim == null)
            {
                return false;
            }
            mFollowerGroupIDs.Add(sim.SimDescription.SimDescriptionId);
            mFollowerGroup.Add(sim);
            return true;
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

                List<Sim> lazyList = new List<Sim>();
                if (!TimePortalTravelWithEx.PreTimeTravel1(this, this, lazyList)) return false;

                if (!TimePortalTravelWithEx.PreTimeTravel2(this, lazyList)) return false;

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
                    succeeded = TimePortalEx.TravelToFuture(null, Actor, new List<Sim>(mFollowerGroup), new List<ulong>(mFollowerGroupIDs));
                }

                if (succeeded)
                {
                    foreach (Sim sim7 in lazyList)
                    {
                        Skill futureSkill = sim7.SkillManager.GetElement(SkillNames.Future);
                        if (futureSkill != null)
                        {
                            futureSkill.AddPoints(TimePortalTravelWithEx.kFollowerFutureSkillPointGain, true, false);
                        }
                    }
                }

                succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));

                EndCommodityUpdates(succeeded);
                Target.PickExitStateAndSound(mTimeTravelDef.TimePeriod, out str, out mExitSound);
                AddOneShotScriptEventHandler(0x3e9, OnExitAnimationEvent);
                AddOneShotScriptEventHandler(0x3ef, OnExitAnimationEvent);
                AddOneShotScriptEventHandler(0x67, ToggleHiddenAnimationEvent);
                AnimateSim(str);

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
                    // Custom
                    GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();

                    TimePortalTravelWithEx.SendEventForActorAndFollowers(Actor, lazyList, EventTypeId.kTravelToPresent);
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
                return (TimePortal.LocalizeString("TravelWith", new object[0x0]) + Localization.Ellipsis);
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TimeMachineTimeTravelWithEx();
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

                return TimePortalTravelWithEx.Definition.PublicTest(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
