using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
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
    public class CelebrityRaiseScenario : CareerEventScenario
    {
        public CelebrityRaiseScenario()
        { }
        protected CelebrityRaiseScenario(CelebrityRaiseScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "GotRaise";
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
            return events.AddListener(this, EventTypeId.kCareerGotRaise);
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            Career career = sim.Occupation as Career;
            if (career == null)
            {
                IncStat("No Career");
                return false;
            }
            else if (career.CurLevel == null)
            {
                IncStat("No Level");
                return false;
            }
            else if (career.CurLevel.NextLevels.Count > 0)
            {
                IncStat("Not Highest");
                return false;
            }
            else if (career is School)
            {
                IncStat("School");
                return false;
            }
            else if (career.IsPartTime)
            {
                IncStat("Parttime");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Friends.AccumulateCelebrity(Sim, GetValue<Option, int>());
            return true;
        }

        public override Scenario Clone()
        {
            return new CelebrityRaiseScenario(this);
        }

        public class Option : IntegerEventOptionItem<ManagerCareer, CelebrityRaiseScenario>
        {
            public Option()
                : base(500)
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityRaise";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
