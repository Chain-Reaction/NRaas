using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class GotoWork : CareerOption
    {
        public override string GetTitlePrefix()
        {
            return "GotoWork";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            Career career = me.Occupation as Career;
            if (career == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return me.CreatedSim.InteractionQueue.Add(WorkInRabbitHoleEx.SingletonEx.CreateInstance(me.Occupation.CareerLoc.Owner, me.CreatedSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
        }

        public class WorkInRabbitHoleEx : WorkInRabbitHole
        {
            public static readonly InteractionDefinition SingletonEx = new Definition();

            float mFinishTime = 0;

            public void OnPreLoad()
            {
                Tunings.Inject<RabbitHole, WorkInRabbitHole.Definition, Definition>(false);
            }

            public override bool BeforeEnteringRabbitHole()
            {
                return true;
            }

            public override void ConfigureInteraction()
            {
                try
                {
                    mCareer = Actor.OccupationAsCareer;

                    mFinishTime = (SimClock.Hours24 + mCareer.CurLevel.DayLength) % 24;

                    InteractionIconKey = new ThumbnailKey(ResourceKey.CreatePNGKey(mCareer.DreamsAndPromisesIcon, 0x0), ThumbnailSize.Medium);
                    mCareer.SetTones(this);

                    StringDelegate delegate2;
                    if (((mCareer.LastTone != null) && mCareer.LastTone.ShouldAddTone(mCareer)) && mCareer.LastTone.Test(this, out delegate2))
                    {
                        CurrentTone = mCareer.LastTone;
                    }
                    if (mCareer.IsSpecialWorkTime)
                    {
                        TimeOfDayStage stage = new TimeOfDayStage(LocalizeString("WorkStage", new object[0x0]), mCareer.HourSpecialWorkTimeEnds, 24f);
                        Stages = new List<Stage>(new Stage[] { stage });
                    }
                    else
                    {
                        float maxDurationInHours = (mCareer.CurLevel.DayLength + Career.kNumHoursEarlyCanShowUpForWork) + mCareer.OvertimeHours;
                        WorkInRabbitHoleStage stage2 = new WorkInRabbitHoleStage(LocalizeString("WorkEndShiftStage", new object[0x0]), mFinishTime, maxDurationInHours, mCareer);
                        Stages = new List<Stage>(new Stage[] { stage2 });
                        ActiveStage = stage2;
                        if (Actor.TraitManager.HasElement(TraitNames.Workaholic))
                        {
                            foreach (CareerTone tone in mCareer.Tones)
                            {
                                if (tone is WorkHardTone)
                                {
                                    CurrentTone = tone;
                                    break;
                                }
                            }
                        }
                        else if (Actor.TraitManager.HasElement(TraitNames.Schmoozer))
                        {
                            foreach (CareerTone tone2 in mCareer.Tones)
                            {
                                if (tone2 is SuckUpToBossTone)
                                {
                                    CurrentTone = tone2;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(Actor, Target, e);
                }
            }

            public override bool InRabbitHole()
            {
                try
                {
                    LotManager.SetAutoGameSpeed();

                    bool succeeded = false;
                    BeginCommodityUpdates();

                    try
                    {
                        DateAndTime previousDateAndTime = SimClock.CurrentTime();
                        float num2 = SimClock.HoursUntil(mFinishTime) + mCareer.OvertimeHours;

                        if (Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                        {
                            EndCommodityUpdates(false);
                            return false;
                        }

                        mCareer.StartWorking();
                        succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, RabbitHole>.InsideLoopFunction(LoopDelegate), null);
                        mCareer.FinishWorking();
                        if (!succeeded)
                        {
                            EventTracker.SendEvent(EventTypeId.kWorkCanceled, Actor);
                        }
                        float num3 = SimClock.ElapsedTime(TimeUnit.Hours, previousDateAndTime);
                        if ((num3 > num2) || (Math.Abs((float)(num3 - num2)) <= kStayLateThreshold))
                        {
                            EventTracker.SendEvent(EventTypeId.kCareerOpportunity_StayedLate, Actor);
                        }
                    }
                    finally
                    {
                        EndCommodityUpdates(succeeded);
                    }

                    ActiveTopic.AddToSim(Actor, "After Work");
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

            protected new void LoopDelegate(StateMachineClient smc, Interaction<Sim, RabbitHole>.LoopData loopData)
            {
                try
                {
                    mCareer.WhileWorking();
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(Actor, Target, e);
                }
            }

            public new class Definition : WorkInRabbitHole.Definition
            {
                public Definition()
                { }

                public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
                {
                    InteractionInstance na = new WorkInRabbitHoleEx();
                    na.Init(ref parameters);
                    return na;
                }

                public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
    }
}
