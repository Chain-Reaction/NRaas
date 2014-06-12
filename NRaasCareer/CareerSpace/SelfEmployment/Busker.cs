using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Metrics;
using NRaas.Gameplay.Rewards;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CareerSpace.SelfEmployment
{
    public class Busker : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kGotTips, OnGotTips);
            new Common.DelayedEventListener(EventTypeId.kGotMoneyFromGig, OnGotMoneyFromGig);
            new Common.DelayedEventListener(EventTypeId.kPerformedConcert, OnPerformedConcert);
            new Common.DelayedEventListener(EventTypeId.kPlayedDrums, OnPlayedInstrument);
            new Common.DelayedEventListener(EventTypeId.kPlayedGuitar, OnPlayedInstrument);
            new Common.DelayedEventListener(EventTypeId.kPlayedPiano, OnPlayedInstrument);
            new Common.DelayedEventListener(EventTypeId.kPlayedBassGuitar, OnPlayedInstrument);
            new Common.DelayedEventListener(EventTypeId.kPlayedLaserHarp, OnPlayedInstrument);
        }

        public static void OnPlayedInstrument(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            SkillNames instrumentSkill = SkillNames.None;
            switch (e.Id)
            {
                case EventTypeId.kPlayedDrums:
                    instrumentSkill = SkillNames.Drums;
                    break;
                case EventTypeId.kPlayedGuitar:
                    instrumentSkill = SkillNames.Guitar;
                    break;
                case EventTypeId.kPlayedBassGuitar:
                    instrumentSkill = SkillNames.BassGuitar;
                    break;
                case EventTypeId.kPlayedPiano:
                    instrumentSkill = SkillNames.Piano;
                    break;
                case EventTypeId.kPlayedLaserHarp:
                    instrumentSkill = SkillNames.LaserHarp;
                    break;
            }

            float opening = 0, closing = 0;
            if (!Bartending.TryGetHoursOfOperation(actor.LotCurrent, ref opening, ref closing))
            {
                return;
            }

            if (!SimClock.IsTimeBetweenTimes(opening, closing))
            {
                return;
            }

            IncrementalEvent iEvent = e as IncrementalEvent;

            float cash = GetPay(actor, instrumentSkill) * iEvent.mIncrement;
            if (cash == 0)
            {
                return;
            }

            foreach (SkillNames skill in MetricMusicSkill.Skills)
            {
                if (SkillBasedCareerBooter.GetSkillBasedCareer(actor, skill) == null)
                {
                    continue;
                }

                SkillBasedCareerBooter.UpdateExperience(actor, skill, (int)cash);
            }
        }

        public static float GetPay(Sim sim, SkillNames skillName)
        {
            BandSkill skill = sim.SkillManager.GetSkill<BandSkill>(skillName);
            if (skill == null) return 0;

            float cashPerHour = skill.SkillLevel + skill.KnownCompositions.Count;

            cashPerHour *= (skill.KnownMasterCompositions.Count + 1);

            int listeners = 0;
            foreach(Sim other in sim.LotCurrent.GetSims())
            {
                if (other.SimDescription.AssignedRole != null) continue;

                if (other.RoomId == sim.RoomId)
                {
                    listeners++;
                }
            }

            cashPerHour *= listeners;

            return cashPerHour;
        }

        public static void OnGotTips(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            if ((actor.Household == null) || (actor.Household.IsSpecialHousehold))
            {
                return;
            }

            IncrementalEvent iEvent = e as IncrementalEvent;
            if (iEvent == null) return;

            foreach (SkillNames skill in MetricMusicSkill.Skills)
            {
                SkillBasedCareerBooter.UpdateExperience(actor, skill, (int)iEvent.mIncrement);
            }
        }

        public static void OnGotMoneyFromGig(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            if ((actor.Household == null) || (actor.Household.IsSpecialHousehold))
            {
                return;
            }

            IncrementalEvent iEvent = e as IncrementalEvent;

            foreach (SkillNames skill in MetricMusicSkill.Skills)
            {
                SkillBasedCareerBooter.UpdateExperience(actor, skill, (int)iEvent.mIncrement);
            }
        }

        public static void OnPerformedConcert(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            if ((actor.Household == null) || (actor.Household.IsSpecialHousehold))
            {
                return;
            }

            if (actor.InteractionQueue == null)
            {
                return;
            }

            InteractionInstance interaction = actor.InteractionQueue.GetCurrentInteraction();
            if (interaction == null)
            {
                return;
            }
                
            ShowVenue venue = interaction.Target as ShowVenue;
            if (venue == null)
            {
                return;
            }

            foreach (SkillNames skill in MetricMusicSkill.Skills)
            {
                SkillBasedCareerBooter.UpdateExperience(actor, skill, (int)venue.ConcertPayAmount);
            }
        }
    }
}
