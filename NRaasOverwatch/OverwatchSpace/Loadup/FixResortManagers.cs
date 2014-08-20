using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class FixResortManagers : ImmediateLoadupOption
    {
        public override string GetTitlePrefix()
        {
            return "FixResortManagers";
        }

        public override void OnWorldLoadFinished()
        {
            Overwatch.Log(GetTitlePrefix());

            try
            {
                bool fix = false;
                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot.ResortManager != null)
                    {
                        if (lot.ResortManager.mOwnerLot == null || LotManager.GetLot(lot.ResortManager.mOwnerLot.LotId) == null)
                        {
                            fix = true;
                            lot.ResortManager.mOwnerLot = lot;
                        }
                    }
                }

                if (fix)
                {
                    Overwatch.Log("Fixed resort managers");
                }
            }
            catch (Exception e)
            {
                Common.Exception(GetTitlePrefix(), e);
            }
        }
    }
}