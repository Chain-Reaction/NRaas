using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class SortInventory : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "SortInventory";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.Inventory == null) return false;

            return (!me.CreatedSim.Inventory.IsEmpty);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return (NRaas.MasterControllerSpace.Helpers.SortInventory.Perform(me.CreatedSim.Inventory, NRaas.MasterControllerSpace.Helpers.SortInventory.GetSortType(Name)) != OptionResult.Failure);
        }
    }
}
