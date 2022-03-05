using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI.Hud;

namespace NRaas.Gameplay.OmniSpace.Tones
{
    [Persistable]
    public class WorkOnLateHomeworkTone : CareerToneEx
    {
        // Methods
        public override void BeginCareerTone(InteractionInstance interactionInstance)
        {
            base.BeginCareerTone(interactionInstance);

            Career.PerformanceBonusPerHour += Sims3.Gameplay.Careers.School.WorkOnLateHomeworkTone.kPerformanceModifier;
            interactionInstance.AddExcludedDream(DreamNames.do_homework);
        }

        public override void EndCareerTone(InteractionInstance interactionInstance)
        {
            base.EndCareerTone(interactionInstance);

            Career.PerformanceBonusPerHour -= Sims3.Gameplay.Careers.School.WorkOnLateHomeworkTone.kPerformanceModifier;
            interactionInstance.RemoveExcludedDream(DreamNames.do_homework);
        }

        public override void OnTimePassed(InteractionInstance interactionInstance, float totalTime, float deltaTime)
        {
            base.OnTimePassed(interactionInstance, totalTime, deltaTime);

            School school = GetSchool();
            if (school == null) return;

            school.UpdateDetention(school.SchoolTuning.DetentionChangeWorkOnHomework, false);
            float completionRate = school.OwnersHomework.GetCompletionRate(school.OwnerDescription.CreatedSim, false, false);
            school.OwnersHomework.UpdateProgress(null, completionRate, deltaTime);
            if ((school.OwnersHomework != null) && school.OwnersHomework.IsComplete())
            {
                EventTracker.SendEvent(EventTypeId.kDidHomework, school.OwnerDescription.CreatedSim, school.OwnersHomework);
                interactionInstance.CurrentTone = null;
            }
        }

        public override bool Test(InteractionInstance inst, out StringDelegate reason)
        {
            if (!base.Test(inst, out reason)) return false;

            School school = GetSchool();
            if (school == null) return false;

            if (school.CurentDetentionStatus == School.DetentionStatus.InDetention)
            {
                return false;
            }
            return ((school.OwnersHomework != null) && !school.OwnersHomework.IsComplete());
        }

        protected School GetSchool()
        {
            if (Career.OwnerDescription == null) return null;

            if (Career.OwnerDescription.CareerManager == null) return null;

            return Career.OwnerDescription.CareerManager.School;
        }
    }
}
