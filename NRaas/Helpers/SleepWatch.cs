using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class SleepWatch
    {
        StopWatch mWatch;

        uint mInterval;

        public SleepWatch()
            : this(100)
        { }
        public SleepWatch(uint interval)
        {
            mInterval = interval;

            mWatch = StopWatch.Create(StopWatch.TickStyles.Milliseconds);
            mWatch.Start();
        }

        public void Restart()
        {
            mWatch.Restart();
        }

        public delegate void Logger(string text);

        public void Sleep()
        {
            if (mWatch.GetElapsedTime() >= mInterval)
            {
                SpeedTrap.Sleep();
                mWatch.Restart();
            }
        }
    }
}

