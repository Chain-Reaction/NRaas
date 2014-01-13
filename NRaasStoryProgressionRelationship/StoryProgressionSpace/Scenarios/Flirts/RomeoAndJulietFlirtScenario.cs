using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
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
    public class RomeoAndJulietFlirtScenario : RegularFlirtBaseScenario
    {
        public RomeoAndJulietFlirtScenario()
            : base(10)
        { }
        protected RomeoAndJulietFlirtScenario(RomeoAndJulietFlirtScenario scenario)
            : base (scenario)
        { }

        protected override bool Force
        {
            get { return true; }
        }

        protected override string NewFlirtStory
        {
            get { return "RomeoAndJuliet"; }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RomeoAndJulietFlirt";
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            List<SimDescription> results = new List<SimDescription>();

            foreach (SimDescription parent in Relationships.GetParents(sim))
            {
                foreach (Relationship relation in Relationship.Get(parent))
                {
                    if (!relation.AreEnemies()) continue;

                    SimDescription other = relation.GetOtherSimDescription(parent);
                    if (other == null) continue;

                    foreach (SimDescription child in Relationships.GetChildren(other))
                    {
                        results.Add(child);
                    }
                }
            }

            return Flirts.FindAnyFor(this, sim, true, Force, results);
        }

        public override Scenario Clone()
        {
            return new RomeoAndJulietFlirtScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerFlirt, RomeoAndJulietFlirtScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RomeoAndJulietFlirt";
            }
        }
    }
}
