using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.Hud;
using System;

namespace NRaas.Gameplay.OmniSpace.Tones
{
    [Persistable]
    public class GiveLecturesTone : CareerToneEx
    {
        // Fields
        private int mLecturesHeldBeforeStartOfTone;
        private float mTimeForStartOfTone;

        // Methods
        public override void BeginCareerTone(InteractionInstance interactionInstance)
        {
            base.BeginCareerTone(interactionInstance);

            mTimeForStartOfTone = SimClock.ElapsedTime(TimeUnit.Hours);
            Education career = OmniCareer.Career<Education>(Career);
            if (career != null)
            {
                mLecturesHeldBeforeStartOfTone = career.LecturesGivenToday;
            }
        }

        public override void OnTimePassed(InteractionInstance interactionInstance, float totalTime, float deltaTime)
        {
            base.OnTimePassed(interactionInstance, totalTime, deltaTime);

            Education career = OmniCareer.Career<Education>(Career);
            if (career != null)
            {
                float num = SimClock.ElapsedTime(TimeUnit.Hours) - mTimeForStartOfTone;
                if (((int)Math.Floor((double)(num / Education.kSimHoursGiveLecturesWorthwhile))) > (career.LecturesGivenToday - mLecturesHeldBeforeStartOfTone))
                {
                    career.LecturesGivenToday++;
                    if ((Career.OwnerDescription != null) && (Career.OwnerDescription.CreatedSim != null))
                    {
                        EventTracker.SendEvent(EventTypeId.kGaveEducationLecture, Career.OwnerDescription.CreatedSim);
                    }
                }
            }
        }

        public override bool Test(InteractionInstance ii, out StringDelegate reason)
        {
            if (!base.Test(ii, out reason)) return false;

            Education career = OmniCareer.Career<Education>(Career);
            if ((career != null) && (career.CurLevel != null))
            {
                return OmniCareer.HasMetric<MetricLecturesGiven>(Career);
            }
            return false;
        }
    }
}
