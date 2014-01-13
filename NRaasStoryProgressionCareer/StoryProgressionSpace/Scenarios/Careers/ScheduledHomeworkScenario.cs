using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
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
    public class ScheduledHomeworkScenario : HomeworkScenario
    {
        public ScheduledHomeworkScenario()
            : base ()
        { }
        protected ScheduledHomeworkScenario(ScheduledHomeworkScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "HomeworkCompletion";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Minimum
        {
            get 
            { 
                int value = GetValue<CompletionOption,int>();
                if (Sim != null)
                {
                    value += AddScoring("HomeworkCompletion", Sim);
                }

                return value;
            }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override int PushChance
        {
            get { return 10; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.SchoolChildren;
        }

        protected override bool Allow()
        {
            if (Minimum <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            if (initialPass) return false;

            if (SimClock.HoursPassedOfDay < 16) return false;

            return true;
        }

        public override Scenario Clone()
        {
            return new ScheduledHomeworkScenario(this);
        }

        public class CompletionOption : IntegerScenarioOptionItem<ManagerCareer, ScheduledHomeworkScenario>, ManagerCareer.ISchoolOption
        {
            public CompletionOption()
                : base(50)
            { }

            public override string GetTitlePrefix()
            {
                return "HomeworkCompletion";
            }

            protected override int Validate(int value)
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 100)
                {
                    value = 100;
                }

                return base.Validate(value);
            }
        }
    }
}
