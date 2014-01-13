using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Booters;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Options
{
    public class TestDescendantScoringSetting : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "TestDescendantScoring";
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters< GameObject> parameters)
        {
            if (!Common.kDebugging) return false;

            if (FutureDescendantService.GetInstance() == null) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< GameObject> parameters)
        {
            Common.StringBuilder results = new Common.StringBuilder("BuildDescendantHouseholdSpecs");

            FutureDescendantService instance = FutureDescendantService.GetInstance();

            instance.CleanUpFutureDescendantService(true);
            instance.InitializeFutureDescendantService();

            FutureDescendantServiceEx.BuildDescendantHouseholdSpecs(instance);

            foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo)
            {
                if (info.HasAncestorFromHousehold(Household.ActiveHousehold))
                {
                    FutureDescendantHouseholdInfoEx.CalculateHouseholdFamilyScore(info, results);
                }
            }

            Common.DebugWriteLog(results);

            return OptionResult.SuccessClose;
        }
    }
}
