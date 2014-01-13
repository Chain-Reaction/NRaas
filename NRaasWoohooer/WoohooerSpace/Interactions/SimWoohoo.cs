using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class SimWoohoo
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
                return WoohooScoring.ScoreFriendly(actor, target, Common.IsAutonomous(actor), "InterestInWoohoo");
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
                return WoohooScoring.ScoreAwkward(actor, target, Common.IsAutonomous(actor), "InterestInWoohoo");
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
                return WoohooScoring.ScoreBoring(actor, target, Common.IsAutonomous(actor), "InterestInWoohoo");
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
                return WoohooScoring.ScoreCreepy(actor, target, Common.IsAutonomous(actor), "InterestInWoohoo");
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
                return WoohooScoring.ScoreInsulting(actor, target, Common.IsAutonomous(actor), "InterestInWoohoo");
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool CommonTest(Sim actor, Sim target, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if ((actor.LotCurrent == null) || (actor.LotCurrent.IsWorldLot))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Not Current Lot");
                return false;
            }
            else if (!CommonWoohoo.HasWoohooableObject(actor.LotCurrent, actor, target))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("No Woohoo Locations");
                return false;
            }
            else if ((actor.Posture != null) && (actor.Posture.Satisfies(CommodityKind.Relaxing, target)))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Actor Posture Fail");
                return false;
            }
            else if ((target.Posture != null) && (target.Posture.Satisfies(CommodityKind.Relaxing, actor)))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Target Posture Fail");
                return false;
            }

            return true;
        }

        public static bool PublicTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (!CommonTest(actor, target, ref greyedOutTooltipCallback)) return false;

            ScoringLookup.IncStat("OnTest Woohoo Try");

            return CommonWoohoo.SatisfiesWoohoo(actor, target, "OnTest Woohoo", isAutonomous, false, true, ref greyedOutTooltipCallback);
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
                ScoringLookup.IncStat("OnAccept Woohoo");

                if (Common.kDebugging)
                {
                    Woohooer.DebugNotify("Woohoo" + Common.NewLine + actor.FullName + Common.NewLine + target.FullName, actor, target);
                }

                WooHooSocialInteraction woohooSocial = i as WooHooSocialInteraction;
                if (woohooSocial != null)
                {
                    woohooSocial.PushWooHoo(actor, target);
                }
                else
                {
                    new CommonWoohoo.PushWoohoo(actor, target, i.Autonomous, CommonWoohoo.WoohooStyle.Safe);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
