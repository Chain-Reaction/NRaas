using NRaas.SaverSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.SaverSpace
{
    [Persistable]
    public class PersistedSettings
    {
        public int mRealMinutesBetweenSaves = SaverOptions.kRealMinutesBetweenSaves;
        public int mSimMinutesBetweenSaves = SaverOptions.kSimMinutesBetweenSaves;
        public List<float> mSimSaveHour = new List<float>(SaverOptions.kSimSaveHour);
        public int mSaveCycles = SaverOptions.kSaveCycles;
        public bool mPauseOnLoad = SaverOptions2.kPauseOnLoad;
        public bool mPauseOnSave = SaverOptions3.kPauseOnSave;

        public bool mSwitchToMapView = SaverOptions.kSwitchToMapView;

        public bool mPromptInBuildBuy = SaverOptions.kPromptInBuildBuy;

        private SaveStyle mSaveStyle = SaveStyle.Default;

        public SaveStyle SaveStyle
        {
            get
            {
                if (mSaveStyle == SaveStyle.Default)
                {
                    if (mRealMinutesBetweenSaves > 0)
                    {
                        return Options.SaveStyle.RealTime;
                    }
                    else if (mSimMinutesBetweenSaves > 0)
                    {
                        return Options.SaveStyle.SimTime;
                    }
                    else
                    {
                        return Options.SaveStyle.SimHour;
                    }
                }
                else
                {
                    return mSaveStyle;
                }
            }
            set
            {
                mSaveStyle = value;
            }
        }
    }
}
