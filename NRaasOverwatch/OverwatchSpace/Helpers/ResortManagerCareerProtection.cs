using NRaas.CommonSpace.Stores;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Helpers
{
    public class ResortManagerCareerProtection : IDisposable
    {
        List<SafeStore> mStore;

        float mHours;

        public ResortManagerCareerProtection(Sim sim, float days)
            : this(NewList(sim), days)
        { }
        public ResortManagerCareerProtection(List<Sim> sims, float days)
        {
            mStore = new List<SafeStore>();

            float num = SimClock.ElapsedTime(TimeUnit.Hours, SimClock.CurrentTime());
            float num2 = days * 24f;

            mHours = num2 - num;

            foreach (Sim sim in sims)
            {
                mStore.Add(new SafeStore(sim.SimDescription, SafeStore.Flag.School));
            }
        }

        protected static List<Sim> NewList(Sim sim)
        {
            List<Sim> sims = new List<Sim>();
            sims.Add(sim);
            return sims;
        }

        public void Dispose()
        {
            foreach (SafeStore store in mStore)
            {
                store.Dispose();

                CareerManager manager = store.mSim.CareerManager;

                if (manager == null) continue;

                Occupation career = manager.mJob;
                if ((career != null) && (!career.HasOpenHours))
                {
                    EnsureVacationDays(career, mHours);
                }

                School school = manager.mSchool;
                if ((school != null) && (!school.HasOpenHours))
                {
                    EnsureVacationDays(school, mHours);
                }
            }
        }

        public static void EnsureVacationDays(Occupation ths, float hours)
        {
            int count = (int)hours / 24;

            bool flag = false;
            while ((ths.HoursUntilWork <= hours) && (count >= 0))
            {
                ths.mUnpaidDaysOff++;
                ths.SetHoursUntilWork();
                flag = true;

                count--;
            }

            if (flag)
            {
                Sim ownerSim = ths.OwnerSim;
                if (ownerSim != null)
                {
                    EventTracker.SendEvent(EventTypeId.kOccupationTookUnpaidTimeOff, ownerSim);
                }
            }
        }
    }
}
