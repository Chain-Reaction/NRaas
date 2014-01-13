using NRaas.CommonSpace.Booters;
using NRaas.TravelerSpace.CareerMergers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Helpers
{
    public class InitializersEx
    {
        public static void Initialize(Initializers ths)
        {
            Common.StringBuilder msg = new Common.StringBuilder("Initialize");

            try
            {
                GameUtils.BeginLoadEvent("GameInit");
                for (int i = 0x0; i < ths.mInitializerRecords.Count; i++)
                {
                    Initializers.Record record = ths.mInitializerRecords[i];
                    GameUtils.BeginLoadEvent("GameInit/" + record.ToString());

                    Initializers.Action action = record.Instantiate();

                    if (action.Valid)
                    {
                        msg += Common.NewLine + "Instance: " + action.mInstance;
                        msg += Common.NewLine + "Init: " + action.mInit;
                        msg += Common.NewLine + "Type: " + action.mType;
                        Traveler.InsanityWriteLog(msg);

                        try
                        {
                            string header = action.mType + " - " + action.mInit;
                            using (Common.TestSpan span = new Common.TestSpan(Common.ExternalTimeSpanLogger.sLogger, header))
                            {
                                if (header == "Sims3.Gameplay.CAS.SimDescription - Void PostLoadFixUp()")
                                {
                                    PostLoadFixUp();
                                }
                                else
                                {
                                    action.DoInit();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(msg, e);
                        }
                    }

                    GameUtils.EndLoadEvent();
                }
                GameUtils.EndLoadEvent();
            }
            finally
            {
                Common.WriteLog(msg);
            }
        }

        public static void PostLoadFixUp()
        {
            List<SimDescription> list = new List<SimDescription>();
            foreach (Household household in Household.sHouseholdList)
            {
                bool flag = !household.IsPreviousTravelerHousehold;
                List<SimDescription> allSimDescriptions = household.AllSimDescriptions;
                int num = 0x0;
                while (num < allSimDescriptions.Count)
                {
                    try
                    {
                        SimDescription.sLoadedSimDescriptions.Remove(allSimDescriptions[num]);
                        allSimDescriptions[num].Fixup();
                        MiniSimDescription description = MiniSimDescription.Find(allSimDescriptions[num].SimDescriptionId);
                        if ((description != null) && flag)
                        {
                            description.Instantiated = true;
                        }
                        if (allSimDescriptions[num].IsEnrolledInBoardingSchool())
                        {
                            list.Add(allSimDescriptions[num]);
                        }
                    }
                    // Custom
                    catch (Exception e)
                    {
                        Common.Exception(allSimDescriptions[num], e);

                        allSimDescriptions[num].Dispose();
                        continue;
                    }
                    num++;
                }
                household.PostSimDescriptionLoadFixup();
            }

            if ((list.Count > 0x0) && !GameUtils.IsInstalled(ProductVersion.Undefined | ProductVersion.EP4))
            {
                foreach (SimDescription description2 in list)
                {
                    description2.BoardingSchool.OnRemovedFromSchool();
                    description2.AssignSchool();
                    if (description2.CreatedSim == null)
                    {
                        description2.Instantiate(description2.LotHome);
                    }
                }
            }

            Dictionary<SimDescription, bool> dictionary = new Dictionary<SimDescription, bool>();
            foreach (KeyValuePair<SimDescription, Dictionary<SimDescription, Relationship>> pair in Relationship.sAllRelationships)
            {
                SimDescription key = pair.Key;
                if (!key.IsValidDescription)
                {
                    dictionary[key] = true;
                }
                List<SimDescription> list3 = new List<SimDescription>(pair.Value.Keys);
                foreach (SimDescription description4 in list3)
                {
                    if (!description4.IsValidDescription)
                    {
                        dictionary[description4] = true;
                    }
                }
            }

            foreach (SimDescription description5 in dictionary.Keys)
            {
                Relationship.RemoveSimDescriptionRelationships(description5);
            }

            SimDescription.sLoadedSimDescriptions = null;
        }

        /*
        public static void FixHouseholdsInWorld(ref string msg)
        {
            Cleanup(ref msg);
            Household.Startup();
        }
        */

        /*
        public static void Cleanup(ref string msg)
        {
            List<Household> list = new List<Household>();
            foreach (Household household in Household.sHouseholdList)
            {
                if (household.LotHome != null)
                {
                    if (household.LotHome.Household != household)
                    {
                        msg += Common.NewLine + "Household Mismatch 1: " + household.Name;
                        if (household.LotHome.Household != null)
                        {
                            msg += Common.NewLine + "Household Mismatch 2: " + household.LotHome.Household.Name;
                        }

                        household.mLotHome = null;
                    }
                    else if (household == Household.sNpcHousehold)
                    {
                        household.MoveOut();
                    }
                }
                if ((household.VirtualLotHome != null) && !household.VirtualLotHome.VirtualSlotHouseholds.Contains(household))
                {
                    household.SetVirtualLot(null);
                }
                List<SimDescription> allSimDescriptions = household.AllSimDescriptions;
                int index = 0x0;
                while (index < allSimDescriptions.Count)
                {
                    if (allSimDescriptions[index] == null)
                    {
                        allSimDescriptions.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            foreach (Household household2 in Household.sHouseholdList)
            {
                household2.LogDumpSummary();
                if (!household2.IsSpecialHousehold)
                {
                    if (household2.mParent == null)
                    {
                        msg += Common.NewLine + "Household Proxy Fail: " + household2.Name;

                        list.Add(household2);
                    }
                    else if (household2.InWorld && (household2.AllSimDescriptions.Count == 0x0))
                    {
                        msg += Common.NewLine + "Household Empty: " + household2.Name;

                        list.Add(household2);
                    }
                    else
                    {
                        bool flag = false;
                        List<SimDescription> list3 = household2.AllSimDescriptions;
                        for (int i = 0x0; (i < (list3.Count - 0x1)) && !flag; i++)
                        {
                            for (int j = i + 0x1; (j < list3.Count) && !flag; j++)
                            {
                                if (list3[i] == list3[j])
                                {
                                    msg += Common.NewLine + "Household Dup Sim: " + household2.Name;

                                    list.Add(household2);
                                    flag = true;
                                }
                            }
                        }
                    }
                }
            }

            foreach (Household household3 in Household.sHouseholdList)
            {
                if (!list.Contains(household3) && !household3.IsSpecialHousehold)
                {
                    List<SimDescription> list4 = household3.AllSimDescriptions;
                    foreach (Household household4 in Household.sHouseholdList)
                    {
                        if (!list.Contains(household3) && !household3.IsSpecialHousehold)
                        {
                            List<SimDescription> list5 = household4.AllSimDescriptions;
                            if ((household4 != household3) && (list4.Count < list5.Count))
                            {
                                foreach (SimDescription description in list4)
                                {
                                    if (list5.Contains(description))
                                    {
                                        if ((household3.LotHome != null) && (household3.LotHome.Household == household3))
                                        {
                                            msg += Common.NewLine + "Household MoveOut: " + household3.Name;

                                            Lot lotHome = household3.LotHome;
                                            household3.MoveOut();
                                            lotHome.MoveIn(household4);
                                        }

                                        msg += Common.NewLine + "Household MoveOut 2: " + household3.Name;

                                        household3.AllSimDescriptions.Clear();
                                        list.Add(household3);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (Household household5 in Household.sHouseholdList)
            {
                if (list.Contains(household5) || household5.IsSpecialHousehold)
                {
                    continue;
                }
                int num4 = 0x0;
                while (num4 < household5.CurrentMembers.AllSimDescriptionList.Count)
                {
                    SimDescription simDescription = household5.CurrentMembers.AllSimDescriptionList[num4];
                    if (simDescription.Household == null)
                    {
                        simDescription.OnHouseholdChanged(household5, true);
                    }
                    else if (simDescription.Household != household5)
                    {
                        if (list.Contains(simDescription.Household))
                        {
                            simDescription.OnHouseholdChanged(household5, true);
                        }
                        else
                        {
                            Household household6 = simDescription.Household;
                            household5.Remove(simDescription);
                            if (!household6.Contains(simDescription))
                            {
                                household6.Add(simDescription);
                            }
                            continue;
                        }
                    }
                    num4++;
                }
                if (household5.CurrentMembers.Count == 0x0)
                {
                    list.Add(household5);
                }
            }

            bool flag2 = false;
            if ((Household.sTouristHousehold != null) && !GameUtils.IsInstalled(ProductVersion.EP1))
            {
                if (!list.Contains(Household.sTouristHousehold))
                {
                    list.Add(Household.sTouristHousehold);
                }
                flag2 = true;
            }

            foreach (Household household7 in list)
            {
                msg += Common.NewLine + "Household Delete: " + household7.Name;

                if (household7.Proxy != null)
                {
                    Simulator.DestroyObject(household7.ObjectId);
                }
                else
                {
                    household7.Dispose();
                    Household.sHouseholdList.Remove(household7);
                }
            }

            if (flag2)
            {
                Household.sTouristHousehold = null;
            }
        }
        */

        /*
        protected static void Dump(ref string msg)
        {
            msg += Common.NewLine + "Count: " + SimDescription.sLoadedSimDescriptions.Count;

            foreach (SimDescription sim in SimDescription.sLoadedSimDescriptions)
            {
                msg += Common.NewLine + " Sim: " + sim.FullName;

                if (sim.Household != null)
                {
                    msg += Common.NewLine + "  Household: " + sim.Household.Name;
                }

                if (sim.LotHome != null)
                {
                    msg += Common.NewLine + "  Lot: " + sim.LotHome.Name;
                    msg += Common.NewLine + "  Lot: " + sim.LotHome.Address;
                }
            }
        }
        */
    }
}