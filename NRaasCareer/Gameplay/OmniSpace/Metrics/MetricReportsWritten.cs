using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.OmniSpace.Metrics
{
    [Persistable]
    public class MetricReportsWritten : Sims3.Gameplay.Careers.LawEnforcement.MetricReportsWritten
    {
        // Methods
        public MetricReportsWritten(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        {
        }

        public override float FindMetricValue(Career career)
        {
            LawEnforcement enforcement = OmniCareer.Career<LawEnforcement>(career);
            int num = 0x0;
            if (enforcement != null)
            {
                for (int i = 0x0; i < LawEnforcement.kWindowForRememberingReports; i++)
                {
                    num += enforcement.mReportsWrittenPastNDays[i];
                }
                num += enforcement.ReportsCompltetedToday;
            }
            return (float)num;
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionPoliceReports", "Gameplay/Careers/Metrics:DescriptionPoliceReports", new object[0x0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitlePoliceReports", "Gameplay/Careers/Metrics:TitlePoliceReports", new object[0x0]);
        }
    }
}
