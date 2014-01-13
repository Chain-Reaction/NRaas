using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class HomeAssignSchoolScenario : AssignSchoolScenario
    {
        public HomeAssignSchoolScenario(SimDescription sim)
            : base (sim)
        { }
        protected HomeAssignSchoolScenario(HomeAssignSchoolScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HomeAssignSchool";
        }

        protected override bool Allow()
        {
            if (!GetValue<HomeOption,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            School school = sim.CareerManager.School;
            if (school != null)
            {
                IncStat("Has School");
                return false;
            }

            return true;
        }

        protected override List<CareerLocation> GetPotentials()
        {
            List<CareerLocation> schools = new List<CareerLocation>();

            foreach (Career career in CareerManager.CareerList)
            {
                if (!(career is School)) continue;

                if (career.Level1 == null) continue;

                if (career.Level1.DayLength != 0) continue;

                if (!career.CareerAgeTest(Sim)) continue;

                CareerLocation location = FindClosestCareerLocation(Sim, career.Guid);
                if (location == null) continue;

                if (location.Owner == null) continue;
            }

            return schools;
        }

        public override Scenario Clone()
        {
            return new HomeAssignSchoolScenario(this);
        }

        public class HomeOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.ISchoolOption
        {
            public HomeOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HomeAssignSchool";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
