using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class EnterEquestrianCompetitionEx : EquestrianCenter.EnterEquestrianCompetition, Common.IPreLoad
    {
        public void OnPreLoad()
        {
            Tunings.Inject<EquestrianCenter, EquestrianCenter.EnterEquestrianCompetition.Definition, Definition>(false);
        }

        private void ShowCompetitionEndScoreEx(CompetitionType type, CompetitionLevel level, string competitionName)
        {
            mCurrentScore += mMoodScoreBonus;
            int currentMoneyReward = 0x0;
            List<EquestrianCenter.PositionScoring> positionScoringTuning = EquestrianCenter.PositionScoringTuning;
            if (mCompetitionType == CompetitionType.Racing)
            {
                RaceShuffle();
            }
            mCurrentPosition = GetPlaceInRace(type, level, EquestrianCenter.kNumberOfCompetitors, mCurrentScore, out currentMoneyReward);
            Responder.Instance.HudModel.UpdateCompetitionStanding(this);
            FinishType poorFinish = FinishType.PoorFinish;
            if ((mCurrentPosition >= EquestrianCenter.kPositionsForFinishTypes[0x1]) && (mCurrentPosition < EquestrianCenter.kPositionsForFinishTypes[0x0]))
            {
                poorFinish = FinishType.StandardFinish;
            }
            else if (mCurrentPosition == EquestrianCenter.kPositionsForFinishTypes[0x2])
            {
                poorFinish = FinishType.Victory;
            }
            RidingSkill skill = Actor.SimDescription.SkillManager.GetSkill<RidingSkill>(SkillNames.Riding);
            skill.AddPoints(EquestrianCenter.kSkillPointsAdded[0x0]);
            bool flag = type == CompetitionType.CrossCountry;
            if ((flag || (type == CompetitionType.Racing)) && !Horse.BuffManager.HasElement(BuffNames.PetSkillFatigue))
            {
                Horse.SimDescription.SkillManager.GetSkill<Racing>(SkillNames.Racing).AddPoints(EquestrianCenter.kSkillPointsAdded[0x1]);
            }
            if ((flag || (type == CompetitionType.Jumping)) && !Horse.BuffManager.HasElement(BuffNames.PetSkillFatigue))
            {
                Horse.SimDescription.SkillManager.GetSkill<Jumping>(SkillNames.Jumping).AddPoints(EquestrianCenter.kSkillPointsAdded[0x2]);
            }
            if (currentMoneyReward != 0x0)
            {
                if (skill.IsEquestrianChampion())
                {
                    currentMoneyReward = (int)(currentMoneyReward * RidingSkill.EquestrianChampionPrizeMultiplier);
                }
                Actor.ModifyFunds(currentMoneyReward);
                skill.UpdateXpForEarningMoney(currentMoneyReward);
                EventTracker.SendEvent(new JockeyEvent(EventTypeId.kJockeyEvent, Actor, currentMoneyReward));
            }

            string str = Localization.LocalizeString(false, "UI/Caption/HudCompetitionPanel/Place:Position" + mCurrentPosition, new object[0x0]);
            switch (poorFinish)
            {
                case FinishType.PoorFinish:
                    if (Actor.IsActiveSim)
                    {
                        Audio.StartSound("sting_eques_poor_finish");
                    }

                    if ((StoryProgression.Main.Skills.MatchesAlertLevel(Actor)) || (StoryProgression.Main.Skills.MatchesAlertLevel(Horse)))
                    {
                        Sim.ActiveActor.ShowTNSIfSelectable(TNSNames.EquestrianCenterPoorFinish, null, Actor, new object[] { Horse, Actor, str, competitionName });
                    }
                    return;

                case FinishType.StandardFinish:
                    if (Actor.IsActiveSim)
                    {
                        Audio.StartSound("sting_eques_standard_finish");
                    }

                    if ((StoryProgression.Main.Skills.MatchesAlertLevel(Actor)) || (StoryProgression.Main.Skills.MatchesAlertLevel(Horse)))
                    {
                        Sim.ActiveActor.ShowTNSIfSelectable(TNSNames.EquestrianCenterStandardFinish, null, Actor, new object[] { Horse, Actor, str, competitionName });
                    }
                    return;

                case FinishType.Victory:
                    RidingSkill.WonCompetition(Actor.SimDescription, Horse.SimDescription, type, level);
                    if (Actor.IsActiveSim)
                    {
                        Audio.StartSound("sting_eques_victory_finish");
                    }
                    if (!Target.mPlayerHasParticipatedAndWon)
                    {
                        Target.mPlayerHasParticipatedAndWon = true;
                        EventTracker.SendEvent(new EquestrianCompetitionEvent(EventTypeId.kEquestrianCompetition, Actor.FirstName, Actor.IsFemale, Horse.FirstName, Horse.IsFemale, competitionName));
                    }

                    if ((StoryProgression.Main.Skills.MatchesAlertLevel(Actor)) || (StoryProgression.Main.Skills.MatchesAlertLevel(Horse)))
                    {
                        Sim.ActiveActor.ShowTNSIfSelectable(TNSNames.EquestrianCenterVictory, null, Actor, new object[] { Horse, Actor, competitionName });
                    }
                    AddCompetitionTrophy(type, level);
                    return;
            }
        }

        public override bool InRabbitHole()
        {
            try
            {
                ResourceKey key;
                Definition interactionDefinition = InteractionDefinition as Definition;
                if (Horse == null)
                {
                    return false;
                }

                BeginCommodityUpdates();
                if (ThumbnailManager.GeneratePromPicture(Horse.ObjectId.Value, Actor.ObjectId.Value, ThumbnailSizeMask.ExtraLarge, out key))
                {
                    mEquestrianRaceThumbnail = key;
                }

                mCompetitionType = interactionDefinition.CompetitionType;
                mCompetitionLevel = interactionDefinition.CompetitionLevel;
                mCompetitionName = Target.GetCompetitionName(mCompetitionType, mCompetitionLevel);
                SetCompetitorsNames();
                ShowCompetitionTNS(mCompetitionType, mCompetitionLevel, mCompetitionName);
                Target.mSimInEquestrianCompetition = true;

                // Custom
                if (SimTypes.IsSelectable(Actor))
                {
                    mbShowCompetitionUI = true;
                    Responder.Instance.HudModel.ShowCompetitionPanel();
                }
                
                if (!DoTimedLoop(EquestrianCenter.kTimeBeforeCompetitionStartsAfterEntering))
                {
                    Target.mSimInEquestrianCompetition = false;
                    mbShowCompetitionUI = false;
                    Actor.ShowTNSIfSelectable(TNSNames.EquestrianCenterLeftBeforeFinish, null, Actor, new object[] { Actor, Horse });
                    Target.mSimInEquestrianCompetition = false;
                    EndCommodityUpdates(false);
                    return false;
                }

                if (Actor.IsActiveSim)
                {
                    Audio.StartSound("rhole_eques_comp_start");
                }
                FreezeRiderMotives(Actor);
                FreezeHorseMotives(Horse);
                Responder.Instance.HudModel.TriggerProgressBarGlow(this);
                EventTracker.SendEvent(new HorseCompetitionEvent(Actor, Horse, EventTypeId.kEnteredHorseCompetition, mCompetitionType, mCompetitionLevel));
                EventTracker.SendEvent(new HorseCompetitionEvent(Horse, Actor, EventTypeId.kEnteredHorseCompetition, mCompetitionType, mCompetitionLevel));
                mMoodScoreBonus = CalculateMoodBonus();
                mStartTime = SimClock.CurrentTime();
                bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), CalculateScoreForRace, mCurrentStateMachine, 0.5f);
                EndCommodityUpdates(succeeded);
                ResumeMotives(Horse);
                ResumeMotives(Actor);

                if (Actor.HasExitReason(ExitReason.Canceled))
                {
                    Actor.ShowTNSIfSelectable(TNSNames.EquestrianCenterLeftBeforeFinish, null, Actor, new object[] { Actor, Horse });
                    Target.mSimInEquestrianCompetition = false;

                    // Custom
                    if (SimTypes.IsSelectable(Actor))
                    {
                        mbShowCompetitionUI = false;
                        Responder.Instance.HudModel.HideCompetitionPanel();
                    }
                    
                    return false;
                }

                Responder.Instance.HudModel.TriggerProgressBarGlow(this);
                ShowCompetitionEndScoreEx(mCompetitionType, mCompetitionLevel, mCompetitionName);
                DoTimedLoop(EquestrianCenter.kTimeAfterCompetitionForUI, ExitReason.Canceled);
                Target.mSimInEquestrianCompetition = false;

                // Custom
                if (SimTypes.IsSelectable(Actor))
                {
                    mbShowCompetitionUI = false;
                    Responder.Instance.HudModel.HideCompetitionPanel();
                }

                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : EquestrianCenter.EnterEquestrianCompetition.Definition
        {
            public Definition()
            { }
            public Definition(CompetitionType type, CompetitionLevel level)
                : base(type, level)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new EnterEquestrianCompetitionEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, EquestrianCenter target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(EquestrianCenter.EnterEquestrianCompetition.Singleton, target));
            }

            public override bool Test(Sim a, EquestrianCenter target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                /*
                if (!target.DoorsOpen)
                {
                    if (this.IsFirstLevel)
                    {
                        if (target.ShowInProgress)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.EnterEquestrianCompetition.LocalizeString(a.IsFemale, "DoorsClosed", new object[0x0]));
                        }
                        else
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.EnterEquestrianCompetition.LocalizeString(a.IsFemale, "DoorsNeedToBeOpen", new object[] { a }));
                        }
                    }
                    return false;
                }
                */

                if (target.IsActorUsingMe(a))
                {
                    return false;
                }

                if (!(a.Posture is RidingPosture) && !(a.Posture is LeadingHorsePosture))
                {
                    if (IsFirstLevel)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.EnterEquestrianCompetition.LocalizeString(a.IsFemale, "RequiresHorse", new object[] { a }));
                    }
                    return false;
                }

                if (IsFirstLevel)
                {
                    return false;
                }

                return EquestrianCenter.EnterEquestrianCompetition.CanEnterCompetition(CompetitionType, CompetitionLevel, a, CannotEnterCompetition, ref greyedOutTooltipCallback);
            }
        }
    }
}

