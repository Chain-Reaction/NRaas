using NRaas.CareerSpace.Booters;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public static class Recruit
    {
        public static bool Test(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                Career actorCareer = actor.Occupation as Career;
                Career targetCareer = target.Occupation as Career;

                if (actorCareer == null) return false;

                if ((targetCareer != null) && (actorCareer.Guid == targetCareer.Guid))
                {
                    if (actorCareer.CurLevelBranchName == targetCareer.CurLevelBranchName) return false;
                }

                if (!actorCareer.CareerAgeTest(target.SimDescription)) return false;

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnAccepted(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                OccupationNames careerName = actor.Occupation.Guid;

                bool success = false;

                if (target.Occupation == null)
                {
                    Career career = CareerManager.GetStaticCareer(careerName);
                    if (career == null) return;

                    string branch = actor.Occupation.CurLevelBranchName;

                    CareerLocation location = Career.FindClosestCareerLocation(target.SimDescription, careerName);
                    if (location == null) return;

                    if (target.SimDescription.AcquireOccupation(new AcquireOccupationParameters(location, false, false)))
                    {
                        success = true;
                    }
                }
                else
                {
                    int level = 1;
                    if (target.Occupation.Guid == actor.Occupation.Guid)
                    {
                        Career actorCareer = actor.Occupation as Career;

                        CareerLevel curLevel = actorCareer.CurLevel;

                        while (curLevel.LastLevel != null)
                        {
                            curLevel = curLevel.LastLevel;
                            if (curLevel.NextLevels.Count > 1)
                            {
                                break;
                            }
                        }

                        if (curLevel != null)
                        {
                            level = curLevel.Level;
                        }
                    }

                    Career staticCareer = CareerManager.GetStaticCareer(careerName);
                    if ((target.Occupation != null) && (careerName != OccupationNames.Undefined) && (actor.Occupation.CurLevelBranchName != null) && (staticCareer != null))
                    {
                        target.Occupation.LeaveJob(Career.LeaveJobReason.kTransfered);

                        CareerLocation location = Career.FindClosestCareerLocation(target.SimDescription, staticCareer.Guid);
                        if (location != null) 
                        {
                            CareerLevel careerLevel = null;
                            if (actor.Occupation.CurLevelBranchName != null)
                            {
                                Dictionary<int, CareerLevel> dictionary = null;
                                if (staticCareer.CareerLevels.TryGetValue(actor.Occupation.CurLevelBranchName, out dictionary))
                                {
                                    dictionary.TryGetValue(level, out careerLevel);
                                }
                            }

                            if (careerLevel == null)                            
                            {
                                foreach (Dictionary<int, CareerLevel> dictionary2 in staticCareer.CareerLevels.Values)
                                {
                                    if (dictionary2.TryGetValue(level, out careerLevel))
                                    {
                                        break;
                                    }
                                }
                            }

                            if (careerLevel != null)
                            {
                                Occupation staticOccupation = CareerManager.GetStaticOccupation(actor.Occupation.Guid);
                                if (staticOccupation != null)
                                {
                                    staticOccupation.DoTransferOfOccupation(target.SimDescription, careerLevel.BranchName, careerLevel.Level);
                                    success = true;
                                }
                            }
                        }
                    }
                }

                if (success)
                {
                    OmniCareer job = actor.Occupation as OmniCareer;
                    if (job != null)
                    {
                        job.AddToRecruits();
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
