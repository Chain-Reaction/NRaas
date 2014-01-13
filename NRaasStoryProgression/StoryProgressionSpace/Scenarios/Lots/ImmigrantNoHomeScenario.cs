using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ImmigrantNoHomeScenario : ScheduledSoloScenario
    {
        public ImmigrantNoHomeScenario()
        { }
        protected ImmigrantNoHomeScenario(ImmigrantNoHomeScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure)
            {
                if (!ImmigrateScenario.TestEmptyHomes(this))
                {
                    return "NoEmptyHomes";
                }
            }

            return "NoImmigrantHome";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0)
            {
                IncStat("Gauge Fail");
                return false;
            }
            else if (ImmigrateScenario.TestEmptyHomes(this))
            {
                if (Lots.FindLot(this, null, 0, ManagerLot.FindLotFlags.CheapestHome | ManagerLot.FindLotFlags.Inspect, null) != null)
                {
                    IncStat("Lot Found");
                    return false;
                }
            }

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new ImmigrantNoHomeScenario(this);
        }
    }
}
