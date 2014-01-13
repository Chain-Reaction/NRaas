using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Skills;
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
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class WoohooCount : Common.IWorldLoadFinished, Common.IWorldQuit
    {
        public void OnWorldLoadFinished()
        {
            new Common.AlarmTask(4f, DaysOfTheWeek.All, OnTimer);
        }

        public void OnWorldQuit()
        {
            Woohooer.Settings.mWoohooCount.Clear();
        }

        protected static void OnTimer()
        {
            try
            {
                List<ulong> keys = new List<ulong>(Woohooer.Settings.mWoohooCount.Keys);
                foreach (ulong key in keys)
                {
                    int value = Woohooer.Settings.mWoohooCount[key] - Woohooer.Settings.mWoohooRechargeRate;

                    if (value < 0)
                    {
                        value = 0;
                    }

                    Woohooer.Settings.mWoohooCount[key] = value;
                }
            }
            catch (Exception e)
            {
                Common.Exception("WoohooCount:OnTimer", e);
            }
        }
    }
}
