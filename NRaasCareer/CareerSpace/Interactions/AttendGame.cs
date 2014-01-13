using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class AttendGame : Stadium.AttendGame, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Stadium, Stadium.AttendGame.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Stadium>(Singleton);
        }

        public static Sim FamilyMemberOfSportsPlayer(Sim a, bool mustBeOmniCareer)
        {
            foreach (Sim sim in a.Household.Sims)
            {
                if (!(sim.Occupation is OmniCareer))
                {
                    if (mustBeOmniCareer) continue;
                }

                ProSports job = OmniCareer.Career<ProSports>(sim.Occupation);

                if ((job != null) && (job.HasWinLossRecordMetric()))
                {
                    return sim;
                }
            }
            return null;
        }

        public override string GetInteractionName()
        {
            if (SimClock.HoursPassedOfDay < ProSports.GameKickOffTime)
            {
                return LocalizeString(base.Actor.SimDescription, "WatchWarmUp", new object[0x0]);
            }
            return LocalizeString(base.Actor.SimDescription, "WatchGameInteractionName", new object[] { base.Target.TeamPoints, base.Target.OpponentPoints });
        }

        public override bool InRabbitHole()
        {
            try
            {
                Journalism job = OmniCareer.Career<Journalism>(Actor.Occupation);

                bool flag = (job != null) && job.CanReviewRabbitHole(Target);
                if (!flag)
                {
                    Sim sim = FamilyMemberOfSportsPlayer(Actor, false);
                    if (sim != null)
                    {
                        if (base.Actor == sim)
                        {
                            SimpleMessageDialog.Show(LocalizeString(Actor.SimDescription, "FreeGameTitle", new object[0x0]), LocalizeString(sim.SimDescription, "FreeGameForSportsPlayer", new object[] { sim }));
                        }
                        else
                        {
                            SimpleMessageDialog.Show(LocalizeString(Actor.SimDescription, "FreeGameTitle", new object[0x0]), LocalizeString(sim.SimDescription, "FreeGame", new object[] { base.Actor, sim }));
                        }
                    }
                    else
                    {
                        if (Actor.FamilyFunds < Stadium.AttendGame.kCostToAttendGame)
                        {
                            return false;
                        }
                        Actor.ModifyFunds(-(int)Stadium.AttendGame.kCostToAttendGame);
                    }
                }
                ConfigureInteraction();
                StartStages();
                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, Stadium>.InsideLoopFunction(WatchGameLoop), null);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                if (succeeded)
                {
                    EventTracker.SendEvent(new AttendedShowEvent(ShowVenue.ShowTypes.kStadiumGame, Actor));
                    EventTracker.SendEvent(EventTypeId.kWatchedStadiumGame, Actor, Target);
                    ChildUtils.AddBuffToParentAndChild(Actor, BuffNames.SawGreatGame, (Origin)(0x8bfc0021188986e0L));
                    if (flag)
                    {
                        job.RabbitHolesReviewed.Add(new Journalism.ReviewedRabbitHole(Target, ShowVenue.ShowTypes.kStadiumGame));
                    }
                }
                return succeeded;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, "AttendGame:" + name, "Gameplay/Objects/RabbitHoles/Stadium/AttendGame:" + name, parameters);
        }

        // Nested Types
        public new class Definition : MetaInteractionDefinition<Sim, Stadium, AttendGame>
        {
            // Methods
            public override string GetInteractionName(Sim a, Stadium target, InteractionObjectPair interaction)
            {
                Journalism job = OmniCareer.Career<Journalism>(a.Occupation);

                if ((job != null) && job.CanReviewRabbitHole(target))
                {
                    return LocalizeString(a.SimDescription, "ReviewGame", new object[0x0]);
                }
                if (FamilyMemberOfSportsPlayer(a, false) == null)
                {
                    return LocalizeString(a.SimDescription, "InteractionName", new object[] { Stadium.AttendGame.kCostToAttendGame });
                }
                return LocalizeString(a.SimDescription, "FreeInteractionName", new object[0x0]);
            }

            public override bool Test(Sim a, Stadium target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!(a.Occupation is OmniCareer))
                {
                    if (FamilyMemberOfSportsPlayer(a, true) == null)
                    {
                        return false;
                    }
                }

                GreyedOutTooltipCallback callback = null;
                if ((!isAutonomous || !a.IsSelectable) || (AutonomyRestrictions.GetLevel() >= AutonomyLevel.Max))
                {
                    ProSports job = OmniCareer.Career<ProSports>(a.Occupation);
                    if (((job != null) && job.HasWinLossRecordMetric()) && !job.IsDayOff)
                    {
                        return false;
                    }
                    bool hasMoney = FamilyMemberOfSportsPlayer(a, false) != null;
                    if (!hasMoney)
                    {
                        hasMoney = a.FamilyFunds >= Stadium.AttendGame.kCostToAttendGame;
                    }
                    if (target.mGameForced)
                    {
                        return true;
                    }

                    Journalism journalism = OmniCareer.Career<Journalism>(a.Occupation);

                    if ((ProSports.IsTodayGameDay() && SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, ProSports.GameStartTime - Stadium.AttendGame.kDoorsOpenTime, ProSports.GameStartTime + Stadium.AttendGame.kDoorsCloseTime)) && (hasMoney && ((journalism == null) || !journalism.BadReviewWrittenOnRabbitHole(target))))
                    {
                        return true;
                    }
                    if (callback == null)
                    {
                        callback = delegate
                        {
                            if ((journalism != null) && journalism.BadReviewWrittenOnRabbitHole(target))
                            {
                                return LocalizeString(a.SimDescription, "NotWelcomeFromBadReview", new object[0x0]);
                            }
                            if (!ProSports.IsTodayGameDay() || !SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, ProSports.GameStartTime, ProSports.GameEndTime))
                            {
                                int num = ProSports.DaysUntilNextGame();
                                if (num == 0x0)
                                {
                                    return LocalizeString(a.SimDescription, "DoorsOpenTodayTooltip", new object[] { SimClockUtils.GetText(ProSports.GameStartTime) });
                                }
                                return LocalizeString(a.SimDescription, "DoorsOpenLaterTooltip", new object[] { num, SimClockUtils.GetText(ProSports.GameStartTime) });
                            }
                            if (!hasMoney)
                            {
                                return LocalizeString(a.SimDescription, "NeedMoneyTooltip", new object[0x0]);
                            }
                            return LocalizeString(a.SimDescription, "DoorsClosedTooltip", new object[0x0]);
                        };
                    }
                    greyedOutTooltipCallback = callback;
                }
                return false;
            }
        }
    }
}
