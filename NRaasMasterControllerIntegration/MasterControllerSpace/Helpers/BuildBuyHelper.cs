using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class BuildBuyHelper : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            LotManager.EnteringBuildBuyMode += new VoidEventHandler(OnEnterBuildBuy);
        }

        public static void OnEnterBuildBuy()
        {
            if(((LotManager.sActiveBuildBuyLot.Household != null && (LotManager.sActiveBuildBuyLot.Household.RealEstateManager != null && LotManager.sActiveBuildBuyLot.Household.RealEstateManager.FindProperty(LotManager.sActiveBuildBuyLot.LotId) == null) && (Household.ActiveHousehold != LotManager.sActiveBuildBuyLot.Household))) || !GameObject.IsBuildBuyRestrictedForStageSetup())
            {
                LotManager.sFamilyFundsDisabled = true;
            }
        }
    }
}
