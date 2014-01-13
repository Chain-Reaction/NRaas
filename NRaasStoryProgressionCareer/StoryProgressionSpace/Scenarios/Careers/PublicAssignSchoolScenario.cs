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
    public class PublicAssignSchoolScenario : AssignSchoolScenario
    {
        public PublicAssignSchoolScenario(SimDescription sim)
            : base (sim)
        { }
        protected PublicAssignSchoolScenario(PublicAssignSchoolScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PublicAssignSchool";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            School school = sim.CareerManager.School;

            if ((school != null) && (school.CurLevel != null) && (school.CurLevel.DayLength == 0))
            {
                IncStat("Home Schooled");
                return false;
            }

            bool required = false;
            bool testType = false;
            if (school == null)
            {
                required = true;
                testType = true;
            }
            else if (!HasValue<ConsiderPublicOption,OccupationNames>(school.Guid))
            {
                required = true;
                testType = true;
            }
            else if (!GetLotOptions(school.CareerLoc.Owner.LotCurrent).AllowCastes(this, Sim))
            {
                required = true;
                testType = false;
            }

            if (!required)
            {
                IncStat("Not Required");
                return false;
            }

            if ((testType) && (!GetValue<AssignPublicSchoolOption, bool>(sim)))
            {
                IncStat("Not Public");
                return false;
            }

            return true;
        }

        protected override List<CareerLocation> GetPotentials()
        {
            List<CareerLocation> schools = new List<CareerLocation>();

            foreach (Career career in CareerManager.CareerList)
            {
                if (career is SchoolHigh)
                {
                    if (!Sim.Teen) continue;
                }
                else if (career is SchoolElementary)
                {
                    if (!Sim.Child) continue;
                }
                else
                {
                    continue;
                }

                if (!HasValue<ConsiderPublicOption, OccupationNames>(career.Guid)) continue;

                IncStat("Potential: " + career.Name);

                CareerLocation location = FindClosestCareerLocation(Sim, career.Guid);
                if (location == null) continue;

                if (location.Owner == null) continue;

                schools.Add(location);
            }

            return schools;
        }

        public override Scenario Clone()
        {
            return new PublicAssignSchoolScenario(this);
        }

        public class ConsiderPublicOption : OccupationManagerOptionItem<ManagerCareer>, ManagerCareer.ISchoolOption
        {
            public ConsiderPublicOption()
                : base(new OccupationNames[] { OccupationNames.SchoolElementary, OccupationNames.SchoolHigh })
            { }

            protected override bool Allow(OccupationNames value)
            {
                School school = CareerManager.GetStaticCareer(value) as School;

                return (school != null);
            }

            public override string GetTitlePrefix()
            {
                return "ConsiderPublic";
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                ScheduledAssignSchoolScenario.SetToImmediateUpdate();
                return true;
            }
        }
    }
}
