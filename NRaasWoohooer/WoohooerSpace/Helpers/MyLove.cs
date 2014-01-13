using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class MyLove : Common.IWorldLoadFinished
    {
        static BuffInstance sMyLove = null;

        public void OnWorldLoadFinished()
        {
            GenericManager<BuffNames, BuffInstance, BuffInstance>.sDictionary.TryGetValue(ResourceUtils.HashString64("NRaasMyLove"), out sMyLove);

            new Common.AlarmTask(1f, TimeUnit.Minutes, OnTimer, 30f, TimeUnit.Minutes);

            new Common.DelayedEventListener(EventTypeId.kRoomChanged, OnRoomChanged);
        }

        protected static Sim ShouldHaveMyLove(Sim sim)
        {
            if (sim.LotCurrent == null) return null;

            if (sim.LotCurrent.IsWorldLot) return null;

            if (Woohooer.Settings.mMyLoveBuffLevel == Options.Romance.MyLoveBuffLevel.Default) return null;

            Sim found = null;

            foreach (Sim other in sim.LotCurrent.GetAllActors())
            {
                if (other == sim) continue;

                if (other.RoomId != sim.RoomId) continue;

                Relationship relation = Relationship.Get(sim, other, false);
                if (relation == null) continue;

                if ((relation.AreRomantic ()) && (relation.LTR.Liking > BuffMyLove.LikingValueForBuff))
                {
                    if ((relation.MarriedInGame) && (SimClock.ElapsedTime(TimeUnit.Days, relation.LTR.WhenStateStarted) < BuffMyLove.DaysSinceLTRChangeForBuff))
                    {
                        return null;
                    }

                    switch (Woohooer.Settings.mMyLoveBuffLevel)
                    {
                        case Options.Romance.MyLoveBuffLevel.Partner:
                            if (sim.Partner != other.SimDescription) continue;
                            break;
                        case Options.Romance.MyLoveBuffLevel.Spouse:
                            if (sim.Partner != other.SimDescription) continue;

                            if (!sim.IsMarried) continue;
                            break;
                    }

                    found = other;
                    break;
                }
            }

            return found;
        }

        protected static void ApplyBuff(Sim sim)
        {
            try
            {
                if (sMyLove == null) return;

                if (sim.BuffManager == null) return;

                Sim other = ShouldHaveMyLove(sim);

                BuffMyLove.BuffInstanceMyLove buff = sim.BuffManager.GetElement(sMyLove.Guid) as BuffMyLove.BuffInstanceMyLove;

                if (other != null)
                {
                    if (buff == null)
                    {
                        if (sim.BuffManager.AddElement(sMyLove.Guid, Origin.FromLover))
                        {
                            buff = sim.BuffManager.GetElement(sMyLove.Guid) as BuffMyLove.BuffInstanceMyLove;

                            ApplyBuff(other);
                        }
                    }

                    if (buff != null)
                    {
                        buff.mTimeoutCount = sMyLove.mTimeoutCount;

                        buff.Lover = other;
                    }
                }
                else
                {
                    sim.BuffManager.RemoveElement(sMyLove.Guid);

                    if ((buff != null) && (buff.Lover != null))
                    {
                        ApplyBuff(buff.Lover);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }

        protected static void OnTimer()
        {
            try
            {
                if (Household.ActiveHousehold == null) return;

                foreach (Sim sim in Households.AllSims(Household.ActiveHousehold))
                {
                    ApplyBuff(sim);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTimer", e);
            }
        }

        protected static void OnRoomChanged(Event e)
        {
            Sim obj = e.Actor as Sim;
            if ((obj != null) && ((obj.Household == Household.ActiveHousehold) || (obj.LotCurrent == LotManager.ActiveLot)))
            {
                ApplyBuff(obj);
            }
        }
    }
}
