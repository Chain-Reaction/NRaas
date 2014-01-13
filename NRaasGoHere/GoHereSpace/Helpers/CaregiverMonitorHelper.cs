using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Interactions;
using NRaas.GoHereSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NRaas.GoHereSpace.Helpers
{
    public class CaregiverMonitorHelper : Common.IPreLoad, Common.IWorldLoadFinished, Common.IPreSave, Common.IPostSave
    {
        static Lot sLot = new Lot();

        public void OnPreLoad()
        {
            Sim.sOnLotChangedDelegates += OnLotChanged;
        }

        public static void OnLotChanged(Sim sim, Lot oldLot, Lot newLot)
        {
            try
            {
                if (sim == null) return;

                if (sim.LotHome == null) return;

                if (sim.LotHome == newLot)
                {
                    Caregivers caregiver;
                    if (GoHere.Settings.mCaregivers.TryGetValue(newLot.LotId, out caregiver))
                    {
                        if ((sim.SimDescription.TeenOrAbove) && (sim.IsHuman))
                        {
                            caregiver.ReturnChildren();
                        }
                    }
                }

                if ((sim.LotHome == oldLot) && (sim.SimDescription.TeenOrAbove) && (sim.IsHuman))
                {
                    TestHomeTask.Perform(sim.Household);
                }
                else if ((sim.LotHome == newLot) && (sim.SimDescription.ToddlerOrBelow))
                {
                    TestHomeTask.Perform(sim.Household);
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }

        public void OnWorldLoadFinished()
        {
            foreach(Household house in Household.sHouseholdList)
            {
                if (house.LotHome == null) continue;

                CaregiverRoutingMonitor monitor = house.GetCaregiverRoutingMonitor(house.LotHome, true);
                if (monitor == null) continue;

                monitor.Lot = sLot;

                monitor.StopMonitoring();
            }
        }

        public void OnPreSave()
        {
            foreach (Household house in Household.sHouseholdList)
            {
                if (house.LotHome == null) continue;

                CaregiverRoutingMonitor monitor = house.GetCaregiverRoutingMonitor(house.LotHome, false);
                if (monitor == null) continue;

                monitor.Lot = house.LotHome;
            }
        }

        public void OnPostSave()
        {
            OnWorldLoadFinished();
        }

        private static List<Sim> GetDayCareChoices()
        {
            List<Sim> actives = new List<Sim>();
            List<Sim> sims = new List<Sim>();

            foreach (Sim sim in LotManager.Actors)
            {
                Daycare career = sim.Occupation as Daycare;
                if (career == null) continue;

                if (sim.LotCurrent != sim.LotHome) continue;

                if (sim.Household == Household.ActiveHousehold)
                {
                    if (!GoHere.Settings.mAllowActiveDayCare) continue;

                    if (career.IsWorkHour(SimClock.CurrentTime()))
                    {
                        actives.Add(sim);
                    }
                }
                else
                {
                    sims.Add(sim);
                }
            }

            if (actives.Count > 0)
            {
                return actives;
            }
            else
            {
                return sims;
            }
        }

        [Persistable]
        public class Caregivers
        {
            ulong mLot;

            Dictionary<ulong, DateAndTime> mOfflotChildren = new Dictionary<ulong, DateAndTime>();

            public Caregivers()
            { }
            public Caregivers(Lot lot)
            {
                mLot = lot.LotId;
            }

            public bool AddChild(Sim sim, Sim watcher)
            {
                DaycareWorkdaySituation situation = DaycareWorkdaySituation.GetDaycareWorkdaySituationForLot(sim.LotCurrent);
                if (situation != null)
                {
                    if (situation.mChildMonitors.ContainsKey(sim.SimDescription.SimDescriptionId)) return true;
                }

                Daycare career = watcher.Occupation as Daycare;

                situation = DaycareWorkdaySituation.GetDaycareWorkdaySituationForLot(watcher.LotHome);
                if (situation == null)
                {
                    if (career != null)
                    {
                        situation = DaycareWorkdaySituation.CreateSituation(career);
                    }
                }

                if (situation == null) return false;

                if (!GoHereEx.Teleport.Perform(sim, watcher.LotHome, false)) return false;

                // Allows toddlers to enter the house if deposited on the front porch
                sim.GreetSimOnLot(watcher.LotHome);

                if (!mOfflotChildren.ContainsKey(sim.SimDescription.SimDescriptionId))
                {
                    mOfflotChildren.Add(sim.SimDescription.SimDescriptionId, SimClock.CurrentTime());
                }

                if (!career.mDaycareChildManagers.ContainsKey(sim.SimDescription.SimDescriptionId))
                {
                    career.mDaycareChildManagers.Add(sim.SimDescription.SimDescriptionId, new DaycareChildManager(sim.SimDescription, career));
                }

                //DaycareToddlerDropoffSituation.CreateSituation(situation.Daycare, sim.SimDescription);

                CASAgeGenderFlags age = sim.SimDescription.Age;
                float ageDays = sim.SimDescription.AgingYearsSinceLastAgeTransition;

                try
                {
                    if (sim.SimDescription.Baby)
                    {
                        sim.SimDescription.Age = CASAgeGenderFlags.Toddler;

                        Relationship relation = Relationship.Get(sim, watcher, true);
                        if (relation != null)
                        {
                            // Required in order to pick up a baby
                            if (relation.CurrentLTRLiking < 30)
                            {
                                relation.LTR.UpdateLiking(30);
                            }
                        }
                    }

                    if (situation.mChildMonitors != null)
                    {
                        situation.mChildMonitors.Remove(sim.SimDescription.SimDescriptionId);
                    }

                    if (Common.kDebugging)
                    {
                        Common.DebugNotify("Added:" + Common.NewLine + sim.FullName + Common.NewLine + watcher.FullName);
                    }

                    situation.AddPerson(sim);
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
                finally
                {
                    if (sim.SimDescription.Age != age)
                    {
                        sim.SimDescription.Age = age;
                        sim.SimDescription.AgingYearsSinceLastAgeTransition = ageDays;
                    }
                }

                return true;
            }

            public void ReturnChildren()
            {
                Lot lot = LotManager.GetLot(mLot);
                if (lot.Household == null) return;

                foreach(KeyValuePair<ulong,DateAndTime> pair in mOfflotChildren)
                {
                    SimDescription simDesc = lot.Household.FindMember(pair.Key);
                    if (simDesc == null) continue;

                    try
                    {
                        DaycareWorkdaySituation situation = null;

                        foreach (Situation sit in Situation.sAllSituations)
                        {
                            situation = sit as DaycareWorkdaySituation;
                            if (situation == null) continue;

                            if (situation.mChildMonitors.ContainsKey(pair.Key))
                            {
                                break;
                            }

                            situation = null;
                        }

                        if (situation != null)
                        {
                            Daycare career = situation.Daycare;
                            if (career != null)
                            {
                                DateAndTime duration = SimClock.CurrentTime() - pair.Value;

                                float hours = SimClock.ConvertFromTicks(duration.Ticks, TimeUnit.Hours);

                                float careerLength = (career.FinishTime - career.StartTime);

                                float ratio = hours / careerLength;

                                string debuggingLog;

                                float xp = 0;

                                DaycareWorkdaySituation priorSituation = career.mDaycareSituation;
                                try
                                {
                                    if (career.mDaycareSituation == null)
                                    {
                                        career.mDaycareSituation = situation;
                                    }

                                    xp = career.GetExperience(pair.Key, Rating.Neutral, out debuggingLog);
                                }
                                finally
                                {
                                    career.mDaycareSituation = priorSituation;
                                }

                                xp *= ratio;

                                career.UpdateXp(xp);

                                int amount = career.GetMoney(pair.Key, Rating.Neutral, out debuggingLog);

                                amount = (int)(amount * ratio);

                                career.PayOwnerSim(amount, GotPaidEvent.PayType.kCareerBonus);

                                if (simDesc.Household != null)
                                {
                                    simDesc.ModifyFunds(-amount);
                                }
                            }

                            if ((situation.mChildMonitors != null) && (situation.mChildMonitors.Count == 1) && (situation.mChildMonitors.ContainsKey(simDesc.SimDescriptionId)))
                            {
                                for (int i = situation.mSimDescIds.Count - 1; i >= 0; i--)
                                {
                                    if (!situation.mChildMonitors.ContainsKey(situation.mSimDescIds[i]))
                                    {
                                        situation.mSimDescIds.RemoveAt(i);
                                    }
                                }
                            }

                            DaycareWorkdaySituation.ScoringRecord oldRecord;
                            if (!situation.ScoringRecords.TryGetValue(simDesc.SimDescriptionId, out oldRecord))
                            {
                                oldRecord = null;
                            }

                            // ScoringRecord will be added by RemovePerson() so remove it now
                            situation.ScoringRecords.Remove(simDesc.SimDescriptionId);

                            situation.RemovePerson(simDesc.CreatedSim);

                            if (oldRecord != null)
                            {
                                DaycareWorkdaySituation.ScoringRecord newRecord;
                                if (situation.ScoringRecords.TryGetValue(simDesc.SimDescriptionId, out newRecord))
                                {
                                    newRecord.Score += oldRecord.Score;
                                    newRecord.ScoreDebugString += Common.NewLine + oldRecord.ScoreDebugString;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(simDesc, e);
                    }

                    Common.DebugNotify("Remove:" + Common.NewLine + simDesc.FullName);

                    if (simDesc.CreatedSim != null)
                    {
                        GoHereEx.Teleport.Perform(simDesc.CreatedSim, simDesc.LotHome, false);
                    }
                }

                mOfflotChildren.Clear();

                GoHere.Settings.mCaregivers.Remove(mLot);
            }
        }

        public class TestHomeTask : Common.FunctionTask
        {
            Household mHouse;

            protected TestHomeTask(Household house)
            {
                mHouse = house;
            }

            public static void Perform(Household house)
            {
                new TestHomeTask(house).AddToSimulator();
            }

            protected override void OnPerform()
            {
                Dictionary<Sim, Vector3> lastOnLotPositions = new Dictionary<Sim, Vector3>();

                CaregiverRoutingMonitor monitor = new CaregiverRoutingMonitor(mHouse, mHouse.LotHome);

                bool stopMonitoring;
                Sim arbitraryChild;
                int caregiverCount;
                if (mHouse.LotHome == null)
                {
                    return;
                }
                else if (CaregiverRoutingMonitor.EnoughCaregiversRemain(mHouse, mHouse.LotHome, lastOnLotPositions, true, out stopMonitoring, out caregiverCount, out arbitraryChild))
                {
                    return;
                }
                else
                {
                    foreach (IBonehildaCoffin coffin in mHouse.LotHome.GetObjects<IBonehildaCoffin>())
                    {
                        if (coffin.isActiveAndNoBonehilda())
                        {
                            coffin.ForceSpawn();
                            return;
                        }
                    }

                    //Common.DebugNotify(delegate { return mHouse.Name + " " + caregiverCount; });

                    bool flag2 = CaregiverRoutingMonitor.IsLotHomeOrDaycareForHousehold(mHouse, mHouse.LotHome);
                    Sim closestSim = null;

                    List<Sim> list = new List<Sim>(mHouse.Sims);
                    DaycareWorkdaySituation daycareWorkdaySituationForLot = DaycareWorkdaySituation.GetDaycareWorkdaySituationForLot(mHouse.LotHome);
                    if ((daycareWorkdaySituationForLot != null) && daycareWorkdaySituationForLot.IsServingHousehold(mHouse))
                    {
                        list.AddRange(daycareWorkdaySituationForLot.Daycare.OwnerDescription.Household.Sims);
                    }

                    float minDistance = float.MaxValue;

                    foreach (Sim sim3 in list)
                    {
                        if (sim3.SimDescription.TeenOrAbove)
                        {
                            Vector3 vector;
                            if (monitor.IsReturningToLotInQuestion(sim3, arbitraryChild))
                            {
                                return;
                            }

                            if (sim3.IsRouting && lastOnLotPositions.TryGetValue(sim3, out vector))
                            {
                                float distance = (sim3.Position - vector).LengthSqr();
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    closestSim = sim3;
                                }
                            }
                        }
                    }

                    if (closestSim == null)
                    {
                        foreach (Sim sim4 in list)
                        {
                            if (sim4.SimDescription.TeenOrAbove)
                            {
                                float distance = (sim4.Position - arbitraryChild.Position).LengthSqr();
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    closestSim = sim4;
                                }
                            }
                        }
                    }

                    bool flag3;
                    bool flag4;
                    bool flag5;
                    if (closestSim != null)
                    {
                        flag3 = false;
                        flag4 = false;
                        InteractionInstance headInteraction = closestSim.InteractionQueue.GetHeadInteraction();
                        if (headInteraction != null)
                        {
                            flag4 = !headInteraction.CancellableByPlayer;
                        }

                        flag5 = true;
                        foreach (Sim sim5 in mHouse.Sims)
                        {
                            if ((sim5.LotCurrent == mHouse.LotHome) && (sim5.SimDescription.ToddlerOrBelow) && (sim5.CurrentInteraction is AgeUp))
                            {
                                flag5 = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }

                    if (flag5)
                    {
                        if ((!GoHere.Settings.mInactiveChildrenAsActive) && (closestSim.IsNPC || CaregiverRoutingMonitor.TreatPlayerSimsLikeNPCs))
                        {
                            if ((caregiverCount == 0x0) && (!GoHere.Settings.mAllowChildHomeAlone))
                            {
                                Common.FunctionTask.Perform(monitor.DematerializeChildren);
                            }

                            return;
                        }
                        else if (!GoHere.Settings.mAllowChildHomeAlone)
                        {
                            if ((mHouse != Household.ActiveHousehold) || mHouse.AutoBabysitter || flag4)
                            {
                                flag3 = true;

                                if (mHouse == Household.ActiveHousehold)
                                {
                                    string str;
                                    if (flag2)
                                    {
                                        str = ChildUtils.Localize(arbitraryChild.SimDescription.IsFemale, "AutoNannyTns", new object[] { arbitraryChild });
                                    }
                                    else
                                    {
                                        str = ChildUtils.Localize(arbitraryChild.SimDescription.IsFemale, "AutoNannyAwayFromHomeTns", new object[] { arbitraryChild });
                                    }

                                    StyledNotification.Format format = new StyledNotification.Format(str, closestSim.ObjectId, arbitraryChild.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                                    StyledNotification.Show(format);
                                }
                            }
                            else
                            {
                                string str2;
                                if (flag2)
                                {
                                    str2 = ChildUtils.Localize(arbitraryChild.SimDescription.IsFemale, "NeedNannyDialogMessage", new object[] { arbitraryChild });
                                }
                                else
                                {
                                    str2 = ChildUtils.Localize(arbitraryChild.SimDescription.IsFemale, "NeedNannyAwayFromHomeDialogMessage", new object[] { arbitraryChild });
                                }

                                flag3 = TwoButtonDialog.Show(str2, ChildUtils.Localize(arbitraryChild.SimDescription.IsFemale, "NeedNannyDialogYes", new object[0x0]), ChildUtils.Localize(arbitraryChild.SimDescription.IsFemale, "NeedNannyDialogNo", new object[0x0]));
                            }
                        }
                        else
                        {
                            flag3 = true;
                            flag5 = true;
                            flag2 = true;
                        }
                    }

                    if (!flag5 || !flag3)
                    {
                        Vector3 position;
                        if (!lastOnLotPositions.TryGetValue(closestSim, out position))
                        {
                            position = Vector3.Invalid;
                        }
                        monitor.PushGoBackIfNeeded(closestSim, position);
                    }
                    else
                    {
                        if (!flag2)
                        {
                            foreach (Sim sim6 in mHouse.LotHome.GetSims())
                            {
                                if (sim6.SimDescription.TeenOrAbove && !sim6.IsRouting)
                                {
                                    flag3 = false;
                                    break;
                                }
                            }
                        }

                        if ((GoHere.Settings.mInactiveChildrenAsActive) || ((flag3 && !closestSim.IsNPC) && !CaregiverRoutingMonitor.TreatPlayerSimsLikeNPCs))
                        {
                            InitiateCaregiving(mHouse);
                        }
                    }
                }
            }
        }

        public static void InitiateCaregiving(Household house)
        {
            bool babySitter = true;

            if (GoHere.Settings.mUseDayCareSims)
            {
                List<Sim> choices = GetDayCareChoices();

                if (choices.Count > 0)
                {
                    Sim choice = RandomUtil.GetRandomObjectFromList(choices);

                    Caregivers caregiver;
                    if (!GoHere.Settings.mCaregivers.TryGetValue(house.LotHome.LotId, out caregiver))
                    {
                        caregiver = new Caregivers(house.LotHome);
                        GoHere.Settings.mCaregivers.Add(house.LotHome.LotId, caregiver);
                    }

                    bool fail = false;
                    foreach (Sim child in house.Sims)
                    {
                        if ((child.SimDescription.ToddlerOrBelow) && (child.LotCurrent == child.LotHome))
                        {
                            if (!caregiver.AddChild(child, choice))
                            {
                                fail = true;
                            }
                        }
                    }

                    if (!fail)
                    {
                        babySitter = false;
                    }
                }
            }

            if (babySitter)
            {
                Babysitter instance = Babysitter.Instance;
                if (instance != null)
                {
                    instance.MakeServiceRequest(house.LotHome, true, ObjectGuid.InvalidObjectGuid, true);
                }
            }
        }
    }
}
