using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
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
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
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
    public class UpdateCareerScenario : SimScenario
    {
        bool mUpdateAlarms = false;

        public UpdateCareerScenario(SimDescription sim, bool updateAlarms)
            : base (sim)
        {
            mUpdateAlarms = updateAlarms;
        }
        protected UpdateCareerScenario(bool updateAlarms)
        {
            mUpdateAlarms = updateAlarms;
        }
        protected UpdateCareerScenario(UpdateCareerScenario scenario)
            : base (scenario)
        {
            mUpdateAlarms = scenario.mUpdateAlarms;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "UpdateCareer";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }

            return base.Allow(sim);
        }
/*
        protected bool ManageService(SimDescription sim)
        {
            foreach (Service s in Services.AllServices)
            {
                s.mPool.Remove(sim);
            }

            if (SimTypes.IsSelectable(sim)) return false;

            if (sim.Occupation == null) return false;

            Service service = null;
            if (sim.Occupation is ActiveFireFighter)
            {
                service = Firefighter.Instance;
            }
            else if (sim.Occupation is Criminal)
            {
                service = Burglar.Instance;
            }
            else if (sim.Occupation is LawEnforcement)
            {
                service = Police.Instance;
            }
            else if (ManagerCareer.HasSkillCareer(sim, SkillNames.Handiness))
            {
                service = Repairman.Instance;
            }

            if (service != null)
            {
                if ((sim.Age & ServiceNPCSpecifications.GetAppropriateAges(service.ServiceType.ToString())) != CASAgeGenderFlags.None)
                {
                    service.AddSimToPool(sim);
                    IncStat("Service: " + service.ServiceType);
                }
            }
            
            return true;
        }
*/
        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            //ManageService(Sim);

            GetData(Sim).InvalidateCache();            

            if ((Sim.Occupation != null) && (Sim.Occupation.Coworkers == null))
            {
                Sim.Occupation.Coworkers = new List<SimDescription>();
                IncStat("Coworker List Added");
            }

            if ((Sim.CareerManager != null) && (Sim.CareerManager.School != null) && (Sim.CareerManager.School.Coworkers == null))
            {
                Sim.CareerManager.School.Coworkers = new List<SimDescription>();
                IncStat("Coworker List Added");
            }

            if (mUpdateAlarms)
            {
                IncStat("Update Alarms");

                foreach (ICommuteSimData data in GetData(Sim).GetList<ICommuteSimData>())
                {
                    data.Reset();
                }
            }

            Add(frame, new WorkUpdateScenario(Sim), ScenarioResult.Start);
            Add(frame, new ActiveCareerPushScenario(Sim), ScenarioResult.Start);
            Add(frame, new SchoolUpdateScenario(Sim), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new UpdateCareerScenario(this);
        }
    }
}
