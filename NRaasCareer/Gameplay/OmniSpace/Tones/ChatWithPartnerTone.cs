using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using System.Collections.Generic;

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
