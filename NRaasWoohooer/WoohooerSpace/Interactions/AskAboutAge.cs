using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class AskAboutAge
    {
        public static bool OnTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if ((actor != Sim.ActiveActor) && (target != Sim.ActiveActor)) return false;

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
                int actorAge = (int)Aging.GetCurrentAgeInDays(actor.SimDescription);
                int targetAge = (int)Aging.GetCurrentAgeInDays(target.SimDescription);

                int difference = 0;
                string suffix = "SameResult";
                if (targetAge > actorAge)
                {
                    suffix = "OlderResult";
                    difference = targetAge - actorAge;
                }
                else if (targetAge < actorAge)
                {
                    suffix = "YoungerResult";
                    difference = actorAge - targetAge;
                }

                if (SimTypes.IsSelectable(actor))
                {
                    Common.Notify(Common.Localize("AskAboutAge:" + suffix, target.IsFemale, new object[] { difference }), target.ObjectId, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSimTalking);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
