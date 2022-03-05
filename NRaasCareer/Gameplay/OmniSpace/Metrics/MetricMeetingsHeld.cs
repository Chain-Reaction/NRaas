using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.Gameplay.OmniSpace.Metrics
{
    [Persistable]
    public class MetricMeetingsHeld : Sims3.Gameplay.Careers.Business.MetricMeetingsHeld
    {
        // Methods
        public MetricMeetingsHeld(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        { }

        public override float FindMetricValue(Career career)
        {
            Business business = OmniCareer.Career<Business>(career);

            int num = 0;
            if (business != null)
            {
                for (int i = 0; i < Business.kWindowForRememberingMeetings; i++)
                {
                    num += business.mMeetingsOnLastNDays[i];
                }
                num += business.MeetingsHeldToday;
            }

            return (float)num;
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionHoldMeetings", "Gameplay/Careers/Metrics:DescriptionHoldMeetings", new object[0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleHoldMeetings", "Gameplay/Careers/Metrics:TitleHoldMeetings", new object[0]);
        }
    }
}
