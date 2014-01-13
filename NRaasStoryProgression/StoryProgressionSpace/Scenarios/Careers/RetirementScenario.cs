using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
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
    public class RetirementScenario : SimScenario
    {
        bool mRetiredAtMax = false;

        public RetirementScenario(SimDescription sim)
            : base(sim)
        { }
        protected RetirementScenario(RetirementScenario scenario)
            : base(scenario)
        {
            mRetiredAtMax = scenario.mRetiredAtMax;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Retire";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public bool RetiredAtMax
        {
            set
            {
                mRetiredAtMax = value;
            }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.Elder)
            {
                IncStat("Not Elder");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.Occupation == null)
            {
                IncStat("No Job");
                return false;
            }
            else if (sim.Occupation is Retired)
            {
                IncStat("Retired");
                return false;
            }
            else if (sim.Occupation is SkillBasedCareer)
            {
                IncStat("Skill Based");
                return false;
            }
            else if (!GetValue<AllowFindJobOption, bool>(sim))
            {
                IncStat("Find Job Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            mRetiredAtMax = false;

            PromotedScenario.PerformRetireAtMax(this, frame);

            if (mRetiredAtMax)
            {
                IncStat("RetireAtMax Denied");
                return false;
            }

            if (Sim.Occupation.Guid != OccupationNames.Firefighter)
            {
                if (AddScoring("Retirement", GetValue<ChanceofRetirementOption, int>(Sim), ScoringLookup.OptionType.Chance, Sim) <= 0)
                {
                    IncStat("Scoring Fail");
                    return false;
                }
            }

            Sim.Occupation.RetireNoConfirmation();
            return true;
        }

        public override Scenario Clone()
        {
            return new RetirementScenario(this);
        }
    }
}
