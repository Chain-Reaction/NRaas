using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;

namespace NRaas.CommonSpace.Helpers
{
    public class HouseholdMemberEx
    {
        // Cutdown version of function
        public static bool ImportContent(Household.Members ths, ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            try
            {
                Household.sRemapSimDescriptionIdsOnImport = !Household.IsTravelImport;

                int memberCount;
                reader.ReadInt32(0xe9b96005, out memberCount, 0);
                for (uint i = 0; i < memberCount; i++)
                {
                    IPropertyStreamReader child = reader.GetChild(i);
                    if (child != null)
                    {
                        ulong simDescriptionId;
                        child.ReadUint64(0x687720a6, out simDescriptionId, 0L);

                        SimDescription simDescription = null;
                        ulong newId = simDescriptionId;
                        if ((Household.sRemapSimDescriptionIdsOnImport) && (Household.sOldIdToNewSimDescriptionMap != null))
                        {
                            if (!Household.sOldIdToNewSimDescriptionMap.TryGetValue(simDescriptionId, out simDescription))
                            {
                                simDescription = null;
                            }
                        }
                        else
                        {
                            foreach (SimDescription sim in ths.AllSimDescriptionList)
                            {
                                if (sim.SimDescriptionId == newId)
                                {
                                    simDescription = sim;
                                    break;
                                }
                            }
                        }

                        if (simDescription != null)
                        {
                            SimDescriptionEx.ImportContent(simDescription, resKeyTable, objIdTable, child);
                        }
                    }
                }
            }
            finally
            {
                Household.sRemapSimDescriptionIdsOnImport = false;
            }

            return true;
        }
    }
}
