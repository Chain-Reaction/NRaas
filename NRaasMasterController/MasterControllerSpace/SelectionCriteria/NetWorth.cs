using NRaas.MasterControllerSpace.Demographics;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.RealEstate;
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
    public class NetWorth : ByValueCriteria<NetWorth.Clumper>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.NetWorth";
        }

        public class Clumper : BaseClumper
        {
            public override int GetValue(Household house)
            {
                int value = 0;

                if (house.LotHome != null)
                {
                    value = house.FamilyFunds + house.LotHome.Cost;
                }
                else
                {
                    value = house.NetWorth();
                }

                if (house.RealEstateManager != null)
                {
                    foreach (PropertyData data in house.RealEstateManager.AllProperties)
                    {
                        value += data.TotalValue;
                    }
                }

                return value;
            }
        }
    }
}
