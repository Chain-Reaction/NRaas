﻿using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using System;

namespace NRaas.CareerSpace.Interactions
{
    public static class AskAboutFunds
    {
        public static bool CallbackTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (target.Household == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Household");
                    return false;
                }

                if (target.Household == actor.Household)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Same Household");
                    return false;
                }

                if (target.Household.IsSpecialHousehold)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Special Household");
                    return false;
                }

                return true;
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
                if ((actor.Household == Household.ActiveHousehold) || (target.Household == Household.ActiveHousehold))
                {
                    Common.Notify(Common.Localize("AskAboutFunds:Result", target.IsFemale, new object[] { target, target.FamilyFunds }), target.ObjectId);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
