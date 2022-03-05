using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;

namespace NRaas.CareerSpace.Booters
{
    public class SchoolBooter : Common.IWorldLoadFinished
    {
        static SchoolBooter()
        { }

        public void OnWorldLoadFinished()
        {
            new Common.AlarmTask(1f, TimeUnit.Minutes, HomeSchooling.OnBootHomework);

            new Common.DelayedEventListener(EventTypeId.kLeftJob, OnLeftJob);
        }

        public static bool Enroll(Sim sim, string unemployedName, CareerLocation location)
        {
            Occupation job = sim.Occupation;
            Occupation retiredJob = sim.CareerManager.RetiredCareer;

            sim.CareerManager.mRetiredCareer = null;

            try
            {
                if (location.Career is School)
                {
                    sim.CareerManager.mJob = null;
                }

                if (!sim.AcquireOccupation(new AcquireOccupationParameters(location, true, false)))
                {
                    return false;
                }

                NRaas.Gameplay.Careers.Unemployed unemployed = sim.Occupation as NRaas.Gameplay.Careers.Unemployed;
                if (unemployed != null)
                {
                    unemployed.UpdateName(unemployedName);
                }
                return true;
            }
            finally
            {
                if (sim.CareerManager.mJob == null)
                {
                    sim.CareerManager.mJob = job;
                }

                sim.CareerManager.mRetiredCareer = retiredJob;
                sim.CareerManager.UpdateCareerUI();
            }
        }

        protected static void OnLeftJob(Sims3.Gameplay.EventSystem.Event e)
        {
            LeftJobEvent leftJob = e as LeftJobEvent;
            if (leftJob != null)
            {
                School school = leftJob.Career as School;
                if (school != null)
                {
                    if ((school.OwnerDescription != null) &&
                        (school.OwnerDescription.CareerManager != null))
                    {
                        if (!school.OwnerDescription.CareerManager.QuitCareers.ContainsKey(school.Guid))
                        {
                            school.OwnerDescription.CareerManager.QuitCareers.Add(school.Guid, school);
                        }
                    }
                }
            }
        }
    }
}
