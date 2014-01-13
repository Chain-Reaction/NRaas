using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/*
 * SwitchWorlds
 * ImportTravellingHousehold
 * OnArrivalAtVacationWorld
 * 
    ,class [mscorlib]System.Exception exception)

L_1008: stloc exception
L_100a: call class [mscorlib]System.AppDomain [mscorlib]System.AppDomain::get_CurrentDomain()
L_100f: ldstr "ScriptErrorWindow"
L_1014: callvirt instance object [mscorlib]System.AppDomain::GetData(string)
L_1019: castclass [SimIFace]Sims3.SimIFace.IScriptErrorWindow
L_101e: ldnull 
L_101f: ldloc exception
L_1020: callvirt instance bool [SimIFace]Sims3.SimIFace.IScriptErrorWindow::DisplayScriptError(class [SimIFace]Sims3.SimIFace.IScriptProxy, class [mscorlib]System.Exception)
L_1025: pop 
*/

/*
    .maxstack  4
L_1010: stloc exception
L_1000: ldnull 
L_1001: ldloc exception
L_1002: ldc.i4.0 
L_1003: newobj instance void [Sims3GameplaySystems]Sims3.Gameplay.Utilities.ScriptError::.ctor(class [SimIFace]Sims3.SimIFace.IScriptProxy, class [mscorlib]System.Exception, int32)
L_1008: call instance string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.ScriptError::WriteMiniScriptError()
L_100d: pop 
*/

/*
 * -- New catch blocks --
 * 
 * ScriptProxy::PostLoad()
 * ScriptProxy::SetLogic()
 * PersistReader::Load() // new .try after existing throws
 * 
 * -- Existing catch blocks --
 * 
 * PersistReader::LoadObject()
 * PersistReader::ExecutePostLoads()
 * PersistReader::ReadObjects()
 * 
    ,class [mscorlib]System.Exception exception)
    .try
    {
...
    }  // end .try
    catch [mscorlib]System.Exception 
    {
L_1000:  call       void ScriptCore.ExceptionTrap::Add(class [mscorlib]System.Exception)
L_1001: leave.s    IL_????
    }  // end handler
 */

/*
    }  // end .try
    catch [mscorlib]System.Exception 
    {
IL_1006:  stloc exception
IL_1007:  ldloc exception
IL_1008:  call       void ScriptCore.ExceptionTrap::Add(class [mscorlib]System.Exception)
IL_100d:  ldloc exception
IL_100e:  throw
    }  // end handler
*/

/* SimDescription.Dispose    
        IL_0075:  pop
        IL_0076:  ldloc.2
        IL_0077:  ldloc.3
        IL_0078:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Sims3.Gameplay.CAS.SimDescription>::get_Item(int32)
        IL_007d:  callvirt   instance void Sims3.Gameplay.CAS.SimDescription::Dispose()
        IL_0082:  leave.s    IL_0088
*/

namespace NRaas
{
    public class Test : ScriptCore.TaskStateCollection
    {
        public void Read(FastStreamReader reader, IPersistReader persistReader)
        {
            int position = reader.Position;
            reader.ReadUInt32();
            uint num2 = reader.ReadUInt32();
            uint num3 = reader.ReadUInt32();
            uint num4 = reader.ReadUInt32();
            uint num5 = reader.ReadUInt32();
            int[] numArray = new int[num2];
            reader.Seek(position + ((int)num3), SeekOrigin.Begin);
            for (uint i = 0x0; i < num2; i++)
            {
                numArray[i] = reader.ReadInt32();
            }
            reader.Seek(position + ((int)num5), SeekOrigin.Begin);
            mMethodList.Capacity = (int)num4;
            mMethodList.Clear();
            for (uint j = 0x0; j < num4; j++)
            {
                MethodEntry entry;
                MethodInfo methodInfo = (MethodInfo)persistReader.ReadReferenceForceLive();
                if (ScriptCore.ExceptionTrap.GetOption("ResetTaskStates") == 1)
                {
                    entry.checksum = 0;
                }
                else
                {
                    entry.checksum = reader.ReadUInt32();
                }
                if (methodInfo != null)
                {
                    uint num8;
                    entry.handle = methodInfo.MethodHandle;
                    if (!ScriptCore.TaskControl.GetMethodChecksum(entry.handle, out num8))
                    {
                        entry.handle = new RuntimeMethodHandle();
                    }
                    else if (num8 != entry.checksum)
                    {
                        entry.handle = new RuntimeMethodHandle();
                    }
                    else if (!this.IsMethodSaveSafe(methodInfo))
                    {
                    }
                }
                else
                {
                    entry.handle = new RuntimeMethodHandle();
                }
                this.mMethodList.Add(entry);
            }
            this.mSavedTaskContexts = new ScriptCore.SavedTaskContext[num2];
            for (uint k = 0x0; k < num2; k++)
            {
                int num10 = numArray[k];
                if (num10 == 0x0)
                {
                    this.mSavedTaskContexts[k] = InvalidSavedTaskContext;
                }
                else if (num10 >= 0x0)
                {
                    reader.Seek(position + num10, SeekOrigin.Begin);
                    ScriptCore.SavedTaskContext invalidSavedTaskContext = ReadTask(reader, persistReader);
                    if (invalidSavedTaskContext == null)
                    {
                        invalidSavedTaskContext = InvalidSavedTaskContext;
                    }
                    this.mSavedTaskContexts[k] = invalidSavedTaskContext;
                }
            }
        }
    }

    /*
    public class NRaasException : Exception
    {
        public NRaasException()
            : base(GetStackTrace())
        { }

        public static string GetStackTrace()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    */
    /*
            class [mscorlib]System.Exception exception)
        .try
        {
            L_1000:  newobj     instance void [mscorlib]System.Exception::.ctor()
            L_1005:  throw

        }  // end .try
        catch [mscorlib]System.Exception 
        {
L_1006:  stloc exception
L_1007:  call       class [mscorlib]System.AppDomain [mscorlib]System.AppDomain::get_CurrentDomain()
L_100c:  ldstr      "ScriptErrorWindow"
L_1011:  callvirt   instance object [mscorlib]System.AppDomain::GetData(string)
L_1016:  castclass  [SimIFace]Sims3.SimIFace.IScriptErrorWindow
L_101b:  ldnull
L_101c:  ldloc exception
L_101d:  callvirt   instance bool [SimIFace]Sims3.SimIFace.IScriptErrorWindow::DisplayScriptError(class [SimIFace]Sims3.SimIFace.IScriptProxy,
                                                                                                    class [mscorlib]System.Exception)
L_1022:  pop
L_1023:  leave.s    IL_????

        }  // end handler
    */
/*
    public class ExceptionTest
    {
        public static void Test()
        {
            try
            {
                throw new Exception();
            }
            catch(Exception exception)
            {
                ((IScriptErrorWindow)AppDomain.CurrentDomain.GetData("ScriptErrorWindow")).DisplayScriptError(null, exception);
            }
        }
    }
*/ 
}