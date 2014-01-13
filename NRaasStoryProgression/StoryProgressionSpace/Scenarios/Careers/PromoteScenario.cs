using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class PromoteScenario : SimScenario
    {
        bool mRaise;

        public PromoteScenario()
        { }
        protected PromoteScenario(PromoteScenario scenario)
            : base (scenario)
        {
            mRaise = scenario.mRaise;
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected abstract bool GiveRaise
        {
            get;
        }

        protected bool WasRaise
        {
            get { return mRaise; }
        }

        protected bool IsHighestLevel(Career career)
        {
            if (career.CurLevel == null) return true;

            if (career.CurLevel.NextLevels == null) return true;

            return (career.CurLevel.NextLevels.Count == 0);
        }

        protected override bool Allow(SimDescription sim)
        {
            Career career = sim.Occupation as Career;
            if (career == null)
            {
                IncStat("No Job");
                return false;
            }
            else if ((!GiveRaise) && (IsHighestLevel(career)))
            {
                IncStat("Maximum Level");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Career career = Sim.Occupation as Career;

            mRaise = false;

            if (IsHighestLevel(career))
            {
                career.GiveRaise(false);

                mRaise = true;
            }
            else
            {
                career.PromoteSim();
            }

            return true;
        }
    }
}
