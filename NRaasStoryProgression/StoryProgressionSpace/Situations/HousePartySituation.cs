using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Situations
{
    public class HousePartySituation : Party
    {      
        bool mbPartyFinishedCalled;
        int mNumberOfGuests;
        float mPartyMood;

        public HousePartySituation()
        { }
        public HousePartySituation(Lot lot, Sim host, List<SimDescription> guests, OutfitCategories clothingStyle, DateAndTime startTime) 
            : this(lot, host, guests, clothingStyle, startTime, HouseParty.kDefaultPartyKeys)
        { }
        protected HousePartySituation(Lot lot, Sim host, List<SimDescription> guests, OutfitCategories clothingStyle, DateAndTime startTime, HouseParty.PartyInvitationKeys invitationKeys) 
            : base(lot, host, guests, clothingStyle, startTime)
        {
            if (lot == host.LotHome)
            {
                EventTracker.SendEvent(EventTypeId.kHousePartySoon, host);
            }
            else
            {
                EventTracker.SendEvent(EventTypeId.kOffLotPartySoon, host);
            }
            mPulse = AddPulseAlarm(host, Pulse, GetParams().MinutesPerLeavingCheck, "Party Pulse");
            if (invitationKeys != null)
            {
                SetState(new DeliverInvitations(this, invitationKeys));
            }
            else
            {
                SetState(new WaitForPreparations(this));
            }
        }
        protected HousePartySituation(Lot lot, Sim host, List<SimDescription> guests, OutfitCategories clothingStyle, DateAndTime startTime, bool bDeliverInvitations)
            : this(lot, host, guests, clothingStyle, startTime, bDeliverInvitations ? HouseParty.kDefaultPartyKeys : null)
        { }

        public override Party.PartyType TypeOfParty
        {
            get
            {
                return Party.PartyType.HouseParty;
            }
        }
 
        private void AddSimToGoHome(Sim sim, List<Sim> simsToGoHome)
        {
            if (!simsToGoHome.Contains(sim))
            {
                simsToGoHome.Add(sim);
            }
        }

        public override void OnOver()
        { }

        public override CommodityKind BeAtHostedSituationMotive(Sim sim)
        {
            if (sim.SimDescription.ChildOrBelow)
            {
                return CommodityKind.ChildEnjoyParty;
            }
            if (sim.SimDescription.TeenOrBelow)
            {
                return CommodityKind.TeenEnjoyParty;
            }
            return CommodityKind.AdultEnjoyParty;
        }

        private void CheckIfGuestsNeedToGoHome()
        {
            List<Sim> simsToGoHome = new List<Sim>();
            foreach (Sim sim in Guests)
            {
                bool flag = VisitSituation.IsGuestAllowedToStayOver(sim);
                if (sim.MoodManager.IsInStrongNegativeMood || (!flag && (sim.BuffManager.HasElement(BuffNames.Tired) || sim.BuffManager.HasElement(BuffNames.Exhausted))))
                {
                    AddSimToGoHome(sim, simsToGoHome);
                }

                if (sim.SimDescription.ChildOrBelow && (SimClock.Hours24 >= GetParams().HourAtWhichChildrenGoHome))
                {
                    AddSimToGoHome(sim, simsToGoHome);
                    foreach (Genealogy genealogy in sim.Genealogy.Parents)
                    {
                        SimDescription simDescription = genealogy.SimDescription;
                        if (simDescription != null)
                        {
                            Sim createdSim = simDescription.CreatedSim;
                            if ((createdSim != null) && Guests.Contains(createdSim))
                            {
                                AddSimToGoHome(createdSim, simsToGoHome);
                            }
                        }
                    }
                }

                float delta = sim.MoodManager.MoodValue * GetParams().MoodToTimeMod;
                DateAndTime time;
                if (!flag && mTimeForSimToLeave.TryGetValue(sim.ObjectId, out time))
                {
                    time = SimClock.Add(time, TimeUnit.Minutes, delta);
                    if (time.CompareTo(SimClock.CurrentTime()) < 0x0)
                    {
                        AddSimToGoHome(sim, simsToGoHome);
                    }
                    else
                    {
                        mTimeForSimToLeave[sim.ObjectId] = time;
                    }
                }
            }
            foreach (Sim sim3 in simsToGoHome)
            {
                if (sim3.LotCurrent == Lot)
                {
                    MakeGuestGoHome(sim3);
                }
            }
        }

        public override void CleanUp()
        {
            base.CleanUp();
            if ((RentScheduler.Instance != null) && (Lot != null))
            {
                RentScheduler.Instance.FreeLot(Lot.LotId);
            }
        }

        public override float GetLikingChange(Sim sim)
        {
            float likingChange = base.GetLikingChange(sim);
            if (Host.HasTrait(TraitNames.LegendaryHost))
            {
                likingChange = Math.Max(likingChange, TraitTuning.LegendaryHostMinLikingValue);
            }
            return likingChange;
        }

        public override PartyParams GetParams()
        {
            return HouseParty.kHousePartyParams;
        }

        public override Party.PartyOutcomes GetPartyOutcome()
        {
            Party.PartyOutcomes kNeutralParty = Party.PartyOutcomes.kNeutralParty;
            if (mNumberOfGuests > 0x0)
            {
                float num = mPartyMood / ((float) mNumberOfGuests);
                bool flag = Host.HasTrait(TraitNames.LegendaryHost);
                if ((num > HouseParty.kThrewGreatPartyBuffMoodRequirement) || flag)
                {
                    return Party.PartyOutcomes.kGoodParty;
                }
                if (num < HouseParty.kThrewLamePartyBuffMoodRequirement)
                {
                    kNeutralParty = Party.PartyOutcomes.kBadParty;
                }
            }
            return kNeutralParty;
        }

        public override Party.PartyOutcomes GetPartyOutcomeForSim(Sim sim)
        {
            Party.PartyOutcomes kNeutralParty = Party.PartyOutcomes.kNeutralParty;
            int simHostedMood = GetSimHostedMood(sim);
            bool flag = Host.HasTrait(TraitNames.LegendaryHost);
            if ((simHostedMood > HouseParty.kThrewGreatPartyBuffMoodRequirement) || flag)
            {
                return Party.PartyOutcomes.kGoodParty;
            }
            if (simHostedMood < HouseParty.kThrewLamePartyBuffMoodRequirement)
            {
                kNeutralParty = Party.PartyOutcomes.kBadParty;
            }
            return kNeutralParty;
        }

        public override Party.SpecialPartyOutcome GetSpecialPartyOutcome()
        {
            return Party.SpecialPartyOutcome.kNoSpecialFailures;
        }

        public override CommodityKind HostMotive()
        {
            return CommodityKind.BeHostAtParty;
        }

        public override string LeavingMessage(Sim sim)
        {
            int val = RandomUtil.GetInt(0x2);
            switch (GetPartyOutcomeForSim(sim))
            {
                case Party.PartyOutcomes.kBadParty:
                    return Common.LocalizeEAString(sim.IsFemale, "Gameplay/Notifications/Party:GuestBad" + val.ToString(), new object[0x0]);

                case Party.PartyOutcomes.kGoodParty:
                    return Common.LocalizeEAString(sim.IsFemale, "Gameplay/Notifications/Party:GuestGood" + val.ToString(), new object[0x0]);
            }
            return Common.LocalizeEAString(sim.IsFemale, "Gameplay/Notifications/Party:GuestOk" + val.ToString(), new object[0x0]);
        }

        protected void OnGoingHomeBuffCleanup(Sim sim)
        {
            sim.BuffManager.UnpauseBuff(BuffNames.AwesomeParty);
            sim.BuffManager.UnpauseBuff(BuffNames.TheLifeOfTheParty);
            sim.BuffManager.RemoveElement(BuffNames.SweetVenueParty);
            sim.PartyAnimalWooList.Clear();
        }

        public virtual void OnGuestHasArrived(Sim sim)
        {
            if (!SomeGuestsHaveArrived)
            {
                EventTracker.SendEvent(new PartyEvent(EventTypeId.kPartyBegan, Host, Host.SimDescription, this));
            }
            SomeGuestsHaveArrived = true;

            StoryProgression.Main.Situations.GreetSimOnLot(sim.SimDescription, Lot);

            VisitSituation.SetVisitToGreeted(sim);
        }

        public override void OnHappening()
        {
            ActiveTopic.AddToSim(Host, "Party Happening");
        }

        public override void OnPreparation()
        {
            ActiveTopic.AddToSim(Host, "Party Preparation");
        }

        private void OnRouteToPartyFailed(Sim participant, float x)
        {
            if (participant.LotCurrent != Lot)
            {
                if (!participant.IsSelectable)
                {
                    if (!mNumRouteFailures.ContainsKey(participant))
                    {
                        mNumRouteFailures[participant] = 0x1;
                    }
                    else
                    {
                        mNumRouteFailures[participant] += 0x1;
                    }
                    RouteSoon soon = new RouteSoon();
                    soon.Guest = participant;
                    soon.Party = this;
                    participant.AddAlarm(5f, TimeUnit.Minutes, new AlarmTimerCallback(soon.Callback), "Delayed Route to Party", AlarmType.DeleteOnReset);
                }
            }
            else
            {
                OnRouteToPartySucceeded(participant, x);
            }
        }

        public new static Relationship GetRelationshipToHostedSituation(SimDescription hostSim, SimDescription simToCheck)
        {
            Relationship relationship2 = Relationship.Get(hostSim, simToCheck, false);

            float liking2 = 0;
            if (relationship2 != null)
            {
                liking2 = relationship2.LTR.Liking;
            }

            foreach (SimDescription description in Households.Humans(hostSim.Household))
            {
                Relationship relationship = Relationship.Get(description, simToCheck, false);

                float liking = 0;
                if (relationship != null)
                {
                    liking = relationship.LTR.Liking;
                }

                if (liking > liking2)
                {
                    relationship2 = relationship;
                }
            }
            return relationship2;
        }

        public void OnRouteToPartySucceeded(Sim actor, float x)
        {
            if (actor.LotCurrent != Lot)
            {
                OnRouteToPartyFailed(actor, x);
            }
            else if (actor != Host)
            {
                OnGuestHasArrived(actor);
                if (!actor.IsSelectable)
                {
                    actor.Motives.SetMax(CommodityKind.Energy);
                }

                Relationship relationshipToHostedSituation = Relationship.Get(Host.SimDescription, actor.SimDescription, false);

                if (actor.SimDescription.Household != Host.SimDescription.Household)
                {
                    relationshipToHostedSituation = GetRelationshipToHostedSituation(Host.SimDescription, actor.SimDescription);
                }

                float delta = GetParams().MinPartyTime;
                if (relationshipToHostedSituation != null)
                {
                    delta += ((relationshipToHostedSituation.LTR.Liking + 100f) * GetParams().RelToTimeMod);
                }

                mTimeForSimToLeave[actor.ObjectId] = SimClock.Add(SimClock.CurrentTime(), TimeUnit.Hours, delta);
                actor.PlayReaction(ReactionTypes.Wave, Host, ReactionSpeed.CriticalWithRoute);
                if (Child is Start)
                {
                    SetState(new Happening(this));
                }
                else if (Child is Prepare)
                {
                    SetState(new GetDressed(this));
                }
            }
        }

        public override void OnSimGoingHome(Sim sim)
        {
            float num = ApplyLikingChange(sim);
            if (Host.HasTrait(TraitNames.LegendaryHost))
            {
                sim.BuffManager.AddElement(BuffNames.AwesomeParty, Origin.FromParty);
            }

            ThoughtBalloonManager.BalloonData bd = new ThoughtBalloonManager.BalloonData("balloon_partyballoons");
            if (num >= 0f)
            {
                bd.LowAxis = ThoughtBalloonAxis.kLike;
            }
            else
            {
                bd.LowAxis = ThoughtBalloonAxis.kDislike;
            }
            bd.mPriority = ThoughtBalloonPriority.High;
            bd.Duration = ThoughtBalloonDuration.Long;
            bd.mCoolDown = ThoughtBalloonCooldown.Long;
            sim.ThoughtBalloonManager.ShowBalloon(bd);
            OnGoingHomeBuffCleanup(sim);
        }

        private void OnStartedPartyFinishes()
        {
            if (!mbPartyFinishedCalled)
            {
                mbPartyFinishedCalled = true;
                Party.PartyOutcomes partyOutcome = GetPartyOutcome();
                Party.SpecialPartyOutcome specialPartyOutcome = GetSpecialPartyOutcome();
                bool flag = Host.HasTrait(TraitNames.LegendaryHost);
                switch (specialPartyOutcome)
                {
                    case Party.SpecialPartyOutcome.kSpecialConditionsFailed:
                        partyOutcome = Party.PartyOutcomes.kBadParty;
                        break;
                }
                switch (partyOutcome)
                {
                    case Party.PartyOutcomes.kBadParty:
                        Host.BuffManager.AddElement(BuffNames.ThrewLameParty, Origin.FromUnhappyGuests);
                        break;

                    case Party.PartyOutcomes.kGoodParty:
                        Host.BuffManager.AddElement(BuffNames.ThrewAGreatParty, Origin.FromHappyGuests);
                        break;
                }
                Host.BuffManager.UnpauseBuff(BuffNames.TheLifeOfTheParty);
                Host.BuffManager.UnpauseBuff(BuffNames.AwesomeParty);
                Host.PartyAnimalWooList.Clear();
                foreach (Sim sim in OtherHosts)
                {
                    if (flag)
                    {
                        sim.BuffManager.AddElement(BuffNames.AwesomeParty, Origin.FromLegendaryHost);
                    }
                    sim.BuffManager.UnpauseBuff(BuffNames.TheLifeOfTheParty);
                    sim.BuffManager.UnpauseBuff(BuffNames.AwesomeParty);
                    sim.PartyAnimalWooList.Clear();
                }
                ActiveTopic.AddToSim(Host, "Party Over");
                EventTracker.SendEvent(new PartyEvent(EventTypeId.kPartyOver, Host, Host.SimDescription, this));
                if (specialPartyOutcome == Party.SpecialPartyOutcome.kSpecialConditionsFailed)
                {
                    ShowSpecialFailureTNS();
                }
                else
                {
                    ShowPartyEndTNS(partyOutcome);
                }
            }
        }

        public override CommodityKind PreparationMotive()
        {
            return CommodityKind.PrepareForParty;
        }

        private void Pulse()
        {
            try
            {
                if (SomeGuestsHaveArrived)
                {
                    CheckIfGuestsNeedToGoHome();
                }
                if (SimClock.CurrentTime().Ticks >= EarliestPossibleEndTime.Ticks)
                {
                    if (SomeGuestsHaveArrived)
                    {
                        int num = 0x0;
                        for (int i = Guests.Count - 0x1; i >= 0x0; i--)
                        {
                            Sim sim = Guests[i];
                            if ((Host != null) && (sim.Household == Host.Household))
                            {
                                RemoveGuest(sim, false);
                                SimsWhoLeftParty.Remove(sim);
                                OtherHosts.Add(sim);
                            }
                            else if (sim.LotCurrent == Lot)
                            {
                                num++;
                            }
                        }
                        foreach (Sim sim2 in SimsWhoLeftParty)
                        {
                            DateAndTime time;
                            if ((mTimeForSimToLeave.TryGetValue(sim2.ObjectId, out time) && (sim2.LotCurrent == Lot)) && (SimClock.ElapsedTime(TimeUnit.Hours, time) <= HostedSituation.kTimeToLeaveHostedSituation))
                            {
                                num++;
                            }
                        }
                        if (num == 0x0)
                        {
                            SetState(new AllGuestsHaveLeft(this));
                        }
                        if (Host.LotCurrent != Lot)
                        {
                            int num3 = 0x0;
                            foreach (Sim sim3 in OtherHosts)
                            {
                                if (sim3.LotCurrent == Lot)
                                {
                                    num3++;
                                }
                            }
                            if (num3 == 0x0)
                            {
                                SetState(new AllHostsHaveLeft(this));
                            }
                        }
                    }
                    else
                    {
                        if (Host.IsSelectable)
                        {
                            string titleText = Common.LocalizeEAString(Host.SimDescription.IsFemale, "Gameplay/Situations/Party:PartyNoShow", new object[] { Host.SimDescription });
                            StyledNotification.Format format = new StyledNotification.Format(titleText, ObjectGuid.InvalidObjectGuid, Host.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                            StyledNotification.Show(format, "w_party");
                        }
                        Exit();
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception)
            {
                Exit();
            }
        }

        public void PushComeToParty(Sim participant)
        {
            float partyAnimalTraitIncreasedChanceGuestFood = 0f;

            if (!StoryProgression.Main.Situations.Allow(StoryProgression.Main.Situations, participant.SimDescription)) return;

            if (!StoryProgression.Main.GetValue<AllowPushPartyOption, bool>(participant.SimDescription)) return;

            if (!StoryProgression.Main.Situations.IsBusy(StoryProgression.Main.Situations, participant, true)) return;

            if (Lot.IsResidentialLot)
            {
                if (Host.TraitManager.HasElement(TraitNames.PartyAnimal))
                {
                    partyAnimalTraitIncreasedChanceGuestFood = TraitTuning.PartyAnimalTraitIncreasedChanceGuestFood;
                }

                if (!participant.IsSelectable && RandomUtil.RandomChance(GetParams().ChanceOfBringingFood + partyAnimalTraitIncreasedChanceGuestFood))
                {
                    Recipe.CreateRandomGroupMealAndPutInInventory(participant);
                    ForceSituationSpecificInteraction(Lot, participant, TakeFoodToLot.Singleton, null, new Callback(OnRouteToPartySucceeded), new Callback(OnRouteToPartyFailed));
                }
                else
                {
                    ForceSituationSpecificInteraction(Lot, participant, GoToLot.Singleton, null, new Callback(OnRouteToPartySucceeded), new Callback(OnRouteToPartyFailed));
                }
            }
            else
            {
                ForceSituationSpecificInteraction(Lot, participant, GoToCommunityLot.Singleton, null, new Callback(OnRouteToPartySucceeded), new Callback(OnRouteToPartyFailed));
            }
        }

        public override void RemoveGuest(Sim sim, bool bLeftEarly)
        {
            base.RemoveGuest(sim, bLeftEarly);
            mNumberOfGuests++;
            mPartyMood += GetSimHostedMood(sim);
        }

        public virtual void ShowPartyEndTNS(Party.PartyOutcomes partyResult)
        {
            if (StoryProgression.Main.Situations.MatchesAlertLevel(Host))
            {
                string str;
                switch (partyResult)
                {
                    case Party.PartyOutcomes.kBadParty:
                        str = Common.LocalizeEAString(Host.IsFemale, "Gameplay/Notifications/Party:BadParty", new object[] { Host.SimDescription });
                        break;

                    case Party.PartyOutcomes.kGoodParty:
                        str = Common.LocalizeEAString(Host.IsFemale, "Gameplay/Notifications/Party:GoodParty", new object[] { Host.SimDescription });
                        break;

                    default:
                        str = Common.LocalizeEAString(Host.IsFemale, "Gameplay/Notifications/Party:NormalParty", new object[] { Host.SimDescription });
                        break;
                }
                StyledNotification.Format format = new StyledNotification.Format(str, ObjectGuid.InvalidObjectGuid, Host.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                StyledNotification.Show(format, "w_party");
            }
        }

        public override void ShowSpecialFailureTNS()
        { }

        public override bool AllowPartyAnimalCheer
        {
            get
            {
                return true;
            }
        }

        public static PartyParams HousePartyParams
        {
            get
            {
                return HouseParty.kHousePartyParams;
            }
        }

        public override bool IsGreatParty
        {
            get
            {
                return (GetPartyOutcome() == Party.PartyOutcomes.kGoodParty);
            }
        }

        public class AllGuestsHaveLeft : ChildSituation<HousePartySituation>
        {
            protected AllGuestsHaveLeft()
            { }

            public AllGuestsHaveLeft(HousePartySituation parent)
                : base(parent)
            { }

            public override void Init(HousePartySituation parent)
            {
                parent.OnStartedPartyFinishes();
                Exit();
            }
        }

        public class AllHostsHaveLeft : ChildSituation<HousePartySituation>
        {
            protected AllHostsHaveLeft()
            { }
            public AllHostsHaveLeft(HousePartySituation parent)
                : base(parent)
            { }

            public override void Init(HousePartySituation parent)
            {
                foreach (Sim sim in parent.Guests)
                {
                    if (sim.LotCurrent == parent.Lot)
                    {
                        Sim.MakeSimGoHome(sim, false);
                    }
                }

                parent.OnStartedPartyFinishes();
                Exit();
            }
        }

        public class DeliverInvitations : ChildSituation<HousePartySituation>
        {
            // Fields
            private AlarmHandle mAlarmHandle;
            private HouseParty.PartyInvitationKeys mInvitationKeys;

            // Methods
            protected DeliverInvitations()
            {
            }

            public DeliverInvitations(HousePartySituation parent, HouseParty.PartyInvitationKeys invitationKeys) 
                : base(parent)
            {
                mInvitationKeys = invitationKeys;
            }

            private void CallToInviteGuest(Sim host, Sim guest, DateAndTime startTime)
            {
                InvitationPhoneCall call = new InvitationPhoneCall(host, guest, startTime, mInvitationKeys);
                PhoneService.PlaceCall(call, 30f);
            }

            public override void CleanUp()
            {
                AlarmManager.RemoveAlarm(mAlarmHandle);
                base.CleanUp();
            }

            private static float HowLongUntilDeliveringInvitations(float howLongUntilPreparations)
            {
                float num2;
                float num3;
                float hour = SimClock.CurrentTime().Hour;
                if (hour > PhoneService.HourItsTooLateToCall)
                {
                    num2 = (PhoneService.HourItsOkayToCall - hour) + 24f;
                    num3 = (PhoneService.HourItsTooLateToCall - hour) + 24f;
                }
                else if (hour < PhoneService.HourItsOkayToCall)
                {
                    num2 = PhoneService.HourItsOkayToCall - hour;
                    num3 = PhoneService.HourItsTooLateToCall - hour;
                }
                else
                {
                    num2 = 0f;
                    num3 = PhoneService.HourItsTooLateToCall - hour;
                }
                if (num3 > howLongUntilPreparations)
                {
                    num3 = howLongUntilPreparations;
                }
                if (num2 > howLongUntilPreparations)
                {
                    num2 = howLongUntilPreparations;
                }
                return RandomUtil.GetFloat(num2, num3);
            }

            public override void Init(HousePartySituation parent)
            {
                float howLongUntilPreparations = SimClock.ElapsedTime(TimeUnit.Hours, SimClock.CurrentTime(), Parent.StartTime) - parent.GetParams().PreparationTime;
                float time = HowLongUntilDeliveringInvitations(howLongUntilPreparations);
                mAlarmHandle = AlarmManager.AddAlarm(time, TimeUnit.Hours, TimeToDeliverInvitations, "Deliver Invitations", AlarmType.AlwaysPersisted, Parent.Host);
            }

            private void TimeToDeliverInvitations()
            {
                foreach (Sim sim in Parent.Guests)
                {
                    if (sim.IsSelectable)
                    {
                        CallToInviteGuest(Parent.Host, sim, Parent.StartTime);
                        break;
                    }
                }
                Parent.SetState(new WaitForPreparations(Parent));
            }

            private class InvitationPhoneCall : SimPhoneCall
            {
                private ulong mHostDescriptionId;
                private HouseParty.PartyInvitationKeys mInvitationKeys;
                private DateAndTime mStartTime;

                protected InvitationPhoneCall()
                { }

                public InvitationPhoneCall(Sim host, Sim sim, DateAndTime startTime, HouseParty.PartyInvitationKeys invitationKeys) : base(sim)
                {
                    mHostDescriptionId = host.SimDescription.SimDescriptionId;
                    mStartTime = startTime;
                    mInvitationKeys = invitationKeys;
                }

                public override PhoneCall.AnswerType GetAnswerType()
                {
                    return PhoneCall.AnswerType.BeQueried;
                }

                public override PhoneCall.QueryResponse GetQueryResponse(Sim sim)
                {
                    if (mInvitationKeys == null)
                    {
                        HouseParty.PartyInvitationKeys kDefaultPartyKeys = HouseParty.kDefaultPartyKeys;
                    }
                    SimDescription description = ManagerSim.Find(mHostDescriptionId);
                    Sim sim2 = (description != null) ? description.CreatedSim : null;
                    if (sim2 == null)
                    {
                        return PhoneCall.QueryResponse.JustHangUp;
                    }
                    if (!TwoButtonDialog.Show(Common.LocalizeEAString(sim.IsFemale, mInvitationKeys.InvitationKey, new object[] { sim, sim2, mStartTime }), Common.LocalizeEAString(mInvitationKeys.AcceptKey), Common.LocalizeEAString(mInvitationKeys.RejectKey)))
                    {
                        return PhoneCall.QueryResponse.RespondNegatively;
                    }
                    return PhoneCall.QueryResponse.RespondPositively;
                }
            }
        }

        public class GetDressed : ChildSituation<HousePartySituation>
        {
            protected GetDressed()
            { }
            public GetDressed(HousePartySituation parent)
                : base(parent)
            { }

            public override void Init(HousePartySituation parent)
            {
                parent.Host.PushSwitchToOutfitInteraction(Sim.ClothesChangeReason.GoingToSituation, parent.ClothingStyle);
                foreach (Sim sim in parent.OtherHosts)
                {
                    sim.PushSwitchToOutfitInteraction(Sim.ClothesChangeReason.GoingToSituation, parent.ClothingStyle);
                }
                Parent.SetState(new Start(Parent));
            }
        }

        public class Happening : ChildSituation<HousePartySituation>
        {
            private Dictionary<Sim, CommodityKind> partyMotives;

            protected Happening()
            {
                partyMotives = new Dictionary<Sim, CommodityKind>();
            }

            public Happening(HousePartySituation parent)
                : base(parent)
            {
                partyMotives = new Dictionary<Sim, CommodityKind>();
            }

            private void AddBeAtPartyMotive(Sim sim)
            {
                CommodityKind commodity = Parent.BeAtHostedSituationMotive(sim);
                sim.Motives.CreateMotive(commodity);
                partyMotives[sim] = commodity;
            }

            public override void CleanUp()
            {
                CommodityKind m = Parent.HostMotive();

                if (Parent.Host != null)
                {
                    if (Parent.Host.Motives != null)
                    {
                        Parent.Host.Motives.RemoveMotive(m);
                    }

                    RemoveBeAtPartyMotive(Parent.Host);
                }

                if (Parent.OtherHosts != null)
                {
                    foreach (Sim sim in Parent.OtherHosts)
                    {
                        if (sim.Motives != null)
                        {
                            sim.Motives.RemoveMotive(m);
                        }
                        RemoveBeAtPartyMotive(sim);
                    }
                }

                if (Parent.Guests != null)
                {
                    foreach (Sim sim2 in Parent.Guests)
                    {
                        RemoveBeAtPartyMotive(sim2);
                    }
                }

                base.CleanUp();
            }

            public override void Init(HousePartySituation parent)
            {
                CommodityKind commodity = parent.HostMotive();
                parent.OnHappening();
                parent.Host.Motives.CreateMotive(commodity);
                AddBeAtPartyMotive(parent.Host);
                Situation.CancelAutonomousInteractions(parent.Host);
                Lot lot = parent.Lot;
                Lot lotHome = parent.Host.LotHome;
                foreach (Sim sim in parent.OtherHosts)
                {
                    Situation.CancelAutonomousInteractions(sim);
                    sim.Motives.CreateMotive(commodity);
                    AddBeAtPartyMotive(sim);
                }

                foreach (Sim sim2 in parent.Guests)
                {
                    AddBeAtPartyMotive(sim2);
                }

                if (StoryProgression.Main.Situations.MatchesAlertLevel(parent.Host))
                {
                    string titleText = Common.LocalizeEAString(parent.Host.SimDescription.IsFemale, "Gameplay/Situations/Party:PartyBeginsTNS", new object[] { parent.Host.SimDescription });
                    StyledNotification.Format format = new StyledNotification.Format(titleText, ObjectGuid.InvalidObjectGuid, parent.Host.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                    StyledNotification.Show(format, "w_party");
                }
            }

            private void RemoveBeAtPartyMotive(Sim sim)
            {
                CommodityKind none = CommodityKind.None;
                partyMotives.TryGetValue(sim, out none);
                if (none != CommodityKind.None)
                {
                    sim.Motives.RemoveMotive(none);
                }
            }
        }

        public delegate void HousePartyCallback();

        public class Prepare : ChildSituation<HousePartySituation>
        {
            private AlarmHandle mAlarmGuestInvite;
            private AlarmHandle mAlarmHandle;

            protected Prepare()
            { }

            public Prepare(HousePartySituation parent)
                : base(parent)
            { }

            public override void CleanUp()
            {
                Parent.Host.Motives.RemoveMotive(Parent.PreparationMotive());
                AlarmManager.RemoveAlarm(mAlarmHandle);
                AlarmManager.RemoveAlarm(mAlarmGuestInvite);
                base.CleanUp();
            }

            public override void Init(HousePartySituation parent)
            {
                if (parent.Host.IsSelectable)
                {
                    string titleText = Common.LocalizeEAString(parent.Host.SimDescription.IsFemale, "Gameplay/Situations/HouseParty:GoPrepare", new object[] { parent.Host.SimDescription });
                    if (parent.Lot != parent.Host.LotHome)
                    {
                        titleText = titleText + Common.NewLine + Common.NewLine + Common.LocalizeEAString(false, "Gameplay/Situations/HouseParty:Venue", new object[] { parent.Lot.Name });
                    }
                    StyledNotification.Show(new StyledNotification.Format(titleText, ObjectGuid.InvalidObjectGuid, parent.Host.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive), "w_party");
                }
                if (!parent.Host.IsSelectable && (parent.Host.LotCurrent != parent.Lot))
                {
                    ForceSituationSpecificInteraction(parent.Lot, parent.Host, GoToLot.Singleton, null, null, null);
                }
                parent.Host.Motives.CreateMotive(parent.PreparationMotive());
                parent.OnPreparation();
                float time = SimClock.ElapsedTime(TimeUnit.Hours, SimClock.CurrentTime(), Parent.StartTime);
                float num2 = time - parent.GetParams().HoursBeforePartyToInvite;
                if (num2 <= 0f)
                {
                    TimeToInviteGuests();
                }
                else
                {
                    mAlarmGuestInvite = AlarmManager.AddAlarm(num2, TimeUnit.Hours, TimeToInviteGuests, "Invite Guests To Party", AlarmType.AlwaysPersisted, Parent.Host);
                }
                mAlarmHandle = AlarmManager.AddAlarm(time, TimeUnit.Hours, TimeToStart, "Waiting for Party to start", AlarmType.AlwaysPersisted, Parent.Host);
            }

            private void TimeToInviteGuests()
            {
                Sim host = Parent.Host;
                foreach (SimDescription description in Parent.GuestDescriptions)
                {
                    if (host != null)
                    {
                        Sim createdSim = description.CreatedSim;
                        if ((createdSim == null) && (!SimTypes.IsDead(description)) && (!SimTypes.IsSelectable(description)))
                        {
                            createdSim = Instantiation.PerformOffLot(description, host.LotHome, null);
                        }

                        if (createdSim != null)
                        {
                            Parent.Guests.Add(createdSim);
                            createdSim.AssignRole(Parent);
                        }
                    }
                }

                ManagerSituation situations = StoryProgression.Main.Situations;

                foreach (Sim sim3 in Parent.Guests)
                {
                    if (!situations.IsBusy(situations, sim3, true)) continue;

                    if (StoryProgression.Main.Situations.Allow(StoryProgression.Main.Situations, sim3.SimDescription))
                    {
                        sim3.PushSwitchToOutfitInteraction(Sim.ClothesChangeReason.GoingToSituation, Parent.ClothingStyle);
                    }

                    if (sim3.LotCurrent != Parent.Lot)
                    {
                        Parent.PushComeToParty(sim3);
                    }
                    else
                    {
                        Parent.OnRouteToPartySucceeded(sim3, 0f);
                    }
                }

                if (Parent.Lot != Parent.Host.LotCurrent)
                {
                    Parent.PushComeToParty(host);
                }
            }

            private void TimeToStart()
            {
                Parent.SetState(new GetDressed(Parent));
            }
        }

        [Persistable]
        private class RouteSoon
        {
            public Sim Guest;
            public HousePartySituation Party;

            public void Callback()
            {
                if (Situation.sAllSituations.Contains(Party))
                {
                    if (Party.mNumRouteFailures[Guest] < HostedSituation.kMaxNumChancesToGetToLot)
                    {
                        Party.PushComeToParty(Guest);
                    }
                    else
                    {
                        Party.Guests.Remove(Guest);
                        Guest.RemoveRole(Party);
                        if (Guest.LotHome == null)
                        {
                            Sim.MakeSimGoHome(Guest, false);
                        }
                    }
                }
            }
        }

        public class Start : ChildSituation<HousePartySituation>
        {
            protected Start()
            { }
            public Start(HousePartySituation parent)
                : base(parent)
            { }

            public override void CleanUp()
            {
                Parent.Host.Motives.RemoveMotive(Parent.PreparationMotive());
                base.CleanUp();
            }

            public override void Init(HousePartySituation parent)
            {
                parent.Host.Motives.CreateMotive(parent.PreparationMotive());
                parent.IsStarted = true;
                foreach (Sim sim in parent.Guests)
                {
                    if (sim.IsSelectable)
                    {
                        sim.SocialComponent.AddShortTermDesireToSocializeWith(parent.Host, parent.GetParams().InitialDesireToSocialize);
                    }
                }
                if (parent.SomeGuestsHaveArrived)
                {
                    parent.SetState(new Happening(parent));
                }
            }
        }

        public class WaitForPreparations : ChildSituation<HousePartySituation>
        {
            private AlarmHandle mAlarmHandle;

            protected WaitForPreparations()
            { }
            public WaitForPreparations(HousePartySituation parent)
                : base(parent)
            { }

            public override void CleanUp()
            {
                AlarmManager.RemoveAlarm(mAlarmHandle);
                base.CleanUp();
            }

            public override void Init(HousePartySituation parent)
            {
                float time = SimClock.ElapsedTime(TimeUnit.Hours, SimClock.CurrentTime(), Parent.StartTime) - parent.GetParams().PreparationTime;
                mAlarmHandle = AlarmManager.AddAlarm(time, TimeUnit.Hours, TimeToPrepare, "Prepare Party", AlarmType.AlwaysPersisted, Parent.Host);
            }

            private void TimeToPrepare()
            {
                Parent.SetState(new Prepare(Parent));
            }
        }
    }
}

