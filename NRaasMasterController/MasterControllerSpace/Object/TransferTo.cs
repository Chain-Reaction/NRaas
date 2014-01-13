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
    public class TransferTo : OptionItem, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "TransferTo";
        }

        public override string HotkeyID
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!MasterController.Settings.mMenuVisibleTransferTo) return false;

            if (parameters.mTarget is Sim) return false;

            if (TransferFrom.GetInventory(parameters.mTarget) == null) return false;

 	        return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (NRaas.MasterControllerSpace.Sims.Basic.TransferTo.Perform(Name, parameters.mActor.IsFemale, parameters.mActor.Inventory, TransferFrom.GetInventory(parameters.mTarget)))
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
