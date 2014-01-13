using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Dialogs;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Annihilation
    {
        public static void RemoveSimDescriptionRelationships(SimDescription x)
        {
            IEnumerable<Relationship> enumerable = Relationship.Get(x);
            if (enumerable != null)
            {
                foreach (Relationship relationship in enumerable)
                {
                    SimDescription otherSimDescription = relationship.GetOtherSimDescription(x);
                    if (x != otherSimDescription)
                    {
                        // Changed
                        if (Relationship.sAllRelationships.ContainsKey(otherSimDescription))
                        {
                            Relationship.sAllRelationships[otherSimDescription].Remove(x);
                        }
                    }
                }
                Relationship.sAllRelationships.Remove(x);
            }
        }

        public static void CleanseGenealogy(IMiniSimDescription me)
        {
            Genealogy genealogy = me.CASGenealogy as Genealogy;
            if (genealogy != null)
            {
                genealogy.ClearAllGenealogyInformation();
            }
        }

        public static bool Perform(SimDescription sim, bool cleanse)
        {
            if (sim == null) return false;

            Common.StringBuilder msg = new Common.StringBuilder("Perform" + Common.NewLine);

            Household house = sim.Household;
            try
            {
                Sim createdSim = sim.CreatedSim;

                if (createdSim != null)
                {
                    createdSim.ReservedVehicle = null;
                }

                if (PetAdoption.sNeighborAdoption != null)
                {
                    if (PetAdoption.sNeighborAdoption.mMother == sim)
                    {
                        try
                        {
                            PetAdoption.ResetNeighborAdoption();
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, e);
                        }
                        finally
                        {
                            PetAdoption.sNeighborAdoption = null;
                        }
                    }
                    else
                    {
                        PetAdoption.sNeighborAdoption.mPetsToAdopt.Remove(sim);
                    }
                }

                foreach (Service service in Services.AllServices)
                {
                    try
                    {
                        if (service == null) continue;

                        msg += Common.NewLine + "Service: " + service.GetType();

                        if (sim.CreatedSim != null)
                        {
                            service.RemoveAssignment(sim.CreatedSim);
                        }

                        service.RemovePreferredSim(sim);

                        service.mPool.Remove(sim);

                        ResortWorker resortWorker = service as ResortWorker;
                        if (resortWorker != null)
                        {
                            if (resortWorker.mWorkerInfo != null)
                            {
                                List<ObjectGuid> remove = new List<ObjectGuid>();

                                foreach (KeyValuePair<ObjectGuid, ResortWorker.WorkerInfo> info in resortWorker.mWorkerInfo)
                                {
                                    if ((info.Value.CurrentSimDescriptionID == sim.SimDescriptionId) ||
                                        (info.Value.DesiredSimDescriptionID == sim.SimDescriptionId))
                                    {
                                        remove.Add(info.Key);
                                    }
                                }

                                foreach (ObjectGuid rem in remove)
                                {
                                    resortWorker.mWorkerInfo[rem] = new ResortWorker.WorkerInfo();
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, null, msg, e);
                    }
                }

                msg += Common.NewLine + "A";

                List<Household> houses = new List<Household>();
                if (house != null)
                {
                    houses.Add(house);
                }

                foreach (Household check in Household.sHouseholdList)
                {
                    if (check == house) continue;

                    if (check.Contains(sim))
                    {
                        houses.Add(check);
                    }
                }

                foreach(Household remove in houses)
                {
                    Household.HouseholdSimsChangedCallback changedCallback = null;

                    try
                    {
                        changedCallback = remove.HouseholdSimsChanged;
                        remove.HouseholdSimsChanged = null;

                        remove.Remove(sim, !remove.IsSpecialHousehold);
                    }
                    finally
                    {
                        remove.HouseholdSimsChanged = changedCallback;
                    }
                }

                msg += Common.NewLine + "B";

                try
                {
                    if (cleanse)
                    {
                        if (sim.LifeEventManager != null)
                        {
                            sim.LifeEventManager.Purge();
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, null, msg, e);
                }

                msg += Common.NewLine + "C";

                RemoveSimDescriptionRelationships(sim);

                msg += Common.NewLine + "D";

                PetPoolManager.RemoveIdFromPool(sim.SimDescriptionId);

                msg += Common.NewLine + "E";

                try
                {
                    if (sim.CareerManager != null)
                    {
                        sim.CareerManager.LeaveAllJobs(Career.LeaveJobReason.kDied);
                    }
                }
                catch (Exception e)
                {
                    sim.CareerManager.mJob = null;
                    sim.CareerManager.mSchool = null;

                    Common.Exception(sim, null, msg, e);
                }

                msg += Common.NewLine + "F";

                if (sim.CASGenealogy == null)
                {
                    // Necessary to stop an error in MidLifeCrisisManager
                    sim.mGenealogy = new Genealogy(sim);
                }

                msg += Common.NewLine + "G";

                try
                {
                    if (sim.Partner != null)
                    {
                        sim.Partner.Partner = null;
                    }

                    sim.Partner = null;
                }
                catch (Exception e)
                {
                    Common.Exception(sim, null, msg, e);
                }

                msg += Common.NewLine + "H";

                if (sim.DeathStyle != SimDescription.DeathType.None)
                {
                    // Passing in a household can invoke the social worker, so don't bother
                    Urnstone.FinalizeSimDeath(sim, null/*house*/);
                }

                msg += Common.NewLine + "I";

                Urnstone urnstone = Urnstones.FindGhostsGrave(sim);

                msg += Common.NewLine + "J";

                if ((cleanse) && (urnstone != null))
                {
                    if (urnstone.InInventory)
                    {
                        Inventory inventory = Inventories.ParentInventory(urnstone);
                        if (inventory != null)
                        {
                            inventory.RemoveByForce(urnstone);
                        }
                    }

                    if (urnstone.DeadSimsDescription != null)
                    {
                        urnstone.DeadSimsDescription.Fixup();
                    }

                    try
                    {
                        urnstone.Dispose();
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(sim, null, msg, e);
                    }

                    urnstone.Destroy();
                }

                msg += Common.NewLine + "K";

                if ((FakeMetaAutonomy.Instance != null) && (FakeMetaAutonomy.Instance.mPool != null))
                {
                    FakeMetaAutonomy.Instance.mPool.Remove(sim);
                }

                msg += Common.NewLine + "L";

                if (Sims3.Gameplay.Services.FakeMetaAutonomy.mToDestroy != null)
                {
                    Sims3.Gameplay.Services.FakeMetaAutonomy.mToDestroy.Remove(sim);
                }

                msg += Common.NewLine + "M";

                if ((houses.Contains(Household.ActiveHousehold)) && (sim.CreatedSim != null))
                {
                    HudModel model = HudController.Instance.Model as HudModel;

                    foreach (SimInfo info in model.mSimList)
                    {
                        if (info.mGuid == sim.CreatedSim.ObjectId)
                        {
                            model.RemoveSimInfo(info);
                            model.mSimList.Remove(info);
                            break;
                        }
                    }
                }

                msg += Common.NewLine + "N";

                try
                {
                    if (sim.AssignedRole != null)
                    {
                        sim.AssignedRole.RemoveSimFromRole();
                    }
                }
                catch (Exception e)
                {
                    Common.DebugException(sim, null, msg, e);

                    sim.AssignedRole = null;
                }

                msg += Common.NewLine + "O1";

                if ((CarNpcManager.Singleton != null) && (CarNpcManager.Singleton.NpcDriversManager != null))
                {
                    foreach (Stack<SimDescription> stack in CarNpcManager.Singleton.NpcDriversManager.mDescPools)
                    {
                        if (stack == null) continue;

                        List<SimDescription> sims = new List<SimDescription>();

                        bool found = false;
                        foreach(SimDescription stackSim in stack)
                        {
                            if (stackSim == sim)
                            {
                                found = true;
                            }
                            else
                            {
                                sims.Add(stackSim);
                            }
                        }

                        if (found)
                        {
                            stack.Clear();

                            foreach (SimDescription stackSim in sims)
                            {
                                stack.Push(stackSim);
                            }
                        }
                    }
                }

                msg += Common.NewLine + "O2";

                if (cleanse)
                {
                    sim.Dispose();
                }

                msg += Common.NewLine + "P";

                if ((house != null) && (createdSim != null))
                {
                    house.OnMemberChanged(sim, createdSim);
                }
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(sim, null, msg, e);

                if (house != null)
                {
                    Household.HouseholdSimsChangedCallback changedCallback = null;

                    try
                    {
                        changedCallback = house.HouseholdSimsChanged;
                        house.HouseholdSimsChanged = null;

                        house.Add(sim);
                    }
                    finally
                    {
                        house.HouseholdSimsChanged = changedCallback;
                    }
                }
                return false;
            }
        }

        public static void Cleanse(Household house)
        {
            try
            {
                foreach (SimDescription sim in new List<SimDescription>(Households.All(house)))
                {
                    Cleanse(sim);
                }

                house.Destroy();
                house.Dispose();
            }
            catch (Exception e)
            {
                Common.Exception(house.Name, e);
            }
        }
        public static bool Cleanse(IMiniSimDescription me)
        {
            try
            {
                if (me == null) return false;

                SimDescription sim = me as SimDescription;
                MiniSimDescription mini = me as MiniSimDescription;

                CleanseGenealogy(me);

                foreach (SimDescription other in SimDescription.GetSimDescriptionsInWorld())
                {
                    MiniSimDescription miniOther = MiniSimDescription.Find(other.SimDescriptionId);
                    if (miniOther == null) continue;

                    miniOther.RemoveMiniRelatioship(me.SimDescriptionId);
                }

                bool result = Perform(sim, true);

                RemoveMSD(me.SimDescriptionId);

                return result;
            }
            catch (Exception e)
            {
                Common.Exception(me, e);
                return false;
            }
        }

        public static void RemoveMSD(ulong id)
        {
            MiniSimDescription.RemoveMSD(id);

            if (FutureDescendantService.sPersistableData != null)
            {
                if (FutureDescendantService.sPersistableData.DescendantHouseholdsMap != null)
                {
                    FutureDescendantService.sPersistableData.DescendantHouseholdsMap.Remove(id);
                }
            }

            CauseEffectService instance = CauseEffectService.GetInstance();
            if (instance != null)
            {
                if (CauseEffectService.sPersistableData != null)
                {
                    if (CauseEffectService.sPersistableData.TimeTravelerSimID == id)
                    {
                        CauseEffectService.sPersistableData.TimeTravelerSimID = 0;
                    }
                }

                List<ITimeStatueUiData> timeStatueData = instance.GetTimeAlmanacTimeStatueData();
                if (timeStatueData != null)
                {
                    foreach (ITimeStatueUiData data in timeStatueData)
                    {
                        TimeStatueRecordData record = data as TimeStatueRecordData;
                        if (record == null) continue;

                        if (record.mRecordHolderId == id)
                        {
                            record.mRecordHolderId = 0;
                        }
                    }
                }
            }
        }
    }
}

