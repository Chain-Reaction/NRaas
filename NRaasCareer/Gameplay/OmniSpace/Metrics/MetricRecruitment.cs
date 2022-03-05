using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.Gameplay.OmniSpace.Metrics
{
    [Persistable]
    public class MetricRecruitment : PerfMetric
    {
        // Methods
        public MetricRecruitment(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        { }

        public override float FindMetricValue(Career career)
        {
            OmniCareer job = career as OmniCareer;
            if (job != null)
            {
                return (float)job.NumRecruits;
            }
            return 0f;
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionRecruits", "Gameplay/Careers/Metrics:DescriptionRecruits", new object[0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleRecruits", "Gameplay/Careers/Metrics:TitleRecruits", new object[0]);
        }
    }
}
