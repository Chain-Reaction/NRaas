using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;

namespace NRaas.CareerSpace.Interactions
{
    public class PlayGame : ProSports.PlayGame, Common.IPreLoad, Common.IAddInteraction
    {
        // Fields
        public static new readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Stadium, ProSports.PlayGame.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Stadium>(Singleton);
        }

        // Methods
        public override bool BeforeEnteringRabbitHole()
        {
            ProSports job = OmniCareer.Career<ProSports>(Actor.Occupation);
            if (job == null) return false;

            if (!job.IsAllowedToWork() && job.IsAllowedToWorkThisTime(ProSports.kGameStartTime, ProSports.kGameLength))
            {
                return job.WaitForWork(Actor, Target);
            }
            return true;
        }

        public override string GetInteractionName()
        {
            if (SimClock.HoursPassedOfDay <= (ProSports.kGameStartTime + (ProSports.kGameKickoffMinutes / 60f)))
            {
                return LocalizeString(Actor.SimDescription, "WarmUp", new object[0]);
            }
            IStadium target = Target as IStadium;
            return LocalizeString(Actor.SimDescription, "ScoreInteractionName", new object[] { target.GetTeamPoints(), target.GetOpponentPoints() });
        }

        public override bool InRabbitHole()
        {
            try
            {
                bool succeeded = false;
                BeginCommodityUpdates();

                try
                {
                    ProSports career = OmniCareer.Career<ProSports>(Actor.OccupationAsCareer);

                    if (career.IsAllowedToWork())
                    {
                        career.StartWorking();
                        career.StartGame();
                        succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, RabbitHole>.InsideLoopFunction(GameLoop), null);
                        career.FinishWorking();
                        if (!succeeded)
                        {
                            career.mActorLeftGame = true;
                        }
                        if (!succeeded)
                        {
                            EventTracker.SendEvent(EventTypeId.kWorkCanceled, Actor);
                        }
                    }
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                return succeeded;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        private static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, "PlayGame:" + name, "Gameplay/Careers/ProfessionalSports/PlayGame:" + name, parameters);
        }

        public override bool RouteNearEntranceAndIntoBuilding(bool canUseCar, Route.RouteMetaType routeMetaType)
        {
            Definition interactionDefinition = InteractionDefinition as Definition;
            if ((interactionDefinition != null) && (interactionDefinition.PlayerChosenVehicle != null))
            {
                Actor.SetReservedVehicle(interactionDefinition.PlayerChosenVehicle);
            }
            return base.RouteNearEntranceAndIntoBuilding(canUseCar, routeMetaType);
        }

        // Nested Types
        protected new class Definition : MetaInteractionDefinition<Sim, RabbitHole, PlayGame>
        {
            // Fields
            public Vehicle PlayerChosenVehicle;

            // Methods
            public Definition()
            {
            }

            public Definition(Vehicle playerChosenVehicle)
            {
                PlayerChosenVehicle = playerChosenVehicle;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return PlayGame.LocalizeString(actor.SimDescription, "InteractionName", new object[0]);
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                GreyedOutTooltipCallback callback = null;

                if (!(a.Occupation is OmniCareer)) return false;

                ProSports job = OmniCareer.Career<ProSports>(a.Occupation);

                RabbitHole hole = target;
                if (job != null)
                {
                    CareerLocation location;
                    if (!hole.CareerLocations.TryGetValue((ulong)job.Guid, out location) || (job.CareerLoc != location))
                    {
                        return false;
                    }
                    bool flag = job.IsAllowedToWorkThisTime(ProSports.kGameStartTime, ProSports.kGameLength);
                    if (!job.SpecialWorkDay || !job.HasWinLossRecordMetric())
                    {
                        return false;
                    }
                    if (flag && !job.IsDayOff)
                    {
                        return true;
                    }
                    if (callback == null)
                    {
                        callback = delegate
                        {
                            int num = ProSports.DaysUntilNextGame();
                            if (num == 0)
                            {
                                return PlayGame.LocalizeString(a.SimDescription, "GameGreyedTooltip", new object[] { SimClockUtils.GetText(ProSports.kGameStartTime) });
                            }
                            return PlayGame.LocalizeString(a.SimDescription, "GameGreyedTooltipFuture", new object[] { SimClockUtils.GetText(ProSports.kGameStartTime), num });
                        };
                    }
                    greyedOutTooltipCallback = callback;
                }
                return false;
            }
        }
    }
}
