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
    public class MetricReadingsPerformed : Sims3.Gameplay.Careers.FortuneTellerCareer.MetricRedingsPerformed // hay EA, grate speeling
    {
        public MetricReadingsPerformed(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        { }

        public override float FindMetricValue(Career career)
        {
            FortuneTellerCareer fortune = OmniCareer.Career<FortuneTellerCareer>(career);
            if (fortune != null)
            {
                return (float)fortune.PrivateReadingsPerfomed;
            }
            return 0f;
        }        

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionReadingsPerformed", "Gameplay/Careers/Metrics:DescriptionReadingsPerformed", new object[0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleReadingsPerformed", "Gameplay/Careers/Metrics:TitleReadingsPerformed", new object[0]);
        }
    }
}