using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class TransferFrom : DualSimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "TransferFrom";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("TransferItem:Source");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("TransferItem:Destination");
        }

        protected override bool AllowSpecies(IMiniSimDescription a, IMiniSimDescription b)
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.Inventory == null) return false;

            return base.PrivateAllow(me);
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            if (!base.PrivateAllow(a, b)) return false;

            if (a == null) return false;

            if (b == null) return false;

            if (a == b) return false;

            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            return TransferTo.Perform(Name, a.IsFemale, a.CreatedSim.Inventory, b.CreatedSim.Inventory);
        }
    }
}
