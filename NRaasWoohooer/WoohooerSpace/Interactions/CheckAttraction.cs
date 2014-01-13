using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
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
    public class CheckAttraction
    {
        public static bool OnFriendlyTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (Woohooer.Settings.mInteractionsUnderRomance) return false;

                if ((actor != Sim.ActiveActor) && (target != Sim.ActiveActor)) return false;

                string reason = null;
                if (!CommonSocials.CanGetRomantic(actor, target, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason)) return false;

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

                string reason;
                if (!CommonSocials.CanGetRomantic(actor, target, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason)) return false;

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
                // Reset the attraction so it is recalculated now
                RelationshipEx.CalculateAttractionScore(Relationship.Get(target.SimDescription, actor.SimDescription, false), true);

                int score = (int)RelationshipEx.GetAttractionScore(target.SimDescription, actor.SimDescription, true);

                int index = score / 10;
                if (index >= 10)
                {
                    index = 9;
                }
                else if (index < 0)
                {
                    index = 0;
                }

                Common.Notify(Common.Localize("CheckAttraction:Result" + index, actor.IsFemale, target.IsFemale, new object[] { actor, target }), actor.ObjectId, target.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
