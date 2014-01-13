using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.PetObjects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Helpers
{
    public class AfterschoolActivityEx
    {
        /*
         * 
         * Ballet
         *  M,W,F
         *  Ballet
         * Scouts
         *  M,T,H
         *  Scouting
         *  Fishing
         *  
         * MusicClub
         *  M,W
         *  Guitar
         *  BassGuitar
         *  Piano
         *  Drums
         * ArtClub
         *  M,H
         *  Painting
         *  Sculpting
         * NewspaperClub
         *  W,F
         *  Writing
         *  Photography
         * StudyClub
         *  T,H
         *  Homework
         * Debate
         *  M,W
         *  Logic
         * ShopClub
         *  T,W
         *  Handiness
         * SportsClub
         *  H,F
         *  Athletic
         * 
         * Monday
         *  Music
         *  Art
         *  Debate
         * Tuesday
         *  Study
         *  Shop
         * Wednesday
         *  Music
         *  Art
         *  Newspaper
         *  Debate
         *  Shop
         * Thursday
         *  Art
         *  Study
         *  Sports
         * Friday
         *  Newspaper
         *  Sports
         * 
         */

        [Tunable, TunableComment("Random chance of a sim collecting a bug during Bug Club")]
        protected static int kBugClubCollectChance = 15;

        [Tunable, TunableComment("Random chance of a sim collecting a rock during Geology Club")]
        protected static int kGeologyClubCollectChance = 15;

        [Tunable, TunableComment("Random chance of a sim collecting a rock during Explorer Club")]
        protected static int kExplorerClubCollectChance = 10;

        public static DaysOfTheWeek GetDaysForActivityType(AfterschoolActivityType activityType)
        {
            return AfterschoolActivityBooter.GetActivity(activityType).mActivity.DaysForActivity;
        }

        public static bool DoesActivityConflictWithJob(SimDescription actor, AfterschoolActivityType activityToCheck)
        {
            School school = actor.CareerManager.School;
            if (school == null)
            {
                return true;
            }

            Career occupationAsCareer = actor.CareerManager.Occupation as Career;
            if ((occupationAsCareer != null) && ((GetDaysForActivityType(activityToCheck) & occupationAsCareer.DaysOfWeekToWork) != DaysOfTheWeek.None))
            {
                float num = (school.CurLevel.FinishTime() + AfterschoolActivity.kAfterschoolActivityLength) + AfterschoolActivity.kBufferTimeBetweenActivityAndJobStarting;
                if (num > occupationAsCareer.StartTime)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsTeenActivity(AfterschoolActivityType type)
        {
            return AfterschoolActivityBooter.GetActivity(type).IsTeen;
        }

        public static bool IsChildActivity(AfterschoolActivityType type)
        {
            return AfterschoolActivityBooter.GetActivity(type).IsChild;
        }

        public static bool AlreadyHasChildActivity(SimDescription actor)
        {
            if (!actor.Child) return false;

            School school = actor.CareerManager.School;
            if (school != null)
            {
                List<AfterschoolActivity> afterschoolActivities = school.AfterschoolActivities;
                if (afterschoolActivities != null)
                {
                    foreach (AfterschoolActivity activity2 in afterschoolActivities)
                    {
                        AfterschoolActivityData data = AfterschoolActivityBooter.GetActivity(activity2.CurrentActivityType);
                        if (data == null) continue;

                        if (data.IsChild) return true;
                    }
                }
            }

            return false;
        }

        public static bool IsValidSimForSimPicker(SimDescription actor, AfterschoolActivityType activityToCheck)
        {
            return AfterschoolActivityBooter.GetActivity(activityToCheck).IsValidFor(actor);
        }

        public static bool HasAfterschoolActivityOfType(SimDescription actor, AfterschoolActivityType activity)
        {
            School school = actor.CareerManager.School;
            if (school != null)
            {
                List<AfterschoolActivity> afterschoolActivities = school.AfterschoolActivities;
                if (afterschoolActivities != null)
                {
                    foreach (AfterschoolActivity activity2 in afterschoolActivities)
                    {
                        if (activity2.CurrentActivityType == activity)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool HasAfterschoolActivityOnDays(SimDescription actor, DaysOfTheWeek daysToCheck)
        {
            School school = actor.CareerManager.School;
            if (school != null)
            {
                List<AfterschoolActivity> afterschoolActivities = school.AfterschoolActivities;
                if (afterschoolActivities != null)
                {
                    foreach (AfterschoolActivity activity in afterschoolActivities)
                    {
                        if ((activity.DaysForActivity & daysToCheck) != DaysOfTheWeek.None)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool HasCompletedRecitalForType(School school, AfterschoolActivityType activityType)
        {
            return AfterschoolActivity.HasCompletedRecitalForType(school, activityType);
        }

        public static bool AddNewActivity(SimDescription actor, AfterschoolActivityType activityType)
        {
            School school = actor.CareerManager.School;
            if (school.AfterschoolActivities == null)
            {
                school.AfterschoolActivities = new List<AfterschoolActivity>();
            }
            else if (AlreadyHasChildActivity(actor) || HasAfterschoolActivityOfType(actor, activityType))
            {
                return false;
            }

            // Custom
            school.AfterschoolActivities.Add(AfterschoolActivityBooter.GetActivity(activityType).mActivity);

            string name = activityType.ToString();

            if (actor.CreatedSim != null)
            {
                actor.CreatedSim.ShowTNSIfSelectable(TNSNames.SignUpForAfterschoolActivityTNS, actor, null, null, actor.IsFemale, actor.IsFemale, new object[] { actor, AfterschoolActivity.LocalizeString(actor.IsFemale, name, new object[0x0]), AfterschoolActivity.LocalizeString(actor.IsFemale, name + "Description", new object[0x0]) });
                actor.CareerManager.UpdateCareerUI();
            }
            return true;
        }

        public static bool MeetsCommonAfterschoolActivityRequirements(SimDescription actor, AfterschoolActivityType activityToCheck, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            bool teen = actor.Teen;
            if (!actor.Child && !teen)
            {
                return false;
            }

            School school = actor.CareerManager.School;
            if (school == null)
            {
                return false;
            }
            else if (actor.IsEnrolledInBoardingSchool())
            {
                return false;
            }
            /*
            else if (!AfterschoolActivity.WorldHasSchoolRabbitHole())
            {
                return false;
            }
            */
            else if (HasAfterschoolActivityOfType(actor, activityToCheck))
            {
                return false;
            }
            else if (HasAfterschoolActivityOnDays(actor, GetDaysForActivityType(activityToCheck)))
            {
                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(AfterschoolActivity.LocalizeString(actor.IsFemale, "DayConflict", new object[0x0]));
                return false;
            }
            else if (DoesActivityConflictWithJob(actor, activityToCheck))
            {
                return false;
            }
            else if (teen && school.HasCompletedTeenAfterschoolActivityRecital)
            {
                return false;
            }
            else if (HasCompletedRecitalForType(school, activityToCheck))
            {
                return false;
            }
            else if (IsChildActivity(activityToCheck) && AlreadyHasChildActivity(actor))
            {
                return false;
            }

            AgingManager singleton = AgingManager.Singleton;
            float num = singleton.AgingYearsToSimDays(singleton.GetCurrentAgingStageLength(actor));
            float num2 = singleton.AgingYearsToSimDays(actor.AgingYearsSinceLastAgeTransition);
            float num3 = num - num2;
            if (num3 <= AfterschoolActivity.kDaysBeforeAgingTrigger)
            {
                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(AfterschoolActivity.LocalizeString(actor.IsFemale, "AboutToAgeUp", new object[] { actor }));
                return false;
            }

            return true;
        }

        public static bool PerformAfterschoolPreLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            AfterschoolActivityData data = AfterschoolActivityBooter.GetActivity(activity.CurrentActivityType);

            data.PerformPreLoop(interaction, activity);
            return true;
        }

        public static InteractionInstance.InsideLoopFunction PerformAfterschoolLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            AfterschoolActivityData data = AfterschoolActivityBooter.GetActivity(activity.CurrentActivityType);

            return data.LoopDelegate(interaction, activity);
        }

        public static bool PerformAfterschoolPostLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            AfterschoolActivityData data = AfterschoolActivityBooter.GetActivity(activity.CurrentActivityType);

            data.PerformPostLoop(interaction, activity);
            return true;
        }

        public static void PerformPreLoopStudyClub(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                School school = interaction.Actor.School;

                school.AddHomeworkToStudent(false);
                interaction.mHomework = school.OwnersHomework;
                if (interaction.mHomework != null)
                {
                    school.DidHomeworkInStudyClubToday = true;
                    interaction.mHomework.PercentComplete = 0f;
                    interaction.mHomeworkCompletionRate = interaction.mHomework.GetCompletionRate(interaction.Actor, false, true);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(interaction.Actor, interaction.Target, e);
            }
        }

        public static void PerformPostLoopArtClub(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                activity.CheckForNewPainting(interaction.Actor);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(interaction.Actor, interaction.Target, e);
            }
        }

        public static void PerformPostLoopChessClub(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                Chess skill = interaction.Actor.SkillManager.GetSkill<Chess>(SkillNames.Chess);
                if (skill != null)
                {
                    if (RandomUtil.CoinFlip())
                    {
                        skill.NumberOfWins++;
                    }
                    else
                    {
                        skill.NumberOfLosses++;
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(interaction.Actor, interaction.Target, e);
            }
        }

        public static InteractionInstance.InsideLoopFunction PerformLoopBugClub(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            return new BugLoop(interaction, activity).Perform;
        }

        public static InteractionInstance.InsideLoopFunction PerformLoopGeologyClub(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            return new GeologyLoop(interaction, activity).Perform;
        }

        public static InteractionInstance.InsideLoopFunction PerformLoopExplorerClub(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            return new ExplorerLoop(interaction, activity).Perform;
        }

        public class BugLoop : AfterschoolActivityData.LoopProxy
        {
            public BugLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
                : base(interaction, activity)
            { }

            protected override void PrivatePerform(StateMachineClient smc, InteractionInstance.LoopData loopData)
            {
                if (SimClock.ElapsedTime(TimeUnit.Minutes, mInteraction.mLastLTRUpdateDateAndTime) >= GoToSchoolInRabbitHole.kSimMinutesBetweenLTRUpdates)
                {
                    if (RandomUtil.RandomChance(kBugClubCollectChance))
                    {
                        List<InsectJig> insects = new List<InsectJig>(Sims3.Gameplay.Queries.GetObjects<InsectJig>());

                        if (insects.Count > 0)
                        {
                            InsectJig insect = RandomUtil.GetRandomObjectFromList(insects);

                            Terrarium terrarium = InsectTerrarium.Create(insect, mInteraction.Actor);

                            if (terrarium != null)
                            {
                                string msg = Common.Localize("AfterschoolFoundObject:Notice", mInteraction.Actor.IsFemale, new object[] { mInteraction.Actor, terrarium.GetLocalizedName() });

                                mInteraction.Actor.ShowTNSIfSelectable(msg, StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, terrarium.ObjectId);
                            }
                        }
                    }
                }

                mInteraction.AfterschoolActivityLoopDelegate(smc, loopData);
            }
        }

        public class GeologyLoop : AfterschoolActivityData.LoopProxy
        {
            public GeologyLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
                : base(interaction, activity)
            { }

            protected override void PrivatePerform(StateMachineClient smc, InteractionInstance.LoopData loopData)
            {
                if (SimClock.ElapsedTime(TimeUnit.Minutes, mInteraction.mLastLTRUpdateDateAndTime) >= GoToSchoolInRabbitHole.kSimMinutesBetweenLTRUpdates)
                {
                    if (RandomUtil.RandomChance(kGeologyClubCollectChance))
                    {
                        List<RockGemMetalBase> rocks = new List<RockGemMetalBase>(Sims3.Gameplay.Queries.GetObjects<RockGemMetalBase>());
                        if (rocks.Count > 0)
                        {
                            RockGemMetalBase rock = RandomUtil.GetRandomObjectFromList(rocks);

                            if (Inventories.TryToMove(rock, mInteraction.Actor))
                            {
                                rock.RegisterCollected(mInteraction.Actor, false);
                                rock.RemoveFromWorld();

                                string msg = Common.Localize("AfterschoolFoundObject:Notice", mInteraction.Actor.IsFemale, new object[] { mInteraction.Actor, rock.GetLocalizedName() });

                                mInteraction.Actor.ShowTNSIfSelectable(msg, StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, rock.ObjectId);
                            }
                        }
                    }
                }

                mInteraction.AfterschoolActivityLoopDelegate(smc, loopData);
            }
        }

        public class ExplorerLoop : AfterschoolActivityData.LoopProxy
        {
            public ExplorerLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
                : base(interaction, activity)
            { }

            protected override void PrivatePerform(StateMachineClient smc, InteractionInstance.LoopData loopData)
            {
                if (SimClock.ElapsedTime(TimeUnit.Minutes, mInteraction.mLastLTRUpdateDateAndTime) >= GoToSchoolInRabbitHole.kSimMinutesBetweenLTRUpdates)
                {
                    if (RandomUtil.RandomChance(kExplorerClubCollectChance))
                    {
                        IGameObject huntable = GetHuntable();
                        if (huntable != null)
                        {
                            if (Inventories.TryToMove(huntable, mInteraction.Actor))
                            {
                                RockGemMetalBase rock = huntable as RockGemMetalBase;
                                if (rock != null)
                                {
                                    rock.RegisterCollected(mInteraction.Actor, false);
                                }

                                string msg = Common.Localize("AfterschoolFoundObject:Notice", mInteraction.Actor.IsFemale, new object[] { mInteraction.Actor, huntable.GetLocalizedName() });

                                mInteraction.Actor.ShowTNSIfSelectable(msg, StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, huntable.ObjectId);
                            }
                            else
                            {
                                huntable.Destroy();
                            }
                        }
                    }
                }

                mInteraction.AfterschoolActivityLoopDelegate(smc, loopData);
            }

            private static IGameObject GetHuntable()
            {
                List<WeightedHuntingData> weightedData = new List<WeightedHuntingData>();

                GetFragmentData(ref weightedData);
                foreach (KeyValuePair<RockGemMetal, RockGemMetalData> pair in RockGemMetalBase.sData)
                {
                    RGMHuntableData huntingData = pair.Value.HuntingData;
                    if (huntingData == null) continue;

                    WeightedHuntingData item = new WeightedHuntingData((ulong)pair.Key, 10, huntingData.MinSkillLevel, huntingData.MaxSkillLevel, huntingData.MaxWeight, huntingData.MaxWeight, 0x0);
                    weightedData.Add(item);
                }

                if (weightedData.Count == 0) return null;

                WeightedHuntingData weightedRandomObjectFromList = RandomUtil.GetWeightedRandomObjectFromList(weightedData);
                return MakeHuntableFromData(weightedRandomObjectFromList);
            }

            private static void GetFragmentData(ref List<WeightedHuntingData> weightedData)
            {
                foreach (HunatbleFragmentData data in DogHuntingSkill.sHuntableFragments)
                {
                    if (data == null) continue;

                    WeightedHuntingData item = new WeightedHuntingData(data.MetadorName, 10, data.MinSkillLevel, data.MaxSkillLevel, data.MinWeight, data.MaxWeight);
                    weightedData.Add(item);
                }
            }

            private static IGameObject MakeHuntableFromData(WeightedHuntingData data)
            {
                IGameObject huntable = null;
                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.StringID))
                    {
                        huntable = RockGemMetalBase.Make((RockGemMetal)data.ID, true);
                    }
                    else
                    {
                        huntable = GlobalFunctions.CreateObjectOutOfWorld(data.StringID, ProductVersion.EP5);
                    }
                }

                return huntable;
            }
        }
    }
}
