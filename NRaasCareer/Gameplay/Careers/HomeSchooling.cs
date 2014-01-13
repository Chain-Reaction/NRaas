using NRaas.CommonSpace.Helpers;
using NRaas.CareerSpace;
using NRaas.CareerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.Careers
{
    // - Download homework off computer each day
    // - Do homework for performance gain
    [Persistable]
    public class HomeSchooling : School
    {
        static AlarmHandle sLocalFieldTripAlarm = AlarmHandle.kInvalidHandle;

        int mLastHomeworkDay = 0;

        public bool mMaxPerformance = false;

        [Persistable(false)]
        private AlarmHandle mHomeworkCheck = AlarmHandle.kInvalidHandle;

        public HomeSchooling()
        { }
        public HomeSchooling(XmlDbRow myRow, XmlDbTable levelTable, XmlDbTable eventDataTable)
            : base(myRow, levelTable, eventDataTable)
        { }

        protected static void Notify(Sim sim, string msg)
        {
            if (sim == null) return;

            if (SimTypes.IsSelectable(sim))
            {
                StyledNotification.Show(new StyledNotification.Format(msg, sim.ObjectId, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage));
            }
        }

        public static void OnBootHomework ()
        {
            try
            {
                foreach (SimDescription sim in Household.EveryHumanSimDescription())
                {
                    if (sim.CareerManager == null) continue;

                    HomeSchooling school = sim.CareerManager.School as HomeSchooling;
                    if (school == null) continue;

                    school.StartHomeworkAlarm();
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception("OnBootHomework", exception);
            }
        }

        public override bool CareerAgeTest(SimDescription sim)
        {
            return ((sim.Child) || (sim.Teen));
        }

        public override void OnDispose()
        {
            if (mHomeworkCheck != AlarmHandle.kInvalidHandle)
            {
                AlarmManager.Global.RemoveAlarm (mHomeworkCheck);
                mHomeworkCheck = AlarmHandle.kInvalidHandle;
            }

 	        base.OnDispose();
        }

        protected void StartHomeworkAlarm()
        {
            try
            {
                if ((OwnerDescription.TeenOrBelow) && (OwnerDescription.CareerManager.School is HomeSchooling))
                {
                    if (mHomeworkCheck != AlarmHandle.kInvalidHandle)
                    {
                        AlarmManager.Global.RemoveAlarm(mHomeworkCheck);
                        mHomeworkCheck = AlarmHandle.kInvalidHandle;
                    }

                    float hoursPassedOfDay = SimClock.HoursPassedOfDay;
                    float time = CurLevel.StartTime - hoursPassedOfDay;
                    if (time < 0f)
                    {
                        time += 24f;
                    }

                    mHomeworkCheck = AlarmManager.Global.AddAlarm(time, TimeUnit.Hours, OnHomeworkCheck, "NRaasHomeworkCheck", AlarmType.NeverPersisted, OwnerDescription);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(OwnerDescription, exception);
            }
        }

        protected void OnHomeworkCheck()
        {
            try
            {
                if (!(OwnerDescription.CareerManager.School is HomeSchooling)) return;

                if (SimTypes.IsSelectable(OwnerDescription))
                {
                    if (GameUtils.IsOnVacation()) return;
                }

                bool schoolDay = ((SimClock.CurrentDayOfWeek != DaysOfTheWeek.Saturday) && (SimClock.CurrentDayOfWeek != DaysOfTheWeek.Sunday));

                if (schoolDay)
                {
                    // Handles the Field Trip situations
                    RegularWorkDayStartAlarmHandler();

                    /*
                    if ((sFieldTripSimDescription != null) && (sFieldTripSimDescription.Count > 0x0) && (sLocalFieldTripAlarm == AlarmHandle.kInvalidHandle))
                    {
                        AlarmManager.Global.RemoveAlarm(sFieldTripAlarm);
                        sLocalFieldTripAlarm = AlarmManager.Global.AddAlarm(kFieldTripDelayMinutes, TimeUnit.Minutes, BeginFieldTripEx, "Field Trip Alarm", AlarmType.DeleteOnReset, null);

                        sFieldTripAlarm = sLocalFieldTripAlarm;
                    }
                    */

                    // Don't call this function, it only works properly when run from an interaction
                    //StartWorking();
                }

                if (mMaxPerformance)
                {
                    mPerformance = 100;
                }
                else if ((mLastHomeworkDay + 1) < SimClock.ElapsedCalendarDays())
                {
                    if (schoolDay)
                    {
                        if (SimTypes.IsSelectable(OwnerDescription))
                        {
                            AddPerformance(-NRaas.Careers.Settings.mPerformancePerHomework);

                            if (OwnerDescription.CareerManager != null)
                            {
                                OwnerDescription.CareerManager.UpdatePerformanceUI(this);
                            }

                            Notify(OwnerDescription.CreatedSim, Common.Localize("HomeSchooling:MissHomework", OwnerDescription.IsFemale, new object[] { OwnerDescription }));

                            mLastHomeworkDay = SimClock.ElapsedCalendarDays();
                        }
                        else
                        {
                            HandleHomework(OwnerDescription.CreatedSim, false);
                        }
                    }
                }

                if (schoolDay)
                {
                    // Disables the field trip check
                    sHasAskedPermissionForFieldTrip = true;

                    FinishWorking();
                }

                StartHomeworkAlarm();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(OwnerDescription, exception);
            }
        }

        /*
        private static void BeginFieldTripEx()
        {
            try
            {
                sFieldTripAlarm = AlarmHandle.kInvalidHandle;

                sLocalFieldTripAlarm = AlarmHandle.kInvalidHandle;

                Lot schoolLot = sFieldTripSchool.CareerLoc.Owner.RabbitHoleProxy.LotCurrent;

                List<SimDescription> list = new List<SimDescription>();
                foreach (SimDescription description in sFieldTripSimDescription)
                {
                    Sim createdSim = description.CreatedSim;
                    if ((createdSim == null) || (createdSim.CareerManager.School == null))
                    {
                        list.Add(description);
                    }
                    else
                    {
                        GoToLot interaction = GoToLot.Singleton.CreateInstance(schoolLot, createdSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as GoToLot;
                        interaction.SetTakingSimToWork();

                        createdSim.InteractionQueue.CancelAllInteractions();
                        createdSim.InteractionQueue.AddNext(interaction);
                    }
                }

                foreach (SimDescription description2 in list)
                {
                    sFieldTripSimDescription.Remove(description2);
                }

                FieldTripSituation.Create(sFieldTripSimDescription, sAge, sFieldTripSchool, sFieldTripRabbitHole);
                ResetFieldTripStatics();
            }
            catch (Exception exception)
            {
                Common.Exception("BeginFieldTripEx", exception);
            }
        }
        */
        public override School.Tuning SchoolTuning
        {
            get 
            {
                School staticSchool = null;

                if ((OwnerDescription == null) || (OwnerDescription.Child))
                {
                    staticSchool = CareerManager.GetStaticCareer(OccupationNames.SchoolElementary) as School;
                }
                else
                {
                    staticSchool = CareerManager.GetStaticCareer(OccupationNames.SchoolHigh) as School;
                }

                if (staticSchool == null) return null;

                return staticSchool.SchoolTuning;
            }
        }

        protected void HandleHomework (Sim sim, bool fromRabbithole)
        {
            if (sim == null) return;

            if (sim.School == null) return;

            if ((SimTypes.IsSelectable(sim)) && 
                (sim.School.OwnersHomework != null) &&
                (sim.School.OwnersHomework.PercentComplete == 0f))
            {
                Notify(sim, Common.Localize("HomeSchooling:HaveHomework", sim.IsFemale, new object[] { sim }));
            }
            else
            {
                bool bExistingHomework = false;

                if (sim.School.OwnersHomework != null)
                {
                    float perfGain = NRaas.Careers.Settings.mPerformancePerHomework * ((sim.School.OwnersHomework.PercentComplete - 50f) / 100f);

                    sim.School.AddPerformance (perfGain);

                    if (sim.SimDescription.CareerManager != null)
                    {
                        sim.SimDescription.CareerManager.UpdatePerformanceUI(this);
                    }

                    sim.School.OwnersHomework.Dispose ();

                    sim.School.OwnersHomework = null;

                    bExistingHomework = true;
                }

                if (sim.School.OwnersHomework == null)
                {
                    sim.School.OwnersHomework = GlobalFunctions.CreateObjectOutOfWorld("Homework") as Homework;
                    sim.School.OwnersHomework.OwningSimDescription = sim.SimDescription;

                    sim.Inventory.TryToAdd(sim.School.OwnersHomework, false);
                }

                sim.School.OwnersHomework.PercentComplete = 0f;
                sim.School.OwnersHomework.Cheated = false;

                mLastHomeworkDay = SimClock.ElapsedCalendarDays();

                string suffix = null;
                if (fromRabbithole)
                {
                    suffix = "Rabbithole";
                }

                if (bExistingHomework)
                {
                    Notify(sim, Common.Localize("HomeSchooling:ExistingHomework" + suffix, sim.IsFemale, new object[] { sim }));
                }
                else
                {
                    Notify(sim, Common.Localize("HomeSchooling:NewHomework" + suffix, sim.IsFemale, new object[] { sim }));
                }

                if (SchoolTuning != null)
                {
                    if (sim.School.Performance >= SchoolTuning.GradeThresholdA)
                    {
                        sim.School.mConsecutiveDaysWithA++;
                        if (sim.School.mConsecutiveDaysWithA >= SchoolTuning.DaysWithAForHonorRoll)
                        {
                            sim.BuffManager.AddElement(BuffNames.HonorStudent, Origin.FromGoodGrades);
                        }
                    }
                    else
                    {
                        ActiveTopic.AddToSim(sim, "Complain About School");
                        sim.School.mConsecutiveDaysWithA = 0;
                        if (sim.School.Performance <= SchoolTuning.GradeThresholdF)
                        {
                            sim.BuffManager.AddElement(BuffNames.Failing, Origin.FromBadGrades);
                        }
                    }
                }

                EventTracker.SendEvent(EventTypeId.kFinishedSchool, sim);
            }
        }

        public class DownloadHomework : Computer.ComputerInteraction, Common.IAddInteraction
        {
            public static readonly InteractionDefinition Singleton = new Definition();

            public void AddInteraction(Common.InteractionInjectorList interactions)
            {
                interactions.Add<Computer>(Singleton);
            }

            public override bool Run()
            {
                try
                {
                    StandardEntry();
                    if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                    {
                        StandardExit();
                        return false;
                    }

                    AnimateSim("GenericTyping");

                    HomeSchooling school = Actor.School as HomeSchooling;
                    if (school != null)
                    {
                        school.HandleHomework(Actor, false);
                        school.StartHomeworkAlarm();
                    }

                    Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                    StandardExit();
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(Actor, Target, exception);
                }
                return true;
            }

            private sealed class Definition : InteractionDefinition<Sim, Computer, DownloadHomework>
            {
                public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
                {
                    return Common.Localize("HomeSchooling:InteractionMenuName", actor.IsFemale);
                }

                public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    HomeSchooling school = a.School as HomeSchooling;
                    if (school == null) return false;

                    if (!target.IsComputerUsable(a, true, false, isAutonomous)) return false;

                    if (isAutonomous)
                    {
                        if ((school.OwnersHomework != null) &&
                            (school.OwnersHomework.PercentComplete == 100f))
                        {
                            if (school.mLastHomeworkDay < SimClock.ElapsedCalendarDays())
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public class RabbitholeHomework : RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>, Common.IAddInteraction
        {
            public static readonly InteractionDefinition Singleton = new Definition();

            public void AddInteraction(Common.InteractionInjectorList interactions)
            {
                interactions.Add<CityHall>(Singleton);
                interactions.Add<SchoolRabbitHole>(Singleton);
            }

            public override void ConfigureInteraction()
            {
                base.ConfigureInteraction();

                TimedStage stage = new TimedStage(GetInteractionName(), 30, false, true, true);
                Stages = new List<Stage>(new Stage[] { stage });

                ActiveStage = stage;
            }

            private void LearnLoop(StateMachineClient smc, InteractionInstance.LoopData data)
            {
                try
                {
                    if (data.mLifeTime > 30)
                    {
                        Actor.AddExitReason(ExitReason.Finished);
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(Actor, Target, exception);
                }
            }

            public override bool InRabbitHole()
            {
                try
                {
                    BeginCommodityUpdates();
                    bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), LearnLoop, mCurrentStateMachine);
                    EndCommodityUpdates(succeeded);

                    HomeSchooling school = Actor.School as HomeSchooling;
                    if (school != null)
                    {
                        school.HandleHomework(Actor, true);
                        school.StartHomeworkAlarm();
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(Actor, Target, exception);
                }
                return true;
            }

            private class Definition : InteractionDefinition<Sim, RabbitHole, RabbitholeHomework>
            {
                public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
                {
                    return Common.Localize("HomeSchooling:RabbitholeMenuName", actor.IsFemale);
                }

                public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    HomeSchooling school = a.School as HomeSchooling;
                    if (school == null) return false;

                    if (isAutonomous)
                    {
                        if ((school.OwnersHomework != null) &&
                            (school.OwnersHomework.PercentComplete == 100f))
                        {
                            if (school.mLastHomeworkDay < SimClock.ElapsedCalendarDays())
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
    }
}
