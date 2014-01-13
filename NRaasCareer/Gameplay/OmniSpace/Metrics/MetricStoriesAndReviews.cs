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
    public class MetricStoriesAndReviews : Sims3.Gameplay.Careers.Journalism.MetricStoriesAndReviews
    {
        // Methods
        public MetricStoriesAndReviews(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        {
        }

        public override float FindMetricValue(Career career)
        {
            Journalism journalism = OmniCareer.Career <Journalism>(career);
            int num = 0x0;
            if (journalism != null)
            {
                for (int i = 0x0; i < Journalism.kWindowForRememberingStories; i++)
                {
                    num += journalism.mStoriesWrittenPastNDays[i];
                }
                num += journalism.StoriesWrittenToday;
            }
            return (float)num;
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionStoriesAndReviews", "Gameplay/Careers/Metrics:DescriptionStoriesAndReviews", new object[0x0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleStoriesAndReviews", "Gameplay/Careers/Metrics:TitleStoriesAndReviews", new object[0x0]);
        }
    }
}
