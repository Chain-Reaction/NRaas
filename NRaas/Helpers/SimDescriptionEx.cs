using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;

namespace NRaas.CommonSpace.Helpers
{
    public class SimDescriptionEx
    {
        // Cutdown version of function
        public static bool ImportContent(SimDescription ths, ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            IPropertyStreamReader child = reader.GetChild(0xe584e282);
            if (child != null)
            {
                if (ths.CareerManager != null)
                {
                    CareerManagerEx.ImportContent(ths.CareerManager, resKeyTable, objIdTable, child);
                }
            }

            return true;
        }
    }
}