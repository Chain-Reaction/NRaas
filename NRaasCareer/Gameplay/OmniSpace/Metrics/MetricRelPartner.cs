using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
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
    public class MetricRelPartner : Sims3.Gameplay.Careers.LawEnforcement.MetricRelPartner
    {
        // Methods
        public MetricRelPartner(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        {
        }

        public override float FindMetricValue(Career career)
        {
            LawEnforcement enforcement = OmniCareer.Career <LawEnforcement> (career);

            SimDescription partner = null;
            if (enforcement != null)
            {
                partner = enforcement.Partner;
            }

            if (partner == null)
            {
                if (career.Coworkers.Count > 0)
                {
                    partner = career.Coworkers[0];
                }
            }

            if (partner == null)
            {
                return 0f;
            }

            Relationship relationship = enforcement.OwnerDescription.GetRelationship(partner, false);
            if (relationship == null)
            {
                return 0f;
            }

            return relationship.GetCareerRating();
        }

        public override string MetricDescription(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionRelPartner", "Gameplay/Careers/Metrics:DescriptionRelPartner", new object[0x0]);
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleRelPartner", "Gameplay/Careers/Metrics:TitleRelPartner", new object[0x0]);
        }
    }
}
