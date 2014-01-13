using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class GoToSchoolInRabbitHoleHelper
    {
        static Dictionary<RabbitHole, SchoolRabbitholeProxy> sSchoolRabbitholeProxies = new Dictionary<RabbitHole, SchoolRabbitholeProxy>();

        static Common.MethodStore sCareerPerformAfterschoolPreLoop = new Common.MethodStore("NRaasCareer", "NRaas.Careers", "PerformAfterschoolPreLoop", new Type[] { typeof(GoToSchoolInRabbitHole), typeof(AfterschoolActivity) });
        static Common.MethodStore sCareerPerformAfterschoolLoop = new Common.MethodStore("NRaasCareer", "NRaas.Careers", "PerformAfterschoolLoop", new Type[] { typeof(GoToSchoolInRabbitHole), typeof(AfterschoolActivity) });
        static Common.MethodStore sCareerPerformAfterschoolPostLoop = new Common.MethodStore("NRaasCareer", "NRaas.Careers", "PerformAfterschoolPostLoop", new Type[] { typeof(GoToSchoolInRabbitHole), typeof(AfterschoolActivity) });

        public delegate void OnChangeOutfit();
         
        protected static bool ApplyOccupationUniform(SimDescription ths, SimOutfit uniform)
        {
            if (((uniform != null) && uniform.IsValid) && !ths.OccultManager.DisallowClothesChange())
            {
                SimOutfit source = CASParts.GetOutfit(ths, CASParts.sPrimary, false); 
                if (source == null) return false;

                CASParts.Key schoolKey = new CASParts.Key("NRaasSchoolOutfit");

                SimOutfit schoolOutfit = CASParts.GetOutfit(ths, schoolKey, false);
                if (schoolOutfit == null)
                {
                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(ths, schoolKey, source))
                    {
                        new SavedOutfit(uniform).Apply(builder, true);
                    }
                }

                schoolOutfit = CASParts.GetOutfit(ths, schoolKey, false);
                if (schoolOutfit != null)
                {
                    Sim createdSim = ths.CreatedSim;
                    if (createdSim != null)
                    {
                        SwitchOutfits.SwitchNoSpin(createdSim, schoolKey);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool PreRouteNearEntranceAndIntoBuilding(GoToSchoolInRabbitHole ths, bool canUseCar, Route.RouteMetaType routeMetaType, OnChangeOutfit changedOutfitFunc)
        {
            try
            {
                // From GoToSchoolInRabbitHole

                GoToSchoolInRabbitHole.Definition interactionDefinition = ths.InteractionDefinition as GoToSchoolInRabbitHole.Definition;
                if ((interactionDefinition != null) && (interactionDefinition.PlayerChosenVehicle != null))
                {
                    ths.Actor.SetReservedVehicle(interactionDefinition.PlayerChosenVehicle);
                }

                bool success = false;

                School school = ths.Actor.School;
                if (school != null)
                {
                    SimOutfit outfit;
                    if (school.TryGetUniformForCurrentLevel(out outfit))
                    {
                        if (ApplyOccupationUniform(ths.Actor.SimDescription, outfit))
                        {
                            success = true;
                        }
                    }
                }
                 
                if ((!success) && (changedOutfitFunc != null))
                {
                    changedOutfitFunc();
                }

                VisitSituation.OnSimLeavingLot(ths.Actor);

                // From RabbitHoleInteraction<TActor, TRabbitHole>
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ths.Actor, ths.Target, e);
                return false;
            }
        }

        public static bool PreInRabbitholeLoop(GoToSchoolInRabbitHole ths)
        {
            return PreInRabbitholeLoop(ths, ths.Actor.School.IsAllowedToWork());
        }
        public static bool PreInRabbitholeLoop(GoToSchoolInRabbitHole ths, bool allowedToWork)
        {
            try
            {
                LotManager.SetAutoGameSpeed();

                //CancellableByPlayer = false;

                ths.BeginCommodityUpdates();

                if (!allowedToWork)
                {
                    ths.EndCommodityUpdates(false);
                    return false;
                }

                ths.Actor.School.StartWorking();
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ths.Actor, ths.Target, e);
                return false;
            }
        }

        public static ISchoolRabbitHole GetTarget(GoToSchoolInRabbitHole ths)
        {
            ISchoolRabbitHole target = ths.Target as ISchoolRabbitHole;
            if (target == null)
            {
                SchoolRabbitholeProxy proxy;
                if (!sSchoolRabbitholeProxies.TryGetValue(ths.Target, out proxy))
                {
                    proxy = new SchoolRabbitholeProxy();
                    sSchoolRabbitholeProxies.Add(ths.Target, proxy);
                }

                target = proxy;
            }

            return target;
        }

        public static void PostInRabbitHoleLoop(GoToSchoolInRabbitHole ths, ref bool succeeded, ref bool detention, ref bool fieldTrip, ref AfterschoolActivity activity, ref bool hasAfterschoolActivity)
        {
            try
            {
                School school = ths.Actor.School;

                ISchoolRabbitHole target = GetTarget(ths);

                activity = ths.GetAfterschoolActivity(ths.Actor, school, target, ref hasAfterschoolActivity);

                fieldTrip = FieldTripSituation.GetFieldTripSituation(ths.Actor) != null;
                detention = school.CurentDetentionStatus == School.DetentionStatus.HasDetention;

                if (detention && !fieldTrip)
                {
                    school.CurentDetentionStatus = School.DetentionStatus.InDetention;
                    float maxDurationInHours = (school.DayLength + Career.kNumHoursEarlyCanShowUpForWork) + school.SchoolTuning.DetentionLengthInHours;
                    TimeOfDayStage stage = new TimeOfDayStage(Localization.LocalizeString(ths.Actor.IsFemale, "Gameplay/Careers/WorkInRabbitHole:SchoolDetention", new object[0x0]), school.CurLevel.FinishTime() + school.SchoolTuning.DetentionLengthInHours, maxDurationInHours);
                    ths.Stages = new List<Stage>(new Stage[] { stage });
                    ths.ActiveStage = stage;
                    school.SetTones(ths);
                    ths.Actor.RemoveExitReason(ExitReason.StageComplete);
                    ths.Actor.BuffManager.AddElement(BuffNames.Detention, Origin.FromSchool);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ths.Actor, ths.Target, e);
            }
        }

        public static void PostDetentionLoop(GoToSchoolInRabbitHole ths, bool succeeded, bool detention, bool fieldTrip, AfterschoolActivity activity, bool hasAfterschoolActivity, ref InteractionInstance.InsideLoopFunction afterSchoolLoop)
        {
            try
            {
                School school = ths.Actor.School;

                ISchoolRabbitHole target = GetTarget(ths);

                bool flag5 = false;
                bool flag6 = false;
                if (!fieldTrip && !detention)
                {
                    if (hasAfterschoolActivity)
                    {
                        flag5 = true;
                    }
                    else if (!ths.Actor.IsInActiveHousehold && target.HasAfterschoolActivityToday)
                    {
                        flag5 = RandomUtil.CoinFlip();
                        flag6 = true;
                    }
                }

                if (!flag5)
                {
                    school.FinishWorking();

                    ths.EndCommodityUpdates(succeeded);
                    return;
                }

                CommodityChange fun = new CommodityChange(CommodityKind.Fun, 50, false, 25, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);

                ths.BeginCommodityUpdate(fun, 1);

                ths.mParticipatedInAfterschoolActivity = true;
                string str = flag6 ? AfterschoolActivityType.ArtClub.ToString() : activity.CurrentActivityType.ToString();
                float num3 = (school.DayLength + Career.kNumHoursEarlyCanShowUpForWork) + AfterschoolActivity.kAfterschoolActivityLength;

                string name = null;
                if (Localization.HasLocalizationString("Gameplay/Careers/AfterschoolActivity:" + str))
                {
                    name = Common.LocalizeEAString(ths.Actor.IsFemale, "Gameplay/Careers/AfterschoolActivity:" + str);
                }
                else
                {
                    name = Common.LocalizeEAString(ths.Actor.IsFemale, "Gameplay/Abstracts/Careers/AfterschoolActivity:" + str);
                }

                TimeOfDayStage stage2 = new TimeOfDayStage(name, school.CurLevel.FinishTime() + AfterschoolActivity.kAfterschoolActivityLength, num3);
                ths.Stages = new List<Stage>(new Stage[] { stage2 });
                ths.ActiveStage = stage2;
                ths.SetAvailableTones(null);
                ths.Actor.RemoveExitReason(ExitReason.StageComplete);

                if (!flag6)
                {
                    activity.StartSkillGainForActivity(ths.Actor);

                    activity.StartAmbientSounds(ths.Actor, ths.Target);

                    if (!sCareerPerformAfterschoolPreLoop.Invoke<bool>(new object[] { ths, activity }))
                    {
                        if (activity.CurrentActivityType == AfterschoolActivityType.StudyClub)
                        {
                            school.AddHomeworkToStudent(false);
                            ths.mHomework = school.OwnersHomework;
                            if (ths.mHomework != null)
                            {
                                school.DidHomeworkInStudyClubToday = true;
                                ths.mHomework.PercentComplete = 0f;
                                ths.mHomeworkCompletionRate = ths.mHomework.GetCompletionRate(ths.Actor, false, true);
                            }
                        }
                    }

                    ths.mLastLTRUpdateDateAndTime = SimClock.CurrentTime();

                    afterSchoolLoop = sCareerPerformAfterschoolLoop.Invoke<InteractionInstance.InsideLoopFunction>(new object[] { ths, activity });
                    if (afterSchoolLoop == null)
                    {
                        afterSchoolLoop = ths.AfterschoolActivityLoopDelegate;
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ths.Actor, ths.Target, e);
            }
        }
        
        public static void PostAfterSchoolLoop(GoToSchoolInRabbitHole ths, bool succeeded, AfterschoolActivity activity, InteractionInstance.InsideLoopFunction afterSchoolLoop)
        {
            try
            {
                if (afterSchoolLoop != null)
                {
                    ths.Target.ClearAmbientSounds(ths.Target);
                    ths.Target.AddAmbientSound("rhole_school_oneshot");

                    foreach (SkillNames names in activity.ActivitySkillNameList)
                    {
                        ths.Actor.SkillManager.StopSkillGain(names);
                    }

                    if (!sCareerPerformAfterschoolPostLoop.Invoke<bool>(new object[] { ths, activity }))
                    {
                        if (activity.CurrentActivityType == AfterschoolActivityType.ArtClub)
                        {
                            activity.CheckForNewPainting(ths.Actor);
                        }
                    }

                    ISchoolRabbitHole target = GetTarget(ths);

                    target.HasAfterschoolActivityToday = false;
                }

                ths.Actor.School.FinishWorking();
                
                ths.EndCommodityUpdates(succeeded);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ths.Actor, ths.Target, e);
            }
        }

        public class InitTask : Common.IWorldQuit
        {
            public void OnWorldQuit()
            {
                sSchoolRabbitholeProxies.Clear();
            }
        }

        public class SchoolRabbitholeProxy : ISchoolRabbitHole
        {
            bool mHasAfterschoolActivityToday = false;

            public bool HasAfterschoolActivityToday
            {
                get { return mHasAfterschoolActivityToday; }
                set { mHasAfterschoolActivityToday = value; }
            }
        }
    }
}

