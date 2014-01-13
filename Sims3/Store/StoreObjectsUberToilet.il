
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern Sims3GameplayObjects
{
  .ver 1:0:0:0
}
.assembly extern Sims3GameplaySystems
{
  .ver 1:0:0:0
}
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern SimIFace
{
  .ver 1:0:0:50
}
.assembly extern UI
{
  .ver 1:0:0:0
}
.assembly StoreObjectsUberToilet
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 31 42 42 32 34 39 46 39 2D 33 34 39 44   // ..$1BB249F9-349D
                                                                                                  2D 46 42 34 30 2D 30 33 45 42 2D 46 39 45 35 39   // -FB40-03EB-F9E59
                                                                                                  32 45 41 45 35 37 32 00 00 )                      // 2EAE572..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1D 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  45 41 2E 63 6F 6D 20 32 30 30 37 2D 32 30 31 30   // EA.com 2007-2010
                                                                                                  00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 14 53 69 6D 73 33 53 74 6F 72 65 55 62 65   // ...Sims3StoreUbe
                                                                                                72 54 6F 69 6C 65 74 00 00 )                      // rToilet..
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 45 41 2E 63 6F 6D 00 00 )                // ...EA.com..
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 14 53 69 6D 73 33 53 74 6F 72 65 55 62 65   // ...Sims3StoreUbe
                                                                                              72 54 6F 69 6C 65 74 00 00 )                      // rToilet..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module StoreObjectsUberToilet.dll
// MVID: {071310FE-824A-4EA9-B054-C5B6B03FA7BA}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00340000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Sims3.Store.Objects.UberToilet
       extends [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet
{
  .custom instance void [SimIFace]Sims3.SimIFace.RuntimeExportAttribute::.ctor() = ( 01 00 00 00 ) 
  .class auto ansi nested public beforefieldinit UseToilet
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/UseToilet>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.UberToilet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       10 (0xa)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_InUse()
        IL_0006:  ldc.i4.0
        IL_0007:  ceq
        IL_0009:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters& parameters) cil managed
      {
        // Code size       33 (0x21)
        .maxstack  4
        .locals init (object[] V_0)
        IL_0000:  ldstr      "Gameplay/Objects/Plumbing/Toilet/UseToilet:Interac"
        + "tionName"
        IL_0005:  ldc.i4.1
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  stloc.0
        IL_000c:  ldloc.0
        IL_000d:  ldc.i4.0
        IL_000e:  ldarg.1
        IL_000f:  ldobj      [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters
        IL_0014:  box        [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters
        IL_0019:  stelem.ref
        IL_001a:  ldloc.0
        IL_001b:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                                object[])
        IL_0020:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/UseToilet>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation mSituation
    .field public bool mCensorEnabled
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       1425 (0x591)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance V_0,
               class [SimIFace]Sims3.SimIFace.StateMachineClient V_1,
               bool V_2,
               bool V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_4,
               bool V_5,
               class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Sink V_6)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_000b:  ldc.i8     0x8bfc001b295fe3b0
      IL_0014:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0019:  stloc.0
      IL_001a:  ldloc.0
      IL_001b:  brfalse.s  IL_0032

      IL_001d:  ldloc.0
      IL_001e:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
      IL_0023:  ldsfld     int32 Sims3.Store.Objects.UberToilet::kTimeoutRemainingForBladderEmergency
      IL_0028:  conv.r4
      IL_0029:  bgt.un.s   IL_0032

      IL_002b:  ldarg.0
      IL_002c:  ldc.i4.4
      IL_002d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::RequestWalkStyle(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/WalkStyle)
      IL_0032:  ldarg.0
      IL_0033:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0038:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SimQueue [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::Line
      IL_003d:  ldarg.0
      IL_003e:  ldc.i4.s   18
      IL_0040:  ldc.i4     0xffe9bfff
      IL_0045:  ldsfld     float32 Sims3.Store.Objects.UberToilet::kTimeToWaitInLine
      IL_004a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SimQueue::WaitForTurn(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SimQueue/WaitBehavior,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                 float32)
      IL_004f:  brtrue.s   IL_0053

      IL_0051:  ldc.i4.0
      IL_0052:  ret

      IL_0053:  ldarg.0
      IL_0054:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0059:  ldarg.0
      IL_005a:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_005f:  ldc.i4     0x31229a4c
      IL_0064:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlotAndCheckInUse(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_0069:  brtrue.s   IL_006d

      IL_006b:  ldc.i4.0
      IL_006c:  ret

      IL_006d:  ldarg.0
      IL_006e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::ClearRequestedWalkStyles()
      IL_0073:  ldarg.0
      IL_0074:  ldarg.0
      IL_0075:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_007a:  ldarg.0
      IL_007b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0080:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_LotCurrent()
      IL_0085:  call       class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation::Create(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                                   class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot)
      IL_008a:  stfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_008f:  ldarg.0
      IL_0090:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_0095:  brfalse.s  IL_00cb

      IL_0097:  ldarg.0
      IL_0098:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_009d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation::Start()
      IL_00a2:  brtrue.s   IL_00a6

      IL_00a4:  ldc.i4.0
      IL_00a5:  ret

      IL_00a6:  ldarg.0
      IL_00a7:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_00ac:  ldarg.0
      IL_00ad:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_00b2:  ldc.i4     0x31229a4c
      IL_00b7:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlotAndCheckInUse(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_00bc:  brtrue.s   IL_00cb

      IL_00be:  ldarg.0
      IL_00bf:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_00c4:  callvirt   instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation::ExitToiletSituation()
      IL_00c9:  ldc.i4.0
      IL_00ca:  ret

      IL_00cb:  ldarg.0
      IL_00cc:  ldc.i4.0
      IL_00cd:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::set_CancellableByPlayer(bool)
      IL_00d2:  ldarg.0
      IL_00d3:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry()
      IL_00d8:  ldarg.0
      IL_00d9:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_00de:  ldstr      "ubertoilet_store"
      IL_00e3:  call       class [SimIFace]Sims3.SimIFace.StateMachineClient [SimIFace]Sims3.SimIFace.StateMachineClient::Acquire(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                                                                  string)
      IL_00e8:  stloc.1
      IL_00e9:  ldloc.1
      IL_00ea:  ldc.i8     0xb82d0015b92943d0
      IL_00f3:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddInterest<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames>(!!0)
      IL_00f8:  ldloc.1
      IL_00f9:  ldstr      "x"
      IL_00fe:  ldarg.0
      IL_00ff:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0104:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0109:  ldloc.1
      IL_010a:  ldstr      "ubertoilet"
      IL_010f:  ldarg.0
      IL_0110:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0115:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_011a:  ldloc.1
      IL_011b:  ldstr      "x"
      IL_0120:  ldstr      "Enter"
      IL_0125:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_012a:  ldloc.1
      IL_012b:  ldstr      "isDirty"
      IL_0130:  ldarg.0
      IL_0131:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0136:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Cleanable()
      IL_013b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent::get_NeedsToBeCleaned()
      IL_0140:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0145:  ldloc.1
      IL_0146:  ldstr      "isBroken"
      IL_014b:  ldarg.0
      IL_014c:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0151:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0156:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_015b:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0160:  ldarg.0
      IL_0161:  ldc.i4.1
      IL_0162:  stfld      bool Sims3.Store.Objects.UberToilet/UseToilet::mCensorEnabled
      IL_0167:  ldarg.0
      IL_0168:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_016d:  ldc.i4.1
      IL_016e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::EnableCensor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/CensorType)
      IL_0173:  ldloc.1
      IL_0174:  ldc.i4.s   120
      IL_0176:  ldarg.0
      IL_0177:  ldftn      instance void Sims3.Store.Objects.UberToilet/UseToilet::TurnOffCensorGrid(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                     class [SimIFace]Sims3.SimIFace.IEvent)
      IL_017d:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0182:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0187:  ldloc.1
      IL_0188:  ldc.i4     0xc8
      IL_018d:  ldarg.0
      IL_018e:  ldftn      instance void Sims3.Store.Objects.UberToilet/UseToilet::TriggerTrapCallback(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                       class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0194:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0199:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_019e:  ldloc.0
      IL_019f:  brfalse.s  IL_01a8

      IL_01a1:  ldloc.0
      IL_01a2:  ldc.i4.1
      IL_01a3:  stfld      bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutPaused
      IL_01a8:  ldc.i4.0
      IL_01a9:  stloc.2
      IL_01aa:  ldarg.0
      IL_01ab:  ldarg.0
      IL_01ac:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_01b1:  call       instance bool Sims3.Store.Objects.UberToilet/UseToilet::ShouldSit(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_01b6:  brfalse    IL_024e

      IL_01bb:  ldc.i4.1
      IL_01bc:  stloc.2
      IL_01bd:  ldarg.0
      IL_01be:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_01c3:  ldloc.1
      IL_01c4:  callvirt   instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::PutDownSeat(class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_01c9:  ldloc.1
      IL_01ca:  ldstr      "x"
      IL_01cf:  ldstr      "peeSitting"
      IL_01d4:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_01d9:  ldarg.0
      IL_01da:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_01df:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SculptureComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_SculptureComponent()
      IL_01e4:  brfalse.s  IL_021d

      IL_01e6:  ldarg.0
      IL_01e7:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_01ec:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SculptureComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_SculptureComponent()
      IL_01f1:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SculptureComponent/SculptureMaterial [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SculptureComponent::get_Material()
      IL_01f6:  ldc.i4.s   16
      IL_01f8:  bne.un.s   IL_021d

      IL_01fa:  ldarg.0
      IL_01fb:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0200:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0205:  ldc.i8     0x3e0002fe4de54e0
      IL_020e:  ldc.i8     0x3e0002fe4de54f0
      IL_0217:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_021c:  pop
      IL_021d:  ldarg.0
      IL_021e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0223:  callvirt   instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::get_ToiletTuning()
      IL_0228:  ldfld      bool [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning::AutoFlushes
      IL_022d:  brfalse.s  IL_026a

      IL_022f:  ldsfld     int32 Sims3.Store.Objects.UberToilet::kChanceOfToiletAutoFlushWhileInUse
      IL_0234:  conv.r4
      IL_0235:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomChance(float32)
      IL_023a:  brfalse.s  IL_026a

      IL_023c:  ldloc.1
      IL_023d:  ldstr      "x"
      IL_0242:  ldstr      "flushReaction"
      IL_0247:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_024c:  br.s       IL_026a

      IL_024e:  ldarg.0
      IL_024f:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0254:  ldloc.1
      IL_0255:  callvirt   instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::PutUpSeat(class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_025a:  ldloc.1
      IL_025b:  ldstr      "x"
      IL_0260:  ldstr      "peeStanding"
      IL_0265:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_026a:  ldarg.0
      IL_026b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0270:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0275:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_027a:  brtrue.s   IL_0295

      IL_027c:  ldarg.0
      IL_027d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0282:  ldfld      valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Sims3.Store.Objects.UberToilet::mToiletOnStatus
      IL_0287:  ldc.i4.2
      IL_0288:  bne.un.s   IL_0295

      IL_028a:  ldarg.0
      IL_028b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0290:  callvirt   instance void Sims3.Store.Objects.UberToilet::StartMusic()
      IL_0295:  ldarg.0
      IL_0296:  ldc.i4     0x10f38d6
      IL_029b:  ldc.r4     0.0
      IL_02a0:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdate(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                                     float32)
      IL_02a5:  ldarg.0
      IL_02a6:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_02ab:  ldarg.0
      IL_02ac:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_02b1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_02b6:  ldarg.0
      IL_02b7:  ldc.i4     0x10f38d6
      IL_02bc:  ldsfld     float32 Sims3.Store.Objects.UberToilet::kMaxLengthUseToilet
      IL_02c1:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::LerpToFill(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                           valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                           float32)
      IL_02c6:  ldarg.0
      IL_02c7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StartStages()
      IL_02cc:  ldarg.0
      IL_02cd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_02d2:  ldc.i8     0x15e6013a7a137001
      IL_02db:  ldc.i8     0x8bfc0ffe01325d2
      IL_02e4:  ldc.i4.1
      IL_02e5:  ldc.i4.1
      IL_02e6:  ldc.i4.0
      IL_02e7:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultImaginaryFriend::GrantMilestoneBuff(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin,
                                                                                                                            bool,
                                                                                                                            bool,
                                                                                                                            bool)
      IL_02ec:  ldarg.0
      IL_02ed:  ldc.i4     0xffe93fef
      IL_02f2:  call       instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_02f7:  stloc.3
      IL_02f8:  ldarg.0
      IL_02f9:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_02fe:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0303:  ldc.i8     0x15e6013a7a137001
      IL_030c:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::UnpauseBuff(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0311:  ldarg.0
      IL_0312:  ldloc.3
      IL_0313:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_0318:  ldloc.3
      IL_0319:  brfalse.s  IL_0336

      IL_031b:  ldarg.0
      IL_031c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0321:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0326:  ldc.i4     0x10f38d6
      IL_032b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motive [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_0330:  ldc.i4.0
      IL_0331:  stfld      bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motive::PotionBladderDecayOverride
      IL_0336:  ldloc.0
      IL_0337:  brfalse.s  IL_0340

      IL_0339:  ldloc.0
      IL_033a:  ldc.i4.0
      IL_033b:  stfld      bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutPaused
      IL_0340:  ldarg.0
      IL_0341:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0346:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Cleanable()
      IL_034b:  ldarg.0
      IL_034c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0351:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent::DirtyInc(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor)
      IL_0356:  ldarg.0
      IL_0357:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_035c:  ldarg.0
      IL_035d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0362:  callvirt   instance bool [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::ShouldPutDownSeat(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor)
      IL_0367:  brfalse.s  IL_0375

      IL_0369:  ldarg.0
      IL_036a:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_036f:  ldloc.1
      IL_0370:  callvirt   instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::PutDownSeat(class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_0375:  ldnull
      IL_0376:  stloc.s    V_4
      IL_0378:  ldarg.0
      IL_0379:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_037e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SimQueue [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::Line
      IL_0383:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.SimQueue::MemberCount()
      IL_0388:  ldc.i4.1
      IL_0389:  cgt
      IL_038b:  stloc.s    V_5
      IL_038d:  ldarg.0
      IL_038e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0393:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0398:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_039d:  brfalse.s  IL_03c4

      IL_039f:  ldloc.2
      IL_03a0:  brfalse.s  IL_03b4

      IL_03a2:  ldloc.1
      IL_03a3:  ldstr      "x"
      IL_03a8:  ldstr      "electricBreakSitting"
      IL_03ad:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_03b2:  br.s       IL_03c4

      IL_03b4:  ldloc.1
      IL_03b5:  ldstr      "x"
      IL_03ba:  ldstr      "electricBreakStanding"
      IL_03bf:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_03c4:  ldarg.0
      IL_03c5:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_03ca:  brfalse.s  IL_03dc

      IL_03cc:  ldarg.0
      IL_03cd:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_03d2:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation::get_SomeoneDidIntrude()
      IL_03d7:  brtrue     IL_0471

      IL_03dc:  ldarg.0
      IL_03dd:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_03e2:  callvirt   instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::get_ToiletTuning()
      IL_03e7:  ldfld      bool [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning::AutoFlushes
      IL_03ec:  brtrue.s   IL_042d

      IL_03ee:  ldarg.0
      IL_03ef:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_03f4:  dup
      IL_03f5:  ldfld      int32 [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::ToiletVolume
      IL_03fa:  ldc.i4.1
      IL_03fb:  add
      IL_03fc:  stfld      int32 [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::ToiletVolume
      IL_0401:  ldarg.0
      IL_0402:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0407:  ldarg.0
      IL_0408:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_040d:  ldarg.0
      IL_040e:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0413:  callvirt   instance bool [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::ShouldFlush(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                   bool)
      IL_0418:  brfalse.s  IL_042d

      IL_041a:  ldarg.0
      IL_041b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0420:  ldarg.0
      IL_0421:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0426:  ldloc.1
      IL_0427:  ldc.i4.1
      IL_0428:  callvirt   instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::FlushToilet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                   class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                   bool)
      IL_042d:  ldarg.0
      IL_042e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0433:  ldarg.0
      IL_0434:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0439:  callvirt   instance bool [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::ShouldWashHands(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor)
      IL_043e:  brfalse.s  IL_0471

      IL_0440:  ldloc.s    V_5
      IL_0442:  brtrue.s   IL_0471

      IL_0444:  ldarg.0
      IL_0445:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_044a:  call       class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Sink [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::FindClosestSink(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_044f:  stloc.s    V_6
      IL_0451:  ldloc.s    V_6
      IL_0453:  brfalse.s  IL_0471

      IL_0455:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Sink/WashHands::Singleton
      IL_045a:  ldloc.s    V_6
      IL_045c:  ldarg.0
      IL_045d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0462:  ldarg.0
      IL_0463:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0468:  ldc.i4.0
      IL_0469:  ldc.i4.1
      IL_046a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_046f:  stloc.s    V_4
      IL_0471:  ldarg.0
      IL_0472:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0477:  ldfld      valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Sims3.Store.Objects.UberToilet::mToiletOnStatus
      IL_047c:  ldc.i4.2
      IL_047d:  bne.un.s   IL_048a

      IL_047f:  ldarg.0
      IL_0480:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0485:  callvirt   instance void Sims3.Store.Objects.UberToilet::StopMusic()
      IL_048a:  ldarg.0
      IL_048b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0490:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0495:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_049a:  brtrue     IL_053e

      IL_049f:  ldarg.0
      IL_04a0:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_04a5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_04aa:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_SelfCleaning()
      IL_04af:  brfalse.s  IL_0516

      IL_04b1:  ldarg.0
      IL_04b2:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_04b7:  ldfld      bool Sims3.Store.Objects.UberToilet::mSelfCleanOn
      IL_04bc:  brfalse.s  IL_0516

      IL_04be:  ldloc.1
      IL_04bf:  ldstr      "x"
      IL_04c4:  ldstr      "autoClean"
      IL_04c9:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_04ce:  ldarg.0
      IL_04cf:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_04d4:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Cleanable()
      IL_04d9:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent::ForceClean()
      IL_04de:  ldarg.0
      IL_04df:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_04e4:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_04e9:  ldc.i8     0xfd3d001e7a4a01e0
      IL_04f2:  ldc.i4.0
      IL_04f3:  conv.i8
      IL_04f4:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_04f9:  pop
      IL_04fa:  ldarg.0
      IL_04fb:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0500:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0505:  ldc.i4     0x10f38d9
      IL_050a:  ldsfld     float32 Sims3.Store.Objects.UberToilet::kChanceOfRelaxed
      IL_050f:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_0514:  br.s       IL_053e

      IL_0516:  ldsfld     float32 Sims3.Store.Objects.UberToilet::kChanceOfRelaxed
      IL_051b:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomChance(float32)
      IL_0520:  brfalse.s  IL_053e

      IL_0522:  ldarg.0
      IL_0523:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0528:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_052d:  ldc.i8     0xfd3d001e7a4a01e0
      IL_0536:  ldc.i4.0
      IL_0537:  conv.i8
      IL_0538:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_053d:  pop
      IL_053e:  ldloc.1
      IL_053f:  ldstr      "x"
      IL_0544:  ldstr      "Exit"
      IL_0549:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                   string)
      IL_054e:  ldarg.0
      IL_054f:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_0554:  brfalse.s  IL_0561

      IL_0556:  ldarg.0
      IL_0557:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_055c:  callvirt   instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation::ExitToiletSituation()
      IL_0561:  ldloc.s    V_5
      IL_0563:  brfalse.s  IL_0571

      IL_0565:  ldarg.0
      IL_0566:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_056b:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Situations.PrivacySituation::RouteToAdjacentRoom(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0570:  pop
      IL_0571:  ldarg.0
      IL_0572:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit()
      IL_0577:  ldloc.s    V_4
      IL_0579:  brfalse.s  IL_058f

      IL_057b:  ldarg.0
      IL_057c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0581:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0586:  ldloc.s    V_4
      IL_0588:  ldc.i4.0
      IL_0589:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_058e:  pop
      IL_058f:  ldloc.3
      IL_0590:  ret
    } // end of method UseToilet::Run

    .method public hidebysig instance void 
            TurnOffCensorGrid(class [SimIFace]Sims3.SimIFace.StateMachineClient sender,
                              class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_0006:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AutoEnableCensor()
      IL_000b:  ret
    } // end of method UseToilet::TurnOffCensorGrid

    .method public hidebysig instance void 
            TriggerTrapCallback(class [SimIFace]Sims3.SimIFace.StateMachineClient sender,
                                class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_000c:  callvirt   instance bool [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::TriggerTrap(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0011:  pop
      IL_0012:  ret
    } // end of method UseToilet::TriggerTrapCallback

    .method public hidebysig instance bool 
            ShouldSit(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim s) cil managed
    {
      // Code size       62 (0x3e)
      .maxstack  2
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance V_0)
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_0006:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.CAS.CASAgeGenderFlags [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_Gender()
      IL_000b:  ldc.i4     0x2000
      IL_0010:  bne.un.s   IL_0014

      IL_0012:  ldc.i4.1
      IL_0013:  ret

      IL_0014:  ldarg.1
      IL_0015:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_001a:  ldc.i8     0xb82d0015b92947f0
      IL_0023:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  brfalse.s  IL_003c

      IL_002c:  ldloc.0
      IL_002d:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
      IL_0032:  ldsfld     int32 Sims3.Store.Objects.UberToilet::kTimeoutRemainingToSit
      IL_0037:  conv.r4
      IL_0038:  bgt.un.s   IL_003c

      IL_003a:  ldc.i4.1
      IL_003b:  ret

      IL_003c:  ldc.i4.0
      IL_003d:  ret
    } // end of method UseToilet::ShouldSit

    .method public hidebysig virtual instance void 
            ConfigureInteraction() cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.TimedStage V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage[] V_1)
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetInteractionName()
      IL_0006:  ldsfld     float32 Sims3.Store.Objects.UberToilet::kMaxLengthUseToilet
      IL_000b:  ldc.i4.0
      IL_000c:  ldc.i4.1
      IL_000d:  ldc.i4.0
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
    } // end of method UseToilet::ConfigureInteraction

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       52 (0x34)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      bool Sims3.Store.Objects.UberToilet/UseToilet::mCensorEnabled
      IL_0006:  brfalse.s  IL_0013

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Actor
      IL_000e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AutoEnableCensor()
      IL_0013:  ldarg.0
      IL_0014:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_0019:  brfalse.s  IL_002d

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_0021:  callvirt   instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation::ExitToiletSituation()
      IL_0026:  ldarg.0
      IL_0027:  ldnull
      IL_0028:  stfld      class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/ToiletSituation Sims3.Store.Objects.UberToilet/UseToilet::mSituation
      IL_002d:  ldarg.0
      IL_002e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_0033:  ret
    } // end of method UseToilet::Cleanup

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::.ctor()
      IL_0006:  ret
    } // end of method UseToilet::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.UberToilet/UseToilet/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.UberToilet/UseToilet::Singleton
      IL_000a:  ret
    } // end of method UseToilet::.cctor

  } // end of class UseToilet

  .class auto ansi nested public beforefieldinit SelectMusic
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/SelectMusic>
    {
      .field public string MenuText
      .field public valuetype Sims3.Store.Objects.UberToilet/MusicOption musicOption
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     string [mscorlib]System.String::Empty
        IL_0006:  stfld      string Sims3.Store.Objects.UberToilet/SelectMusic/Definition::MenuText
        IL_000b:  ldarg.0
        IL_000c:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/SelectMusic>::.ctor()
        IL_0011:  ret
      } // end of method Definition::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(string text,
                                   valuetype Sims3.Store.Objects.UberToilet/MusicOption selectedSound) cil managed
      {
        // Code size       32 (0x20)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     string [mscorlib]System.String::Empty
        IL_0006:  stfld      string Sims3.Store.Objects.UberToilet/SelectMusic/Definition::MenuText
        IL_000b:  ldarg.0
        IL_000c:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/SelectMusic>::.ctor()
        IL_0011:  ldarg.0
        IL_0012:  ldarg.1
        IL_0013:  stfld      string Sims3.Store.Objects.UberToilet/SelectMusic/Definition::MenuText
        IL_0018:  ldarg.0
        IL_0019:  ldarg.2
        IL_001a:  stfld      valuetype Sims3.Store.Objects.UberToilet/MusicOption Sims3.Store.Objects.UberToilet/SelectMusic/Definition::musicOption
        IL_001f:  ret
      } // end of method Definition::.ctor

      .method public hidebysig virtual instance string[] 
              GetPath(bool isFemale) cil managed
      {
        // Code size       38 (0x26)
        .maxstack  4
        .locals init (string[] V_0)
        IL_0000:  ldc.i4.1
        IL_0001:  newarr     [mscorlib]System.String
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  ldc.i4.0
        IL_0009:  ldstr      "Store/Objects/UberToilet:SelectMusicMenuPath"
        IL_000e:  ldc.i4.0
        IL_000f:  newarr     [mscorlib]System.Object
        IL_0014:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                                object[])
        IL_0019:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::get_Ellipsis()
        IL_001e:  call       string [mscorlib]System.String::Concat(string,
                                                                    string)
        IL_0023:  stelem.ref
        IL_0024:  ldloc.0
        IL_0025:  ret
      } // end of method Definition::GetPath

      .method public hidebysig virtual instance void 
              AddInteractions(class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop,
                              class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                              class Sims3.Store.Objects.UberToilet target,
                              class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair> results) cil managed
      {
        // Code size       125 (0x7d)
        .maxstack  4
        .locals init (valuetype Sims3.Store.Objects.UberToilet/MusicOption V_0,
                 class [mscorlib]System.Collections.IEnumerator V_1,
                 class [mscorlib]System.IDisposable V_2)
        IL_0000:  ldtoken    Sims3.Store.Objects.UberToilet/MusicOption
        IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_000a:  call       class [mscorlib]System.Array [mscorlib]System.Enum::GetValues(class [mscorlib]System.Type)
        IL_000f:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Array::GetEnumerator()
        IL_0014:  stloc.1
        .try
        {
          IL_0015:  br.s       IL_0061

          IL_0017:  ldloc.1
          IL_0018:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
          IL_001d:  unbox.any  Sims3.Store.Objects.UberToilet/MusicOption
          IL_0022:  stloc.0
          IL_0023:  ldarg.s    results
          IL_0025:  ldarg.2
          IL_0026:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
          IL_002b:  ldstr      "Music"
          IL_0030:  ldloc.0
          IL_0031:  box        Sims3.Store.Objects.UberToilet/MusicOption
          IL_0036:  callvirt   instance string [mscorlib]System.Object::ToString()
          IL_003b:  call       string [mscorlib]System.String::Concat(string,
                                                                      string)
          IL_0040:  ldc.i4.0
          IL_0041:  newarr     [mscorlib]System.Object
          IL_0046:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
          IL_004b:  ldloc.0
          IL_004c:  newobj     instance void Sims3.Store.Objects.UberToilet/SelectMusic/Definition::.ctor(string,
                                                                                                          valuetype Sims3.Store.Objects.UberToilet/MusicOption)
          IL_0051:  ldarg.1
          IL_0052:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair::get_Target()
          IL_0057:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
          IL_005c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair>::Add(!0)
          IL_0061:  ldloc.1
          IL_0062:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
          IL_0067:  brtrue.s   IL_0017

          IL_0069:  leave.s    IL_007c

        }  // end .try
        finally
        {
          IL_006b:  ldloc.1
          IL_006c:  isinst     [mscorlib]System.IDisposable
          IL_0071:  stloc.2
          IL_0072:  ldloc.2
          IL_0073:  brfalse.s  IL_007b

          IL_0075:  ldloc.2
          IL_0076:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          IL_007b:  endfinally
        }  // end handler
        IL_007c:  ret
      } // end of method Definition::AddInteractions

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.UberToilet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      string Sims3.Store.Objects.UberToilet/SelectMusic/Definition::MenuText
        IL_0006:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.UberToilet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       151 (0x97)
        .maxstack  6
        .locals init (object[] V_0,
                 object[] V_1,
                 object[] V_2)
        IL_0000:  ldarg.2
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
        IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
        IL_000b:  brfalse.s  IL_0033

        IL_000d:  ldarg.s    greyedOutTooltipCallback
        IL_000f:  ldarg.1
        IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0015:  ldstr      "SelectMusicBroken"
        IL_001a:  ldc.i4.1
        IL_001b:  newarr     [mscorlib]System.Object
        IL_0020:  stloc.0
        IL_0021:  ldloc.0
        IL_0022:  ldc.i4.0
        IL_0023:  ldarg.1
        IL_0024:  stelem.ref
        IL_0025:  ldloc.0
        IL_0026:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
                                                                                   string,
                                                                                   object[])
        IL_002b:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_0030:  stind.ref
        IL_0031:  ldc.i4.0
        IL_0032:  ret

        IL_0033:  ldarg.2
        IL_0034:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_InUse()
        IL_0039:  brfalse.s  IL_0061

        IL_003b:  ldarg.s    greyedOutTooltipCallback
        IL_003d:  ldarg.1
        IL_003e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0043:  ldstr      "SelectMusicUse"
        IL_0048:  ldc.i4.1
        IL_0049:  newarr     [mscorlib]System.Object
        IL_004e:  stloc.1
        IL_004f:  ldloc.1
        IL_0050:  ldc.i4.0
        IL_0051:  ldarg.1
        IL_0052:  stelem.ref
        IL_0053:  ldloc.1
        IL_0054:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
                                                                                   string,
                                                                                   object[])
        IL_0059:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_005e:  stind.ref
        IL_005f:  ldc.i4.0
        IL_0060:  ret

        IL_0061:  ldarg.2
        IL_0062:  ldfld      int32 Sims3.Store.Objects.UberToilet::mSelectedSound
        IL_0067:  ldarg.0
        IL_0068:  ldfld      valuetype Sims3.Store.Objects.UberToilet/MusicOption Sims3.Store.Objects.UberToilet/SelectMusic/Definition::musicOption
        IL_006d:  bne.un.s   IL_0095

        IL_006f:  ldarg.s    greyedOutTooltipCallback
        IL_0071:  ldarg.1
        IL_0072:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0077:  ldstr      "SelectMusicSelected"
        IL_007c:  ldc.i4.1
        IL_007d:  newarr     [mscorlib]System.Object
        IL_0082:  stloc.2
        IL_0083:  ldloc.2
        IL_0084:  ldc.i4.0
        IL_0085:  ldarg.1
        IL_0086:  stelem.ref
        IL_0087:  ldloc.2
        IL_0088:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
                                                                                   string,
                                                                                   object[])
        IL_008d:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_0092:  stind.ref
        IL_0093:  ldc.i4.0
        IL_0094:  ret

        IL_0095:  ldc.i4.1
        IL_0096:  ret
      } // end of method Definition::Test

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  2
      .locals init (class Sims3.Store.Objects.UberToilet/SelectMusic/Definition V_0)
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0006:  isinst     Sims3.Store.Objects.UberToilet/SelectMusic/Definition
      IL_000b:  stloc.0
      IL_000c:  ldarg.0
      IL_000d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0012:  ldloc.0
      IL_0013:  ldfld      valuetype Sims3.Store.Objects.UberToilet/MusicOption Sims3.Store.Objects.UberToilet/SelectMusic/Definition::musicOption
      IL_0018:  stfld      int32 Sims3.Store.Objects.UberToilet::mSelectedSound
      IL_001d:  ldarg.0
      IL_001e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0023:  ldfld      valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Sims3.Store.Objects.UberToilet::mToiletOnStatus
      IL_0028:  ldc.i4.1
      IL_0029:  bne.un.s   IL_0036

      IL_002b:  ldarg.0
      IL_002c:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0031:  callvirt   instance void Sims3.Store.Objects.UberToilet::StartMusic()
      IL_0036:  ldc.i4.1
      IL_0037:  ret
    } // end of method SelectMusic::Run

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::.ctor()
      IL_0006:  ret
    } // end of method SelectMusic::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.UberToilet/SelectMusic/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.UberToilet/SelectMusic::Singleton
      IL_000a:  ret
    } // end of method SelectMusic::.cctor

  } // end of class SelectMusic

  .class auto ansi nested public beforefieldinit TurnMusicOnOff
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/TurnMusicOnOff>
    {
      .field public string MenuText
      .field public valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus toiletStatus
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     string [mscorlib]System.String::Empty
        IL_0006:  stfld      string Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition::MenuText
        IL_000b:  ldarg.0
        IL_000c:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/TurnMusicOnOff>::.ctor()
        IL_0011:  ret
      } // end of method Definition::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(string text,
                                   valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus selectedStatus) cil managed
      {
        // Code size       32 (0x20)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     string [mscorlib]System.String::Empty
        IL_0006:  stfld      string Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition::MenuText
        IL_000b:  ldarg.0
        IL_000c:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/TurnMusicOnOff>::.ctor()
        IL_0011:  ldarg.0
        IL_0012:  ldarg.1
        IL_0013:  stfld      string Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition::MenuText
        IL_0018:  ldarg.0
        IL_0019:  ldarg.2
        IL_001a:  stfld      valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition::toiletStatus
        IL_001f:  ret
      } // end of method Definition::.ctor

      .method public hidebysig virtual instance string[] 
              GetPath(bool isFemale) cil managed
      {
        // Code size       38 (0x26)
        .maxstack  4
        .locals init (string[] V_0)
        IL_0000:  ldc.i4.1
        IL_0001:  newarr     [mscorlib]System.String
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  ldc.i4.0
        IL_0009:  ldstr      "Store/Objects/UberToilet:TurnOnToiletMenuPath"
        IL_000e:  ldc.i4.0
        IL_000f:  newarr     [mscorlib]System.Object
        IL_0014:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                                object[])
        IL_0019:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::get_Ellipsis()
        IL_001e:  call       string [mscorlib]System.String::Concat(string,
                                                                    string)
        IL_0023:  stelem.ref
        IL_0024:  ldloc.0
        IL_0025:  ret
      } // end of method Definition::GetPath

      .method public hidebysig virtual instance void 
              AddInteractions(class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop,
                              class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                              class Sims3.Store.Objects.UberToilet target,
                              class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair> results) cil managed
      {
        // Code size       125 (0x7d)
        .maxstack  4
        .locals init (valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus V_0,
                 class [mscorlib]System.Collections.IEnumerator V_1,
                 class [mscorlib]System.IDisposable V_2)
        IL_0000:  ldtoken    Sims3.Store.Objects.UberToilet/ToiletOnStatus
        IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_000a:  call       class [mscorlib]System.Array [mscorlib]System.Enum::GetValues(class [mscorlib]System.Type)
        IL_000f:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Array::GetEnumerator()
        IL_0014:  stloc.1
        .try
        {
          IL_0015:  br.s       IL_0061

          IL_0017:  ldloc.1
          IL_0018:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
          IL_001d:  unbox.any  Sims3.Store.Objects.UberToilet/ToiletOnStatus
          IL_0022:  stloc.0
          IL_0023:  ldarg.s    results
          IL_0025:  ldarg.2
          IL_0026:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
          IL_002b:  ldstr      "SetSound"
          IL_0030:  ldloc.0
          IL_0031:  box        Sims3.Store.Objects.UberToilet/ToiletOnStatus
          IL_0036:  callvirt   instance string [mscorlib]System.Object::ToString()
          IL_003b:  call       string [mscorlib]System.String::Concat(string,
                                                                      string)
          IL_0040:  ldc.i4.0
          IL_0041:  newarr     [mscorlib]System.Object
          IL_0046:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
          IL_004b:  ldloc.0
          IL_004c:  newobj     instance void Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition::.ctor(string,
                                                                                                             valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus)
          IL_0051:  ldarg.1
          IL_0052:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair::get_Target()
          IL_0057:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
          IL_005c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair>::Add(!0)
          IL_0061:  ldloc.1
          IL_0062:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
          IL_0067:  brtrue.s   IL_0017

          IL_0069:  leave.s    IL_007c

        }  // end .try
        finally
        {
          IL_006b:  ldloc.1
          IL_006c:  isinst     [mscorlib]System.IDisposable
          IL_0071:  stloc.2
          IL_0072:  ldloc.2
          IL_0073:  brfalse.s  IL_007b

          IL_0075:  ldloc.2
          IL_0076:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          IL_007b:  endfinally
        }  // end handler
        IL_007c:  ret
      } // end of method Definition::AddInteractions

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.UberToilet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      string Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition::MenuText
        IL_0006:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.UberToilet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       99 (0x63)
        .maxstack  6
        .locals init (object[] V_0,
                 object[] V_1)
        IL_0000:  ldarg.2
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
        IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
        IL_000b:  brfalse.s  IL_0033

        IL_000d:  ldarg.s    greyedOutTooltipCallback
        IL_000f:  ldarg.1
        IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0015:  ldstr      "SelectMusicBroken"
        IL_001a:  ldc.i4.1
        IL_001b:  newarr     [mscorlib]System.Object
        IL_0020:  stloc.0
        IL_0021:  ldloc.0
        IL_0022:  ldc.i4.0
        IL_0023:  ldarg.1
        IL_0024:  stelem.ref
        IL_0025:  ldloc.0
        IL_0026:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
                                                                                   string,
                                                                                   object[])
        IL_002b:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_0030:  stind.ref
        IL_0031:  ldc.i4.0
        IL_0032:  ret

        IL_0033:  ldarg.2
        IL_0034:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_InUse()
        IL_0039:  brfalse.s  IL_0061

        IL_003b:  ldarg.s    greyedOutTooltipCallback
        IL_003d:  ldarg.1
        IL_003e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0043:  ldstr      "SelectMusicUse"
        IL_0048:  ldc.i4.1
        IL_0049:  newarr     [mscorlib]System.Object
        IL_004e:  stloc.1
        IL_004f:  ldloc.1
        IL_0050:  ldc.i4.0
        IL_0051:  ldarg.1
        IL_0052:  stelem.ref
        IL_0053:  ldloc.1
        IL_0054:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
                                                                                   string,
                                                                                   object[])
        IL_0059:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_005e:  stind.ref
        IL_005f:  ldc.i4.0
        IL_0060:  ret

        IL_0061:  ldc.i4.1
        IL_0062:  ret
      } // end of method Definition::Test

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       87 (0x57)
      .maxstack  2
      .locals init (class Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition V_0,
               valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus V_1)
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0006:  isinst     Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition
      IL_000b:  stloc.0
      IL_000c:  ldarg.0
      IL_000d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0012:  ldloc.0
      IL_0013:  ldfld      valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition::toiletStatus
      IL_0018:  stfld      valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Sims3.Store.Objects.UberToilet::mToiletOnStatus
      IL_001d:  ldarg.0
      IL_001e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0023:  ldfld      valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Sims3.Store.Objects.UberToilet::mToiletOnStatus
      IL_0028:  stloc.1
      IL_0029:  ldloc.1
      IL_002a:  switch     ( 
                            IL_004a,
                            IL_003d,
                            IL_004a)
      IL_003b:  br.s       IL_0055

      IL_003d:  ldarg.0
      IL_003e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0043:  callvirt   instance void Sims3.Store.Objects.UberToilet::StartMusic()
      IL_0048:  br.s       IL_0055

      IL_004a:  ldarg.0
      IL_004b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0050:  callvirt   instance void Sims3.Store.Objects.UberToilet::StopMusic()
      IL_0055:  ldc.i4.1
      IL_0056:  ret
    } // end of method TurnMusicOnOff::Run

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::.ctor()
      IL_0006:  ret
    } // end of method TurnMusicOnOff::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.UberToilet/TurnMusicOnOff/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.UberToilet/TurnMusicOnOff::Singleton
      IL_000a:  ret
    } // end of method TurnMusicOnOff::.cctor

  } // end of class TurnMusicOnOff

  .class auto ansi nested public beforefieldinit SelfCleanOnOff
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/SelfCleanOnOff>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.UberToilet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
        IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_SelfCleaning()
        IL_000b:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.UberToilet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       54 (0x36)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  ldfld      bool Sims3.Store.Objects.UberToilet::mSelfCleanOn
        IL_0006:  brfalse.s  IL_001f

        IL_0008:  ldarg.1
        IL_0009:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_000e:  ldstr      "SelfCleanOff"
        IL_0013:  ldc.i4.0
        IL_0014:  newarr     [mscorlib]System.Object
        IL_0019:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
                                                                                   string,
                                                                                   object[])
        IL_001e:  ret

        IL_001f:  ldarg.1
        IL_0020:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0025:  ldstr      "SelfCleanOn"
        IL_002a:  ldc.i4.0
        IL_002b:  newarr     [mscorlib]System.Object
        IL_0030:  call       string Sims3.Store.Objects.UberToilet::LocalizeString(bool,
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
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet,class Sims3.Store.Objects.UberToilet/SelfCleanOnOff>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       54 (0x36)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_000c:  ldfld      bool Sims3.Store.Objects.UberToilet::mSelfCleanOn
      IL_0011:  ldc.i4.0
      IL_0012:  ceq
      IL_0014:  stfld      bool Sims3.Store.Objects.UberToilet::mSelfCleanOn
      IL_0019:  ldarg.0
      IL_001a:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_001f:  callvirt   instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::get_ToiletTuning()
      IL_0024:  ldarg.0
      IL_0025:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::Target
      IL_002a:  ldfld      bool Sims3.Store.Objects.UberToilet::mSelfCleanOn
      IL_002f:  stfld      bool [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning::AutoFlushes
      IL_0034:  ldc.i4.1
      IL_0035:  ret
    } // end of method SelfCleanOnOff::Run

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.UberToilet>::.ctor()
      IL_0006:  ret
    } // end of method SelfCleanOnOff::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.UberToilet/SelfCleanOnOff/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.UberToilet/SelfCleanOnOff::Singleton
      IL_000a:  ret
    } // end of method SelfCleanOnOff::.cctor

  } // end of class SelfCleanOnOff

  .class auto ansi nested public MusicOption
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype Sims3.Store.Objects.UberToilet/MusicOption Sound1 = int32(0x00000000)
    .field public static literal valuetype Sims3.Store.Objects.UberToilet/MusicOption Sound2 = int32(0x00000001)
    .field public static literal valuetype Sims3.Store.Objects.UberToilet/MusicOption Sound3 = int32(0x00000002)
  } // end of class MusicOption

  .class auto ansi nested public ToiletOnStatus
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus OFF = int32(0x00000000)
    .field public static literal valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus ON = int32(0x00000001)
    .field public static literal valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Auto = int32(0x00000002)
  } // end of class ToiletOnStatus

  .field public static string sLocalizationKey
  .field public class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData sittingData
  .field public int32 mSelectedSound
  .field public valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus mToiletOnStatus
  .field public bool mSelfCleanOn
  .field public static string[] mMusicOptions
  .field public class [SimIFace]Sims3.SimIFace.ObjectSound mMusicSoundHandle
  .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive kEnvironmentTuning
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent/Tuning kCleanableTuning
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning kRepairableTuning
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning kToiletTuning
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kMaxLengthUseToilet
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static int32 kTimeoutRemainingForBladderEmergency
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 62 49 66 20 74 68 65 20 68 61 73 54 6F 50   // ..bIf the hasToP
                                                                                            65 65 20 74 69 6D 65 6F 75 74 20 68 61 73 20 6C   // ee timeout has l
                                                                                            65 73 73 20 74 69 6D 65 20 72 65 6D 61 69 6E 69   // ess time remaini
                                                                                            6E 67 20 74 68 61 6E 20 74 68 69 73 20 69 6E 20   // ng than this in 
                                                                                            6D 69 6E 75 74 65 73 2C 20 79 6F 75 72 20 53 69   // minutes, your Si
                                                                                            6D 20 72 75 6E 73 20 74 6F 20 74 68 65 20 74 6F   // m runs to the to
                                                                                            69 6C 65 74 2E 00 00 )                            // ilet...
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kTimeToWaitInLine
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 72 52 61 6E 67 65 3A 20 50 6F 73 69 74 69   // ..rRange: Positi
                                                                                            76 65 20 66 6C 6F 61 74 73 2E 20 20 44 65 73 63   // ve floats.  Desc
                                                                                            72 69 70 74 69 6F 6E 3A 20 41 6D 6F 75 6E 74 20   // ription: Amount 
                                                                                            6F 66 20 53 69 6D 20 6D 69 6E 75 74 65 73 20 61   // of Sim minutes a
                                                                                            20 53 69 6D 20 77 69 6C 6C 20 77 61 69 74 20 69   //  Sim will wait i
                                                                                            6E 20 6C 69 6E 65 20 62 65 66 6F 72 65 20 74 69   // n line before ti
                                                                                            6D 69 6E 67 20 6F 75 74 20 61 6E 64 20 65 78 69   // ming out and exi
                                                                                            74 69 6E 67 2E 00 00 )                            // ting...
  .field public static int32 kChanceOfToiletAutoFlushWhileInUse
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 68 54 68 69 73 20 76 61 6C 75 65 20 69 73   // ..hThis value is
                                                                                            20 74 68 65 20 63 68 61 6E 63 65 20 62 65 74 77   //  the chance betw
                                                                                            65 65 6E 20 30 20 61 6E 64 20 31 30 30 20 6F 66   // een 0 and 100 of
                                                                                            20 61 20 61 75 74 6F 20 66 6C 75 73 68 69 6E 67   //  a auto flushing
                                                                                            20 74 6F 69 6C 65 74 20 66 6C 75 73 68 69 6E 67   //  toilet flushing
                                                                                            20 77 68 69 6C 65 20 74 68 65 20 53 69 6D 20 69   //  while the Sim i
                                                                                            73 20 75 73 69 6E 67 20 69 74 2E 00 00 )          // s using it...
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static int32 kTimeoutRemainingToSit
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 54 49 66 20 74 68 65 20 68 61 73 54 6F 50   // ..TIf the hasToP
                                                                                            65 65 20 74 69 6D 65 6F 75 74 20 68 61 73 20 6C   // ee timeout has l
                                                                                            65 73 73 20 74 69 6D 65 20 72 65 6D 61 69 6E 69   // ess time remaini
                                                                                            6E 67 20 74 68 61 6E 20 74 68 69 73 20 69 6E 20   // ng than this in 
                                                                                            6D 69 6E 75 74 65 73 2C 20 74 68 65 79 20 77 69   // minutes, they wi
                                                                                            6C 6C 20 73 69 74 2E 00 00 )                      // ll sit...
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kChanceOfRelaxed
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 54 43 68 61 6E 63 65 20 6F 66 20 67 65 74   // ..TChance of get
                                                                                            74 69 6E 67 20 72 65 6C 61 78 65 64 20 6D 6F 6F   // ting relaxed moo
                                                                                            64 6C 65 74 20 77 69 74 68 20 6E 6F 6E 75 70 67   // dlet with nonupg
                                                                                            72 61 64 65 20 74 6F 69 6C 65 74 2E 20 20 66 6C   // rade toilet.  fl
                                                                                            6F 61 74 2C 20 67 70 65 20 64 65 66 61 75 6C 74   // oat, gpe default
                                                                                            20 33 30 2E 30 66 2E 00 00 )                      //  30.0f...
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
    IL_0001:  ldsfld     string Sims3.Store.Objects.UberToilet::sLocalizationKey
    IL_0006:  ldarg.1
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  ldarg.2
    IL_000d:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(bool,
                                                                                                            string,
                                                                                                            object[])
    IL_0012:  ret
  } // end of method UberToilet::LocalizeString

  .method public hidebysig specialname virtual 
          instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive 
          get_EnvironmentTuning() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive Sims3.Store.Objects.UberToilet::kEnvironmentTuning
    IL_0005:  ret
  } // end of method UberToilet::get_EnvironmentTuning

  .method public hidebysig specialname virtual 
          instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent/Tuning 
          get_CleanableTuning() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent/Tuning Sims3.Store.Objects.UberToilet::kCleanableTuning
    IL_0005:  ret
  } // end of method UberToilet::get_CleanableTuning

  .method public hidebysig specialname virtual 
          instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning 
          get_RepairableTuning() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning Sims3.Store.Objects.UberToilet::kRepairableTuning
    IL_0005:  ret
  } // end of method UberToilet::get_RepairableTuning

  .method public hidebysig specialname virtual 
          instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning 
          get_ToiletTuning() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning Sims3.Store.Objects.UberToilet::kToiletTuning
    IL_0005:  ret
  } // end of method UberToilet::get_ToiletTuning

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4     0x31229a4c
    IL_0007:  ldc.i4.0
    IL_0008:  ldstr      ""
    IL_000d:  ldc.i4.0
    IL_000e:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData::.ctor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.PartArea,
                                                                                                            valuetype [SimIFace]Sims3.SimIFace.Slot,
                                                                                                            valuetype [SimIFace]Sims3.SimIFace.Slot,
                                                                                                            string,
                                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitStyle)
    IL_0013:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData Sims3.Store.Objects.UberToilet::sittingData
    IL_0018:  ldarg.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 Sims3.Store.Objects.UberToilet::mSelectedSound
    IL_001f:  ldarg.0
    IL_0020:  ldc.i4.1
    IL_0021:  stfld      bool Sims3.Store.Objects.UberToilet::mSelfCleanOn
    IL_0026:  ldarg.0
    IL_0027:  call       instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::.ctor()
    IL_002c:  ret
  } // end of method UberToilet::.ctor

  .method public hidebysig virtual instance void 
          OnStartup() cil managed
  {
    // Code size       329 (0x149)
    .maxstack  6
    .locals init (class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade> V_0,
             object[] V_1,
             class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData[] V_2,
             object[] V_3,
             object[] V_4,
             object[] V_5,
             object[] V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.3
    IL_0002:  newarr     [mscorlib]System.Object
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  ldarg.0
    IL_000b:  callvirt   instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::get_ToiletTuning()
    IL_0010:  ldfld      int32 [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning::ComfyBuffScore
    IL_0015:  box        [mscorlib]System.Int32
    IL_001a:  stelem.ref
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  ldsfld     string Sims3.Store.Objects.UberToilet::sLocalizationKey
    IL_0022:  ldstr      ":SitInteractionName"
    IL_0027:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_002c:  stelem.ref
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.2
    IL_002f:  ldc.i4.1
    IL_0030:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData
    IL_0035:  stloc.2
    IL_0036:  ldloc.2
    IL_0037:  ldc.i4.0
    IL_0038:  ldarg.0
    IL_0039:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData Sims3.Store.Objects.UberToilet::sittingData
    IL_003e:  stelem.ref
    IL_003f:  ldloc.2
    IL_0040:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SitData>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0045:  stelem.ref
    IL_0046:  ldloc.1
    IL_0047:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SittableComponent>(object[])
    IL_004c:  pop
    IL_004d:  ldarg.0
    IL_004e:  ldc.i4.1
    IL_004f:  newarr     [mscorlib]System.Object
    IL_0054:  stloc.3
    IL_0055:  ldloc.3
    IL_0056:  ldc.i4.0
    IL_0057:  ldarg.0
    IL_0058:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.BoobyTrapComponent/BoobyTrapTuning [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::get_BoobyTrapTuning()
    IL_005d:  stelem.ref
    IL_005e:  ldloc.3
    IL_005f:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.BoobyTrapComponent>(object[])
    IL_0064:  pop
    IL_0065:  ldarg.0
    IL_0066:  ldc.i4.1
    IL_0067:  newarr     [mscorlib]System.Object
    IL_006c:  stloc.s    V_4
    IL_006e:  ldloc.s    V_4
    IL_0070:  ldc.i4.0
    IL_0071:  ldarg.0
    IL_0072:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent/Tuning [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::get_CleanableTuning()
    IL_0077:  stelem.ref
    IL_0078:  ldloc.s    V_4
    IL_007a:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.ToiletCleanable>(object[])
    IL_007f:  pop
    IL_0080:  ldarg.0
    IL_0081:  ldc.i4.2
    IL_0082:  newarr     [mscorlib]System.Object
    IL_0087:  stloc.s    V_5
    IL_0089:  ldloc.s    V_5
    IL_008b:  ldc.i4.0
    IL_008c:  ldc.i4.1
    IL_008d:  box        [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/RepairableType
    IL_0092:  stelem.ref
    IL_0093:  ldloc.s    V_5
    IL_0095:  ldc.i4.1
    IL_0096:  ldarg.0
    IL_0097:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet::get_RepairableTuning()
    IL_009c:  stelem.ref
    IL_009d:  ldloc.s    V_5
    IL_009f:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.ToiletRepairable>(object[])
    IL_00a4:  pop
    IL_00a5:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade>::.ctor()
    IL_00aa:  stloc.0
    IL_00ab:  ldloc.0
    IL_00ac:  ldc.i4.1
    IL_00ad:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade>::Add(!0)
    IL_00b2:  ldloc.0
    IL_00b3:  ldc.i4.2
    IL_00b4:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade>::Add(!0)
    IL_00b9:  ldarg.0
    IL_00ba:  ldc.i4.1
    IL_00bb:  newarr     [mscorlib]System.Object
    IL_00c0:  stloc.s    V_6
    IL_00c2:  ldloc.s    V_6
    IL_00c4:  ldc.i4.0
    IL_00c5:  ldloc.0
    IL_00c6:  stelem.ref
    IL_00c7:  ldloc.s    V_6
    IL_00c9:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent>(object[])
    IL_00ce:  pop
    IL_00cf:  ldarg.0
    IL_00d0:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.UberToilet/UseToilet::Singleton
    IL_00d5:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00da:  ldarg.0
    IL_00db:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Repair::Singleton
    IL_00e0:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00e5:  ldarg.0
    IL_00e6:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Clean::Singleton
    IL_00eb:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00f0:  ldarg.0
    IL_00f1:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Vomit::Singleton
    IL_00f6:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00fb:  ldarg.0
    IL_00fc:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Flush::Singleton
    IL_0101:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0106:  ldarg.0
    IL_0107:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/UpgradeSelfCleaning::Singleton
    IL_010c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0111:  ldarg.0
    IL_0112:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/UpgradePreventBreakage::Singleton
    IL_0117:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_011c:  ldarg.0
    IL_011d:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tinker::Singleton
    IL_0122:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0127:  ldarg.0
    IL_0128:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.UberToilet/SelectMusic::Singleton
    IL_012d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0132:  ldarg.0
    IL_0133:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.UberToilet/TurnMusicOnOff::Singleton
    IL_0138:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_013d:  ldarg.0
    IL_013e:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.UberToilet/SelfCleanOnOff::Singleton
    IL_0143:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0148:  ret
  } // end of method UberToilet::OnStartup

  .method public hidebysig virtual instance void 
          Dispose() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.UberToilet::StopMusic()
    IL_0006:  ldarg.0
    IL_0007:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::Dispose()
    IL_000c:  ret
  } // end of method UberToilet::Dispose

  .method public hidebysig virtual instance void 
          DoReset(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/ResetInformation resetInformation) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.UberToilet::StopMusic()
    IL_0006:  ldarg.0
    IL_0007:  ldfld      valuetype Sims3.Store.Objects.UberToilet/ToiletOnStatus Sims3.Store.Objects.UberToilet::mToiletOnStatus
    IL_000c:  ldc.i4.1
    IL_000d:  bne.un.s   IL_0015

    IL_000f:  ldarg.0
    IL_0010:  call       instance void Sims3.Store.Objects.UberToilet::StartMusic()
    IL_0015:  ldarg.0
    IL_0016:  ldarg.1
    IL_0017:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::DoReset(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/ResetInformation)
    IL_001c:  ret
  } // end of method UberToilet::DoReset

  .method public hidebysig specialname virtual 
          instance string  get_OverflowFxName() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      "s30_toiletModerateOverflow"
    IL_0005:  ret
  } // end of method UberToilet::get_OverflowFxName

  .method public hidebysig instance void 
          StartMusic() cil managed
  {
    // Code size       80 (0x50)
    .maxstack  4
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.UberToilet::StopMusic()
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 Sims3.Store.Objects.UberToilet::mSelectedSound
    IL_000c:  ldc.i4.m1
    IL_000d:  bne.un.s   IL_001c

    IL_000f:  ldsfld     string[] Sims3.Store.Objects.UberToilet::mMusicOptions
    IL_0014:  call       !!0 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetRandomObjectFromList<string>(!!0[])
    IL_0019:  stloc.0
    IL_001a:  br.s       IL_0029

    IL_001c:  ldsfld     string[] Sims3.Store.Objects.UberToilet::mMusicOptions
    IL_0021:  ldarg.0
    IL_0022:  ldfld      int32 Sims3.Store.Objects.UberToilet::mSelectedSound
    IL_0027:  ldelem.ref
    IL_0028:  stloc.0
    IL_0029:  ldarg.0
    IL_002a:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.UberToilet::mMusicSoundHandle
    IL_002f:  brtrue.s   IL_004f

    IL_0031:  ldarg.0
    IL_0032:  ldarg.0
    IL_0033:  call       instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
    IL_0038:  ldloc.0
    IL_0039:  newobj     instance void [SimIFace]Sims3.SimIFace.ObjectSound::.ctor(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                   string)
    IL_003e:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.UberToilet::mMusicSoundHandle
    IL_0043:  ldarg.0
    IL_0044:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.UberToilet::mMusicSoundHandle
    IL_0049:  callvirt   instance bool [SimIFace]Sims3.SimIFace.Sound::StartLoop()
    IL_004e:  pop
    IL_004f:  ret
  } // end of method UberToilet::StartMusic

  .method public hidebysig instance void 
          StopMusic() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.UberToilet::mMusicSoundHandle
    IL_0006:  brfalse.s  IL_0025

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.UberToilet::mMusicSoundHandle
    IL_000e:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Stop()
    IL_0013:  ldarg.0
    IL_0014:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.UberToilet::mMusicSoundHandle
    IL_0019:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Dispose()
    IL_001e:  ldarg.0
    IL_001f:  ldnull
    IL_0020:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.UberToilet::mMusicSoundHandle
    IL_0025:  ret
  } // end of method UberToilet::StopMusic

  .method public hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       139 (0x8b)
    .maxstack  3
    .locals init (string[] V_0)
    IL_0000:  ldstr      "Store/Objects/UberToilet:"
    IL_0005:  stsfld     string Sims3.Store.Objects.UberToilet::sLocalizationKey
    IL_000a:  ldc.i4.3
    IL_000b:  newarr     [mscorlib]System.String
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  ldstr      "uber_song_one"
    IL_0018:  stelem.ref
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.1
    IL_001b:  ldstr      "uber_song_two"
    IL_0020:  stelem.ref
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.2
    IL_0023:  ldstr      "uber_song_three"
    IL_0028:  stelem.ref
    IL_0029:  ldloc.0
    IL_002a:  stsfld     string[] Sims3.Store.Objects.UberToilet::mMusicOptions
    IL_002f:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive::.ctor()
    IL_0034:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive Sims3.Store.Objects.UberToilet::kEnvironmentTuning
    IL_0039:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent/Tuning::.ctor()
    IL_003e:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent/Tuning Sims3.Store.Objects.UberToilet::kCleanableTuning
    IL_0043:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning::.ctor()
    IL_0048:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning Sims3.Store.Objects.UberToilet::kRepairableTuning
    IL_004d:  newobj     instance void [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning::.ctor()
    IL_0052:  stsfld     class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning Sims3.Store.Objects.UberToilet::kToiletTuning
    IL_0057:  ldc.r4     30.
    IL_005c:  stsfld     float32 Sims3.Store.Objects.UberToilet::kMaxLengthUseToilet
    IL_0061:  ldc.i4.s   30
    IL_0063:  stsfld     int32 Sims3.Store.Objects.UberToilet::kTimeoutRemainingForBladderEmergency
    IL_0068:  ldc.r4     0.0
    IL_006d:  stsfld     float32 Sims3.Store.Objects.UberToilet::kTimeToWaitInLine
    IL_0072:  ldc.i4.s   90
    IL_0074:  stsfld     int32 Sims3.Store.Objects.UberToilet::kChanceOfToiletAutoFlushWhileInUse
    IL_0079:  ldc.i4.s   30
    IL_007b:  stsfld     int32 Sims3.Store.Objects.UberToilet::kTimeoutRemainingToSit
    IL_0080:  ldc.r4     30.
    IL_0085:  stsfld     float32 Sims3.Store.Objects.UberToilet::kChanceOfRelaxed
    IL_008a:  ret
  } // end of method UberToilet::.cctor

  .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive
          EnvironmentTuning()
  {
    .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/EnvironmentMotive Sims3.Store.Objects.UberToilet::get_EnvironmentTuning()
  } // end of property UberToilet::EnvironmentTuning
  .property instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent/Tuning
          CleanableTuning()
  {
    .get instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.CleanableComponent/Tuning Sims3.Store.Objects.UberToilet::get_CleanableTuning()
  } // end of property UberToilet::CleanableTuning
  .property instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning
          RepairableTuning()
  {
    .get instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning Sims3.Store.Objects.UberToilet::get_RepairableTuning()
  } // end of property UberToilet::RepairableTuning
  .property instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning
          ToiletTuning()
  {
    .get instance class [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Toilet/Tuning Sims3.Store.Objects.UberToilet::get_ToiletTuning()
  } // end of property UberToilet::ToiletTuning
  .property instance string OverflowFxName()
  {
    .get instance string Sims3.Store.Objects.UberToilet::get_OverflowFxName()
  } // end of property UberToilet::OverflowFxName
} // end of class Sims3.Store.Objects.UberToilet


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file D:\Downloads\For Games\Sims3 Downloads\Hacks\Coding\Sims3\Store\StoreObjectsUberToilet.res
