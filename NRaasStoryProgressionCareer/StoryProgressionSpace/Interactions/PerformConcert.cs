using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
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
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class PerformConcert : ShowVenue.PerformConcert, Common.IPreLoad
    {
        public new static readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<ShowVenue, ShowVenue.PerformConcert.Definition, Definition>(false);
        }

        public override bool InRabbitHole()
        {
            try
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

                    int level = Music.LevelToGetPaidForConcerts;

                    Music music = Actor.Occupation as Music;
                    if (music != null)
                    {
                        music.ConcertsPerformed++;
                    }
                    else
                    {
                        level -= 3;
                    }


                    if (Actor.Occupation.CareerLevel >= level)
                    {
                        int concertPayAmount = Target.ConcertPayAmount;

                        StoryProgression.Main.Money.AdjustFunds(Actor.SimDescription, "Concert", concertPayAmount);

                        if (StoryProgression.Main.Careers.MatchesAlertLevel(Actor))
                        {
                            Common.Notify(Actor, Common.Localize("ConcertPlay:Success", Actor.IsFemale, new object[] { Actor, concertPayAmount }));
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

        // Nested Types
        private new class Definition : InteractionDefinition<Sim, ShowVenue, PerformConcert>
        {
            // Methods
            public override string GetInteractionName(Sim a, ShowVenue target, InteractionObjectPair interaction)
            {
                return ShowVenue.PerformConcert.LocalizeString("InteractionName", new object[0x0]);
            }

            public override bool Test(Sim a, ShowVenue target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a.Occupation == null)
                    {
                        return false;
                    }

                    if (!target.PerformConcertAllowed(ref greyedOutTooltipCallback))
                    {
                        return false;
                    }

                    if ((SimClock.Hours24 >= ShowVenue.kPerformConcertAvailableStartingAtHour) && (SimClock.Hours24 < ShowVenue.kPerformConcertAvailableEndingAtHour))
                    {
                        return true;
                    }
                }
                catch(Exception e)
                {
                    Common.Exception(a, target, e);
                }

                return false;
            }
        }
    }
}
