using System;
using System.Collections.Generic;
using Sims3.UI.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Objects.Miscellaneous.TS3Apartments;
using Sims3.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;
using System.Text;
using Sims3.Gameplay.UI;
using Sims3.UI.Hud;
using Sims3.UI.Controller;

namespace TS3Apartments
{
    public static class ApartmentController
    {

        #region Create Family
        /// <summary>
        /// Creates a family and ads them as roommates. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="familyFunds"></param>
        /// <param name="sims"></param>
        /// <param name="household"></param>
        /// <returns></returns>
        public static ApartmentFamily CreateFamily(string name, string familyFunds, string rent, List<SimDescription> sims, List<MinorPet> minorPets, Household household)
        {
            ApartmentFamily af = new ApartmentFamily();
            af.FamilyName = name;
            int.TryParse(familyFunds, out af.FamilyFunds);
            int.TryParse(rent, out af.Rent);

            household.SetFamilyFunds(household.FamilyFunds - af.FamilyFunds);

            af.Residents = sims;
            af.MinorPets = minorPets;

            return af;

        }

        #endregion Create Family

        #region Cleanup Family
        /// <summary>
        /// Checks the family members and removes ones no longer living here
        /// </summary>
        /// <param name="c"></param>
        public static void CleanupFamily(Controller c)
        {
            foreach (ApartmentFamily af in c.Families)
            {
                if (af.Residents != null)
                {
                    List<SimDescription> remove = new List<SimDescription>();
                    foreach (SimDescription sd in af.Residents)
                    {
                        //Remove if dead
                        //Not living on lot
                        if ((sd.IsDead && !sd.IsPlayableGhost) || 
                            sd.Household == null || 
                            (sd.Household != null && sd.Household.LotHome != null && sd.LotHome.LotId != c.LotCurrent.LotId))
                        {
                            remove.Add(sd);
                        }
                    }
                    if (remove != null && remove.Count > 0)
                    {
                        foreach (SimDescription sd in remove)
                        {
                            af.Residents.Remove(sd);
                            CommonMethods.PrintMessage(sd.FullName + " removed from resident list of " + af.FamilyName);
                        }
                    }
                }
            }
        }

        #endregion Cleanup Family

        #region Delete Family

        /// <summary>
        /// Delete the selected family
        /// </summary>
        /// <param name="c"></param>
        /// <param name="f"></param>
        public static void DeleteFamily(Controller c, ApartmentFamily f)
        {
            if (f.Residents != null && f.Residents.Count > 0)
            {
                foreach (SimDescription item in f.Residents)
                {
                    if (item.IsNeverSelectable)
                        RemoveRoommate(item);
                }
            }

            //If not active family, join the funds 
            if (!f.IsActive)
                c.LotCurrent.Household.SetFamilyFunds(c.LotCurrent.Household.FamilyFunds + f.FamilyFunds);

            c.Families.Remove(f);
        }

        #endregion

        #region Switch Active Household

        public static void LoadActiveHousehold(ApartmentFamily activeFamily, Controller c)
        {
            try
            {
                //Clean the families first
                CleanupFamily(c);

                //Change active families
                int currentFunds = c.LotCurrent.Household.FamilyFunds;
                int currentUnpaidBills = c.LotCurrent.Household.UnpaidBills;

                //Handle the new active family first
                foreach (ApartmentFamily f in c.Families)
                {
                    if (f.FamilyId == activeFamily.FamilyId)
                    {
                        f.IsActive = true;
                        c.LotCurrent.Household.SetFamilyFunds(f.FamilyFunds);
                        c.LotCurrent.Household.UnpaidBills = f.UnpaidBills;
                        ApartmentController.SwitchActive(f, true);

                        break;

                    }
                }

                //Select the first sim in the active household
                if (activeFamily.Residents.Count > 0 && c.LotCurrent.Household.IsActive)
                {
                    PlumbBob.ForceSelectActor(activeFamily.Residents[0].CreatedSim);
                }

                //Make everybody else roommates
                foreach (ApartmentFamily f in c.Families)
                {
                    if (f.FamilyId != activeFamily.FamilyId)
                    {
                        //If this is the previous active family
                        if (f.IsActive)
                        {
                            f.FamilyFunds = currentFunds;
                            f.UnpaidBills = currentUnpaidBills;
                        }

                        f.IsActive = false;
                        ApartmentController.SwitchActive(f, false);
                    }
                }

                //Household.ActiveHousehold.mh
                // HudModel hudModel = Sims3.UI.Responder.Instance.HudModel as HudModel;


            }
            catch (Exception ex)
            {
                CommonMethods.PrintMessage("LoadActiveHousehold: " + ex.Message);
            }
        }


        /// <summary>
        /// Switch the active family
        /// </summary>
        /// <param name="family"></param>
        /// <param name="isActive"></param>
        private static void SwitchActive(ApartmentFamily family, bool isActive)
        {
            if (isActive)
            {
                foreach (SimDescription s in family.Residents)
                {
                    //If this is true, this sim is a roommate
                    if (s.IsNeverSelectable)
                    {
                        RemoveRoommate(s);
                    }
                }

                foreach (MinorPet p in family.MinorPets)
                {
                    p.StartBehaviorSMC(true);
                }
            }
            else
            {
                foreach (SimDescription s in family.Residents)
                {
                    AddRoommate(s);
                }

                foreach (MinorPet p in family.MinorPets)
                {
                      p.StopBehaviorSMC();
                }
            }
        }

        #endregion Switch Active Household

        #region Reset Lot
        /// <summary>
        /// Reset the apartment lot and combine families
        /// </summary>
        /// <param name="c"></param>
        public static void ResetLot(Controller c)
        {
            //Reset families            
            foreach (ApartmentFamily f in c.Families)
            {
                if (!f.IsActive)
                {
                    foreach (SimDescription s in f.Residents)
                    {
                        RemoveRoommate(s);
                    }

                    c.LotCurrent.Household.SetFamilyFunds(c.LotCurrent.Household.FamilyFunds + f.FamilyFunds);
                }
            }
            c.Families = new List<ApartmentFamily>();

            //Select the first sim in the active household
            if (c.LotCurrent != null && c.LotCurrent.Household != null && c.LotCurrent.Household.Sims != null && c.LotCurrent.Household.Sims.Count > 0)
                PlumbBob.SelectActor(c.LotCurrent.Household.Sims[0]);


        }

        #endregion

        #region StopAccpetingRoommates
        public static void StopAcceptingRoommates()
        {
            List<Household> hList = new List<Household>(Sims3.Gameplay.Queries.GetObjects<Household>());
            foreach (var h in hList)
            {
                Household.RoommateManager.StopAcceptingRoommates(false);
            }
        }
        #endregion StopAccpetingRoommates

        #region Add Roommate
        /// <summary>
        /// Adds the selected sims as roommates
        /// </summary>
        /// <param name="mini"></param>
        /// <param name="household"></param>
        /// <returns></returns>
        private static bool AddRoommate(SimDescription roommate)
        {
            try
            {
                //Do this only if not roommate already
                if (roommate != null && !roommate.IsNeverSelectable)
                {
                    roommate.IsNeverSelectable = true;
                    Household.RoommateManager.mRoommates.Add(roommate.SimDescriptionId);

                    Household.RoommateManager.AddRoommateInteractions(roommate.CreatedSim);
                    if (roommate.Household != null)
                    {
                        foreach (SimDescription current in roommate.Household.SimDescriptions)
                        {
                            if (current != roommate)
                            {
                                Relationship relationship = current.GetRelationship(roommate, true);
                                relationship.Roommates = true;
                            }
                        }
                    }
                    Household.RoommateManager.SetupCallbacks(roommate.Household);
                    if (roommate.Household != null)
                    {
                        roommate.Household.OnMemberChanged(roommate, roommate.CreatedSim);
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        #endregion Add Roommate

        #region Remove Roommate
        /// <summary>
        /// Make roommate selectable
        /// </summary>
        /// <param name="roommate"></param>
        private static void RemoveRoommate(SimDescription roommate)
        {
            try
            {
                Door[] doors = roommate.LotHome.GetObjects<Door>();
                List<DoorInfo> doorInfoList = new List<DoorInfo>();
                DoorInfo df;

                for (int i = 0; i < doors.Length; i++)
                {
                    df = new DoorInfo();
                    df.doorId = doors[i].ObjectId;
                    df.doorIndex = i;
                    df.mLockType = doors[i].LockType;

                    //Door locks
                    if (doors[i].mLockOwner != null && doors[i].mLockOwner.SimDescriptionId == roommate.SimDescriptionId)
                    {
                        df.isOwner = true;
                    }

                    if (doors[i].mOwnerList != null && doors[i].mOwnerList.Contains(roommate.SimDescriptionId))
                    {
                        df.isInOwnerList = true;
                    }

                    if (df.isOwner || df.isInOwnerList)
                        doorInfoList.Add(df);
                }

                //Add remove sims from household
                bool success = Household.RoommateManager.RemoveRoommateInternal(roommate);

                Household h = roommate.Household;
                h.Remove(roommate);
                h.Add(roommate);

                //Restore door locks            
                foreach (DoorInfo info in doorInfoList)
                {
                    doors[info.doorIndex].SetLockType(info.mLockType);

                    // CommonMethods.PrintMessage(roommate.FullName + " " + info.mLockType + " owner: " + info.isOwner +  " isOwnerList: " + info.isInOwnerList);

                    if (info.isInOwnerList)
                    {
                        List<SimDescription> list = new List<SimDescription>();
                        if (doors[info.doorIndex].OwnerList != null)
                        {
                            foreach (ulong item in doors[info.doorIndex].OwnerList)
                            {
                                //Find the sim in question 
                                SimDescription sd = roommate.Household.SimDescriptions.Find(delegate(SimDescription sd2) { return sd2.SimDescriptionId == item; });
                                if (sd != null)
                                    list.Add(sd);
                            }
                        }
                        list.Add(roommate);
                        doors[info.doorIndex].SetLockTypeOwnerList(list);
                    }

                    if (info.isOwner)
                    {
                        doors[info.doorIndex].SetLockTypeAndOwner(info.mLockType, roommate);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonMethods.PrintMessage("Remove Roommate: " + ex.Message);
            }
            finally
            {
            }
        }

        #endregion Remove Roommate
    }

    public class DoorInfo
    {
        public ObjectGuid doorId;
        public CommonDoor.tLock mLockType;
        public int doorIndex;
        public bool isOwner;
        public bool isInOwnerList;
    }


}

