using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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
    public class NewFlirtScenario : FlirtBaseScenario
    {
        public NewFlirtScenario(SimDescription sim, SimDescription target, string storyPrefix, bool report)
            : base(sim, target, storyPrefix, report)
        { }
        protected NewFlirtScenario(NewFlirtScenario scenario)
            : base (scenario)
        { }

        protected override bool TargetAllow(SimDescription sim)
        {
            Relationship relationship = ManagerSim.GetRelationship(Sim, Target);
            if (relationship == null)
            {
                IncStat("Bad Relation");
                return false;
            }
            else if (relationship.AreRomantic())
            {
                IncStat("Already Romantic");
                return false;
            }
            else if (Sim.Partner == Target)
            {
                IncStat("Partnered");
                return false;
            }

            LongTermRelationship LTR = relationship.LTR;
            if (LTR == null)
            {
                IncStat("Bad LTR");
                return false;
            }
            else if (LTR.Liking <= Sims3.Gameplay.Actors.Sim.kRomanceUseLikingGate)
            {
                IncStat("Too Low");
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
            Relationship relationship = ManagerSim.GetRelationship(Sim, Target);
            if (relationship == null) return false;

            LongTermRelationship LTR = relationship.LTR;
            if (LTR == null) return false;

            Sim.SetFirstKiss(Target);
            Sim.SetFirstRomance(Target);

            SetElapsedTime<DayOfLastFlirtOption>(Sim);
            SetElapsedTime<DayOfLastFlirtOption>(Target);

            if ((Sim.CreatedSim != null) && (Target.CreatedSim != null))
            {
                EventTracker.SendEvent(new AskOnDateEvent(Sim.CreatedSim, Target.CreatedSim, true));
            }

            Romances.BumpToHigherState(this, Sim, Target);

            IncStat(LTR.CurrentLTR.ToString());

            if (SimID.Matches(Flirts.PreviousLoveLoss, Sim))
            {
                Flirts.PreviousLoveLoss = null;
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new NewFlirtScenario(this);
        }
    }
}
