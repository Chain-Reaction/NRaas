using NRaas.CommonSpace.Stores;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class DreamCatcher
    {
        static Dictionary<ulong, DreamStore> sDreamStore = null;

        public static void StoreAllDreams()
        {
            sDreamStore = new Dictionary<ulong, DreamStore>();

            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                if (sim.Household != Household.ActiveHousehold)
                {
                    try
                    {
                        DreamStore store = new DreamStore(sim, true, true);
                        if (!store.IsNeeded) continue;

                        sDreamStore[sim.SimDescription.SimDescriptionId] = store;
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
        }

        public static void PruneDreamManager(Sim sim)
        {
            if (sim == null) return;

            DreamsAndPromisesManager dnp = sim.DreamsAndPromisesManager;
            if (dnp == null) return;

            List<ActiveNodeBase> activeNodes = dnp.mActiveNodes;
            if (activeNodes != null)
            {
                for (int i = activeNodes.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        activeNodes[i].CreateExportFromNode();
                    }
                    catch (Exception e)
                    {
                        activeNodes.RemoveAt(i);

                        Common.DebugException("ActiveNode", e);
                    }
                }
            }

            List<ActiveDreamNode> sleepingNodes = dnp.mSleepingNodes;
            if (sleepingNodes != null)
            {
                for (int i = sleepingNodes.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        sleepingNodes[i].CreateExportFromNode();
                    }
                    catch (Exception e)
                    {
                        sleepingNodes.RemoveAt(i);

                        Common.DebugException("SleepingNode", e);
                    }
                }
            }
        }

        public static void RestoreDreams(Household house)
        {
            if (sDreamStore == null) return;

            foreach (Sim sim in house.AllActors)
            {
                DreamStore store;
                if (sDreamStore.TryGetValue(sim.SimDescription.SimDescriptionId, out store))
                {
                    store.Restore(sim);

                    sDreamStore.Remove(sim.SimDescription.SimDescriptionId);
                }
            }
        }

        public static void RestoreAllDreams()
        {
            if (sDreamStore == null) return;

            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                try
                {
                    DreamStore store;
                    if (sDreamStore.TryGetValue(sim.SimDescription.SimDescriptionId, out store))
                    {
                        store.Restore(sim);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }

            sDreamStore = null;
        }

        public static bool Select(Sim sim, bool allowHouseholdSwitch, bool catchDreams)
        {
            return Select(sim, allowHouseholdSwitch, catchDreams, false);
        }
        public static bool Select(Sim sim, bool allowHouseholdSwitch, bool catchDreams, bool forceFull)
        {
            if (sim == null) return false;

            if (sim.SimDescription == null) return false;

            if (sim.HasBeenDestroyed) return false;

            if ((sim.SimDescription.IsNeverSelectable) || (sim.LotHome == null)) return false;

            return SelectNoLotCheck(sim, allowHouseholdSwitch, catchDreams, forceFull);
        }
        public static bool SelectNoLotCheck(Sim sim, bool allowHouseholdSwitch, bool catchDreams)
        {
            return SelectNoLotCheck(sim, allowHouseholdSwitch, catchDreams, false);
        }
        public static bool SelectNoLotCheck(Sim sim, bool allowHouseholdSwitch, bool catchDreams, bool forceFull)
        {
            if (sim.Household == Household.ActiveHousehold)
            {
                Task.PerformImmediate(sim, allowHouseholdSwitch, catchDreams, forceFull);
            }
            else
            {
                Task.Perform(sim, allowHouseholdSwitch, catchDreams);
            }
            return true;
        }

        public static bool SelectNoLotCheckImmediate(Sim sim, bool allowHouseholdSwitch, bool catchDreams)
        {
            return SelectNoLotCheckImmediate(sim, allowHouseholdSwitch, catchDreams, false);
        }
        public static bool SelectNoLotCheckImmediate(Sim sim, bool allowHouseholdSwitch, bool catchDreams, bool forceFull)
        {
            Task.PerformImmediate(sim, allowHouseholdSwitch, catchDreams, forceFull);
            return true;
        }

        public delegate void Logger(string text);

        public static void OnWorldLoadFinishedDreams()
        {
            StoreAllDreams();

            EventTracker.AddListener(EventTypeId.kEventSimSelected, OnSelected); // Must be immediate

            new Common.AlarmTask(1f, TimeUnit.Seconds, RestoreAllDreams);
        }

        public static ListenerAction OnSelected(Event e)
        {
            try
            {
                Common.FunctionTask.Perform(RestoreAllDreams);
                return ListenerAction.Remove;
            }
            catch (Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
                return ListenerAction.Keep;
            }
        }

        public static void AdjustSelectable(SimDescription sim, bool active, bool dreamCatcher)
        {
            try
            {
                SafeStore.Flag flags = SafeStore.Flag.StoreOpportunities;
                if (active)
                {
                    flags |= SafeStore.Flag.Selectable;
                }
                else
                {
                    flags |= SafeStore.Flag.Unselectable;
                }

                using (SafeStore store = new SafeStore(sim, flags))
                {
                    if (active)
                    {
                        if (!Household.RoommateManager.IsNPCRoommate(sim))
                        {
                            sim.CreatedSim.OnBecameSelectable();
                        }
                    }
                    else
                    {
                        DreamStore dreamStore = null;

                        if (dreamCatcher)
                        {
                            dreamStore = new DreamStore(sim.CreatedSim, false, true);
                        }

                        try
                        {
                            sim.CreatedSim.OnBecameUnselectable();
                        }
                        finally
                        {
                            if (dreamStore != null)
                            {
                                dreamStore.Restore(sim.CreatedSim);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }

        public class HouseholdStore : IDisposable
        {
            Household mOldHouse;
            
            List<SimDescription> mNewSims;

            Dictionary<Sim, DreamStore> mDreamStore = new Dictionary<Sim, DreamStore>();

            Dictionary<SimDescription, SafeStore> mSafeStore = new Dictionary<SimDescription, SafeStore>();

            public HouseholdStore(Household newHouse, bool catchDreams)
                : this(Households.All(newHouse), catchDreams)
            {}
            public HouseholdStore(IEnumerable<SimDescription> newSims, bool catchDreams)
            {
                if (newSims != null)
                {
                    mNewSims = new List<SimDescription>(newSims);
                }

                mOldHouse = PlumbBob.sCurrentNonNullHousehold;

                if ((mOldHouse != null) && (catchDreams))
                {
                    foreach (Sim member in Households.AllSims(mOldHouse))
                    {
                        DreamStore element = new DreamStore(member, false, true);

                        mDreamStore.Add(member, element);
                    }
                }

                if (mNewSims != null)
                {
                    foreach (SimDescription member in mNewSims)
                    {
                        Corrections.CleanupBrokenSkills(member, null);

                        mSafeStore.Add(member, new SafeStore(member, SafeStore.Flag.Selectable | SafeStore.Flag.Unselectable | SafeStore.Flag.StoreOpportunities));

                        if ((member.CreatedSim != null) &&
                            (member.CreatedSim.Autonomy != null))
                        {
                            Motives motives = member.CreatedSim.Autonomy.Motives;
                            if ((motives == null) || (motives.GetMotive(CommodityKind.Hunger) == null))
                            {
                                member.CreatedSim.Autonomy.RecreateAllMotives();
                            }
                        }
                    }
                }
            }

            public void Dispose()
            {
                if (mNewSims != null)
                {
                    foreach (SimDescription member in mNewSims)
                    {
                        SafeStore element = mSafeStore[member];

                        element.Dispose();
                    }
                }

                if (mOldHouse != null)
                {
                    foreach (Sim member in Households.AllSims(mOldHouse))
                    {
                        if (!mDreamStore.ContainsKey(member)) continue;

                        mDreamStore[member].Restore(member);
                    }
                }
            }
        }

        public class Task : Common.FunctionTask
        {
            Sim mSim;

            bool mAllowHouseholdSwitch;
            bool mCatchDreams;

            protected Task(Sim sim, bool allowHouseholdSwitch, bool catchDreams)
            {
                mSim = sim;
                mAllowHouseholdSwitch = allowHouseholdSwitch;
                mCatchDreams = catchDreams;
            }

            public static void Perform(Sim sim, bool allowHouseholdSwitch, bool catchDreams)
            {
                new Task(sim, allowHouseholdSwitch, catchDreams).AddToSimulator();
            }

            public static void PrepareToBecomeActiveHousehold(Household ths)
            {
                foreach (Sim.Placeholder placeholder in Sims3.Gameplay.Queries.GetObjects<Sim.Placeholder>())
                {
                    if (((placeholder.SimDescription != null) && (placeholder.SimDescription.Household == ths)) && !(placeholder is CaregiverRoutingMonitor.ChildPlaceholder))
                    {
                        foreach (Situation situation in new List<Situation>(Situation.sAllSituations))
                        {
                            AgeUpNpcSituation situation2 = situation as AgeUpNpcSituation;
                            if ((situation2 != null) && (situation2.SimDescription == placeholder.SimDescription))
                            {
                                situation2.Exit();
                            }
                        }
                    }
                }

                if (ths.AllActors.Count != ths.AllSimDescriptions.Count)
                {
                    Vector3 position = ths.LotHome.EntryPoint();
                    foreach (SimDescription description in ths.AllSimDescriptions)
                    {
                        try
                        {
                            if ((description.CreatedSim == null) && description.IsEnrolledInBoardingSchool())
                            {
                                continue;
                            }

                            bool flag = true;
                            foreach (Sim sim in ths.AllActors)
                            {
                                if (sim.SimDescription == description)
                                {
                                    flag = false;
                                    break;
                                }
                            }

                            if (flag)
                            {
                                Sim sim = Instantiation.Perform(description, position, null, null);
                                if (sim != null)
                                {
                                    sim.SetObjectToReset();
                                }
                                SpeedTrap.Sleep();
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(description, e);
                        }
                    }
                }

                if (ths.PrepareToBecomeActiveHouseholdNeedsSimsSentHome())
                {
                    // Custom
                    foreach (CaregiverRoutingMonitor.ChildPlaceholder placeholder in ths.LotHome.GetObjects<CaregiverRoutingMonitor.ChildPlaceholder>())
                    {
                        if ((placeholder.SimDescription != null) && (placeholder.SimDescription.Household == ths))
                        {
                            placeholder.Rematerialize();
                        }
                    }
                }

                /*
                if ((GameUtils.IsInstalled(ProductVersion.EP11)) && (!GameUtils.IsFutureWorld()))
                {
                    FutureDescendantService instance = FutureDescendantService.GetInstance();
                    if (instance != null)
                    {
                        instance.CleanUpFutureDescendantService(true);
                        instance.InitializeFutureDescendantService();
                    }
                }*/
            }

            public static bool PerformImmediate(Sim sim, bool allowHouseholdSwitch, bool catchDreams, bool forceFull)
            {
                bool success = false;

                Common.StringBuilder msg = new Common.StringBuilder("DreamCatcher: " + sim.FullName + Common.NewLine);

                try
                {
                    msg += "A";

                    if ((!forceFull) && (PlumbBob.sCurrentNonNullHousehold != null) && (PlumbBob.sCurrentNonNullHousehold == sim.Household))
                    {
                        msg += "B";

                        if (PlumbBob.SelectedActor == sim)
                        {
                            return true;
                        }

                        msg += "C";

                        using (SafeStore store = new SafeStore(sim.SimDescription, SafeStore.Flag.Selectable | SafeStore.Flag.Unselectable | SafeStore.Flag.StoreOpportunities))
                        {
                            if (PlumbBob.SelectActor(sim)) return true;

                            msg += "C2";

                            return false;
                        }
                    }

                    msg += "D";

                    if (!allowHouseholdSwitch) return false;

                    msg += "E";

                    if (GameUtils.IsInstalled(ProductVersion.EP5))
                    {
                        PetAdoption.RemoveNeighborAdoptionOnHousehold(sim.Household.LotHome);
                    }

                    if (sim.Posture == null)
                    {
                        sim.Posture = null; // Resets to Standing
                    }

                    using (HouseholdStore store = new HouseholdStore(sim.Household, catchDreams))
                    {
                        if (sim.Household.LotHome != null)
                        {
                            PrepareToBecomeActiveHousehold(sim.Household);
                        }

                        foreach (SimDescription member in sim.Household.AllSimDescriptions)
                        {
                            if (member.GetMiniSimForProtection() == null)
                            {
                                MiniSimDescription miniSim;
                                if (MiniSimDescription.sMiniSims.TryGetValue(member.SimDescriptionId, out miniSim))
                                {
                                    miniSim.mHomeWorld = WorldName.UserCreated;
                                }
                            }
                        }

                        Sim previousSim = Sim.ActiveActor;

                        try
                        {
                            success = PlumbBob.ForceSelectActor(sim);
                        }
                        catch (Exception exception)
                        {
                            Common.Exception(sim, exception);

                            if (previousSim != null)
                            {
                                PlumbBob.ForceSelectActor(previousSim);
                            }
                        }
                    }

                    msg += "F " + success;

                    if ((MapTagManager.ActiveMapTagManager != null) && (sim.LotHome != null))
                    {
                        MapTagManager.ActiveMapTagManager.Reset();
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
                finally
                {
                    //Common.DebugNotify(msg);
                }

                return success;
            }

            protected override void OnPerform()
            {
                PerformImmediate(mSim, mAllowHouseholdSwitch, mCatchDreams, true);
            }
        }
        public class DreamStore
        {
            bool mInitialStore = false;

            SimDescription mSim;

            DnPExportData mDnPExportData;

            DreamsAndPromisesManager mDnpManager;

            OpportunityManager mOpportunityManager;

            OpportunityStore mOppStore;

            public DreamStore(Sim sim, bool initialStore, bool simpleRetain)
            {
                mInitialStore = initialStore;

                mSim = sim.SimDescription;

                mDnPExportData = null;

                bool storeDnP = false;

                if ((sim.mDreamsAndPromisesManager != null) &&
                    (sim.DreamsAndPromisesManager.mPromiseNodes != null))
                {
                    foreach (ActiveDreamNode node in sim.DreamsAndPromisesManager.mPromiseNodes)
                    {
                        if (node != null)
                        {
                            storeDnP = true;
                            break;
                        }
                    }
                }

                if (storeDnP)
                {
                    OnLoadFixup(sim, false);

                    if (simpleRetain)
                    {
                        mDnpManager = sim.DreamsAndPromisesManager;
                        sim.mDreamsAndPromisesManager = null;
                    }
                    else
                    {
                        try
                        {
                            mDnPExportData = new DnPExportData(mSim);

                            sim.NullDnPManager();
                        }
                        catch (Exception e)
                        {
                            Common.Exception(mSim, e);
                        }
                    }
                }

                if (sim.HasOpportunity())
                {
                    foreach (Opportunity opp in sim.OpportunityManager.List)
                    {
                        if (mSim.OpportunityHistory.GetCurrentOpportunity(opp.OpportunityCategory) == null)
                        {
                            mSim.OpportunityHistory.AddCurrentOpportunity(opp.OpportunityCategory, opp.Guid, null, opp.TargetObject as Sim, opp.ParentOpportunities, opp.TargetInteractionNumberItemsRequiredList(), opp.Name, opp.Description, opp.DeadlineString);
                        }
                    }

                    Corrections.CleanupOpportunities(mSim, false, null);

                    if (simpleRetain)
                    {
                        mOpportunityManager = sim.OpportunityManager;
                    }
                    else
                    {
                        mOppStore = new OpportunityStore(sim.SimDescription, true);
                    }

                    sim.mOpportunityManager = null;
                }
                else
                {
                    mSim.NeedsOpportunityImport = false;
                }
            }

            protected static bool NeedsFixup(ActiveNodeBase node)
            {
                ActiveDreamNode dreamNode = node as ActiveDreamNode;
                if (dreamNode != null)
                {
                    if (dreamNode.NodeInstance == null) return true;
                }
                else
                {
                    ActiveTimerNode timerNode = node as ActiveTimerNode;
                    if (timerNode != null)
                    {
                        if (timerNode.NodeInstance == null) return true;
                    }
                }

                return false;
            }

            protected static void OnLoadFixup(Sim sim, bool initialLoad)
            {
                for (int i = sim.DreamsAndPromisesManager.mActiveNodes.Count - 1; i >= 0; i--)
                {
                    ActiveNodeBase node = sim.DreamsAndPromisesManager.mActiveNodes[i];
                    if ((initialLoad) || (NeedsFixup(node)))
                    {
                        try
                        {
                            node.OnLoadFixup();
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim.SimDescription, e);
                        }

                        if (NeedsFixup(node))
                        {
                            sim.DreamsAndPromisesManager.mActiveNodes.RemoveAt(i);
                        }
                        else
                        {
                            try
                            {
                                sim.DreamsAndPromisesManager.AddToReferenceList(node);
                            }
                            catch (Exception e)
                            {
                                Common.Exception(sim.SimDescription, e);
                            }
                        }
                    }
                }

                for (int i = sim.DreamsAndPromisesManager.mSleepingNodes.Count - 1; i >= 0; i--)
                {
                    ActiveNodeBase node = sim.DreamsAndPromisesManager.mSleepingNodes[i];
                    if (NeedsFixup(node))
                    {
                        try
                        {
                            node.OnLoadFixup();
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim.SimDescription, e);
                        }

                        if (NeedsFixup(node))
                        {
                            sim.DreamsAndPromisesManager.mSleepingNodes.RemoveAt(i);
                        }
                    }
                }
            }

            public bool IsNeeded
            {
                get
                {
                    if ((mDnPExportData != null) || (mDnpManager != null)) return true;

                    if (mSim.NeedsOpportunityImport) return true;

                    return false;
                }
            }

            public void Restore(Sim sim)
            {
                try
                {
                    bool dnpUpdated = false;

                    if (mDnPExportData != null)
                    {
                        sim.NullDnPManager();

                        DreamsAndPromisesManager.CreateFromExportData(sim, mDnPExportData);
                        sim.SimDescription.DnPExportData = null;

                        dnpUpdated = true;
                    }
                    else if (mDnpManager != null)
                    {
                        sim.NullDnPManager();

                        sim.mDreamsAndPromisesManager = mDnpManager;

                        dnpUpdated = true;
                    }

                    if ((dnpUpdated) && (sim.DreamsAndPromisesManager != null))
                    {
                        OnLoadFixup(sim, mInitialStore);

                        sim.DreamsAndPromisesManager.SetToUpdate(true, true);
                    }

                    if (mOpportunityManager != null)
                    {
                        if (sim.mOpportunityManager != null)
                        {
                            sim.mOpportunityManager.CancelAllOpportunities();
                            sim.mOpportunityManager.TearDownLocationBasedOpportunities();
                        }

                        sim.mOpportunityManager = mOpportunityManager;
                    }
                    else if (sim.mOpportunityManager == null)
                    {
                        sim.mOpportunityManager = new OpportunityManager(sim);
                        sim.mOpportunityManager.SetupLocationBasedOpportunities();

                        if (sim.mOpportunitiesAlarmHandle == AlarmHandle.kInvalidHandle)
                        {
                            sim.ScheduleOpportunityCall();
                        }
                    }

                    try
                    {
                        if (mOppStore != null)
                        {
                            mOppStore.Dispose();
                        }

                        if (sim.OpportunityManager != null)
                        {
                            sim.OpportunityManager.Fixup();
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }

                    // Remove the opportunity alarm for inactive sims, as there is no check for selectability within it
                    if (sim.CelebrityManager != null)
                    {
                        sim.CelebrityManager.RemoveOppotunityAlarm();
                    }
                }
                catch (Exception e)
                {
                    Common.DebugException(sim, e);
                }
            }
        }
    }
}
