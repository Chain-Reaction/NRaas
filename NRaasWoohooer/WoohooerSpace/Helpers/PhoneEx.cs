using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class PhoneEx : Common.IDelayedWorldLoadFinished
    {
        public void OnDelayedWorldLoadFinished()
        {
            ScheduleNextCall(0);
        }

        private static void ScheduleNextCall(int interval)
        {
            int num2;
            int num3;
            int num = SimClock.Hours24;
            if (((num + interval) >= PhoneService.HourItsOkayToCall) && (((num + interval) + Phone.kMinimumCallWindow) < PhoneService.HourItsTooLateToCall))
            {
                num2 = interval;
                num3 = PhoneService.HourItsTooLateToCall - num;
            }
            else
            {
                num2 = (0x18 - num) + PhoneService.HourItsOkayToCall;
                num3 = (0x18 - num) + PhoneService.HourItsTooLateToCall;
            }
            float time = RandomUtil.GetFloat(num2 * 60f, (num3 * 60f) - 1f);
            Phone.mPhoneCallAttemptAlarm = AlarmManager.Global.AddAlarm(time, TimeUnit.Minutes, ConsiderGeneratingPhoneCall, "PhoneCall", AlarmType.NeverPersisted, null);
        }

        private static void ConsiderGeneratingPhoneCall()
        {
            Phone.RandomCallType weightedIndex = (Phone.RandomCallType)RandomUtil.GetWeightedIndex(new float[] { Phone.kChanceForNoCall, Phone.kChanceForRandomChat, Phone.kChanceForRandomInviteOver });
            switch (weightedIndex)
            {
                case Phone.RandomCallType.kChat:
                    Sim sim;
                    SimDescription description;
                    if (!RandomUtil.RandomChance01(Phone.kChanceForBoardingSchoolCallWhenChatting) || !FindCallParticipants(-100f, out sim, out description, Phone.RandomCallType.kBoardingSchool))
                    {
                        if (RandomUtil.RandomChance01(Phone.kChanceToSuggestBachelorPartyWhenChatting) && FindCallParticipants(Phone.kLTRThresholdForSimToBeCalledChatMin, out sim, out description, Phone.RandomCallType.kSuggestBachelorParty))
                        {
                            PhoneService.PlaceCall(new BachelorParty.PhoneCallSuggestBachelorParty(sim, description), Phone.kRandomCallTimeout);
                        }
                        else if (RandomUtil.RandomChance01(Phone.kChanceToAskToPromWhenChatting) && FindCallParticipants(Phone.kLTRThresholdForSimToBeCalledChatMin, out sim, out description, Phone.RandomCallType.kAskToProm))
                        {
                            PhoneService.PlaceCall(new PromSituation.PhoneCallAskToProm(sim, description), Phone.kRandomCallTimeout);
                        }
                        else if (FindCallParticipants(Phone.kLTRThresholdForSimToBeCalledChatMin, out sim, out description, weightedIndex))
                        {
                            PhoneService.PlaceCall(new Phone.RandomChatCall(sim, description), Phone.kRandomCallTimeout);
                        }
                        break;
                    }
                    BoardingSchool.PlaceBoardingSchoolPhoneCall(sim, description, Phone.kRandomCallTimeout);
                    break;

                case Phone.RandomCallType.kInviteOver:
                    Sim sim2;
                    SimDescription description2;
                    if (FindCallParticipants(Phone.kLTRThresholdForSimToBeInvitedOverMin, out sim2, out description2, weightedIndex))
                    {
                        PhoneService.PlaceCall(new Phone.RandomInvitationCall(sim2, description2), Phone.kRandomCallTimeout);
                    }
                    break;
            }

            ScheduleNextCall(Phone.kMinimumCallInterval);
        }

        private static bool FindCallParticipants(float minLtr, out Sim callee, out SimDescription caller, Phone.RandomCallType callType)
        {
            callee = null;
            caller = null;
            if ((Sim.ActiveActor != null) && (Sim.ActiveActor.Household != null))
            {
                List<Phone.CallCandidate> randomList = new List<Phone.CallCandidate>();
                foreach (Sim sim in Sim.ActiveActor.Household.Sims)
                {
                    SimDescription simDescription = sim.SimDescription;
                    foreach (object obj2 in Phone.Call.GetAppropriateCallers(sim, minLtr, callType == Phone.RandomCallType.kBoardingSchool))
                    {
                        int num;
                        float num2;
                        SimDescription description2 = (SimDescription)obj2;
                        if ((Phone.Call.IsSimAvailableForCall(description2, out num, out num2) == Phone.Call.SimAvailability.Available) && (((callType != Phone.RandomCallType.kInviteOver) || (description2.CreatedSim == null)) || description2.CreatedSim.IsAtHome))
                        {
                            float weight = 0f;
                            bool flag = false;
                            switch (callType)
                            {
                                case Phone.RandomCallType.kSuggestBachelorParty:
                                    flag = BachelorParty.IsAvailableToHaveBachelorPartySuggested(sim);
                                    if (flag)
                                    {
                                        weight = BachelorParty.GetWeightToPlaceSuggestBachelorPartyCall(simDescription, description2);
                                    }
                                    break;

                                case Phone.RandomCallType.kAskToProm:
                                    if ((PromSituation.IsGoingToProm(sim) && !PromSituation.HasPromDate(sim)) && !PromSituation.HasPromDate(description2))
                                    {
                                        // Custom
                                        string reason;
                                        if (CommonSocials.CanGetRomantic(sim.SimDescription, description2, out reason))
                                        {
                                            weight = RomanceVisibilityState.GetWeightToPlaceCallOrInviteOver(simDescription, description2);
                                            flag = true;
                                        }
                                    }
                                    break;

                                case Phone.RandomCallType.kBoardingSchool:
                                    if (description2.IsEnrolledInBoardingSchool())
                                    {
                                        weight = BoardingSchool.GetWeightToPlaceBoardingSchoolCall(simDescription, description2);
                                        flag = true;
                                    }
                                    break;

                                default:
                                    weight = RomanceVisibilityState.GetWeightToPlaceCallOrInviteOver(simDescription, description2);
                                    flag = true;
                                    break;
                            }

                            if (flag)
                            {
                                randomList.Add(new Phone.CallCandidate(sim, description2, weight));
                            }
                        }
                    }
                }
                if (randomList.Count > 0x0)
                {
                    Phone.CallCandidate weightedRandomObjectFromList = RandomUtil.GetWeightedRandomObjectFromList(randomList);
                    callee = weightedRandomObjectFromList.Callee;
                    caller = weightedRandomObjectFromList.Caller;
                    return true;
                }
            }
            return false;
        }
    }
}
