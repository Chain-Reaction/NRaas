using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Helpers
{
    public class CastSpellEx
    {
        public static bool CommonSpellTests(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            /*
            if (!a.HasTrait(TraitNames.WitchHiddenTrait))
            {
                return false;
            }
            */
            if (target.SimDescription.IsEP11Bot)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Plumbot Fail");
                return false;
            }
            if (!a.Posture.AllowsNormalSocials())
            {
                greyedOutTooltipCallback = Common.DebugTooltip("NormalSocials Fail");
                return false;
            }
            if (!target.Posture.AllowsNormalSocials())
            {
                greyedOutTooltipCallback = Common.DebugTooltip("NormalSocials Fail");
                return false;
            }
            if (!a.SimDescription.ShowSocialsOnSim)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("ShowSocialsOnSim Fail");
                return false;
            }
            if (!target.SimDescription.IsZombie)
            {
                if (!target.SimDescription.ShowSocialsOnSim)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("ShowSocialsOnSim Fail");
                    return false;
                }
                if (!target.CanBeSocializedWith || !a.CanBeSocializedWith)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("CanBeSocializedWith Fail");
                    return false;
                }
            }
            if (SocialComponent.IsInServicePreventingSocialization(target))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("IsInServicePreventingSocialization Fail");
                return false;
            }
            if ((target.SimDescription.AssignedRole != null) && (((target.SimDescription.AssignedRole.Type == Role.RoleType.LocationMerchant) || (target.SimDescription.AssignedRole.Type == Role.RoleType.GenericMerchant)) || (((target.SimDescription.AssignedRole.Type == Role.RoleType.PetStoreMerchant) || (target.SimDescription.AssignedRole.Type == Role.RoleType.HobbyShopMerchant)) || (target.SimDescription.AssignedRole.Type == Role.RoleType.PotionShopMerchant))))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Merchant Fail");
                return false;
            }
            if ((target.Service != null) && (target.Service is GrimReaper))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Reaper Fail");
                return false;
            }
            
            IPreventSocialization currentInteraction = target.CurrentInteraction as IPreventSocialization;
            if ((currentInteraction != null) && !currentInteraction.SocializationAllowed(a, target))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("SocializationAllowed Fail");
                return false;
            }

            if (target.SimDescription.ChildOrBelow || target.SimDescription.IsPet)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Child/Pet Fail");
                return false;
            }
            return true;
        }
    }
}
