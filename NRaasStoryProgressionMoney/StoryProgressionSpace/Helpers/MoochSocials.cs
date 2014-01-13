using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class MoochSocials : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            BooterLogger.AddError(SocialRHSReplacer.Perform<MoochSocials>("Mooch Money (Large)", "AfterMoochLargeMoney"));
            BooterLogger.AddError(SocialRHSReplacer.Perform<MoochSocials>("Mooch Money (Small)", "AfterMoochSmallMoney"));
        }

        public static void AfterMoochLargeMoney(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                AfterMoochMoney(actor, target, TraitTuning.MoochTraitLargeMoneyMoochSkill, Math.Max(0x0, actor.SkillManager.GetSkillLevel(SkillNames.Mooch)) + 0x2);
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

        public static void AfterMoochSmallMoney(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                AfterMoochMoney(actor, target, TraitTuning.MoochTraitSmallMoneyMoochSkill, Math.Max(0x0, actor.SkillManager.GetSkillLevel(SkillNames.Mooch)));
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

        public static void AfterMoochMoney(Sim actor, Sim target, float moochSkillPoints, int moochSkillLevel)
        {
            try
            {
                Skill skill = actor.SkillManager.AddElement(SkillNames.Mooch);
                if (actor.SimDescription.Child)
                {
                    skill.AddSkillPointsLevelClamped(moochSkillPoints, TraitTuning.MoochTraitChildSkill);
                }
                else if (actor.SimDescription.Teen)
                {
                    skill.AddSkillPointsLevelClamped(moochSkillPoints, TraitTuning.MoochTraitTeenSkill);
                }
                else
                {
                    skill.AddPoints(moochSkillPoints);
                }

                int delta = Mooch.MoochAmounts[moochSkillLevel];
                if (RandomUtil.RandomChance01(TraitTuning.MoochTraitMoneyQuadChance))
                {
                    delta *= 0x4;
                }
                else if (RandomUtil.RandomChance01(TraitTuning.MoochTraitMoneyDoubleChance))
                {
                    delta *= 0x2;
                }
                delta = (int)(delta * RandomUtil.RandomFloatGaussianDistribution(TraitTuning.MoochTraitMoneyRandomBegin, TraitTuning.MoochTraitMoneyRandomEnd));

                NRaas.StoryProgression.Main.Money.AdjustFunds(target.SimDescription, "GiveAway", -delta);

                NRaas.StoryProgression.Main.Money.AdjustFunds(actor.SimDescription, "GiveAway", delta);

                actor.ShowTNSIfSelectable(SocialCallback.LocalizeString(actor.IsFemale, "MoochMoney", new object[] { actor, delta, target }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, target.ObjectId);
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
