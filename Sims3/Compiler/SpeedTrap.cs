using Sims3.SimIFace;
using Sims3.SimIFace.SACS;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.UniqueNameSpace
{
    public class SpeedTrap
    {
        public static void Sleep(uint tickCount)
        {
            NRaas.SpeedTrap.End();

            Sims3.SimIFace.Simulator.Sleep(tickCount);

            NRaas.SpeedTrap.Begin();
        }
    }
}

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
    IL_008f:  stloc.s    V_9
    IL_0091:  ldloca.s   V_7
    //IL_0093:  ldarg.1
    //IL_0094:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.FastStreamReader::ReadUInt32()
	L_0005:  ldc.i4.0
    IL_0099:  stfld      uint32 ScriptCore.TaskStateCollection/MethodEntry::checksum
         */

        /*
L_0000:  call       void NRaas.SpeedTrap::End()
L_0005:  ldarg.0
L_0006:  call       void [SimIFace]Sims3.SimIFace.Simulator::Sleep(uint32)
L_000b:  call       void NRaas.SpeedTrap::Begin()
         * 
L_0000:  call       void NRaas.SpeedTrap::End()
L_0005:  ldc.i4.0
L_0006:  call       void [SimIFace]Sims3.SimIFace.Simulator::Sleep(uint32)
L_000b:  call       void NRaas.SpeedTrap::Begin()
         */
        /*
         * void Sims3.SimIFace.Simulator::Sleep(uint32)
         * void [SimIFace]Sims3.SimIFace.Simulator::Sleep(uint32)
         * 
         * void NRaas.SpeedTrap::Sleep(uint32)
         */

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
