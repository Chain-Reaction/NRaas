using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Helpers
{
    public class AskToWatchChildHelper : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
        }

        [Persistable]
        public class AskToWatchChildController
        {
            public static Common.AlarmTask sReturnAlarm;
            public static Common.AlarmTask sMischiefAlarm;
            public ulong mReturnToSim = 0L;
            public ulong mWatchingSim = 0L;
            public int mWatchDays = 0;
            public int mWatchHours = 0;
            public bool mWatchingSimDied = false;

            [Persistable(false)]
            public Common.DelayedEventListener mSimDiedListener;
            public Common.DelayedEventListener mSimAgedUpListener;

            public AskToWatchChildController()
            {
            }

            public bool Init()
            {
                return false;
            }

            public void AddAlarmAndListeners()
            {
                if(sReturnAlarm == null || !sReturnAlarm.Valid)
                {
                    int hours = 24 * mWatchDays;
                    sReturnAlarm = new Common.AlarmTask(hours + mWatchHours, TimeUnit.Hours, this.OnReturnAlarm);
                }

                if (mSimDiedListener == null || (mSimDiedListener.Listener != null && mSimDiedListener.Listener.IsCompleted))
                {
                    mSimDiedListener = new Common.DelayedEventListener(EventTypeId.kSimDied, this.OnSimDied);
                }
            }

            public void OnReturnAlarm()
            {
                                                
            }

            public void OnSimDied(Event e)
            {
                if (e.Actor != null && e.Actor.SimDescription != null)
                {
                    if (e.Actor.SimDescription.SimDescriptionId == this.mReturnToSim)
                    {
                        this.mWatchingSimDied = true;
                        this.OnReturnAlarm();
                    }
                }
            }
        }            
    }
}
