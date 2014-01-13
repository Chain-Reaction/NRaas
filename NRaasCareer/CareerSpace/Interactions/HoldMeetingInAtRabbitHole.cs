using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
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
    public class HoldMeetingInAtRabbitHole : Business.HoldMeetingInAtRabbitHole, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<BusinessAndJournalismRabbitHole, Business.HoldMeetingInAtRabbitHole.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<BusinessAndJournalismRabbitHole>(Singleton);
        }

        public override bool InRabbitHole()
        {
            try
            {
                StartStages();
                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    if (succeeded)
                    {
                        Business job = OmniCareer.Career<Business>(Actor.Occupation);
                        if (job != null)
                        {
                            job.MeetingsHeldToday++;
                            Household household = Actor.Household;
                            household.ModifyFamilyFunds(Business.kMoneyForHoldingMeetings);
                            Actor.ShowTNSIfSelectable(LocalizeString(Actor.SimDescription, "MoneyEarned", new object[] { Actor, Business.kMoneyForHoldingMeetings }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                        }
                    }
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
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

        public static string LocalizeString(SimDescription sim, string name, object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, name, "Gameplay/Careers/Business/HoldMeetingInAtRabbitHole:" + name, parameters);
        }

        [DoesntRequireTuning]
        public new class Definition : InteractionDefinition<Sim, RabbitHole, HoldMeetingInAtRabbitHole>
        {
            public Sim Actor;

            public Definition()
            { }
            public Definition(Sim actor)
            {
                Actor = actor;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return HoldMeetingInAtRabbitHole.LocalizeString(actor.SimDescription, "HoldMeeting", new object[0x0]);
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                OmniCareer job = a.Occupation as OmniCareer;
                if ((job != null) && (job.HasMetric<MetricMeetingsHeld>()))
                {
                    float num = SimClock.Hours24;
                    if ((num >= Business.kEarliestTimeToHoldMeeting) && (num <= Business.kLatestTimeToHoldMeeting))
                    {
                        if (a.MoodManager.MoodValue >= Business.kMinMoodToHoldMeeting)
                        {
                            return true;
                        }

                        greyedOutTooltipCallback = delegate
                        {
                            return HoldMeetingInAtRabbitHole.LocalizeString(a.SimDescription, "MoodConstraintOnMeetings", new object[] { a.SimDescription });
                        };
                    }
                    else
                    {
                        greyedOutTooltipCallback = delegate {
                            return HoldMeetingInAtRabbitHole.LocalizeString(a.SimDescription, "NoTimeToHoldMeeting", new object[] { a.SimDescription });
                        };
                    }
                }

                return false;
            }
        }
    }
}
