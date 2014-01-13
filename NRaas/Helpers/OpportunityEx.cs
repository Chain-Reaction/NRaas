using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class OpportunityEx
    {
        private static bool CheckAllRequirements(Sim s, Opportunity op, OpportunityNames lastOpName, ref string msg)
        {
            Opportunity.OpportunitySharedData sharedData = op.SharedData;
            WorldName mTargetWorldRequired = sharedData.mTargetWorldRequired;
            WorldName currentWorld = GameUtils.GetCurrentWorld();

            if (mTargetWorldRequired == WorldName.SunsetValley)
            {
                mTargetWorldRequired = s.SimDescription.HomeWorld;
            }
            if ((mTargetWorldRequired == currentWorld) || (mTargetWorldRequired == WorldName.Undefined))
            {
                foreach (Opportunity.OpportunitySharedData.RequirementInfo info in sharedData.mRequirementList)
                {
                    if (info.mType == RequirementType.OpportunityComplete)
                    {
                        OpportunityNames mGuid = (OpportunityNames)info.mGuid;
                        if (lastOpName == mGuid)
                        {
                            continue;
                        }
                    }
                    if (!op.CheckRequirement(info, s, sharedData))
                    {
                        msg += Common.NewLine + " Failure: " + info.mType;

                        return false;
                    }
                }
            }
            OpportunityNames mCompletionTriggerOpportunity = sharedData.mCompletionTriggerOpportunity;
            if (mCompletionTriggerOpportunity != OpportunityNames.Undefined)
            {
                Opportunity staticOpportunity = OpportunityManager.GetStaticOpportunity(mCompletionTriggerOpportunity);
                if ((staticOpportunity != null) && !op.CheckAllRequirements(s, staticOpportunity, sharedData.mGuid))
                {
                    msg += Common.NewLine + " Failure: B";
                    return false;
                }
            }
            return true;
        }

        protected static bool IsAvailable(Opportunity ths, Sim s, ref string msg)
        {
            msg += Common.NewLine + ths.Guid;

            if (s == null)
            {
                msg += Common.NewLine + " Failure: C";
                return false;
            }
            if (!s.OpportunityManager.CheckRepeatOpportunity(ths.RepeatLevel, ths.Guid))
            {
                msg += Common.NewLine + " Failure: D";
                return false;
            }
            if (ths.IsChildOfOrEqualTo(s.OpportunityManager.GetLastOpportunity(ths.OpportunityCategory)) && !s.SimDescription.OpportunityHistory.HasCurrentOpportunity(ths.OpportunityCategory, ths.Guid))
            {
                msg += Common.NewLine + " Failure: E";
                return false;
            }
            if (!CheckAllRequirements(s, ths, OpportunityNames.Undefined, ref msg))
            {
                return false;
            }
            if (ths.mSharedData.mRequirementDelegate != null)
            {
                if (!ths.mSharedData.mRequirementDelegate(s, ths))
                {
                    msg += Common.NewLine + " Failure: F";
                }
            }
            return true;
        }

        public static List<Opportunity> GetAllOpportunities(Sim sim, OpportunityCategory category)
        {
            Dictionary<OpportunityNames, Opportunity> opportunityList = new Dictionary<OpportunityNames, Opportunity>();

            if (category == OpportunityCategory.None)
            {
                foreach (Opportunity opp in OpportunityManager.sDictionary.Values)
                {
                    if (opp.IsCareer) continue;

                    opportunityList[opp.Guid] = opp;
                }
            }
            else
            {
                Dictionary<OpportunityNames, Opportunity> categoryList = null;

                switch (category)
                {
                    case OpportunityCategory.AdventureEgypt:
                        categoryList = OpportunityManager.sAdventureEgyptOpportunityList;
                        break;
                    case OpportunityCategory.AdventureFrance:
                        categoryList = OpportunityManager.sAdventureFranceOpportunityList;
                        break;
                    case OpportunityCategory.AdventureChina:
                        categoryList = OpportunityManager.sAdventureChinaOpportunityList;
                        break;
                    case OpportunityCategory.Dare:
                        categoryList = OpportunityManager.sDareOpportunityList;
                        break;
                    case OpportunityCategory.DayJob:
                        categoryList = OpportunityManager.sDayJobOpportunityList;
                        break;
                    case OpportunityCategory.SocialGroup:
                        categoryList = OpportunityManager.sSocialGroupOpportunityList;
                        break;
                    case OpportunityCategory.Skill:
                        categoryList = OpportunityManager.sSkillOpportunityList;
                        break;
                    case OpportunityCategory.Career:
                        categoryList = OpportunityManager.sCareerPhoneCallOpportunityList;
                        break;
                    case OpportunityCategory.Special:
                        categoryList = OpportunityManager.sCelebritySystemOpportunityList;
                        break;
                }

                if (categoryList != null)
                {
                    foreach (Opportunity opp in categoryList.Values)
                    {
                        opportunityList[opp.Guid] = opp;
                    }
                }
            }

            return GetAllOpportunities(sim, (category != OpportunityCategory.None), opportunityList);
        }
        protected static List<Opportunity> GetAllOpportunities(Sim sim, bool singleCategory, Dictionary<OpportunityNames, Opportunity> opportunityList)
        {
            //Common.StringBuilder msg = new Common.StringBuilder("GetAllOpportunities");

            List<Opportunity> allOpportunities = new List<Opportunity>();

            if (!GameStates.IsOnVacation)
            {
                CareerManager manager = sim.CareerManager;
                if (manager != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Sims3.Gameplay.Careers.Career career = manager.Occupation as Sims3.Gameplay.Careers.Career;
                        if (i == 1)
                        {
                            career = manager.School;
                        }

                        if (career == null) continue;

                        foreach (Sims3.Gameplay.Careers.Career.EventDaily daily in career.CareerEventList)
                        {
                            Sims3.Gameplay.Careers.Career.EventOpportunity oppEvent = daily as Sims3.Gameplay.Careers.Career.EventOpportunity;
                            if (oppEvent == null) continue;

                            if (opportunityList.ContainsKey(oppEvent.mOpportunity)) continue;

                            Opportunity opportunity = OpportunityManager.GetStaticOpportunity(oppEvent.mOpportunity);
                            if (opportunity == null) continue;

                            opportunityList.Add(opportunity.Guid, opportunity);
                        }
                    }
                }
            }

            foreach (Opportunity opportunity in opportunityList.Values)
            {
                //msg += Common.NewLine + "A: " + opportunity.Guid;

                Repeatability origRepeatability = opportunity.RepeatLevel;
                OpportunityNames origTriggerOpp = opportunity.SharedData.mCompletionTriggerOpportunity;
                WorldName targetWorldRequired = opportunity.SharedData.mTargetWorldRequired;

                try
                {
                    if (!singleCategory)
                    {
                        opportunity.mSharedData.mRepeatLevel = Repeatability.Always;
                        opportunity.mSharedData.mCompletionTriggerOpportunity = OpportunityNames.Undefined;
                    }

                    if (opportunity.TargetWorldRequired == WorldName.SunsetValley) 
                    {
                        if ((singleCategory) && (Common.IsOnTrueVacation()))
                        {
                            if (sim.SimDescription.HomeWorld != GameUtils.GetCurrentWorld()) continue;
                        }
                        else
                        {
                            opportunity.SharedData.mTargetWorldRequired = WorldName.Undefined;
                        }
                    }
                    else if (opportunity.TargetWorldRequired != WorldName.Undefined)
                    {
                        if (opportunity.TargetWorldRequired != GameUtils.GetCurrentWorld()) continue;
                    }

                    if (GameStates.IsOnVacation)
                    {
                        bool career = false;
                        foreach (Opportunity.OpportunitySharedData.RequirementInfo info in opportunity.SharedData.mRequirementList)
                        {
                            if (info.mType == RequirementType.Career)
                            {
                                career = true;
                                break;
                            }
                        }

                        if (career) continue;
                    }

                    //if (IsAvailable(opportunity, sim, ref msg))
                    if (opportunity.IsAvailable(sim))
                    {
                        allOpportunities.Add(opportunity);
                    }
                }
                catch (Exception e)
                {
                    Common.DebugException(opportunity.Guid + Common.NewLine + opportunity.Name, e);
                }
                finally
                {
                    opportunity.mSharedData.mRepeatLevel = origRepeatability;
                    opportunity.mSharedData.mCompletionTriggerOpportunity = origTriggerOpp;
                    opportunity.SharedData.mTargetWorldRequired = targetWorldRequired;
                }
            }

            List<Opportunity> allPotentials = new List<Opportunity>();
            foreach (Opportunity opportunity in allOpportunities)
            {
                string name = null;

                //msg += Common.NewLine + "B: " + opportunity.Guid;

                try
                {
                    if (sim.OpportunityManager.HasOpportunity(opportunity.OpportunityCategory)) continue;

                    Opportunity toAdd = opportunity.Clone();
                    toAdd.Actor = sim;

                    // EA has coding to spawn the Time Traveler in SetupTargets(), don't do it in that case
                    if ((toAdd.SharedData.mTargetType != OpportunityTargetTypes.Sim) || (toAdd.SharedData.mTargetData != "TimeTraveler"))
                    {
                        if (!sim.OpportunityManager.SetupTargets(toAdd))
                        {
                            continue;
                        }
                    }
                    toAdd.SetLocalizationIndex();

                    name = toAdd.Name;

                    allPotentials.Add(toAdd);
                }
                catch(Exception e)
                {
                    Common.DebugException(opportunity.Guid + Common.NewLine + name, e);
                }
            }

            //Common.DebugWriteLog(msg);

            return allPotentials;
        }

        public static bool Perform(SimDescription me, OpportunityNames guid)
        {
            Opportunity opportunity = null;
            GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary.TryGetValue((ulong)guid, out opportunity);

            Repeatability origRepeatability = Repeatability.Undefined;
            if (opportunity != null)
            {
                origRepeatability = opportunity.RepeatLevel;

                opportunity.mSharedData.mRepeatLevel = Repeatability.Always;
            }

            try
            {
                me.Household.mCompletedHouseholdOpportunities.Remove((ulong)guid);

                if (opportunity.IsLocationBased)
                {
                    List<Lot> alreadyChosenLots = new List<Lot>();

                    foreach (ulong lotId in OpportunityManager.sLocationBasedOpportunities.Values)
                    {
                        Lot lot = LotManager.GetLot(lotId);
                        if (lot == null) continue;

                        alreadyChosenLots.Add(lot);
                    }

                    Lot lotTarget = OpportunityManager.GetLotTarget(opportunity, alreadyChosenLots);
                    if (lotTarget != null)
                    {
                        lotTarget.AddLocationBasedOpportunity(opportunity.EventDay, opportunity.EventStartTime, opportunity.EventEndTime, opportunity.Guid);
                        alreadyChosenLots.Add(lotTarget);
                    }
                }

                return me.CreatedSim.OpportunityManager.AddOpportunityNow(guid, true, false);
            }
            finally
            {
                if (opportunity != null)
                {
                    opportunity.mSharedData.mRepeatLevel = origRepeatability;
                }
            }
        }
    }
}
