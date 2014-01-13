
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern Sims3GameplaySystems
{
  .ver 1:0:0:0
}
.assembly extern SimIFace
{
  .ver 1:0:0:50
}
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern UI
{
  .ver 1:0:0:0
}
.assembly extern Sims3GameplayObjects
{
  .ver 1:0:0:0
}
.assembly StoreObjectsSaunaClassic
{
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 16 53 69 6D 73 33 53 74 6F 72 65 53 61 75   // ...Sims3StoreSau
                                                                                              6E 61 43 6C 61 73 73 69 63 00 00 )                // naClassic..

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 45 41 2E 63 6F 6D 00 00 )                // ...EA.com..
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 32 43 46 46 33 44 39 31 2D 38 46 30 46   // ..$2CFF3D91-8F0F
                                                                                                  2D 41 32 34 34 2D 37 39 41 46 2D 44 41 36 42 45   // -A244-79AF-DA6BE
                                                                                                  33 41 35 37 43 37 35 00 00 )                      // 3A57C75..
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1D 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  45 41 2E 63 6F 6D 20 32 30 30 37 2D 32 30 31 30   // EA.com 2007-2010
                                                                                                  00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 16 53 69 6D 73 33 53 74 6F 72 65 53 61 75   // ...Sims3StoreSau
                                                                                                6E 61 43 6C 61 73 73 69 63 00 00 )                // naClassic..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module StoreObjectsSaunaClassic.dll
// MVID: {F4A196D8-CA02-4FC1-9349-6050E0768068}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00370000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Sims3.Store.Objects.SaunaClassic
       extends [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject
       implements [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ICouch,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ICantBeDetonated,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.INotHauntable,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject,
                  [SimIFace]Sims3.SimIFace.IScriptLogic,
                  [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IObjectUI,
                  [SimIFace]Sims3.SimIFace.CustomContent.IExportableContent,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ISittable,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ISmashableObjectWhileBurning,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGiveComfort,
                  [Sims3GameplaySystems]Sims3.Gameplay.InteractionsShared.IDoNotAllowViewOnContainedObjects
{
  .custom instance void [SimIFace]Sims3.SimIFace.RuntimeExportAttribute::.ctor() = ( 01 00 00 00 ) 
  .class auto ansi nested public beforefieldinit SaunaSittableComponent
         extends [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SittableComponent
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SittableComponent::.ctor()
      IL_0006:  ret
    } // end of method SaunaSittableComponent::.ctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject o,
                                 int32 comfyBuffScore,
                                 string interactionName,
                                 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData> partList) cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  ldarg.3
      IL_0004:  ldarg.s    partList
      IL_0006:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SittableComponent::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                                        int32,
                                                                                                                        string,
                                                                                                                        class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData>)
      IL_000b:  ret
    } // end of method SaunaSittableComponent::.ctor

    .method public hidebysig virtual instance void 
            OnStartup() cil managed
    {
      // Code size       33 (0x21)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ObjectComponent::get_Owner()
      IL_0006:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.InteractionsShared.Sit::Singleton
      IL_000b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
      IL_0010:  ldarg.0
      IL_0011:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ObjectComponent::get_Owner()
      IL_0016:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.InteractionsShared.SitAndWait::MySingleton
      IL_001b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
      IL_0020:  ret
    } // end of method SaunaSittableComponent::OnStartup

  } // end of class SaunaSittableComponent

  .class auto ansi nested public beforefieldinit SportsStarWorkout
         extends [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff
  {
    .class auto ansi nested public beforefieldinit BuffInstanceSportsStarWorkout
           extends [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance
    {
      .field public valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle mRepeatingAlarm
      .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim mAffectedSim
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_0006:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mRepeatingAlarm
        IL_000b:  ldarg.0
        IL_000c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::.ctor()
        IL_0011:  ret
      } // end of method BuffInstanceSportsStarWorkout::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff buff,
                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames buffGuid,
                                   int32 effectValue,
                                   float32 timeoutCount) cil managed
      {
        // Code size       23 (0x17)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_0006:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mRepeatingAlarm
        IL_000b:  ldarg.0
        IL_000c:  ldarg.1
        IL_000d:  ldarg.2
        IL_000e:  ldarg.3
        IL_000f:  ldarg.s    timeoutCount
        IL_0011:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                 int32,
                                                                                                                 float32)
        IL_0016:  ret
      } // end of method BuffInstanceSportsStarWorkout::.ctor

      .method public hidebysig virtual instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance 
              Clone() cil managed
      {
        // Code size       30 (0x1e)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mBuff
        IL_0006:  ldarg.0
        IL_0007:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mBuffGuid
        IL_000c:  ldarg.0
        IL_000d:  ldfld      int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mEffectValue
        IL_0012:  ldarg.0
        IL_0013:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_0018:  newobj     instance void Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                                   int32,
                                                                                                                                   float32)
        IL_001d:  ret
      } // end of method BuffInstanceSportsStarWorkout::Clone

      .method public hidebysig instance void 
              StartRepeatingAlarm(bool startAlarm) cil managed
      {
        // Code size       116 (0x74)
        .maxstack  9
        IL_0000:  ldarg.1
        IL_0001:  brfalse.s  IL_0045

        IL_0003:  ldarg.0
        IL_0004:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mRepeatingAlarm
        IL_0009:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_000e:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::op_Equality(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle,
                                                                                                          valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0013:  brfalse.s  IL_0073

        IL_0015:  ldarg.0
        IL_0016:  ldarg.0
        IL_0017:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mAffectedSim
        IL_001c:  ldc.r4     0.0
        IL_0021:  ldc.i4.1
        IL_0022:  ldarg.0
        IL_0023:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::ResetFatigueCallback()
        IL_0029:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback::.ctor(object,
                                                                                                                    native int)
        IL_002e:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kBuffResetMotivesFrequency
        IL_0033:  ldc.i4.1
        IL_0034:  ldstr      "Enchantment Alarm"
        IL_0039:  ldc.i4.2
        IL_003a:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddAlarmRepeating(float32,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback,
                                                                                                                                                                                        float32,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                                        string,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmType)
        IL_003f:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mRepeatingAlarm
        IL_0044:  ret

        IL_0045:  ldarg.0
        IL_0046:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mRepeatingAlarm
        IL_004b:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_0050:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::op_Inequality(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle,
                                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0055:  brfalse.s  IL_0073

        IL_0057:  ldarg.0
        IL_0058:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mAffectedSim
        IL_005d:  ldarg.0
        IL_005e:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mRepeatingAlarm
        IL_0063:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveAlarm(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0068:  ldarg.0
        IL_0069:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_006e:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mRepeatingAlarm
        IL_0073:  ret
      } // end of method BuffInstanceSportsStarWorkout::StartRepeatingAlarm

      .method public hidebysig instance void 
              ResetFatigueCallback() cil managed
      {
        // Code size       27 (0x1b)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mAffectedSim
        IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_000b:  ldc.i4     0x10f38dc
        IL_0010:  ldc.r4     100.
        IL_0015:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::SetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                           float32)
        IL_001a:  ret
      } // end of method BuffInstanceSportsStarWorkout::ResetFatigueCallback

    } // end of class BuffInstanceSportsStarWorkout

    .field public static literal uint64 mGuid = uint64(0x9A7F5F1919DF0013)
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff/BuffData info) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff/BuffData)
      IL_0007:  ret
    } // end of method SportsStarWorkout::.ctor

    .method public hidebysig virtual instance void 
            OnAddition(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                       class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance bi,
                       bool travelReaddition) cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  2
      .locals init (class Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout V_0)
      IL_0000:  ldarg.2
      IL_0001:  isinst     Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldarg.1
      IL_0009:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::get_Actor()
      IL_000e:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::mAffectedSim
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.1
      IL_0015:  callvirt   instance void Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::StartRepeatingAlarm(bool)
      IL_001a:  ret
    } // end of method SportsStarWorkout::OnAddition

    .method public hidebysig virtual instance void 
            OnRemoval(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance bi) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  2
      .locals init (class Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout V_0)
      IL_0000:  ldarg.2
      IL_0001:  isinst     Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.0
      IL_0009:  callvirt   instance void Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::StartRepeatingAlarm(bool)
      IL_000e:  ret
    } // end of method SportsStarWorkout::OnRemoval

    .method public hidebysig virtual instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance 
            CreateBuffInstance() cil managed
    {
      // Code size       25 (0x19)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_BuffGuid()
      IL_0007:  ldarg.0
      IL_0008:  call       instance int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_EffectValue()
      IL_000d:  ldarg.0
      IL_000e:  call       instance float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_TimeoutSimMinutes()
      IL_0013:  newobj     instance void Sims3.Store.Objects.SaunaClassic/SportsStarWorkout/BuffInstanceSportsStarWorkout::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                                 int32,
                                                                                                                                 float32)
      IL_0018:  ret
    } // end of method SportsStarWorkout::CreateBuffInstance

    .method public hidebysig virtual instance bool 
            ShouldBlock(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff b) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffFatigued
      IL_0006:  brfalse.s  IL_000a

      IL_0008:  ldc.i4.1
      IL_0009:  ret

      IL_000a:  ldarg.0
      IL_000b:  ldarg.1
      IL_000c:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::ShouldBlock(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff)
      IL_0011:  ret
    } // end of method SportsStarWorkout::ShouldBlock

  } // end of class SportsStarWorkout

  .class auto ansi nested public beforefieldinit InnerPeace
         extends [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff
  {
    .class auto ansi nested public beforefieldinit BuffInstanceInnerPeace
           extends [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance
    {
      .field public valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle mRepeatingAlarm
      .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim mAffectedSim
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_0006:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mRepeatingAlarm
        IL_000b:  ldarg.0
        IL_000c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::.ctor()
        IL_0011:  ret
      } // end of method BuffInstanceInnerPeace::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff buff,
                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames buffGuid,
                                   int32 effectValue,
                                   float32 timeoutCount) cil managed
      {
        // Code size       23 (0x17)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_0006:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mRepeatingAlarm
        IL_000b:  ldarg.0
        IL_000c:  ldarg.1
        IL_000d:  ldarg.2
        IL_000e:  ldarg.3
        IL_000f:  ldarg.s    timeoutCount
        IL_0011:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                 int32,
                                                                                                                 float32)
        IL_0016:  ret
      } // end of method BuffInstanceInnerPeace::.ctor

      .method public hidebysig virtual instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance 
              Clone() cil managed
      {
        // Code size       30 (0x1e)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mBuff
        IL_0006:  ldarg.0
        IL_0007:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mBuffGuid
        IL_000c:  ldarg.0
        IL_000d:  ldfld      int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mEffectValue
        IL_0012:  ldarg.0
        IL_0013:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_0018:  newobj     instance void Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                     valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                     int32,
                                                                                                                     float32)
        IL_001d:  ret
      } // end of method BuffInstanceInnerPeace::Clone

      .method public hidebysig instance void 
              StartRepeatingAlarm(bool startAlarm) cil managed
      {
        // Code size       116 (0x74)
        .maxstack  9
        IL_0000:  ldarg.1
        IL_0001:  brfalse.s  IL_0045

        IL_0003:  ldarg.0
        IL_0004:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mRepeatingAlarm
        IL_0009:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_000e:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::op_Equality(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle,
                                                                                                          valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0013:  brfalse.s  IL_0073

        IL_0015:  ldarg.0
        IL_0016:  ldarg.0
        IL_0017:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mAffectedSim
        IL_001c:  ldc.r4     0.0
        IL_0021:  ldc.i4.1
        IL_0022:  ldarg.0
        IL_0023:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::ResetFunCallback()
        IL_0029:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback::.ctor(object,
                                                                                                                    native int)
        IL_002e:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kBuffResetMotivesFrequency
        IL_0033:  ldc.i4.1
        IL_0034:  ldstr      "Enchantment Alarm"
        IL_0039:  ldc.i4.2
        IL_003a:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddAlarmRepeating(float32,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback,
                                                                                                                                                                                        float32,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                                        string,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmType)
        IL_003f:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mRepeatingAlarm
        IL_0044:  ret

        IL_0045:  ldarg.0
        IL_0046:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mRepeatingAlarm
        IL_004b:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_0050:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::op_Inequality(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle,
                                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0055:  brfalse.s  IL_0073

        IL_0057:  ldarg.0
        IL_0058:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mAffectedSim
        IL_005d:  ldarg.0
        IL_005e:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mRepeatingAlarm
        IL_0063:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveAlarm(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0068:  ldarg.0
        IL_0069:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_006e:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mRepeatingAlarm
        IL_0073:  ret
      } // end of method BuffInstanceInnerPeace::StartRepeatingAlarm

      .method public hidebysig instance void 
              ResetFunCallback() cil managed
      {
        // Code size       54 (0x36)
        .maxstack  3
        .locals init (float32 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mAffectedSim
        IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_000b:  ldc.i4     0x10f38da
        IL_0010:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
        IL_0015:  stloc.0
        IL_0016:  ldloc.0
        IL_0017:  ldc.r4     0.0
        IL_001c:  bge.un.s   IL_0035

        IL_001e:  ldarg.0
        IL_001f:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mAffectedSim
        IL_0024:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_0029:  ldc.i4     0x10f38da
        IL_002e:  ldloc.0
        IL_002f:  neg
        IL_0030:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::SetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                           float32)
        IL_0035:  ret
      } // end of method BuffInstanceInnerPeace::ResetFunCallback

    } // end of class BuffInstanceInnerPeace

    .field public static literal uint64 mGuid = uint64(0x9A7F5F1919DF0014)
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff/BuffData info) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff/BuffData)
      IL_0007:  ret
    } // end of method InnerPeace::.ctor

    .method public hidebysig virtual instance void 
            OnAddition(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                       class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance bi,
                       bool travelReaddition) cil managed
    {
      // Code size       48 (0x30)
      .maxstack  2
      .locals init (class Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace V_0)
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::get_Actor()
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_000b:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kBuffFitnessModifier
      IL_0010:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::UpdateShapeDeltaMultiplier(float32)
      IL_0015:  ldarg.2
      IL_0016:  isinst     Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace
      IL_001b:  stloc.0
      IL_001c:  ldloc.0
      IL_001d:  ldarg.1
      IL_001e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::get_Actor()
      IL_0023:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::mAffectedSim
      IL_0028:  ldloc.0
      IL_0029:  ldc.i4.1
      IL_002a:  callvirt   instance void Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::StartRepeatingAlarm(bool)
      IL_002f:  ret
    } // end of method InnerPeace::OnAddition

    .method public hidebysig virtual instance void 
            OnRemoval(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance bi) cil managed
    {
      // Code size       37 (0x25)
      .maxstack  2
      .locals init (class Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace V_0)
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::get_Actor()
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_000b:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kBuffFitnessModifier
      IL_0010:  neg
      IL_0011:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::UpdateShapeDeltaMultiplier(float32)
      IL_0016:  ldarg.2
      IL_0017:  isinst     Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace
      IL_001c:  stloc.0
      IL_001d:  ldloc.0
      IL_001e:  ldc.i4.0
      IL_001f:  callvirt   instance void Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::StartRepeatingAlarm(bool)
      IL_0024:  ret
    } // end of method InnerPeace::OnRemoval

    .method public hidebysig virtual instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance 
            CreateBuffInstance() cil managed
    {
      // Code size       25 (0x19)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_BuffGuid()
      IL_0007:  ldarg.0
      IL_0008:  call       instance int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_EffectValue()
      IL_000d:  ldarg.0
      IL_000e:  call       instance float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_TimeoutSimMinutes()
      IL_0013:  newobj     instance void Sims3.Store.Objects.SaunaClassic/InnerPeace/BuffInstanceInnerPeace::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   int32,
                                                                                                                   float32)
      IL_0018:  ret
    } // end of method InnerPeace::CreateBuffInstance

    .method public hidebysig virtual instance bool 
            ShouldBlock(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff b) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffOverworked
      IL_0006:  brfalse.s  IL_000a

      IL_0008:  ldc.i4.1
      IL_0009:  ret

      IL_000a:  ldarg.0
      IL_000b:  ldarg.1
      IL_000c:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::ShouldBlock(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff)
      IL_0011:  ret
    } // end of method InnerPeace::ShouldBlock

  } // end of class InnerPeace

  .class auto ansi nested public beforefieldinit RefreshedStore
         extends [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff
  {
    .class auto ansi nested public beforefieldinit BuffInstanceRefreshedStore
           extends [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance
    {
      .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim mAffectedSim
      .field public class [SimIFace]Sims3.SimIFace.VisualEffect mDripEffectLArm
      .field public class [SimIFace]Sims3.SimIFace.VisualEffect mDripEffectRArm
      .field public class [SimIFace]Sims3.SimIFace.VisualEffect mDripEffectLThigh
      .field public class [SimIFace]Sims3.SimIFace.VisualEffect mDripEffectRThigh
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::.ctor()
        IL_0006:  ret
      } // end of method BuffInstanceRefreshedStore::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff buff,
                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames buffGuid,
                                   int32 effectValue,
                                   float32 timeoutCount) cil managed
      {
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  ldarg.2
        IL_0003:  ldarg.3
        IL_0004:  ldarg.s    timeoutCount
        IL_0006:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                 int32,
                                                                                                                 float32)
        IL_000b:  ret
      } // end of method BuffInstanceRefreshedStore::.ctor

      .method public hidebysig virtual instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance 
              Clone() cil managed
      {
        // Code size       30 (0x1e)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mBuff
        IL_0006:  ldarg.0
        IL_0007:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mBuffGuid
        IL_000c:  ldarg.0
        IL_000d:  ldfld      int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mEffectValue
        IL_0012:  ldarg.0
        IL_0013:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_0018:  newobj     instance void Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                             int32,
                                                                                                                             float32)
        IL_001d:  ret
      } // end of method BuffInstanceRefreshedStore::Clone

      .method public hidebysig instance void 
              StartDripFX() cil managed
      {
        // Code size       246 (0xf6)
        .maxstack  3
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mAffectedSim
        IL_0006:  brtrue.s   IL_0009

        IL_0008:  ret

        IL_0009:  ldarg.0
        IL_000a:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLArm
        IL_000f:  brtrue.s   IL_0044

        IL_0011:  ldarg.0
        IL_0012:  ldstr      "store_sauna_sweatArm"
        IL_0017:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
        IL_001c:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLArm
        IL_0021:  ldarg.0
        IL_0022:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLArm
        IL_0027:  ldarg.0
        IL_0028:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mAffectedSim
        IL_002d:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Slot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/FXJoints::LeftForearm
        IL_0032:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                           valuetype [SimIFace]Sims3.SimIFace.Slot)
        IL_0037:  pop
        IL_0038:  ldarg.0
        IL_0039:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLArm
        IL_003e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
        IL_0043:  pop
        IL_0044:  ldarg.0
        IL_0045:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRArm
        IL_004a:  brtrue.s   IL_007f

        IL_004c:  ldarg.0
        IL_004d:  ldstr      "store_sauna_sweatArm"
        IL_0052:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
        IL_0057:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRArm
        IL_005c:  ldarg.0
        IL_005d:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRArm
        IL_0062:  ldarg.0
        IL_0063:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mAffectedSim
        IL_0068:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Slot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/FXJoints::RightForearm
        IL_006d:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                           valuetype [SimIFace]Sims3.SimIFace.Slot)
        IL_0072:  pop
        IL_0073:  ldarg.0
        IL_0074:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRArm
        IL_0079:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
        IL_007e:  pop
        IL_007f:  ldarg.0
        IL_0080:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLThigh
        IL_0085:  brtrue.s   IL_00ba

        IL_0087:  ldarg.0
        IL_0088:  ldstr      "store_sauna_sweatThigh"
        IL_008d:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
        IL_0092:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLThigh
        IL_0097:  ldarg.0
        IL_0098:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLThigh
        IL_009d:  ldarg.0
        IL_009e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mAffectedSim
        IL_00a3:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Slot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/FXJoints::LeftThigh
        IL_00a8:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                           valuetype [SimIFace]Sims3.SimIFace.Slot)
        IL_00ad:  pop
        IL_00ae:  ldarg.0
        IL_00af:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLThigh
        IL_00b4:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
        IL_00b9:  pop
        IL_00ba:  ldarg.0
        IL_00bb:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRThigh
        IL_00c0:  brtrue.s   IL_00f5

        IL_00c2:  ldarg.0
        IL_00c3:  ldstr      "store_sauna_sweatThigh"
        IL_00c8:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
        IL_00cd:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRThigh
        IL_00d2:  ldarg.0
        IL_00d3:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRThigh
        IL_00d8:  ldarg.0
        IL_00d9:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mAffectedSim
        IL_00de:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Slot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/FXJoints::RightThigh
        IL_00e3:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                           valuetype [SimIFace]Sims3.SimIFace.Slot)
        IL_00e8:  pop
        IL_00e9:  ldarg.0
        IL_00ea:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRThigh
        IL_00ef:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
        IL_00f4:  pop
        IL_00f5:  ret
      } // end of method BuffInstanceRefreshedStore::StartDripFX

      .method public hidebysig instance void 
              StopDripFX() cil managed
      {
        // Code size       153 (0x99)
        .maxstack  2
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLArm
        IL_0006:  brfalse.s  IL_0026

        IL_0008:  ldarg.0
        IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLArm
        IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLArm
        IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
        IL_001f:  ldarg.0
        IL_0020:  ldnull
        IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLArm
        IL_0026:  ldarg.0
        IL_0027:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRArm
        IL_002c:  brfalse.s  IL_004c

        IL_002e:  ldarg.0
        IL_002f:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRArm
        IL_0034:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
        IL_0039:  pop
        IL_003a:  ldarg.0
        IL_003b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRArm
        IL_0040:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
        IL_0045:  ldarg.0
        IL_0046:  ldnull
        IL_0047:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRArm
        IL_004c:  ldarg.0
        IL_004d:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLThigh
        IL_0052:  brfalse.s  IL_0072

        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLThigh
        IL_005a:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
        IL_005f:  pop
        IL_0060:  ldarg.0
        IL_0061:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLThigh
        IL_0066:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
        IL_006b:  ldarg.0
        IL_006c:  ldnull
        IL_006d:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectLThigh
        IL_0072:  ldarg.0
        IL_0073:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRThigh
        IL_0078:  brfalse.s  IL_0098

        IL_007a:  ldarg.0
        IL_007b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRThigh
        IL_0080:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
        IL_0085:  pop
        IL_0086:  ldarg.0
        IL_0087:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRThigh
        IL_008c:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
        IL_0091:  ldarg.0
        IL_0092:  ldnull
        IL_0093:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mDripEffectRThigh
        IL_0098:  ret
      } // end of method BuffInstanceRefreshedStore::StopDripFX

    } // end of class BuffInstanceRefreshedStore

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff/BuffData info) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff/BuffData)
      IL_0007:  ret
    } // end of method RefreshedStore::.ctor

    .method public hidebysig virtual instance void 
            OnAddition(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                       class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance bi,
                       bool travelReaddition) cil managed
    {
      // Code size       29 (0x1d)
      .maxstack  2
      .locals init (class Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore V_0)
      IL_0000:  ldarg.2
      IL_0001:  isinst     Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_001c

      IL_000a:  ldloc.0
      IL_000b:  ldarg.1
      IL_000c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::get_Actor()
      IL_0011:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::mAffectedSim
      IL_0016:  ldloc.0
      IL_0017:  callvirt   instance void Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::StartDripFX()
      IL_001c:  ret
    } // end of method RefreshedStore::OnAddition

    .method public hidebysig virtual instance void 
            OnRemoval(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance bi) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  1
      .locals init (class Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore V_0)
      IL_0000:  ldarg.2
      IL_0001:  isinst     Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0010

      IL_000a:  ldloc.0
      IL_000b:  callvirt   instance void Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::StopDripFX()
      IL_0010:  ret
    } // end of method RefreshedStore::OnRemoval

    .method public hidebysig virtual instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance 
            CreateBuffInstance() cil managed
    {
      // Code size       25 (0x19)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_BuffGuid()
      IL_0007:  ldarg.0
      IL_0008:  call       instance int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_EffectValue()
      IL_000d:  ldarg.0
      IL_000e:  call       instance float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_TimeoutSimMinutes()
      IL_0013:  newobj     instance void Sims3.Store.Objects.SaunaClassic/RefreshedStore/BuffInstanceRefreshedStore::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                           valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                           int32,
                                                                                                                           float32)
      IL_0018:  ret
    } // end of method RefreshedStore::CreateBuffInstance

  } // end of class RefreshedStore

  .class auto ansi nested public beforefieldinit BatheInWater
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/BatheInWater>
    {
      .class auto ansi nested public BathType
             extends [mscorlib]System.Enum
      {
        .field public specialname rtspecialname int32 value__
        .field public static literal valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Water = int32(0x00000000)
        .field public static literal valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Mud = int32(0x00000001)
      } // end of class BathType

      .class auto ansi nested public beforefieldinit '<>c__DisplayClass2'
             extends [mscorlib]System.Object
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a
        .method public hidebysig specialname rtspecialname 
                instance void  .ctor() cil managed
        {
          // Code size       7 (0x7)
          .maxstack  8
          IL_0000:  ldarg.0
          IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
          IL_0006:  ret
        } // end of method '<>c__DisplayClass2'::.ctor

        .method public hidebysig instance string 
                '<Test>b__0'() cil managed
        {
          // Code size       28 (0x1c)
          .maxstack  8
          IL_0000:  ldarg.0
          IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/'<>c__DisplayClass2'::a
          IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
          IL_000b:  ldstr      "InUse"
          IL_0010:  ldc.i4.0
          IL_0011:  newarr     [mscorlib]System.Object
          IL_0016:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
          IL_001b:  ret
        } // end of method '<>c__DisplayClass2'::'<Test>b__0'

      } // end of class '<>c__DisplayClass2'

      .field public valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType mBathType
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/BatheInWater>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType bathType) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/BatheInWater>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::mBathType
        IL_000d:  ret
      } // end of method Definition::.ctor

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.SaunaClassic target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       118 (0x76)
        .maxstack  4
        .locals init (class [UI]Sims3.SimIFace.GreyedOutTooltipCallback V_0,
                 class Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/'<>c__DisplayClass2' V_1)
        IL_0000:  ldnull
        IL_0001:  stloc.0
        IL_0002:  newobj     instance void Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/'<>c__DisplayClass2'::.ctor()
        IL_0007:  stloc.1
        IL_0008:  ldloc.1
        IL_0009:  ldarg.1
        IL_000a:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/'<>c__DisplayClass2'::a
        IL_000f:  ldarg.3
        IL_0010:  brfalse.s  IL_002a

        IL_0012:  ldloc.1
        IL_0013:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/'<>c__DisplayClass2'::a
        IL_0018:  ldc.i8     0x4c800171bd46a80
        IL_0021:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_0026:  brfalse.s  IL_002a

        IL_0028:  ldc.i4.0
        IL_0029:  ret

        IL_002a:  ldarg.3
        IL_002b:  brfalse.s  IL_0042

        IL_002d:  ldloc.1
        IL_002e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/'<>c__DisplayClass2'::a
        IL_0033:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_OccultManager()
        IL_0038:  ldc.i4.2
        IL_0039:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultManager::HasOccultType(valuetype [UI]Sims3.UI.Hud.OccultTypes)
        IL_003e:  brfalse.s  IL_0042

        IL_0040:  ldc.i4.0
        IL_0041:  ret

        IL_0042:  ldloc.1
        IL_0043:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/'<>c__DisplayClass2'::a
        IL_0048:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
        IL_004d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
        IL_0052:  brfalse.s  IL_0056

        IL_0054:  ldc.i4.0
        IL_0055:  ret

        IL_0056:  ldarg.2
        IL_0057:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::mSimInTub
        IL_005c:  brfalse.s  IL_0074

        IL_005e:  ldarg.s    greyedOutTooltipCallback
        IL_0060:  ldloc.0
        IL_0061:  brtrue.s   IL_0070

        IL_0063:  ldloc.1
        IL_0064:  ldftn      instance string Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/'<>c__DisplayClass2'::'<Test>b__0'()
        IL_006a:  newobj     instance void [UI]Sims3.SimIFace.GreyedOutTooltipCallback::.ctor(object,
                                                                                              native int)
        IL_006f:  stloc.0
        IL_0070:  ldloc.0
        IL_0071:  stind.ref
        IL_0072:  ldc.i4.0
        IL_0073:  ret

        IL_0074:  ldc.i4.1
        IL_0075:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.SaunaClassic target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       46 (0x2e)
        .maxstack  5
        .locals init (string V_0,
                 object[] V_1)
        IL_0000:  ldstr      "BatheInWater"
        IL_0005:  stloc.0
        IL_0006:  ldarg.0
        IL_0007:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::mBathType
        IL_000c:  ldc.i4.1
        IL_000d:  bne.un.s   IL_0015

        IL_000f:  ldstr      "BatheInMud"
        IL_0014:  stloc.0
        IL_0015:  ldarg.1
        IL_0016:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_001b:  ldloc.0
        IL_001c:  ldc.i4.1
        IL_001d:  newarr     [mscorlib]System.Object
        IL_0022:  stloc.1
        IL_0023:  ldloc.1
        IL_0024:  ldc.i4.0
        IL_0025:  ldarg.1
        IL_0026:  stelem.ref
        IL_0027:  ldloc.1
        IL_0028:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_002d:  ret
      } // end of method Definition::GetInteractionName

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition MudSingleton
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition WaterSingleton
    .field public valuetype [SimIFace]Sims3.SimIFace.ObjectGuid mDuckyGuid
    .method public hidebysig virtual instance void 
            ConfigureInteraction() cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.TimedStage V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage[] V_1)
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetInteractionName()
      IL_0006:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kBathLengthInMinutes
      IL_000b:  ldc.i4.0
      IL_000c:  ldc.i4.1
      IL_000d:  ldc.i4.1
      IL_000e:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.TimedStage::.ctor(string,
                                                                                                             float32,
                                                                                                             bool,
                                                                                                             bool,
                                                                                                             bool)
      IL_0013:  stloc.0
      IL_0014:  ldarg.0
      IL_0015:  ldc.i4.1
      IL_0016:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage
      IL_001b:  stloc.1
      IL_001c:  ldloc.1
      IL_001d:  ldc.i4.0
      IL_001e:  ldloc.0
      IL_001f:  stelem.ref
      IL_0020:  ldloc.1
      IL_0021:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0026:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::set_Stages(class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage>)
      IL_002b:  ret
    } // end of method BatheInWater::ConfigureInteraction

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       883 (0x373)
      .maxstack  6
      .locals init (valuetype [SimIFace]Sims3.SimIFace.Slot[] V_0,
               int32 V_1,
               class Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition V_2,
               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.AgingState V_4,
               bool V_5,
               valuetype [SimIFace]Sims3.SimIFace.Slot[] V_6,
               object[] V_7)
      IL_0000:  ldc.i4.2
      IL_0001:  newarr     [SimIFace]Sims3.SimIFace.Slot
      IL_0006:  stloc.s    V_6
      IL_0008:  ldloc.s    V_6
      IL_000a:  ldc.i4.0
      IL_000b:  ldc.i4     0x31229a48
      IL_0010:  stelem.i4
      IL_0011:  ldloc.s    V_6
      IL_0013:  ldc.i4.1
      IL_0014:  ldc.i4     0x31229a4a
      IL_0019:  stelem.i4
      IL_001a:  ldloc.s    V_6
      IL_001c:  stloc.0
      IL_001d:  ldarg.0
      IL_001e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0023:  ldarg.0
      IL_0024:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0029:  ldloc.0
      IL_002a:  ldloca.s   V_1
      IL_002c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlotList(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                          valuetype [SimIFace]Sims3.SimIFace.Slot[],
                                                                                                          int32&)
      IL_0031:  brtrue.s   IL_0035

      IL_0033:  ldc.i4.0
      IL_0034:  ret

      IL_0035:  ldarg.0
      IL_0036:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_003b:  isinst     Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition
      IL_0040:  stloc.2
      IL_0041:  ldloc.2
      IL_0042:  brtrue.s   IL_0046

      IL_0044:  ldc.i4.0
      IL_0045:  ret

      IL_0046:  ldarg.0
      IL_0047:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_004c:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::mSimInTub
      IL_0051:  brfalse.s  IL_0055

      IL_0053:  ldc.i4.0
      IL_0054:  ret

      IL_0055:  ldarg.0
      IL_0056:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_005b:  ldarg.0
      IL_005c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0061:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::mSimInTub
      IL_0066:  ldarg.0
      IL_0067:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry()
      IL_006c:  ldarg.0
      IL_006d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0072:  ldc.i8     0x4c800171bd46a80
      IL_007b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_0080:  brtrue.s   IL_0095

      IL_0082:  ldarg.0
      IL_0083:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0088:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_OccultManager()
      IL_008d:  ldc.i4.2
      IL_008e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultManager::HasOccultType(valuetype [UI]Sims3.UI.Hud.OccultTypes)
      IL_0093:  brfalse.s  IL_00ae

      IL_0095:  ldarg.0
      IL_0096:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_009b:  ldc.i4     0x8f
      IL_00a0:  ldarg.0
      IL_00a1:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_00a6:  ldc.i4.3
      IL_00a7:  ldc.i4.2
      IL_00a8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ThoughtBalloons.ThoughtBalloonAxis,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_00ad:  pop
      IL_00ae:  ldarg.0
      IL_00af:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_00b4:  ldc.i4.3
      IL_00b5:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SwitchToOutfitWithSpin(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason)
      IL_00ba:  ldarg.0
      IL_00bb:  ldstr      "Sauna_store"
      IL_00c0:  ldstr      "SimEnter"
      IL_00c5:  ldstr      "x"
      IL_00ca:  ldstr      "saunaX"
      IL_00cf:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_00d4:  ldloc.2
      IL_00d5:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::mBathType
      IL_00da:  ldc.i4.1
      IL_00db:  bne.un.s   IL_00fb

      IL_00dd:  ldarg.0
      IL_00de:  ldstr      "Mudbath"
      IL_00e3:  ldc.i4.1
      IL_00e4:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             bool)
      IL_00e9:  ldarg.0
      IL_00ea:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_00ef:  ldstr      "mud"
      IL_00f4:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetGeometryState(string)
      IL_00f9:  br.s       IL_0117

      IL_00fb:  ldarg.0
      IL_00fc:  ldstr      "Mudbath"
      IL_0101:  ldc.i4.0
      IL_0102:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             bool)
      IL_0107:  ldarg.0
      IL_0108:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_010d:  ldstr      "water"
      IL_0112:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetGeometryState(string)
      IL_0117:  ldloc.1
      IL_0118:  ldc.i4.1
      IL_0119:  bne.un.s   IL_0129

      IL_011b:  ldarg.0
      IL_011c:  ldstr      "IsMirrored"
      IL_0121:  ldc.i4.1
      IL_0122:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             bool)
      IL_0127:  br.s       IL_0135

      IL_0129:  ldarg.0
      IL_012a:  ldstr      "IsMirrored"
      IL_012f:  ldc.i4.0
      IL_0130:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             bool)
      IL_0135:  ldarg.0
      IL_0136:  ldstr      "RubberDucky"
      IL_013b:  ldc.i4.1
      IL_013c:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::OutOfWorld
      IL_0141:  ldc.i4.0
      IL_0142:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::UnitZ
      IL_0147:  call       valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.GlobalFunctions::CreateProp(string,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.ProductVersion,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                                          int32,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_014c:  stfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/BatheInWater::mDuckyGuid
      IL_0151:  ldarg.0
      IL_0152:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/BatheInWater::mDuckyGuid
      IL_0157:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetObject(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_015c:  stloc.3
      IL_015d:  ldloc.3
      IL_015e:  brfalse.s  IL_0178

      IL_0160:  ldloc.3
      IL_0161:  ldarg.0
      IL_0162:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0167:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_016c:  ldarg.0
      IL_016d:  ldstr      "ducky"
      IL_0172:  ldloc.3
      IL_0173:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0178:  ldarg.0
      IL_0179:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_017e:  ldarg.0
      IL_017f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0184:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RegisterGroupTalk()
      IL_0189:  ldarg.0
      IL_018a:  ldstr      "TakeBath"
      IL_018f:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0194:  ldarg.0
      IL_0195:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StartStages()
      IL_019a:  ldarg.0
      IL_019b:  ldc.i4     0xffa9bfff
      IL_01a0:  ldarg.0
      IL_01a1:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/BatheInWater::LoopUpdate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_01a7:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_01ac:  ldarg.0
      IL_01ad:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_01b2:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                     class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_01b7:  pop
      IL_01b8:  ldarg.0
      IL_01b9:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_01be:  ldc.i8     0x4c800171bd46a80
      IL_01c7:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_01cc:  brfalse.s  IL_01e4

      IL_01ce:  ldarg.0
      IL_01cf:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_01d4:  ldc.i4.s   9
      IL_01d6:  ldarg.0
      IL_01d7:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_01dc:  ldc.i4.3
      IL_01dd:  ldc.i4.4
      IL_01de:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ThoughtBalloons.ThoughtBalloonAxis,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_01e3:  pop
      IL_01e4:  ldarg.0
      IL_01e5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_01ea:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::UnregisterGroupTalk()
      IL_01ef:  ldarg.0
      IL_01f0:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_01f5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_01fa:  ldc.i8     0xb82d0015b9294950
      IL_0203:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0208:  ldarg.0
      IL_0209:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_020e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0213:  ldc.i8     0x4c800171bd46b80
      IL_021c:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0221:  ldarg.0
      IL_0222:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0227:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_022c:  ldc.i8     0x676900408fffc030
      IL_0235:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_023a:  ldloc.2
      IL_023b:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::mBathType
      IL_0240:  ldc.i4.1
      IL_0241:  bne.un     IL_02cf

      IL_0246:  ldarg.0
      IL_0247:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_024c:  ldc.i4     0x2000
      IL_0251:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0256:  brfalse.s  IL_02cf

      IL_0258:  ldarg.0
      IL_0259:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_025e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_0263:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.AgingState [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::AgingState
      IL_0268:  stloc.s    V_4
      IL_026a:  ldloc.s    V_4
      IL_026c:  brfalse.s  IL_02cf

      IL_026e:  ldloc.s    V_4
      IL_0270:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kMudBathAgeExtension
      IL_0275:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.AgingState::ExtendAgingState(float32)
      IL_027a:  ldarg.0
      IL_027b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0280:  ldarg.0
      IL_0281:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0286:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_028b:  ldstr      "FeelingYounger"
      IL_0290:  ldc.i4.1
      IL_0291:  newarr     [mscorlib]System.Object
      IL_0296:  stloc.s    V_7
      IL_0298:  ldloc.s    V_7
      IL_029a:  ldc.i4.0
      IL_029b:  ldarg.0
      IL_029c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_02a1:  stelem.ref
      IL_02a2:  ldloc.s    V_7
      IL_02a4:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                   string,
                                                                                   object[])
      IL_02a9:  ldc.i4.3
      IL_02aa:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                               valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle)
      IL_02af:  pop
      IL_02b0:  call       class [Sims3GameplaySystems]Sims3.Gameplay.UI.Responder [Sims3GameplaySystems]Sims3.Gameplay.UI.Responder::get_Instance()
      IL_02b5:  callvirt   instance class [UI]Sims3.UI.Hud.IHudModel [Sims3GameplaySystems]Sims3.Gameplay.UI.Responder::get_HudModel()
      IL_02ba:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.UI.HudModel
      IL_02bf:  ldarg.0
      IL_02c0:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_02c5:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
      IL_02ca:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.UI.HudModel::OnSimAgeChanged(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_02cf:  ldloc.2
      IL_02d0:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::mBathType
      IL_02d5:  brtrue.s   IL_030c

      IL_02d7:  ldarg.0
      IL_02d8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_02dd:  ldc.i4     0x2000
      IL_02e2:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_02e7:  brfalse.s  IL_030c

      IL_02e9:  ldarg.0
      IL_02ea:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_02ef:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_02f4:  ldc.i8     0xb82d0015b9294740
      IL_02fd:  ldc.i8     0x8bfc002118898608
      IL_0306:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_030b:  pop
      IL_030c:  ldc.i4.1
      IL_030d:  stloc.s    V_5
      IL_030f:  ldarg.0
      IL_0310:  ldloc.s    V_5
      IL_0312:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_0317:  ldarg.0
      IL_0318:  ldstr      "SimExit"
      IL_031d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0322:  ldarg.0
      IL_0323:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0328:  ldc.i4.4
      IL_0329:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SwitchToOutfitWithSpin(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason)
      IL_032e:  ldarg.0
      IL_032f:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0334:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::mSimInTub
      IL_0339:  ldarg.0
      IL_033a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_033f:  bne.un.s   IL_034d

      IL_0341:  ldarg.0
      IL_0342:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0347:  ldnull
      IL_0348:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::mSimInTub
      IL_034d:  ldarg.0
      IL_034e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit()
      IL_0353:  ldarg.0
      IL_0354:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0359:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_035e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsFrankenstein()
      IL_0363:  brfalse.s  IL_0370

      IL_0365:  ldarg.0
      IL_0366:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_036b:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultFrankenstein::PushFrankensteinShortOut(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0370:  ldloc.s    V_5
      IL_0372:  ret
    } // end of method BatheInWater::Run

    .method public hidebysig instance void 
            LoopUpdate(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData loopData) cil managed
    {
      // Code size       431 (0x1af)
      .maxstack  7
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Socializing.Relationship V_1,
               class Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition V_2,
               valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> V_3)
      IL_0000:  ldc.i4.1
      IL_0001:  ldarg.0
      IL_0002:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0007:  ldarg.0
      IL_0008:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_000d:  call       class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTracker::SendEvent(valuetype [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTypeId,
                                                                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_0012:  pop
      IL_0013:  ldarg.0
      IL_0014:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0019:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_001e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsFrankenstein()
      IL_0023:  brfalse.s  IL_0036

      IL_0025:  ldarg.0
      IL_0026:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_002b:  ldc.i4     0x200
      IL_0030:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0035:  ret

      IL_0036:  ldarg.0
      IL_0037:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_003c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_HasGroupTalk()
      IL_0041:  brfalse    IL_0102

      IL_0046:  ldarg.0
      IL_0047:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_004c:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_GroupTalkMembers()
      IL_0051:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Count()
      IL_0056:  ldc.i4.1
      IL_0057:  ble        IL_0102

      IL_005c:  ldarg.0
      IL_005d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0062:  ldnull
      IL_0063:  ldnull
      IL_0064:  ldc.i4.1
      IL_0065:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::DoGroupTalk(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/GroupTalkDelegate,
                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/GroupTalkDelegate,
                                                                                                      bool)
      IL_006a:  brfalse    IL_0102

      IL_006f:  ldarg.0
      IL_0070:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0075:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
      IL_007a:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::GetEnumerator()
      IL_007f:  stloc.3
      .try
      {
        IL_0080:  br.s       IL_00e9

        IL_0082:  ldloca.s   V_3
        IL_0084:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Current()
        IL_0089:  stloc.0
        IL_008a:  ldloc.0
        IL_008b:  ldarg.0
        IL_008c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
        IL_0091:  beq.s      IL_00c2

        IL_0093:  ldarg.0
        IL_0094:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
        IL_0099:  ldloc.0
        IL_009a:  ldc.i4.1
        IL_009b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Socializing.Relationship [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetRelationship(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                           bool)
        IL_00a0:  stloc.1
        IL_00a1:  ldloc.1
        IL_00a2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Socializing.LongTermRelationship [Sims3GameplaySystems]Sims3.Gameplay.Socializing.Relationship::get_LTR()
        IL_00a7:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kGroupChatRelationshipBump
        IL_00ac:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.LongTermRelationship::UpdateLiking(float32)
        IL_00b1:  ldc.i4.5
        IL_00b2:  ldarg.0
        IL_00b3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
        IL_00b8:  ldloc.0
        IL_00b9:  ldc.i4.1
        IL_00ba:  ldc.i4.0
        IL_00bb:  ldc.i4.0
        IL_00bc:  ldc.i4.0
        IL_00bd:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.SocialComponent::SetSocialFeedbackForActorAndTarget(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Socializing.CommodityTypes,
                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                       bool,
                                                                                                                                       int32,
                                                                                                                                       valuetype [UI]Sims3.UI.Controller.LongTermRelationshipTypes,
                                                                                                                                       valuetype [UI]Sims3.UI.Controller.LongTermRelationshipTypes)
        IL_00c2:  ldloc.0
        IL_00c3:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_00c8:  ldc.i4     0x10f38e0
        IL_00cd:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
        IL_00d2:  brfalse.s  IL_00e9

        IL_00d4:  ldloc.0
        IL_00d5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_00da:  ldc.i4     0x10f38e0
        IL_00df:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kGroupChatSocialBump
        IL_00e4:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              float32)
        IL_00e9:  ldloca.s   V_3
        IL_00eb:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::MoveNext()
        IL_00f0:  brtrue.s   IL_0082

        IL_00f2:  leave.s    IL_0102

      }  // end .try
      finally
      {
        IL_00f4:  ldloca.s   V_3
        IL_00f6:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>
        IL_00fc:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0101:  endfinally
      }  // end handler
      IL_0102:  ldarg.0
      IL_0103:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0108:  isinst     Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition
      IL_010d:  stloc.2
      IL_010e:  ldloc.2
      IL_010f:  brfalse    IL_01ae

      IL_0114:  ldarga.s   loopData
      IL_0116:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData::mLifeTime
      IL_011b:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kBathDurationToRemoveFatigue
      IL_0120:  ble.un     IL_01ae

      IL_0125:  ldarg.0
      IL_0126:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_012b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0130:  ldc.i8     0xb82d0015b9294790
      IL_0139:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_013e:  ldarg.0
      IL_013f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0144:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0149:  ldc.i8     0xb82d0015b9294980
      IL_0152:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0157:  ldloc.2
      IL_0158:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::mBathType
      IL_015d:  brtrue.s   IL_0182

      IL_015f:  ldarg.0
      IL_0160:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0165:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_016a:  ldc.i8     0x9a7f5f1919df0013
      IL_0173:  ldc.i8     0x8bfc002118898608
      IL_017c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElementPaused(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                         valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_0181:  pop
      IL_0182:  ldloc.2
      IL_0183:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::mBathType
      IL_0188:  ldc.i4.1
      IL_0189:  bne.un.s   IL_01ae

      IL_018b:  ldarg.0
      IL_018c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0191:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0196:  ldc.i8     0x9a7f5f1919df0014
      IL_019f:  ldc.i8     0x8bfc002118898608
      IL_01a8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElementPaused(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                         valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_01ad:  pop
      IL_01ae:  ret
    } // end of method BatheInWater::LoopUpdate

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       101 (0x65)
      .maxstack  3
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/BatheInWater::mDuckyGuid
      IL_0006:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetObject(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  brfalse.s  IL_002c

      IL_000f:  ldloc.0
      IL_0010:  ldc.r4     0.0
      IL_0015:  ldc.r4     0.0
      IL_001a:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetOpacity(float32,
                                                                                                               float32)
      IL_001f:  ldloc.0
      IL_0020:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::UnParent()
      IL_0025:  pop
      IL_0026:  ldloc.0
      IL_0027:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::Destroy()
      IL_002c:  ldarg.0
      IL_002d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0032:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0037:  ldc.i8     0x9a7f5f1919df0013
      IL_0040:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::UnpauseBuff(uint64)
      IL_0045:  ldarg.0
      IL_0046:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_004b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0050:  ldc.i8     0x9a7f5f1919df0014
      IL_0059:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::UnpauseBuff(uint64)
      IL_005e:  ldarg.0
      IL_005f:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_0064:  ret
    } // end of method BatheInWater::Cleanup

    .method public hidebysig virtual instance void 
            AddExcludedDreams(class [mscorlib]System.Collections.Generic.ICollection`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.DreamsAndPromises.DreamNames> excludedDreams) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddExcludedDreams(class [mscorlib]System.Collections.Generic.ICollection`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.DreamsAndPromises.DreamNames>)
      IL_0007:  ldarg.0
      IL_0008:  ldc.i4     0xd4070a7
      IL_000d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddExcludedDream(valuetype [Sims3GameplaySystems]Sims3.Gameplay.DreamsAndPromises.DreamNames)
      IL_0012:  ret
    } // end of method BatheInWater::AddExcludedDreams

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::.ctor()
      IL_0006:  ret
    } // end of method BatheInWater::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  newobj     instance void Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::.ctor(valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType)
      IL_0006:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/BatheInWater::MudSingleton
      IL_000b:  ldc.i4.0
      IL_000c:  newobj     instance void Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition::.ctor(valuetype Sims3.Store.Objects.SaunaClassic/BatheInWater/Definition/BathType)
      IL_0011:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/BatheInWater::WaterSingleton
      IL_0016:  ret
    } // end of method BatheInWater::.cctor

  } // end of class BatheInWater

  .class auto ansi nested public beforefieldinit SteamVeggies
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/SteamVeggies>
    {
      .class auto ansi nested public beforefieldinit '<>c__DisplayClass6'
             extends [mscorlib]System.Object
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a
        .method public hidebysig specialname rtspecialname 
                instance void  .ctor() cil managed
        {
          // Code size       7 (0x7)
          .maxstack  8
          IL_0000:  ldarg.0
          IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
          IL_0006:  ret
        } // end of method '<>c__DisplayClass6'::.ctor

        .method public hidebysig instance string 
                '<Test>b__4'() cil managed
        {
          // Code size       39 (0x27)
          .maxstack  5
          .locals init (object[] V_0)
          IL_0000:  ldarg.0
          IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/SteamVeggies/Definition/'<>c__DisplayClass6'::a
          IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
          IL_000b:  ldstr      "NoVeggiesInInventory"
          IL_0010:  ldc.i4.1
          IL_0011:  newarr     [mscorlib]System.Object
          IL_0016:  stloc.0
          IL_0017:  ldloc.0
          IL_0018:  ldc.i4.0
          IL_0019:  ldarg.0
          IL_001a:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/SteamVeggies/Definition/'<>c__DisplayClass6'::a
          IL_001f:  stelem.ref
          IL_0020:  ldloc.0
          IL_0021:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
          IL_0026:  ret
        } // end of method '<>c__DisplayClass6'::'<Test>b__4'

      } // end of class '<>c__DisplayClass6'

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.SaunaClassic target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       52 (0x34)
        .maxstack  4
        .locals init (class [UI]Sims3.SimIFace.GreyedOutTooltipCallback V_0,
                 class Sims3.Store.Objects.SaunaClassic/SteamVeggies/Definition/'<>c__DisplayClass6' V_1)
        IL_0000:  ldnull
        IL_0001:  stloc.0
        IL_0002:  newobj     instance void Sims3.Store.Objects.SaunaClassic/SteamVeggies/Definition/'<>c__DisplayClass6'::.ctor()
        IL_0007:  stloc.1
        IL_0008:  ldloc.1
        IL_0009:  ldarg.1
        IL_000a:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/SteamVeggies/Definition/'<>c__DisplayClass6'::a
        IL_000f:  ldloc.1
        IL_0010:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/SteamVeggies/Definition/'<>c__DisplayClass6'::a
        IL_0015:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient Sims3.Store.Objects.SaunaClassic/SteamVeggies::GetCheapestVeggies(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_001a:  brtrue.s   IL_0032

        IL_001c:  ldarg.s    greyedOutTooltipCallback
        IL_001e:  ldloc.0
        IL_001f:  brtrue.s   IL_002e

        IL_0021:  ldloc.1
        IL_0022:  ldftn      instance string Sims3.Store.Objects.SaunaClassic/SteamVeggies/Definition/'<>c__DisplayClass6'::'<Test>b__4'()
        IL_0028:  newobj     instance void [UI]Sims3.SimIFace.GreyedOutTooltipCallback::.ctor(object,
                                                                                              native int)
        IL_002d:  stloc.0
        IL_002e:  ldloc.0
        IL_002f:  stind.ref
        IL_0030:  ldc.i4.0
        IL_0031:  ret

        IL_0032:  ldc.i4.1
        IL_0033:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.SaunaClassic target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       29 (0x1d)
        .maxstack  5
        .locals init (object[] V_0)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0006:  ldstr      "SteamVeggies"
        IL_000b:  ldc.i4.1
        IL_000c:  newarr     [mscorlib]System.Object
        IL_0011:  stloc.0
        IL_0012:  ldloc.0
        IL_0013:  ldc.i4.0
        IL_0014:  ldarg.1
        IL_0015:  stelem.ref
        IL_0016:  ldloc.0
        IL_0017:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_001c:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/SteamVeggies>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static literal int32 mVeggieSteamEventTrigger = int32(0x00000386)
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .field public valuetype [SimIFace]Sims3.SimIFace.ObjectGuid mPanGuid
    .field public valuetype [SimIFace]Sims3.SimIFace.ObjectGuid mVeggiesGuid
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       551 (0x227)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe V_0,
               bool V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient V_2,
               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject V_4,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IFoodContainer V_5,
               class [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.ServingContainer V_6,
               bool V_7,
               class [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.ServingContainer V_8)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_000c:  ldc.i4     0xa678e703
      IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlot(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                      valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_0016:  brtrue.s   IL_001a

      IL_0018:  ldc.i4.0
      IL_0019:  ret

      IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe> [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe::NameToRecipeHash
      IL_001f:  ldarg.0
      IL_0020:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0025:  ldfld      string Sims3.Store.Objects.SaunaClassic::mRecipeName
      IL_002a:  ldloca.s   V_0
      IL_002c:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe>::TryGetValue(!0,
                                                                                                                                                                                            !1&)
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brfalse.s  IL_0038

      IL_0035:  ldloc.0
      IL_0036:  brtrue.s   IL_003a

      IL_0038:  ldc.i4.0
      IL_0039:  ret

      IL_003a:  ldarg.0
      IL_003b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0040:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient Sims3.Store.Objects.SaunaClassic/SteamVeggies::GetCheapestVeggies(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0045:  stloc.2
      IL_0046:  ldloc.2
      IL_0047:  brtrue.s   IL_004b

      IL_0049:  ldc.i4.0
      IL_004a:  ret

      IL_004b:  ldarg.0
      IL_004c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0051:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0056:  ldloc.2
      IL_0057:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToRemove(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_005c:  pop
      IL_005d:  ldarg.0
      IL_005e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry()
      IL_0063:  ldarg.0
      IL_0064:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0069:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
      IL_006e:  ldc.i4.5
      IL_006f:  conv.i8
      IL_0070:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
      IL_0075:  pop
      IL_0076:  ldarg.0
      IL_0077:  ldstr      "Sauna_store"
      IL_007c:  ldstr      "SimEnter"
      IL_0081:  ldstr      "x"
      IL_0086:  ldstr      "saunaX"
      IL_008b:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_0090:  ldarg.0
      IL_0091:  ldstr      "FryingPan"
      IL_0096:  ldc.i4.1
      IL_0097:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::OutOfWorld
      IL_009c:  ldc.i4.0
      IL_009d:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::UnitZ
      IL_00a2:  call       valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.GlobalFunctions::CreateProp(string,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.ProductVersion,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                                          int32,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_00a7:  stfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mPanGuid
      IL_00ac:  ldarg.0
      IL_00ad:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mPanGuid
      IL_00b2:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetObject(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_00b7:  stloc.3
      IL_00b8:  ldloc.3
      IL_00b9:  brfalse.s  IL_00c7

      IL_00bb:  ldloc.3
      IL_00bc:  ldarg.0
      IL_00bd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_00c2:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_00c7:  ldarg.0
      IL_00c8:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_00cd:  ldstr      "FryingPan"
      IL_00d2:  ldarg.0
      IL_00d3:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mPanGuid
      IL_00d8:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetPropActor(string,
                                                                                                   valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_00dd:  ldarg.0
      IL_00de:  ldstr      "foodSteamedVeggies"
      IL_00e3:  ldc.i4.m1
      IL_00e4:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::OutOfWorld
      IL_00e9:  ldc.i4.0
      IL_00ea:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::UnitZ
      IL_00ef:  call       valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.GlobalFunctions::CreateProp(string,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.ProductVersion,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                                          int32,
                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_00f4:  stfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mVeggiesGuid
      IL_00f9:  ldarg.0
      IL_00fa:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mVeggiesGuid
      IL_00ff:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetObject(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_0104:  stloc.s    V_4
      IL_0106:  ldloc.s    V_4
      IL_0108:  brfalse.s  IL_0117

      IL_010a:  ldloc.s    V_4
      IL_010c:  ldarg.0
      IL_010d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0112:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0117:  ldarg.0
      IL_0118:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_011d:  ldstr      "SaunaVeggies"
      IL_0122:  ldarg.0
      IL_0123:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mVeggiesGuid
      IL_0128:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetPropActor(string,
                                                                                                   valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_012d:  ldnull
      IL_012e:  stloc.s    V_5
      IL_0130:  ldloc.0
      IL_0131:  ldc.i4.1
      IL_0132:  ldc.i4.6
      IL_0133:  ldarg.0
      IL_0134:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0139:  ldc.i4.0
      IL_013a:  ldc.i4.1
      IL_013b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IFoodContainer [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe::CreateFinishedFood(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe/MealQuantity,
                                                                                                                                                                                                      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Objects.Quality,
                                                                                                                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                                      int32,
                                                                                                                                                                                                      bool)
      IL_0140:  stloc.s    V_5
      IL_0142:  ldloc.s    V_5
      IL_0144:  brfalse.s  IL_0176

      IL_0146:  ldloc.s    V_5
      IL_0148:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.ServingContainer
      IL_014d:  stloc.s    V_6
      IL_014f:  ldloc.s    V_6
      IL_0151:  brfalse.s  IL_0176

      IL_0153:  ldloc.s    V_6
      IL_0155:  callvirt   instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.FoodProp [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.ServingContainer::GetContainedFood()
      IL_015a:  ldc.r4     0.0
      IL_015f:  ldc.r4     0.0
      IL_0164:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetOpacity(float32,
                                                                                                               float32)
      IL_0169:  ldarg.0
      IL_016a:  ldstr      "plate"
      IL_016f:  ldloc.s    V_5
      IL_0171:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0176:  ldarg.0
      IL_0177:  ldc.i4     0x386
      IL_017c:  ldarg.0
      IL_017d:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/SteamVeggies::OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                         class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0183:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0188:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddOneShotScriptEventHandler(uint32,
                                                                                                                                             class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_018d:  ldarg.0
      IL_018e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_0193:  ldarg.0
      IL_0194:  ldstr      "CookVeggies"
      IL_0199:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_019e:  ldc.i4.1
      IL_019f:  stloc.s    V_7
      IL_01a1:  ldarg.0
      IL_01a2:  ldloc.s    V_7
      IL_01a4:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_01a9:  ldarg.0
      IL_01aa:  ldstr      "SimExit"
      IL_01af:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_01b4:  ldloc.s    V_5
      IL_01b6:  brfalse.s  IL_021e

      IL_01b8:  ldloc.s    V_5
      IL_01ba:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.ServingContainer
      IL_01bf:  stloc.s    V_8
      IL_01c1:  ldloc.s    V_8
      IL_01c3:  brfalse.s  IL_01e2

      IL_01c5:  ldloc.s    V_8
      IL_01c7:  callvirt   instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.ServingContainer::StartSmokeSteamVFX()
      IL_01cc:  ldloc.s    V_8
      IL_01ce:  callvirt   instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.FoodProp [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.ServingContainer::GetContainedFood()
      IL_01d3:  ldc.r4     1.
      IL_01d8:  ldc.r4     0.0
      IL_01dd:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetOpacity(float32,
                                                                                                               float32)
      IL_01e2:  ldloc.s    V_5
      IL_01e4:  ldc.r4     1.
      IL_01e9:  ldc.r4     0.0
      IL_01ee:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::SetOpacity(float32,
                                                                                                                 float32)
      IL_01f3:  ldarg.0
      IL_01f4:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_01f9:  ldloc.s    V_5
      IL_01fb:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable
      IL_0200:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::EnterWhileHolding(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable)
      IL_0205:  ldarg.0
      IL_0206:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_020b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0210:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.CookingObjects.EatHeldFood::Singleton
      IL_0215:  ldloc.s    V_5
      IL_0217:  ldc.i4.0
      IL_0218:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                       bool)
      IL_021d:  pop
      IL_021e:  ldarg.0
      IL_021f:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit()
      IL_0224:  ldloc.s    V_7
      IL_0226:  ret
    } // end of method SteamVeggies::Run

    .method public hidebysig instance void 
            OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                             class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       41 (0x29)
      .maxstack  2
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject V_0)
      IL_0000:  ldarg.2
      IL_0001:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.IEvent::get_EventId()
      IL_0006:  ldc.i4     0x386
      IL_000b:  bne.un.s   IL_0028

      IL_000d:  ldarg.0
      IL_000e:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mPanGuid
      IL_0013:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetObject(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_0018:  stloc.0
      IL_0019:  ldloc.0
      IL_001a:  brfalse.s  IL_0028

      IL_001c:  ldarg.0
      IL_001d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0022:  ldloc.0
      IL_0023:  callvirt   instance void Sims3.Store.Objects.SaunaClassic::StartVeggieSteamFX(class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0028:  ret
    } // end of method SteamVeggies::OnAnimationEvent

    .method public hidebysig static class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient 
            GetCheapestVeggies(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor) cil managed
    {
      // Code size       121 (0x79)
      .maxstack  2
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient V_0,
               class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient> V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient V_2,
               valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient> V_3)
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0008:  ldc.i4.0
      IL_0009:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<!!0> [Sims3GameplaySystems]Sims3.Gameplay.Inventory::FindAll<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient>(bool)
      IL_000e:  stloc.1
      IL_000f:  ldloc.1
      IL_0010:  brfalse.s  IL_0077

      IL_0012:  ldloc.1
      IL_0013:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0077

      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient>::GetEnumerator()
      IL_0021:  stloc.3
      .try
      {
        IL_0022:  br.s       IL_005e

        IL_0024:  ldloca.s   V_3
        IL_0026:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient>::get_Current()
        IL_002b:  stloc.2
        IL_002c:  ldloc.2
        IL_002d:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient::Data
        IL_0032:  brfalse.s  IL_005e

        IL_0034:  ldloc.2
        IL_0035:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient::Data
        IL_003a:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData::Key
        IL_003f:  ldstr      "Vegetable"
        IL_0044:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.ItemTypes::IsOfType(string,
                                                                                                     string)
        IL_0049:  brfalse.s  IL_005e

        IL_004b:  ldloc.0
        IL_004c:  brfalse.s  IL_005c

        IL_004e:  ldloc.2
        IL_004f:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Cost()
        IL_0054:  ldloc.0
        IL_0055:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Cost()
        IL_005a:  bge.s      IL_005e

        IL_005c:  ldloc.2
        IL_005d:  stloc.0
        IL_005e:  ldloca.s   V_3
        IL_0060:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient>::MoveNext()
        IL_0065:  brtrue.s   IL_0024

        IL_0067:  leave.s    IL_0077

      }  // end .try
      finally
      {
        IL_0069:  ldloca.s   V_3
        IL_006b:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient>
        IL_0071:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0076:  endfinally
      }  // end handler
      IL_0077:  ldloc.0
      IL_0078:  ret
    } // end of method SteamVeggies::GetCheapestVeggies

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       106 (0x6a)
      .maxstack  3
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mPanGuid
      IL_0006:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetObject(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  brfalse.s  IL_002c

      IL_000f:  ldloc.0
      IL_0010:  ldc.r4     0.0
      IL_0015:  ldc.r4     0.0
      IL_001a:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetOpacity(float32,
                                                                                                               float32)
      IL_001f:  ldloc.0
      IL_0020:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::UnParent()
      IL_0025:  pop
      IL_0026:  ldloc.0
      IL_0027:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::Destroy()
      IL_002c:  ldarg.0
      IL_002d:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ObjectGuid Sims3.Store.Objects.SaunaClassic/SteamVeggies::mVeggiesGuid
      IL_0032:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetObject(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_0037:  stloc.1
      IL_0038:  ldloc.1
      IL_0039:  brfalse.s  IL_0058

      IL_003b:  ldloc.1
      IL_003c:  ldc.r4     0.0
      IL_0041:  ldc.r4     0.0
      IL_0046:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetOpacity(float32,
                                                                                                               float32)
      IL_004b:  ldloc.1
      IL_004c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::UnParent()
      IL_0051:  pop
      IL_0052:  ldloc.1
      IL_0053:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::Destroy()
      IL_0058:  ldarg.0
      IL_0059:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_005e:  callvirt   instance void Sims3.Store.Objects.SaunaClassic::StopVeggieSteamFX()
      IL_0063:  ldarg.0
      IL_0064:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_0069:  ret
    } // end of method SteamVeggies::Cleanup

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::.ctor()
      IL_0006:  ret
    } // end of method SteamVeggies::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.SaunaClassic/SteamVeggies/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/SteamVeggies::Singleton
      IL_000a:  ret
    } // end of method SteamVeggies::.cctor

  } // end of class SteamVeggies

  .class auto ansi nested public beforefieldinit MakeSteam
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/MakeSteam>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.SaunaClassic target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
        IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
        IL_000b:  brfalse.s  IL_000f

        IL_000d:  ldc.i4.0
        IL_000e:  ret

        IL_000f:  ldc.i4.1
        IL_0010:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.SaunaClassic target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       29 (0x1d)
        .maxstack  5
        .locals init (object[] V_0)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0006:  ldstr      "MakeSteam"
        IL_000b:  ldc.i4.1
        IL_000c:  newarr     [mscorlib]System.Object
        IL_0011:  stloc.0
        IL_0012:  ldloc.0
        IL_0013:  ldc.i4.0
        IL_0014:  ldarg.1
        IL_0015:  stelem.ref
        IL_0016:  ldloc.0
        IL_0017:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_001c:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/MakeSteam>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static literal int32 mSteamyStoneTrigger = int32(0x00000384)
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       691 (0x2b3)
      .maxstack  7
      .locals init (bool V_0,
               class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_2,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_3,
               bool V_4,
               float32 V_5,
               float32 V_6,
               float32 V_7,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.DateAndTime V_8,
               class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_9,
               class [Sims3GameplaySystems]Sims3.Gameplay.Socializing.Relationship V_10,
               valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> V_11,
               valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> V_12)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_000c:  ldc.i4     0xa678e703
      IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlot(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                      valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_0016:  brtrue.s   IL_001a

      IL_0018:  ldc.i4.0
      IL_0019:  ret

      IL_001a:  ldarg.0
      IL_001b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0020:  ldc.i8     0x4c800171bd46ad0
      IL_0029:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_002e:  brtrue.s   IL_003c

      IL_0030:  ldarg.0
      IL_0031:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0036:  ldc.i4.3
      IL_0037:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SwitchToOutfitWithSpin(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason)
      IL_003c:  ldarg.0
      IL_003d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry()
      IL_0042:  ldarg.0
      IL_0043:  ldstr      "Sauna_store"
      IL_0048:  ldstr      "SimEnter"
      IL_004d:  ldstr      "x"
      IL_0052:  ldstr      "saunaX"
      IL_0057:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_005c:  ldarg.0
      IL_005d:  ldc.i4     0x384
      IL_0062:  ldarg.0
      IL_0063:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/MakeSteam::OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                      class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0069:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_006e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddOneShotScriptEventHandler(uint32,
                                                                                                                                             class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0073:  ldarg.0
      IL_0074:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_0079:  ldarg.0
      IL_007a:  ldstr      "PourWater"
      IL_007f:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0084:  ldc.i4.1
      IL_0085:  stloc.0
      IL_0086:  ldarg.0
      IL_0087:  ldloc.0
      IL_0088:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_008d:  ldarg.0
      IL_008e:  ldstr      "SimExit"
      IL_0093:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0098:  ldarg.0
      IL_0099:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_009e:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> Sims3.Store.Objects.SaunaClassic::GetSimsInFootprint()
      IL_00a3:  stloc.1
      IL_00a4:  ldloc.1
      IL_00a5:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::GetEnumerator()
      IL_00aa:  stloc.s    V_11
      .try
      {
        IL_00ac:  br.s       IL_00cb

        IL_00ae:  ldloca.s   V_11
        IL_00b0:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Current()
        IL_00b5:  stloc.2
        IL_00b6:  ldloc.2
        IL_00b7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_00bc:  ldc.i4     0x10f38da
        IL_00c1:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kFunBoostForSteam
        IL_00c6:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              float32)
        IL_00cb:  ldloca.s   V_11
        IL_00cd:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::MoveNext()
        IL_00d2:  brtrue.s   IL_00ae

        IL_00d4:  leave.s    IL_00e4

      }  // end .try
      finally
      {
        IL_00d6:  ldloca.s   V_11
        IL_00d8:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>
        IL_00de:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_00e3:  endfinally
      }  // end handler
      IL_00e4:  ldarg.0
      IL_00e5:  ldc.i4.1
      IL_00e6:  ldc.i4.0
      IL_00e7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_00ec:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.InteractionsShared.Sit::Singleton
      IL_00f1:  ldarg.0
      IL_00f2:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_00f7:  ldarg.0
      IL_00f8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_00fd:  ldarg.0
      IL_00fe:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0103:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_0108:  ldc.i4.0
      IL_0109:  ldc.i4.0
      IL_010a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_010f:  stloc.3
      IL_0110:  ldloc.3
      IL_0111:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::RunInteraction()
      IL_0116:  stloc.s    V_4
      IL_0118:  ldloc.s    V_4
      IL_011a:  brfalse    IL_02b1

      IL_011f:  ldarg.0
      IL_0120:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_0125:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_MinSitWaitTime()
      IL_012a:  stloc.s    V_5
      IL_012c:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_MaxSitWaitTime()
      IL_0131:  stloc.s    V_6
      IL_0133:  ldarg.0
      IL_0134:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0139:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_013e:  ldarg.0
      IL_013f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0144:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RegisterGroupTalk()
      IL_0149:  ldloc.s    V_5
      IL_014b:  ldloc.s    V_6
      IL_014d:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomFloatGaussianDistribution(float32,
                                                                                                                         float32)
      IL_0152:  stloc.s    V_7
      IL_0154:  call       valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.DateAndTime [Sims3GameplaySystems]Sims3.Gameplay.Utilities.SimClock::CurrentTime()
      IL_0159:  stloc.s    V_8
      IL_015b:  br         IL_022b

      IL_0160:  ldarg.0
      IL_0161:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0166:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShouldDoGroupTalk()
      IL_016b:  brfalse    IL_021f

      IL_0170:  ldarg.0
      IL_0171:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0176:  ldnull
      IL_0177:  ldnull
      IL_0178:  ldc.i4.1
      IL_0179:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::DoGroupTalk(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/GroupTalkDelegate,
                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/GroupTalkDelegate,
                                                                                                      bool)
      IL_017e:  brfalse    IL_021f

      IL_0183:  ldarg.0
      IL_0184:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0189:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
      IL_018e:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::GetEnumerator()
      IL_0193:  stloc.s    V_12
      .try
      {
        IL_0195:  br.s       IL_0206

        IL_0197:  ldloca.s   V_12
        IL_0199:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Current()
        IL_019e:  stloc.s    V_9
        IL_01a0:  ldloc.s    V_9
        IL_01a2:  ldarg.0
        IL_01a3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
        IL_01a8:  beq.s      IL_01dd

        IL_01aa:  ldarg.0
        IL_01ab:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
        IL_01b0:  ldloc.s    V_9
        IL_01b2:  ldc.i4.1
        IL_01b3:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Socializing.Relationship [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetRelationship(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                           bool)
        IL_01b8:  stloc.s    V_10
        IL_01ba:  ldloc.s    V_10
        IL_01bc:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Socializing.LongTermRelationship [Sims3GameplaySystems]Sims3.Gameplay.Socializing.Relationship::get_LTR()
        IL_01c1:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kGroupChatRelationshipBump
        IL_01c6:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.LongTermRelationship::UpdateLiking(float32)
        IL_01cb:  ldc.i4.5
        IL_01cc:  ldarg.0
        IL_01cd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
        IL_01d2:  ldloc.s    V_9
        IL_01d4:  ldc.i4.1
        IL_01d5:  ldc.i4.0
        IL_01d6:  ldc.i4.0
        IL_01d7:  ldc.i4.0
        IL_01d8:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.SocialComponent::SetSocialFeedbackForActorAndTarget(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Socializing.CommodityTypes,
                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                       bool,
                                                                                                                                       int32,
                                                                                                                                       valuetype [UI]Sims3.UI.Controller.LongTermRelationshipTypes,
                                                                                                                                       valuetype [UI]Sims3.UI.Controller.LongTermRelationshipTypes)
        IL_01dd:  ldloc.s    V_9
        IL_01df:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_01e4:  ldc.i4     0x10f38e0
        IL_01e9:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
        IL_01ee:  brfalse.s  IL_0206

        IL_01f0:  ldloc.s    V_9
        IL_01f2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_01f7:  ldc.i4     0x10f38e0
        IL_01fc:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kGroupChatSocialBump
        IL_0201:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              float32)
        IL_0206:  ldloca.s   V_12
        IL_0208:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::MoveNext()
        IL_020d:  brtrue.s   IL_0197

        IL_020f:  leave.s    IL_021f

      }  // end .try
      finally
      {
        IL_0211:  ldloca.s   V_12
        IL_0213:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>
        IL_0219:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_021e:  endfinally
      }  // end handler
      IL_021f:  ldc.i4.1
      IL_0220:  ldloc.s    V_8
      IL_0222:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Utilities.SimClock::ElapsedTime(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.DateAndTime)
      IL_0227:  ldloc.s    V_7
      IL_0229:  bgt.s      IL_0245

      IL_022b:  ldarg.0
      IL_022c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0231:  ldsfld     float32 [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::kWaitForExitReasonDefaultTime
      IL_0236:  ldc.i4     0xffa9bfff
      IL_023b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::WaitForExitReason(float32,
                                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0240:  brfalse    IL_0160

      IL_0245:  ldarg.0
      IL_0246:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_024b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::UnregisterGroupTalk()
      IL_0250:  ldarg.0
      IL_0251:  ldc.i4.1
      IL_0252:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_0257:  ldarg.0
      IL_0258:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Cancelled()
      IL_025d:  brtrue.s   IL_02b1

      IL_025f:  ldarg.0
      IL_0260:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0265:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_026a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::GetNextInteraction()
      IL_026f:  brtrue.s   IL_02b1

      IL_0271:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::CoinFlip()
      IL_0276:  brfalse.s  IL_02b1

      IL_0278:  ldarg.0
      IL_0279:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_027e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0283:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/MakeSteam::Singleton
      IL_0288:  ldarg.0
      IL_0289:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_028e:  ldarg.0
      IL_028f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_0294:  ldarg.0
      IL_0295:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Actor
      IL_029a:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_029f:  ldarg.0
      IL_02a0:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_02a5:  ldc.i4.1
      IL_02a6:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_02ab:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::Add(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_02b0:  pop
      IL_02b1:  ldloc.0
      IL_02b2:  ret
    } // end of method MakeSteam::Run

    .method public hidebysig instance void 
            OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                             class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       25 (0x19)
      .maxstack  8
      IL_0000:  ldarg.2
      IL_0001:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.IEvent::get_EventId()
      IL_0006:  ldc.i4     0x384
      IL_000b:  bne.un.s   IL_0018

      IL_000d:  ldarg.0
      IL_000e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0013:  callvirt   instance void Sims3.Store.Objects.SaunaClassic::StartStonesSteamFX()
      IL_0018:  ret
    } // end of method MakeSteam::OnAnimationEvent

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_0006:  ret
    } // end of method MakeSteam::Cleanup

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::.ctor()
      IL_0006:  ret
    } // end of method MakeSteam::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.SaunaClassic/MakeSteam/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/MakeSteam::Singleton
      IL_000a:  ret
    } // end of method MakeSteam::.cctor

  } // end of class MakeSteam

  .class auto ansi nested public beforefieldinit CuddleSeatedWooHooSauna
         extends [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.NestedCuddleInteraction
  {
    .class auto ansi nested public TryingFor
           extends [mscorlib]System.Enum
    {
      .field public specialname rtspecialname int32 value__
      .field public static literal valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor TryForNothing = int32(0x00000000)
      .field public static literal valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor TryForGirl = int32(0x00000001)
      .field public static literal valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor TryForBoy = int32(0x00000002)
    } // end of class TryingFor

    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna>
    {
      .class auto ansi nested public beforefieldinit '<>c__DisplayClassa'
             extends [mscorlib]System.Object
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a
        .method public hidebysig specialname rtspecialname 
                instance void  .ctor() cil managed
        {
          // Code size       7 (0x7)
          .maxstack  8
          IL_0000:  ldarg.0
          IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
          IL_0006:  ret
        } // end of method '<>c__DisplayClassa'::.ctor

        .method public hidebysig instance string 
                '<Test>b__8'() cil managed
        {
          // Code size       39 (0x27)
          .maxstack  5
          .locals init (object[] V_0)
          IL_0000:  ldarg.0
          IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
          IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
          IL_000b:  ldstr      "HydrophobicNotAllowed"
          IL_0010:  ldc.i4.1
          IL_0011:  newarr     [mscorlib]System.Object
          IL_0016:  stloc.0
          IL_0017:  ldloc.0
          IL_0018:  ldc.i4.0
          IL_0019:  ldarg.0
          IL_001a:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
          IL_001f:  stelem.ref
          IL_0020:  ldloc.0
          IL_0021:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
          IL_0026:  ret
        } // end of method '<>c__DisplayClassa'::'<Test>b__8'

      } // end of class '<>c__DisplayClassa'

      .field public valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor mTryingFor
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor desired) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::mTryingFor
        IL_000d:  ret
      } // end of method Definition::.ctor

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       178 (0xb2)
        .maxstack  4
        .locals init (bool V_0,
                 bool V_1,
                 class [UI]Sims3.SimIFace.GreyedOutTooltipCallback V_2,
                 class Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa' V_3)
        IL_0000:  ldnull
        IL_0001:  stloc.2
        IL_0002:  newobj     instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::.ctor()
        IL_0007:  stloc.3
        IL_0008:  ldloc.3
        IL_0009:  ldarg.1
        IL_000a:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
        IL_000f:  ldloc.3
        IL_0010:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
        IL_0015:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
        IL_001a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
        IL_001f:  brtrue.s   IL_0033

        IL_0021:  ldloc.3
        IL_0022:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
        IL_0027:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
        IL_002c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
        IL_0031:  brfalse.s  IL_0035

        IL_0033:  ldc.i4.0
        IL_0034:  ret

        IL_0035:  ldloc.3
        IL_0036:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
        IL_003b:  ldarg.2
        IL_003c:  call       bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.NestedSocialInteraction`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated>::Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                                   class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_0041:  stloc.0
        IL_0042:  ldloc.0
        IL_0043:  brfalse.s  IL_00b0

        IL_0045:  ldc.i4.0
        IL_0046:  stloc.1
        IL_0047:  ldarg.0
        IL_0048:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::mTryingFor
        IL_004d:  brtrue.s   IL_005f

        IL_004f:  ldloc.3
        IL_0050:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
        IL_0055:  ldarg.2
        IL_0056:  ldc.i4.1
        IL_0057:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::CanGetRomantic(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           bool)
        IL_005c:  stloc.1
        IL_005d:  br.s       IL_006e

        IL_005f:  ldloc.3
        IL_0060:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
        IL_0065:  ldarg.2
        IL_0066:  ldarg.s    greyedOutTooltipCallback
        IL_0068:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy::CanTryForBaby(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_006d:  stloc.1
        IL_006e:  ldloc.1
        IL_006f:  brfalse.s  IL_00ae

        IL_0071:  ldloc.3
        IL_0072:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::a
        IL_0077:  ldc.i8     0x4c800171bd46a80
        IL_0080:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_0085:  brtrue.s   IL_0098

        IL_0087:  ldarg.2
        IL_0088:  ldc.i8     0x4c800171bd46a80
        IL_0091:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_0096:  brfalse.s  IL_00ae

        IL_0098:  ldarg.s    greyedOutTooltipCallback
        IL_009a:  ldloc.2
        IL_009b:  brtrue.s   IL_00aa

        IL_009d:  ldloc.3
        IL_009e:  ldftn      instance string Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition/'<>c__DisplayClassa'::'<Test>b__8'()
        IL_00a4:  newobj     instance void [UI]Sims3.SimIFace.GreyedOutTooltipCallback::.ctor(object,
                                                                                              native int)
        IL_00a9:  stloc.2
        IL_00aa:  ldloc.2
        IL_00ab:  stind.ref
        IL_00ac:  ldc.i4.0
        IL_00ad:  ret

        IL_00ae:  ldloc.1
        IL_00af:  ret

        IL_00b0:  ldc.i4.0
        IL_00b1:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       117 (0x75)
        .maxstack  5
        .locals init (object[] V_0,
                 object[] V_1,
                 object[] V_2)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::mTryingFor
        IL_0006:  ldc.i4.2
        IL_0007:  bne.un.s   IL_002a

        IL_0009:  ldarg.1
        IL_000a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_000f:  ldstr      "TryForBoy"
        IL_0014:  ldc.i4.2
        IL_0015:  newarr     [mscorlib]System.Object
        IL_001a:  stloc.0
        IL_001b:  ldloc.0
        IL_001c:  ldc.i4.0
        IL_001d:  ldarg.1
        IL_001e:  stelem.ref
        IL_001f:  ldloc.0
        IL_0020:  ldc.i4.1
        IL_0021:  ldarg.2
        IL_0022:  stelem.ref
        IL_0023:  ldloc.0
        IL_0024:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_0029:  ret

        IL_002a:  ldarg.0
        IL_002b:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::mTryingFor
        IL_0030:  ldc.i4.1
        IL_0031:  bne.un.s   IL_0054

        IL_0033:  ldarg.1
        IL_0034:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0039:  ldstr      "TryForGirl"
        IL_003e:  ldc.i4.2
        IL_003f:  newarr     [mscorlib]System.Object
        IL_0044:  stloc.1
        IL_0045:  ldloc.1
        IL_0046:  ldc.i4.0
        IL_0047:  ldarg.1
        IL_0048:  stelem.ref
        IL_0049:  ldloc.1
        IL_004a:  ldc.i4.1
        IL_004b:  ldarg.2
        IL_004c:  stelem.ref
        IL_004d:  ldloc.1
        IL_004e:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_0053:  ret

        IL_0054:  ldarg.1
        IL_0055:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_005a:  ldstr      "WooHoo"
        IL_005f:  ldc.i4.2
        IL_0060:  newarr     [mscorlib]System.Object
        IL_0065:  stloc.2
        IL_0066:  ldloc.2
        IL_0067:  ldc.i4.0
        IL_0068:  ldarg.1
        IL_0069:  stelem.ref
        IL_006a:  ldloc.2
        IL_006b:  ldc.i4.1
        IL_006c:  ldarg.2
        IL_006d:  stelem.ref
        IL_006e:  ldloc.2
        IL_006f:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_0074:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance string[] 
              GetPath(bool isFemale) cil managed
      {
        // Code size       39 (0x27)
        .maxstack  5
        .locals init (string[] V_0)
        IL_0000:  ldc.i4.1
        IL_0001:  newarr     [mscorlib]System.String
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  ldc.i4.0
        IL_0009:  ldarg.1
        IL_000a:  ldstr      "WoohooPath"
        IL_000f:  ldc.i4.0
        IL_0010:  newarr     [mscorlib]System.Object
        IL_0015:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_001a:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::get_Ellipsis()
        IL_001f:  call       string [mscorlib]System.String::Concat(string,
                                                                    string)
        IL_0024:  stelem.ref
        IL_0025:  ldloc.0
        IL_0026:  ret
      } // end of method Definition::GetPath

    } // end of class Definition

    .field public static literal int32 mSteamyStoneTrigger = int32(0x00000384)
    .field public class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Beds.WooHoo/WooHooPrivacySituation mSituation
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster mReactToSocialBroadcaster
    .field public bool mPrivacyFailed
    .field public class Sims3.Store.Objects.SaunaClassic mSauna
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition TryForBoySingleton
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition TryForGirlSingleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       1610 (0x64a)
      .maxstack  6
      .locals init (class Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_2,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_3,
               class Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna V_4,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_5,
               bool V_6,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_7,
               bool V_8,
               bool V_9,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_10,
               bool V_11)
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0006:  isinst     Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition
      IL_000b:  stloc.0
      IL_000c:  ldarg.0
      IL_000d:  call       instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.NestedSocialInteraction`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated>::StartSync()
      IL_0012:  brtrue.s   IL_001c

      IL_0014:  ldarg.0
      IL_0015:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.NestedSocialInteraction`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated>::DoResume()
      IL_001a:  ldc.i4.0
      IL_001b:  ret

      IL_001c:  ldarg.0
      IL_001d:  ldc.i4.0
      IL_001e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry(bool)
      IL_0023:  ldarg.0
      IL_0024:  ldarg.0
      IL_0025:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_002a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_002f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::get_Container()
      IL_0034:  isinst     Sims3.Store.Objects.SaunaClassic
      IL_0039:  stfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_003e:  ldarg.0
      IL_003f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0044:  ldarg.0
      IL_0045:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_004a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::GetLeftSim()
      IL_004f:  bne.un     IL_058b

      IL_0054:  ldarg.0
      IL_0055:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_005a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_005f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::get_Container()
      IL_0064:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup
      IL_0069:  stloc.1
      IL_006a:  ldloc.1
      IL_006b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup::get_SeatingGroup()
      IL_0070:  ldarg.0
      IL_0071:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0076:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_007b:  stloc.2
      IL_007c:  ldloc.1
      IL_007d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup::get_SeatingGroup()
      IL_0082:  ldarg.0
      IL_0083:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0088:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_008d:  stloc.3
      IL_008e:  ldloc.2
      IL_008f:  brfalse.s  IL_0094

      IL_0091:  ldloc.3
      IL_0092:  brtrue.s   IL_00a2

      IL_0094:  ldarg.0
      IL_0095:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_009a:  ldc.i4.2
      IL_009b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_00a0:  ldc.i4.0
      IL_00a1:  ret

      IL_00a2:  ldarg.0
      IL_00a3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.NestedSocialInteraction`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated>::ReturnInstance
      IL_00a8:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated::EnsureMaster()
      IL_00ad:  ldarg.0
      IL_00ae:  ldarg.0
      IL_00af:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.NestedSocialInteraction`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated>::ReturnInstance
      IL_00b4:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_00b9:  stfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_00be:  ldarg.0
      IL_00bf:  ldstr      "WooHoo"
      IL_00c4:  call       instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::StartSocial(string)
      IL_00c9:  pop
      IL_00ca:  ldarg.0
      IL_00cb:  ldarg.0
      IL_00cc:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_00d1:  ldarg.0
      IL_00d2:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_00d7:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::InitiateSocialUI(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_00dc:  ldarg.0
      IL_00dd:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_LinkedInteractionInstance()
      IL_00e2:  isinst     Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna
      IL_00e7:  stloc.s    V_4
      IL_00e9:  ldloc.s    V_4
      IL_00eb:  ldarg.0
      IL_00ec:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_00f1:  stfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_00f6:  ldarg.0
      IL_00f7:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_00fc:  brfalse.s  IL_015a

      IL_00fe:  ldarg.0
      IL_00ff:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0104:  ldnull
      IL_0105:  ldstr      "Woo Hoo Reject"
      IL_010a:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_010f:  ldarg.0
      IL_0110:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0115:  ldnull
      IL_0116:  ldstr      "ExitSitting"
      IL_011b:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_0120:  ldarg.0
      IL_0121:  ldstr      "WooHoo"
      IL_0126:  ldc.i4.1
      IL_0127:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::FinishSocial(string,
                                                                                                                                                                                                                                          bool)
      IL_012c:  ldarg.0
      IL_012d:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::FinishSocialContext()
      IL_0132:  ldarg.0
      IL_0133:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0138:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_013d:  ldc.i8     0x70a900201f452381
      IL_0146:  ldc.i8     0x8bfc002118898376
      IL_014f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_0154:  pop
      IL_0155:  br         IL_0597

      IL_015a:  ldarg.0
      IL_015b:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0160:  ldnull
      IL_0161:  ldstr      "ExitSitting"
      IL_0166:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_016b:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.InteractionsShared.Stand::Singleton
      IL_0170:  ldarg.0
      IL_0171:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_0176:  ldarg.0
      IL_0177:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_017c:  ldarg.0
      IL_017d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0182:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_0187:  ldc.i4.0
      IL_0188:  ldc.i4.0
      IL_0189:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_018e:  stloc.s    V_5
      IL_0190:  ldloc.s    V_5
      IL_0192:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::RunInteraction()
      IL_0197:  stloc.s    V_6
      IL_0199:  ldloc.s    V_6
      IL_019b:  brfalse    IL_055d

      IL_01a0:  ldarg.0
      IL_01a1:  ldarg.0
      IL_01a2:  newobj     instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Beds.WooHoo/WooHooPrivacySituation::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_01a7:  stfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Beds.WooHoo/WooHooPrivacySituation Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSituation
      IL_01ac:  ldarg.0
      IL_01ad:  ldarg.0
      IL_01ae:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Beds.WooHoo/WooHooPrivacySituation Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSituation
      IL_01b3:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation::Start()
      IL_01b8:  ldc.i4.0
      IL_01b9:  ceq
      IL_01bb:  stfld      bool Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mPrivacyFailed
      IL_01c0:  ldloc.s    V_4
      IL_01c2:  ldarg.0
      IL_01c3:  ldfld      bool Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mPrivacyFailed
      IL_01c8:  stfld      bool Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mPrivacyFailed
      IL_01cd:  ldarg.0
      IL_01ce:  ldfld      bool Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mPrivacyFailed
      IL_01d3:  brtrue     IL_055d

      IL_01d8:  ldarg.0
      IL_01d9:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_01de:  ldarg.0
      IL_01df:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_01e4:  ldc.i4     0xa678e703
      IL_01e9:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlot(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                      valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_01ee:  brfalse    IL_055d

      IL_01f3:  ldarg.0
      IL_01f4:  ldstr      "Sauna_store"
      IL_01f9:  ldstr      "SimEnter"
      IL_01fe:  ldstr      "x"
      IL_0203:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_0208:  ldarg.0
      IL_0209:  ldstr      "saunaX"
      IL_020e:  ldloc.2
      IL_020f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::get_Host()
      IL_0214:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0219:  ldarg.0
      IL_021a:  ldstr      "saunaY"
      IL_021f:  ldloc.3
      IL_0220:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::get_Host()
      IL_0225:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_022a:  ldarg.0
      IL_022b:  ldc.i4     0x384
      IL_0230:  ldarg.0
      IL_0231:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                    class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0237:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_023c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddOneShotScriptEventHandler(uint32,
                                                                                                                                             class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0241:  ldarg.0
      IL_0242:  ldstr      "PourWater"
      IL_0247:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_024c:  ldarg.0
      IL_024d:  ldstr      "SimExit"
      IL_0252:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0257:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.InteractionsShared.Sit::Singleton
      IL_025c:  ldarg.0
      IL_025d:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_0262:  ldarg.0
      IL_0263:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0268:  ldarg.0
      IL_0269:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_026e:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_0273:  ldc.i4.0
      IL_0274:  ldc.i4.0
      IL_0275:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_027a:  stloc.s    V_7
      IL_027c:  ldloc.s    V_7
      IL_027e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::RunInteraction()
      IL_0283:  stloc.s    V_8
      IL_0285:  ldloc.s    V_8
      IL_0287:  brfalse    IL_055d

      IL_028c:  ldarg.0
      IL_028d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0292:  ldc.i4.0
      IL_0293:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetalBase::HandleNearbyWoohoo(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetalBase/HowMuchWooHoo)
      IL_0298:  ldarg.0
      IL_0299:  ldstr      "sauna_store"
      IL_029e:  ldstr      "SimEnter"
      IL_02a3:  ldstr      "x"
      IL_02a8:  ldstr      "y"
      IL_02ad:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_02b2:  ldarg.0
      IL_02b3:  ldstr      "saunaX"
      IL_02b8:  ldarg.0
      IL_02b9:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_02be:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_02c3:  ldarg.0
      IL_02c4:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_02c9:  ldc.i4.s   110
      IL_02cb:  ldarg.0
      IL_02cc:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                    class [SimIFace]Sims3.SimIFace.IEvent)
      IL_02d2:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_02d7:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_02dc:  ldarg.0
      IL_02dd:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_02e2:  ldc.i4.s   111
      IL_02e4:  ldarg.0
      IL_02e5:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                    class [SimIFace]Sims3.SimIFace.IEvent)
      IL_02eb:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_02f0:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_02f5:  ldarg.0
      IL_02f6:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_02fb:  ldc.i4.s   120
      IL_02fd:  ldarg.0
      IL_02fe:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                    class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0304:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0309:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_030e:  ldarg.0
      IL_030f:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0314:  ldc.i4.s   121
      IL_0316:  ldarg.0
      IL_0317:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                    class [SimIFace]Sims3.SimIFace.IEvent)
      IL_031d:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0322:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0327:  ldloc.0
      IL_0328:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::mTryingFor
      IL_032d:  brfalse.s  IL_0348

      IL_032f:  ldarg.0
      IL_0330:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0335:  ldc.i4.s   112
      IL_0337:  ldarg.0
      IL_0338:  ldftn      instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::OnBabyCheckEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                    class [SimIFace]Sims3.SimIFace.IEvent)
      IL_033e:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0343:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0348:  ldarg.0
      IL_0349:  ldstr      "saunaX"
      IL_034e:  ldloc.2
      IL_034f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::get_Host()
      IL_0354:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0359:  ldarg.0
      IL_035a:  ldstr      "saunaY"
      IL_035f:  ldloc.3
      IL_0360:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::get_Host()
      IL_0365:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_036a:  ldarg.0
      IL_036b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0370:  ldarg.0
      IL_0371:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_0376:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::GetLeftSim()
      IL_037b:  bne.un.s   IL_03d7

      IL_037d:  ldarg.0
      IL_037e:  ldstr      "IsMirrored"
      IL_0383:  ldc.i4.1
      IL_0384:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             bool)
      IL_0389:  ldarg.0
      IL_038a:  ldstr      "SuffixX"
      IL_038f:  ldarg.0
      IL_0390:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_0395:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
      IL_039a:  ldarg.0
      IL_039b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_03a0:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_03a5:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::IKSuffix
      IL_03aa:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             string)
      IL_03af:  ldarg.0
      IL_03b0:  ldstr      "SuffixY"
      IL_03b5:  ldarg.0
      IL_03b6:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_03bb:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
      IL_03c0:  ldarg.0
      IL_03c1:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_03c6:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_03cb:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::IKSuffix
      IL_03d0:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             string)
      IL_03d5:  br.s       IL_042f

      IL_03d7:  ldarg.0
      IL_03d8:  ldstr      "IsMirrored"
      IL_03dd:  ldc.i4.0
      IL_03de:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             bool)
      IL_03e3:  ldarg.0
      IL_03e4:  ldstr      "SuffixY"
      IL_03e9:  ldarg.0
      IL_03ea:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_03ef:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
      IL_03f4:  ldarg.0
      IL_03f5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_03fa:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_03ff:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::IKSuffix
      IL_0404:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             string)
      IL_0409:  ldarg.0
      IL_040a:  ldstr      "SuffixX"
      IL_040f:  ldarg.0
      IL_0410:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_0415:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
      IL_041a:  ldarg.0
      IL_041b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0420:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0425:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::IKSuffix
      IL_042a:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             string)
      IL_042f:  ldarg.0
      IL_0430:  ldstr      "Woohoo"
      IL_0435:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateJoinSims(string)
      IL_043a:  ldarg.0
      IL_043b:  ldstr      "SimExit"
      IL_0440:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateJoinSims(string)
      IL_0445:  ldarg.0
      IL_0446:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_044b:  ldc.i4.1
      IL_044c:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetalBase::HandleNearbyWoohoo(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetalBase/HowMuchWooHoo)
      IL_0451:  ldarg.0
      IL_0452:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0457:  ldarg.0
      IL_0458:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_045d:  ldarg.0
      IL_045e:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_0463:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.SocialCallback::RunPostWoohoo(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_0468:  ldarg.0
      IL_0469:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_046e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0473:  ldc.i4     0x10f38da
      IL_0478:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooFunBump
      IL_047d:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_0482:  ldarg.0
      IL_0483:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0488:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_048d:  ldc.i4     0x10f38e0
      IL_0492:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooSocialBump
      IL_0497:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_049c:  ldarg.0
      IL_049d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_04a2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_04a7:  ldc.i4     0x10f38d9
      IL_04ac:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooHygieneBump
      IL_04b1:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_04b6:  ldarg.0
      IL_04b7:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_04bc:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_04c1:  ldc.i4     0x10f38da
      IL_04c6:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooFunBump
      IL_04cb:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_04d0:  ldarg.0
      IL_04d1:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_04d6:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_04db:  ldc.i4     0x10f38e0
      IL_04e0:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooSocialBump
      IL_04e5:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_04ea:  ldarg.0
      IL_04eb:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_04f0:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_04f5:  ldc.i4     0x10f38d9
      IL_04fa:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooHygieneBump
      IL_04ff:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_0504:  ldarg.0
      IL_0505:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_050a:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::mLotCurrent
      IL_050f:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.LotType [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_LotType()
      IL_0514:  ldc.i4.1
      IL_0515:  bne.un.s   IL_055d

      IL_0517:  ldarg.0
      IL_0518:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_051d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0522:  ldc.i8     0x15e6003433a42be0
      IL_052b:  ldc.i8     0x15e6003433a42c00
      IL_0534:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_0539:  pop
      IL_053a:  ldarg.0
      IL_053b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0540:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0545:  ldc.i8     0x15e6003433a42be0
      IL_054e:  ldc.i8     0x15e6003433a42c00
      IL_0557:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_055c:  pop
      IL_055d:  ldarg.0
      IL_055e:  ldstr      "WooHoo"
      IL_0563:  ldc.i4.1
      IL_0564:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::FinishSocial(string,
                                                                                                                                                                                                                                          bool)
      IL_0569:  ldarg.0
      IL_056a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_056f:  ldc.i4     0x2000
      IL_0574:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0579:  ldarg.0
      IL_057a:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_057f:  ldc.i4     0x2000
      IL_0584:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0589:  br.s       IL_0597

      IL_058b:  ldarg.0
      IL_058c:  ldc.i4     0xffa9bfff
      IL_0591:  call       instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0596:  pop
      IL_0597:  ldarg.0
      IL_0598:  ldarg.0
      IL_0599:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.NestedSocialInteraction`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated>::IsMaster
      IL_059e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::FinishLinkedInteraction(bool)
      IL_05a3:  ldarg.0
      IL_05a4:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_05a9:  brtrue.s   IL_05b6

      IL_05ab:  ldarg.0
      IL_05ac:  ldfld      bool Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mPrivacyFailed
      IL_05b1:  ldc.i4.0
      IL_05b2:  ceq
      IL_05b4:  br.s       IL_05b7

      IL_05b6:  ldc.i4.0
      IL_05b7:  stloc.s    V_9
      IL_05b9:  ldarg.0
      IL_05ba:  ldloc.s    V_9
      IL_05bc:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_05c1:  ldarg.0
      IL_05c2:  ldc.i4.0
      IL_05c3:  ldc.i4.0
      IL_05c4:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_05c9:  ldarg.0
      IL_05ca:  ldc.i4.0
      IL_05cb:  stfld      bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.NestedCuddleInteraction::InvokeDoResumeOnCleanup
      IL_05d0:  ldarg.0
      IL_05d1:  ldfld      bool Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mPrivacyFailed
      IL_05d6:  brtrue.s   IL_05e8

      IL_05d8:  ldarg.0
      IL_05d9:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_05de:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_05e3:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::SetFirstWooHoo()
      IL_05e8:  ldarg.0
      IL_05e9:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_05ee:  brfalse.s  IL_0639

      IL_05f0:  ldarg.0
      IL_05f1:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.NestedSocialInteraction`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated>::IsMaster
      IL_05f6:  brtrue.s   IL_0639

      IL_05f8:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.InteractionsShared.Stand::Singleton
      IL_05fd:  ldarg.0
      IL_05fe:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_0603:  ldarg.0
      IL_0604:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0609:  ldarg.0
      IL_060a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_060f:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_0614:  ldc.i4.0
      IL_0615:  ldc.i4.0
      IL_0616:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_061b:  stloc.s    V_10
      IL_061d:  ldloc.s    V_10
      IL_061f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::RunInteraction()
      IL_0624:  stloc.s    V_11
      IL_0626:  ldloc.s    V_11
      IL_0628:  brfalse.s  IL_0639

      IL_062a:  ldarg.0
      IL_062b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0630:  ldc.i4.s   11
      IL_0632:  ldc.i4.4
      IL_0633:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_0638:  pop
      IL_0639:  ldarg.0
      IL_063a:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::WaitForSyncComplete()
      IL_063f:  pop
      IL_0640:  ldarg.0
      IL_0641:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_0646:  ldc.i4.0
      IL_0647:  ceq
      IL_0649:  ret
    } // end of method CuddleSeatedWooHooSauna::Run

    .method public hidebysig instance void 
            OnAnimationEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                             class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       133 (0x85)
      .maxstack  2
      IL_0000:  ldarg.2
      IL_0001:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.IEvent::get_EventId()
      IL_0006:  ldc.i4.s   110
      IL_0008:  bne.un.s   IL_0027

      IL_000a:  ldarg.0
      IL_000b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0010:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::FadeOut()
      IL_0015:  ldarg.0
      IL_0016:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_001b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::FadeOut()
      IL_0020:  ldarg.0
      IL_0021:  call       instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::StartJealousyBroadcaster()
      IL_0026:  ret

      IL_0027:  ldarg.2
      IL_0028:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.IEvent::get_EventId()
      IL_002d:  ldc.i4.s   111
      IL_002f:  bne.un.s   IL_0048

      IL_0031:  ldarg.0
      IL_0032:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0037:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::FadeIn()
      IL_003c:  ldarg.0
      IL_003d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0042:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::FadeIn()
      IL_0047:  ret

      IL_0048:  ldarg.2
      IL_0049:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.IEvent::get_EventId()
      IL_004e:  ldc.i4.s   120
      IL_0050:  bne.un.s   IL_005a

      IL_0052:  ldarg.0
      IL_0053:  ldc.i4.1
      IL_0054:  call       instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::EnableDisableCensor(bool)
      IL_0059:  ret

      IL_005a:  ldarg.2
      IL_005b:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.IEvent::get_EventId()
      IL_0060:  ldc.i4.s   121
      IL_0062:  bne.un.s   IL_006c

      IL_0064:  ldarg.0
      IL_0065:  ldc.i4.0
      IL_0066:  call       instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::EnableDisableCensor(bool)
      IL_006b:  ret

      IL_006c:  ldarg.2
      IL_006d:  callvirt   instance uint32 [SimIFace]Sims3.SimIFace.IEvent::get_EventId()
      IL_0072:  ldc.i4     0x384
      IL_0077:  bne.un.s   IL_0084

      IL_0079:  ldarg.0
      IL_007a:  ldfld      class Sims3.Store.Objects.SaunaClassic Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSauna
      IL_007f:  callvirt   instance void Sims3.Store.Objects.SaunaClassic::StartStonesSteamFX()
      IL_0084:  ret
    } // end of method CuddleSeatedWooHooSauna::OnAnimationEvent

    .method public hidebysig instance void 
            OnBabyCheckEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                             class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       269 (0x10d)
      .maxstack  3
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_0,
               float32 V_1,
               class Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_000c:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kBabyMadeChance
      IL_0011:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy::ShouldImpregnate(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                              float32)
      IL_0016:  brfalse    IL_010c

      IL_001b:  ldarg.0
      IL_001c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0021:  ldarg.0
      IL_0022:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0027:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy::Start(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                   class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_002c:  brfalse    IL_010c

      IL_0031:  ldarg.0
      IL_0032:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0037:  stloc.0
      IL_0038:  ldarg.0
      IL_0039:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_003e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_0043:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::Pregnancy
      IL_0048:  brfalse.s  IL_0051

      IL_004a:  ldarg.0
      IL_004b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0050:  stloc.0
      IL_0051:  ldsfld     float32[] Sims3.Store.Objects.SaunaClassic::kBabyTraitChance
      IL_0056:  call       int32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetWeightedIndex(float32[])
      IL_005b:  conv.r4
      IL_005c:  stloc.1
      IL_005d:  ldloc.1
      IL_005e:  ldc.r4     1.
      IL_0063:  bne.un.s   IL_0080

      IL_0065:  ldloc.0
      IL_0066:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_006b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::Pregnancy
      IL_0070:  ldc.i8     0x4c800171bd46a80
      IL_0079:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy::SetForcedBabyTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_007e:  br.s       IL_00a1

      IL_0080:  ldloc.1
      IL_0081:  ldc.r4     2.
      IL_0086:  bne.un.s   IL_00a1

      IL_0088:  ldloc.0
      IL_0089:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_008e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::Pregnancy
      IL_0093:  ldc.i8     0xb82d0015b9294550
      IL_009c:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy::SetForcedBabyTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_00a1:  ldstr      "sting_baby_conception"
      IL_00a6:  call       uint32 [SimIFace]Sims3.SimIFace.Audio::StartSound(string)
      IL_00ab:  pop
      IL_00ac:  ldarg.0
      IL_00ad:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_00b2:  isinst     Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition
      IL_00b7:  stloc.2
      IL_00b8:  ldloc.2
      IL_00b9:  brfalse.s  IL_010c

      IL_00bb:  ldloc.2
      IL_00bc:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::mTryingFor
      IL_00c1:  ldc.i4.2
      IL_00c2:  bne.un.s   IL_00e2

      IL_00c4:  ldc.i4.0
      IL_00c5:  stloc.3
      IL_00c6:  br.s       IL_00dc

      IL_00c8:  ldloc.0
      IL_00c9:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_00ce:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::Pregnancy
      IL_00d3:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy::IncreaseMaleChance()
      IL_00d8:  ldloc.3
      IL_00d9:  ldc.i4.1
      IL_00da:  add
      IL_00db:  stloc.3
      IL_00dc:  ldloc.3
      IL_00dd:  ldc.i4.s   50
      IL_00df:  blt.s      IL_00c8

      IL_00e1:  ret

      IL_00e2:  ldloc.2
      IL_00e3:  ldfld      valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::mTryingFor
      IL_00e8:  ldc.i4.1
      IL_00e9:  bne.un.s   IL_010c

      IL_00eb:  ldc.i4.0
      IL_00ec:  stloc.s    V_4
      IL_00ee:  br.s       IL_0106

      IL_00f0:  ldloc.0
      IL_00f1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_00f6:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::Pregnancy
      IL_00fb:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Pregnancy::IncreaseFemaleChance()
      IL_0100:  ldloc.s    V_4
      IL_0102:  ldc.i4.1
      IL_0103:  add
      IL_0104:  stloc.s    V_4
      IL_0106:  ldloc.s    V_4
      IL_0108:  ldc.i4.s   50
      IL_010a:  blt.s      IL_00f0

      IL_010c:  ret
    } // end of method CuddleSeatedWooHooSauna::OnBabyCheckEvent

    .method public hidebysig instance void 
            StartJealousyBroadcaster() cil managed
    {
      // Code size       93 (0x5d)
      .maxstack  6
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mReactToSocialBroadcaster
      IL_0006:  brtrue.s   IL_0044

      IL_0008:  ldarg.0
      IL_0009:  ldarg.0
      IL_000a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_000f:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcasterParams [Sims3GameplaySystems]Sims3.Gameplay.Socializing.Conversation::get_ReactToSocialParams()
      IL_0014:  ldnull
      IL_0015:  ldftn      void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.SocialComponent::ReactToJealousEventHigh(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                          class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster)
      IL_001b:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster/BroadcastCallback::.ctor(object,
                                                                                                                                    native int)
      IL_0020:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                  class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcasterParams,
                                                                                                                  class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster/BroadcastCallback)
      IL_0025:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mReactToSocialBroadcaster
      IL_002a:  ldarg.0
      IL_002b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0030:  ldarg.0
      IL_0031:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0036:  ldarg.0
      IL_0037:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_003c:  ldc.i4.0
      IL_003d:  ceq
      IL_003f:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.SocialComponent::SendCheatingEvents(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                     bool)
      IL_0044:  ldarg.0
      IL_0045:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.NestedSocialInteraction`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated>::IsMaster
      IL_004a:  brfalse.s  IL_005c

      IL_004c:  ldarg.0
      IL_004d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_LinkedInteractionInstance()
      IL_0052:  isinst     Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna
      IL_0057:  callvirt   instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::StartJealousyBroadcaster()
      IL_005c:  ret
    } // end of method CuddleSeatedWooHooSauna::StartJealousyBroadcaster

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       66 (0x42)
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mReactToSocialBroadcaster
      IL_0006:  brfalse.s  IL_001a

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mReactToSocialBroadcaster
      IL_000e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster::Dispose()
      IL_0013:  ldarg.0
      IL_0014:  ldnull
      IL_0015:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mReactToSocialBroadcaster
      IL_001a:  ldarg.0
      IL_001b:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Beds.WooHoo/WooHooPrivacySituation Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSituation
      IL_0020:  brfalse.s  IL_0034

      IL_0022:  ldarg.0
      IL_0023:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Beds.WooHoo/WooHooPrivacySituation Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSituation
      IL_0028:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Situation::Exit()
      IL_002d:  ldarg.0
      IL_002e:  ldnull
      IL_002f:  stfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Beds.WooHoo/WooHooPrivacySituation Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mSituation
      IL_0034:  ldarg.0
      IL_0035:  ldc.i4.0
      IL_0036:  call       instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::EnableDisableCensor(bool)
      IL_003b:  ldarg.0
      IL_003c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.NestedCuddleInteraction::Cleanup()
      IL_0041:  ret
    } // end of method CuddleSeatedWooHooSauna::Cleanup

    .method public hidebysig instance void 
            EnableDisableCensor(bool enable) cil managed
    {
      // Code size       84 (0x54)
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0006:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.CAS.OutfitCategories [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CurrentOutfitCategory()
      IL_000b:  ldc.i4.1
      IL_000c:  beq.s      IL_002a

      IL_000e:  ldarg.1
      IL_000f:  brfalse.s  IL_001f

      IL_0011:  ldarg.0
      IL_0012:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0017:  ldc.i4.1
      IL_0018:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::EnableCensor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/CensorType)
      IL_001d:  br.s       IL_002a

      IL_001f:  ldarg.0
      IL_0020:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0025:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AutoEnableCensor()
      IL_002a:  ldarg.0
      IL_002b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0030:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.CAS.OutfitCategories [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CurrentOutfitCategory()
      IL_0035:  ldc.i4.1
      IL_0036:  beq.s      IL_0053

      IL_0038:  ldarg.1
      IL_0039:  brfalse.s  IL_0048

      IL_003b:  ldarg.0
      IL_003c:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0041:  ldc.i4.1
      IL_0042:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::EnableCensor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/CensorType)
      IL_0047:  ret

      IL_0048:  ldarg.0
      IL_0049:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_004e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AutoEnableCensor()
      IL_0053:  ret
    } // end of method CuddleSeatedWooHooSauna::EnableDisableCensor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  stfld      bool Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::mPrivacyFailed
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.NestedCuddleInteraction::.ctor()
      IL_000d:  ret
    } // end of method CuddleSeatedWooHooSauna::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       34 (0x22)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::.ctor(valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor)
      IL_0006:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::Singleton
      IL_000b:  ldc.i4.2
      IL_000c:  newobj     instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::.ctor(valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor)
      IL_0011:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::TryForBoySingleton
      IL_0016:  ldc.i4.1
      IL_0017:  newobj     instance void Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/Definition::.ctor(valuetype Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna/TryingFor)
      IL_001c:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::TryForGirlSingleton
      IL_0021:  ret
    } // end of method CuddleSeatedWooHooSauna::.cctor

  } // end of class CuddleSeatedWooHooSauna

  .class auto ansi nested public beforefieldinit StartSaunaSeatedCuddleA
         extends [Sims3GameplaySystems]Sims3.Gameplay.Interactions.SocialInteractionBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       90 (0x5a)
        .maxstack  4
        .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
        IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
        IL_000b:  brfalse.s  IL_000f

        IL_000d:  ldc.i4.0
        IL_000e:  ret

        IL_000f:  ldarg.2
        IL_0010:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
        IL_0015:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
        IL_001a:  brfalse.s  IL_001e

        IL_001c:  ldc.i4.0
        IL_001d:  ret

        IL_001e:  ldarg.1
        IL_001f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CurrentInteraction()
        IL_0024:  stloc.0
        IL_0025:  ldloc.0
        IL_0026:  brfalse.s  IL_0043

        IL_0028:  ldloc.0
        IL_0029:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Target()
        IL_002e:  ldarg.2
        IL_002f:  bne.un.s   IL_0043

        IL_0031:  ldloc.0
        IL_0032:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated
        IL_0037:  brtrue.s   IL_0041

        IL_0039:  ldloc.0
        IL_003a:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.NestedCuddleInteraction
        IL_003f:  brfalse.s  IL_0043

        IL_0041:  ldc.i4.0
        IL_0042:  ret

        IL_0043:  ldarg.1
        IL_0044:  ldarg.2
        IL_0045:  ldc.i4.0
        IL_0046:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::CanGetRomantic(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           bool)
        IL_004b:  brfalse.s  IL_0058

        IL_004d:  ldarg.1
        IL_004e:  ldarg.2
        IL_004f:  ldarg.s    greyedOutTooltipCallback
        IL_0051:  ldc.i4.1
        IL_0052:  call       bool Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA/Definition::CanCuddle(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                 class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&,
                                                                                                                 bool)
        IL_0057:  ret

        IL_0058:  ldc.i4.0
        IL_0059:  ret
      } // end of method Definition::Test

      .method public hidebysig static bool 
              CanCuddle(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                        class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim target,
                        class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback,
                        bool standingAllowed) cil managed
      {
        // Code size       226 (0xe2)
        .maxstack  4
        .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup V_0,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_1,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_2,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_3,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_4)
        IL_0000:  ldarg.0
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
        IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
        IL_000b:  brfalse.s  IL_000f

        IL_000d:  ldc.i4.0
        IL_000e:  ret

        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsAtHome()
        IL_0015:  brfalse.s  IL_0037

        IL_0017:  ldarg.0
        IL_0018:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_IsOutside()
        IL_001d:  brtrue.s   IL_0037

        IL_001f:  ldarg.1
        IL_0020:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsAtHome()
        IL_0025:  brtrue.s   IL_0037

        IL_0027:  ldarg.1
        IL_0028:  ldarg.0
        IL_0029:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LotHome()
        IL_002e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::IsGreetedOnLot(class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot)
        IL_0033:  brtrue.s   IL_0037

        IL_0035:  ldc.i4.0
        IL_0036:  ret

        IL_0037:  ldarg.1
        IL_0038:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsPerformingAService()
        IL_003d:  brfalse.s  IL_0049

        IL_003f:  ldarg.1
        IL_0040:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Situations.VisitSituation::IsSocializing(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_0045:  brtrue.s   IL_0049

        IL_0047:  ldc.i4.0
        IL_0048:  ret

        IL_0049:  ldarg.0
        IL_004a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
        IL_004f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::get_Container()
        IL_0054:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup
        IL_0059:  stloc.0
        IL_005a:  ldloc.0
        IL_005b:  brfalse    IL_00e0

        IL_0060:  ldloc.0
        IL_0061:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup::get_SeatingGroup()
        IL_0066:  ldarg.0
        IL_0067:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_006c:  stloc.1
        IL_006d:  ldloc.0
        IL_006e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup::get_SeatingGroup()
        IL_0073:  ldarg.1
        IL_0074:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_0079:  stloc.2
        IL_007a:  ldloc.1
        IL_007b:  brfalse.s  IL_00e0

        IL_007d:  ldloc.1
        IL_007e:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitStyle [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::SitStyle
        IL_0083:  ldc.i4.1
        IL_0084:  beq.s      IL_0088

        IL_0086:  ldc.i4.0
        IL_0087:  ret

        IL_0088:  ldloc.2
        IL_0089:  brfalse.s  IL_00cc

        IL_008b:  ldloc.2
        IL_008c:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitStyle [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::SitStyle
        IL_0091:  ldc.i4.1
        IL_0092:  beq.s      IL_0096

        IL_0094:  ldc.i4.0
        IL_0095:  ret

        IL_0096:  ldloc.1
        IL_0097:  ldloc.2
        IL_0098:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::IsAdjacentTo(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat)
        IL_009d:  brfalse.s  IL_00a1

        IL_009f:  ldc.i4.1
        IL_00a0:  ret

        IL_00a1:  ldloc.2
        IL_00a2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::GetEmptyAdjacentSeat()
        IL_00a7:  stloc.3
        IL_00a8:  ldloc.3
        IL_00a9:  brfalse.s  IL_00ad

        IL_00ab:  ldc.i4.1
        IL_00ac:  ret

        IL_00ad:  ldarg.2
        IL_00ae:  ldarg.0
        IL_00af:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_00b4:  ldstr      "Gameplay/Objects/Plumbing/HotTubBase:AdjacentSeatU"
        + "navailible"
        IL_00b9:  ldc.i4.0
        IL_00ba:  newarr     [mscorlib]System.Object
        IL_00bf:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(bool,
                                                                                                                string,
                                                                                                                object[])
        IL_00c4:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_00c9:  stind.ref
        IL_00ca:  ldc.i4.0
        IL_00cb:  ret

        IL_00cc:  ldarg.3
        IL_00cd:  brfalse.s  IL_00e0

        IL_00cf:  ldloc.1
        IL_00d0:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::GetEmptyAdjacentSeat()
        IL_00d5:  stloc.s    V_4
        IL_00d7:  ldloc.s    V_4
        IL_00d9:  ldnull
        IL_00da:  ceq
        IL_00dc:  ldc.i4.0
        IL_00dd:  ceq
        IL_00df:  ret

        IL_00e0:  ldc.i4.0
        IL_00e1:  ret
      } // end of method Definition::CanCuddle

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       91 (0x5b)
        .maxstack  3
        .locals init (bool V_0)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
        IL_0006:  ldc.i4     0x40f3d23
        IL_000b:  ldnull
        IL_000c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
        IL_0011:  stloc.0
        IL_0012:  ldloc.0
        IL_0013:  brfalse.s  IL_0044

        IL_0015:  ldarg.1
        IL_0016:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
        IL_001b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::get_Container()
        IL_0020:  ldarg.2
        IL_0021:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
        IL_0026:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::get_Container()
        IL_002b:  beq.s      IL_0044

        IL_002d:  ldarg.1
        IL_002e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0033:  ldstr      "CuddleInvite"
        IL_0038:  ldc.i4.0
        IL_0039:  newarr     [mscorlib]System.Object
        IL_003e:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_0043:  ret

        IL_0044:  ldarg.1
        IL_0045:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_004a:  ldstr      "Cuddle"
        IL_004f:  ldc.i4.0
        IL_0050:  newarr     [mscorlib]System.Object
        IL_0055:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_005a:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .field public static int32 kTargetLookAtPriority
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static int32 kHowLongToWaitForOtherToArrive
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kMinDistanceToMoveAwayWhenRejected
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 37 54 68 65 20 6D 69 6E 69 6D 75 6D 20 64   // ..7The minimum d
                                                                                              69 73 74 61 6E 63 65 20 61 20 73 69 6D 20 77 69   // istance a sim wi
                                                                                              6C 6C 20 6D 6F 76 65 20 61 77 61 79 20 77 68 65   // ll move away whe
                                                                                              6E 20 72 65 6A 65 63 74 65 64 00 00 )             // n rejected..
    .field public static float32 kMaxDistanceToMoveAwayWhenRejected
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 37 54 68 65 20 6D 61 78 69 6D 75 6D 20 64   // ..7The maximum d
                                                                                              69 73 74 61 6E 63 65 20 61 20 73 69 6D 20 77 69   // istance a sim wi
                                                                                              6C 6C 20 6D 6F 76 65 20 61 77 61 79 20 77 68 65   // ll move away whe
                                                                                              6E 20 72 65 6A 65 63 74 65 64 00 00 )             // n rejected..
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster mReactToSocialBroadcaster
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       1566 (0x61e)
      .maxstack  9
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_2,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_4,
               class Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA V_5,
               class Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB V_6,
               bool V_7,
               bool V_8,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority V_9,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated V_10,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated V_11,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames[] V_12)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_000c:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GreetSimOnMyLotIfPossible(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0011:  ldarg.0
      IL_0012:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0017:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::EnsureLivingChairPosture(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_001c:  ldarg.0
      IL_001d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0022:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_0027:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::get_Container()
      IL_002c:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup
      IL_0031:  stloc.0
      IL_0032:  ldloc.0
      IL_0033:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup::get_SeatingGroup()
      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_003e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0043:  stloc.1
      IL_0044:  ldloc.0
      IL_0045:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup::get_SeatingGroup()
      IL_004a:  ldarg.0
      IL_004b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0050:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0055:  stloc.2
      IL_0056:  ldarg.0
      IL_0057:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SafeToSync()
      IL_005c:  brtrue.s   IL_0060

      IL_005e:  ldc.i4.0
      IL_005f:  ret

      IL_0060:  ldarg.0
      IL_0061:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0066:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_006b:  ldarg.0
      IL_006c:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0071:  ldsfld     int32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kTargetLookAtPriority
      IL_0076:  ldc.i4.1
      IL_0077:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::SetInteractionLookAt(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                                                int32,
                                                                                                                                valuetype [SimIFace]Sims3.SimIFace.LookAtJointFilter)
      IL_007c:  pop
      IL_007d:  ldloc.2
      IL_007e:  brfalse    IL_0115

      IL_0083:  ldloc.2
      IL_0084:  ldloc.1
      IL_0085:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::IsAdjacentTo(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat)
      IL_008a:  brtrue     IL_0115

      IL_008f:  ldloc.2
      IL_0090:  ldarg.0
      IL_0091:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0096:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::GetEmptyAdjacentSeat(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_009b:  stloc.3
      IL_009c:  ldloc.3
      IL_009d:  brfalse.s  IL_0113

      IL_009f:  ldloc.1
      IL_00a0:  ldloc.3
      IL_00a1:  ldarg.0
      IL_00a2:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_00a7:  ldarg.0
      IL_00a8:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_00ad:  ldarg.0
      IL_00ae:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_00b3:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::CreateChangeSeatInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat,
                                                                                                                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                       bool,
                                                                                                                                                                                                       bool)
      IL_00b8:  stloc.s    V_4
      IL_00ba:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::Singleton
      IL_00bf:  ldarg.0
      IL_00c0:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_00c5:  ldarg.0
      IL_00c6:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_00cb:  ldarg.0
      IL_00cc:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_00d1:  ldarg.0
      IL_00d2:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_00d7:  ldarg.0
      IL_00d8:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_00dd:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_00e2:  isinst     Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA
      IL_00e7:  stloc.s    V_5
      IL_00e9:  ldarg.0
      IL_00ea:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_00ef:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_00f4:  ldloc.s    V_4
      IL_00f6:  ldc.i4.1
      IL_00f7:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_00fc:  pop
      IL_00fd:  ldarg.0
      IL_00fe:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0103:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0108:  ldloc.s    V_5
      IL_010a:  ldc.i4.1
      IL_010b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0110:  pop
      IL_0111:  ldc.i4.1
      IL_0112:  ret

      IL_0113:  ldc.i4.0
      IL_0114:  ret

      IL_0115:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::Singleton
      IL_011a:  ldarg.0
      IL_011b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0120:  ldarg.0
      IL_0121:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0126:  ldarg.0
      IL_0127:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_012c:  ldc.i4.0
      IL_012d:  ldarg.0
      IL_012e:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_0133:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0138:  isinst     Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB
      IL_013d:  stloc.s    V_6
      IL_013f:  ldloc.s    V_6
      IL_0141:  ldloc.1
      IL_0142:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatX
      IL_0147:  ldloc.s    V_6
      IL_0149:  ldloc.2
      IL_014a:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatY
      IL_014f:  ldarg.0
      IL_0150:  ldloc.s    V_6
      IL_0152:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::set_LinkedInteractionInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0157:  ldarg.0
      IL_0158:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_015d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0162:  ldloc.s    V_6
      IL_0164:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::Add(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0169:  pop
      IL_016a:  ldarg.0
      IL_016b:  ldarg.0
      IL_016c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0171:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcasterParams [Sims3GameplaySystems]Sims3.Gameplay.Socializing.Conversation::get_ReactToSocialParams()
      IL_0176:  ldnull
      IL_0177:  ldftn      void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.SocialComponent::ReactToJealousEventMedium(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster)
      IL_017d:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster/BroadcastCallback::.ctor(object,
                                                                                                                                    native int)
      IL_0182:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                  class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcasterParams,
                                                                                                                  class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster/BroadcastCallback)
      IL_0187:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::mReactToSocialBroadcaster
      IL_018c:  ldarg.0
      IL_018d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0192:  ldarg.0
      IL_0193:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0198:  ldarg.0
      IL_0199:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_019e:  ldc.i4.0
      IL_019f:  ceq
      IL_01a1:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.SocialComponent::SendCheatingEvents(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                     bool)
      IL_01a6:  ldarg.0
      IL_01a7:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_01ac:  ldc.i4.1
      IL_01ad:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncRole [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SynchronizationRole
      IL_01b2:  ldarg.0
      IL_01b3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_01b8:  ldc.i4.1
      IL_01b9:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SynchronizationLevel
      IL_01be:  ldarg.0
      IL_01bf:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_01c4:  ldarg.0
      IL_01c5:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_01ca:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SynchronizationTarget
      IL_01cf:  ldarg.0
      IL_01d0:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_01d5:  ldc.i4.0
      IL_01d6:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SynchronizationLevel
      IL_01db:  ldarg.0
      IL_01dc:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_01e1:  ldarg.0
      IL_01e2:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_01e7:  ldc.i4.1
      IL_01e8:  ldsfld     int32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kHowLongToWaitForOtherToArrive
      IL_01ed:  conv.r4
      IL_01ee:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::WaitForSynchronizationLevelWithSim(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel,
                                                                                                                             float32)
      IL_01f3:  brtrue.s   IL_01f7

      IL_01f5:  ldc.i4.0
      IL_01f6:  ret

      IL_01f7:  ldarg.0
      IL_01f8:  ldstr      "Cuddle"
      IL_01fd:  call       instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::StartSocial(string)
      IL_0202:  pop
      IL_0203:  ldloc.s    V_6
      IL_0205:  ldarg.0
      IL_0206:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_020b:  stfld      bool Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::Reject
      IL_0210:  ldarg.0
      IL_0211:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0216:  ldarg.0
      IL_0217:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_021c:  ldc.i4.2
      IL_021d:  ldsfld     int32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kHowLongToWaitForOtherToArrive
      IL_0222:  conv.r4
      IL_0223:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::WaitForSynchronizationLevelWithSim(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel,
                                                                                                                             float32)
      IL_0228:  brtrue.s   IL_022c

      IL_022a:  ldc.i4.0
      IL_022b:  ret

      IL_022c:  ldarg.0
      IL_022d:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_0232:  brfalse    IL_0476

      IL_0237:  ldarg.0
      IL_0238:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_023d:  ldarg.0
      IL_023e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0243:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::SimsAreAdjacent(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0248:  brfalse    IL_03c4

      IL_024d:  ldarg.0
      IL_024e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0253:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::EnsureLivingChairPosture(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0258:  ldloc.2
      IL_0259:  ldloc.1
      IL_025a:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::Right
      IL_025f:  ceq
      IL_0261:  stloc.s    V_7
      IL_0263:  ldarg.0
      IL_0264:  ldstr      "SeatingCuddle"
      IL_0269:  ldstr      "EnterSitting"
      IL_026e:  ldstr      "x"
      IL_0273:  ldstr      "y"
      IL_0278:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_027d:  ldarg.0
      IL_027e:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0283:  ldstr      "y"
      IL_0288:  ldstr      "EnterSitting"
      IL_028d:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_0292:  ldarg.0
      IL_0293:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0298:  ldstr      "SeatX"
      IL_029d:  ldstr      "EnterSitting"
      IL_02a2:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_02a7:  ldarg.0
      IL_02a8:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_02ad:  ldstr      "SeatY"
      IL_02b2:  ldstr      "EnterSitting"
      IL_02b7:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_02bc:  ldarg.0
      IL_02bd:  ldstr      "SeatX"
      IL_02c2:  ldloc.1
      IL_02c3:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::get_Host()
      IL_02c8:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_02cd:  ldarg.0
      IL_02ce:  ldstr      "SuffixX"
      IL_02d3:  ldloc.1
      IL_02d4:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::IKSuffix
      IL_02d9:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             string)
      IL_02de:  ldarg.0
      IL_02df:  ldstr      "SeatY"
      IL_02e4:  ldloc.2
      IL_02e5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::get_Host()
      IL_02ea:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_02ef:  ldarg.0
      IL_02f0:  ldstr      "SuffixY"
      IL_02f5:  ldloc.2
      IL_02f6:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::IKSuffix
      IL_02fb:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             string)
      IL_0300:  ldarg.0
      IL_0301:  ldstr      "surface"
      IL_0306:  ldloc.1
      IL_0307:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::get_Host()
      IL_030c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetActor(string,
                                                                                                                         class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0311:  ldarg.0
      IL_0312:  ldstr      "IsMirrored"
      IL_0317:  ldloc.s    V_7
      IL_0319:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             bool)
      IL_031e:  ldloc.1
      IL_031f:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IHotTubSeat
      IL_0324:  ldnull
      IL_0325:  cgt.un
      IL_0327:  stloc.s    V_8
      IL_0329:  ldarg.0
      IL_032a:  ldstr      "IsHottub"
      IL_032f:  ldloc.s    V_8
      IL_0331:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::SetParameter(string,
                                                                                                                             bool)
      IL_0336:  ldarg.0
      IL_0337:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_033c:  ldc.i4.0
      IL_033d:  ldstr      "x"
      IL_0342:  ldstr      "CuddleReject"
      IL_0347:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(bool,
                                                                                                   string,
                                                                                                   string)
      IL_034c:  ldarg.0
      IL_034d:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0352:  ldc.i4.0
      IL_0353:  ldstr      "SeatX"
      IL_0358:  ldstr      "CuddleReject"
      IL_035d:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(bool,
                                                                                                   string,
                                                                                                   string)
      IL_0362:  ldarg.0
      IL_0363:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0368:  ldc.i4.0
      IL_0369:  ldstr      "SeatY"
      IL_036e:  ldstr      "CuddleReject"
      IL_0373:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(bool,
                                                                                                   string,
                                                                                                   string)
      IL_0378:  ldarg.0
      IL_0379:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_037e:  ldc.i4.1
      IL_037f:  ldstr      "y"
      IL_0384:  ldstr      "CuddleReject"
      IL_0389:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(bool,
                                                                                                   string,
                                                                                                   string)
      IL_038e:  ldarg.0
      IL_038f:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0394:  ldnull
      IL_0395:  ldstr      "ExitSitting"
      IL_039a:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_039f:  ldarg.0
      IL_03a0:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_03a5:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kMinDistanceToMoveAwayWhenRejected
      IL_03aa:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kMaxDistanceToMoveAwayWhenRejected
      IL_03af:  ldc.i4.1
      IL_03b0:  ldc.i4.0
      IL_03b1:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority::.ctor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriorityLevel)
      IL_03b6:  ldc.i4.0
      IL_03b7:  ldc.i4.1
      IL_03b8:  ldc.i4.1
      IL_03b9:  ldc.i4.0
      IL_03ba:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteAway(float32,
                                                                                                    float32,
                                                                                                    bool,
                                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                    bool,
                                                                                                    bool,
                                                                                                    bool,
                                                                                                    valuetype [SimIFace]Sims3.SimIFace.RouteDistancePreference)
      IL_03bf:  br         IL_0474

      IL_03c4:  ldarg.0
      IL_03c5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_03ca:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_03cf:  ldc.i8     0xb82d0015b92943d0
      IL_03d8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_03dd:  brfalse.s  IL_03f3

      IL_03df:  ldarg.0
      IL_03e0:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_03e5:  ldc.i4.s   42
      IL_03e7:  ldc.i4.1
      IL_03e8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_03ed:  pop
      IL_03ee:  br         IL_0474

      IL_03f3:  ldarg.0
      IL_03f4:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_03f9:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_03fe:  ldc.i8     0xb82d0015b9294520
      IL_0407:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_040c:  brfalse.s  IL_041f

      IL_040e:  ldarg.0
      IL_040f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0414:  ldc.i4.s   40
      IL_0416:  ldc.i4.1
      IL_0417:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_041c:  pop
      IL_041d:  br.s       IL_0474

      IL_041f:  ldarg.0
      IL_0420:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0425:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_042a:  ldc.i4.2
      IL_042b:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames
      IL_0430:  stloc.s    V_12
      IL_0432:  ldloc.s    V_12
      IL_0434:  ldc.i4.0
      IL_0435:  ldc.i8     0xb82d0015b92942d0
      IL_043e:  stelem.i8
      IL_043f:  ldloc.s    V_12
      IL_0441:  ldc.i4.1
      IL_0442:  ldc.i8     0x4c800171bd46a40
      IL_044b:  stelem.i8
      IL_044c:  ldloc.s    V_12
      IL_044e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasAnyElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames[])
      IL_0453:  brfalse.s  IL_0466

      IL_0455:  ldarg.0
      IL_0456:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_045b:  ldc.i4.s   65
      IL_045d:  ldc.i4.1
      IL_045e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_0463:  pop
      IL_0464:  br.s       IL_0474

      IL_0466:  ldarg.0
      IL_0467:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_046c:  ldc.i4.3
      IL_046d:  ldc.i4.1
      IL_046e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_0473:  pop
      IL_0474:  ldc.i4.0
      IL_0475:  ret

      IL_0476:  ldarg.0
      IL_0477:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_047c:  ldc.i4.4
      IL_047d:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SynchronizationLevel
      IL_0482:  ldarg.0
      IL_0483:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0488:  ldarg.0
      IL_0489:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_048e:  ldc.i4.4
      IL_048f:  ldsfld     int32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kHowLongToWaitForOtherToArrive
      IL_0494:  conv.r4
      IL_0495:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::WaitForSynchronizationLevelWithSim(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel,
                                                                                                                             float32)
      IL_049a:  brtrue.s   IL_049e

      IL_049c:  ldc.i4.0
      IL_049d:  ret

      IL_049e:  ldarg.0
      IL_049f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_04a4:  ldarg.0
      IL_04a5:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_04aa:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::SimsAreAdjacent(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_04af:  brtrue.s   IL_04d2

      IL_04b1:  ldarg.0
      IL_04b2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_04b7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_04bc:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::CurrentStateMachine
      IL_04c1:  ldstr      "x"
      IL_04c6:  ldstr      "routeFail"
      IL_04cb:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_04d0:  ldc.i4.0
      IL_04d1:  ret

      IL_04d2:  ldloc.2
      IL_04d3:  brtrue.s   IL_04e7

      IL_04d5:  ldloc.0
      IL_04d6:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup::get_SeatingGroup()
      IL_04db:  ldarg.0
      IL_04dc:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_04e1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_04e6:  stloc.2
      IL_04e7:  ldloca.s   V_9
      IL_04e9:  ldc.i4.0
      IL_04ea:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority::.ctor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriorityLevel)
      IL_04ef:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated::Singleton
      IL_04f4:  ldarg.0
      IL_04f5:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_04fa:  ldarg.0
      IL_04fb:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0500:  ldloc.s    V_9
      IL_0502:  ldc.i4.0
      IL_0503:  ldarg.0
      IL_0504:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_0509:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_050e:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated
      IL_0513:  stloc.s    V_10
      IL_0515:  ldloc.s    V_10
      IL_0517:  ldarg.0
      IL_0518:  callvirt   instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::AdoptSocialEffect(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<!0,!1>)
      IL_051d:  ldloc.s    V_10
      IL_051f:  ldloc.1
      IL_0520:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated::SeatX
      IL_0525:  ldloc.s    V_10
      IL_0527:  ldloc.2
      IL_0528:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated::SeatY
      IL_052d:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated::Singleton
      IL_0532:  ldarg.0
      IL_0533:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0538:  ldarg.0
      IL_0539:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_053e:  ldloc.s    V_9
      IL_0540:  ldc.i4.0
      IL_0541:  ldarg.0
      IL_0542:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_0547:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_054c:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated
      IL_0551:  stloc.s    V_11
      IL_0553:  ldloc.s    V_11
      IL_0555:  ldloc.1
      IL_0556:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated::SeatX
      IL_055b:  ldloc.s    V_11
      IL_055d:  ldloc.2
      IL_055e:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.CuddleSeated::SeatY
      IL_0563:  ldloc.s    V_10
      IL_0565:  ldloc.s    V_11
      IL_0567:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::set_LinkedInteractionInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_056c:  ldc.i4.s   10
      IL_056e:  ldarg.0
      IL_056f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0574:  ldarg.0
      IL_0575:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_057a:  ldstr      "Cuddle"
      IL_057f:  ldc.i4.0
      IL_0580:  ldarg.0
      IL_0581:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_0586:  ldc.i4.0
      IL_0587:  ceq
      IL_0589:  ldc.i4.0
      IL_058a:  ldc.i4.0
      IL_058b:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.SocialEvent::.ctor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTypeId,
                                                                                                             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             string,
                                                                                                             bool,
                                                                                                             bool,
                                                                                                             bool,
                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Socializing.CommodityTypes)
      IL_0590:  call       class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTracker::SendEvent(class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event)
      IL_0595:  pop
      IL_0596:  ldc.i4.s   10
      IL_0598:  ldarg.0
      IL_0599:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_059e:  ldarg.0
      IL_059f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_05a4:  ldstr      "Cuddle"
      IL_05a9:  ldc.i4.1
      IL_05aa:  ldarg.0
      IL_05ab:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_05b0:  ldc.i4.0
      IL_05b1:  ceq
      IL_05b3:  ldc.i4.0
      IL_05b4:  ldc.i4.0
      IL_05b5:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.SocialEvent::.ctor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTypeId,
                                                                                                             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             string,
                                                                                                             bool,
                                                                                                             bool,
                                                                                                             bool,
                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Socializing.CommodityTypes)
      IL_05ba:  call       class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTracker::SendEvent(class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event)
      IL_05bf:  pop
      IL_05c0:  ldc.i4     0x122
      IL_05c5:  ldarg.0
      IL_05c6:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_05cb:  ldarg.0
      IL_05cc:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_05d1:  call       class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTracker::SendEvent(valuetype [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTypeId,
                                                                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_05d6:  pop
      IL_05d7:  ldc.i4     0x122
      IL_05dc:  ldarg.0
      IL_05dd:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_05e2:  ldarg.0
      IL_05e3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_05e8:  call       class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTracker::SendEvent(valuetype [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTypeId,
                                                                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_05ed:  pop
      IL_05ee:  ldarg.0
      IL_05ef:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_05f4:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_05f9:  ldloc.s    V_10
      IL_05fb:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::Add(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0600:  pop
      IL_0601:  ldarg.0
      IL_0602:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0607:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_060c:  ldloc.s    V_11
      IL_060e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::Add(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0613:  pop
      IL_0614:  ldarg.0
      IL_0615:  ldfld      bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Rejected
      IL_061a:  ldc.i4.0
      IL_061b:  ceq
      IL_061d:  ret
    } // end of method StartSaunaSeatedCuddleA::Run

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       33 (0x21)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::mReactToSocialBroadcaster
      IL_0006:  brfalse.s  IL_001a

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::mReactToSocialBroadcaster
      IL_000e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster::Dispose()
      IL_0013:  ldarg.0
      IL_0014:  ldnull
      IL_0015:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.ReactionBroadcaster Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::mReactToSocialBroadcaster
      IL_001a:  ldarg.0
      IL_001b:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_0020:  ret
    } // end of method StartSaunaSeatedCuddleA::Cleanup

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.SocialInteractionBase::.ctor()
      IL_0006:  ret
    } // end of method StartSaunaSeatedCuddleA::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::Singleton
      IL_000a:  call       int32 [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtPriorityForSocializingSim()
      IL_000f:  stsfld     int32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kTargetLookAtPriority
      IL_0014:  ldc.i4.s   50
      IL_0016:  stsfld     int32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kHowLongToWaitForOtherToArrive
      IL_001b:  ldc.r4     3.
      IL_0020:  stsfld     float32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kMinDistanceToMoveAwayWhenRejected
      IL_0025:  ldc.r4     7.
      IL_002a:  stsfld     float32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kMaxDistanceToMoveAwayWhenRejected
      IL_002f:  ret
    } // end of method StartSaunaSeatedCuddleA::.cctor

  } // end of class StartSaunaSeatedCuddleA

  .class auto ansi nested public beforefieldinit StartSaunaSeatedCuddleB
         extends [Sims3GameplaySystems]Sims3.Gameplay.Interactions.SocialInteractionBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       2 (0x2)
        .maxstack  8
        IL_0000:  ldc.i4.1
        IL_0001:  ret
      } // end of method Definition::Test

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .field public static int32 kTargetLookAtPriority
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat SeatX
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat SeatY
    .field public bool Reject
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       472 (0x1d8)
      .maxstack  9
      .locals init (class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData> V_0,
               valuetype [SimIFace]Sims3.SimIFace.Slot V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData V_2,
               class Sims3.Store.Objects.SaunaClassic V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_4,
               bool V_5)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StartSync(bool)
      IL_0007:  brtrue.s   IL_000b

      IL_0009:  ldc.i4.0
      IL_000a:  ret

      IL_000b:  ldarg.0
      IL_000c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0011:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0016:  ldarg.0
      IL_0017:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_001c:  ldsfld     int32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::kTargetLookAtPriority
      IL_0021:  ldc.i4.1
      IL_0022:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::SetInteractionLookAt(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                                                int32,
                                                                                                                                valuetype [SimIFace]Sims3.SimIFace.LookAtJointFilter)
      IL_0027:  pop
      IL_0028:  ldarg.0
      IL_0029:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatY
      IL_002e:  brfalse.s  IL_004a

      IL_0030:  ldarg.0
      IL_0031:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatY
      IL_0036:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData::get_ContainedSim()
      IL_003b:  ldarg.0
      IL_003c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0041:  beq.s      IL_004a

      IL_0043:  ldarg.0
      IL_0044:  ldnull
      IL_0045:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatY
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatY
      IL_0050:  brtrue     IL_01b1

      IL_0055:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData>::.ctor()
      IL_005a:  stloc.0
      IL_005b:  ldarg.0
      IL_005c:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatX
      IL_0061:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::Left
      IL_0066:  ldarg.0
      IL_0067:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_006c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData::CanBeUsedBy(class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData,
                                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0071:  brfalse.s  IL_0084

      IL_0073:  ldloc.0
      IL_0074:  ldarg.0
      IL_0075:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatX
      IL_007a:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::Left
      IL_007f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData>::Add(!0)
      IL_0084:  ldarg.0
      IL_0085:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatX
      IL_008a:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::Right
      IL_008f:  ldarg.0
      IL_0090:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0095:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData::CanBeUsedBy(class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData,
                                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_009a:  brfalse.s  IL_00ad

      IL_009c:  ldloc.0
      IL_009d:  ldarg.0
      IL_009e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatX
      IL_00a3:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::Right
      IL_00a8:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData>::Add(!0)
      IL_00ad:  ldarg.0
      IL_00ae:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_00b3:  ldloc.0
      IL_00b4:  ldnull
      IL_00b5:  ldloca.s   V_1
      IL_00b7:  ldloca.s   V_2
      IL_00b9:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToUnusedPart(class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData>,
                                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData,
                                                                                                            valuetype [SimIFace]Sims3.SimIFace.Slot&,
                                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData&)
      IL_00be:  brtrue.s   IL_00c2

      IL_00c0:  ldc.i4.0
      IL_00c1:  ret

      IL_00c2:  ldarg.0
      IL_00c3:  ldfld      bool Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::Reject
      IL_00c8:  brfalse.s  IL_012c

      IL_00ca:  ldarg.0
      IL_00cb:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_00d0:  ldarg.0
      IL_00d1:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_00d6:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Position()
      IL_00db:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteTurnToFace(valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_00e0:  pop
      IL_00e1:  ldarg.0
      IL_00e2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_00e7:  ldc.i4.s   76
      IL_00e9:  ldc.i4.2
      IL_00ea:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_00ef:  pop
      IL_00f0:  ldarg.0
      IL_00f1:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_00f6:  ldc.i4.2
      IL_00f7:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SynchronizationLevel
      IL_00fc:  ldarg.0
      IL_00fd:  ldc.i4.0
      IL_00fe:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::FinishLinkedInteraction(bool)
      IL_0103:  ldarg.0
      IL_0104:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::WaitForSyncComplete()
      IL_0109:  pop
      IL_010a:  ldarg.0
      IL_010b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0110:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kMinDistanceToMoveAwayWhenRejected
      IL_0115:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::kMaxDistanceToMoveAwayWhenRejected
      IL_011a:  ldc.i4.1
      IL_011b:  ldc.i4.0
      IL_011c:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority::.ctor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriorityLevel)
      IL_0121:  ldc.i4.0
      IL_0122:  ldc.i4.1
      IL_0123:  ldc.i4.1
      IL_0124:  ldc.i4.0
      IL_0125:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteAway(float32,
                                                                                                    float32,
                                                                                                    bool,
                                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                    bool,
                                                                                                    bool,
                                                                                                    bool,
                                                                                                    valuetype [SimIFace]Sims3.SimIFace.RouteDistancePreference)
      IL_012a:  ldc.i4.1
      IL_012b:  ret

      IL_012c:  ldnull
      IL_012d:  stloc.3
      IL_012e:  ldarg.0
      IL_012f:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatX
      IL_0134:  brfalse.s  IL_0147

      IL_0136:  ldarg.0
      IL_0137:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatX
      IL_013c:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData::Container
      IL_0141:  isinst     Sims3.Store.Objects.SaunaClassic
      IL_0146:  stloc.3
      IL_0147:  ldloc.3
      IL_0148:  brtrue.s   IL_0163

      IL_014a:  ldarg.0
      IL_014b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatY
      IL_0150:  brfalse.s  IL_0163

      IL_0152:  ldarg.0
      IL_0153:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::SeatY
      IL_0158:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData::Container
      IL_015d:  isinst     Sims3.Store.Objects.SaunaClassic
      IL_0162:  stloc.3
      IL_0163:  ldloc.3
      IL_0164:  brtrue.s   IL_0168

      IL_0166:  ldc.i4.0
      IL_0167:  ret

      IL_0168:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.InteractionsShared.Sit::Singleton
      IL_016d:  ldloc.3
      IL_016e:  ldarg.0
      IL_016f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0174:  ldarg.0
      IL_0175:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_017a:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_017f:  ldc.i4.0
      IL_0180:  ldc.i4.0
      IL_0181:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0186:  stloc.s    V_4
      IL_0188:  ldloc.s    V_4
      IL_018a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::RunInteraction()
      IL_018f:  stloc.s    V_5
      IL_0191:  ldloc.s    V_5
      IL_0193:  brtrue.s   IL_01b1

      IL_0195:  ldarg.0
      IL_0196:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_019b:  ldc.i4.2
      IL_019c:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SynchronizationLevel
      IL_01a1:  ldarg.0
      IL_01a2:  ldc.i4.0
      IL_01a3:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::FinishLinkedInteraction(bool)
      IL_01a8:  ldarg.0
      IL_01a9:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::WaitForSyncComplete()
      IL_01ae:  pop
      IL_01af:  ldc.i4.0
      IL_01b0:  ret

      IL_01b1:  ldarg.0
      IL_01b2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_01b7:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::EnsureLivingChairPosture(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_01bc:  ldarg.0
      IL_01bd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_01c2:  ldc.i4.4
      IL_01c3:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SyncLevel [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SynchronizationLevel
      IL_01c8:  ldarg.0
      IL_01c9:  ldc.i4.0
      IL_01ca:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::FinishLinkedInteraction(bool)
      IL_01cf:  ldarg.0
      IL_01d0:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::WaitForSyncComplete()
      IL_01d5:  pop
      IL_01d6:  ldc.i4.1
      IL_01d7:  ret
    } // end of method StartSaunaSeatedCuddleB::Run

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.SocialInteractionBase::.ctor()
      IL_0006:  ret
    } // end of method StartSaunaSeatedCuddleB::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::Singleton
      IL_000a:  call       int32 [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtPriorityForSocializingSim()
      IL_000f:  stsfld     int32 Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleB::kTargetLookAtPriority
      IL_0014:  ret
    } // end of method StartSaunaSeatedCuddleB::.cctor

  } // end of class StartSaunaSeatedCuddleB

  .class auto ansi nested public beforefieldinit ToggleWaterfall
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.ImmediateInteraction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/ToggleWaterfall>
    {
      .custom instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.DoesntRequireTuningAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.SaunaClassic target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       2 (0x2)
        .maxstack  8
        IL_0000:  ldc.i4.1
        IL_0001:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.SaunaClassic target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       54 (0x36)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  ldfld      bool Sims3.Store.Objects.SaunaClassic::mWaterfallFlowing
        IL_0006:  brfalse.s  IL_001f

        IL_0008:  ldarg.1
        IL_0009:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_000e:  ldstr      "WaterfallOff"
        IL_0013:  ldc.i4.0
        IL_0014:  newarr     [mscorlib]System.Object
        IL_0019:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_001e:  ret

        IL_001f:  ldarg.1
        IL_0020:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0025:  ldstr      "WaterfallOn"
        IL_002a:  ldc.i4.0
        IL_002b:  newarr     [mscorlib]System.Object
        IL_0030:  call       string Sims3.Store.Objects.SaunaClassic::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
        IL_0035:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic,class Sims3.Store.Objects.SaunaClassic/ToggleWaterfall>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       38 (0x26)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_000c:  ldfld      bool Sims3.Store.Objects.SaunaClassic::mWaterfallFlowing
      IL_0011:  ldc.i4.0
      IL_0012:  ceq
      IL_0014:  stfld      bool Sims3.Store.Objects.SaunaClassic::mWaterfallFlowing
      IL_0019:  ldarg.0
      IL_001a:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::Target
      IL_001f:  callvirt   instance void Sims3.Store.Objects.SaunaClassic::SetWaterfallMaterial()
      IL_0024:  ldc.i4.1
      IL_0025:  ret
    } // end of method ToggleWaterfall::Run

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.ImmediateInteraction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic>::.ctor()
      IL_0006:  ret
    } // end of method ToggleWaterfall::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.SaunaClassic/ToggleWaterfall/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/ToggleWaterfall::Singleton
      IL_000a:  ret
    } // end of method ToggleWaterfall::.cctor

  } // end of class ToggleWaterfall

  .class auto ansi nested public SteamedVeggies
         extends [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject
         implements [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable,
                    [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.IBaseCarryable,
                    [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.IHasRouteRadius,
                    [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                    [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject,
                    [SimIFace]Sims3.SimIFace.IScriptLogic,
                    [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                    [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IObjectUI,
                    [SimIFace]Sims3.SimIFace.CustomContent.IExportableContent,
                    [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryableHasProductVersion
  {
    .custom instance void [SimIFace]Sims3.SimIFace.RuntimeExportAttribute::.ctor() = ( 01 00 00 00 ) 
    .class auto ansi nested public beforefieldinit InteractionProxy
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/SteamedVeggies>
    {
      .class auto ansi nested public beforefieldinit Definition
             extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/SteamedVeggies,class Sims3.Store.Objects.SaunaClassic/SteamedVeggies/InteractionProxy>
      {
        .custom instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.DoesntRequireTuningAttribute::.ctor() = ( 01 00 00 00 ) 
        .method public hidebysig virtual 
                instance string  GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                                    class Sims3.Store.Objects.SaunaClassic/SteamedVeggies target,
                                                    class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
        {
          // Code size       6 (0x6)
          .maxstack  8
          IL_0000:  ldstr      "Never Displayed: InteractionProxy"
          IL_0005:  ret
        } // end of method Definition::GetInteractionName

        .method public hidebysig virtual 
                instance bool  Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Actor,
                                    class Sims3.Store.Objects.SaunaClassic/SteamedVeggies Target,
                                    bool isAutonomous,
                                    class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
        {
          // Code size       2 (0x2)
          .maxstack  8
          IL_0000:  ldc.i4.0
          IL_0001:  ret
        } // end of method Definition::Test

        .method public hidebysig specialname rtspecialname 
                instance void  .ctor() cil managed
        {
          // Code size       7 (0x7)
          .maxstack  8
          IL_0000:  ldarg.0
          IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/SteamedVeggies,class Sims3.Store.Objects.SaunaClassic/SteamedVeggies/InteractionProxy>::.ctor()
          IL_0006:  ret
        } // end of method Definition::.ctor

      } // end of class Definition

      .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
      .method public hidebysig virtual instance bool 
              Run() cil managed
      {
        // Code size       11 (0xb)
        .maxstack  8
        IL_0000:  ldstr      "This interaction should never be run"
        IL_0005:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor(string)
        IL_000a:  throw
      } // end of method InteractionProxy::Run

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.SaunaClassic/SteamedVeggies>::.ctor()
        IL_0006:  ret
      } // end of method InteractionProxy::.ctor

      .method public hidebysig specialname rtspecialname static 
              void  .cctor() cil managed
      {
        // Code size       11 (0xb)
        .maxstack  8
        IL_0000:  newobj     instance void Sims3.Store.Objects.SaunaClassic/SteamedVeggies/InteractionProxy/Definition::.ctor()
        IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/SteamedVeggies/InteractionProxy::Singleton
        IL_000a:  ret
      } // end of method InteractionProxy::.cctor

    } // end of class InteractionProxy

    .field public static literal string kPackageName = "SteamedVeggies"
    .field public static literal string kScriptClass = "Sims3.Store.Objects.SaunaClassic+SteamedVeggies"
    .field public static string sLocalizationKey
    .method public hidebysig static string 
            LocalizeString(string name,
                           object[] parameters) cil managed
    {
      .param [2]
      .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldsfld     string Sims3.Store.Objects.SaunaClassic/SteamedVeggies::sLocalizationKey
      IL_0005:  ldstr      ":"
      IL_000a:  ldarg.0
      IL_000b:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_0010:  ldarg.1
      IL_0011:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                              object[])
      IL_0016:  ret
    } // end of method SteamedVeggies::LocalizeString

    .method public hidebysig static string 
            LocalizeString(bool isFemale,
                           string name,
                           object[] parameters) cil managed
    {
      .param [3]
      .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldsfld     string Sims3.Store.Objects.SaunaClassic/SteamedVeggies::sLocalizationKey
      IL_0006:  ldarg.1
      IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_000c:  ldarg.2
      IL_000d:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(bool,
                                                                                                              string,
                                                                                                              object[])
      IL_0012:  ret
    } // end of method SteamedVeggies::LocalizeString

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  ldstr      "Store/Objects/SteamedVeggies"
      IL_0005:  stsfld     string Sims3.Store.Objects.SaunaClassic/SteamedVeggies::sLocalizationKey
      IL_000a:  ret
    } // end of method SteamedVeggies::.cctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::.ctor()
      IL_0006:  ret
    } // end of method SteamedVeggies::.ctor

    .method public hidebysig virtual instance void 
            OnCreation() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::OnCreation()
      IL_0006:  ret
    } // end of method SteamedVeggies::OnCreation

    .method public hidebysig virtual instance void 
            OnStartup() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method SteamedVeggies::OnStartup

    .method public hidebysig specialname virtual 
            instance bool  get_InUse() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } // end of method SteamedVeggies::get_InUse

    .method public hidebysig newslot specialname virtual final 
            instance float32  get_CarryRouteToObjectRadius() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldc.r4     1.
      IL_0005:  ret
    } // end of method SteamedVeggies::get_CarryRouteToObjectRadius

    .method public hidebysig newslot specialname virtual final 
            instance string  get_CarryModelName() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldstr      "iceCreamCone"
      IL_0005:  ret
    } // end of method SteamedVeggies::get_CarryModelName

    .method public hidebysig newslot specialname virtual final 
            instance valuetype [SimIFace]Sims3.SimIFace.ProductVersion 
            get_CarryProductVersion() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } // end of method SteamedVeggies::get_CarryProductVersion

    .property instance bool InUse()
    {
      .get instance bool Sims3.Store.Objects.SaunaClassic/SteamedVeggies::get_InUse()
    } // end of property SteamedVeggies::InUse
    .property instance float32 CarryRouteToObjectRadius()
    {
      .get instance float32 Sims3.Store.Objects.SaunaClassic/SteamedVeggies::get_CarryRouteToObjectRadius()
    } // end of property SteamedVeggies::CarryRouteToObjectRadius
    .property instance string CarryModelName()
    {
      .get instance string Sims3.Store.Objects.SaunaClassic/SteamedVeggies::get_CarryModelName()
    } // end of property SteamedVeggies::CarryModelName
    .property instance valuetype [SimIFace]Sims3.SimIFace.ProductVersion
            CarryProductVersion()
    {
      .get instance valuetype [SimIFace]Sims3.SimIFace.ProductVersion Sims3.Store.Objects.SaunaClassic/SteamedVeggies::get_CarryProductVersion()
    } // end of property SteamedVeggies::CarryProductVersion
  } // end of class SteamedVeggies

  .field public static literal uint64 kGuidOnlyHadAHeartStore = uint64(0x9A7F5F1919DF0015)
  .field public static literal uint64 kGuidFeelingRustyStore = uint64(0x9A7F5F1919DF0016)
  .field public static literal uint64 kGuidSmellLikeWetDog = uint64(0x9A7F5F1919DF0017)
  .field public static literal uint64 kGuidRefreshedStore = uint64(0x9A7F5F1919DF0018)
  .field public static literal uint64 kGuidPrunedSkinStore = uint64(0x9A7F5F1919DF0019)
  .field public static string sLocalizationKey
  .field public string mRecipeName
  .field public bool mWaterfallFlowing
  .field public valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle mProximityRepeatingAlarm
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mFoggyEffect
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mSteamyStones
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mGlowyStones
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mSteamyVeggies
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mWaterfallEffect
  .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim mSimInTub
  .field public static int32 kComfyBuffScore
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 16 42 75 66 66 20 73 63 6F 72 65 20 66 6F   // ...Buff score fo
                                                                                            72 20 73 69 74 74 69 6E 67 00 00 )                // r sitting..
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive kEnvironmentTuning
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kBabyMadeChance
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 24 43 68 61 6E 63 65 20 6F 66 20 6D 61 6B   // ..$Chance of mak
                                                                                            69 6E 67 20 61 20 62 61 62 79 20 73 75 63 63 65   // ing a baby succe
                                                                                            73 73 66 75 6C 6C 79 00 00 )                      // ssfully..
  .field public static float32[] kBabyTraitChance
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 57 57 65 69 67 68 74 65 64 20 76 61 6C 75   // ..WWeighted valu
                                                                                            65 73 20 66 6F 72 20 63 68 61 6E 63 65 20 6F 66   // es for chance of
                                                                                            20 62 61 62 79 20 67 65 74 74 69 6E 67 20 74 68   //  baby getting th
                                                                                            65 73 65 20 74 72 61 69 74 73 3A 20 4E 6F 6E 65   // ese traits: None
                                                                                            2C 20 48 79 64 72 6F 70 68 6F 62 69 63 2C 20 50   // , Hydrophobic, P
                                                                                            61 72 74 79 41 6E 69 6D 61 6C 00 00 )             // artyAnimal..
  .field public static float32 kBathDurationToRemoveFatigue
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 4D 53 69 6D 20 6D 69 6E 75 74 65 73 20 74   // ..MSim minutes t
                                                                                            6F 20 62 65 20 69 6E 20 74 75 62 20 62 65 66 6F   // o be in tub befo
                                                                                            72 65 20 73 6F 72 65 2C 20 66 61 74 69 67 75 65   // re sore, fatigue
                                                                                            2C 20 73 74 72 65 73 73 2C 20 61 6E 64 20 73 6D   // , stress, and sm
                                                                                            65 6C 6C 79 20 61 72 65 20 72 65 6D 6F 76 65 64   // elly are removed
                                                                                            00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kProximityTestFrequency
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 3A 48 6F 77 20 66 72 65 71 75 65 6E 74 6C   // ..:How frequentl
                                                                                            79 20 74 6F 20 72 75 6E 20 70 72 6F 78 69 6D 69   // y to run proximi
                                                                                            74 79 20 63 68 65 63 6B 73 20 66 6F 72 20 64 6F   // ty checks for do
                                                                                            67 73 20 61 6E 64 20 72 6F 62 6F 74 73 00 00 )    // gs and robots..
  .field public static float32 kMudBathAgeExtension
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 26 44 61 79 73 20 74 68 65 20 6D 75 64 20   // ..&Days the mud 
                                                                                            62 61 74 68 20 65 78 74 65 6E 64 73 20 61 20 73   // bath extends a s
                                                                                            69 6D 27 73 20 6C 69 66 65 00 00 )                // im's life..
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kPrunedSkinChance
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 42 50 65 72 63 65 6E 74 20 63 68 61 6E 67   // ..BPercent chang
                                                                                            65 20 61 20 73 69 6D 20 77 69 6C 6C 20 67 65 74   // e a sim will get
                                                                                            20 70 72 75 6E 65 64 20 73 6B 69 6E 20 77 68 65   //  pruned skin whe
                                                                                            6E 20 64 6F 6E 65 20 77 69 74 68 20 74 68 65 20   // n done with the 
                                                                                            73 61 75 6E 61 00 00 )                            // sauna..
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kFunBoostForSteam
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 2F 48 6F 77 20 6D 75 63 68 20 66 75 6E 20   // ../How much fun 
                                                                                            63 72 65 61 74 69 6E 67 20 6D 6F 72 65 20 73 74   // creating more st
                                                                                            65 61 6D 20 67 69 76 65 73 20 65 76 65 72 79 6F   // eam gives everyo
                                                                                            6E 65 00 00 )                                     // ne..
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kWeightLossPerMinute
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 5D 6D 75 6C 74 69 70 6C 69 65 72 20 66 6F   // ..]multiplier fo
                                                                                            72 20 65 78 63 65 73 73 20 77 65 69 67 68 74 20   // r excess weight 
                                                                                            74 6F 20 6C 6F 73 65 20 65 76 65 72 79 20 73 69   // to lose every si
                                                                                            6D 20 6D 69 6E 75 74 65 2E 20 20 53 68 6F 75 6C   // m minute.  Shoul
                                                                                            64 20 62 65 20 76 65 72 79 20 73 6C 69 67 68 74   // d be very slight
                                                                                            6C 79 20 6C 65 73 73 20 74 68 61 6E 20 31 2E 30   // ly less than 1.0
                                                                                            00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kBathLengthInMinutes
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 25 4C 65 6E 67 74 68 20 6F 66 20 61 20 73   // ..%Length of a s
                                                                                            65 73 73 69 6F 6E 20 69 6E 20 74 68 65 20 73 61   // ession in the sa
                                                                                            75 6E 61 20 62 61 74 68 00 00 )                   // una bath..
  .field public static float32 kBuffFitnessModifier
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 80 8F 52 61 6E 67 65 3A 20 50 6F 73 69 74   // ....Range: Posit
                                                                                            69 76 65 20 66 6C 6F 61 74 20 6D 75 6C 74 69 70   // ive float multip
                                                                                            6C 69 65 72 73 20 61 62 6F 76 65 20 31 2E 30 66   // liers above 1.0f
                                                                                            2E 20 20 44 65 73 63 72 69 70 74 69 6F 6E 3A 20   // .  Description: 
                                                                                            20 4D 75 6C 74 69 70 6C 69 65 72 20 74 68 61 74   //  Multiplier that
                                                                                            20 69 6E 63 72 65 61 73 65 73 20 74 68 65 20 73   //  increases the s
                                                                                            70 65 65 64 20 61 74 20 77 68 69 63 68 20 74 68   // peed at which th
                                                                                            65 20 53 69 6D 20 69 6E 63 72 65 61 73 65 73 20   // e Sim increases 
                                                                                            68 69 73 2F 68 65 72 20 62 6F 64 79 20 73 68 61   // his/her body sha
                                                                                            70 65 2E 00 00 )                                  // pe...
  .field public static float32 kGroupChatRelationshipBump
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 30 42 75 6D 70 20 74 6F 20 67 69 76 65 20   // ..0Bump to give 
                                                                                            72 65 6C 61 74 69 6F 6E 73 68 69 70 20 77 68 65   // relationship whe
                                                                                            6E 20 63 68 61 74 74 69 6E 67 20 69 6E 20 73 61   // n chatting in sa
                                                                                            75 6E 61 00 00 )                                  // una..
  .field public static float32 kGroupChatSocialBump
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 2A 42 75 6D 70 20 74 6F 20 67 69 76 65 20   // ..*Bump to give 
                                                                                            73 6F 63 69 61 6C 20 77 68 65 6E 20 63 68 61 74   // social when chat
                                                                                            74 69 6E 67 20 69 6E 20 73 61 75 6E 61 00 00 )    // ting in sauna..
  .field public static float32 kBuffResetMotivesFrequency
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 41 48 6F 77 20 6D 61 6E 79 20 73 69 6D 20   // ..AHow many sim 
                                                                                            6D 69 6E 75 74 65 73 20 62 65 74 77 65 65 6E 20   // minutes between 
                                                                                            72 65 73 65 74 74 69 6E 67 20 74 68 65 20 68 69   // resetting the hi
                                                                                            64 64 65 6E 20 66 61 74 69 67 75 65 20 6D 6F 74   // dden fatigue mot
                                                                                            69 76 65 2E 00 00 )                               // ive...
  .field public static float32 kWooHooFunBump
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 19 42 75 6D 70 20 74 6F 20 67 69 76 65 20   // ...Bump to give 
                                                                                            61 66 74 65 72 20 57 6F 6F 48 6F 6F 00 00 )       // after WooHoo..
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kWooHooSocialBump
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 19 42 75 6D 70 20 74 6F 20 67 69 76 65 20   // ...Bump to give 
                                                                                            61 66 74 65 72 20 57 6F 6F 48 6F 6F 00 00 )       // after WooHoo..
  .field public static float32 kWooHooHygieneBump
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 19 42 75 6D 70 20 74 6F 20 67 69 76 65 20   // ...Bump to give 
                                                                                            61 66 74 65 72 20 57 6F 6F 48 6F 6F 00 00 )       // after WooHoo..
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig static string 
          LocalizeString(bool isFemale,
                         string name,
                         object[] parameters) cil managed
  {
    .param [3]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldsfld     string Sims3.Store.Objects.SaunaClassic::sLocalizationKey
    IL_0006:  ldstr      ":"
    IL_000b:  ldarg.1
    IL_000c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0011:  ldarg.2
    IL_0012:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(bool,
                                                                                                            string,
                                                                                                            object[])
    IL_0017:  ret
  } // end of method SaunaClassic::LocalizeString

  .method public hidebysig specialname virtual 
          instance int32  get_ComfyBuffScore() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 Sims3.Store.Objects.SaunaClassic::kComfyBuffScore
    IL_0005:  ret
  } // end of method SaunaClassic::get_ComfyBuffScore

  .method public hidebysig specialname virtual 
          instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive 
          get_EnvironmentTuning() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive Sims3.Store.Objects.SaunaClassic::kEnvironmentTuning
    IL_0005:  ret
  } // end of method SaunaClassic::get_EnvironmentTuning

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SteamedVeggies"
    IL_0006:  stfld      string Sims3.Store.Objects.SaunaClassic::mRecipeName
    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.1
    IL_000d:  stfld      bool Sims3.Store.Objects.SaunaClassic::mWaterfallFlowing
    IL_0012:  ldarg.0
    IL_0013:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
    IL_0018:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic::mProximityRepeatingAlarm
    IL_001d:  ldarg.0
    IL_001e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::.ctor()
    IL_0023:  ret
  } // end of method SaunaClassic::.ctor

  .method public hidebysig virtual instance void 
          OnCreation() cil managed
  {
    // Code size       176 (0xb0)
    .maxstack  9
    .locals init (valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitDirection[] V_0,
             valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitDirection[] V_1,
             valuetype [SimIFace]Sims3.SimIFace.Slot[] V_2,
             valuetype [SimIFace]Sims3.SimIFace.Slot[] V_3)
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::OnCreation()
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
    IL_000c:  brtrue     IL_00a9

    IL_0011:  ldarg.0
    IL_0012:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::.ctor()
    IL_0017:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
    IL_001c:  ldc.i4.3
    IL_001d:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitDirection
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.0
    IL_0025:  ldc.i4.2
    IL_0026:  stelem.i4
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.2
    IL_0029:  ldc.i4.1
    IL_002a:  stelem.i4
    IL_002b:  ldloc.1
    IL_002c:  stloc.0
    IL_002d:  ldarg.0
    IL_002e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
    IL_0033:  ldarg.0
    IL_0034:  ldc.i4.7
    IL_0035:  ldstr      "_02_bench"
    IL_003a:  ldc.i4.1
    IL_003b:  ldc.i4     0xa820f8a4
    IL_0040:  ldc.i4.3
    IL_0041:  newarr     [SimIFace]Sims3.SimIFace.Slot
    IL_0046:  stloc.2
    IL_0047:  ldloc.2
    IL_0048:  ldc.i4.0
    IL_0049:  ldc.i4     0xa678e706
    IL_004e:  stelem.i4
    IL_004f:  ldloc.2
    IL_0050:  ldc.i4.1
    IL_0051:  ldc.i4     0xa678e705
    IL_0056:  stelem.i4
    IL_0057:  ldloc.2
    IL_0058:  ldc.i4.2
    IL_0059:  ldc.i4     0xa678e704
    IL_005e:  stelem.i4
    IL_005f:  ldloc.2
    IL_0060:  ldloc.0
    IL_0061:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup,
                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartArea,
                                                                                                        string,
                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitStyle,
                                                                                                        valuetype [SimIFace]Sims3.SimIFace.Slot,
                                                                                                        valuetype [SimIFace]Sims3.SimIFace.Slot[],
                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitDirection[])
    IL_0066:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::AddSeatRight(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat)
    IL_006b:  ldarg.0
    IL_006c:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
    IL_0071:  ldarg.0
    IL_0072:  ldc.i4.6
    IL_0073:  ldstr      "_01_bench"
    IL_0078:  ldc.i4.1
    IL_0079:  ldc.i4     0xa820f8a7
    IL_007e:  ldc.i4.3
    IL_007f:  newarr     [SimIFace]Sims3.SimIFace.Slot
    IL_0084:  stloc.3
    IL_0085:  ldloc.3
    IL_0086:  ldc.i4.0
    IL_0087:  ldc.i4     0x31229a44
    IL_008c:  stelem.i4
    IL_008d:  ldloc.3
    IL_008e:  ldc.i4.1
    IL_008f:  ldc.i4     0x31229a45
    IL_0094:  stelem.i4
    IL_0095:  ldloc.3
    IL_0096:  ldc.i4.2
    IL_0097:  ldc.i4     0xa678e707
    IL_009c:  stelem.i4
    IL_009d:  ldloc.3
    IL_009e:  ldloc.0
    IL_009f:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.IHasSeatingGroup,
                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartArea,
                                                                                                        string,
                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitStyle,
                                                                                                        valuetype [SimIFace]Sims3.SimIFace.Slot,
                                                                                                        valuetype [SimIFace]Sims3.SimIFace.Slot[],
                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitDirection[])
    IL_00a4:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::AddSeatRight(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat)
    IL_00a9:  ldarg.0
    IL_00aa:  call       instance void Sims3.Store.Objects.SaunaClassic::SetWaterfallMaterial()
    IL_00af:  ret
  } // end of method SaunaClassic::OnCreation

  .method public hidebysig virtual instance void 
          OnStartup() cil managed
  {
    // Code size       288 (0x120)
    .maxstack  5
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.XmlDbData V_0,
             class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe V_1,
             object[] V_2,
             object[] V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.3
    IL_0002:  newarr     [mscorlib]System.Object
    IL_0007:  stloc.2
    IL_0008:  ldloc.2
    IL_0009:  ldc.i4.0
    IL_000a:  ldarg.0
    IL_000b:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::get_ComfyBuffScore()
    IL_0010:  box        [mscorlib]System.Int32
    IL_0015:  stelem.ref
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  ldarg.0
    IL_0019:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::get_SitInteractionName()
    IL_001e:  stelem.ref
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.2
    IL_0021:  ldarg.0
    IL_0022:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::get_SeatingGroup()
    IL_0027:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData[] [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_SitDataArray()
    IL_002c:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0031:  stelem.ref
    IL_0032:  ldloc.2
    IL_0033:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class Sims3.Store.Objects.SaunaClassic/SaunaSittableComponent>(object[])
    IL_0038:  pop
    IL_0039:  ldc.i4     0x200
    IL_003e:  call       bool [SimIFace]Sims3.SimIFace.GameUtils::IsInstalled(valuetype [SimIFace]Sims3.SimIFace.ProductVersion)
    IL_0043:  brfalse.s  IL_0076

    IL_0045:  ldarg.0
    IL_0046:  ldc.i4.2
    IL_0047:  newarr     [mscorlib]System.Object
    IL_004c:  stloc.3
    IL_004d:  ldloc.3
    IL_004e:  ldc.i4.0
    IL_004f:  ldarg.0
    IL_0050:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::get_SeatingGroup()
    IL_0055:  stelem.ref
    IL_0056:  ldloc.3
    IL_0057:  ldc.i4.1
    IL_0058:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.CAS.CASAGSAvailabilityFlags [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PetSurfaceComponent::kAdultAndElderDogs
    IL_005d:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.CAS.CASAGSAvailabilityFlags [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PetSurfaceComponent::kAdultAndElderLittleDogs
    IL_0062:  or
    IL_0063:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.CAS.CASAGSAvailabilityFlags [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PetSurfaceComponent::kAdultAndElderCats
    IL_0068:  or
    IL_0069:  box        [SimIFace]Sims3.SimIFace.CAS.CASAGSAvailabilityFlags
    IL_006e:  stelem.ref
    IL_006f:  ldloc.3
    IL_0070:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PetSurfaceComponent>(object[])
    IL_0075:  pop
    IL_0076:  ldarg.0
    IL_0077:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveComponent<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.BoobyTrapComponent>()
    IL_007c:  ldarg.0
    IL_007d:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Scoot::Singleton
    IL_0082:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0087:  ldarg.0
    IL_0088:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Nap::Singleton
    IL_008d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0092:  ldstr      "RecipeMasterList_store"
    IL_0097:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.XmlDbData [Sims3GameplaySystems]Sims3.Gameplay.Utilities.XmlDbData::ReadData(string)
    IL_009c:  stloc.0
    IL_009d:  ldloc.0
    IL_009e:  brfalse.s  IL_00bb

    IL_00a0:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe> [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe::NameToRecipeHash
    IL_00a5:  ldarg.0
    IL_00a6:  ldfld      string Sims3.Store.Objects.SaunaClassic::mRecipeName
    IL_00ab:  ldloca.s   V_1
    IL_00ad:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe>::TryGetValue(!0,
                                                                                                                                                                                          !1&)
    IL_00b2:  brtrue.s   IL_00bb

    IL_00b4:  ldloc.0
    IL_00b5:  ldc.i4.1
    IL_00b6:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Recipe::LoadRecipeData(class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.XmlDbData,
                                                                                                              bool)
    IL_00bb:  ldarg.0
    IL_00bc:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Nap::Singleton
    IL_00c1:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveInteractionByType(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00c6:  ldarg.0
    IL_00c7:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.GoForJoyride::Singleton
    IL_00cc:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveInteractionByType(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00d1:  ldarg.0
    IL_00d2:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.RouteAndGetToCostumeForJoyride::Singleton
    IL_00d7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveInteractionByType(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00dc:  ldarg.0
    IL_00dd:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/BatheInWater::WaterSingleton
    IL_00e2:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00e7:  ldarg.0
    IL_00e8:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/BatheInWater::MudSingleton
    IL_00ed:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00f2:  ldarg.0
    IL_00f3:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/SteamVeggies::Singleton
    IL_00f8:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00fd:  ldarg.0
    IL_00fe:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/MakeSteam::Singleton
    IL_0103:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0108:  ldarg.0
    IL_0109:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/ToggleWaterfall::Singleton
    IL_010e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0113:  ldarg.0
    IL_0114:  call       instance void Sims3.Store.Objects.SaunaClassic::StartProximityAlarm()
    IL_0119:  ldarg.0
    IL_011a:  call       instance void Sims3.Store.Objects.SaunaClassic::StartStonesGlowFX()
    IL_011f:  ret
  } // end of method SaunaClassic::OnStartup

  .method public hidebysig virtual instance void 
          DoReset(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/ResetInformation resetInformation) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.SaunaClassic::Cleanup()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::DoReset(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/ResetInformation)
    IL_000d:  ldarg.0
    IL_000e:  call       instance void Sims3.Store.Objects.SaunaClassic::StartProximityAlarm()
    IL_0013:  ldarg.0
    IL_0014:  call       instance void Sims3.Store.Objects.SaunaClassic::StartStonesGlowFX()
    IL_0019:  ret
  } // end of method SaunaClassic::DoReset

  .method public hidebysig virtual instance void 
          Dispose() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.SaunaClassic::Cleanup()
    IL_0006:  ldarg.0
    IL_0007:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::Dispose()
    IL_000c:  ret
  } // end of method SaunaClassic::Dispose

  .method public hidebysig instance void 
          Cleanup() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::mSimInTub
    IL_0007:  ldarg.0
    IL_0008:  call       instance void Sims3.Store.Objects.SaunaClassic::StopProximityAlarm()
    IL_000d:  ldarg.0
    IL_000e:  call       instance void Sims3.Store.Objects.SaunaClassic::StopFoggyFX()
    IL_0013:  ldarg.0
    IL_0014:  call       instance void Sims3.Store.Objects.SaunaClassic::StopVeggieSteamFX()
    IL_0019:  ldarg.0
    IL_001a:  call       instance void Sims3.Store.Objects.SaunaClassic::StopStonesSteamFX()
    IL_001f:  ldarg.0
    IL_0020:  call       instance void Sims3.Store.Objects.SaunaClassic::StopStonesGlowFX()
    IL_0025:  ldarg.0
    IL_0026:  call       instance void Sims3.Store.Objects.SaunaClassic::StopWaterfallFX()
    IL_002b:  ldarg.0
    IL_002c:  call       instance void Sims3.Store.Objects.SaunaClassic::SetWaterfallMaterial()
    IL_0031:  ret
  } // end of method SaunaClassic::Cleanup

  .method public hidebysig specialname virtual 
          instance string  get_SitInteractionName() cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldsfld     string Sims3.Store.Objects.SaunaClassic::sLocalizationKey
    IL_0005:  ldstr      ":SitInteractionName"
    IL_000a:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000f:  ret
  } // end of method SaunaClassic::get_SitInteractionName

  .method public hidebysig virtual instance valuetype [SimIFace]Sims3.SimIFace.Vector3[] 
          GetFirePositions() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  3
    .locals init (valuetype [SimIFace]Sims3.SimIFace.Vector3[] V_0)
    IL_0000:  ldc.i4.2
    IL_0001:  newarr     [SimIFace]Sims3.SimIFace.Vector3
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldelema    [SimIFace]Sims3.SimIFace.Vector3
    IL_000e:  ldarg.0
    IL_000f:  ldc.i4     0xa820f8a7
    IL_0014:  call       instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetSlotPosition(valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0019:  stobj      [SimIFace]Sims3.SimIFace.Vector3
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.1
    IL_0020:  ldelema    [SimIFace]Sims3.SimIFace.Vector3
    IL_0025:  ldarg.0
    IL_0026:  ldc.i4     0xa820f8a4
    IL_002b:  call       instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetSlotPosition(valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0030:  stobj      [SimIFace]Sims3.SimIFace.Vector3
    IL_0035:  ldloc.0
    IL_0036:  ret
  } // end of method SaunaClassic::GetFirePositions

  .method public hidebysig newslot virtual final 
          instance bool  Sims3.Gameplay.Interfaces.ISittable.StateMachineEnterAndSit(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture sitPosture,
                                                                                     valuetype [SimIFace]Sims3.SimIFace.Slot routingSlot,
                                                                                     object sitContext) cil managed
  {
    .override [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ISittable::StateMachineEnterAndSit
    // Code size       347 (0x15b)
    .maxstack  5
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.ThoughtBalloons.ThoughtBalloonManager/BalloonData V_0,
             class [mscorlib]System.Type V_1,
             valuetype [mscorlib]System.Reflection.BindingFlags V_2,
             class [mscorlib]System.Reflection.FieldInfo V_3,
             class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition> V_4)
    IL_0000:  ldarg.2
    IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
    IL_000b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
    IL_0010:  brfalse.s  IL_0035

    IL_0012:  ldnull
    IL_0013:  stloc.0
    IL_0014:  ldstr      "balloon_moodlet_pregnant"
    IL_0019:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ThoughtBalloons.ThoughtBalloonManager/BalloonData::.ctor(string)
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  brfalse.s  IL_0033

    IL_0022:  ldarg.2
    IL_0023:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_0028:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ThoughtBalloons.ThoughtBalloonManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_ThoughtBalloonManager()
    IL_002d:  ldloc.0
    IL_002e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ThoughtBalloons.ThoughtBalloonManager::ShowBalloon(class [Sims3GameplaySystems]Sims3.Gameplay.ThoughtBalloons.ThoughtBalloonManager/BalloonData)
    IL_0033:  ldc.i4.0
    IL_0034:  ret

    IL_0035:  ldarg.2
    IL_0036:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_003b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Children.CarryingChildPosture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CarryingChildPosture()
    IL_0040:  brtrue.s   IL_004f

    IL_0042:  ldarg.2
    IL_0043:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_0048:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarryingPetPosture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CarryingPetPosture()
    IL_004d:  brfalse.s  IL_0051

    IL_004f:  ldc.i4.0
    IL_0050:  ret

    IL_0051:  ldarg.2
    IL_0052:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_0057:  ldc.i8     0x4c800171bd46ad0
    IL_0060:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
    IL_0065:  brtrue.s   IL_0073

    IL_0067:  ldarg.2
    IL_0068:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_006d:  ldc.i4.3
    IL_006e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SwitchToOutfitWithSpin(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason)
    IL_0073:  ldarg.0
    IL_0074:  ldarg.1
    IL_0075:  ldarg.2
    IL_0076:  ldarg.3
    IL_0077:  ldarg.s    sitContext
    IL_0079:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::StateMachineEnterAndSit(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture,
                                                                                                                                     valuetype [SimIFace]Sims3.SimIFace.Slot,
                                                                                                                                     object)
    IL_007e:  brfalse    IL_0159

    IL_0083:  ldarg.2
    IL_0084:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair> [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::get_Interactions()
    IL_0089:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.StartSeatedCuddleA::Singleton
    IL_008e:  ldarg.2
    IL_008f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_0094:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                  class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
    IL_0099:  callvirt   instance bool class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair>::Remove(!0)
    IL_009e:  pop
    IL_009f:  ldarg.2
    IL_00a0:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_00a5:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.StartSeatedCuddleA::Singleton
    IL_00aa:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveInteractionByType(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00af:  ldtoken    [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture
    IL_00b4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b9:  stloc.1
    IL_00ba:  ldc.i4.s   52
    IL_00bc:  stloc.2
    IL_00bd:  ldloc.1
    IL_00be:  ldstr      "mSocialInteractionDefinitions"
    IL_00c3:  ldloc.2
    IL_00c4:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Type::GetField(string,
                                                                                                              valuetype [mscorlib]System.Reflection.BindingFlags)
    IL_00c9:  stloc.3
    IL_00ca:  ldloc.3
    IL_00cb:  brfalse.s  IL_00ec

    IL_00cd:  ldloc.3
    IL_00ce:  ldarg.2
    IL_00cf:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
    IL_00d4:  isinst     class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition>
    IL_00d9:  stloc.s    V_4
    IL_00db:  ldloc.s    V_4
    IL_00dd:  brfalse.s  IL_00ec

    IL_00df:  ldloc.s    V_4
    IL_00e1:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.StartSeatedCuddleA::Singleton
    IL_00e6:  callvirt   instance bool class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition>::Remove(!0)
    IL_00eb:  pop
    IL_00ec:  ldarg.2
    IL_00ed:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::Singleton
    IL_00f2:  ldarg.2
    IL_00f3:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_00f8:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
    IL_00fd:  ldarg.2
    IL_00fe:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::TryForBoySingleton
    IL_0103:  ldarg.2
    IL_0104:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_0109:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
    IL_010e:  ldarg.2
    IL_010f:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/CuddleSeatedWooHooSauna::TryForGirlSingleton
    IL_0114:  ldarg.2
    IL_0115:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_011a:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
    IL_011f:  ldarg.2
    IL_0120:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::Singleton
    IL_0125:  ldarg.2
    IL_0126:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_012b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
    IL_0130:  ldarg.2
    IL_0131:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.SaunaClassic/StartSaunaSeatedCuddleA::Singleton
    IL_0136:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::AddSocialInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_013b:  ldarg.2
    IL_013c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture::get_Sim()
    IL_0141:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
    IL_0146:  ldc.i8     0x9a7f5f1919df0018
    IL_014f:  ldc.i4.0
    IL_0150:  conv.i8
    IL_0151:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElementPaused(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
    IL_0156:  pop
    IL_0157:  ldc.i4.1
    IL_0158:  ret

    IL_0159:  ldc.i4.0
    IL_015a:  ret
  } // end of method SaunaClassic::Sims3.Gameplay.Interfaces.ISittable.StateMachineEnterAndSit

  .method public hidebysig newslot virtual final 
          instance bool  Sims3.Gameplay.Interfaces.ISittable.CanGetInAndOut(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim sim) cil managed
  {
    .override [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ISittable::CanGetInAndOut
    // Code size       56 (0x38)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
    IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsVisuallyPregnant()
    IL_000b:  brfalse.s  IL_000f

    IL_000d:  ldc.i4.0
    IL_000e:  ret

    IL_000f:  ldarg.1
    IL_0010:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
    IL_0015:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_ChildOrBelow()
    IL_001a:  brfalse.s  IL_001e

    IL_001c:  ldc.i4.0
    IL_001d:  ret

    IL_001e:  ldarg.1
    IL_001f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Children.CarryingChildPosture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CarryingChildPosture()
    IL_0024:  brtrue.s   IL_002e

    IL_0026:  ldarg.1
    IL_0027:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarryingPetPosture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CarryingPetPosture()
    IL_002c:  brfalse.s  IL_0030

    IL_002e:  ldc.i4.0
    IL_002f:  ret

    IL_0030:  ldarg.0
    IL_0031:  ldarg.1
    IL_0032:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::CanGetInAndOut(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
    IL_0037:  ret
  } // end of method SaunaClassic::Sims3.Gameplay.Interfaces.ISittable.CanGetInAndOut

  .method public hidebysig newslot virtual final 
          instance void  Sims3.Gameplay.Interfaces.ISittable.StateMachineStandAndExit(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim sim,
                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient smc) cil managed
  {
    .override [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ISittable::StateMachineStandAndExit
    // Code size       108 (0x6c)
    .maxstack  3
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture V_0,
             class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
    IL_0006:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SittingPosture
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  pop
    IL_000e:  ldarg.0
    IL_000f:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::get_SeatingGroup()
    IL_0014:  ldarg.1
    IL_0015:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Item(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  brtrue.s   IL_001f

    IL_001e:  ret

    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
    IL_0025:  ldc.i8     0x9a7f5f1919df0018
    IL_002e:  callvirt   instance void class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::RemoveElement(uint64)
    IL_0033:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kPrunedSkinChance
    IL_0038:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomChance(float32)
    IL_003d:  brfalse.s  IL_0063

    IL_003f:  ldarg.1
    IL_0040:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
    IL_0045:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsFrankenstein()
    IL_004a:  brtrue.s   IL_0063

    IL_004c:  ldarg.1
    IL_004d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
    IL_0052:  ldc.i8     0x9a7f5f1919df0019
    IL_005b:  ldc.i4.0
    IL_005c:  conv.i8
    IL_005d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
    IL_0062:  pop
    IL_0063:  ldarg.0
    IL_0064:  ldarg.1
    IL_0065:  ldarg.2
    IL_0066:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::StateMachineStandAndExit(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient)
    IL_006b:  ret
  } // end of method SaunaClassic::Sims3.Gameplay.Interfaces.ISittable.StateMachineStandAndExit

  .method public hidebysig instance void 
          StartFoggyFX() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.SaunaClassic::StopFoggyFX()
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mFoggyEffect
    IL_000c:  brtrue.s   IL_003c

    IL_000e:  ldarg.0
    IL_000f:  ldstr      "store_sauna_steam"
    IL_0014:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
    IL_0019:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mFoggyEffect
    IL_001e:  ldarg.0
    IL_001f:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mFoggyEffect
    IL_0024:  ldarg.0
    IL_0025:  ldc.i4     0x9cec4dcb
    IL_002a:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_002f:  pop
    IL_0030:  ldarg.0
    IL_0031:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mFoggyEffect
    IL_0036:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_003b:  pop
    IL_003c:  ret
  } // end of method SaunaClassic::StartFoggyFX

  .method public hidebysig instance void 
          StopFoggyFX() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mFoggyEffect
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mFoggyEffect
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mFoggyEffect
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mFoggyEffect
    IL_0026:  ret
  } // end of method SaunaClassic::StopFoggyFX

  .method public hidebysig instance void 
          StartStonesSteamFX() cil managed
  {
    // Code size       67 (0x43)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.SaunaClassic::StopStonesSteamFX()
    IL_0006:  ldarg.0
    IL_0007:  call       instance void Sims3.Store.Objects.SaunaClassic::StartFoggyFX()
    IL_000c:  ldarg.0
    IL_000d:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyStones
    IL_0012:  brtrue.s   IL_0042

    IL_0014:  ldarg.0
    IL_0015:  ldstr      "store_sauna_steamStones"
    IL_001a:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
    IL_001f:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyStones
    IL_0024:  ldarg.0
    IL_0025:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyStones
    IL_002a:  ldarg.0
    IL_002b:  ldc.i4     0x318a282e
    IL_0030:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0035:  pop
    IL_0036:  ldarg.0
    IL_0037:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyStones
    IL_003c:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_0041:  pop
    IL_0042:  ret
  } // end of method SaunaClassic::StartStonesSteamFX

  .method public hidebysig instance void 
          StopStonesSteamFX() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyStones
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyStones
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyStones
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyStones
    IL_0026:  ret
  } // end of method SaunaClassic::StopStonesSteamFX

  .method public hidebysig instance void 
          StartStonesGlowFX() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mGlowyStones
    IL_0006:  brtrue.s   IL_0036

    IL_0008:  ldarg.0
    IL_0009:  ldstr      "store_sauna_glow"
    IL_000e:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
    IL_0013:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mGlowyStones
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mGlowyStones
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4     0x318a282e
    IL_0024:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0029:  pop
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mGlowyStones
    IL_0030:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_0035:  pop
    IL_0036:  ret
  } // end of method SaunaClassic::StartStonesGlowFX

  .method public hidebysig instance void 
          StopStonesGlowFX() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mGlowyStones
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mGlowyStones
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mGlowyStones
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mGlowyStones
    IL_0026:  ret
  } // end of method SaunaClassic::StopStonesGlowFX

  .method public hidebysig instance void 
          StartVeggieSteamFX(class [SimIFace]Sims3.SimIFace.IHasScriptProxy pan) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyVeggies
    IL_0006:  brtrue.s   IL_0036

    IL_0008:  ldarg.0
    IL_0009:  ldstr      "store_sauna_steamVegetables"
    IL_000e:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
    IL_0013:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyVeggies
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyVeggies
    IL_001e:  ldarg.1
    IL_001f:  ldc.i4     0x318a282c
    IL_0024:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0029:  pop
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyVeggies
    IL_0030:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_0035:  pop
    IL_0036:  ret
  } // end of method SaunaClassic::StartVeggieSteamFX

  .method public hidebysig instance void 
          StopVeggieSteamFX() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyVeggies
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyVeggies
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyVeggies
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mSteamyVeggies
    IL_0026:  ret
  } // end of method SaunaClassic::StopVeggieSteamFX

  .method public hidebysig instance void 
          StartWaterfallFX() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mWaterfallEffect
    IL_0006:  brtrue.s   IL_0036

    IL_0008:  ldarg.0
    IL_0009:  ldstr      "store_sauna_waterfall"
    IL_000e:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
    IL_0013:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mWaterfallEffect
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mWaterfallEffect
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4     0x318a282f
    IL_0024:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0029:  pop
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mWaterfallEffect
    IL_0030:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_0035:  pop
    IL_0036:  ret
  } // end of method SaunaClassic::StartWaterfallFX

  .method public hidebysig instance void 
          StopWaterfallFX() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mWaterfallEffect
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mWaterfallEffect
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mWaterfallEffect
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.SaunaClassic::mWaterfallEffect
    IL_0026:  ret
  } // end of method SaunaClassic::StopWaterfallFX

  .method public hidebysig instance void 
          SetWaterfallMaterial() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Sims3.Store.Objects.SaunaClassic::mWaterfallFlowing
    IL_0006:  brfalse.s  IL_001a

    IL_0008:  ldarg.0
    IL_0009:  ldstr      "WaterfallOn"
    IL_000e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetMaterial(string)
    IL_0013:  ldarg.0
    IL_0014:  call       instance void Sims3.Store.Objects.SaunaClassic::StartWaterfallFX()
    IL_0019:  ret

    IL_001a:  ldarg.0
    IL_001b:  ldstr      "WaterfallOff"
    IL_0020:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetMaterial(string)
    IL_0025:  ldarg.0
    IL_0026:  call       instance void Sims3.Store.Objects.SaunaClassic::StopWaterfallFX()
    IL_002b:  ret
  } // end of method SaunaClassic::SetWaterfallMaterial

  .method public hidebysig instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim 
          GetLeftSim() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  2
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
    IL_0006:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat> [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Seats()
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat>::get_Item(int32)
    IL_0011:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData::get_ContainedSim()
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ret
  } // end of method SaunaClassic::GetLeftSim

  .method public hidebysig instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim 
          GetRightSim() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  2
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.MultiSeatObject::mSeatingGroup
    IL_0006:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat> [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.SeatingGroup::get_Seats()
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Seating.Seat>::get_Item(int32)
    IL_0011:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartData::get_ContainedSim()
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ret
  } // end of method SaunaClassic::GetRightSim

  .method public hidebysig instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim 
          GetOtherSittingSim(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_0,
             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::GetLeftSim()
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.SaunaClassic::GetRightSim()
    IL_000d:  stloc.1
    IL_000e:  ldloc.0
    IL_000f:  brfalse.s  IL_0014

    IL_0011:  ldloc.1
    IL_0012:  brtrue.s   IL_0016

    IL_0014:  ldnull
    IL_0015:  ret

    IL_0016:  ldloc.0
    IL_0017:  ldarg.1
    IL_0018:  bne.un.s   IL_001c

    IL_001a:  ldloc.1
    IL_001b:  ret

    IL_001c:  ldloc.1
    IL_001d:  ldarg.1
    IL_001e:  bne.un.s   IL_0022

    IL_0020:  ldloc.0
    IL_0021:  ret

    IL_0022:  ldnull
    IL_0023:  ret
  } // end of method SaunaClassic::GetOtherSittingSim

  .method public hidebysig instance void 
          StartProximityAlarm() cil managed
  {
    // Code size       73 (0x49)
    .maxstack  9
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic::mProximityRepeatingAlarm
    IL_0006:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
    IL_000b:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::op_Equality(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle,
                                                                                                      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
    IL_0010:  brfalse.s  IL_0048

    IL_0012:  ldarg.0
    IL_0013:  ldarg.0
    IL_0014:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kProximityTestFrequency
    IL_0019:  ldc.i4.1
    IL_001a:  ldarg.0
    IL_001b:  ldftn      instance void Sims3.Store.Objects.SaunaClassic::DoProximityBuffs()
    IL_0021:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback::.ctor(object,
                                                                                                                native int)
    IL_0026:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kProximityTestFrequency
    IL_002b:  ldc.i4.1
    IL_002c:  ldstr      "sauna Alarm"
    IL_0031:  ldc.i4.0
    IL_0032:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddAlarmRepeating(float32,
                                                                                                                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                                    class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback,
                                                                                                                                                                                    float32,
                                                                                                                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                                    string,
                                                                                                                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmType)
    IL_0037:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic::mProximityRepeatingAlarm
    IL_003c:  ldarg.0
    IL_003d:  ldarg.0
    IL_003e:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic::mProximityRepeatingAlarm
    IL_0043:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AlarmWillYield(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
    IL_0048:  ret
  } // end of method SaunaClassic::StartProximityAlarm

  .method public hidebysig instance void 
          StopProximityAlarm() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic::mProximityRepeatingAlarm
    IL_0006:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
    IL_000b:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::op_Inequality(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle,
                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
    IL_0010:  brfalse.s  IL_0029

    IL_0012:  ldarg.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic::mProximityRepeatingAlarm
    IL_0019:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveAlarm(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
    IL_001e:  ldarg.0
    IL_001f:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
    IL_0024:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.SaunaClassic::mProximityRepeatingAlarm
    IL_0029:  ret
  } // end of method SaunaClassic::StopProximityAlarm

  .method public hidebysig static bool  IsADog(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim sim) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsADogSpecies()
    IL_0006:  brtrue.s   IL_0017

    IL_0008:  ldarg.0
    IL_0009:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_OccultManager()
    IL_000e:  ldc.i4.s   64
    IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultManager::HasOccultType(valuetype [UI]Sims3.UI.Hud.OccultTypes)
    IL_0015:  brfalse.s  IL_0019

    IL_0017:  ldc.i4.1
    IL_0018:  ret

    IL_0019:  ldc.i4.0
    IL_001a:  ret
  } // end of method SaunaClassic::IsADog

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> 
          GetSimsInFootprint() cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> V_0,
             class [mscorlib]System.Array V_1,
             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_2,
             class [mscorlib]System.Collections.IEnumerator V_3,
             class [mscorlib]System.IDisposable V_4)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldtoken    [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldarg.0
    IL_0011:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Position()
    IL_0016:  ldc.r4     4.
    IL_001b:  call       class [mscorlib]System.Array [SimIFace]Sims3.SimIFace.Queries::GetObjects(class [mscorlib]System.Type,
                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                   float32)
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  brfalse.s  IL_006c

    IL_0024:  ldloc.1
    IL_0025:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Array::GetEnumerator()
    IL_002a:  stloc.3
    .try
    {
      IL_002b:  br.s       IL_004e

      IL_002d:  ldloc.3
      IL_002e:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0033:  castclass  [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim
      IL_0038:  stloc.2
      IL_0039:  ldloc.2
      IL_003a:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_RoomId()
      IL_003f:  ldarg.0
      IL_0040:  call       instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_RoomId()
      IL_0045:  bne.un.s   IL_004e

      IL_0047:  ldloc.0
      IL_0048:  ldloc.2
      IL_0049:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Add(!0)
      IL_004e:  ldloc.3
      IL_004f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brtrue.s   IL_002d

      IL_0056:  leave.s    IL_006c

    }  // end .try
    finally
    {
      IL_0058:  ldloc.3
      IL_0059:  isinst     [mscorlib]System.IDisposable
      IL_005e:  stloc.s    V_4
      IL_0060:  ldloc.s    V_4
      IL_0062:  brfalse.s  IL_006b

      IL_0064:  ldloc.s    V_4
      IL_0066:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_006b:  endfinally
    }  // end handler
    IL_006c:  ldloc.0
    IL_006d:  ret
  } // end of method SaunaClassic::GetSimsInFootprint

  .method public hidebysig instance void 
          DoProximityBuffs() cil managed
  {
    // Code size       268 (0x10c)
    .maxstack  3
    .locals init (bool V_0,
             class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> V_1,
             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_2,
             class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance V_3,
             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_4,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> V_5,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> V_6)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  call       instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> Sims3.Store.Objects.SaunaClassic::GetSimsInFootprint()
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::GetEnumerator()
    IL_000f:  stloc.s    V_5
    .try
    {
      IL_0011:  br         IL_009f

      IL_0016:  ldloca.s   V_5
      IL_0018:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Current()
      IL_001d:  stloc.2
      IL_001e:  ldloc.2
      IL_001f:  call       bool Sims3.Store.Objects.SaunaClassic::IsADog(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0024:  brfalse.s  IL_0028

      IL_0026:  ldc.i4.1
      IL_0027:  stloc.0
      IL_0028:  ldloc.2
      IL_0029:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_OccultManager()
      IL_002e:  ldc.i4.2
      IL_002f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultManager::HasOccultType(valuetype [UI]Sims3.UI.Hud.OccultTypes)
      IL_0034:  brfalse.s  IL_0083

      IL_0036:  ldloc.2
      IL_0037:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_003c:  ldc.i8     0x9a7f5f1919df0016
      IL_0045:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_004a:  stloc.3
      IL_004b:  ldloc.3
      IL_004c:  brfalse.s  IL_006c

      IL_004e:  ldloc.3
      IL_004f:  call       int64 [Sims3GameplaySystems]Sims3.Gameplay.Utilities.SimClock::get_CurrentTicks()
      IL_0054:  stfld      int64 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mStartTime
      IL_0059:  ldloc.3
      IL_005a:  ldloc.3
      IL_005b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mBuff
      IL_0060:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::get_TimeoutSimMinutes()
      IL_0065:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
      IL_006a:  br.s       IL_0083

      IL_006c:  ldloc.2
      IL_006d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0072:  ldc.i8     0x9a7f5f1919df0016
      IL_007b:  ldc.i4.0
      IL_007c:  conv.i8
      IL_007d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_0082:  pop
      IL_0083:  ldloc.2
      IL_0084:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_0089:  ldloc.2
      IL_008a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_008f:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescriptionCore::get_Weight()
      IL_0094:  ldsfld     float32 Sims3.Store.Objects.SaunaClassic::kWeightLossPerMinute
      IL_0099:  mul
      IL_009a:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescriptionCore::set_Weight(float32)
      IL_009f:  ldloca.s   V_5
      IL_00a1:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::MoveNext()
      IL_00a6:  brtrue     IL_0016

      IL_00ab:  leave.s    IL_00bb

    }  // end .try
    finally
    {
      IL_00ad:  ldloca.s   V_5
      IL_00af:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>
      IL_00b5:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_00ba:  endfinally
    }  // end handler
    IL_00bb:  ldloc.0
    IL_00bc:  brfalse.s  IL_010b

    IL_00be:  ldloc.1
    IL_00bf:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::GetEnumerator()
    IL_00c4:  stloc.s    V_6
    .try
    {
      IL_00c6:  br.s       IL_00f2

      IL_00c8:  ldloca.s   V_6
      IL_00ca:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Current()
      IL_00cf:  stloc.s    V_4
      IL_00d1:  ldloc.s    V_4
      IL_00d3:  call       bool Sims3.Store.Objects.SaunaClassic::IsADog(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_00d8:  brtrue.s   IL_00f2

      IL_00da:  ldloc.s    V_4
      IL_00dc:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_00e1:  ldc.i8     0x9a7f5f1919df0017
      IL_00ea:  ldc.i4.0
      IL_00eb:  conv.i8
      IL_00ec:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_00f1:  pop
      IL_00f2:  ldloca.s   V_6
      IL_00f4:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::MoveNext()
      IL_00f9:  brtrue.s   IL_00c8

      IL_00fb:  leave.s    IL_010b

    }  // end .try
    finally
    {
      IL_00fd:  ldloca.s   V_6
      IL_00ff:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>
      IL_0105:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_010a:  endfinally
    }  // end handler
    IL_010b:  ret
  } // end of method SaunaClassic::DoProximityBuffs

  .method public hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       225 (0xe1)
    .maxstack  3
    IL_0000:  ldstr      "Sims3.Store.Objects"
    IL_0005:  ldc.i4.6
    IL_0006:  callvirt   instance string [mscorlib]System.String::Substring(int32)
    IL_000b:  ldc.i4.s   46
    IL_000d:  ldc.i4.s   47
    IL_000f:  callvirt   instance string [mscorlib]System.String::Replace(char,
                                                                          char)
    IL_0014:  ldstr      "/SaunaClassic"
    IL_0019:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_001e:  stsfld     string Sims3.Store.Objects.SaunaClassic::sLocalizationKey
    IL_0023:  ldc.i4.s   10
    IL_0025:  stsfld     int32 Sims3.Store.Objects.SaunaClassic::kComfyBuffScore
    IL_002a:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive::.ctor()
    IL_002f:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive Sims3.Store.Objects.SaunaClassic::kEnvironmentTuning
    IL_0034:  ldc.r4     20.
    IL_0039:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kBabyMadeChance
    IL_003e:  ldc.i4.3
    IL_003f:  newarr     [mscorlib]System.Single
    IL_0044:  dup
    IL_0045:  ldtoken    field valuetype '<PrivateImplementationDetails>{F4A196D8-CA02-4FC1-9349-6050E0768068}'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>{F4A196D8-CA02-4FC1-9349-6050E0768068}'::'$$method0x6000086-1'
    IL_004a:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004f:  stsfld     float32[] Sims3.Store.Objects.SaunaClassic::kBabyTraitChance
    IL_0054:  ldc.r4     30.
    IL_0059:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kBathDurationToRemoveFatigue
    IL_005e:  ldc.r4     2.
    IL_0063:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kProximityTestFrequency
    IL_0068:  ldc.r4     1.
    IL_006d:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kMudBathAgeExtension
    IL_0072:  ldc.r4     20.
    IL_0077:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kPrunedSkinChance
    IL_007c:  ldc.r4     15.
    IL_0081:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kFunBoostForSteam
    IL_0086:  ldc.r4     0.99699998
    IL_008b:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kWeightLossPerMinute
    IL_0090:  ldc.r4     60.
    IL_0095:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kBathLengthInMinutes
    IL_009a:  ldc.r4     2.
    IL_009f:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kBuffFitnessModifier
    IL_00a4:  ldc.r4     5.
    IL_00a9:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kGroupChatRelationshipBump
    IL_00ae:  ldc.r4     5.
    IL_00b3:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kGroupChatSocialBump
    IL_00b8:  ldc.r4     5.
    IL_00bd:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kBuffResetMotivesFrequency
    IL_00c2:  ldc.r4     100.
    IL_00c7:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooFunBump
    IL_00cc:  ldc.r4     25.
    IL_00d1:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooSocialBump
    IL_00d6:  ldc.r4     20.
    IL_00db:  stsfld     float32 Sims3.Store.Objects.SaunaClassic::kWooHooHygieneBump
    IL_00e0:  ret
  } // end of method SaunaClassic::.cctor

  .property instance int32 ComfyBuffScore()
  {
    .get instance int32 Sims3.Store.Objects.SaunaClassic::get_ComfyBuffScore()
  } // end of property SaunaClassic::ComfyBuffScore
  .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive
          EnvironmentTuning()
  {
    .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive Sims3.Store.Objects.SaunaClassic::get_EnvironmentTuning()
  } // end of property SaunaClassic::EnvironmentTuning
  .property instance string SitInteractionName()
  {
    .get instance string Sims3.Store.Objects.SaunaClassic::get_SitInteractionName()
  } // end of property SaunaClassic::SitInteractionName
} // end of class Sims3.Store.Objects.SaunaClassic

.class public auto ansi '<PrivateImplementationDetails>{F4A196D8-CA02-4FC1-9349-6050E0768068}'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi nested public '__StaticArrayInitTypeSize=12'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 12
  } // end of class '__StaticArrayInitTypeSize=12'

  .field static public valuetype '<PrivateImplementationDetails>{F4A196D8-CA02-4FC1-9349-6050E0768068}'/'__StaticArrayInitTypeSize=12' '$$method0x6000086-1' at I_00002A58
} // end of class '<PrivateImplementationDetails>{F4A196D8-CA02-4FC1-9349-6050E0768068}'


// =============================================================

.data cil I_00002A58 = bytearray (
                 00 00 F0 41 00 00 48 42 00 00 20 42)             // ...A..HB.. B
// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file D:\Downloads\For Games\Sims3 Downloads\Hacks\Coding\Sims3\Store\StoreObjectsSaunaClassic.res
