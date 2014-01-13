using Sims3.Gameplay.Academics;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;

namespace NRaas.CommonSpace.Helpers
{
    public class CollegeGraduationEx
    {
        public static void GraduateSim(SimDescription simDesc, AcademicDegree degree)
        {
            if ((simDesc != null) && (degree != null))
            {
                IHandleUniversityGraduation graduationRabbitHole = CollegeGraduation.GetGraduationRabbitHole() as IHandleUniversityGraduation;
                if (graduationRabbitHole != null)
                {
                    Annex annex = graduationRabbitHole as Annex;
                    if (annex != null)
                    {
                        AnnexEx.AddSimToGraduationList(annex, simDesc, degree);
                    }
                    else
                    {
                        graduationRabbitHole.AddSimToGraduationList(simDesc, degree);
                    }
                }
                else if (simDesc.CreatedSim != null)
                {
                    CollegeGraduation.GraduateInPlace entry = CollegeGraduation.GraduateInPlace.Singleton.CreateInstance(simDesc.CreatedSim, simDesc.CreatedSim, new InteractionPriority(InteractionPriorityLevel.High), false, false) as CollegeGraduation.GraduateInPlace;
                    simDesc.CreatedSim.InteractionQueue.AddNext(entry);
                    ActiveTopic.RemoveTopicFromSim(simDesc.CreatedSim, "University Graduation");
                }
            }
        }
    }
}