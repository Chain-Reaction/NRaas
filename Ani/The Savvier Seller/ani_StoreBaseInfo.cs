using Sims3.SimIFace;
using Sims3.Gameplay.CAS;

namespace ani_StoreSetBase
{
    [Persistable]
    public class ani_StoreBaseInfo
    {   
        public bool RestockBuyMode;
        public bool RestockCraftable;

        public ulong Owner;
        public string OwnerName;

        public int CooldownInMinutes;

        
        public ObjectGuid RegisterId;
        public string RegisterName;

        public ani_StoreBaseInfo()
        {
            RegisterId = ObjectGuid.InvalidObjectGuid;
            RestockBuyMode = true;
            RestockCraftable = false;
            CooldownInMinutes = 60;
        }
    }
}
