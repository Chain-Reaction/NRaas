using Sims3.SimIFace;
using System.Collections.Generic;

namespace ani_BistroSet
{
    [Persistable]
    public class OFBOvenInfo
    {
        public bool Open;
        public bool PayWhenActive;
        public List<Shift> Shifts;

        public OFBOvenInfo()
        {
            Open = false;
            PayWhenActive = true;
            Shifts = new List<Shift>();
        }
    }
}
