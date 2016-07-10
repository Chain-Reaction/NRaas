using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Dialogs;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Helpers
{
    public class FutureDescendantServiceEx
    {
        public static List<ulong> OccultProcessed = new List<ulong>();
        public static Dictionary<ulong, SimDescription> UnpackedSims = new Dictionary<ulong, SimDescription>();        

        public static FutureDescendantService GetInstance()
        {
            if (FutureDescendantService.sInstance == null)
            {
                FutureDescendantService.CreateInstance();
            }
            return FutureDescendantService.GetInstance();
        }

        public static bool ActiveHouseholdHasDescendants()
        {
            if (Household.ActiveHousehold == null)
            {
                return false;
            }

            return GetInstance().getDescendantHouseHolds(Household.ActiveHousehold.SimDescriptions[0]).Count > 0;
        }

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

                    if (me.Genealogy == null || me.Genealogy.SimDescription == null) continue;

                    if (me.SkinToneKey == ResourceKey.kInvalidResourceKey || me.Genealogy.SimDescription.SkinToneKey == ResourceKey.kInvalidResourceKey) continue;   

                    choices.Add(sim);
                }
            }

            RandomUtil.RandomizeListOfObjects(choices);

            foreach (SimDescription sim in choices)
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

                    if (me.Genealogy == null || me.Genealogy.SimDescription == null) continue;

                    if (me.SkinToneKey == ResourceKey.kInvalidResourceKey || me.Genealogy.SimDescription.SkinToneKey == ResourceKey.kInvalidResourceKey) continue;                    

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

                List<SimDescription> children = Relationships.GetChildren(simDesc);
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
            Common.StringBuilder msg = new Common.StringBuilder("ProcessDescendantHouseholds");
           
            for (int i = 0x0; i < FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Count; i++)
            {
                try
                {
                    FutureDescendantService.FutureDescendantHouseholdInfo info = FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo[i];
                    Household descendantHousehold = info.DescendantHousehold;            
                    if (descendantHousehold != null)
                    {
                        if (Household.ActiveHousehold != null && info.HasAncestorFromHousehold(Household.ActiveHousehold))
                        {
                            msg += Common.NewLine + "descendantHousehold is not null.";
                            while (descendantHousehold.NumMembers > info.mCurrentDesiredHouseholdSize)
                            {
                                msg += Common.NewLine + "Removing descendant because the current size (" + descendantHousehold.NumMembers + ") is greater than the desired (" + info.mCurrentDesiredHouseholdSize + ")";
                                info.RemoveDescendant();
                            }

                            while (descendantHousehold.NumMembers < info.mCurrentDesiredHouseholdSize)
                            {
                                msg += Common.NewLine + "Adding descendant because the current size (" + descendantHousehold.NumMembers + ") is less  than the desired (" + info.mCurrentDesiredHouseholdSize + ")";
                                // Custom
                                if (!FutureDescendantHouseholdInfoEx.CreateAndAddDescendant(info)) break;
                            }

                            foreach (ulong num2 in Household.mDirtyNameSimIds)
                            {
                                if (info.IsSimAProgenitor(num2))
                                {
                                    SimDescription description = SimDescription.Find(num2);
                                    if (description != null)
                                    {
                                        foreach (SimDescription description2 in info.DescendantHousehold.SimDescriptions)
                                        {
                                            description2.LastName = description.LastName;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        msg += Common.NewLine + "descendantHousehold is null so instatiating a new one.";
                        // Custom
                        Household household2 = FutureDescendantHouseholdInfoEx.Instantiate(info);
                        FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo[i].mFutureDescendantHouseholdInfoDirty = true;
                        if (household2 == null)
                        {
                            msg += Common.NewLine + "NULL";
                        }
                    }                                       
                }
                catch (Exception e)
                {
                    Common.Exception(i.ToString(), e);
                }
                finally
                {                    
                    Common.DebugWriteLog(msg);
                }
            }
            Household.ClearDirtyNameSimIDs();
            return true;
        }        

        public static void BuildDescendantHouseholdSpecs(FutureDescendantService ths)
        {
            Common.StringBuilder msg = new Common.StringBuilder("BuildDescendantHouseholdSpecs");

            try
            {
                if (Household.ActiveHousehold != null)
                {                    
                    // Custom
                    for (int i = FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Count - 1; i >= 0; i--)
                    {
                        FutureDescendantService.FutureDescendantHouseholdInfo info = FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo[i];
                        if (info.HasAncestorFromHousehold(Household.ActiveHousehold) && info.mProgenitorSimIds.Count != 4)
                        {                            
                            msg += Common.NewLine + "mProgenitorSimIds wasn't 4 (" + info.mProgenitorSimIds.Count + ") so removing " + info.mHouseholdName;
                            FutureDescendantService.sPersistableData.InvalidDescendantHouseholdsInfo.Add(info);

                            FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.RemoveAt(i);
                        }
                    }
                    //                    

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
            finally
            {
                Common.DebugWriteLog(msg);
            }
        }

        public static void OnClickTimeAlmanac(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            Simulator.AddObject(new Sims3.UI.OneShotFunctionTask(new Sims3.UI.Function(FutureDescendantServiceEx.ShowTimeAlmanacDialog)));
            eventArgs.Handled = true;
        }

        public static void ShowTimeAlmanacDialog()
        {
            if (!Responder.Instance.IsGameStatePending || !Responder.Instance.IsGameStateShuttingDown)
            {
                ICauseEffectUiData causeEffectData = null;
                List<ITimeStatueUiData> timeStatueData = null;
                List<IDescendantHouseholdUiData> descendantHouseholdInfo = null;
                CauseEffectService instance = CauseEffectService.GetInstance();
                if (instance != null)
                {
                    causeEffectData = instance.GetTimeAlmanacCauseEffectData();
                    timeStatueData = instance.GetTimeAlmanacTimeStatueData();
                }
                FutureDescendantService service2 = GetInstance();
                if (service2 != null)
                {
                    // custom
                    descendantHouseholdInfo = GetTimeAlamanacDescendantHouseholdData(service2);
                }                
                Sim currentSim = PlumbBob.SelectedActor;                
                TimeAlmanacDialog.TimeAlmanacResult result = TimeAlmanacDialog.Show(currentSim.ObjectId, causeEffectData, descendantHouseholdInfo, timeStatueData);
                bool flag = currentSim.OpportunityManager.HasOpportunity(OpportunityCategory.Special);
                bool flag2 = result != TimeAlmanacDialog.TimeAlmanacResult.DoNothing;
                if (flag2 && flag)
                {
                    string promptText = Localization.LocalizeString(currentSim.IsFemale, "Ui/Caption/TimeAlmanac:ChangeEventPrompt", new object[] { currentSim, currentSim.OpportunityManager.GetActiveOpportunity(OpportunityCategory.Special).Name });
                    string buttonTrue = Localization.LocalizeString("Ui/Caption/Global:Yes", new object[0]);
                    string buttonFalse = Localization.LocalizeString("Ui/Caption/Global:No", new object[0]);
                    flag2 = TwoButtonDialog.Show(promptText, buttonTrue, buttonFalse);
                    if (flag2)
                    {
                        currentSim.OpportunityManager.CancelOpportunityByCategory(OpportunityCategory.Special);
                    }
                }
                if (flag2)
                {
                    switch (result)
                    {
                        case TimeAlmanacDialog.TimeAlmanacResult.DoNothing:
                            break;

                        case TimeAlmanacDialog.TimeAlmanacResult.TrashOpportunity:
                            currentSim.OpportunityManager.ClearLastOpportunity(OpportunityCategory.Special);
                            currentSim.OpportunityManager.AddOpportunityNow(OpportunityNames.EP11_Trigger_DystopiaFuture, true, false);
                            return;

                        case TimeAlmanacDialog.TimeAlmanacResult.MeteorOpportunity:
                            if (CauseEffectWorldState.kUtopiaState != instance.GetCurrentCauseEffectWorldState())
                            {
                                if (CauseEffectWorldState.kDystopiaState == instance.GetCurrentCauseEffectWorldState())
                                {
                                    currentSim.OpportunityManager.ClearLastOpportunity(OpportunityCategory.Special);
                                    currentSim.OpportunityManager.AddOpportunityNow(OpportunityNames.EP11_Undo_DystopiaFuture, true, false);
                                    return;
                                }
                                break;
                            }
                            currentSim.OpportunityManager.ClearLastOpportunity(OpportunityCategory.Special);
                            unchecked
                            {
                                currentSim.OpportunityManager.AddOpportunityNow((OpportunityNames)(-5928144135704983787L), true, false);
                            }
                            return;

                        case TimeAlmanacDialog.TimeAlmanacResult.RainbowOpportunity:
                            currentSim.OpportunityManager.ClearLastOpportunity(OpportunityCategory.Special);
                            currentSim.OpportunityManager.AddOpportunityNow(OpportunityNames.EP11_Trigger_UtopiaFuture, true, false);
                            break;

                        default:
                            return;
                    }
                }
            }
        }

        public static List<IDescendantHouseholdUiData> GetTimeAlamanacDescendantHouseholdData(FutureDescendantService instance)
        {
            // custom
            BuildDescendantHouseholdSpecs(instance);
            List<IDescendantHouseholdUiData> list = new List<IDescendantHouseholdUiData>();
            List<ulong> remove = new List<ulong>();
            foreach (ulong num in FutureDescendantService.sPersistableData.DescendantHouseholdsMap.Keys)
            {
                // Overwatch does this too but no harm in doing it here too                
                if (SimDescription.Find(num) == null && MiniSims.Find(num) == null)
                {                    
                    remove.Add(num);                    
                }

                FutureDescendantService.DescendantHouseholdUiData item = new FutureDescendantService.DescendantHouseholdUiData
                {
                    mAncestorSimId = num
                };
                int householdWealthScore = 0;
                int numberOfMembers = 0;
                bool flag = false;
                foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.DescendantHouseholdsMap[num])
                {                    
                    if (remove.Contains(num) && info.IsSimAnAncestor(num))
                    {
                        FutureDescendantService.sPersistableData.InvalidDescendantHouseholdsInfo.Add(info);
                        flag = true;
                        continue;
                    }

                    if (!info.HasAncestorFromHousehold(Household.ActiveHousehold))
                    {
                        flag = true;
                        continue;
                    }

                    foreach (ulong num4 in info.mHouseholdMembers)
                    {
                        IMiniSimDescription iMiniSimDescription = SimDescription.GetIMiniSimDescription(num4);
                        if ((iMiniSimDescription != null) && !item.mHouseholdMembers.Contains(iMiniSimDescription))
                        {
                            item.mHouseholdMembers.Add(iMiniSimDescription);
                        }
                    }
                    householdWealthScore += info.mCurrentHouseholdWealthScore;
                    numberOfMembers += info.mCurrentDesiredHouseholdSize;
                }
                householdWealthScore /= Math.Max(1, FutureDescendantService.sPersistableData.DescendantHouseholdsMap[num].Count);
                numberOfMembers /= Math.Max(1, FutureDescendantService.sPersistableData.DescendantHouseholdsMap[num].Count);
                item.mHouseholdWorth = FutureDescendantService.GetWealthTypeString(householdWealthScore);
                item.mHouseholdSize = FutureDescendantService.GetHouseholdSizeString(numberOfMembers);
                if (item != null && !flag)
                {
                    list.Add(item);
                }
            }
            return list;
        }        

        public static void FixupOccults(FutureDescendantService ths, FutureDescendantService.FutureDescendantHouseholdInfo descendantHouseholdInfo)
        {
            Common.StringBuilder msg = new Common.StringBuilder("FixupOccults");

            if (descendantHouseholdInfo == null)
            {
                msg += Common.NewLine + "descendantHouseholdInfo null";
            }

            if (descendantHouseholdInfo.DescendantHousehold == null)
            {
                msg += Common.NewLine + "descendantHouseholdInfo.DescendantHousehold null";
            }

            if (!descendantHouseholdInfo.mFutureDescendantHouseholdInfoDirty)
            {
                msg += Common.NewLine + "mFutureDescendantHosueholdInfoDirty is false";
            }

            if (Traveler.Settings.mChanceOfOccultHybrid == 0)
            {
                msg += Common.NewLine + "Hybrid 0, returning";
                Common.DebugWriteLog(msg);
                ths.FixupOccults(descendantHouseholdInfo);
                return;
            }            

            try
            {                
                if (((descendantHouseholdInfo != null) && descendantHouseholdInfo.mFutureDescendantHouseholdInfoDirty) && (descendantHouseholdInfo.DescendantHousehold != null))                
                {                    
                    descendantHouseholdInfo.mFutureDescendantHouseholdInfoDirty = false;
                    List<OccultTypes> list = null;                    
                    float minAlienPercentage = 0f;
                    float maxAlienPercentage = 0f;                   
                    if (descendantHouseholdInfo.mProgenitorSimIds != null)
                    {                                                                        
                        foreach (ulong num in descendantHouseholdInfo.mProgenitorSimIds)
                        {
                            SimDescription item = null;
                            bool unpacked = false;
                            msg += Common.NewLine + "Num: " + num;                           
                            item = FutureDescendantHouseholdInfoEx.CreateProgenitor(num, out unpacked);

                            if (CrossWorldControl.sRetention.transitionedOccults.ContainsKey(num))
                            {
                                list = CrossWorldControl.sRetention.transitionedOccults[num];
                            }
                            else
                            {
                                msg += Common.NewLine + "Couldn't find Sim in transitionedOccults (Maybe they had none?)";
                            }

                            if (item != null)
                            {
                                // EA appears to transition this... I hope :)
                                msg += Common.NewLine + "Working on " + item.FullName;
                                if (SimTypes.IsServiceAlien(item))
                                {
                                    msg += Common.NewLine + "Is full blood Alien";
                                    maxAlienPercentage = 1f;
                                }
                                else
                                {
                                    msg += Common.NewLine + "Died (2)";
                                    if (item.AlienDNAPercentage == 0 && item.IsAlien)
                                    {
                                        msg += Common.NewLine + "IsAlien";
                                        minAlienPercentage = 0;
                                        maxAlienPercentage = 1;
                                    }
                                    else
                                    {
                                        if (item.AlienDNAPercentage > maxAlienPercentage)
                                        {
                                            maxAlienPercentage = item.mAlienDNAPercentage;
                                        }
                                        else if (item.AlienDNAPercentage > minAlienPercentage)
                                        {
                                            minAlienPercentage = item.mAlienDNAPercentage;
                                        }
                                    }
                                }                                                                 
                            }
                            else
                            {
                                msg += Common.NewLine + "Failed to find SimDesc";
                            }
                        }
                    }                    

                    if (descendantHouseholdInfo.mHouseholdMembers != null)
                    {                        
                        foreach (ulong num3 in descendantHouseholdInfo.mHouseholdMembers)
                        {
                            SimDescription newSim = SimDescription.Find(num3);
                            if (newSim != null && !newSim.IsDead)
                            {
                                msg += Common.NewLine + "Processing: " + newSim.FullName;                                
                                if (Traveler.Settings.mChanceOfOccultMutation == 0)
                                {
                                    msg += Common.NewLine + "Occult Mutation 0";
                                    List<OccultTypes> descendantOccults = OccultTypeHelper.CreateList(newSim, true);
                                    foreach (OccultTypes type in descendantOccults)
                                    {
                                        if (list == null || !list.Contains(type))
                                        {
                                            OccultTypeHelper.Remove(newSim, type, true);
                                        }
                                    }
                                }

                                if (list != null && list.Count > 0)
                                {
                                    msg += Common.NewLine + "Applying Occult Chance to " + newSim.FullName;
                                    bool success = FutureDescendantHouseholdInfoEx.ApplyOccultChance(newSim, list, Traveler.Settings.mChanceOfOccultHybrid, Traveler.Settings.mChanceOfOccultMutation, Traveler.Settings.mMaxOccult);
                                    if (success)
                                    {
                                        msg += Common.NewLine + "Added occults";
                                    }
                                }
                                else
                                {
                                    msg += Common.NewLine + "No occults found...";
                                }

                                if (minAlienPercentage > 0 || maxAlienPercentage > 0 && RandomUtil.CoinFlip())
                                {
                                    float percent = (minAlienPercentage + maxAlienPercentage) / 2f;
                                    float jitter = SimDescription.kAlienDNAJitterPercent * 2; // 2 generations have passed considering Travelers approach to descendants
                                    percent = RandomUtil.GetFloat(-jitter, jitter);
                                    newSim.SetAlienDNAPercentage(MathUtils.Clamp(percent, 0f, 1f));
                                    
                                    msg += Common.NewLine + "Made alien. Percent: " + newSim.mAlienDNAPercentage;
                                }
                            }
                            else
                            {
                                msg += Common.NewLine + "New Sim was null.";
                            }
                        }
                    }                    
                }
            }
            catch (Exception e)
            {
                Common.Exception("", e);
            }
            finally
            {
                Common.DebugWriteLog(msg);
            }
        }

        public static void RegenerateDescendants()
        {
            if (!GameUtils.IsFutureWorld() || Traveler.Settings.mDisableDescendants)
            {
                return;
            }

            try
            {
                OccultProcessed.Clear();
                
                foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo)
                {                    
                    if (info.HasAncestorFromHousehold(Household.ActiveHousehold) && info.DescendantHousehold != null)
                    {
                        Annihilation.Cleanse(info.DescendantHousehold);
                    }
                }
                
                PostFutureWorldLoadProcess(GetInstance());
            }
            catch (Exception e)
            {
                Common.Exception("RegenerateDescendants", e);
            }           
        }

        public static void WipeDescendants()
        {
            if (FutureDescendantService.sPersistableData == null)
            {
                return;
            }

            if (Household.ActiveHousehold == null)
            {
                return;
            }

            foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo)
            {
                if (info.HasAncestorFromHousehold(Household.ActiveHousehold))
                {
                    FutureDescendantService.sPersistableData.InvalidDescendantHouseholdsInfo.Add(info);

                    if (info.DescendantHousehold != null && !GameUtils.IsFutureWorld())
                    {
                        foreach (ulong id in info.mHouseholdMembers)
                        {
                            MiniSimDescription mini = MiniSims.Find(id);
                            if (mini != null)
                            {
                                foreach (SimDescription desc in Household.ActiveHousehold.AllSimDescriptions)
                                {
                                    mini.RemoveMiniRelatioship(desc.SimDescriptionId);
                                }
                            }
                        }
                    }

                    if (info.DescendantHousehold != null && GameUtils.IsFutureWorld())
                    {
                        Annihilation.Cleanse(info.DescendantHousehold);                        
                    }                    
                }
            }

            foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.InvalidDescendantHouseholdsInfo)
            {
                FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Remove(info);
            }

            if (GameUtils.IsFutureWorld())
            {
                // will axe map tags
                GetInstance().RemoveInactiveDescendantHouseholds();
            }
        }

        public static void AddListeners()
        {            
            GetInstance().InitializeHouseholdEventListeners();
        }

        public static void ClearListeners()
        {            
            GetInstance().CleanUpEventListeners();
        }

        public static void TransitionSettings()
        {
            // I'll figure out why ITransition doesn't work one day...
            if (CrossWorldControl.sRetention.transitionedSettings.ContainsKey("DisableDescendants"))
            {
                Traveler.Settings.mDisableDescendants = (bool)CrossWorldControl.sRetention.transitionedSettings["DisableDescendants"];
            }

            if (CrossWorldControl.sRetention.transitionedSettings.ContainsKey("ChanceOfOccultMutation"))
            {
                Traveler.Settings.mChanceOfOccultMutation = (int)CrossWorldControl.sRetention.transitionedSettings["ChanceOfOccultMutation"];
            }

            if (CrossWorldControl.sRetention.transitionedSettings.ContainsKey("ChanceOfOccultHybrid"))
            {
                Traveler.Settings.mChanceOfOccultHybrid = (int)CrossWorldControl.sRetention.transitionedSettings["ChanceOfOccultHybrid"];
            }

            if (CrossWorldControl.sRetention.transitionedSettings.ContainsKey("MaxOccult"))
            {
                Traveler.Settings.mMaxOccult = (int)CrossWorldControl.sRetention.transitionedSettings["MaxOccult"];
            }
        }

        public static void PostFutureWorldLoadProcess(FutureDescendantService ths)
        {
            Common.StringBuilder msg = new Common.StringBuilder("PostFutureWorldLoadProcess");

            TransitionSettings();

            if (Traveler.Settings.mDisableDescendants)
            {
                msg += Common.NewLine + "Disabled, returning";
                Common.DebugWriteLog(msg);
                ths.RemoveInactiveDescendantHouseholds();
                return;
            }

            try
            {
                ths.PostWorldLoadFixupOfHouseholds();

                if (!Traveler.Settings.mDisableDescendantModification || (Traveler.Settings.mDisableDescendantModification && !ActiveHouseholdHasDescendants()))
                {
                // Custom
                ProcessDescendantHouseholds(ths);
                }

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
                        msg += Common.NewLine + "DoesHouseholdLotRequireUpdate = true";
                        flag = ths.UpdateHouseholdHomeLot(ref household);
                        if (!flag)
                        {
                            msg += Common.NewLine + "UpdateHouseholdHomeLot returned false... removing household " + household.mHouseholdName;
                            remove.Add(household);
                        }
                    }

                    if (flag)
                    {
                        msg += Common.NewLine + "UpdateHouseholdHomeLot returned true";
                        household.UpdateHouseholdRelationship();
                        household.SetupDescendantAlarm();                        
                        FixupOccults(ths, household);
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
                    msg += Common.NewLine + "Successfully invalidated " + info2.mHouseholdName;
                    FutureDescendantService.sPersistableData.InvalidDescendantHouseholdsInfo.Add(info2);
                }                
                ths.CleanupdAvailableLotLists();               
                ths.RemoveInactiveDescendantHouseholds();                

                new Common.AlarmTask(20, TimeUnit.Minutes, PackupMinis);                               
            }
            catch (Exception e)
            {
                Common.Exception("PostFutureWorldLoadProcess", e);
            }
            finally
            {
                Common.DebugWriteLog(msg);
            }
        }

        public static void PackupMinis()
        {
            // stops generation of descendants from erroring out when the data vanishes
            // before it finishes            
            foreach (KeyValuePair<ulong, SimDescription> entry in UnpackedSims)
            {
                if (entry.Value != null && entry.Value.IsValidDescription)
                {
                    entry.Value.PackUpToMiniSimDescription();
                }
            }
        }
    }    
}