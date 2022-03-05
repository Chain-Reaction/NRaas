using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.Gameplay.Careers
{
    [Persistable]
    public class PartTimeJob : OmniCareer
    {
        public PartTimeJob()
        { }
        public PartTimeJob(XmlDbRow myRow, XmlDbTable levelTable, XmlDbTable eventDataTable)
            : base(myRow, levelTable, eventDataTable)
        { }

        public override bool CanRetire()
        {
            return false;
        }

        public override bool CareerAgeTest(SimDescription sim)
        {
            return sim.TeenOrAbove;
        }

        public override bool ExportContent(IPropertyStreamWriter writer)
        {
            return true;
        }

        public override bool ImportContent(IPropertyStreamReader reader)
        {
            return true;
        }
    }
}
