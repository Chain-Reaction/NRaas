using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.TimeTravel;
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
    public class FutureDescendantServiceEx
    {
        private static SimDescription GetPotentialMate(SimDescription me, List<SimDescription> testAgainst, bool testRelation)
        {
            List<SimDescription> choices = new List<SimDescription>();

            foreach (List<SimDescription> sims in SimListing.GetFullResidents(false).Values)
            {
                foreach (SimDescription sim in sims)
                {
                    if (sim.LotHome == null) continue;

                    if (testRelation)
                    {
                        if (sim.Partner != null) continue;
                    }

                    if (SimTypes.IsSkinJob(sim)) continue;

                    if (!SimTypes.IsEquivalentSpecies(me, sim)) continue;

                    choices.Add(sim);
                }
            }

            RandomUtil.RandomizeListOfObjects(choices);

            foreach(SimDescription sim in choices)
            {
                if (!sim.CheckAutonomousGenderPreference(me)) continue;

                if (!Relationships.CanHaveRomanceWith(null, sim, me, false, true, testRelation, false)) continue;

                if (testRelation)
                {
                    Relationship relation = Relationship.Get(me, sim, false);
                    if ((relation != null) && (relation.CurrentLTRLiking < 0)) continue;

                    bool found = false;
                    foreach (SimDescription test in testAgainst)
                    {
                        if (Relationships.IsCloselyRelated(test, sim, false))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) continue;
                }

                return sim;
            }

            return null;
        }

        private static SimDescription GetPotentialInLaw(SimDescription me, SimDescription partner, List<SimDescription> testAgainst, bool testRelation)
        {
            List<SimDescription> choices = new List<SimDescription>();

            foreach (List<SimDescription> sims in SimListing.GetFullResidents(false).Values)
            {
                foreach (SimDescription sim in sims)
                {
                    if (sim.LotHome == null) continue;

                    if (testRelation)
                    {
                        if (sim.Partner != null) continue;
                    }

                    if (SimTypes.IsSkinJob(sim)) continue;

                    if (!SimTypes.IsEquivalentSpecies(me, sim)) continue;

                    choices.Add(sim);
                }
            }

            RandomUtil.RandomizeListOfObjects(choices);

            foreach (SimDescription sim in choices)
            {
                if (sim == me) continue;

                if (sim == partner) continue;

                if (testRelation)
                {
                    bool found = false;
                    foreach (SimDescription test in testAgainst)
                    {
                        if (Relationships.IsCloselyRelated(test, sim, false))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) continue;
                }

                return sim;
            }

            return null;
        }

        private static bool ProcessSim(FutureDescendantService ths, SimDescription simDesc)
        {
            try
            {
                if (FutureDescendantService.sPersistableData == null)
                {
                    return false;
                }
                else if (simDesc == null)
                {
                    return false;
                }
                else if (Household.ActiveHousehold != simDesc.Household)
                {
                    return false;
                }
                else if ((Household.RoommateManager != null) && Household.RoommateManager.IsNPCRoommate(simDesc))
                {
                    return false;
                }
                else if (simDesc.IsRobot)
                {
                    return false;
                }
                else if (simDesc.Genealogy == null)
                {
                    return false;
                }

                List<SimDescription> children = Relationships.GetChildren (simDesc);
                bool processed = false;
                if ((children != null) && (children.Count > 0x0))
                {
                    foreach (SimDescription child in children)
                    {
                        processed |= ProcessSim(ths, child);
                    }
                }

                if (!processed)
                {
                    foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo)
                    {
                        if (info.IsSimAProgenitor(simDesc.SimDescriptionId) || info.IsSimAnAncestor(simDesc.SimDescriptionId))
                        {
                            return true;
                        }
                    }

                    Common.StringBuilder msg = new Common.StringBuilder("FutureDescendantHouseholdInfo");
                    msg += Common.NewLine + simDesc.FullName;

                    FutureDescendantService.FutureDescendantHouseholdInfo item = new FutureDescendantService.FutureDescendantHouseholdInfo();

                    List<SimDescription> tests = new List<SimDescription>();

                    try
                    {
                        tests.Add(simDesc);

                        item.AddProgenitorSim(simDesc.SimDescriptionId);

                        SimDescription mate = simDesc.Partner;
                        if (mate == null)
                        {
                            mate = GetPotentialMate(simDesc, tests, true);
                            if (mate == null)
                            {
                                mate = GetPotentialMate(simDesc, tests, false);
                                if (mate == null)
                                {
                                    msg += Common.NewLine + "Fail";
                                    return false;
                                }
                            }
                        }

                        msg += Common.NewLine + mate.FullName;

                        tests.Add(mate);

                        item.AddProgenitorSim(mate.SimDescriptionId);

                        SimDescription inLaw = GetPotentialInLaw(simDesc, mate, tests, true);
                        if (inLaw == null)
                        {
                            inLaw = GetPotentialInLaw(simDesc, mate, tests, false);
                            if (inLaw == null)
                            {
                                msg += Common.NewLine + "Fail";
                                return false;
                            }
                        }

                        msg += Common.NewLine + inLaw.FullName;

                        tests.Add(inLaw);

                        item.AddProgenitorSim(inLaw.SimDescriptionId);

                        mate = inLaw.Partner;
                        if (mate == null)
                        {
                            mate = GetPotentialMate(inLaw, tests, true);
                            if (mate == null)
                            {
                                mate = GetPotentialMate(inLaw, tests, false);
                                if (mate == null)
                                {
                                    msg += Common.NewLine + "Fail";
                                    return false;
                                }
                            }
                        }

                        msg += Common.NewLine + mate.FullName;

                        item.AddProgenitorSim(mate.SimDescriptionId);

                        if (item.mProgenitorSimIds.Count != 4)
                        {
                            msg += Common.NewLine + "Fail";
                            return false;
                        }
                    }
                    finally
                    {
                        Common.DebugWriteLog(msg);
                    }

                    FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Add(item);
                }
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(simDesc, e);
                return false;
            }
        }

        private static bool ProcessDescendantHouseholds(FutureDescendantService ths)
        {
            for (int i = 0x0; i < FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Count; i++)
            {
                try
                {
                    FutureDescendantService.FutureDescendantHouseholdInfo info = FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo[i];
                    Household descendantHousehold = info.DescendantHousehold;
                    if (descendantHousehold != null)
                    {
                        while (descendantHousehold.NumMembers > info.mCurrentDesiredHouseholdSize)
                        {
                            info.RemoveDescendant();
                        }

                        while (descendantHousehold.NumMembers < info.mCurrentDesiredHouseholdSize)
                        {
                            // Custom
                            if (!FutureDescendantHouseholdInfoEx.CreateAndAddDescendant(info)) break;
                        }

						foreach (ulong num2 in Household.mDirtyNameSimIds)
						{
							if (info.IsSimAProgenitor(num2))
							{
								SimDescription description = SimDescription.Find(num2);
								if(description != null)
								{
									foreach (SimDescription description2 in info.DescendantHousehold.SimDescriptions)
									{
										description2.LastName = description.LastName;
									}
								}
							}
						}
                    }
                    else
                    {
                        // Custom
                        Household household2 = FutureDescendantHouseholdInfoEx.Instantiate(info);					
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(i.ToString(), e);
                }
            }
			Household.ClearDirtyNameSimIDs();
            return false;
        }

        public static void BuildDescendantHouseholdSpecs(FutureDescendantService ths)
        {
            try
            {
                if (Household.ActiveHousehold != null)
                {
                    // Custom
                    for (int i = FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Count - 1; i >= 0; i--)
                    {
                        FutureDescendantService.FutureDescendantHouseholdInfo info = FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo[i];
                        if (info.mProgenitorSimIds.Count != 4)
                        {
                            FutureDescendantService.sPersistableData.InvalidDescendantHouseholdsInfo.Add(info);

                            FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.RemoveAt(i);
                        }
                    }

                    ths.RemoveInactiveDescendantHouseholds();

                    List<SimDescription> simDescriptions = Household.ActiveHousehold.SimDescriptions;
                    foreach (SimDescription description in simDescriptions)
                    {
                        // Custom
                        ProcessSim(ths, description);
                    }

                    foreach (SimDescription description2 in simDescriptions)
                    {
                        ths.GenerateAncestorMap(description2);
                    }

                    foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo)
                    {
                        if (info.HasAncestorFromHousehold(Household.ActiveHousehold))
                        {
                            info.CalculateHouseholdScores();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("", e);
            }
        }

        public static void PostFutureWorldLoadProcess(FutureDescendantService ths)
        {
            try
            {
                ths.PostWorldLoadFixupOfHouseholds();

                // Custom
                ProcessDescendantHouseholds(ths);

                ths.BuildAvailableLotLists();

                foreach (SimDescription description in GameStates.TravelHousehold.SimDescriptions)
                {
                    ths.RemoveDescendantMapTags(description.SimDescriptionId);
                }

                List<FutureDescendantService.FutureDescendantHouseholdInfo> remove = new List<FutureDescendantService.FutureDescendantHouseholdInfo>();
                for (int i = 0x0; i < FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Count; i++)
                {
                    bool flag = true;
                    FutureDescendantService.FutureDescendantHouseholdInfo household = FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo[i];
                    if (household.DoesHouseholdLotRequireUpdate())
                    {
                        flag = ths.UpdateHouseholdHomeLot(ref household);
                        if (!flag)
                        {
                            remove.Add(household);
                        }
                    }

                    if (flag)
                    {
                        household.UpdateHouseholdRelationship();
                        household.SetupDescendantAlarm();
                        ths.FixupOccults(household);
                        ths.UpdateHouseholdMapTags(household.mAncestorsSimIds, household.DescendantHousehold);
                    }
                }

                foreach (FutureDescendantService.FutureDescendantHouseholdInfo info2 in remove)
                {
                    FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Remove(info2);
                    foreach (List<FutureDescendantService.FutureDescendantHouseholdInfo> list2 in FutureDescendantService.sPersistableData.DescendantHouseholdsMap.Values)
                    {
                        if (list2.Contains(info2))
                        {
                            list2.Remove(info2);
                        }
                    }
                    FutureDescendantService.sPersistableData.InvalidDescendantHouseholdsInfo.Add(info2);
                }
                ths.CleanupdAvailableLotLists();
                ths.RemoveInactiveDescendantHouseholds();
            }
            catch (Exception e)
            {
                Common.Exception("PostFutureWorldLoadProcess", e);
            }
        }
    }
}