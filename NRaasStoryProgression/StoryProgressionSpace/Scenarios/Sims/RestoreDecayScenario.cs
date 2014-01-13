using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class RestoreDecayScenario : SimScenario
    {
        public RestoreDecayScenario()
            : base ()
        { }
        protected RestoreDecayScenario(RestoreDecayScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RestoreDecay";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            if (Household.ActiveHousehold == null) return null;

            return HouseholdsEx.All(Household.ActiveHousehold);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hiberating");
                return false;
            }
            else if (Sim.CreatedSim.Motives == null)
            {
                IncStat("No Motives");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            foreach (Motive motive in Sim.CreatedSim.Motives.AllMotives)
            {
                if (motive.mInstanceHasDecay)
                {
                    IncStat("Unnecessary");
                    return false;
                }
            }

            Sim.CreatedSim.Motives.RestoreDecays();
            return true;
        }

        public override Scenario Clone()
        {
            return new RestoreDecayScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, RestoreDecayScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RestoreDecay";
            }
        }
    }
}
