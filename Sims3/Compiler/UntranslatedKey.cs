using Sims3.SimIFace;
using Sims3.SimIFace.SACS;
using System;
using System.Collections.Generic;
using System.Reflection;

public class ScriptErrorFunctions
{
    /*
  .method public hidebysig static bool  DisplayScriptError(class Sims3.SimIFace.IScriptProxy proxy,
                                                           class [mscorlib]System.Exception e) cil managed

     * 
     * 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.AppDomain [mscorlib]System.AppDomain::get_CurrentDomain()
    IL_0005:  ldstr      "ScriptErrorWindow"
    IL_000a:  callvirt   instance object [mscorlib]System.AppDomain::GetData(string)
    IL_000f:  castclass  [SimIFace]Sims3.SimIFace.IScriptErrorWindow
    IL_0014:  ldarg.0
    IL_0015:  ldarg.1
    IL_0016:  callvirt   instance bool [SimIFace]Sims3.SimIFace.IScriptErrorWindow::DisplayScriptError(class [SimIFace]Sims3.SimIFace.IScriptProxy,
                                                                                                       class [mscorlib]System.Exception)
    IL_001b:  ret
     */
    public class ScriptErrorWindowEx : Sims3.SimIFace.ScriptErrorWindow
    {
        public new static bool DisplayScriptError(Sims3.SimIFace.IScriptProxy proxy, Exception e)
        {
            return ((Sims3.SimIFace.IScriptErrorWindow)AppDomain.CurrentDomain.GetData("ScriptErrorWindow")).DisplayScriptError(proxy, e);
        }
    }

    /*
  .method public hidebysig static string 
          GetUnlocalizedStringFormat(uint64 key) cil managed
     * 
     * 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarga.s   key
    IL_0002:  call       instance string [mscorlib]System.UInt64::ToString()
    IL_0007:  ret
     */
    private static string GetUnlocalizedStringFormat(ulong key)
    {
        return key.ToString();
    }

    /*
  .method public hidebysig static string 
          GetUnlocalizedStringFormat(string key) cil managed
     * 
     * 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
     */
    private static string GetUnlocalizedStringFormat(string key)
    {
        return key;
    }

    public class WorldEx : Sims3.SimIFace.World
    {
        /*
.method private hidebysig static void  OnStartupApp() cil managed
         * 
      // Code size       46 (0x2e)
      .maxstack  3
      .locals init (class [mscorlib]System.Exception V_0)
      IL_0000:  ldsfld     class [mscorlib]System.EventHandler [SimIFace]Sims3.SimIFace.World::sOnStartupAppEventHandler
      IL_0005:  brfalse.s  IL_002d

      .try
      {
        IL_0007:  ldsfld     class [mscorlib]System.EventHandler [SimIFace]Sims3.SimIFace.World::sOnStartupAppEventHandler
        IL_000c:  ldsfld     class [SimIFace]Sims3.SimIFace.IWorld [SimIFace]Sims3.SimIFace.World::gWorld
        IL_0011:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
        IL_0016:  newobj     instance void [SimIFace]Sims3.SimIFace.World/OnStartupAppEventArgs::.ctor()
        IL_001b:  callvirt   instance void [mscorlib]System.EventHandler::Invoke(object,
                                                                                 class [mscorlib]System.EventArgs)
        IL_0020:  leave.s    IL_002d

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0022:  stloc.0
        IL_0023:  ldnull
        IL_0024:  ldloc.0
        IL_0025:  call       bool [SimIFace]Sims3.SimIFace.ScriptErrorWindow::DisplayScriptError(class [SimIFace]Sims3.SimIFace.IScriptProxy,
                                                                                                 class [mscorlib]System.Exception)
        IL_002a:  pop
        IL_002b:  leave.s    IL_002d

      }  // end handler
      IL_002d:  ret
         */
        private new static void OnStartupApp()
        {
            if (sOnStartupAppEventHandler != null)
            {
                try
                {
                    sOnStartupAppEventHandler(gWorld.GetType(), new OnStartupAppEventArgs());
                }
                catch (Exception e)
                {
                    ScriptErrorWindow.DisplayScriptError(null, e);
                }
            }
        }

        /*
.method private hidebysig static void  OnObjectPlacedInLot(uint64 objectId,
         * 
      // Code size       48 (0x30)
      .maxstack  5
      .locals init (class [mscorlib]System.Exception V_0)
      IL_0000:  ldsfld     class [mscorlib]System.EventHandler [SimIFace]Sims3.SimIFace.World::sOnObjectPlacedInLotEventHandler
      IL_0005:  brfalse.s  IL_002f

      .try
      {
        IL_0007:  ldsfld     class [mscorlib]System.EventHandler [SimIFace]Sims3.SimIFace.World::sOnObjectPlacedInLotEventHandler
        IL_000c:  ldsfld     class [SimIFace]Sims3.SimIFace.IWorld [SimIFace]Sims3.SimIFace.World::gWorld
        IL_0011:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
        IL_0016:  ldarg.0
        IL_0017:  ldarg.1
        IL_0018:  newobj     instance void [SimIFace]Sims3.SimIFace.World/OnObjectPlacedInLotEventArgs::.ctor(uint64,
                                                                                                              uint64)
        IL_001d:  callvirt   instance void [mscorlib]System.EventHandler::Invoke(object,
                                                                                 class [mscorlib]System.EventArgs)
        IL_0022:  leave.s    IL_002f

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0024:  stloc.0
        IL_0025:  ldnull
        IL_0026:  ldloc.0
        IL_0027:  call       bool [SimIFace]Sims3.SimIFace.ScriptErrorWindow::DisplayScriptError(class [SimIFace]Sims3.SimIFace.IScriptProxy,
                                                                                                 class [mscorlib]System.Exception)
        IL_002c:  pop
        IL_002d:  leave.s    IL_002f

      }  // end handler
      IL_002f:  ret
         */
        private new static void OnObjectPlacedInLot(ulong objectId, ulong lotId)
        {
            if (sOnObjectPlacedInLotEventHandler != null)
            {
                try
                {
                    sOnObjectPlacedInLotEventHandler(gWorld.GetType(), new OnObjectPlacedInLotEventArgs(objectId, lotId));
                }
                catch (Exception e)
                {
                    ScriptErrorWindow.DisplayScriptError(null, e);
                }
            }
        }

        /*
.method private hidebysig static void  OnWorldLoadFinished() cil managed
         * 
      // Code size       51 (0x33)
      .maxstack  3
      .locals init (class [mscorlib]System.Exception V_0)
      IL_0000:  call       void [SimIFace]Sims3.SimIFace.World::ResetCompositeTimes()
      IL_0005:  ldsfld     class [mscorlib]System.EventHandler [SimIFace]Sims3.SimIFace.World::sOnWorldLoadFinishedEventHandler
      IL_000a:  brfalse.s  IL_0032

      .try
      {
        IL_000c:  ldsfld     class [mscorlib]System.EventHandler [SimIFace]Sims3.SimIFace.World::sOnWorldLoadFinishedEventHandler
        IL_0011:  ldsfld     class [SimIFace]Sims3.SimIFace.IWorld [SimIFace]Sims3.SimIFace.World::gWorld
        IL_0016:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
        IL_001b:  newobj     instance void [SimIFace]Sims3.SimIFace.World/OnWorldLoadFinishedEventArgs::.ctor()
        IL_0020:  callvirt   instance void [mscorlib]System.EventHandler::Invoke(object,
                                                                                 class [mscorlib]System.EventArgs)
        IL_0025:  leave.s    IL_0032

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0027:  stloc.0
        IL_0028:  ldnull
        IL_0029:  ldloc.0
        IL_002a:  call       bool [SimIFace]Sims3.SimIFace.ScriptErrorWindow::DisplayScriptError(class [SimIFace]Sims3.SimIFace.IScriptProxy,
                                                                                                 class [mscorlib]System.Exception)
        IL_002f:  pop
        IL_0030:  leave.s    IL_0032

      }  // end handler
      IL_0032:  ret
         */
        private new static void OnWorldLoadFinished()
        {
            ResetCompositeTimes();
            if (sOnWorldLoadFinishedEventHandler != null)
            {
                try
                {
                    sOnWorldLoadFinishedEventHandler(gWorld.GetType(), new OnWorldLoadFinishedEventArgs());
                }
                catch (Exception e)
                {
                    ScriptErrorWindow.DisplayScriptError(null, e);
                }
            }
        }

        /*
.method private hidebysig static void  OnWorldQuit() cil managed
         * 
      // Code size       46 (0x2e)
      .maxstack  3
      .locals init (class [mscorlib]System.Exception V_0)
      IL_0000:  ldsfld     class [mscorlib]System.EventHandler [SimIFace]Sims3.SimIFace.World::sOnWorldQuitEventHandler
      IL_0005:  brfalse.s  IL_002d

      .try
      {
        IL_0007:  ldsfld     class [mscorlib]System.EventHandler [SimIFace]Sims3.SimIFace.World::sOnWorldQuitEventHandler
        IL_000c:  ldsfld     class [SimIFace]Sims3.SimIFace.IWorld [SimIFace]Sims3.SimIFace.World::gWorld
        IL_0011:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
        IL_0016:  newobj     instance void [SimIFace]Sims3.SimIFace.World/OnWorldQuitEventArgs::.ctor()
        IL_001b:  callvirt   instance void [mscorlib]System.EventHandler::Invoke(object,
                                                                                 class [mscorlib]System.EventArgs)
        IL_0020:  leave.s    IL_002d

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0022:  stloc.0
        IL_0023:  ldnull
        IL_0024:  ldloc.0
        IL_0025:  call       bool [SimIFace]Sims3.SimIFace.ScriptErrorWindow::DisplayScriptError(class [SimIFace]Sims3.SimIFace.IScriptProxy,
                                                                                                 class [mscorlib]System.Exception)
        IL_002a:  pop
        IL_002b:  leave.s    IL_002d

      }  // end handler
      IL_002d:  ret
         */
        private new static void OnWorldQuit()
        {
            if (sOnWorldQuitEventHandler != null)
            {
                try
                {
                    sOnWorldQuitEventHandler(gWorld.GetType(), new OnWorldQuitEventArgs());
                }
                catch (Exception e)
                {
                    ScriptErrorWindow.DisplayScriptError(null, e);
                }
            }
        }
    }

    public class StateMachineClientEx : Sims3.SimIFace.StateMachineClient
    {
        private new void OnEvent(IEvent evt, bool callback, bool synchronous)
        {
            if ((evt.EventSubType == 0x2) && (evt.EventId >= 0x96))
            {
                string msg = string.Format(this.Name + ":{0:x}:{1}:{2}:{3}", new object[] { evt.EventType, Enum.GetName(typeof(SacsEventSubTypes), evt.EventSubType), ResourceUtils.UnHash(evt.EventId), evt.EventMessage });

                try
                {
                    throw new Exception();
                }
                catch (Exception e)
                {
                    msg += System.Environment.NewLine + e.StackTrace;
                }

                Exception exception = new SacsErrorException(msg, mPendingException);
                mPendingException = null;
                if (!callback)
                {
                    throw exception;
                }
                mPendingException = exception;
            }

            if (synchronous)
            {
                FireSynchronousEventHandlers(evt);
            }
            else
            {
                FireEventHandlers(evt);
            }
        }
    }

    public struct PersistVariableEx
    {
        /*
    .method public hidebysig newslot virtual final 
            instance void  Read(class Sims3.SimIFace.FastStreamReader reader,
                                class Sims3.SimIFace.IPersistReader persistReader) cil managed
         * 
         * 
               object V_4,
               class [mscorlib]System.Type V_5,
               class [mscorlib]System.Reflection.MethodInfo V_6,
               object[] V_7)
      IL_0000:  ldarg.2
      IL_0001:  callvirt   instance object [SimIFace]Sims3.SimIFace.IPersistReader::ReadReference()
      IL_0006:  castclass  [mscorlib]System.Type
      IL_000b:  stloc.0
      IL_000c:  ldarg.1
      IL_000d:  callvirt   instance string [SimIFace]Sims3.SimIFace.FastStreamReader::ReadString()
      IL_0012:  stloc.1
      IL_0013:  ldarg.2
      IL_0014:  callvirt   instance object [SimIFace]Sims3.SimIFace.IPersistReader::ReadReference()
      IL_0019:  stloc.2
      IL_001a:  ldloc.0
      IL_001b:  brfalse.s  IL_0099

      IL_001d:  ldnull
      IL_001e:  stloc.3
      IL_001f:  ldloc.0
      IL_0020:  ldloc.1
      IL_0021:  ldc.i4.s   56
      IL_0023:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Type::GetField(string,
                                                                                                                valuetype [mscorlib]System.Reflection.BindingFlags)
      IL_0028:  stloc.3
      IL_0029:  ldloc.3
      IL_002a:  brfalse.s  IL_0099

      IL_002c:  ldloc.3
      IL_002d:  callvirt   instance bool [mscorlib]System.Reflection.FieldInfo::get_IsStatic()
      IL_0032:  brfalse.s  IL_0099

      IL_0034:  ldloc.3
      IL_0035:  call       bool [SimIFace]Sims3.SimIFace.PersistStatic::IsFieldSerializable(class [mscorlib]System.Reflection.FieldInfo)
      IL_003a:  brfalse.s  IL_0099

      .try
      {
        IL_003c:  ldloc.3
        IL_003d:  ldnull
        IL_003e:  ldloc.2
        IL_003f:  callvirt   instance void [mscorlib]System.Reflection.FieldInfo::SetValue(object,
                                                                                           object)
        IL_0044:  ldloc.3
        IL_0045:  ldnull
        IL_0046:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
        IL_004b:  stloc.s    V_4
        IL_004d:  ldstr      "ScriptCore.ExceptionTrap, ScriptCore"
        IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetType(string)
        IL_0057:  stloc.s    V_5
        IL_0059:  ldloc.s    V_5
        IL_005b:  brfalse.s  IL_0094

        IL_005d:  ldloc.s    V_5
        IL_005f:  ldstr      "ExternalLoadReference"
        IL_0064:  ldc.i4.s   24
        IL_0066:  callvirt   instance class [mscorlib]System.Reflection.MethodInfo [mscorlib]System.Type::GetMethod(string,
                                                                                                                    valuetype [mscorlib]System.Reflection.BindingFlags)
        IL_006b:  stloc.s    V_6
        IL_006d:  ldloc.s    V_6
        IL_006f:  brfalse.s  IL_0094

        IL_0071:  ldloc.s    V_6
        IL_0073:  ldnull
        IL_0074:  ldc.i4.3
        IL_0075:  newarr     [mscorlib]System.Object
        IL_007a:  stloc.s    V_7
        IL_007c:  ldloc.s    V_7
        IL_007e:  ldc.i4.0
        IL_007f:  ldloc.2
        IL_0080:  stelem.ref
        IL_0081:  ldloc.s    V_7
        IL_0083:  ldc.i4.1
        IL_0084:  ldloc.s    V_4
        IL_0086:  stelem.ref
        IL_0087:  ldloc.s    V_7
        IL_0089:  ldc.i4.2
        IL_008a:  ldloc.3
        IL_008b:  stelem.ref
        IL_008c:  ldloc.s    V_7
        IL_008e:  callvirt   instance object [mscorlib]System.Reflection.MethodBase::Invoke(object,
                                                                                            object[])
        IL_0093:  pop
        IL_0094:  leave.s    IL_0099

      }  // end .try
      catch [mscorlib]System.ArgumentException 
      {
        IL_0096:  pop
        IL_0097:  leave.s    IL_0099

      }  // end handler
      IL_0099:  ret
         */
        public void Read(FastStreamReader reader, IPersistReader persistReader)
        {
            Type type = (Type)persistReader.ReadReference();
            string name = reader.ReadString();
            object obj2 = persistReader.ReadReference();
            if (type != null)
            {
                FieldInfo fieldInfo = null;
                fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (((fieldInfo != null) && fieldInfo.IsStatic) && PersistStatic.IsFieldSerializable(fieldInfo))
                {
                    try
                    {
                        fieldInfo.SetValue(null, obj2);

                        object parent = fieldInfo.GetValue(null);

                        Type exceptionTrap = Type.GetType("ScriptCore.ExceptionTrap, ScriptCore");
                        if (exceptionTrap != null)
                        {
                            MethodInfo loadReference = exceptionTrap.GetMethod("ExternalLoadReference", BindingFlags.Public | BindingFlags.Static);
                            if (loadReference != null)
                            {
                                loadReference.Invoke(null, new object[] { obj2, parent, fieldInfo });
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
        }
    }
}
