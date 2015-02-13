using Sims3.SimIFace;
using Sims3.Gameplay.CAS;
using System.Collections.Generic;

namespace ani_taxcollector
{
    [Persistable]
    public class TaxInformation
    {
        public string Name;
        public int Funds;
        public float Multiplier;
        public List<Household> TaxableHouseholds;
        

        public TaxInformation()
        {
            Name = "#Name";
            Funds = 0;
            Multiplier = 1f;
            TaxableHouseholds = new List<Household>();
        }

    }
}
