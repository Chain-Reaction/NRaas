using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class MeetTheTeacherScenario : MeetTheWorkerScenario
    {
        public MeetTheTeacherScenario()
        { }
        protected MeetTheTeacherScenario(MeetTheTeacherScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "MeetTheTeacher";
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.SchoolChildren;
        }
        
        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override Career GetCareer(SimDescription sim)
        {
            if (sim.CareerManager == null) return null;

            return sim.CareerManager.School;
        }

        public override Scenario Clone()
        {
            return new MeetTheTeacherScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerFriendship, MeetTheTeacherScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "MeetTheTeacher";
            }
        }
    }
}
