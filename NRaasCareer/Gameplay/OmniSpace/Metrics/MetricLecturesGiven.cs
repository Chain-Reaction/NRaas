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
    public class MetricLecturesGiven : Sims3.Gameplay.Careers.Education.MetricLecturesGiven
    {
        // Methods
        public MetricLecturesGiven(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        { }

        public override float FindMetricValue(Career career)
        {
            Education education = OmniCareer.Career<Education>(career);

            int num = 0x0;
            if (education != null)
            {
                for (int i = 0x0; i < Education.kWindowForRememberingLectures; i++)
                {
                    num += education.mLecturesInLastNDays[i];
                }
                num += education.LecturesGivenToday;
            }
            return (float)num;
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionGiveLectures", "Gameplay/Careers/Metrics:DescriptionGiveLectures", new object[0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleGiveLectures", "Gameplay/Careers/Metrics:TitleGiveLectures", new object[0]);
        }
    }
}
