using Sims3.SimIFace;
using System;

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