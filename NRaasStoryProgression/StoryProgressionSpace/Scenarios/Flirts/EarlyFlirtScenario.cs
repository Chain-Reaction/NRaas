using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Flirts
{
    public class EarlyFlirtScenario : FlirtBaseScenario
    {
        public EarlyFlirtScenario(SimDescription sim, SimDescription target, string storyPrefix, bool report)
            : base (sim, target, storyPrefix, report)
        { }
        protected EarlyFlirtScenario(EarlyFlirtScenario scenario)
            : base (scenario)
        { }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (ManagerSim.GetLTR(Sim, Target) > Sims3.Gameplay.Actors.Sim.kRomanceUseLikingGate)
            {
                IncStat("Too High");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Flirts.FindAnyFor(this, sim, false, false);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (SimID.Matches(Flirts.PreviousLoveLoss, Sim))
            {
                Flirts.PreviousLoveLoss = null;
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new EarlyFlirtScenario(this);
        }
    }
}
