using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;

namespace NRaas.OverwatchSpace.Loadup
{
    public class ReplaceEP11BaseCampString : DelayedLoadupOption
    {
        public ReplaceEP11BaseCampString()
        { }

        public override void OnDelayedWorldLoadFinished()
        {
            foreach (Lot.CommercialSubTypeData data in Lot.sCommnunityTypeData)
            {
                if (data.CommercialLotSubType == CommercialLotSubType.kEP11_BaseCampFuture)
                {
                    data.LocalizationStringKey = "Gameplay/Excel/Venues/CommuntiyTypes:BaseCampEP11";
                    Overwatch.Log("Replaced EP11 Base Camp string");
                    break;
                }
            }
        }
    }
}
