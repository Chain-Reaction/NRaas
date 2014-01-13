using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
    public class ImmigrantManageBossScenario : NewBossScenario
    {
        public ImmigrantManageBossScenario()
            : base()
        { }
        protected ImmigrantManageBossScenario(ImmigrantManageBossScenario scenario)
            : base(scenario)
        { }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Occupation == null) return false;

            if (sim.Occupation.CareerLoc == null) return false;

            if (!NeedsBoss(sim.Occupation as Career, true)) return false;

            return base.Allow(sim);
        }

        public class Installer : ExpansionInstaller<ManagerCareer>
        {
            protected override bool PrivateInstall(ManagerCareer main, bool initial)
            {
                if (initial)
                {
                    ImmigrantRequirementScenario.OnImmigrantManageBossScenario += new UpdateDelegate(OnInstall);
                }

                return true;
            }

            public static void OnInstall(Scenario scenario, ScenarioFrame frame)
            {
                scenario.Add(frame, new ImmigrantManageBossScenario(), ScenarioResult.Start);
            }
        }
    }
}
