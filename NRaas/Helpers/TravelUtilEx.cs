using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class TravelUtilEx
    {
        public enum Type : uint
        {
            None = 0x00,
            Toddlers = 0x01,
            Pets = 0x02,
            Friends = 0x04,
            Pregnant = 0x08,
            Teens = 0x10,
            Children = 0x20,
            Recovering = 0x40,
        }

        public static Dictionary<SimDescription, string> GetTravelChoices(Sim travelInitiator, Type filter, bool forUniversity)
        {
            Dictionary<SimDescription, string> results = new Dictionary<SimDescription, string>();

            foreach (Sim sim in LotManager.Actors)
            {
                if (sim.Household != travelInitiator.Household)
                {
                    if (GameStates.sTravelData != null) continue;
                }

                if ((sim.Household != travelInitiator.Household) || (!SimTypes.IsSelectable(sim)))
                {
                    bool allow = false;

                    if ((filter & Type.Friends) != Type.None)
                    {
                        foreach (Sim member in Households.AllSims(travelInitiator.Household))
                        {
                            if (member == sim) continue;

                            Relationship relation = Relationship.Get(sim, travelInitiator, false);
                            if (relation == null) continue;

                            if (relation.AreFriendsOrRomantic())
                            {
                                allow = true;
                                break;
                            }
                        }
                    }

                    if (!allow) continue;
                }

                results[sim.SimDescription] = TravelUtilEx.CheckForReasonsToFailTravel(sim.SimDescription, filter, forUniversity ? WorldName.University : WorldName.Undefined, false, false);
            }

            return results;
        }

        public static string CheckForReasonsToFailTravel(SimDescription simDescription, Type filter, WorldName worldName, bool isWorldMove, bool testMoveRequested)
        {
            try
            {
                if (simDescription == null) return null;

                if (testMoveRequested)
                {
                    if ((TravelUtil.PlayerMadeTravelRequest) || (GameStates.WorldMoveRequested))
                    {
                        return TravelUtil.LocalizeString(simDescription.IsFemale, "AnotherTravelRequested", new object[] { simDescription });
                    }
                }

                bool playerMadeTravelRequest = TravelUtil.PlayerMadeTravelRequest;
                try
                {
                    TravelUtil.PlayerMadeTravelRequest = false;

                    string failReason = null;
                    if (!InWorldSubState.IsEditTownValid(simDescription.LotHome, ref failReason))
                    {
                        if (!string.IsNullOrEmpty(failReason) && Localization.HasLocalizationString("Gameplay/Visa/TravelUtil:EditTownInvalid" + failReason))
                        {
                            return TravelUtil.LocalizeString(simDescription.IsFemale, "EditTownInvalid" + failReason, new object[] { simDescription });
                        }

                        return TravelUtil.LocalizeString(simDescription.IsFemale, "EditTownInvalid", new object[] { simDescription });
                    }
                }
                finally
                {
                    TravelUtil.PlayerMadeTravelRequest = playerMadeTravelRequest;
                }

                foreach (Sim sim in Households.AllSims(simDescription.Household))
                {
                    try
                    {
                        if (sim.IsDying())
                        {
                            return TravelUtil.LocalizeString(sim.IsFemale, "FamilyMemberIsDying", new object[] { sim });
                        }
                        else if (sim.Autonomy.SituationComponent.InSituationOfType(typeof(ParentsLeavingTownSituation)))
                        {
                            return TravelUtil.LocalizeString(sim.IsFemale, "ParentsOutOfTown", new object[] { sim });
                        }
                        else if (sim.InteractionQueue.HasInteractionOfType(AgeDown.Singleton))
                        {
                            return TravelUtil.LocalizeString(sim.IsFemale, "SomeoneIsAgingDown", new object[] { sim });
                        }
                        else if ((sim != simDescription.CreatedSim) && sim.InteractionQueue.HasInteractionOfType(typeof(IAmMovingInteraction)))
                        {
                            return TravelUtil.LocalizeString(sim.IsFemale, "AnotherTravelRequested", new object[] { sim });
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                        return "(Error)";
                    }
                }

                if (!isWorldMove)
                {
                    if ((worldName != WorldName.FutureWorld) && (simDescription.ToddlerOrBelow))
                    {
                        if ((filter & Type.Toddlers) == Type.None)
                        {
                            return TravelUtil.LocalizeString(simDescription.IsFemale, "TooYoungToTravel", new object[] { simDescription });
                        }
                    }

                    if ((worldName == WorldName.University) && (simDescription.Teen))
                    {
                        if ((filter & Type.Teens) == Type.None)
                        {
                            return TravelUtil.LocalizeString(simDescription.IsFemale, "TooYoungToTravel", new object[] { simDescription });
                        }
                    }

                    if ((worldName == WorldName.University) && (simDescription.ChildOrBelow))
                    {
                        if ((filter & Type.Children) == Type.None)
                        {
                            return TravelUtil.LocalizeString(simDescription.IsFemale, "TooYoungToTravel", new object[] { simDescription });
                        }
                    }

                    if (simDescription.IsPet)
                    {
                        if ((filter & Type.Pets) == Type.None)
                        {
                            return Common.Localize("Travel:PetDeny", simDescription.IsFemale, new object[] { simDescription });
                        }
                    }

                    if ((worldName != WorldName.University) && (worldName != WorldName.FutureWorld) && (simDescription.CreatedSim != null) && simDescription.CreatedSim.BuffManager.HasElement(BuffNames.WentToLocation))
                    {
                        if ((filter & Type.Recovering) == Type.None)
                        {
                            return TravelUtil.LocalizeString(simDescription.IsFemale, "RecoveringFromVacation", new object[] { simDescription });
                        }
                    }
                }

                if (Sims3.Gameplay.Passport.Passport.IsHouseholdSimAwayInPassport(simDescription.Household))
                {
                    return TravelUtil.LocalizeString(simDescription.IsFemale, "SimInPassport", new object[] { simDescription });
                }

                if ((simDescription.CreatedSim != null) && simDescription.CreatedSim.BuffManager.HasElement(BuffNames.VampireBite))
                {
                    return TravelUtil.LocalizeString(simDescription.IsFemale, "VampireBite", new object[] { simDescription });
                }

                if (simDescription.IsVisuallyPregnant)
                {
                    if ((filter & Type.Pregnant) == Type.None)
                    {
                        return TravelUtil.LocalizeString(simDescription.IsFemale, "IsPregnant", new object[] { simDescription });
                    }
                }

                Sim createdSim = simDescription.CreatedSim;

                if ((createdSim != null) && createdSim.BuffManager.HasElement(BuffNames.MalePregnancy))
                {
                    if ((filter & Type.Pregnant) == Type.None)
                    {
                        return TravelUtil.LocalizeString(simDescription.IsFemale, "ConditionPreventsTravel", new object[] { simDescription });
                    }
                }

                if ((createdSim != null) && createdSim.IsDying())
                {
                    return TravelUtil.LocalizeString(simDescription.IsFemale, "IsDying", new object[] { simDescription });
                }

                if ((createdSim != null) && (createdSim.CurrentInteraction is IGoToJail))
                {
                    return TravelUtil.LocalizeString(simDescription.IsFemale, "IsInJail", new object[] { simDescription });
                }

                if (simDescription.IsEnrolledInBoardingSchool())
                {
                    return TravelUtil.LocalizeString(simDescription.IsFemale, "IsInBoardingSchool", new object[] { simDescription });
                }

                if ((ParentsLeavingTownSituation.Adults != null) && ParentsLeavingTownSituation.Adults.Contains(simDescription.SimDescriptionId))
                {
                    return TravelUtil.LocalizeString(simDescription.IsFemale, "ParentsOutOfTown", new object[] { simDescription });
                }

                if (simDescription.mCurrentPassportChallengeID != PassportChallenges.ChallengeIds.InvalidChallenge)
                {
                    return TravelUtil.LocalizeString(simDescription.IsFemale, "AwayInPassport", new object[] { simDescription });
                }

                if ((createdSim != null) && createdSim.BuffManager.HasElement(BuffNames.Ensorcelled))
                {
                    return TravelUtil.LocalizeString(simDescription.IsFemale, "Ensorcelled", new object[] { simDescription });
                }

                if (filter == Type.None)
                {
                    if ((createdSim != null) && (TravelUtil.HasBlockingTransformBuff(createdSim, worldName == WorldName.FutureWorld)))
                    {
                        return TravelUtil.LocalizeString(simDescription.IsFemale, "NoTravelWhileTransformed", new object[0]);
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(simDescription, e);
                return "(Error)";
            }
        }

        public static bool CanSimTriggerTravelToUniversityWorld(Sim actor, Type filter, bool testMoveRequested, ref GreyedOutTooltipCallback callback)
        {
            if (actor == null) 
            {
                return false;
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP9))
            {
                return false;
            }

            if ((actor.DegreeManager != null) && (actor.DegreeManager.HasCompletedAllDegrees()))
            {
                callback = new GreyedOutTooltipCallback(new TravelUtil.GreyedOutTooltipHelper(actor, "AlreadyHasAllDegrees", true).TextTextAndAway);
                return false;
            }

            string reason = CheckForReasonsToFailTravel(actor.SimDescription, filter, WorldName.Undefined, false, testMoveRequested);
            if (!string.IsNullOrEmpty(reason))
            {
                callback = delegate { return reason; };
                return false;
            }

            return true;
        }
    }
}

