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
    public class NewSchoolScenario : CareerHiredBaseScenario
    {
        public NewSchoolScenario()
        { }
        protected NewSchoolScenario(NewSchoolScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            return "NewSchool";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!(Event.Career is School))
            {
                IncStat("Not School");
                return false;
            }
            else if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else
            {
                bool found = false;
                if (sim.CareerManager.QuitCareers != null)
                {
                    foreach (Occupation career in sim.CareerManager.QuitCareers.Values)
                    {
                        if (sim.Teen)
                        {
                            if (career is SchoolHigh)
                            {
                                found = true;
                                break;
                            }
                        }
                        else if (sim.Child)
                        {
                            if (career is SchoolElementary)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if (!found)
                {
                    IncStat("First School");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        public override Scenario Clone()
        {
            return new NewSchoolScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, NewSchoolScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "NewSchool";
            }
        }
    }
}
