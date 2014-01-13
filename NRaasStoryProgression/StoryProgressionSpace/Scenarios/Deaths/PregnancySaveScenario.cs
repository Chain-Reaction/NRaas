using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class PregnancySaveScenario : DualSimScenario
    {
        public PregnancySaveScenario(SimDescription sim)
            : base (sim)
        { }
        protected PregnancySaveScenario(PregnancySaveScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PregnantSave";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Pregnancies.PregnantSims;
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!sim.IsPregnant)
            {
                IncStat("Not Pregnant");
                return false;
            }
            else if (sim.Pregnancy.DadDescriptionId != Sim.SimDescriptionId)
            {
                IncStat("Not Dad");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            GetData<ManagerDeath.DyingSimData>(Sim).Saved = true;

            AgingManager.Singleton.CancelAgingAlarmsForSim(Sim.AgingState);
            return true;
        }

        public override Scenario Clone()
        {
            return new PregnancySaveScenario(this);
        }
    }
}
