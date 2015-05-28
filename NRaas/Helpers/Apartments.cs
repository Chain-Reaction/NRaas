using Sims3.Gameplay.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class Apartments
    {
        static Common.MethodStore sGetApartmentDescriptionsBySimID = new Common.MethodStore("AniApartmentMod", "Ani.ApartmentMod", "GetApartmentDescriptionsBySimID", new Type[] { typeof(ulong) }); // List<SimDescription>
        static Common.MethodStore sGetApartmentDescriptionsByAptID = new Common.MethodStore("AniApartmentMod", "Ani.ApartmentMod", "GetApartmentDescriptionsByAptID", new Type[] { typeof(ulong) }); // List<SimDescription>
        static Common.MethodStore sGetApartmentIDForSim = new Common.MethodStore("AniApartmentMod", "Ani.ApartmentMod", "GetApartmentIDForSim", new Type[] { typeof(ulong) }); // ulong
        static Common.MethodStore sGetApartmentIDsForLot = new Common.MethodStore("AniApartmentMod", "Ani.ApartmentMod", "GetApartmentIDsForLot", new Type[] { typeof(ulong) }); // List<ulong>
        static Common.MethodStore sGetApartmentLotID = new Common.MethodStore("AniApartmentMod", "Ani.ApartmentMod", "GetLotIDByAptID", new Type[] { typeof(ulong) }); // ulong

        public static ulong GetLotIDByAptID(ulong id)
        {
            ulong lotId = 0;
            if (id != 0 && sGetApartmentLotID.Valid)
            {
                lotId = sGetApartmentLotID.Invoke<ulong>(new object[] { id });
            }

            return lotId;
        }

        public static ulong GetApartmentIDBySim(SimDescription id)
        {
            ulong aptId = 0;

            if (id == null || id.LotHome == null) return aptId;

            if (sGetApartmentIDForSim.Valid)
            {
                aptId = sGetApartmentDescriptionsBySimID.Invoke<ulong>(new object[] { id.SimDescriptionId });
            }
            else
            {
                aptId = id.LotHome.LotId;
            }

            return aptId;
        }

        public static List<ulong> GetApartmentIDsByLotID(ulong id)
        {
            List<ulong> aptIds = new List<ulong>();
            if (id != 0 && sGetApartmentDescriptionsByAptID.Valid)
            {
                aptIds = sGetApartmentDescriptionsByAptID.Invoke<List<ulong>>(new object[] { id });
            }
            else
            {
                aptIds = new List<ulong> { id };
            }

            return aptIds;
        }
        
        public static List<SimDescription> GetApartmentDescriptionsBySim(SimDescription id)
        {
            List<SimDescription> results = new List<SimDescription>();

            if (id == null || id.Household == null) return results;

            if (sGetApartmentDescriptionsBySimID.Valid)
            {
                results = Apartments.sGetApartmentDescriptionsBySimID.Invoke<List<SimDescription>>(new object[] { id.SimDescriptionId });
            }
            else
            {
                results.AddRange(id.Household.SimDescriptions);
            }

            return results;
        }

        public static List<SimDescription> GetApartmentDescriptionsByApartmentID(ulong id)
        {
            List<SimDescription> results = new List<SimDescription>();
            if (id != 0 && sGetApartmentDescriptionsByAptID.Valid)
            {
                results = Apartments.sGetApartmentDescriptionsByAptID.Invoke<List<SimDescription>>(new object[] { id });
            }

            return results;
        }
    }
}
