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
    public class GiveLectureInAtRabbitHole : Education.GiveLectureInAtRabbitHole, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, Education.GiveLectureInAtRabbitHole.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<RabbitHole>(Singleton);
        }

        public override bool InRabbitHole()
        {
            StartStages();
            BeginCommodityUpdates();

            bool succeeded = false;

            try
            {
                succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
            }
            finally
            {
                EndCommodityUpdates(succeeded);
            }

            if (succeeded)
            {
                Education occupation = OmniCareer.Career<Education>(Actor.Occupation);
                if (occupation != null)
                {
                    occupation.LecturesGivenToday++;
                    EventTracker.SendEvent(EventTypeId.kGaveEducationLecture, Actor);
                    Actor.ModifyFunds(Education.kMoneyForLecture);
                    Actor.ShowTNSIfSelectable(LocalizeString(Actor.IsFemale, "MoneyEarned", new object[] { Actor, Education.kMoneyForLecture }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                }
            }
            return succeeded;
        }

        public static string LocalizeString(SimDescription sim, string name, object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, name, "Gameplay/Careers/Education/GiveLectureInAtRabbitHole:" + name, parameters);
        }

        // Nested Types
        [DoesntRequireTuning]
        public new class Definition : InteractionDefinition<Sim, RabbitHole, GiveLectureInAtRabbitHole>
        {
            // Fields
            public Sim Actor;

            // Methods
            public Definition()
            {
            }

            public Definition(Sim actor)
            {
                Actor = actor;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return GiveLectureInAtRabbitHole.LocalizeString(actor.SimDescription, "GiveLecture", new object[0x0]);
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                OmniCareer job = a.Occupation as OmniCareer;
                if ((job != null) && (job.HasMetric<MetricLecturesGiven>()))
                {
                    float num = SimClock.Hours24;
                    if ((num >= Education.kEarliestTimeToGiveLecture) && (num <= Education.kLatestTimeToGiveLecture))
                    {
                        if (a.MoodManager.MoodValue >= Education.kMinMoodToGiveLecture)
                        {
                            return true;
                        }
                        greyedOutTooltipCallback = delegate {
                            return HoldMeetingInAtRabbitHole.LocalizeString(a.SimDescription, "MoodConstraintOnLectures", new object[] { a.SimDescription });
                        };
                    }
                    else
                    {
                        greyedOutTooltipCallback = delegate
                        {
                            return HoldMeetingInAtRabbitHole.LocalizeString(a.SimDescription, "NoTimeToGiveLecture", new object[] { a.SimDescription });
                        };
                    }
                }
                return false;
            }
        }
    }
}
