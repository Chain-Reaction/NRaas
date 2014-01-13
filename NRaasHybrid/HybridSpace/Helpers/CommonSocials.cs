using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using NRaas.HybridSpace.Interactions;
using NRaas.HybridSpace.Interfaces;
using NRaas.HybridSpace.MagicControls;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.Alchemy;
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
    public class CommonSocials : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP7))
            {
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestMagicWandEquipped"));
            }
        }

        public static bool TestMagicWandEquipped(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                MagicControl controlActor = MagicControl.GetBestControl(actor, SpellcastingDuelEx.Singleton as IMagicalDefinition);
                if (controlActor == null) return false;

                MagicControl controlTarget = MagicControl.GetBestControl(target, SpellcastingDuelEx.Singleton as IMagicalDefinition);
                if (controlTarget == null) return false;

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }
    }
}
