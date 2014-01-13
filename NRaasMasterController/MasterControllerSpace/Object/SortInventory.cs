using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Object
{
    public class SortInventory : OptionItem, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "SortInventory";
        }

        public override string HotkeyID
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!MasterController.Settings.mMenuVisibleSortInventory) return false;

            if (parameters.mTarget is Sim) return false;

            if (parameters.mTarget is Lot) return false;

            if (parameters.mTarget is Mailbox) return false;

            if (parameters.mTarget.Inventory == null) return false;

 	        return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return NRaas.MasterControllerSpace.Helpers.SortInventory.Perform(parameters.mTarget.Inventory, NRaas.MasterControllerSpace.Helpers.SortInventory.GetSortType(Name));
        }
    }
}
