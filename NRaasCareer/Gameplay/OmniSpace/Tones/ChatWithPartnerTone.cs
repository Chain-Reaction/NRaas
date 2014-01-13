using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.OmniSpace.Tones
{
    [Persistable]
    public class ChatWithPartnerTone : HangWithCoworkersTone
    {
        // Methods
        public bool HasPartner(Career career, out SimDescription partner)
        {
            partner = null;
            LawEnforcement enforcement = OmniCareer.Career<LawEnforcement> (career);
            if (enforcement != null)
            {
                partner = enforcement.Partner;
            }
            return (partner != null);
        }

        public override List<SimDescription> GetCoworkersKnown()
        {
            SimDescription description;
            if (HasPartner(Career, out description))
            {
                List<SimDescription> list = new List<SimDescription>(0x1);
                list.Add(description);
                return list;
            }
            return new List<SimDescription>(0x0);
        }

        public override bool ShouldAddTone(Career career)
        {
            SimDescription description;
            return HasPartner(career, out description);
        }
    }
}
