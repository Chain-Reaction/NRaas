using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class NewClassmateScenario : CoworkerBaseScenario
    {
        public NewClassmateScenario()
        { }
        public NewClassmateScenario(SimDescription sim)
            : base(sim)
        { }
        protected NewClassmateScenario(NewClassmateScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "NewClassmate";
        }

        protected override Career Career
        {
            get { return Sim.CareerManager.School; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.SchoolChildren;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (sim.CareerManager.School == null)
            {
                IncStat("No School");
                return false;
            }
            else if (sim.CareerManager.School.CareerLoc == null)
            {
                IncStat("No Location");
                return false;
            }

            return true;
        }

        protected override bool IsValid(Career job, SimDescription sim, bool checkExisting)
        {
            if (!base.IsValid(job, sim, checkExisting))
            {
                return false;
            }

            Career simJob = sim.CareerManager.School;
            if (simJob == null)
            {
                IncStat("Valid:No Job");
                return false;
            }
            else if ((checkExisting) && (simJob.Guid != job.Guid))
            {
                IncStat("Valid:Wrong Job");
                return false;
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            Careers.VerifyTone(Sim);
            return (mNewCoworkers.Count > 0);
        }

        public override Scenario Clone()
        {
            return new NewClassmateScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerCareer, NewClassmateScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ManageClassmate";
            }
        }
    }
}
