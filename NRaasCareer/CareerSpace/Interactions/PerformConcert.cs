using NRaas.CareerSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;

namespace NRaas.CareerSpace.Interactions
{
    public class PerformConcert : ShowVenue.PerformConcert, Common.IPreLoad, Common.IAddInteraction
    {
        public new static readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<ShowVenue, ShowVenue.PerformConcert.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<ShowVenue>(Singleton);
        }

        public override bool BeforeEnteringRabbitHole()
        {
            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            if (!base.Target.PerformConcertAllowed(ref greyedOutTooltipCallback))
            {
                base.Actor.ShowTNSIfSelectable(OmniCareer.LocalizeString(base.Actor.SimDescription, "AttendClassInRabbitHole:ConflictingEventTNS", "Gameplay/Abstracts/RabbitHole/AttendClassInRabbitHole:ConflictingEventTNS", new object[0x0]), StyledNotification.NotificationStyle.kSimTalking, base.Actor.ObjectId, base.Target.ObjectId);
                return false;
            }
            return base.BeforeEnteringRabbitHole();
        }

        public override bool InRabbitHole()
        {
            BeginCommodityUpdates();

            bool succeeded = false;

            try
            {
                Target.StartPlayerConcert();
                StartStages();
                succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                Target.EndPlayerConcert();
            }
            finally
            {
                EndCommodityUpdates(succeeded);
            }

            if (succeeded)
            {
                EventTracker.SendEvent(EventTypeId.kPerformedConcert, Actor);

                int level = 10;

                Music job = OmniCareer.Career<Music>(Actor.Occupation);
                if (job != null)
                {
                    job.ConcertsPerformed++;

                    OmniCareer omni = Actor.Occupation as OmniCareer;
                    if (omni != null)
                    {
                        if (omni.PaidForConcerts())
                        {
                            level = 0;
                        }
                    }
                    else
                    {
                        level = Music.LevelToGetPaidForConcerts;
                    }
                }

                SkillBasedCareer career = SkillBasedCareerBooter.GetSkillBasedCareer(Actor, SkillNames.Guitar);
                if (career != null)
                {
                    level = NRaas.Careers.Settings.mBuskerLevelToGetPaidForConcerts;
                }

                if (Actor.Occupation.CareerLevel >= level)
                {
                    int concertPayAmount = Target.ConcertPayAmount;

                    Actor.ModifyFunds (concertPayAmount);

                    Actor.ShowTNSIfSelectable(LocalizeString(Actor.SimDescription, "ConcertPay", new object[] { Actor, concertPayAmount }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                }
                else
                {
                    Actor.Occupation.ShowOccupationTNS(OmniCareer.LocalizeString(Actor.SimDescription, "PerformTone:ConcertPerformed", "Gameplay/Careers/Music/PerformTone:ConcertPerformed", new object[] { Actor }));
                }
            }
            return true;
        }

        private static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, "PerformConcert:" + name, "Gameplay/Objects/RabbitHoles/ShowVenue/PerformConcert:" + name, parameters);
        }

        private new class Definition : InteractionDefinition<Sim, ShowVenue, PerformConcert>
        {
            public override string GetInteractionName(Sim a, ShowVenue target, InteractionObjectPair interaction)
            {
                return PerformConcert.LocalizeString(a.SimDescription, "InteractionName", new object[0x0]);
            }

            public override bool Test(Sim a, ShowVenue target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                bool success = false;

                SkillBasedCareer career = SkillBasedCareerBooter.GetSkillBasedCareer(a, SkillNames.Guitar);
                if (career != null)
                {
                    success = true;

                    if (career.CareerLevel < NRaas.Careers.Settings.mBuskerLevelToGetPaidForConcerts)
                    {
                        greyedOutTooltipCallback = delegate
                        {
                            return Common.Localize("Busker:ConcertLevelTooLow", a.IsFemale, new object[] { Music.LevelToGetPaidForConcerts - 3 });
                        };
                        return false;
                    }
                }
                else
                {
                    success = OmniCareer.HasMetric<MetricConcertsPerformed>(a.Occupation);
                }

                if (success)
                {
                    if (!target.PerformConcertAllowed(ref greyedOutTooltipCallback))
                    {
                        return false;
                    }
                    if ((SimClock.Hours24 >= ShowVenue.kPerformConcertAvailableStartingAtHour) && (SimClock.Hours24 < ShowVenue.kPerformConcertAvailableEndingAtHour))
                    {
                        return true;
                    }

                    greyedOutTooltipCallback = delegate
                    {
                        return PerformConcert.LocalizeString(a.SimDescription, "CannotPerformConcert", new object[] { SimClockUtils.GetText(ShowVenue.kPerformConcertAvailableStartingAtHour), SimClockUtils.GetText(ShowVenue.kPerformConcertAvailableEndingAtHour) });
                    };
                }
                return false;
            }
        }
    }
}
