using NRaas.CommonSpace.Options;
using NRaas.CupcakeSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Store.Objects;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace.Options.Displays
{
    public class EmptyDisplay : OperationSettingOption<GameObject>, ICaseOption
    {
        IGameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "EmptyDisplay";
        }        

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            if (mTarget == null) return false;

            if (!(mTarget is CraftersConsignmentDisplay)) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            DisplayHelper.DisplayTypes type;
            Dictionary<int, Slot> slots = DisplayHelper.GetEmptyOrFoodSlots(mTarget as CraftersConsignmentDisplay, out type);

            foreach (KeyValuePair<int, Slot> slot in slots)
            {
                IGameObject obj = mTarget.GetContainedObject(slot.Value);
                if (obj != null)
                {
                    obj.UnParent();
                    obj.Destroy();
                }
            }

            Common.Notify(Common.Localize("General:Success"));

            return OptionResult.SuccessClose;
        }
    }
}