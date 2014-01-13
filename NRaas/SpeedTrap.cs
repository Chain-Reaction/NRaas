using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace NRaas
{
    public class SpeedTrap
    {
        static Delegate OnSleep;

        public static void SetDelegates(Delegate onSleep)
        {
            OnSleep = onSleep;
        }

        /*
         * void Sims3.SimIFace.Simulator::Sleep(uint32)
         * 
         * void SpeedTrap::Sleep(uint32)
         */

        public static void Sleep()
        {
            Sleep(0);
        }
        public static void Sleep(uint tickCount)
        {
            try
            {
                if (Simulator.CheckYieldingContext(false))
                {
                    End();

                    Sims3.SimIFace.Simulator.Sleep(tickCount);

                    Begin();
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.DebugException("Sleep", e);
                throw;
            }
        }

        public static void Begin()
        {
            if (OnSleep != null)
            {
                OnSleep.DynamicInvoke(new object[] { "Begin" });
            }
        }

        public static void End()
        {
            if (OnSleep != null)
            {
                OnSleep.DynamicInvoke(new object[] { "End" });
            }
        }
    }
}