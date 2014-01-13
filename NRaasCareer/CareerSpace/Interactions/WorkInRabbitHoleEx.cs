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
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class WorkInRabbitHoleEx : WorkInRabbitHole, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, WorkInRabbitHole.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<RabbitHole, WorkInRabbitHole.Definition>(Singleton);
        }

        public override bool BeforeEnteringRabbitHole()
        {
            try
            {
                // Custom
                if (!IsAllowedToWork(mCareer))
                {
                    return mCareer.WaitForWork(Actor, Target);
                }
                return true;
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

        public override bool InRabbitHole()
        {
            try
            {
                LotManager.SetAutoGameSpeed();
                bool succeeded = false;
                BeginCommodityUpdates();

                // Custom
                if (IsAllowedToWork(mCareer) || mCareer.ShouldBeAtWork())
                {
                    DateAndTime previousDateAndTime = SimClock.CurrentTime();
                    float num2 = SimClock.HoursUntil(mCareer.CurLevel.FinishTime()) + mCareer.OvertimeHours;
                    while (!Actor.WaitForExitReason(1f, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)) && !mCareer.IsSpecialWorkTime)
                    {
                        if (mCareer.IsRegularWorkTime())
                        {
                            break;
                        }
                    }

                    if (Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                    {
                        EndCommodityUpdates(false);
                        return false;
                    }

                    mCareer.StartWorking();
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new InteractionInstance.InsideLoopFunction(LoopDelegate), null);
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
                EndCommodityUpdates(succeeded);

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

        protected static bool IsAllowedToWork(Career ths)
        {
            using (HomeworldReversion reversion = new HomeworldReversion(ths.OwnerDescription))
            {
                return ths.IsAllowedToWork();
            }
        }

        public class HomeworldReversion : IDisposable
        {
            WorldName mOriginal;

            SimDescription mSim;

            public HomeworldReversion(SimDescription sim)
            {
                mSim = sim;

                mOriginal = mSim.HomeWorld;

                if (!GameUtils.IsOnVacation())
                {
                    mSim.mHomeWorld = GameUtils.GetCurrentWorld();
                }
            }

            public void Dispose()
            {
                mSim.mHomeWorld = mOriginal;
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

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    CareerLocation location;
                    Career career = a.OccupationAsCareer;
                    if (career == null)
                    {
                        return false;
                    }
                    else if (!a.IsSelectable && Party.IsGuestAtAParty(a))
                    {
                        return false;
                    }
                    else if (!target.CareerLocations.TryGetValue((ulong)career.Guid, out location) || (career.CareerLoc != location))
                    {
                        return false;
                    }

                    bool flag = IsAllowedToWork(career) || career.ShouldBeAtWork();
                    if (career.SpecialWorkDay)
                    {
                        return false;
                    }

                    if (!flag || (career.DayLength == 0f))
                    {
                        greyedOutTooltipCallback = delegate
                        {
                            if (career.DayLength == 0f)
                            {
                                return WorkInRabbitHole.LocalizeString("ZeroDayLengthError", new object[0x0]);
                            }
                            return Localization.LocalizeString("Gameplay/Careers/WorkInRabbitHole:Closed", new object[] { career.GetHoursUntilWorkStringRaw() });
                        };
                        return false;
                    }

                    if (((isAutonomous && a.IsSelectable) && (career.CurLevel.HasCarpool && career.CarpoolEnabled)) && (career.HoursUntilWork >= Career.kHoursBeforeStartCarpoolArrives))
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}

