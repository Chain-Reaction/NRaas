using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.Gameplay.OmniSpace.Metrics
{
    [Persistable]
    public class MetricJournals : PerfMetric
    {
        // Methods
        public MetricJournals(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        {
        }

        public override float FindMetricValue(Career career)
        {
            OmniCareer job = career as OmniCareer;
            if (job != null)
            {
                return (float)job.NumJournalsRead;
            }
            return 0f;
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionJournals", "Gameplay/Careers/Metrics:DescriptionMedicalJournals", new object[0x0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleJournals", "Gameplay/Careers/Metrics:TitleMedicalJournals", new object[0x0]);
        }
    }
}
