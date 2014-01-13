using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
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
    public class ImmigrantRequirementPostScenario : ScheduledSoloScenario
    {
        ManagerLot.ImmigrationRequirement mRequirement;

        public ImmigrantRequirementPostScenario(ManagerLot.ImmigrationRequirement requirement)
        {
            mRequirement = requirement;
        }
        protected ImmigrantRequirementPostScenario(ImmigrantRequirementPostScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PostImmigrantRequirement";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (GetValue<ImmigrantRequirementScenario.PressureOption, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Dictionary<CareerLocation, JobNeed> locations = new Dictionary<CareerLocation, JobNeed>();

            foreach (SimDescription sim in Careers.Employed)
            {
                Career career = sim.Occupation as Career;
                if (career == null) continue;

                if (ManagerCareer.IsPlaceholderCareer(career)) continue;

                if (career.CareerLoc == null) continue;

                if (!NewBossScenario.NeedsBoss(career, true)) continue;

                if ((career.Boss == null) && (career.CurLevel.NextLevels.Count > 0))
                {
                    JobNeed need;
                    if (!locations.TryGetValue(career.CareerLoc, out need))
                    {
                        need = new JobNeed();
                        locations.Add(career.CareerLoc, need);
                    }

                    if (SimTypes.IsSelectable(sim))
                    {
                        need.mNeed += 10;
                    }
                    else
                    {
                        need.mNeed++;
                    }

                    if ((need.Sim == null) || (need.Sim.Occupation.CareerLevel < sim.Occupation.CareerLevel))
                    {
                        need.Sim = sim;
                    }
                }
            }

            int maxNeed = 0;
            foreach (JobNeed need in locations.Values)
            {
                if ((mRequirement.mCareerSim == null) || (maxNeed < need.mNeed))
                {
                    maxNeed = need.mNeed;
                    mRequirement.mCareerSim = need.Sim;
                }
            }

            return true;
        }

        protected class JobNeed
        {
            public int mNeed = 0;

            public SimDescription Sim = null;
        }

        public override Scenario Clone()
        {
            return new ImmigrantRequirementPostScenario(this);
        }

        public class Installer : ExpansionInstaller<ManagerCareer>
        {
            protected override bool PrivateInstall(ManagerCareer main, bool initial)
            {
                if (initial)
                {
                    ImmigrantRequirementScenario.OnImmigrantRequirementPostScenario += OnRun;
                }

                return true;
            }

            public static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                ImmigrantRequirementScenario s = scenario as ImmigrantRequirementScenario;
                if (s == null) return;

                scenario.Add(frame, new ImmigrantRequirementPostScenario(s.Requirement), ScenarioResult.Start);
            }
        }
    }
}
