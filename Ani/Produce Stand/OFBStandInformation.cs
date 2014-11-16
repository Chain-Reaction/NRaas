using Sims3.SimIFace;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;

namespace ani_OFBStand
{
    [Persistable]
    public class OFBStandInformation
    {
        public SimDescription Owner;
        public SimDescription Employee;

        public bool IsStandOpen;
        public bool AlwayCanBuy;
        public bool ChangeToWorkOutfit;
        public bool PayWageFromOwnersFunds;

        public float StartTime;
        public float EndTime;
        
        public int PayPerHour;
        public float PriceIncrease;
        public int ServingPrice;

        public int CooldownInMinutes;
               

        public OFBStandInformation()
        {
            IsStandOpen = false;
            AlwayCanBuy = false;
            ChangeToWorkOutfit = false;
            PayWageFromOwnersFunds = true;
            StartTime = 7f;
            EndTime = 18f;
            PayPerHour = 0;
            PriceIncrease = 1f;
            CooldownInMinutes = 60;
            ServingPrice = 10;
        }
    }
}
