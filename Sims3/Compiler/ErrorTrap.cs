using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ScriptCore
{
    public class ExceptionTrap
    {
        public delegate void ScriptError(ScriptProxy proxy, Exception e);

        public delegate void PostLoad(ScriptProxy proxy, Sims3.SimIFace.IScriptLogic logic, bool postLoad);

        public delegate void RemoveObjectFunc(ObjectGuid id);

        public delegate int GetOptionFunc(string option);

        public delegate void NotifyFunc(object[] objects);

        public delegate void ProcessObjectGuidFunc(ObjectGuid guid, bool added);

        public delegate void LoadObjectFunc(int index, ref object obj);
        public delegate void LoadArrayReferenceFunc(object child, ref object parent);
        public delegate void LoadReferenceFunc(ref object child, ref object parent, FieldInfo field);

        public static NotifyFunc OnNotify = null;

        public static ScriptError OnScriptError;

        public static PostLoad OnPrePostLoad;
        public static PostLoad OnPostPostLoad;

        public static ProcessObjectGuidFunc OnProcessObjectGuid;
        public static LoadObjectFunc OnLoadObject;
        public static LoadReferenceFunc OnLoadReference;
        public static LoadArrayReferenceFunc OnLoadArrayReference;
        public static RemoveObjectFunc OnRemoveObject;
        public static GetOptionFunc OnGetOption;

        public static int sDepth = 0;

        public static void Notify(string text)
        {
            Notify(new object[] { text });
        }
        public static void Notify(object[] objects)
        {
            if (OnNotify != null)
            {
                OnNotify(objects);
            }
        }

        public static bool Exception(ScriptProxy proxy, Exception e)
        {
            if (OnScriptError != null)
            {
                OnScriptError(proxy, e);
                return true;
            }

            return false;
        }

        public static void RemoveObject(ObjectGuid id)
        {
            if (OnRemoveObject != null)
            {
                OnRemoveObject(id);
            }
        }

        public static ObjectGuid ProcessObjectGuid(ObjectGuid guid, bool added)
        {
            if (OnProcessObjectGuid != null)
            {
                OnProcessObjectGuid(guid, added);
            }

            return guid;
        }

        public static void LoadObject(int index, ScriptCore.PersistReader.ObjectLoadMode loadMode, ref object obj)
        {
            if (OnLoadObject != null)
            {
                if (loadMode != PersistReader.ObjectLoadMode.Create) return;

                OnLoadObject(index, ref obj);
            }
        }

        public static void ExternalLoadReference(object child, object parent, FieldInfo field)
        {
            LoadReference(ref child, ref parent, field);
        }

        public static void LoadReference(ref object child, ref object parent)
        {
            if (OnLoadReference != null)
            {
                OnLoadReference(ref child, ref parent, null);
            }
        }
        public static void LoadReference(ref object child, ref object parent, FieldInfo field)
        {
            if (OnLoadReference != null)
            {
                OnLoadReference(ref child, ref parent, field);
            }
        }
        public static void LoadReference<T>(T[] children, ref object parent)
        {
            if (OnLoadArrayReference != null)
            {
                OnLoadArrayReference(children, ref parent);
            }
        }

        public static int GetOption(string option)
        {
            if (OnGetOption != null)
            {
                return OnGetOption(option);
            }

            return 0;
        }
    }

    public class TaskStateCollectionEx : ScriptCore.TaskStateCollection
    {
        /*
         * } // end of method TaskStateCollection::Read
         * 
    L_0091:  ldstr      "ResetTaskStates"
    L_0096:  call       int32 ScriptCore.ExceptionTrap::GetOption(string)
    L_009b:  ldc.i4.1
    L_009c:  bne.un.s   IL_0091

    L_009e:  ldloca.s   V_7
    L_00a0:  ldc.i4.0
    L_00a1:  stfld      uint32 [ScriptCore]ScriptCore.TaskStateCollection/MethodEntry::checksum
    L_00a6:  br.s       IL_009e
         * 
    IL_0091:  ldloca.s   V_7
    IL_0093:  ldarg.1
    IL_0094:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.FastStreamReader::ReadUInt32()
    IL_0099:  stfld      uint32 ScriptCore.TaskStateCollection/MethodEntry::checksum
         * 
         */
    }

    public class WorldEx : ScriptCore.World
    {
        /*
  .method public hidebysig instance void 
          RemoveObjectFromObjectManager(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid scriptHandle) cil managed

            // Code size       19 (0x13)
            .maxstack  8
            IL_0000:  ldarg.1
            IL_0001:  call       void ScriptCore.ExceptionTrap::RemoveObject(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
            IL_0006:  ldarga.s   scriptHandle
            IL_0008:  call       instance uint64 [SimIFace]Sims3.SimIFace.ObjectGuid::get_Value()
            IL_000d:  call       void [ScriptCore]ScriptCore.World::World_RemoveObjectFromObjectManagerImpl(uint64)
            IL_0012:  ret
        */
        public new void RemoveObjectFromObjectManager(ObjectGuid scriptHandle)
        {
            ExceptionTrap.RemoveObject(scriptHandle);

            World_RemoveObjectFromObjectManagerImpl(scriptHandle.Value);
        }
    }

    public class LoadSaveManagerEx : ScriptCore.LoadSaveManager
    {
        internal new static void OnObjectGroupsPreLoad()
        {
            ObjectGroupsPreLoadHandler objectGroupsPreLoad = sInstance.ObjectGroupsPreLoad;
            if (objectGroupsPreLoad != null)
            {
                try
                {
                    objectGroupsPreLoad();
                }
                catch (Exception e)
                {
                    ExceptionTrap.Exception(null, e);
                }
            }
        }

        /*
.method public hidebysig newslot virtual final 
instance uint32  SaveGame(class [SimIFace]Sims3.SimIFace.LoadSaveFileInfo info,
                        bool isOnVacation) cil managed
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.LoadSaveFileInfo::get_NativeHandle()
    IL_0006:  ldc.i4.0
    IL_0007:  ldc.i4.1
    IL_0008:  call       uint32 [ScriptCore]ScriptCore.LoadSaveManager::LoadSaveManager_SaveGame_Impl(uint32,
                                                                                                      bool,
                                                                                                      bool)
    IL_000d:  ret
         */
        public new uint SaveGame(LoadSaveFileInfo info, bool isOnVacation)
        {
            return LoadSaveManager_SaveGame_Impl(info.NativeHandle, false, true);
        }
    }

    public class SimulatorEx : ScriptCore.Simulator
    {
        public new ObjectGuid CreateObject(ResourceKey prototypeKey, Hashtable overrideData)
        {
            ObjectGuid kBadObjectGuid = Sims3.SimIFace.Simulator.kBadObjectGuid;
            if (overrideData != null)
            {
                byte[] bytes;
                ArrayList list = new ArrayList();
                ArrayList list2 = new ArrayList();
                ArrayList list3 = new ArrayList();
                foreach (DictionaryEntry entry in overrideData)
                {
                    string str = (string)entry.Key;
                    object obj2 = entry.Value;
                    if (obj2 != null)
                    {
                        if (obj2.GetType() == typeof(string))
                        {
                            list.Add(str);
                        }
                        else if (obj2.GetType() == typeof(ResourceKey))
                        {
                            list2.Add(str);
                        }
                        else if (obj2.GetType() == typeof(uint))
                        {
                            list3.Add(str);
                        }
                    }
                }
                int num = (list.Count + list2.Count) + list3.Count;
                ASCIIEncoding encoding = new ASCIIEncoding();
                MemoryStream output = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(output);
                writer.Write(num);
                foreach (string str2 in list)
                {
                    string s = (string)overrideData[str2];
                    writer.Write((byte)0x0);
                    bytes = encoding.GetBytes(str2);
                    writer.Write((byte)bytes.Length);
                    writer.Write(bytes);
                    bytes = encoding.GetBytes(s);
                    writer.Write((byte)bytes.Length);
                    writer.Write(bytes);
                }
                foreach (string str4 in list2)
                {
                    ResourceKey key = (ResourceKey)overrideData[str4];
                    writer.Write((byte)0x1);
                    bytes = encoding.GetBytes(str4);
                    writer.Write((byte)bytes.Length);
                    writer.Write(bytes);
                    writer.Write(key.InstanceId);
                    writer.Write(key.GroupId);
                    writer.Write(key.TypeId);
                }
                foreach (string str5 in list3)
                {
                    uint num2 = (uint)overrideData[str5];
                    writer.Write((byte)0x2);
                    bytes = encoding.GetBytes(str5);
                    writer.Write((byte)bytes.Length);
                    writer.Write(bytes);
                    writer.Write(num2);
                }
                writer.Flush();

                return ExceptionTrap.ProcessObjectGuid(new ObjectGuid(Simulator_CreateObjectImpl(prototypeKey.InstanceId, prototypeKey.GroupId, prototypeKey.TypeId, output.GetBuffer())), true);
            }
            return ExceptionTrap.ProcessObjectGuid(new ObjectGuid(Simulator_CreateObjectImpl(prototypeKey.InstanceId, prototypeKey.GroupId, prototypeKey.TypeId, null)), true);
        }

        public new ObjectGuid AddObject(object obj, bool postLoad, ObjectGuid objectId)
        {
            if (obj is IScriptLogic)
            {
                ScriptProxy proxy = new ScriptProxy();
                if (!proxy.SetLogic((IScriptLogic)obj, postLoad))
                {
                    return Sims3.SimIFace.Simulator.kBadObjectGuid;
                }
                Simulator_AddObjectImpl(proxy, objectId.Value);
                return ExceptionTrap.ProcessObjectGuid(proxy.ObjectId, true);
            }
            ITask task = (ITask)obj;
            Simulator_AddObjectImpl(task, objectId.Value);
            return ExceptionTrap.ProcessObjectGuid(task.ObjectId, true);
        }

        public new void DestroyObject(ObjectGuid scriptHandle)
        {
            Simulator_DestroyObjectImpl(ExceptionTrap.ProcessObjectGuid(scriptHandle, false).Value);
        }
    }

    public class ScriptProxyEx : ScriptCore.ScriptProxy
    {
        /*
} // end of method ScriptProxy::set_ObjectId

  .method public hidebysig newslot virtual final 
          instance void  Simulate() cil managed
         * 
         * 
    // Code size       103 (0x67)
    .maxstack  2
    .locals init (class [mscorlib]System.Exception V_0,
             class [mscorlib]System.Exception V_1)
    // Code size       123 (0x7b)
    .maxstack  2
    .locals init (class [mscorlib]System.Exception V_0,
             class [mscorlib]System.Exception V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
    IL_0006:  brtrue.s   IL_0009

    IL_0008:  ret

    IL_0009:  call       void NRaas.SpeedTrap::Begin()
    .try
    {
      IL_000e:  ldarg.0
      IL_000f:  call       instance void [ScriptCore]ScriptCore.ScriptProxy::PreSimulate()
      IL_0014:  leave.s    IL_003d

    }  // end .try
    catch [SimIFace]Sims3.SimIFace.ResetException 
    {
      IL_0016:  pop
      IL_0017:  ldarg.0
      IL_0018:  call       instance void [ScriptCore]ScriptCore.ScriptProxy::OnReset()
      IL_001d:  call       void NRaas.SpeedTrap::End()
      IL_0022:  leave.s    IL_007a

    }  // end handler
    catch [mscorlib]System.Exception 
    {
      IL_0024:  stloc.0
      IL_0025:  ldarg.0
      IL_0026:  ldloc.0
      IL_0027:  call       instance bool [ScriptCore]ScriptCore.ScriptProxy::OnScriptError(class [mscorlib]System.Exception)
      IL_002c:  brtrue.s   IL_0036

      IL_002e:  ldarg.0
      IL_002f:  call       instance void [ScriptCore]ScriptCore.ScriptProxy::OnReset()
      IL_0034:  rethrow
      IL_0036:  call       void NRaas.SpeedTrap::End()
      IL_003b:  leave.s    IL_007a

    }  // end handler
    .try
    {
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_0043:  callvirt   instance void [SimIFace]Sims3.SimIFace.IScriptLogic::Simulate()
      IL_0048:  leave.s    IL_0075

    }  // end .try
    catch [SimIFace]Sims3.SimIFace.ResetException 
    {
      IL_004a:  pop
      IL_004b:  ldarg.0
      IL_004c:  call       instance void [ScriptCore]ScriptCore.ScriptProxy::OnReset()
      IL_0051:  leave.s    IL_0075

    }  // end handler
    catch [mscorlib]System.Exception 
    {
      IL_0053:  stloc.1
      IL_0054:  ldarg.0
      IL_0055:  call       instance void [ScriptCore]ScriptCore.ScriptProxy::SetObjectNotToReset()
      IL_005a:  ldarg.0
      IL_005b:  ldloc.1
      IL_005c:  call       instance bool [ScriptCore]ScriptCore.ScriptProxy::OnScriptError(class [mscorlib]System.Exception)
      IL_0061:  brtrue.s   IL_006b

      IL_0063:  ldarg.0
      IL_0064:  call       instance void [ScriptCore]ScriptCore.ScriptProxy::OnReset()
      IL_0069:  rethrow
      IL_006b:  leave.s    IL_006d

    }  // end handler
    IL_006d:  ldc.i4.0
    IL_006e:  call       void [SimIFace]Sims3.SimIFace.Simulator::Sleep(uint32)
    IL_0073:  br.s       IL_003d

    IL_0075:  call       void NRaas.SpeedTrap::End()
    IL_007a:  ret
         */
        public new void Simulate()
        {
            if (mTarget == null) return;

            NRaas.SpeedTrap.Begin();

            try
            {
                PreSimulate();
            }
            catch (ResetException)
            {
                OnReset();

                NRaas.SpeedTrap.End();
                return;
            }
            catch (Exception exception)
            {
                if (!OnScriptError(exception))
                {
                    OnReset();
                    throw;
                }

                NRaas.SpeedTrap.End();
                return;
            }

            while (true)
            {
                try
                {
                    mTarget.Simulate();
                    break;
                }
                catch (ResetException)
                {
                    OnReset();
                    break;
                }
                catch (Exception exception)
                {
                    SetObjectNotToReset();
                    if (!OnScriptError(exception))
                    {
                        OnReset();
                        throw;
                    }
                }

                // Required if OnScriptError() returns true
                Sims3.SimIFace.Simulator.Sleep(0);
            }

            NRaas.SpeedTrap.End();
        }
    }

    public class ScriptProxyEx2 : ScriptCore.ScriptProxy
    {
        /*
  .method public hidebysig newslot virtual final 
          instance bool  OnScriptError(class [mscorlib]System.Exception e) cil managed
         * 
         * 
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       bool ScriptCore.ExceptionTrap::Exception(class [ScriptCore]ScriptCore.ScriptProxy,
                                                                  class [mscorlib]System.Exception)
    IL_0007:  brfalse.s  IL_000b

    IL_0009:  ldc.i4.1
    IL_000a:  ret

    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  call       bool [SimIFace]Sims3.SimIFace.ScriptErrorWindow::DisplayScriptError(class [SimIFace]Sims3.SimIFace.IScriptProxy,
                                                                                             class [mscorlib]System.Exception)
    IL_0012:  ret
         */
        public new bool OnScriptError(Exception e)
        {
            if (ExceptionTrap.Exception(this, e))
            {
                return true;
            }
            else
            {
                return Sims3.SimIFace.ScriptErrorWindow.DisplayScriptError(this, e);
            }
        }
    }

    public class ScriptProxyEx3 : ScriptCore.ScriptProxy
    {
        /*
  .method assembly hidebysig instance bool 
          SetLogic(class [SimIFace]Sims3.SimIFace.IScriptLogic obj,
                   bool postLoad) cil managed
         * 
         * 
    // Code size       188 (0xbc)
    .maxstack  4
    .locals init (class [mscorlib]System.Exception V_0,
             object[] V_1)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_000d:  ldarg.0
      IL_000e:  callvirt   instance void [SimIFace]Sims3.SimIFace.IScriptLogic::set_Proxy(class [SimIFace]Sims3.SimIFace.IScriptProxy)
      .try
      {
        IL_0013:  ldsfld     class ScriptCore.ExceptionTrap/PostLoad ScriptCore.ExceptionTrap::OnPrePostLoad
        IL_0018:  brfalse.s  IL_002c

        IL_001a:  ldsfld     class ScriptCore.ExceptionTrap/PostLoad ScriptCore.ExceptionTrap::OnPrePostLoad
        IL_001f:  ldarg.0
        IL_0020:  ldarg.0
        IL_0021:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
        IL_0026:  ldarg.2
        IL_0027:  callvirt   instance void ScriptCore.ExceptionTrap/PostLoad::Invoke(class [ScriptCore]ScriptCore.ScriptProxy,
                                                                                     class [SimIFace]Sims3.SimIFace.IScriptLogic,
                                                                                     bool)
        .try
        {
          IL_002c:  ldarg.0
          IL_002d:  ldarg.0
          IL_002e:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
          IL_0033:  ldarg.2
          IL_0034:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [SimIFace]Sims3.SimIFace.IScriptLogic::Init(bool)
          IL_0039:  stfld      valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [ScriptCore]ScriptCore.ScriptProxy::mExecuteType
          IL_003e:  leave.s    IL_005a

        }  // end .try
        finally
        {
          IL_0040:  ldsfld     class ScriptCore.ExceptionTrap/PostLoad ScriptCore.ExceptionTrap::OnPostPostLoad
          IL_0045:  brfalse.s  IL_0059

          IL_0047:  ldsfld     class ScriptCore.ExceptionTrap/PostLoad ScriptCore.ExceptionTrap::OnPostPostLoad
          IL_004c:  ldarg.0
          IL_004d:  ldarg.0
          IL_004e:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
          IL_0053:  ldarg.2
          IL_0054:  callvirt   instance void ScriptCore.ExceptionTrap/PostLoad::Invoke(class [ScriptCore]ScriptCore.ScriptProxy,
                                                                                       class [SimIFace]Sims3.SimIFace.IScriptLogic,
                                                                                       bool)
          IL_0059:  endfinally
        }  // end handler
        IL_005a:  leave.s    IL_0066

      }  // end .try
      catch [SimIFace]Sims3.SimIFace.ResetException 
      {
        IL_005c:  pop
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [ScriptCore]ScriptCore.ScriptProxy::mExecuteType
        IL_0064:  leave.s    IL_0066

      }  // end handler
      IL_0066:  ldc.i4.3
      IL_0067:  newarr     [mscorlib]System.Object
      IL_006c:  stloc.1
      IL_006d:  ldloc.1
      IL_006e:  ldc.i4.0
      IL_006f:  ldstr      "ScriptProxy:SetLogic"
      IL_0074:  stelem.ref
      IL_0075:  ldloc.1
      IL_0076:  ldc.i4.1
      IL_0077:  ldarg.0
      IL_0078:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [ScriptCore]ScriptCore.ScriptProxy::mExecuteType
      IL_007d:  box        [SimIFace]Sims3.SimIFace.ScriptExecuteType
      IL_0082:  stelem.ref
      IL_0083:  ldloc.1
      IL_0084:  ldc.i4.2
      IL_0085:  ldarg.0
      IL_0086:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_008b:  stelem.ref
      IL_008c:  ldloc.1
      IL_008d:  call       void ScriptCore.ExceptionTrap::Notify(object[])
      IL_0092:  ldarg.0
      IL_0093:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [ScriptCore]ScriptCore.ScriptProxy::mExecuteType
      IL_0098:  brtrue.s   IL_00ad

      IL_009a:  ldarg.0
      IL_009b:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_00a0:  ldnull
      IL_00a1:  callvirt   instance void [SimIFace]Sims3.SimIFace.IScriptLogic::set_Proxy(class [SimIFace]Sims3.SimIFace.IScriptProxy)
      IL_00a6:  ldarg.0
      IL_00a7:  ldnull
      IL_00a8:  stfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_00ad:  leave.s    IL_00ba

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_00af:  stloc.0
      IL_00b0:  ldarg.0
      IL_00b1:  ldloc.0
      IL_00b2:  call       instance bool [ScriptCore]ScriptCore.ScriptProxy::OnScriptError(class [mscorlib]System.Exception)
      IL_00b7:  pop
      IL_00b8:  leave.s    IL_00ba

    }  // end handler
    IL_00ba:  ldc.i4.1
    IL_00bb:  ret
         */
        public new bool SetLogic(Sims3.SimIFace.IScriptLogic obj, bool postLoad)
        {
            try
            {
                mTarget = obj;
                mTarget.Proxy = this;
                try
                {
                    if (ExceptionTrap.OnPrePostLoad != null)
                    {
                        ExceptionTrap.OnPrePostLoad(this, mTarget, postLoad);
                    }

                    try
                    {
                        mExecuteType = mTarget.Init(postLoad);
                    }
                    finally
                    {
                        if (ExceptionTrap.OnPostPostLoad != null)
                        {
                            ExceptionTrap.OnPostPostLoad(this, mTarget, postLoad);
                        }
                    }
                }
                catch (Sims3.SimIFace.ResetException)
                {
                    mExecuteType = Sims3.SimIFace.ScriptExecuteType.InitFailed;
                }

                ExceptionTrap.Notify(new object[] { "ScriptProxy:SetLogic", mExecuteType, mTarget });

                if (mExecuteType == Sims3.SimIFace.ScriptExecuteType.InitFailed)
                {
                    mTarget.Proxy = null;
                    mTarget = null;
                }
            }
            catch (Exception exception)
            {
                OnScriptError(exception);
            }
            return true;
        }

        /*
  } // end of method ScriptProxy::Load

  .method public hidebysig newslot virtual final 
          instance void  PostLoad() cil managed
         * 
         * 
    // Code size       167 (0xa7)
    .maxstack  4
    .locals init (class [mscorlib]System.Exception V_0,
             object[] V_1)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_0006:  brfalse    IL_0099

      IL_000b:  ldsfld     class ScriptCore.ExceptionTrap/PostLoad ScriptCore.ExceptionTrap::OnPrePostLoad
      IL_0010:  brfalse.s  IL_0024

      IL_0012:  ldsfld     class ScriptCore.ExceptionTrap/PostLoad ScriptCore.ExceptionTrap::OnPrePostLoad
      IL_0017:  ldarg.0
      IL_0018:  ldarg.0
      IL_0019:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_001e:  ldc.i4.1
      IL_001f:  callvirt   instance void ScriptCore.ExceptionTrap/PostLoad::Invoke(class [ScriptCore]ScriptCore.ScriptProxy,
                                                                                   class [SimIFace]Sims3.SimIFace.IScriptLogic,
                                                                                   bool)
      .try
      {
        IL_0024:  ldarg.0
        IL_0025:  ldarg.0
        IL_0026:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
        IL_002b:  ldc.i4.1
        IL_002c:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [SimIFace]Sims3.SimIFace.IScriptLogic::Init(bool)
        IL_0031:  stfld      valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [ScriptCore]ScriptCore.ScriptProxy::mExecuteType
        IL_0036:  leave.s    IL_0052

      }  // end .try
      finally
      {
        IL_0038:  ldsfld     class ScriptCore.ExceptionTrap/PostLoad ScriptCore.ExceptionTrap::OnPostPostLoad
        IL_003d:  brfalse.s  IL_0051

        IL_003f:  ldsfld     class ScriptCore.ExceptionTrap/PostLoad ScriptCore.ExceptionTrap::OnPostPostLoad
        IL_0044:  ldarg.0
        IL_0045:  ldarg.0
        IL_0046:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
        IL_004b:  ldc.i4.1
        IL_004c:  callvirt   instance void ScriptCore.ExceptionTrap/PostLoad::Invoke(class [ScriptCore]ScriptCore.ScriptProxy,
                                                                                     class [SimIFace]Sims3.SimIFace.IScriptLogic,
                                                                                     bool)
        IL_0051:  endfinally
      }  // end handler
      IL_0052:  ldc.i4.3
      IL_0053:  newarr     [mscorlib]System.Object
      IL_0058:  stloc.1
      IL_0059:  ldloc.1
      IL_005a:  ldc.i4.0
      IL_005b:  ldstr      "ScriptProxy:PostLoad"
      IL_0060:  stelem.ref
      IL_0061:  ldloc.1
      IL_0062:  ldc.i4.1
      IL_0063:  ldarg.0
      IL_0064:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [ScriptCore]ScriptCore.ScriptProxy::mExecuteType
      IL_0069:  box        [SimIFace]Sims3.SimIFace.ScriptExecuteType
      IL_006e:  stelem.ref
      IL_006f:  ldloc.1
      IL_0070:  ldc.i4.2
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_0077:  stelem.ref
      IL_0078:  ldloc.1
      IL_0079:  call       void ScriptCore.ExceptionTrap::Notify(object[])
      IL_007e:  ldarg.0
      IL_007f:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ScriptExecuteType [ScriptCore]ScriptCore.ScriptProxy::mExecuteType
      IL_0084:  brtrue.s   IL_0099

      IL_0086:  ldarg.0
      IL_0087:  ldfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_008c:  ldnull
      IL_008d:  callvirt   instance void [SimIFace]Sims3.SimIFace.IScriptLogic::set_Proxy(class [SimIFace]Sims3.SimIFace.IScriptProxy)
      IL_0092:  ldarg.0
      IL_0093:  ldnull
      IL_0094:  stfld      class [SimIFace]Sims3.SimIFace.IScriptLogic [ScriptCore]ScriptCore.ScriptProxy::mTarget
      IL_0099:  leave.s    IL_00a6

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_009b:  stloc.0
      IL_009c:  ldarg.0
      IL_009d:  ldloc.0
      IL_009e:  call       instance bool [ScriptCore]ScriptCore.ScriptProxy::OnScriptError(class [mscorlib]System.Exception)
      IL_00a3:  pop
      IL_00a4:  leave.s    IL_00a6

    }  // end handler
    IL_00a6:  ret
         */
        public new void PostLoad()
        {
            try
            {
                if (mTarget != null)
                {
                    if (ExceptionTrap.OnPrePostLoad != null)
                    {
                        ExceptionTrap.OnPrePostLoad(this, mTarget, true);
                    }

                    try
                    {
                        mExecuteType = mTarget.Init(true);
                    }
                    finally
                    {
                        if (ExceptionTrap.OnPostPostLoad != null)
                        {
                            ExceptionTrap.OnPostPostLoad(this, mTarget, true);
                        }
                    }

                    ExceptionTrap.Notify(new object[] { "ScriptProxy:PostLoad", mExecuteType, mTarget });

                    if (mExecuteType == Sims3.SimIFace.ScriptExecuteType.InitFailed)
                    {
                        mTarget.Proxy = null;
                        mTarget = null;
                    }
                }
            }
            catch (Exception exception)
            {
                OnScriptError(exception);
            }
        }
    }

    public class PersistReaderEx : ScriptCore.PersistReader
    {
        /*
      IL_03b1:  callvirt   instance void [mscorlib]System.Reflection.FieldInfo::SetValue(object,
                                                                                         object)

AL_1004:  ldloca.s   V_10
AL_1006:  ldarg.2
AL_1007:  ldloc V_6
AL_1008:  call       void ScriptCore.ExceptionTrap::LoadReference(object&,
                                                                    object&,
                                                                    class [mscorlib]System.Reflection.FieldInfo)
         */
        public void PartialLoadObject(PersistedTypeCode saveType, ref object obj, ScriptCore.PersistReader.ObjectLoadMode loadMode)
        {
            object obj3 = null;

            FieldInfo mFieldInfo = null;

            ExceptionTrap.LoadReference(ref obj3, ref obj, mFieldInfo);
        }

        /*
      IL_051d:  callvirt   instance void [mscorlib]System.Array::SetValue(object,
                                                                          int32)
AL_2002:  ldloca.s V_20
AL_2003:  ldarg.2
AL_2006:  call       void ScriptCore.ExceptionTrap::LoadReference(object&,
                                                                object&)
        */
        public void PartialLoadArray(PersistedTypeCode saveType, ref object obj, ScriptCore.PersistReader.ObjectLoadMode loadMode)
        {
            object obj3 = null;

            ExceptionTrap.LoadReference(ref obj3, ref obj);
        }
    }

    public class DictionaryFormatterEx
    {
        /*
    IDictionaryFormatter`3::Read
    IL_0057:  callvirt   instance void class [mscorlib]System.Collections.Generic.IDictionary`2<!K,!V>::Add(!0,
                                                                                                            !1)

AL_1004:  ldloca V_3
AL_1005:  ldarg.1
AL_1008:  call       void ScriptCore.ExceptionTrap::LoadReference(object&,
                                                                    object&)
AL_100d:  ldloca V_4
AL_100e:  ldarg.1
AL_1011:  call       void ScriptCore.ExceptionTrap::LoadReference(object&,
                                                                    object&)
         */
        public void Read(ref object obj, FastStreamReader reader, IPersistReader persistReader, Type type)
        {
            object obj2 = null;
            object obj3 = null;

            ExceptionTrap.LoadReference(ref obj2, ref obj);
            ExceptionTrap.LoadReference(ref obj3, ref obj);
        }
    }

    public class ListFormatterEx<T> : ListFormatter<T>
    {
        /* ListFormatter`1::Read

    IL_001c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<!T>::AddRange(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    
AL_1002:  ldloc.0
AL_1003:  ldarg.1
AL_1005:  call       void ScriptCore.ExceptionTrap::LoadReference<!T>(!!0[],
                                                                        object&)
         */

        public new void Read(ref object obj, FastStreamReader reader, IPersistReader persistReader, Type type)
        {
            T[] collection = null;

            ExceptionTrap.LoadReference(collection, ref obj);
        }
    }

    public class StackFormatterEx<T> : StackFormatter<T>
    {
        /*  StackFormatter`1::Read
    IL_0037:  bge.s      IL_0024

AL_1002:  ldloc.0
AL_1003:  ldarg.1
AL_1005:  call       void ScriptCore.ExceptionTrap::LoadReference<!T>(!!0[],
                                                                        object&)
         */

        public new void Read(ref object obj, FastStreamReader reader, IPersistReader persistReader, Type type)
        {
            T[] collection = null;

            ExceptionTrap.LoadReference(collection, ref obj);
        }
    }

    public class QueueFormatterEx<T> : QueueFormatter<T>
    {
        /*  QueueFormatter`1::Read
    IL_0035:  blt.s      IL_0020

AL_1002:  ldloc.0
AL_1003:  ldarg.1
AL_1005:  call       void ScriptCore.ExceptionTrap::LoadReference<!T>(!!0[],
                                                                        object&)
         */

        public new void Read(ref object obj, FastStreamReader reader, IPersistReader persistReader, Type type)
        {
            T[] collection = null;

            ExceptionTrap.LoadReference(collection, ref obj);
        }
    }

    public class ArrayListFormatterEx : ArrayListFormatter
    {
        /*  ArrayListFormatter::Read
    IL_001c:  callvirt   instance void [mscorlib]System.Collections.ArrayList::AddRange(class [mscorlib]System.Collections.ICollection)

AL_1002:  ldloc.0
AL_1003:  ldarg.1
AL_1005:  call       void ScriptCore.ExceptionTrap::LoadReference<object>(!!0[],
                                                                        object&)
         */

        public new void Read(ref object obj, FastStreamReader reader, IPersistReader persistReader, Type type)
        {
            object[] collection = null;

            ExceptionTrap.LoadReference(collection, ref obj);
        }
    }

    public class HashtableFormatterEx : HashtableFormatter
    {
        /*  HashtableFormatter::Read
    IL_002e:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
AL_1004:  ldloca V_3
AL_1005:  ldarg.1
AL_1007:  call       void ScriptCore.ExceptionTrap::LoadReference(object&,
                                                                    object&)
AL_100c:  ldloca V_4
AL_100d:  ldarg.1
AL_100f:  call       void ScriptCore.ExceptionTrap::LoadReference(object&,
                                                                    object&)
         */

        public new void Read(ref object obj, FastStreamReader reader, IPersistReader persistReader, Type type)
        {
            object key = null;
            object obj3 = null;

            ExceptionTrap.LoadReference(ref key, ref obj);
            ExceptionTrap.LoadReference(ref obj3, ref obj);
        }
    }

    public class ExceptionTest
    {
        /*
    ,class [mscorlib]System.Exception V_Exception)
    .try
    {
      L_1006:  leave.s    L_1013
    }  // end .try
    catch [mscorlib]System.Exception 
    {
      L_1008:  stloc.s V_Exception
      L_1009:  ldnull
      L_100a:  ldloc.s V_Exception
      L_100b:  call       bool ScriptCore.ExceptionTrap::Exception(class [ScriptCore]ScriptCore.ScriptProxy,
                                                                    class [mscorlib]System.Exception)
      L_1010:  pop
      L_1011:  leave.s    L_1013

    }  // end handler
    L_1013:  ret
         */

        public void Test()
        {
            try
            {
                Test();
            }
            catch (Exception e)
            {
                ExceptionTrap.Exception(null, e);
            }
        }
    }

    public class OnlineFeaturesEx : ScriptCore.OnlineFeatures
    {
        public new bool GetStoreManifestFile()
        {
            ExceptionTrap.Notify("GetStoreManifestFile");

            return OnlineFeaturesHost_GetStoreManifestFile();
        }

        public new bool IsPackInstalled(string packageId)
        {
            ExceptionTrap.Notify("IsPackInstalled");

            return OnlineFeaturesHost_IsPackInstalled(packageId);
        }

        public new void LogTelemetryEvent(TelemetryModuleId moduleId, TelemetryGroupId grpId, string telString)
        {
            ExceptionTrap.Notify(new object[] { "LogTelemetryEvent", telString });

            OnlineFeaturesHost_LogTelemetryEvent(moduleId, grpId, telString);
        }

        public new bool PostPipeMessage(Sims3Launcher.S3L_ServerName pipename, Sims3Launcher.S3L_NamedPipeMessage message)
        {
            ExceptionTrap.Notify("PostPipeMessage");

            return OnlineFeaturesHost_PostPipeMessage(pipename, message, "");
        }

        public new bool PostPipeMessage(Sims3Launcher.S3L_ServerName pipename, Sims3Launcher.S3L_NamedPipeMessage message, string messageData)
        {
            ExceptionTrap.Notify("PostPipeMessage");

            return OnlineFeaturesHost_PostPipeMessage(pipename, message, messageData);
        }

        public new void StartLauncher(bool bTakeFocus)
        {
            OnlineFeaturesHost_StartLauncher(bTakeFocus);
        }
    }
}