using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.Hud;
using System;

namespace NRaas.Gameplay.OmniSpace.Tones
{
    [Persistable]
    public class HoldMeetingsTone : CareerToneEx
    {
        // Fields
        private int mMeetingsHeldBeforeStartOfTone;
        private float mTimeForStartOfTone;

        // Methods
        public override void BeginCareerTone(InteractionInstance interactionInstance)
        {
            base.BeginCareerTone(interactionInstance);

            mTimeForStartOfTone = SimClock.ElapsedTime(TimeUnit.Hours);
            Business career = OmniCareer.Career<Business> (Career);
            if (career != null)
            {
                mMeetingsHeldBeforeStartOfTone = career.MeetingsHeldToday;
            }
        }

        public override void OnTimePassed(InteractionInstance interactionInstance, float totalTime, float deltaTime)
        {
            base.OnTimePassed(interactionInstance, totalTime, deltaTime);

            Business career = OmniCareer.Career<Business>(Career);
            if (career != null)
            {
                float num = SimClock.ElapsedTime(TimeUnit.Hours) - mTimeForStartOfTone;
                if (((int)Math.Floor((double)(num / Business.kSimHoursToCallMeetingWothwhile))) > (career.MeetingsHeldToday - mMeetingsHeldBeforeStartOfTone))
                {
                    career.MeetingsHeldToday++;
                }
            }
        }

        public override bool Test(InteractionInstance ii, out StringDelegate reason)
        {
            if (!base.Test(ii, out reason)) return false;

            Business career = OmniCareer.Career<Business>(Career);
            if ((career != null) && (career.CurLevel != null))
            {
                return OmniCareer.HasMetric<MetricMeetingsHeld>(Career);
            }
            return false;
        }
    }
}
