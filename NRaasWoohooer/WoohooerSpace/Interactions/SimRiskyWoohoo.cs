using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class SimRiskyWoohoo
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
                return WoohooScoring.ScoreFriendly(actor, target, Common.IsAutonomous(actor), "InterestInRisky");
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
                return WoohooScoring.ScoreAwkward(actor, target, Common.IsAutonomous(actor), "InterestInRisky");
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
                return WoohooScoring.ScoreBoring(actor, target, Common.IsAutonomous(actor), "InterestInRisky");
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
                return WoohooScoring.ScoreCreepy(actor, target, Common.IsAutonomous(actor), "InterestInRisky");
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
                return WoohooScoring.ScoreInsulting(actor, target, Common.IsAutonomous(actor), "InterestInRisky");
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        private static bool Test(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (!SimWoohoo.CommonTest(actor, target, ref greyedOutTooltipCallback)) return false;

            ScoringLookup.IncStat("OnTest Risky Try");

            return CommonPregnancy.SatisfiesRisky(actor, target, "OnTest Risky", isAutonomous, false, ref greyedOutTooltipCallback);
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

                if (Woohooer.Settings.ReplaceWithRisky)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Replace With Risky Fail");
                    return false;
                }

                return Test(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
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
                ScoringLookup.IncStat("OnAccept Risky");

                if (Common.kDebugging)
                {
                    Woohooer.DebugNotify("Risky" + Common.NewLine + actor.FullName + Common.NewLine + target.FullName, actor, target);
                }

                WooHooSocialInteraction woohooSocial = i as WooHooSocialInteraction;
                if (woohooSocial != null)
                {
                    woohooSocial.PushWooHoo(actor, target);
                }
                else
                {
                    new CommonWoohoo.PushWoohoo(actor, target, i.Autonomous, CommonWoohoo.WoohooStyle.Risky);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
