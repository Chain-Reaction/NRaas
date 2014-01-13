using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class JetpackRisky
    {
        public static bool OnTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Jetpack.JetpackTestCommon(actor, target))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("TestCommon Denied");
                    return false;
                }

                if (actor.SkillManager.GetSkillLevel(SkillNames.Future) < SocialTest.kFutureSkillRequiredJetpackWoohoo)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Skill Denied");
                    return false;
                }

                SocialJig socialjig = null;
                if (!Jetpack.CheckSpaceForFlyAroundJig(actor, target, ref socialjig, true, true))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/Objects/EP11/Jetpack:NotEnoughSpace", new object[] { target }));
                    return false;
                }

                if (!Woohooer.Settings.mAutonomousJetPack)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Location Denied");
                    return false;
                }

                return CommonPregnancy.SatisfiesRisky(actor, target, "JetPackRisky", isAutonomous, true, ref greyedOutTooltipCallback);
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

        public static void OnAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                actor.Motives.SetDecay(CommodityKind.Fun, true);
                target.Motives.SetDecay(CommodityKind.Fun, true);
                actor.Motives.ChangeValue(CommodityKind.Fun, Jetpack.kFunGainJetPackWoohoo);
                target.Motives.ChangeValue(CommodityKind.Fun, Jetpack.kFunGainJetPackWoohoo);

                if (CommonPregnancy.IsSuccess(actor, target, i.Autonomous, CommonWoohoo.WoohooStyle.Risky))
                {
                    CommonPregnancy.Impregnate(actor, target, i.Autonomous, CommonWoohoo.WoohooStyle.Risky);
                }

                CommonWoohoo.RunPostWoohoo(actor, target, actor.GetActiveJetpack(), CommonWoohoo.WoohooStyle.Risky, CommonWoohoo.WoohooLocation.Jetpack, true);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
