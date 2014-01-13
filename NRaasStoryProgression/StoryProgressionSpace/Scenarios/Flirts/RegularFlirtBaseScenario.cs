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
    public abstract class RegularFlirtBaseScenario : FlirtScenario
    {
        public RegularFlirtBaseScenario(int delta)
            : base(delta)
        { }
        protected RegularFlirtBaseScenario(RegularFlirtBaseScenario scenario)
            : base (scenario)
        { }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 5; }
        }

        protected virtual bool Force
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override int PregnancyChance
        {
            get { return -1; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool TestFlirtCooldown
        {
            get { return true; }
        }

        protected override ManagerRomance.AffairStory AffairStory
        {
            get
            {
                return ManagerRomance.AffairStory.All;
            }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Flirts.FlirtySims;
        }

        protected override bool Allow(SimDescription sim)
        {
            int score = AddScoring("Monogamous", sim);

            if (score > 0)
            {
                int totalFlirts = 0;
                foreach (Relationship relation in Relationship.GetRelationships(sim))
                {
                    if (relation.AreRomantic())
                    {
                        totalFlirts++;

                        if (score > 0)
                        {
                            IncStat("Monogamy");
                            return false;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if ((!Force) && (AddScoring("FlirtySingle", sim) <= 0))
            {
                IncStat("Unflirty");
                return false;
            }

            return base.Allow(sim);
        }
    }
}
