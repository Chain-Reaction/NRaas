using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class RelationshipEx
    {
        static TraitNames[] sAttractionTraits = new TraitNames[] { TraitNames.Attractive, TraitNames.MasterOfSeduction };

        public static bool UnderEAControl(Relationship ths)
        {
            SimDescription simA = ths.SimDescriptionA;
            SimDescription simB = ths.SimDescriptionB;
            if ((((simA != null) && (simB != null)) && (SimTypes.IsSelectable(simA) || SimTypes.IsSelectable(simB))) && (simA.IsHuman && simB.IsHuman))
            {
                if ((simA.Service is GrimReaper) || (simB.Service is GrimReaper))
                {
                    return true;
                }
                else
                {
                    switch (ths.LTR.CurrentLTR)
                    {
                        case LongTermRelationshipTypes.Spouse:
                        case LongTermRelationshipTypes.Fiancee:
                            return false;
                    }

                    if (simA.CanHaveRomanceWith(simB) && simA.CheckAutonomousGenderPreference(simB))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static float GetAttractionScore(SimDescription simA, SimDescription simB, bool forceUpdate)
        {
            return GetAttractionScore(Relationship.Get(simA, simB, false), forceUpdate);
        }
        public static float GetAttractionScore(Relationship ths, bool forceUpdate)
        {
            if (ths == null) return 0;

            if ((forceUpdate) || (ths.AttractionScore == float.PositiveInfinity))
            {
                CalculateAttractionScore(ths, false);
                if (ths.AttractionScore == float.PositiveInfinity) return 0;
            }

            return ths.AttractionScore;
        }

        public static void CalculateAttractionScore(Relationship ths, bool displayNotice)
        {
            if (Common.AssemblyCheck.IsInstalled("NRaasChemistry")) return;

            if (ths == null) return;

            ths.AttractionScore = 0;

            //if (ths.AttractionScore == float.PositiveInfinity)
            {
                SimDescription simA = ths.SimDescriptionA;
                SimDescription simB = ths.SimDescriptionB;
                if ((simA != null) && (simB != null) && (simA.IsHuman) && (simB.IsHuman))
                {
                    string reason;
                    GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                    if (CommonSocials.CanGetRomantic(simA, simB, true, false, true, ref greyedOutTooltipCallback, out reason))
                    {
                        float score = RandomUtil.GetFloat(Relationship.kBaseRandomAttraction[0x0], Relationship.kBaseRandomAttraction[0x1]);

                        score += Woohooer.Settings.mAttractionBaseChanceScoringV3[PersistedSettings.GetSpeciesIndex(simA)];

                        float origScore = score;
                        float highScore = 0f;
                        float newScore = 0f;

                        switch (ths.LTR.CurrentLTR)
                        {
                            case LongTermRelationshipTypes.Spouse:
                            case LongTermRelationshipTypes.Fiancee:
                                if (score > 0)
                                {
                                    score *= 2;
                                }
                                else
                                {
                                    score = 0;
                                }
                                break;
                        }

                        newScore = score - origScore;
                        origScore = score;

                        Relationship.AttractionType attractionType = Relationship.AttractionType.None;
                        foreach (Trait traitA in simA.TraitManager.List)
                        {
                            if (traitA.IsReward) continue;

                            foreach (Trait traitB in simB.TraitManager.List)
                            {
                                if (traitB.IsReward) continue;

                                if (traitA.TraitGuid == traitB.TraitGuid)
                                {
                                    score += Relationship.kTraitModifier;
                                }
                                else if (TraitManager.DoTraitsConflict(traitA.Guid, traitB.Guid))
                                {
                                    score -= Relationship.kTraitModifier;
                                }
                            }
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Trait, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Trait;
                        }

                        Occupation occupation = simA.Occupation;
                        if (occupation != null)
                        {
                            score += occupation.CareerLevel * Relationship.kCareerBonusPerLevel;
                        }

                        Occupation occupation2 = simB.Occupation;
                        if (occupation2 != null)
                        {
                            score += occupation2.CareerLevel * Relationship.kCareerBonusPerLevel;
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Career, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Career;
                        }

                        foreach (Skill skill in simA.SkillManager.List)
                        {
                            score += skill.SkillLevel * Relationship.kSkillBonusPerLevel;
                        }

                        foreach (Skill skill2 in simB.SkillManager.List)
                        {
                            score += skill2.SkillLevel * Relationship.kSkillBonusPerLevel;
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Skill, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Skill;
                        }

                        List<OccultTypes> listA = OccultTypeHelper.CreateList(simA);
                        List<OccultTypes> listB = OccultTypeHelper.CreateList(simB);

                        foreach (OccultTypes typeA in listA)
                        {
                            if (listB.Contains(typeA))
                            {
                                score += Relationship.kBonusForMatchingOccult;
                            }
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Occult, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Occult;
                        }

                        CelebrityManager celebrityManagerA = simA.CelebrityManager;
                        if (celebrityManagerA != null)
                        {
                            score += celebrityManagerA.Level * Relationship.kCelebrityBonusPerLevel;
                        }

                        CelebrityManager celebrityManagerB = simB.CelebrityManager;
                        if (celebrityManagerB != null)
                        {
                            score += celebrityManagerB.Level * Relationship.kCelebrityBonusPerLevel;
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Celebrity, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Celebrity;
                        }

                        if (simA.Zodiac == simB.Zodiac)
                        {
                            score += Relationship.kMatchingSignsBonus;
                        }

                        if (simA.FavoriteColor == simB.FavoriteColor)
                        {
                            score += Relationship.kMatchingSignsBonus / 2;
                        }

                        if (simA.FavoriteFood == simB.FavoriteFood)
                        {
                            score += Relationship.kMatchingSignsBonus / 2;
                        }

                        if (simA.FavoriteMusic == simB.FavoriteMusic)
                        {
                            score += Relationship.kMatchingSignsBonus / 2;
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Astro, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Astro;
                        }

                        int familyFundsA = 0;

                        Household house = simA.Household;
                        if (house != null)
                        {
                            familyFundsA = house.FamilyFunds;
                        }

                        if (familyFundsA >= Relationship.kAttractedMoneyAmounts[0x2])
                        {
                            score += Relationship.kAttractedMoneyBonuses[0x2];
                        }
                        else if (familyFundsA >= Relationship.kAttractedMoneyAmounts[0x1])
                        {
                            score += Relationship.kAttractedMoneyBonuses[0x1];
                        }
                        else if (familyFundsA >= Relationship.kAttractedMoneyAmounts[0x0])
                        {
                            score += Relationship.kAttractedMoneyBonuses[0x0];
                        }

                        int familyFundsB = 0;

                        house = simB.Household;
                        if (house != null)
                        {
                            familyFundsB = house.FamilyFunds;
                        }
                        if (familyFundsB >= Relationship.kAttractedMoneyAmounts[0x2])
                        {
                            score += Relationship.kAttractedMoneyBonuses[0x2];
                        }
                        else if (familyFundsB >= Relationship.kAttractedMoneyAmounts[0x1])
                        {
                            score += Relationship.kAttractedMoneyBonuses[0x1];
                        }
                        else if (familyFundsB >= Relationship.kAttractedMoneyAmounts[0x0])
                        {
                            score += Relationship.kAttractedMoneyBonuses[0x0];
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Money, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Money;
                        }

                        foreach (TraitNames trait in sAttractionTraits)
                        {
                            if (simA.HasTrait(trait))
                            {
                                score += Relationship.kAttractionLifetimeRewardBonus;
                            }

                            if (simB.HasTrait(trait))
                            {
                                score += Relationship.kAttractionLifetimeRewardBonus;
                            }
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Attractive, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Attractive;
                        }

                        BuffManager buffManagerA = null;
                        if (simA.CreatedSim != null)
                        {
                            buffManagerA = simA.CreatedSim.BuffManager;
                        }

                        BuffManager buffManagerB = null;
                        if (simB.CreatedSim != null)
                        {
                            buffManagerB = simB.CreatedSim.BuffManager;
                        }

                        foreach (BuffNames posBuff in Relationship.kPositiveBuffList)
                        {
                            if ((buffManagerA != null) && (buffManagerA.HasElement(posBuff)))
                            {
                                score += Relationship.kPerBuffModifier;
                            }
                            if ((buffManagerB != null) && (buffManagerB.HasElement(posBuff)))
                            {
                                score += Relationship.kPerBuffModifier;
                            }
                        }

                        foreach (BuffNames negBuff in Relationship.kNegativeBuffList)
                        {
                            if ((buffManagerA != null) && (buffManagerA.HasElement(negBuff)))
                            {
                                score -= Relationship.kPerBuffModifier;
                            }
                            if ((buffManagerB != null) && (buffManagerB.HasElement(negBuff)))
                            {
                                score -= Relationship.kPerBuffModifier;
                            }
                        }

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Buffs, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Buffs;
                        }

                        score += simA.Fitness * 25;
                        score += simB.Fitness * 25;

                        if (simA.Weight < 0)
                        {
                            score += 25 - simA.Weight * -25;
                        }
                        else
                        {
                            score += 25 - simA.Weight * 25;
                        }

                        if (simB.Weight < 0)
                        {
                            score += 25 - simB.Weight * -25;
                        }
                        else
                        {
                            score += 25 - simB.Weight * 25;
                        }

                        int ageA = 0;
                        switch (simA.Age)
                        {
                            case CASAgeGenderFlags.Teen:
                                ageA = 1;
                                break;
                            case CASAgeGenderFlags.YoungAdult:
                                ageA = 2;
                                break;
                            case CASAgeGenderFlags.Adult:
                                ageA = 3;
                                break;
                            case CASAgeGenderFlags.Elder:
                                ageA = 4;
                                break;
                        }

                        int ageB = 0;
                        switch (simB.Age)
                        {
                            case CASAgeGenderFlags.Teen:
                                ageB = 1;
                                break;
                            case CASAgeGenderFlags.YoungAdult:
                                ageB = 2;
                                break;
                            case CASAgeGenderFlags.Adult:
                                ageB = 3;
                                break;
                            case CASAgeGenderFlags.Elder:
                                ageB = 4;
                                break;
                        }

                        score -= Math.Abs(ageA - ageB) * 25;

                        newScore = score - origScore;
                        origScore = score;

                        if (ths.TestWasNewHighScore(Relationship.AttractionType.Physical, newScore, ref highScore))
                        {
                            attractionType = Relationship.AttractionType.Physical;
                        }

                        ths.AttractionScore = score;

                        if (Common.kDebugging)
                        {
                            Common.DebugNotify(simA.FullName + Common.NewLine + simB.FullName + Common.NewLine + "Attraction: " + score + Common.NewLine + attractionType);
                        }

                        if ((ths.AreAttracted) && ((SimTypes.IsSelectable(simA)) || (SimTypes.IsSelectable(simB))))
                        {
                            if (AttractionHelper.TestEnableAttractionNPCController(ths))
                            {
                                if (ths.AttractionNPCController == null)
                                {
                                    ths.AttractionNPCController = new AttractionNPCBehaviorController(ths);
                                }
                            }
                            else
                            {
                                if (ths.AttractionNPCController != null)
                                {
                                    ths.AttractionNPCController.Dispose();
                                }
                            }

                            switch (ths.LTR.CurrentLTR)
                            {
                                case LongTermRelationshipTypes.Spouse:
                                case LongTermRelationshipTypes.Fiancee:
                                case LongTermRelationshipTypes.Ex:
                                case LongTermRelationshipTypes.ExSpouse:
                                case LongTermRelationshipTypes.Partner:
                                case LongTermRelationshipTypes.RomanticInterest:
                                    break;
                                default:
                                    if (displayNotice)
                                    {
                                        Sim createdSimA = simA.CreatedSim;
                                        Sim createdSimB = simB.CreatedSim;
                                        if ((createdSimA != null) && (createdSimB != null))
                                        {
                                            if ((createdSimA.LotCurrent == createdSimB.LotCurrent) && (createdSimA.RoomId == createdSimB.RoomId))
                                            {
                                                VisualEffect effect = VisualEffect.Create("ep8AttractionSystem");
                                                createdSimB.ParentHeadlineFx(effect);
                                                effect.SubmitOneShotEffect(VisualEffect.TransitionType.SoftTransition);

                                                VisualEffect effect2 = VisualEffect.Create("ep8AttractionSystem");
                                                createdSimA.ParentHeadlineFx(effect2);
                                                effect2.SubmitOneShotEffect(VisualEffect.TransitionType.SoftTransition);

                                                ThoughtBalloonManager.BalloonData bd = new ThoughtBalloonManager.BalloonData(createdSimA.GetThumbnailKey());
                                                bd.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                                                createdSimB.ThoughtBalloonManager.ShowBalloon(bd);

                                                ThoughtBalloonManager.BalloonData data2 = new ThoughtBalloonManager.BalloonData(createdSimB.GetThumbnailKey());
                                                data2.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                                                createdSimA.ThoughtBalloonManager.ShowBalloon(data2);

                                                TNSNames reasonTNS = ths.GetReasonTNS(attractionType);
                                                if (reasonTNS != TNSNames.None)
                                                {
                                                    Sim target = createdSimB.IsSelectable ? createdSimB : createdSimA;
                                                    Sim actor = (createdSimB == target) ? createdSimA : createdSimB;
                                                    target.ShowTNSIfSelectable(reasonTNS, actor, target, new object[] { actor, target });
                                                }

                                                EventTracker.SendEvent(EventTypeId.kMeetAttractiveSim, createdSimB);
                                                EventTracker.SendEvent(EventTypeId.kMeetAttractiveSim, createdSimA);
                                                Audio.StartObjectSound(createdSimB.IsSelectable ? createdSimB.ObjectId : createdSimA.ObjectId, "sting_attraction", false);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        else if (ths.AttractionNPCController != null)
                        {
                            ths.AttractionNPCController.Dispose();
                        }
                    }
                    else
                    {
                        if (greyedOutTooltipCallback != null)
                        {
                            Common.DebugNotify(simA.FullName + Common.NewLine + simB.FullName + Common.NewLine + greyedOutTooltipCallback());
                        }
                    }
                }
            }

            if (ths.AttractionNPCController != null)
            {
                ths.AttractionNPCController.Dispose();
            }
        }
    }
}
