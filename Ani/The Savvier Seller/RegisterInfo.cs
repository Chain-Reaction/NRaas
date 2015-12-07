using Sims3.SimIFace;
using System.Collections.Generic;
using Sims3.Gameplay.CAS;

namespace ani_StoreSetRegister
{
    [Persistable]
    public class RegisterInfo
    {
        public ulong OwnerId;
        public string OwnerName;
        public string RegisterName;

        public ulong CurrentCustomer;

        public int ServingPrice;

        public bool Open;
        public int HourlyWage;
        public bool PayWhenActive;
        public bool ChangeToWorkOutfit;

        public Dictionary<SimDescription, int> ShoppingData;

        public RegisterInfo()
        {
            OwnerId = 0uL;
            Open = true;
            OwnerName = string.Empty;
            RegisterName = "Register #";
            ServingPrice = 25;
            HourlyWage = 10;
            PayWhenActive = true;
            ChangeToWorkOutfit = true;
            ShoppingData = new Dictionary<SimDescription, int>();
        }
    }
}
