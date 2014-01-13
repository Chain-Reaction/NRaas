using NRaas.CareerSpace;
using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

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
