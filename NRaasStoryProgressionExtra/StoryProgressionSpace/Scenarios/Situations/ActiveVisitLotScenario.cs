using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class ActiveVisitLotScenario : SimScenario
    {
        public ActiveVisitLotScenario()
        { }
        public ActiveVisitLotScenario(SimDescription sim)
            : base(sim)
        { }
        protected ActiveVisitLotScenario(ActiveVisitLotScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ActiveVisitLot";
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override int MaximumReschedules
        {
            get { return 4; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return HouseholdsEx.All(Household.ActiveHousehold);
        }

        protected override bool Allow()
        {
            if (!RandomUtil.RandomChance(GetValue<Option,int>())) return false;

            if (Household.ActiveHousehold == null) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (Party.IsGuestOrHostAtAParty(sim.CreatedSim))
            {
                IncStat("At Party");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override bool Push()
        {
            Lot lot = Lots.GetCommunityLot(Sim.CreatedSim, null, true);
            if (lot == null) return false;

            Sim.CreatedSim.InteractionQueue.CancelAutonomousInteractions();

            return Situations.PushVisit(this, Sim, lot);
        }

        public override Scenario Clone()
        {
            return new ActiveVisitLotScenario(this);
        }

        // Attached to ManagerSim to make use of the faster cycling
        public class Option : IntegerScenarioOptionItem<ManagerSim, ActiveVisitLotScenario>
        {
            public Option()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "ActiveVisitLot";
            }
        }
    }
}
