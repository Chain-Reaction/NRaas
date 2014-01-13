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
    public class TransferFrom : OptionItem, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "TransferFrom";
        }

        public override string HotkeyID
        {
            get { return null; }
        }

        public static Inventory GetInventory(GameObject target)
        {
            Lot lot = target as Lot;
            if (lot != null)
            {
                if (lot.Household == null) return null;

                if (lot.Household.SharedFamilyInventory == null) return null;

                return lot.Household.SharedFamilyInventory.Inventory;
            }
            else                 
            {
                return target.Inventory;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!MasterController.Settings.mMenuVisibleTransferFrom) return false;

            if (parameters.mTarget is Sim) return false;

            if (GetInventory(parameters.mTarget) == null) return false;

 	        return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (NRaas.MasterControllerSpace.Sims.Basic.TransferTo.Perform(Name, parameters.mActor.IsFemale, GetInventory(parameters.mTarget), parameters.mActor.Inventory))
            {
                return OptionResult.SuccessClose;
            }
            else
            {
                return OptionResult.Failure;
            }
        }
    }
}
