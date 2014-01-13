using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
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
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class SimTryForBaby
    {
        public static bool OnScoreNeutral(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnScoreFriendly(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return WoohooScoring.ScoreFriendly(actor, target, Common.IsAutonomous(actor), "InterestInTryForBaby");
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnScoreAwkward(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return WoohooScoring.ScoreAwkward(actor, target, Common.IsAutonomous(actor), "InterestInTryForBaby");
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnScoreBoring(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return WoohooScoring.ScoreBoring(actor, target, Common.IsAutonomous(actor), "InterestInTryForBaby");
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnScoreCreepy(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return WoohooScoring.ScoreCreepy(actor, target, Common.IsAutonomous(actor), "InterestInTryForBaby");
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnScoreInsulting(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return WoohooScoring.ScoreInsulting(actor, target, Common.IsAutonomous(actor), "InterestInTryForBaby");
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool PublicTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (!SimWoohoo.CommonTest(actor, target, ref greyedOutTooltipCallback)) return false;

            ScoringLookup.IncStat("OnTest TryForBaby Try");

            return CommonPregnancy.SatisfiesTryForBaby(actor, target, "OnTest TryForBaby", isAutonomous, false, ref greyedOutTooltipCallback);
        }

        public static bool OnTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (Woohooer.Settings.mEAStandardWoohoo)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("EA Ruleset Fail");
                    return false;
                }

                return PublicTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (Exception exception)
            {
                Common.Exception(actor, target, exception);
                return false;
            }
        }

        public static void OnAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                ScoringLookup.IncStat("OnAccept TryForBaby");

                if (Common.kDebugging)
                {
                    Woohooer.DebugNotify("TryForBaby" + Common.NewLine + actor.FullName + Common.NewLine + target.FullName, actor, target);
                }

                WooHooSocialInteraction woohooSocial = i as WooHooSocialInteraction;
                if (woohooSocial != null)
                {
                    woohooSocial.PushWooHoo(actor, target);
                }
                else
                {
                    new CommonWoohoo.PushWoohoo(actor, target, i.Autonomous, CommonWoohoo.WoohooStyle.TryForBaby);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
