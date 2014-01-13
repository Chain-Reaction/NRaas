using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Situations;
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
    public abstract class CareerPushScenario : OccupationPushScenario
    {
        public CareerPushScenario(SimDescription sim)
            : base (sim)
        {}
        protected CareerPushScenario(CareerPushScenario scenario)
            : base (scenario)
        { }

        public delegate InteractionInstance WorkInteraction(Career job);

        public static WorkInteraction OnWorkInteraction;

        public static InteractionInstance GetWorkInteraction(Career job)
        {
            InteractionInstance instance = null;

            try
            {
                if (OnWorkInteraction != null)
                {
                    instance = OnWorkInteraction(job);
                }
                else
                {
                    instance = job.CreateWorkInteractionInstance();
                }
            }
            catch (Exception e)
            {
                StoryProgression.DebugException(job.OwnerDescription, e);
            }

            return instance;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim sim = Sim.CreatedSim;
            if (sim == null)
            {
                Sims.Instantiate(Sim, null, false);

                sim = Sim.CreatedSim;
            }

            if (sim == null)
            {
                IncStat("Hibernating");
                return false;
            }

            Career job = Occupation as Career;

            Careers.VerifyTone(job);

            if (!ManagerCareer.ValidCareer(job))
            {
                IncStat("Career Invalidated");
                return false;
            }

            InteractionInstance instance = GetWorkInteraction(job);

            if (instance == null)
            {
                IncStat("No Interaction");
                return false;
            }
            else if (!Test(sim, instance.InteractionDefinition))
            {
                return false;
            }
            else
            {
                if (sim.InteractionQueue.Add(instance))
                {
                    if (GetValue<AllowGoHomePushOption, bool>(Sim))
                    {
                        Manager.AddAlarm(new GoHomePushScenario(Sim));
                    }
                }
                else
                {
                    IncStat("Failure");
                }

                mReport = PostSlackerWarning();
                return true;
            }
        }
    }
}
