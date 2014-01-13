using NRaas.CommonSpace.Helpers;
using NRaas.RegisterSpace.Helpers;
using NRaas.RegisterSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Interactions
{
    public class ShowStageTendEx : ShowStage.Tend, Common.IPreLoad
    {
        public void OnPreLoad()
        {
            Tunings.Inject<ShowStage, ShowStage.Tend.Definition, Definition>(false);

            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                Proprietor assignedRole = Actor.SimDescription.AssignedRole as Proprietor;
                if (assignedRole == null)
                {
                    return false;
                }

                if (Actor.LotCurrent != Target.LotCurrent)
                {
                    Actor.RouteToLot(Target.LotCurrent.LotId);
                }

                if (!Target.SimFestRouteOffStageToEmceeIdleSpot(Actor))
                {
                    Actor.RouteToObjectRadius(Target, 5f);
                }

                if (Target.OwnerProprietor == null)
                {
                    Target.OwnerProprietor = Actor;
                }

                if (!Target.InSimFest && SimFestManager.IsSimFestScheduledTodayAtLot(Target.LotCurrent.LotId))
                {
                    Gig scheduledGigForToday = Target.LotCurrent.GetScheduledGigForToday();
                    bool flag = false;
                    foreach (Sim sim in Household.ActiveHousehold.Sims)
                    {
                        if (sim.OccupationAsPerformanceCareer != null)
                        {
                            SteadyGig gig2 = sim.OccupationAsPerformanceCareer.TryToGetSteadyGigOwnedBySim(Actor);
                            if ((gig2 != null) && (gig2.Day == SimClock.CurrentDayOfWeek))
                            {
                                flag = true;
                            }
                        }
                    }

                    if (((scheduledGigForToday == null) || scheduledGigForToday.IsNPCGig) && !flag)
                    {
                        float startTime;
                        float endTime;
                        if (scheduledGigForToday != null)
                        {
                            Target.LotCurrent.UnscheduleGig(scheduledGigForToday);
                            if (scheduledGigForToday == scheduledGigForToday.Occupation.CurrentJob)
                            {
                                scheduledGigForToday.LeaveJob(Career.LeaveJobReason.kJobBecameInvalid);
                            }
                            scheduledGigForToday.Occupation.RemoveJob(scheduledGigForToday, true);
                        }

                        Target.GetRoleTimes(out startTime, out endTime);
                        if (assignedRole.IsThereEnoughTimeForASimFest(endTime))
                        {
                            assignedRole.InitSimFest(endTime);
                            Target.InSimFest = true;
                            Target.AddRoleGivingInteraction(Actor);
                            Target.InSimFest = true;

                            // Custom
                            RegisterSpace.Helpers.ShowStageEx.SetupSimFest(Target);
                        }
                    }
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

        public new class Definition : ShowStage.Tend.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ShowStageTendEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
