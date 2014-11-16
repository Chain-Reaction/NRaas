using System.Collections.Generic;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;
using System;
using Sims3.Gameplay.CAS;

namespace TS3Apartments
{
    [Persistable]
    public class ApartmentFamily 
    {
        public bool IsActive;
        public ulong FamilyId;
        public string FamilyName;
        public int FamilyFunds;
        public int UnpaidBills;
        public int Rent;
        public List<SimDescription> Residents;
        public List<MinorPet> MinorPets;
       
        /// <summary>
        /// Constructor
        /// </summary>
        public ApartmentFamily()
        {
            Random r = new Random();
            FamilyId = ((ulong) r.Next());           
            Residents = new List<SimDescription>();
            MinorPets = new List<MinorPet>();
            IsActive = false;
            FamilyName = FamilyId.ToString();
            FamilyFunds = 0;
            Rent = 0;
            UnpaidBills = 0;

            

        }
    }
}
