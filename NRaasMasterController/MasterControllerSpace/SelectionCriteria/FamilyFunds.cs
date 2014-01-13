using NRaas.MasterControllerSpace.Demographics;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class FamilyFunds : ByValueCriteria<FamilyFunds.Clumper>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.FamilyFunds";
        }

        public class Clumper : BaseClumper
        {
            public override int GetValue(Household house)
            {
                return house.FamilyFunds;
            }
        }
    }
}
