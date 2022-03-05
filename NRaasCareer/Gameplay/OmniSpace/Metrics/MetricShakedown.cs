﻿using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.Gameplay.OmniSpace.Metrics
{
    [Persistable]
    public class MetricShakedown : PerfMetric
    {
        public MetricShakedown(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        { }

        public override float FindMetricValue(Career career)
        {
            OmniCareer job = career as OmniCareer;
            if (job != null)
            {
                return (float)job.ShakedownFunds;
            }
            return 0f;
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionShakedown", "Gameplay/Careers/Metrics:DescriptionShakedown", new object[0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleShakedown", "Gameplay/Careers/Metrics:TitleShakedown", new object[0]);
        }
    }
}
