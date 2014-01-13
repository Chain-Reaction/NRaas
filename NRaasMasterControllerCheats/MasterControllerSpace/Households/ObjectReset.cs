using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Town;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class ObjectReset : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "ObjectReset";
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            return (lot != null);
        }

        protected override OptionResult RunAll(List<LotHouseItem> houses)
        {
            List<Lot> lots = new List<Lot>();

            foreach (LotHouseItem lotHome in houses)
            {
                if (lotHome.mLot == null) continue;

                lots.Add(lotHome.mLot);
            }

            return new LotProcessor(GetTitlePrefix(), lots).Perform(OnReset);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            return OptionResult.Failure;
        }

        public static bool OnReset(IGameObject obj)
        {
            Households.Reset.ResetObject(obj, false);
            return true;
        }
    }
}
