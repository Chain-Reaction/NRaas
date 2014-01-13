using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Object;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public abstract class PrimaryInteraction<T> : ListedInteraction<T, GameObject>
        where T : class, IInteractionOptionItem<IActor,GameObject,GameHitParameters<GameObject>>
    {       
        protected static bool IsVisible(GameObject target)
        {
            Lot lotTarget = target as Lot;
            if (lotTarget != null)
            {
                if (!lotTarget.IsBaseCampLotType)
                {
                    if (!MasterController.Settings.mMenuVisibleLot) return false;
                }
            }
            else if (target is Terrain)
            {
                if (!MasterController.Settings.mMenuVisibleLot) return false;
            }
            else if (target is Sim)
            {
                if (!MasterController.Settings.mMenuVisibleSim) return false;
            }
            else if (target is Sims3.Gameplay.Objects.Electronics.Computer)
            {
                if (!MasterController.Settings.mMenuVisibleComputer) return false;
            }

            return true;
        }

        public static bool PublicTest(IActor a, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (!IsVisible(target)) return false;

            if (!(target is Sim))
            {
                Lot lot = OptionItem.GetLot(target, hit);
                if (lot != null)
                {
                    if (!lot.IsResidentialLot)
                    {
                        if (lot.LastDisplayedLevel < 0) return false;
                    }
                }
            }

            return true;
        }

        protected override bool Test(IActor a, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return PublicTest(a, target, hit, ref greyedOutTooltipCallback);
        }
    }
}
