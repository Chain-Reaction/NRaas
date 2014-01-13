using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class AlarmManagerReference
    {
        Common.AlarmTask mAlarm;

        StoryProgressionObject mManager = null;

        public AlarmManagerReference(StoryProgressionObject manager, Common.AlarmTask alarm)
        { 
            mManager = manager;
            mAlarm = alarm;
        }

        public bool Valid
        {
            get { return ((mAlarm != null) && (mAlarm.Valid)); }
        }

        public void Dispose()
        {
            mManager.RemoveAlarm(mAlarm);
            mAlarm = null;
        }
    }
}

