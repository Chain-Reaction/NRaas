using System;
using Sims3.SimIFace;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.RabbitHoles;
using System.Collections.Generic;
using Sims3.Gameplay.ActorSystems;
using Sims3.UI;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Core;

[assembly: Tunable]
namespace OpenCinema
{
    public class OpenCinemaRH
    {
        [Tunable]
        protected static bool leffaNuuttis;

        [Tunable]
        protected static int movieFee;

        [TunableComment("Range:  Simoleons.  Description:  Number of Simoleons earned when performing a concert."), Tunable]
        private static int kPerformConcertPayoff = 0x3e8;
        [Tunable, TunableComment("Range:  Float under 1.0f.  Description:  Percentage of variance in pay when performing a concert.")]
        private static float kPerformConcertPayoffVariance = 0.25f;


        static OpenCinemaRH()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
        }

        static void World_OnWorldLoadFinishedEventHandler(object sender, EventArgs e)
        {
            EventTracker.AddListener(EventTypeId.kWatchedTv, new ProcessEventDelegate(OpenCinemaRH.WatchedTV));
            EventTracker.AddListener(EventTypeId.kPerformedGuitar, new ProcessEventDelegate(OpenCinemaRH.PerformedSong));
        }

        #region Watched TV
        protected static ListenerAction WatchedTV(Event e)
        {

            Sim sim = e.Actor as Sim;
            if (sim != null)
            {
                if (sim.LotCurrent.IsCommunityLot)
                {
                    List<Theatre> rList = new List<Theatre>(Sims3.Gameplay.Queries.GetObjects<Theatre>());
                    foreach (Theatre r in rList)
                    {
                        //If we are watching TV in a community lot with a theater RH object
                        if (r.LotCurrent.IsCommunityLot && r.LotCurrent == sim.LotCurrent)
                        {
                            //Give The moodlet if we don't have it, or if time left <= 14h  
                            if (!sim.BuffManager.HasElement(BuffNames.SawGreatMovie) || (sim.BuffManager.HasElement(BuffNames.SawGreatMovie) && sim.BuffManager.GetElement(BuffNames.SawGreatMovie).TimeoutCount <= 840f))
                            {
                                //Remove the old Buff if we have it 
                                if (sim.BuffManager.HasElement(BuffNames.SawGreatMovie))
                                {
                                    sim.BuffManager.RemoveElement(BuffNames.SawGreatMovie);
                                }

                                //Pay for move
                                CommonMethods.PayForMovie(sim, sim.LotCurrent, movieFee);

                                //Satisfy want
                                CommonMethods.FulfillWant(sim, r);

                                //Get Buff
                                sim.BuffManager.AddElement(BuffNames.SawGreatMovie, Origin.FromTheatre);

                            }

                        }
                    }
                }
            }
            return ListenerAction.Keep;
        }

        #endregion

        #region Performed Song


        protected static ListenerAction PerformedSong(Event e)
        {
            Sim sim = e.Actor as Sim;

            if (sim != null)
            {
                if (sim.LotCurrent.IsCommunityLot)
                {
                    List<Theatre> rList = new List<Theatre>(Sims3.Gameplay.Queries.GetObjects<Theatre>());
                    foreach (Theatre r in rList)
                    {
                        //If we performed a song in a lot with a theater in it, trigger perfomance 
                        if (r.LotCurrent.IsCommunityLot && r.LotCurrent == sim.LotCurrent && sim.Occupation != null && sim.Occupation.Guid == OccupationNames.Music)
                        {                            
                            EventTracker.SendEvent(EventTypeId.kPerformedConcert, sim);

                            Music occupation = sim.Occupation as Music;
                            occupation.ConcertsPerformed++;
                            if ((sim.Occupation as Music).CurLevel.Level >= Music.LevelToGetPaidForConcerts)
                            {
                                int concertPayAmount = (int)(RandomUtil.GetDouble((double)(1f - kPerformConcertPayoffVariance), (double)(1f + kPerformConcertPayoffVariance)) * kPerformConcertPayoff);
                                sim.Household.ModifyFamilyFunds(concertPayAmount);
                                sim.ShowTNSIfSelectable(LocalizeString("ConcertPay", new object[] { sim, concertPayAmount }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, sim.ObjectId);
                            }
                            else
                            {
                                sim.Occupation.ShowOccupationTNS(Localization.LocalizeString(sim.IsFemale, "Gameplay/Careers/Music/PerformTone:ConcertPerformed", new object[] { sim }));
                            }

                        }
                    }
                }
            }
            return ListenerAction.Keep;
        }
        private static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("Gameplay/Objects/RabbitHoles/ShowVenue/PerformConcert:" + name, parameters);
        }


        #endregion
    }
}
