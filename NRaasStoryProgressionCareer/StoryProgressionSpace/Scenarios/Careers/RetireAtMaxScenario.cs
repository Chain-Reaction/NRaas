using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
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
    public class RetireAtMaxScenario : SimScenario
    {
        public RetireAtMaxScenario (SimDescription sim)
            : base(sim)
        { }
        protected RetireAtMaxScenario(RetireAtMaxScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RetireAtMax";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

 	        return base.Allow();
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
                IncStat("Careers Denied");
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
            else if (sim.Occupation.CareerLevel != sim.Occupation.HighestLevel)
            {
                IncStat("Not Max");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim.Occupation.RetireNoConfirmation();
            return true;
        }

        public override Scenario Clone()
        {
            return new RetireAtMaxScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IRetirementOption  
        {
            public Option()
                : base(false)
            { }

            public override bool Install(ManagerCareer main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    PromotedScenario.OnRetireAtMaxScenario += OnPerform;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "RetireAtMax";
            }

            public static void OnPerform(Scenario scenario, ScenarioFrame frame)
            {
                RetirementScenario retirement = scenario as RetirementScenario;
                if (retirement != null)
                {
                    retirement.RetiredAtMax = scenario.GetValue<Option, bool>();
                }

                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new RetireAtMaxScenario(s.Sim), ScenarioResult.Start);
            }
        }
    }
}
