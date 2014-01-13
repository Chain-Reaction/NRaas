using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using NRaas.StoryProgressionSpace.Scoring;
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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ImmigrantCareerScenario : ImmigrantScenario
    {
        SimDescription mCareerSim = null;

        public ImmigrantCareerScenario(SimDescription sim, ManagerLot.ImmigrationRequirement requirement)
            : base(sim, requirement)
        { }
        protected ImmigrantCareerScenario(ImmigrantCareerScenario scenario)
            : base (scenario)
        {
            mCareerSim = scenario.mCareerSim;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Headhunter";
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.TeenOrBelow)
            {
                IncStat("Too Young");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            try
            {
                if (mRequirement.mCareerSim != null)
                {
                    AcquireOccupationParameters parameters = new AcquireOccupationParameters(mRequirement.CareerLoc, false, false);
                    parameters.JumpStartJob = true;
                    parameters.JumpStartLevel = mRequirement.CareerLevel.NextLevels[0];

                    Sim.AcquireOccupation(parameters);
                }
            }
            catch (Exception e)
            {
                Common.DebugException(Sim, e);
                return false;
            }

            if (mRequirement.mCareerSim == null)
            {
                Add(frame, new FindJobScenario(Sim, true, false), ScenarioResult.Start);

                mCareerSim = null;
            }
            else
            {
                if (Sim.Occupation != null)
                {
                    foreach (KeyValuePair<uint, DreamNodeInstance> instance in DreamsAndPromisesManager.sMajorWishes)
                    {
                        if (instance.Value.InputSubject == null) continue;

                        if (instance.Value.InputSubject.mType != DreamNodePrimitive.InputSubjectType.Career) continue;

                        OccupationNames career = (OccupationNames)instance.Value.InputSubject.EnumValue;
                        if (career != Sim.Occupation.Guid) continue;

                        Sim.LifetimeWish = instance.Key;
                        break;
                    }
                }

                Add(frame, new CareerChangedScenario(Sim), ScenarioResult.Start);

                mRequirement.mCareerSim = null;
            }

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Lots;
            }

            if (mCareerSim == null) return null;

            if (parameters == null)
            {
                parameters = new object[] { Sim, mCareerSim };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new ImmigrantCareerScenario(this);
        }
    }
}
