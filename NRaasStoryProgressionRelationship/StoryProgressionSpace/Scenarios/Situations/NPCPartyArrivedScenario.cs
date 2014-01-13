using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class NPCPartyArriveScenario : SimEventScenario<Event>
    {
        public NPCPartyArriveScenario()
        { }
        protected NPCPartyArriveScenario(NPCPartyArriveScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "NPCPartyArrive";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kPartyArrivedGuest);
        }

        protected override Scenario Handle(Event paramE, ref ListenerAction result)
        {
            NPCPartyArriveScenario scenario = base.Handle(paramE, ref result) as NPCPartyArriveScenario;
            if (scenario == null) return null;

            scenario.PrivateUpdate(null);
            return null;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim host = Event.TargetObject as Sim;

            foreach (Situation situation in Situation.sAllSituations)
            {
                NpcParty party = situation as NpcParty;
                if (party == null) continue;

                if (party.Host != host) continue;

                NpcParty.WaitForSelectableGuestToArrive child = party.Child as NpcParty.WaitForSelectableGuestToArrive;
                if (child == null)
                {
                    IncStat("Wrong Child Situation");
                    continue;
                }

                // Doing so stops the game from teleporting the other guests onto the lot
                child.selectableSimEnteredLot = true;

                new PopulatePartyTask(this, party).AddToSimulator();

                IncStat("Party Altered");
                return true;
            }

            IncStat("Find Fail");
            return false;
        }

        public override Scenario Clone()
        {
            return new NPCPartyArriveScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSituation, NPCPartyArriveScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "NPCPartyArrive";
            }
        }

        public class PopulatePartyTask : Common.FunctionTask
        {
            NpcParty mParty;

            NPCPartyArriveScenario mScenario;

            public PopulatePartyTask(NPCPartyArriveScenario scenario, NpcParty party)
            {
                mScenario = scenario;
                mParty = party;
            }

            protected override void OnPerform()
            {
                mScenario.IncStat("PopulatePartyTask");

                mParty.mPlaceNpcGuestsFunction = null;
                mParty.SomeGuestsHaveArrived = true;

                Lot lotHome = mParty.Host.LotHome;

                GatheringScenario.PushBuffetInteractions(mScenario, mParty.Host.SimDescription, lotHome);

                List<Sim> sims = new List<Sim>(HouseholdsEx.AllSims(lotHome.Household));

                SimDescription host = mParty.Host.SimDescription;
                foreach (SimDescription sim in mParty.GuestDescriptions)
                {
                    if (!NpcParty.NpcGuestTest(sim, host)) continue;

                    if (!mScenario.Situations.Allow(mScenario, sim))
                    {
                        mScenario.IncStat("NpcParty Push User Denied");
                        continue;
                    }

                    Sim createdSim = sim.CreatedSim;
                    if (createdSim == null)
                    {
                        createdSim = Instantiation.PerformOffLot(sim, lotHome, null);
                    }

                    if (createdSim != null)
                    {
                        if (createdSim.LotCurrent != lotHome)
                        {
                            if (!mScenario.Situations.PushVisit(mScenario, sim, lotHome))
                            {
                                mScenario.IncStat("NpcParty Push Fail");
                                continue;
                            }
                        }

                        mParty.Guests.Add(createdSim);
                        createdSim.AssignRole(mParty);

                        VisitSituation.SetVisitToGreeted(createdSim);

                        sims.Add(createdSim);
                    }
                }

                foreach (Sim sim in sims)
                {
                    if (sim.LotCurrent == lotHome)
                    {
                        sim.Motives.SetMax(CommodityKind.Energy);
                        sim.Motives.SetMax(CommodityKind.Hygiene);

                        sim.PushSwitchToOutfitInteraction(Sims3.Gameplay.Actors.Sim.ClothesChangeReason.Force, mParty.ClothingStyle);
                    }
                }

                EventTracker.SendEvent(new PartyEvent(EventTypeId.kPartyBegan, mParty.Host, host, mParty));

                mParty.SetState(new NpcParty.Happening(mParty));
            }
        }
    }
}
