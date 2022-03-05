using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.Gameplay.OmniSpace.Events
{
    public class PlayPrankOnCoworker : Career.EventDaily
    {
        // Methods
        public PlayPrankOnCoworker()
        {
        }

        public PlayPrankOnCoworker(XmlDbRow row, Dictionary<string, Dictionary<int, CareerLevel>> careerLevels, string careerName) : base(row, careerLevels, Career.Event.DisplayTypes.TNS)
        {
        }

        public bool CheckIfAttemptingPrankOnConsecutiveDay(Business business)
        {
            if ((business.LastPrankedSim == null) || (business.LastPrankedSim != business.SimAffectedByPrank))
            {
                return false;
            }
            if (business.WasLastPrankSuccessful)
            {
                string titleText = LocalizeString(business.SimConspiringPrank, "AttemptToPrankAgain", new object[0x0]);
                StyledNotification.Format format = new StyledNotification.Format(titleText, business.SimAffectedByPrank.CreatedSim.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                StyledNotification.Show(format);
            }
            else
            {
                string str2 = LocalizeString(business.SimConspiringPrank, "BossGettingAngry", new object[] { business.SimConspiringPrank });
                StyledNotification.Format format2 = new StyledNotification.Format(str2, StyledNotification.NotificationStyle.kGameMessagePositive);
                if (business.SimConspiringPrank.CreatedSim != null)
                {
                    format2.mObject2 = business.SimConspiringPrank.CreatedSim.ObjectId;
                }
                else
                {
                    format2.mObject2Key = business.SimConspiringPrank.GetThumbnailKey(ThumbnailSize.Medium, 0x0);
                }
                StyledNotification.Show(format2);
                business.AddPerformance(Business.kCareerPerformanceLostWhenPrankRepeated);
            }
            business.WasLastPrankSuccessful = false;
            business.IsPrankSet = false;
            return true;
        }

        public override bool IsEligible(Career c)
        {
            Business business = OmniCareer.Career<Business>(c);
            if (business == null) return false;

            return (business.OwnerDescription == business.SimConspiringPrank);
        }

        public static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, name, "Gameplay/Careers/Business/PlayPrankOnCoworker:" + name, parameters);
        }

        public override void OnEndOfDay(Career c)
        {
            Business business = OmniCareer.Career<Business> (c);
            if (business != null)
            {
                if ((business.SimConspiringPrank != null) && (business.OwnerDescription == business.SimConspiringPrank))
                {
                    if (business.IsPrankSet)
                    {
                        base.Display(LocalizeString(business.SimConspiringPrank, "PrankCouldNotBeDone", new object[] { business.SimAffectedByPrank, business.SimConspiringPrank }), business.SimConspiringPrank.CreatedSim.ObjectId, c);
                        business.WasLastPrankSuccessful = false;
                    }
                    if ((business.SimAffectedByPrank != null) && (business.LastPrankedSim != business.SimAffectedByPrank))
                    {
                        business.LastPrankedSim = business.SimAffectedByPrank;
                    }
                    else
                    {
                        business.LastPrankedSim = null;
                    }
                    business.SimAffectedByPrank = null;
                    business.IsPrankSet = false;
                }
            }
        }

        public void PerformPrank(Business business)
        {
            if (!CheckIfAttemptingPrankOnConsecutiveDay(business))
            {
                bool flag = false;
                Sim createdSim = business.SimConspiringPrank.CreatedSim;
                Sim sim2 = business.SimAffectedByPrank.CreatedSim;
                bool flag2 = business.SimAffectedByPrank == createdSim.Occupation.Boss;
                Relationship relationship = Relationship.Get(createdSim, sim2, true);
                LTRData data = LTRData.Get(relationship.LTR.CurrentLTR);
                if (!relationship.STC.InNegativeContext || (data.RelationshipClass >= LTRData.RelationshipClassification.Medium))
                {
                    base.Display(LocalizeString(createdSim.SimDescription, "FunnyPrankWorked", new object[] { createdSim.SimDescription, business.SimAffectedByPrank }), createdSim.ObjectId, business);
                    flag = true;
                    float change = flag2 ? Business.kLTRIncreaseWhenPrankSucceedsOnBoss : Business.kLTRIncreaseWhenPrankSucceeds;
                    relationship.LTR.UpdateLiking(change);
                    relationship.UpdateSTCFromOutsideConversation(sim2, createdSim, CommodityTypes.Funny, (float) Business.kSTCValueOnPranks);
                }
                else if (relationship.STC.InNegativeContext || (data.RelationshipClass == LTRData.RelationshipClassification.Low))
                {
                    base.Display(LocalizeString(createdSim.SimDescription, "BadPrankFailed", new object[] { createdSim.SimDescription, business.SimAffectedByPrank }), createdSim.ObjectId, business);
                    CommodityTypes[] randomList = new CommodityTypes[] { CommodityTypes.Boring, CommodityTypes.Creepy, CommodityTypes.Insulting, CommodityTypes.Awkward, CommodityTypes.Steamed };
                    float num2 = flag2 ? Business.kLTRDecreaseWhenPrankFailsOnBoss : Business.kLTRDecreaseWhenPrankFails;
                    relationship.LTR.UpdateLiking(num2);
                    relationship.UpdateSTCFromOutsideConversation(sim2, createdSim, RandomUtil.GetRandomObjectFromList(randomList), (float) Business.kSTCValueOnPranks);
                }
                if (flag2)
                {
                    float perfChange = flag ? Business.kCareerPerformanceChange : (-1f * Business.kCareerPerformanceChange);
                    business.AddPerformance(perfChange);
                }
                business.WasLastPrankSuccessful = flag;
                business.IsPrankSet = false;
            }
        }

        public override void RunEvent(Career c)
        {
            Business business = OmniCareer.Career<Business>(c);
            if (business != null)
            {
                if (((business.SimAffectedByPrank != null) && ((business.SimAffectedByPrank.CreatedSim != null) && (business.SimAffectedByPrank.CreatedSim.Occupation != null))) && ((business.OwnerDescription != null) && (business.OwnerDescription.CreatedSim != null)))
                {
                    if (business.SimAffectedByPrank.CreatedSim.Occupation.IsAtWork)
                    {
                        PerformPrank(business);
                    }
                    else
                    {
                        SetAnotherAlarm(business);
                    }
                }
            }
        }

        public void SetAnotherAlarm(Business business)
        {
            Sim createdSim = business.SimConspiringPrank.CreatedSim;
            AlarmHandle sureShotEventAlarmHandle = business.CareerEventManager.SureShotEventAlarmHandle;
            createdSim.RemoveAlarm(sureShotEventAlarmHandle);
            float time = (float) Math.Max(0.0, (double) Math.Min(Business.kTimeToWaitForNextPrankAttempt, this.GetTimeUntilEvent(business)));
            business.CareerEventManager.SureShotEventAlarmHandle = createdSim.AddAlarm(time, TimeUnit.Hours, new AlarmTimerCallback(business.CareerEventManager.SureShotEventDelegate), "SureShotCareerEventAlarmTwo", AlarmType.AlwaysPersisted);
        }
    }
}
