using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class GenderPreference
    {
        public static bool OnFriendlyTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (Woohooer.Settings.mInteractionsUnderRomance) return false;

                if ((actor != Sim.ActiveActor) && (target != Sim.ActiveActor)) return false;

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnRomanceTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Woohooer.Settings.mInteractionsUnderRomance) return false;

                if ((actor != Sim.ActiveActor) && (target != Sim.ActiveActor)) return false;

                if (!CommonSocials.OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                string suffix = null;

                if ((target.SimDescription.mGenderPreferenceMale > 0) && (target.SimDescription.mGenderPreferenceFemale > 0))
                {
                    suffix = "Bi";
                }
                else if ((target.SimDescription.mGenderPreferenceMale == 0) && (target.SimDescription.mGenderPreferenceFemale == 0))
                {
                    suffix = "Undefined";
                }
                else if ((target.SimDescription.mGenderPreferenceMale <= 0) && (target.SimDescription.mGenderPreferenceFemale <= 0))
                {
                    suffix = "Celibate";
                }
                else if (target.IsFemale)
                {
                    if (target.SimDescription.mGenderPreferenceMale > 0)
                    {
                        suffix = "Hetro";
                    }
                    else
                    {
                        suffix = "Same";
                    }
                }
                else
                {
                    if (target.SimDescription.mGenderPreferenceMale > 0)
                    {
                        suffix = "Same";
                    }
                    else
                    {
                        suffix = "Hetro";
                    }
                }

                Common.Notify(Common.Localize("GenderPreference:" + suffix, target.IsFemale, new object[] { target }), target.ObjectId, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSimTalking);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
