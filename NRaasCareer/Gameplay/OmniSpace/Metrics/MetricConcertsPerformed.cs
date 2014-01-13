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
    public class MetricConcertsPerformed : Music.MetricConcertsPerformed
    {
        // Methods
        public MetricConcertsPerformed(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        { }

        public override float FindMetricValue(Career career)
        {
            Music music = OmniCareer.Career<Music> (career);
            if (music == null) return 0;

            return (float)music.ConcertsPerformed;
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionConcertsPerformed", "Gameplay/Careers/Metrics:DescriptionConcertsPerformed", new object[0x0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleConcertsPerformed", "Gameplay/Careers/Metrics:TitleConcertsPerformed", new object[0x0]);
        }
    }
}
