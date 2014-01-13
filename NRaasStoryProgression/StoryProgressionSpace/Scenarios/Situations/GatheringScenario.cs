using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public abstract class GatheringScenario : DualSimScenario
    {
        Lot mLot = null;

        List<SimDescription> mGuests = new List<SimDescription>();

        bool mReport = true;

        public GatheringScenario()
        { }
        protected GatheringScenario(Lot lot)
        {
            mLot = lot;
        }
        protected GatheringScenario(SimDescription sim, Lot lot)
            : base (sim)
        {
            mLot = lot;
        }
        protected GatheringScenario(GatheringScenario scenario)
            : base (scenario)
        {
            mLot = scenario.Lot;
            mReport = false;
            //mGuests.AddRange(scenario.mGuests);
        }

        protected override int TargetContinueChance
        {
            get { return 100; }
        }

        protected virtual int TargetMinimum
        {
            get { return 3; }
        }

        protected override int TargetMaximum
        {
            get { return GetValue<MaxGatherOption,int>(); }
        }

        protected override bool TargetAllowActive
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool ShouldReport
        {
            get { return false; }
        }

        protected override bool TestOpposing
        {
            get { return true; }
        }

        protected abstract OutfitCategories PartyAttire
        {
            get;
        }

        protected virtual int ChanceOfHomeLot
        {
            get { return 50; }
        }

        protected List<SimDescription> Guests
        {
            get { return mGuests; }
        }

        protected Lot Lot
        {
            get { return mLot; }
            set { mLot = value; }
        }

        protected bool IsHomeParty
        {
            get { return (Sim.LotHome == Lot); }
        }

        protected override bool Allow()
        {
            if (SimClock.CurrentTime().Hour <= 6)
            {
                IncStat("Too Early");
                return false;
            }
            else if (SimClock.CurrentTime().Hour >= 21)
            {
                IncStat("Too Late");
                return false;
            }

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (!GetValue<AllowPushPartyOption, bool>(sim))
            {
                IncStat("Party Denied");
                return false;
            }

 	        return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Situations.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }

            return base.Allow(sim);
        }

        protected override GatherResult TargetGather(List<Scenario> list, ref bool random)
        {
            if (Target == null)
            {
                if (Lot == null)
                {
                    if (RandomUtil.RandomChance(ChanceOfHomeLot))
                    {
                        Lot = Sim.LotHome;
                    }

                    if (Lot == null)
                    {
                        Lot = Lots.GetCommunityLot(Sim.CreatedSim, null, false);

                        if (Lot == null)
                        {
                            return GatherResult.Failure;
                        }
                    }
                }
            }

            GatherResult result = base.TargetGather(list, ref random);
            if (result == GatherResult.Success)
            {
                RandomUtil.RandomizeListOfObjects(list);

                int count = 0;
                foreach(Scenario choice in list)
                {
                    GatheringScenario guest = choice as GatheringScenario;
                    if (guest == null) continue;

                    count++;
                    mGuests.Add(guest.Target);

                    if (count >= TargetMaximum)
                    {
                        break;
                    }
                }

                list.Clear();
                return GatherResult.Update;
            }

            return result;
        }

        public static void PushBuffetInteractions(IScoringGenerator stats, SimDescription sim, Lot lot)
        {
            foreach (BuffetTable table in lot.GetObjects<BuffetTable>())
            {
                NRaas.StoryProgression.Main.Situations.PushInteraction(stats, sim, table, BuffetTable.Serve.Singleton);
            }
        }

        protected virtual Party GetParty(Lot lot, Sim host, List<SimDescription> guests, OutfitCategories partyAttire, DateAndTime startTime)
        {
            return new HousePartySituation(lot, host, guests, partyAttire, startTime);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Guests.Count < TargetMinimum)
            {
                IncStat("Too Few");
                return false;
            }

            AddStat("Guests", Guests.Count);

            if (Household.ActiveHousehold != null)
            {
                foreach (SimDescription active in HouseholdsEx.Humans(Household.ActiveHousehold))
                {
                    Target = active;
                    if (!TargetAllow(active)) continue;

                    if (mGuests.Contains(active)) continue;

                    if (ManagerFriendship.AreFriends(Sim, active))
                    {
                        mGuests.Add(active);
                    }
                }

                Target = null;
            }

            int delay = 3;

            if (Lot == Sim.LotHome)
            {
                Situations.PushGoHome(this, Sim);

                PushBuffetInteractions(this, Sim, Lot);

                DateAndTime startTime = SimClock.CurrentTime();
                startTime.Ticks += SimClock.ConvertToTicks(3f, TimeUnit.Hours);

                /*
                if (Lot != Sim.LotHome)
                {
                    DateAndTime rentTime = startTime;
                    rentTime.Ticks -= SimClock.ConvertToTicks(Sims3.Gameplay.Situations.Party.HoursToStartRentBeforePartyStart, TimeUnit.Hours);
                    if (rentTime.CompareTo(SimClock.CurrentTime()) < 0)
                    {
                        rentTime = SimClock.Add(SimClock.CurrentTime(), TimeUnit.Minutes, 2f);
                    }

                    if (!RentScheduler.Instance.RentLot(Lot, Sim.CreatedSim, rentTime, Guests))
                    {
                        IncStat("Couldn't Rent");
                        Lot = Sim.LotHome;
                    }
                }
                */

                Party party = GetParty(Lot, Sim.CreatedSim, Guests, PartyAttire, startTime);

                EventTracker.SendEvent(new PartyEvent(EventTypeId.kThrewParty, Sim.CreatedSim, Sim, party));

                delay = 3;
            }
            else
            {
                List<Sim> followers = new List<Sim>();

                foreach (SimDescription guest in Guests)
                {
                    if (SimTypes.IsSelectable(guest)) continue;

                    if (!Sims.Instantiate(guest, Lot, false)) continue;

                    Sim guestSim = guest.CreatedSim;
                    if (guestSim == null) continue;

                    guestSim.PushSwitchToOutfitInteraction(Sims3.Gameplay.Actors.Sim.ClothesChangeReason.GoingToSituation, PartyAttire);

                    followers.Add(guestSim);
                }

                AddStat("Followers", followers.Count);

                if (!Situations.PushMassVisit(this, Sim, followers, Lot)) return false;

                delay = 0;
            }

            if (mReport)
            {
                Manager.AddAlarm(new DelayedStoryScenario(this, delay));
            }

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, Lot };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        protected class DelayedStoryScenario : ScheduledSoloScenario, IAlarmScenario
        {
            GatheringScenario mScenario;

            int mDelay = 3;

            public DelayedStoryScenario(GatheringScenario scenario, int delay)
            {
                mScenario = scenario;
                mDelay = delay;
            }
            protected DelayedStoryScenario(DelayedStoryScenario scenario)
                : base(scenario)
            {
                mScenario = scenario.mScenario;
                mDelay = scenario.mDelay;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "DelayedGatheringStory";
                }
                else
                {
                    return mScenario.GetTitlePrefix(type);
                }
            }

            protected override bool Progressed
            {
                get { return true; }
            }

            public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
            {
                return alarms.AddAlarm(this, mDelay);
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return true;
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                return mScenario.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new DelayedStoryScenario(this);
            }
        }

        public class MaxGatherOption : IntegerManagerOptionItem<ManagerSituation>, ManagerSituation.IGatheringOption
        {
            public MaxGatherOption()
                : base(PartyPickerDialog.kDefaultMaxAllowed)
            { }

            public override string GetTitlePrefix()
            {
                return "MaxGather";
            }
        }
    }
}
