using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CareerSpace.Skills;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public static class Electrocute
    {
        public static bool CallbackTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return Assassination.Allow(actor, target, SimDescription.DeathType.Electrocution, isAutonomous, false, false, ref greyedOutTooltipCallback);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnAccepted(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                Kill.OnAccepted(actor, target, SimDescription.DeathType.Electrocution);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
