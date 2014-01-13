using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Selection;
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
    public class PrivateAssignSchoolScenario : AssignSchoolScenario
    {
        public PrivateAssignSchoolScenario(SimDescription sim)
            : base (sim)
        { }
        protected PrivateAssignSchoolScenario(PrivateAssignSchoolScenario scenario)
            : base (scenario)
        { }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PrivateAssignSchool";
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (!GetValue<AssignPrivateSchoolOption,bool>(sim))
            {
                IncStat("Not Private");
                return false;
            }
            else if (GetValue<AssignPublicSchoolOption, bool>(sim))
            {
                IncStat("Public");
                return false;
            }

            School school = sim.CareerManager.School;

            bool required = false;
            if (school == null)
            {
                required = true;
            }
            else if (HasValue<PublicAssignSchoolScenario.ConsiderPublicOption, OccupationNames>(school.Guid))
            {
                required = true;
            }
            else if (!GetLotOptions(school.CareerLoc.Owner.LotCurrent).AllowCastes(this, Sim))
            {
                required = true;
            }

            if (!required)
            {
                IncStat("Not Required");
                return false;
            }

            return true;
        }

        protected override List<CareerLocation> GetPotentials()
        {
            List<OccupationNames> careers = new List<OccupationNames>();
            bool dream = Careers.GetPotentialCareers(this, Sim, careers, false);

            List<CareerLocation> dreamSchools = new List<CareerLocation>();

            foreach (OccupationNames career in careers)
            {
                Career staticJob = CareerManager.GetStaticCareer(career);
                if (staticJob == null) continue;

                CareerLocation jobLocation = FindClosestCareerLocation(Sim, staticJob.Guid);
                if (jobLocation == null) continue;

                if (jobLocation.Owner == null) continue;

                if (jobLocation.Owner.CareerLocations == null) continue;

                foreach (CareerLocation schoolLoc in jobLocation.Owner.CareerLocations.Values)
                {
                    School staticSchool = schoolLoc.Career as School;
                    if (staticSchool == null) continue;

                    if (HasValue<DisallowCareerOption, OccupationNames>(Sim, staticSchool.Guid)) continue;

                    if (HasValue<PublicAssignSchoolScenario.ConsiderPublicOption, OccupationNames>(staticSchool.Guid)) continue;

                    // Disallow home schooling at this point
                    if ((staticSchool.Level1 == null) || (staticSchool.Level1.DayLength == 0)) continue;

                    if (staticSchool is SchoolHigh)
                    {
                        if (career == OccupationNames.SchoolHigh) continue;

                        if (!Sim.Teen) continue;
                    }
                    else if (staticSchool is SchoolElementary)
                    {
                        if (career == OccupationNames.SchoolElementary) continue;

                        if (!Sim.Child) continue;
                    }
                    else
                    {
                        if (!staticJob.CareerAgeTest(Sim)) continue;
                    }

                    CareerLocation location = FindClosestCareerLocation(Sim, staticSchool.Guid);
                    if (location == null) continue;

                    dreamSchools.Add(location);
                }
            }

            AddStat("Dream Schools", dreamSchools.Count);

            if ((GetValue<PromptToAssignSchoolOption, bool>()) && (Careers.MatchesAlertLevel(Sim)))
            {
                List<FindJobScenario.JobItem> items = new List<FindJobScenario.JobItem>();

                bool found = false;

                foreach (Career career in CareerManager.CareerList)
                {
                    if (career is SchoolHigh)
                    {
                        if (!Sim.Teen) continue;

                        if (career.Guid != OccupationNames.SchoolHigh)
                        {
                            found = true;
                        }
                    }
                    else if (career is SchoolElementary)
                    {
                        if (!Sim.Child) continue;

                        if (career.Guid != OccupationNames.SchoolElementary)
                        {
                            found = true;
                        }
                    }
                    else if (career is School)
                    {
                        if (!career.CareerAgeTest(Sim)) continue;

                        if ((career.Level1 != null) && (career.Level1.DayLength != 0))
                        {
                            found = true;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (HasValue<DisallowCareerOption, OccupationNames>(Sim, career.Guid)) continue;

                    if (HasValue<PublicAssignSchoolScenario.ConsiderPublicOption, OccupationNames>(career.Guid)) continue;

                    CareerLocation location = FindClosestCareerLocation(Sim, career.Guid);
                    if (location == null) continue;

                    items.Add(new FindJobScenario.JobItem(location.Career, dreamSchools.Contains(location)));
                }

                FindJobScenario.JobItem choice = null;
                if ((items.Count > 1) && (found))
                {
                    if (AcceptCancelDialog.Show(ManagerSim.GetPersonalInfo(Sim, Common.Localize("RichAssignSchool:Prompt", Sim.IsFemale))))
                    {
                        choice = new CommonSelection<FindJobScenario.JobItem>(Common.Localize("ChooseCareer:Header", Sim.IsFemale), Sim.FullName, items, new FindJobScenario.JobPreferenceColumn()).SelectSingle();
                    }
                }
                else if (items.Count == 1)
                {
                    Career career = items[0].Value as Career;

                    // Do not auto-enroll sims in home-schooling
                    if ((career.Level1 != null) && (career.Level1.DayLength != 0))
                    {
                        choice = items[0];
                    }
                }

                if (choice != null)
                {
                    SetValue<ManualSchoolOption, bool>(Sim, true);

                    dreamSchools.Clear();

                    CareerLocation location = FindClosestCareerLocation(Sim, choice.Value.Guid);
                    if (location != null)
                    {
                        dreamSchools.Add(location);
                    }
                }
            }
            
            if (dreamSchools.Count == 0)
            {
                IncStat("Random");

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

                    if (HasValue<DisallowCareerOption, OccupationNames>(Sim, career.Guid)) continue;

                    if (HasValue<PublicAssignSchoolScenario.ConsiderPublicOption, OccupationNames>(career.Guid)) continue;

                    CareerLocation location = FindClosestCareerLocation(Sim, career.Guid);
                    if (location == null) continue;

                    if (location.Owner == null) continue;

                    dreamSchools.Add(location);
                }

                /*
                if (dreamSchools.Count < 4)
                {
                    IncStat("Too Few");

                    dreamSchools.Clear();
                }*/
            }

            return dreamSchools;
        }

        public override Scenario Clone()
        {
            return new PrivateAssignSchoolScenario(this);
        }

        public class PromptToAssignSchoolOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.ISchoolOption
        {
            public PromptToAssignSchoolOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "PromptToAssignRichSchool";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
