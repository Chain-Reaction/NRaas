using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class PayUniversityProfessorsScenario : SimScenario
    {
        public PayUniversityProfessorsScenario()
            : base()
        { }
        protected PayUniversityProfessorsScenario(PayUniversityProfessorsScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PayUniversityProfessors";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (GetValue<Option, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            // Run every 10 sim-minutes
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if ((Sim.OccupationAsAcademicCareer == null) || (Sim.CareerManager == null) || (Sim.CareerManager.DegreeManager == null))
            {
                IncStat("Unnecessary");
                return false;
            }

            if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }

            if (sim.OccupationAsAcademicCareer.Level != 2)
            {
                IncStat("Not Professor");
                return false;
            }

            return base.Allow(sim);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            List<SimDescription> sims = new List<SimDescription>(Careers.UniversitySims);

            return sims;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            AcademicCareer career = Sim.OccupationAsAcademicCareer;

            if (Careers.Allow(this, Sim))
            {                
                PayProfessor(career);
            }
            
            return true;
        }        

        protected bool PayProfessor(AcademicCareer career)
        {
            Sim createdSim = career.OwnerDescription.CreatedSim;
            if ((createdSim != null) &&
                (!career.SpecialWorkDay) &&
                (!(createdSim.CurrentInteraction is Pregnancy.GoToHospital)))
            {
                DateAndTime time = SimClock.CurrentTime();
                //AcademicCareer.CourseCreationSpec spec = career.GetNextCourse(SimClock.CurrentDayOfWeek, 0);
                // this needs more work because the above returns the first days course which the below fails to match if sim is attending later day course
                // but I'm not sure if passing the current hour would return the current course or the one after it...
                if (career.IsAtWork /*&& (spec != null && (SimClock.IsTimeBetweenTimes(time.Hour, spec.mCourseStartTime, spec.mCourseStartTime + spec.mJobCreationSpec.JobStaticData.HoursAvailable)*/ || career.IsSpecialWorkTime)//))
                {                    
                    int payPerHour = GetValue<ProfessorPayPerHour, int>();
                    if (payPerHour > 0)
                    {                        
                        payPerHour /= 6;                        
                        career.PayOwnerSim(payPerHour, GotPaidEvent.PayType.kCareerNormalPay);
                    }
                }                
                
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new PayUniversityProfessorsScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerCareer, PayUniversityProfessorsScenario>, ManagerCareer.IPerformanceOption, IDebuggingOption
        {
            int mCount = 1;

            public Option()
                : base(3)
            { }

            public override string GetTitlePrefix()
            {
                return "PayProfessorsInterval";
            }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                if (mCount >= Value)
                {
                    mCount = 1;
                    base.PrivateUpdate(fullUpdate, initialPass);
                }
                else
                {
                    mCount++;
                }
            }
        }

        public class ProfessorPayPerHour : IntegerScenarioOptionItem<ManagerCareer, PayUniversityProfessorsScenario>, ManagerCareer.IHomeworldUniversityOption
        {
            public ProfessorPayPerHour()
                : base(1000)
            { }

            public override string GetTitlePrefix()
            {
                return "ProfessorPayPerHour";
            }
        }
    }
}