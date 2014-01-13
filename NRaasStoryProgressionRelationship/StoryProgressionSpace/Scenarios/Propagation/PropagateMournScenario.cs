using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Propagation
{
    public class PropagateMournScenario : PropagateBuffScenario
    {
        public PropagateMournScenario(SimDescription deadSim)
            : base(deadSim, BuffNames.Mourning, Origin.FromWitnessingDeath)
        { }
        protected PropagateMournScenario(PropagateMournScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PropagateMourn";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public override void SetActors(SimDescription actor, SimDescription target)
        {
            Sim = actor;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((!Relationships.IsCloselyRelated(Sim, Target, false)) || (ManagerSim.GetLTR(Sim, Target) < 50))
            {
                IncStat("Low LTR");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (ManagerSim.HasTrait(Target, TraitNames.Evil))
            {
                Target.CreatedSim.BuffManager.AddElement(BuffNames.FiendishlyDelighted, Origin);
            }
            else
            {
                float multiple = 0;

                if (Relationships.IsBloodRelated(Sim.Genealogy, Target.Genealogy, false))
                {
                    multiple++;
                }
                else if (Relationships.IsCloselyRelated(Sim, Target, false))
                {
                    multiple += 0.5f;
                }

                int ltr = ManagerSim.GetLTR(Sim, Target);
                if (ltr > 75)
                {
                    multiple += 1.5f;
                }
                else if (ltr > 50)
                {
                    multiple++;
                }
                else if (ltr > 25)
                {
                    multiple += 0.5f;
                }

                mTimeoutLength = multiple * 24 * 60;

                base.PrivateUpdate(frame);

                BuffMourning.BuffInstanceMourning mourning = Target.CreatedSim.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning;
                if (mourning != null)
                {
                    mourning.MissedSim = Sim;
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new PropagateMournScenario(this);
        }

        public new class Option : BooleanManagerOptionItem<ManagerDeath>
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerDeath main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    DiedScenario.OnPropagateMournScenario += OnRun;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "PropagateMourn";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new PropagateMournScenario(s.Sim), ScenarioResult.Start);
            }
        }
    }
}
