using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class OccupationPushScenario : OccupationScenario
    {
        protected bool mReport = false;

        public OccupationPushScenario(SimDescription sim)
            : base (sim)
        {}
        protected OccupationPushScenario(OccupationPushScenario scenario)
            : base (scenario)
        {
            mReport = scenario.mReport;
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledWorkPushScenario.Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected static bool AttendingGraduation(SimDescription sim)
        {
            if (School.sGraduatingSims == null) return false;

            foreach (SimDescription member in HouseholdsEx.Humans(sim.Household))
            {
                if (School.sGraduatingSims.ContainsKey(member)) return true;
            }

            return false;
        }

        protected override bool Allow(SimDescription sim)
        {
            /*
            if (!Situations.Allow(this, sim, false))
            {
                IncStat("User Denied");
                return false;
            }
            else */if (!GetValue<AllowPushWorkOption,bool>(Sim))
            {
                IncStat("Push Denied");
                return false;
            }
            else
            {
                Occupation job = Occupation;

                if (job.IsDayOff)
                {
                    IncStat("Day Off");
                    return false;
                }

                if (sim.CreatedSim != null)
                {
                    if (sim.CreatedSim.Autonomy == null)
                    {
                        IncStat("No Autonomy");
                        return false;
                    }
                    else if (sim.CreatedSim.Autonomy.SituationComponent == null)
                    {
                        IncStat("No SituationComponent");
                        return false;
                    }
                    else if (sim.CreatedSim.Autonomy.SituationComponent.Situations == null)
                    {
                        IncStat("No Situation List");
                        return false;
                    }
                    else if (IsInSituation(sim.CreatedSim))
                    {
                        IncStat("In Situation");
                        return false;
                    }
                    else if (HasInteraction(sim.CreatedSim))
                    {
                        IncStat("At Prom");
                        return false;
                    }
                    else if (AttendingGraduation(sim))
                    {
                        IncStat("Graduation");
                        return false;
                    }
                }

                DateAndTime NowTime = SimClock.CurrentTime();
                if (!job.IsWorkHour(NowTime))
                {
                    IncStat("Not Work Hours");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected static bool IsInteriorDesignerClient(Sim sim)
        {
            if (sim.LotCurrent != null)
            {
                if (!sim.LotCurrent.CanSimTreatAsHome(sim)) return false;
            }

            if (Household.ActiveHousehold == null) return false;

            foreach(Sim active in CommonSpace.Helpers.Households.AllHumans(Household.ActiveHousehold))
            {
                if (active.LotCurrent == null) continue;

                if (!active.LotCurrent.CanSimTreatAsHome(sim)) continue;

                if (InteriorDesigner.SimIsWorkingAtJobOnLot(active, sim.LotHome)) return true;
            }

            return false;
        }

        protected static bool IsInSituation(Sim sim)
        {
            if (Party.IsGuestAtAParty(sim))
            {
                return true;
            }
            else if (PromSituation.IsHeadedToProm(sim))
            {
                return true;
            }
            else if (ManagerSituation.HasBlockingSituation(sim, null))
            {
                return true;
            }
            else if (IsInteriorDesignerClient(sim))
            {
                return true;
            }

            return false;
        }

        protected static bool HasInteraction(Sim sim)
        {
            if (sim.InteractionQueue == null) return false;

            foreach (InteractionInstance instance in sim.InteractionQueue.InteractionList)
            {
                if (instance is PromSituation.GoToProm) return true;

                if (instance is ICountsAsWorking) return true;
            }

            return false;
        }

        protected override bool ShouldReport
        {
            get { return mReport; }
        }

        protected abstract bool PostSlackerWarning();

        protected bool Test(Sim sim, InteractionDefinition interaction)
        {
            List<InteractionDefinition> interactions = new List<InteractionDefinition>();

            if (interaction != null)
            {
                interactions.Add(interaction);
            }

            return Test(sim, interactions);
        }
        protected bool Test(Sim sim, List<InteractionDefinition> interactions)
        {
            if (InteractionsEx.HasInteraction<ICountsAsWorking>(sim))
            {
                IncStat("At Work");
                return false;
            }
            else if (Situations.HasInteraction(sim, interactions, true))
            {
                IncStat("At Work");
                return false;
            }
            else
            {
                if (Situations.DebuggingLevel >= Common.DebugLevel.High)
                {
                    if (sim.InteractionQueue == null)
                    {
                        IncStat("Interaction Queue = null");
                    }
                    else
                    {
                        if (sim.InteractionQueue.Count == 0)
                        {
                            IncStat("Interaction Queue empty");
                        }
                        else
                        {
                            foreach (InteractionInstance instance2 in sim.InteractionQueue.InteractionList)
                            {
                                IncStat("Interaction: " + instance2.ToString());
                            }
                        }
                    }
                }

                VisitSituation.AnnounceTimeToGoToWork(sim);

                if (SimTypes.IsSpecial(Sim))
                {
                    IncStat("Special Pushed to Work");
                }
                else
                {
                    IncStat("Pushed to Work");
                }

                try
                {
                    // Don't queue stomp on their birthday
                    if (sim.SimDescription.YearsSinceLastAgeTransition != 0)
                    {
                        sim.InteractionQueue.CancelAllInteractions();

                        IncStat("Queue Stomped");

                        if ((!SimTypes.IsSelectable(sim.SimDescription)) && (sim.LotCurrent != null) && (sim.LotCurrent.CanSimTreatAsHome(sim)))
                        {
                            Callbox callbox = sim.LotHome.FindCallbox();
                            if (callbox != null)
                            {
                                Vector3 fwd = Vector3.Invalid;
                                Vector3 pos = Vector3.Invalid;

                                World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(callbox.Position);
                                fglParams.BooleanConstraints = FindGoodLocationBooleans.Routable | FindGoodLocationBooleans.PreferEmptyTiles;
                                if (GlobalFunctions.FindGoodLocation(sim, fglParams, out pos, out fwd))
                                {
                                    sim.ResetBindPoseWithRotation();
                                    sim.SetPosition(pos);
                                    sim.SetForward(fwd);
                                    sim.RemoveFromWorld();
                                    sim.AddToWorld();
                                    sim.SetHiddenFlags(HiddenFlags.Nothing);
                                    sim.SetOpacity(1f, 0f);

                                    IncStat("Bounce to Mailbox");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.DebugException(sim, e);

                    IncStat("Cancel Fail");
                }

                sim.InteractionQueue.RemoveGoHomeInteractions(true);

                return true;
            }
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Situations;
            }

            if (parameters == null)
            {
                parameters = new object[] { Sim, SimClock.CurrentTime() };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
