
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
.assembly extern UI
{
  .ver 1:0:0:0
}
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly StoreObjectsRoutineMachine
{
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 18 53 69 6D 73 33 53 74 6F 72 65 52 6F 75   // ...Sims3StoreRou
                                                                                              74 69 6E 65 4D 61 63 68 69 6E 65 00 00 )          // tineMachine..
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 45 41 2E 63 6F 6D 00 00 )                // ...EA.com..
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 37 30 35 35 42 35 32 32 2D 33 46 35 46   // ..$7055B522-3F5F
                                                                                                  2D 41 38 37 36 2D 42 39 30 34 2D 45 36 35 44 34   // -A876-B904-E65D4
                                                                                                  34 36 31 32 46 33 38 00 00 )                      // 4612F38..
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 18 53 69 6D 73 33 53 74 6F 72 65 52 6F 75   // ...Sims3StoreRou
                                                                                                74 69 6E 65 4D 61 63 68 69 6E 65 00 00 )          // tineMachine..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1D 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  45 41 2E 63 6F 6D 20 32 30 30 37 2D 32 30 31 30   // EA.com 2007-2010
                                                                                                  00 00 ) 
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module StoreObjectsRoutineMachine.dll
// MVID: {58ABC899-0CEF-4388-830A-6159A6BA8B79}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00230000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Sims3.Store.Objects.RoutineMachine
       extends [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject
       implements [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ITeleporter,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject,
                  [SimIFace]Sims3.SimIFace.IScriptLogic,
                  [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IObjectUI,
                  [SimIFace]Sims3.SimIFace.CustomContent.IExportableContent
{
  .custom instance void [SimIFace]Sims3.SimIFace.RuntimeExportAttribute::.ctor() = ( 01 00 00 00 ) 
  .class auto ansi nested public beforefieldinit RoutineMachineRepairableComponent
         extends [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::.ctor()
      IL_0006:  ret
    } // end of method RoutineMachineRepairableComponent::.ctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class Sims3.Store.Objects.RoutineMachine owner,
                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/RepairableType 'type',
                                 class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning tuning) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  ldarg.3
      IL_0004:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                                          valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/RepairableType,
                                                                                                                          class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning)
      IL_0009:  ret
    } // end of method RoutineMachineRepairableComponent::.ctor

    .method public hidebysig virtual instance void 
            OnBroken(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                     bool startFx) cil managed
    {
      // Code size       41 (0x29)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ObjectComponent::get_Owner()
      IL_0006:  castclass  Sims3.Store.Objects.RoutineMachine
      IL_000b:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StartBrokenVisualEffects()
      IL_0010:  ldarg.0
      IL_0011:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ObjectComponent::get_Owner()
      IL_0016:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Controllers.PuddleManager/FlowingPuddleTuning Sims3.Store.Objects.RoutineMachine::kPuddlesTuning
      IL_001b:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Controllers.PuddleManager::AddFlowingPuddle(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Controllers.PuddleManager/FlowingPuddleTuning)
      IL_0020:  ldarg.0
      IL_0021:  ldarg.1
      IL_0022:  ldarg.2
      IL_0023:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::OnBroken(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                             bool)
      IL_0028:  ret
    } // end of method RoutineMachineRepairableComponent::OnBroken

    .method public hidebysig virtual instance void 
            OnRepaired(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor actor) cil managed
    {
      // Code size       35 (0x23)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ObjectComponent::get_Owner()
      IL_0006:  castclass  Sims3.Store.Objects.RoutineMachine
      IL_000b:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StopBrokenVisualEffects()
      IL_0010:  ldarg.0
      IL_0011:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ObjectComponent::get_Owner()
      IL_0016:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Controllers.PuddleManager::RemoveFlowingPuddle(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject)
      IL_001b:  ldarg.0
      IL_001c:  ldarg.1
      IL_001d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::OnRepaired(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor)
      IL_0022:  ret
    } // end of method RoutineMachineRepairableComponent::OnRepaired

  } // end of class RoutineMachineRepairableComponent

  .class auto ansi nested public beforefieldinit ShowerPrivacySituation
         extends [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation
  {
    .field public class [SimIFace]Sims3.SimIFace.StateMachineClient StateMachine
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation::.ctor()
      IL_0006:  ret
    } // end of method ShowerPrivacySituation::.ctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance takeShowerInteraction) cil managed
    {
      // Code size       35 (0x23)
      .maxstack  5
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim[] V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InstanceActor()
      IL_0007:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_LotCurrent()
      IL_000c:  ldc.i4.1
      IL_000d:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim
      IL_0012:  stloc.0
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.0
      IL_0015:  ldarg.1
      IL_0016:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InstanceActor()
      IL_001b:  stelem.ref
      IL_001c:  ldloc.0
      IL_001d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot,
                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim[])
      IL_0022:  ret
    } // end of method ShowerPrivacySituation::.ctor

    .method public hidebysig virtual instance bool 
            OtherSimCanIgnorePrivacy(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim sim) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation::OtherSimCanIgnorePrivacy(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0007:  brfalse.s  IL_000b

      IL_0009:  ldc.i4.1
      IL_000a:  ret

      IL_000b:  ldarg.0
      IL_000c:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Situations.PrivacySituation::SyncTarget
      IL_0011:  ldarg.1
      IL_0012:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::CanIgnoreNormalPrivacySituationsWith(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0017:  ret
    } // end of method ShowerPrivacySituation::OtherSimCanIgnorePrivacy

    .method public hidebysig virtual instance void 
            ReactToIntruder(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim participant,
                            class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim intruder) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation::StateMachine
      IL_0006:  brfalse.s  IL_0013

      IL_0008:  ldarg.1
      IL_0009:  ldc.i4     0x200
      IL_000e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0013:  ldarg.0
      IL_0014:  ldarg.1
      IL_0015:  ldarg.2
      IL_0016:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation::ReactToIntruder(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                  class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_001b:  ret
    } // end of method ShowerPrivacySituation::ReactToIntruder

  } // end of class ShowerPrivacySituation

  .class abstract auto ansi nested public beforefieldinit TeleportBase
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>
         implements [Sims3GameplaySystems]Sims3.Gameplay.Actors.IOutfitChangeInteraction,
                    [UI]Sims3.UI.Hud.IInteractionInstance
  {
    .class auto ansi nested public beforefieldinit TeleportDefinitionBase`1<.ctor (Sims3.Store.Objects.RoutineMachine/TeleportBase) T>
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,!T>
           implements [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Children.IInteractionProhibitsChildCare,
                      [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       35 (0x23)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
        IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
        IL_000b:  brtrue.s   IL_0015

        IL_000d:  ldarg.2
        IL_000e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Charred()
        IL_0013:  brfalse.s  IL_0017

        IL_0015:  ldc.i4.0
        IL_0016:  ret

        IL_0017:  ldarg.1
        IL_0018:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsPerformingAService()
        IL_001d:  brfalse.s  IL_0021

        IL_001f:  ldc.i4.0
        IL_0020:  ret

        IL_0021:  ldc.i4.1
        IL_0022:  ret
      } // end of method TeleportDefinitionBase`1::Test

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,!T>::.ctor()
        IL_0006:  ret
      } // end of method TeleportDefinitionBase`1::.ctor

    } // end of class TeleportDefinitionBase`1

    .class auto ansi nested public DreamscapeNode
           extends [mscorlib]System.Enum
    {
      .field public specialname rtspecialname int32 value__
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode Logic = int32(0x00000000)
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode Athletic = int32(0x00000001)
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode Charisma = int32(0x00000002)
    } // end of class DreamscapeNode

    .class auto ansi nested public RewardItemTypes
           extends [mscorlib]System.Enum
    {
      .field public specialname rtspecialname int32 value__
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/RewardItemTypes Metal = int32(0x00000000)
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/RewardItemTypes Gem = int32(0x00000001)
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/RewardItemTypes Seed = int32(0x00000002)
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/RewardItemTypes Ingredient = int32(0x00000003)
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/RewardItemTypes HighValueGem = int32(0x00000004)
      .field public static literal valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/RewardItemTypes Gnome = int32(0x00000005)
    } // end of class RewardItemTypes

    .field public static literal string sLocalizationExploreKey = "Store/Objects/RoutineMachine/Explore:"
    .field public static float32 kChangeHungerTime
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 2F 4D 61 78 20 74 69 6D 65 20 74 6F 20 72   // ../Max time to r
                                                                                              65 66 69 6C 6C 20 74 68 69 73 20 6D 6F 74 69 76   // efill this motiv
                                                                                              65 2E 20 20 47 50 45 20 44 65 66 61 75 6C 74 3A   // e.  GPE Default:
                                                                                              20 35 00 00 )                                     //  5..
    .field public static float32 kChangeBladderTime
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 2F 4D 61 78 20 74 69 6D 65 20 74 6F 20 72   // ../Max time to r
                                                                                              65 66 69 6C 6C 20 74 68 69 73 20 6D 6F 74 69 76   // efill this motiv
                                                                                              65 2E 20 20 47 50 45 20 44 65 66 61 75 6C 74 3A   // e.  GPE Default:
                                                                                              20 35 00 00 )                                     //  5..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kChangeHygieneTime
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 30 4D 61 78 20 74 69 6D 65 20 74 6F 20 72   // ..0Max time to r
                                                                                              65 66 69 6C 6C 20 74 68 69 73 20 6D 6F 74 69 76   // efill this motiv
                                                                                              65 2E 20 20 47 50 45 20 44 65 66 61 75 6C 74 3A   // e.  GPE Default:
                                                                                              20 31 30 00 00 )                                  //  10..
    .field public static float32 kThresholdHunger
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 34 42 65 6C 6F 77 20 74 68 69 73 20 76 61   // ..4Below this va
                                                                                              6C 75 65 2C 20 66 69 78 20 74 68 65 69 72 20 68   // lue, fix their h
                                                                                              75 6E 67 65 72 2E 20 20 47 50 45 20 44 65 66 61   // unger.  GPE Defa
                                                                                              75 6C 74 3A 20 35 30 00 00 )                      // ult: 50..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kThresholdBladder
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 34 42 65 6C 6F 77 20 74 68 69 73 20 76 61   // ..4Below this va
                                                                                              6C 75 65 2C 20 66 69 78 20 74 68 65 69 72 20 62   // lue, fix their b
                                                                                              6C 61 64 64 65 72 2E 20 47 50 45 20 44 65 66 61   // ladder. GPE Defa
                                                                                              75 6C 74 3A 20 35 30 00 00 )                      // ult: 50..
    .field public static float32 kThresholdHygiene
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 35 42 65 6C 6F 77 20 74 68 69 73 20 76 61   // ..5Below this va
                                                                                              6C 75 65 2C 20 66 69 78 20 74 68 65 69 72 20 68   // lue, fix their h
                                                                                              79 67 69 65 6E 65 2E 20 20 47 50 45 20 64 65 66   // ygiene.  GPE def
                                                                                              61 75 6C 74 3A 20 35 30 00 00 )                   // ault: 50..
    .field public static float32 kSleepyTimeStart
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 45 53 74 61 72 74 69 6E 67 20 74 69 6D 65   // ..EStarting time
                                                                                              20 77 68 65 72 65 20 73 69 6D 20 70 6C 61 79 73   //  where sim plays
                                                                                              20 79 61 77 6E 20 61 6E 69 6D 61 74 69 6F 6E 2E   //  yawn animation.
                                                                                              20 47 50 45 20 44 65 66 61 75 6C 74 20 33 20 28   //  GPE Default 3 (
                                                                                              33 3A 30 30 20 61 6D 29 00 00 )                   // 3:00 am)..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kSleepyTimeEnd
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 44 45 6E 64 69 6E 67 20 74 69 6D 65 20 77   // ..DEnding time w
                                                                                              68 65 72 65 20 73 69 6D 20 70 6C 61 79 73 20 79   // here sim plays y
                                                                                              61 77 6E 20 61 6E 69 6D 61 74 69 6F 6E 2E 20 20   // awn animation.  
                                                                                              47 50 45 20 44 65 66 61 75 6C 74 20 38 20 28 38   // GPE Default 8 (8
                                                                                              3A 30 30 20 61 6D 29 00 00 )                      // :00 am)..
    .field public static float32 kMinEnergyToTeleport
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 61 4D 69 6E 69 6D 75 6D 20 65 6E 65 72 67   // ..aMinimum energ
                                                                                              79 20 6D 6F 74 69 76 65 20 74 6F 20 74 65 6C 65   // y motive to tele
                                                                                              70 6F 72 74 20 73 75 63 63 65 73 73 66 75 6C 6C   // port successfull
                                                                                              79 2E 20 20 42 65 6C 6F 77 20 74 68 69 73 20 67   // y.  Below this g
                                                                                              6F 65 73 20 74 6F 20 44 72 65 61 6D 73 63 61 70   // oes to Dreamscap
                                                                                              65 2E 20 20 47 50 45 20 64 65 66 61 75 6C 74 3A   // e.  GPE default:
                                                                                              20 2D 32 30 00 00 )                               //  -20..
    .field public static float32 kSpeakerFun
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 16 53 70 65 61 6B 65 72 20 46 75 6E 20 70   // ...Speaker Fun p
                                                                                              65 72 20 6D 69 6E 75 74 65 00 00 )                // er minute..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper mSwitchOutfitHelper
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper OutfitHelper
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot mTargetLot
    .field public class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation mSituation
    .field public static float32 kExploreMinBetweenEvents
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 2F 4D 69 6E 69 6D 75 6D 20 74 69 6D 65 20   // ../Minimum time 
                                                                                              62 65 74 77 65 65 6E 20 65 78 70 6C 6F 72 65 20   // between explore 
                                                                                              65 76 65 6E 74 73 2C 20 69 6E 20 6D 69 6E 75 74   // events, in minut
                                                                                              65 73 00 00 )                                     // es..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kExploreMaxBetweenEvents
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 23 4D 61 78 69 6D 75 6D 20 74 69 6D 65 20   // ..#Maximum time 
                                                                                              62 65 74 77 65 65 6E 20 65 78 70 6C 6F 72 65 20   // between explore 
                                                                                              65 76 65 6E 74 73 00 00 )                         // events..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static int32 kNumExploreEvents
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 66 4E 75 6D 62 65 72 20 6F 66 20 65 78 70   // ..fNumber of exp
                                                                                              6C 6F 72 65 20 65 76 65 6E 74 20 73 65 74 73 2E   // lore event sets.
                                                                                              20 20 49 4D 50 4F 52 54 41 4E 54 3A 20 76 65 72   //   IMPORTANT: ver
                                                                                              69 66 79 20 74 68 61 74 20 61 6C 6C 20 48 41 4C   // ify that all HAL
                                                                                              20 73 74 72 69 6E 67 20 73 65 74 73 20 61 72 65   //  string sets are
                                                                                              20 69 6E 63 6C 75 64 65 64 20 69 6E 20 74 68 65   //  included in the
                                                                                              20 70 61 63 6B 61 67 65 2E 00 00 )                //  package...
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static int32 kSkillCheck1Required
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 3D 4D 69 6E 69 6D 75 6D 20 73 6B 69 6C 6C   // ..=Minimum skill
                                                                                              20 6C 65 76 65 6C 20 74 6F 20 70 61 73 73 20 74   //  level to pass t
                                                                                              68 69 73 20 73 6B 69 6C 6C 20 63 68 65 63 6B 2E   // his skill check.
                                                                                              20 20 47 50 45 20 44 65 66 61 75 6C 74 3A 20 33   //   GPE Default: 3
                                                                                              00 00 ) 
    .field public static int32 kSkillCheck2Required
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 58 4D 69 6E 69 6D 75 6D 20 73 6B 69 6C 6C   // ..XMinimum skill
                                                                                              20 6C 65 76 65 6C 20 6F 66 20 61 74 68 6C 65 74   //  level of athlet
                                                                                              69 63 20 6F 72 20 6C 6F 67 69 63 20 73 6B 69 6C   // ic or logic skil
                                                                                              6C 20 74 6F 20 70 61 73 73 20 74 68 69 73 20 73   // l to pass this s
                                                                                              6B 69 6C 6C 20 63 68 65 63 6B 2E 20 20 47 50 45   // kill check.  GPE
                                                                                              20 44 65 66 61 75 6C 74 3A 20 35 00 00 )          //  Default: 5..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kChanceOfExploreRandomFailure
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 50 50 65 72 63 65 6E 74 20 63 68 61 6E 63   // ..PPercent chanc
                                                                                              65 20 74 68 61 74 20 61 20 73 6B 69 6C 6C 20 63   // e that a skill c
                                                                                              68 65 63 6B 20 77 69 6C 6C 20 66 61 69 6C 20 66   // heck will fail f
                                                                                              72 6F 6D 20 72 61 6E 64 6F 6D 20 63 68 61 6E 63   // rom random chanc
                                                                                              65 2E 20 20 47 50 45 20 64 65 66 61 75 6C 74 3A   // e.  GPE default:
                                                                                              20 31 30 00 00 )                                  //  10..
    .field public static float32 kChanceOfExploreAwesomeGnome
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 3F 43 68 61 6E 63 65 20 6F 66 20 67 65 74   // ..?Chance of get
                                                                                              74 69 6E 67 20 61 20 67 6E 6F 6D 65 20 77 68 65   // ting a gnome whe
                                                                                              6E 20 67 65 74 74 69 6E 67 20 61 6C 6C 20 73 6B   // n getting all sk
                                                                                              69 6C 6C 20 63 68 65 63 6B 73 20 63 6F 72 72 65   // ill checks corre
                                                                                              63 74 00 00 )                                     // ct..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kDreamscapeEnergyBoost
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 2D 45 4E 65 72 67 79 20 67 72 61 6E 74 65   // ..-ENergy grante
                                                                                              64 20 66 72 6F 6D 20 70 61 73 73 69 6E 67 20 61   // d from passing a
                                                                                              20 44 72 65 61 6D 73 63 61 70 65 20 74 65 73 74   //  Dreamscape test
                                                                                              00 00 ) 
    .field public float32 mTimeUntilEvent
    .field public static float32 kRewardRareGemMinCost
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 52 56 61 6C 75 65 20 28 6D 69 6E 69 6D 75   // ..RValue (minimu
                                                                                              6D 29 20 6F 66 20 72 61 72 65 20 67 65 6D 20 66   // m) of rare gem f
                                                                                              6F 75 6E 64 20 69 6E 20 72 6F 75 74 69 6E 65 20   // ound in routine 
                                                                                              6D 61 63 68 69 6E 65 20 64 72 65 61 6D 73 63 61   // machine dreamsca
                                                                                              70 65 2E 20 20 47 50 45 20 44 65 66 61 75 6C 74   // pe.  GPE Default
                                                                                              3A 20 32 30 30 00 00 )                            // : 200..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static string[] kGnomeList
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 27 4C 69 73 74 20 6F 66 20 67 6E 6F 6D 65   // ..'List of gnome
                                                                                              73 20 74 6F 20 72 61 6E 64 6F 6D 6C 79 20 73 65   // s to randomly se
                                                                                              6C 65 63 74 20 66 72 6F 6D 2E 00 00 )             // lect from...
    .field public static valuetype [SimIFace]Sims3.SimIFace.ProductVersion[] kGnomeProductVersions
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 29 4C 69 73 74 20 6F 66 20 70 72 6F 64 75   // ..)List of produ
                                                                                              63 74 20 76 65 72 73 69 6F 6E 73 20 6F 66 20 74   // ct versions of t
                                                                                              68 6F 73 65 20 67 6E 6F 6D 65 73 2E 00 00 )       // hose gnomes...
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig newslot specialname virtual final 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper 
            get_SwitchOutfitHelper() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::mSwitchOutfitHelper
      IL_0006:  ret
    } // end of method TeleportBase::get_SwitchOutfitHelper

    .method public hidebysig newslot specialname virtual final 
            instance void  set_SwitchOutfitHelper(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper 'value') cil managed
    {
      // Code size       37 (0x25)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::mSwitchOutfitHelper
      IL_0006:  ldarg.1
      IL_0007:  bne.un.s   IL_000a

      IL_0009:  ret

      IL_000a:  ldarg.0
      IL_000b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::mSwitchOutfitHelper
      IL_0010:  brfalse.s  IL_001d

      IL_0012:  ldarg.0
      IL_0013:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::mSwitchOutfitHelper
      IL_0018:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Dispose()
      IL_001d:  ldarg.0
      IL_001e:  ldarg.1
      IL_001f:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::mSwitchOutfitHelper
      IL_0024:  ret
    } // end of method TeleportBase::set_SwitchOutfitHelper

    .method public hidebysig newslot specialname virtual 
            instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason 
            get_NewClothesStyle() cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      IL_0000:  ldc.i4.s   10
      IL_0002:  ret
    } // end of method TeleportBase::get_NewClothesStyle

    .method public hidebysig virtual instance void 
            ConfigureInteraction() cil managed
    {
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0007:  ldc.i4.0
      IL_0008:  ldc.i4.8
      IL_0009:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.CAS.OutfitCategories)
      IL_000e:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::OutfitHelper
      IL_0013:  ldarg.0
      IL_0014:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::ConfigureInteraction()
      IL_0019:  ret
    } // end of method TeleportBase::ConfigureInteraction

    .method public hidebysig virtual instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot 
            GetTargetLot() cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot Sims3.Store.Objects.RoutineMachine/TeleportBase::mTargetLot
      IL_0006:  dup
      IL_0007:  brtrue.s   IL_000f

      IL_0009:  pop
      IL_000a:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Core.LotManager::GetWorldLot()
      IL_000f:  ret
    } // end of method TeleportBase::GetTargetLot

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       2159 (0x86f)
      .maxstack  5
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_0,
               bool V_1,
               bool V_2,
               bool V_3,
               valuetype [SimIFace]Sims3.SimIFace.CAS.OutfitCategories V_4,
               class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper V_5,
               class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager V_6,
               valuetype [SimIFace]Sims3.SimIFace.CAS.OutfitCategories V_7,
               class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper V_8)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::CaregiverCheck()
      IL_000b:  brtrue.s   IL_000f

      IL_000d:  ldc.i4.0
      IL_000e:  ret

      IL_000f:  ldarg.0
      IL_0010:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0015:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Children.CarryingChildPosture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CarryingChildPosture()
      IL_001a:  brtrue.s   IL_0029

      IL_001c:  ldarg.0
      IL_001d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0022:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarryingPetPosture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CarryingPetPosture()
      IL_0027:  brfalse.s  IL_002b

      IL_0029:  ldc.i4.0
      IL_002a:  ret

      IL_002b:  ldarg.0
      IL_002c:  ldloca.s   V_0
      IL_002e:  ldarg.0
      IL_002f:  ldflda     class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot Sims3.Store.Objects.RoutineMachine/TeleportBase::mTargetLot
      IL_0034:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine/TeleportBase::PreTeleport(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject&,
                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot&)
      IL_0039:  stloc.1
      IL_003a:  ldloc.1
      IL_003b:  brtrue.s   IL_003f

      IL_003d:  ldc.i4.0
      IL_003e:  ret

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::OutfitHelper
      IL_0045:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Start()
      IL_004a:  ldarg.0
      IL_004b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0050:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0055:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_005a:  brtrue.s   IL_006c

      IL_005c:  ldarg.0
      IL_005d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0062:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Charred()
      IL_0067:  brfalse    IL_01f3

      IL_006c:  ldarg.0
      IL_006d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0072:  ldarg.0
      IL_0073:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0078:  ldc.i4     0x31229a4c
      IL_007d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlotAndCheckInUse(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_0082:  brfalse    IL_01f1

      IL_0087:  ldarg.0
      IL_0088:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry()
      IL_008d:  ldarg.0
      IL_008e:  ldstr      "routinemachine_store"
      IL_0093:  ldstr      "EnterGetReady"
      IL_0098:  ldstr      "x"
      IL_009d:  ldstr      "routinemachine"
      IL_00a2:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_00a7:  ldarg.0
      IL_00a8:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_00ad:  ldstr      "tired"
      IL_00b2:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSleepyTimeStart
      IL_00b7:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSleepyTimeEnd
      IL_00bc:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.SimClock::IsTimeBetweenTimes(float32,
                                                                                                            float32)
      IL_00c1:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_00c6:  ldarg.0
      IL_00c7:  ldstr      "GetInMachine"
      IL_00cc:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_00d1:  ldarg.0
      IL_00d2:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_00d7:  ldarg.0
      IL_00d8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00dd:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StartPortalTeleportVisualEffects(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_00e2:  ldarg.0
      IL_00e3:  ldstr      "DropThroughPortal"
      IL_00e8:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_00ed:  ldarg.0
      IL_00ee:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_00f3:  ldarg.0
      IL_00f4:  ldstr      "InsideDreamscape"
      IL_00f9:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_00fe:  ldarg.0
      IL_00ff:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0104:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0109:  ldc.i4.s   100
      IL_010b:  ldarg.0
      IL_010c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0111:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::UpdateBreakage(int32,
                                                                                                                                   class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0116:  pop
      IL_0117:  ldarg.0
      IL_0118:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_011d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_0122:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsFrankenstein()
      IL_0127:  brfalse.s  IL_014e

      IL_0129:  ldarg.0
      IL_012a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_012f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0134:  ldc.i8     0x6c2a012d6179df90
      IL_013d:  ldc.i8     0x6c2a002d6179dfa0
      IL_0146:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_014b:  pop
      IL_014c:  br.s       IL_01bd

      IL_014e:  ldarg.0
      IL_014f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0154:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0159:  ldc.i8     0x4c800171bd46b80
      IL_0162:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0167:  brtrue.s   IL_0184

      IL_0169:  ldarg.0
      IL_016a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_016f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_0174:  ldc.i8     0x4c800171bd46b00
      IL_017d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_0182:  brfalse.s  IL_0199

      IL_0184:  ldarg.0
      IL_0185:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_018a:  ldc.i4.4
      IL_018b:  ldarg.0
      IL_018c:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0191:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::Kill(valuetype [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription/DeathType,
                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject)
      IL_0196:  pop
      IL_0197:  br.s       IL_01bd

      IL_0199:  ldarg.0
      IL_019a:  ldc.i4.0
      IL_019b:  conv.i8
      IL_019c:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffSinged::SingeViaInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.IOutfitChangeInteraction,
                                                                                                                  valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_01a1:  ldarg.0
      IL_01a2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01a7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_01ac:  ldc.i8     0x4c800171bd46b80
      IL_01b5:  ldc.i4.0
      IL_01b6:  conv.i8
      IL_01b7:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_01bc:  pop
      IL_01bd:  ldarg.0
      IL_01be:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01c3:  ldarg.0
      IL_01c4:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_01c9:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Controllers.FireManager::SimShockedBy(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject)
      IL_01ce:  ldarg.0
      IL_01cf:  ldstr      "ReturnFromDreamscape"
      IL_01d4:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_01d9:  ldarg.0
      IL_01da:  ldstr      "ExitDreamscape"
      IL_01df:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_01e4:  ldarg.0
      IL_01e5:  ldc.i4.0
      IL_01e6:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_01eb:  ldarg.0
      IL_01ec:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit()
      IL_01f1:  ldc.i4.0
      IL_01f2:  stloc.1
      IL_01f3:  ldloc.1
      IL_01f4:  brfalse    IL_084f

      IL_01f9:  ldarg.0
      IL_01fa:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01ff:  ldarg.0
      IL_0200:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0205:  ldc.i4     0x31229a4c
      IL_020a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlotAndCheckInUse(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_020f:  brfalse    IL_084f

      IL_0214:  ldc.i4.0
      IL_0215:  stloc.2
      IL_0216:  ldarg.0
      IL_0217:  ldarg.0
      IL_0218:  newobj     instance void Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_021d:  stfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_0222:  ldarg.0
      IL_0223:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_0228:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Situations.GenericPrivacySituation::Start()
      IL_022d:  brtrue.s   IL_0231

      IL_022f:  ldc.i4.0
      IL_0230:  ret

      IL_0231:  ldarg.0
      IL_0232:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0237:  ldarg.0
      IL_0238:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_023d:  ldc.i4     0x31229a4c
      IL_0242:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlotAndCheckInUse(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_0247:  brtrue.s   IL_025d

      IL_0249:  ldarg.0
      IL_024a:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_024f:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Situation::Exit()
      IL_0254:  ldarg.0
      IL_0255:  ldnull
      IL_0256:  stfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_025b:  ldc.i4.0
      IL_025c:  ret

      IL_025d:  ldc.i4.0
      IL_025e:  stloc.3
      IL_025f:  ldarg.0
      IL_0260:  ldc.i4.0
      IL_0261:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::set_CancellableByPlayer(bool)
      IL_0266:  ldarg.0
      IL_0267:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry()
      IL_026c:  ldarg.0
      IL_026d:  ldstr      "routinemachine_store"
      IL_0272:  ldstr      "EnterGetReady"
      IL_0277:  ldstr      "x"
      IL_027c:  ldstr      "routinemachine"
      IL_0281:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_0286:  ldarg.0
      IL_0287:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_028c:  ldstr      "tired"
      IL_0291:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSleepyTimeStart
      IL_0296:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSleepyTimeEnd
      IL_029b:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.SimClock::IsTimeBetweenTimes(float32,
                                                                                                            float32)
      IL_02a0:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_02a5:  ldarg.0
      IL_02a6:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_02ab:  ldarg.0
      IL_02ac:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_02b1:  stfld      class [SimIFace]Sims3.SimIFace.StateMachineClient Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation::StateMachine
      IL_02b6:  ldarg.0
      IL_02b7:  ldstr      "GetInMachine"
      IL_02bc:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_02c1:  ldarg.0
      IL_02c2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_02c7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_02cc:  ldc.i4     0x10f38d7
      IL_02d1:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_02d6:  brfalse    IL_03bd

      IL_02db:  ldarg.0
      IL_02dc:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_02e1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_02e6:  ldc.i4     0x10f38d7
      IL_02eb:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_02f0:  ldarg.0
      IL_02f1:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_02f6:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_02fb:  ldc.i4     0x10f38d7
      IL_0300:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetMax(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_0305:  bge.un     IL_03bd

      IL_030a:  ldarg.0
      IL_030b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0310:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0315:  ldc.i4     0x10f38d7
      IL_031a:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_031f:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kThresholdHunger
      IL_0324:  bge.un     IL_03bd

      IL_0329:  ldarg.0
      IL_032a:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_032f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_0334:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0339:  ldc.i4.s   13
      IL_033b:  bne.un.s   IL_0348

      IL_033d:  ldarg.0
      IL_033e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0343:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StartSpeakerEffects()
      IL_0348:  ldloc.3
      IL_0349:  brtrue.s   IL_0358

      IL_034b:  ldc.i4.1
      IL_034c:  stloc.3
      IL_034d:  ldarg.0
      IL_034e:  ldstr      "InAirIdle"
      IL_0353:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0358:  ldarg.0
      IL_0359:  ldstr      "SatisfyHunger"
      IL_035e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0363:  ldarg.0
      IL_0364:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0369:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_036e:  ldarg.0
      IL_036f:  ldc.i4     0x10f38d7
      IL_0374:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChangeHungerTime
      IL_0379:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::LerpToFill(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                           valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                           float32)
      IL_037e:  ldarg.0
      IL_037f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0384:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_0389:  ldarg.0
      IL_038a:  ldc.i4     0xffa9bfff
      IL_038f:  ldarg.0
      IL_0390:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::SatisfyHungerDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_0396:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_039b:  ldarg.0
      IL_039c:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_03a1:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                       class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_03a6:  pop
      IL_03a7:  ldarg.0
      IL_03a8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_03ad:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_03b2:  ldarg.0
      IL_03b3:  ldstr      "InAirIdle"
      IL_03b8:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_03bd:  ldarg.0
      IL_03be:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_03c3:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_03c8:  ldc.i4     0x10f38d6
      IL_03cd:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_03d2:  brfalse    IL_04c5

      IL_03d7:  ldarg.0
      IL_03d8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_03dd:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_03e2:  ldc.i4     0x10f38d6
      IL_03e7:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_03ec:  ldarg.0
      IL_03ed:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_03f2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_03f7:  ldc.i4     0x10f38d6
      IL_03fc:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetMax(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_0401:  bge.un     IL_04c5

      IL_0406:  ldarg.0
      IL_0407:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_040c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0411:  ldc.i4     0x10f38d6
      IL_0416:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_041b:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kThresholdBladder
      IL_0420:  bge.un     IL_04c5

      IL_0425:  ldarg.0
      IL_0426:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_042b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_0430:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0435:  ldc.i4.s   13
      IL_0437:  bne.un.s   IL_0444

      IL_0439:  ldarg.0
      IL_043a:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_043f:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StartSpeakerEffects()
      IL_0444:  ldloc.3
      IL_0445:  brtrue.s   IL_0454

      IL_0447:  ldc.i4.1
      IL_0448:  stloc.3
      IL_0449:  ldarg.0
      IL_044a:  ldstr      "InAirIdle"
      IL_044f:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0454:  ldarg.0
      IL_0455:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_045a:  ldc.i4.1
      IL_045b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::EnableCensor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/CensorType)
      IL_0460:  ldarg.0
      IL_0461:  ldstr      "SatisfyBladder"
      IL_0466:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_046b:  ldarg.0
      IL_046c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0471:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0476:  ldarg.0
      IL_0477:  ldc.i4     0x10f38d6
      IL_047c:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChangeBladderTime
      IL_0481:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::LerpToFill(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                           valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                           float32)
      IL_0486:  ldarg.0
      IL_0487:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_048c:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_0491:  ldarg.0
      IL_0492:  ldc.i4     0xffa9bfff
      IL_0497:  ldarg.0
      IL_0498:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::SatisfyBladderDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_049e:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_04a3:  ldarg.0
      IL_04a4:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_04a9:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                       class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_04ae:  pop
      IL_04af:  ldarg.0
      IL_04b0:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_04b5:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_04ba:  ldarg.0
      IL_04bb:  ldstr      "InAirIdle"
      IL_04c0:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_04c5:  ldarg.0
      IL_04c6:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_04cb:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_04d0:  ldc.i4     0x10f38d9
      IL_04d5:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_04da:  brfalse    IL_06ea

      IL_04df:  ldarg.0
      IL_04e0:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_04e5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_04ea:  ldc.i4     0x10f38d9
      IL_04ef:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_04f4:  ldarg.0
      IL_04f5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_04fa:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_04ff:  ldc.i4     0x10f38d9
      IL_0504:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetMax(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_0509:  bge.un     IL_06ea

      IL_050e:  ldarg.0
      IL_050f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0514:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0519:  ldc.i4     0x10f38d9
      IL_051e:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_0523:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kThresholdHygiene
      IL_0528:  bge.un     IL_06ea

      IL_052d:  ldarg.0
      IL_052e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0533:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_0538:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_053d:  ldc.i4.s   13
      IL_053f:  bne.un.s   IL_054c

      IL_0541:  ldarg.0
      IL_0542:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0547:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StartSpeakerEffects()
      IL_054c:  ldloc.3
      IL_054d:  brtrue.s   IL_055c

      IL_054f:  ldc.i4.1
      IL_0550:  stloc.3
      IL_0551:  ldarg.0
      IL_0552:  ldstr      "InAirIdle"
      IL_0557:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_055c:  ldarg.0
      IL_055d:  ldc.i4     0x3e9
      IL_0562:  ldarg.0
      IL_0563:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::ShowerStartEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                           class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0569:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_056e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddSynchronousOneShotScriptEventHandler(uint32,
                                                                                                                                                        class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0573:  ldarg.0
      IL_0574:  ldc.i4     0x3ea
      IL_0579:  ldarg.0
      IL_057a:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::ShowerEndEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                         class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0580:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0585:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddSynchronousOneShotScriptEventHandler(uint32,
                                                                                                                                                        class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_058a:  ldc.i4.1
      IL_058b:  stloc.s    V_4
      IL_058d:  ldarg.0
      IL_058e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0593:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_0598:  ldc.i8     0x4c800171bd46ad0
      IL_05a1:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_05a6:  brfalse.s  IL_05ae

      IL_05a8:  ldc.i4.s   16
      IL_05aa:  stloc.s    V_4
      IL_05ac:  br.s       IL_05ba

      IL_05ae:  ldarg.0
      IL_05af:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_05b4:  ldc.i4.2
      IL_05b5:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::EnableCensor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/CensorType)
      IL_05ba:  ldarg.0
      IL_05bb:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_05c0:  ldc.i4.2
      IL_05c1:  ldloc.s    V_4
      IL_05c3:  ldc.i4.0
      IL_05c4:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.CAS.OutfitCategories,
                                                                                                                   bool)
      IL_05c9:  stloc.s    V_5
      .try
      {
        IL_05cb:  ldloc.s    V_5
        IL_05cd:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Start()
        IL_05d2:  ldloc.s    V_5
        IL_05d4:  ldc.i4.0
        IL_05d5:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Wait(bool)
        IL_05da:  ldloc.s    V_5
        IL_05dc:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::ChangeOutfit()
        IL_05e1:  leave.s    IL_05ef

      }  // end .try
      finally
      {
        IL_05e3:  ldloc.s    V_5
        IL_05e5:  brfalse.s  IL_05ee

        IL_05e7:  ldloc.s    V_5
        IL_05e9:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_05ee:  endfinally
      }  // end handler
      IL_05ef:  ldc.i4.1
      IL_05f0:  stloc.2
      IL_05f1:  ldarg.0
      IL_05f2:  ldstr      "SatisfyHygenie"
      IL_05f7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_05fc:  ldarg.0
      IL_05fd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0602:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0607:  ldarg.0
      IL_0608:  ldc.i4     0x10f38d9
      IL_060d:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChangeHygieneTime
      IL_0612:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::LerpToFill(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                           valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                           float32)
      IL_0617:  ldarg.0
      IL_0618:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_061d:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_0622:  ldarg.0
      IL_0623:  ldc.i4     0xffa9bfff
      IL_0628:  ldarg.0
      IL_0629:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::SatisfyHygieneDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_062f:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_0634:  ldarg.0
      IL_0635:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_063a:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                       class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_063f:  pop
      IL_0640:  ldarg.0
      IL_0641:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0646:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_064b:  ldarg.0
      IL_064c:  ldstr      "InAirIdle"
      IL_0651:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0656:  ldarg.0
      IL_0657:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_065c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0661:  stloc.s    V_6
      IL_0663:  ldloc.s    V_6
      IL_0665:  ldc.i8     0xb82d0015b9294950
      IL_066e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0673:  ldloc.s    V_6
      IL_0675:  ldc.i8     0x4c800171bd46b80
      IL_067e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0683:  ldloc.s    V_6
      IL_0685:  ldc.i8     0xb8490018e14ec1f0
      IL_068e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0693:  ldarg.0
      IL_0694:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0699:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_069e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsMummy()
      IL_06a3:  brfalse.s  IL_06bf

      IL_06a5:  ldloc.s    V_6
      IL_06a7:  ldc.i8     0xb5dd0028adc88270
      IL_06b0:  ldc.i8     0x8bfc002118898737
      IL_06b9:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_06be:  pop
      IL_06bf:  ldarg.0
      IL_06c0:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_06c5:  ldc.i8     0x4c800171bd46a80
      IL_06ce:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_06d3:  brfalse.s  IL_06ea

      IL_06d5:  ldarg.0
      IL_06d6:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_06db:  ldc.i4.s   9
      IL_06dd:  ldarg.0
      IL_06de:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_06e3:  ldc.i4.4
      IL_06e4:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_06e9:  pop
      IL_06ea:  ldarg.0
      IL_06eb:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_06f0:  ldarg.0
      IL_06f1:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason Sims3.Store.Objects.RoutineMachine/TeleportBase::get_NewClothesStyle()
      IL_06f6:  ldloca.s   V_7
      IL_06f8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetOutfitForClothingChange(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason,
                                                                                                                     valuetype [SimIFace]Sims3.SimIFace.CAS.OutfitCategories&)
      IL_06fd:  brtrue.s   IL_0702

      IL_06ff:  ldloc.2
      IL_0700:  brfalse.s  IL_0752

      IL_0702:  ldarg.0
      IL_0703:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0708:  ldstr      "inAirIdle"
      IL_070d:  ldloc.3
      IL_070e:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0713:  ldarg.0
      IL_0714:  ldstr      "PrepareToChangeClothes"
      IL_0719:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_071e:  ldarg.0
      IL_071f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0724:  ldc.i4.0
      IL_0725:  ldloc.s    V_7
      IL_0727:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.CAS.OutfitCategories)
      IL_072c:  stloc.s    V_8
      .try
      {
        IL_072e:  ldloc.s    V_8
        IL_0730:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Start()
        IL_0735:  ldloc.s    V_8
        IL_0737:  ldc.i4.0
        IL_0738:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Wait(bool)
        IL_073d:  ldloc.s    V_8
        IL_073f:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::ChangeOutfit()
        IL_0744:  leave.s    IL_0752

      }  // end .try
      finally
      {
        IL_0746:  ldloc.s    V_8
        IL_0748:  brfalse.s  IL_0751

        IL_074a:  ldloc.s    V_8
        IL_074c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0751:  endfinally
      }  // end handler
      IL_0752:  ldarg.0
      IL_0753:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0758:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AutoEnableCensor()
      IL_075d:  ldarg.0
      IL_075e:  ldc.i4.s   100
      IL_0760:  ldarg.0
      IL_0761:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::TeleportOutEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                           class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0767:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_076c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddSynchronousOneShotScriptEventHandler(uint32,
                                                                                                                                                        class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0771:  ldarg.0
      IL_0772:  ldstr      "DropThroughPortal"
      IL_0777:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_077c:  ldarg.0
      IL_077d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0782:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StopSpeakerEffects()
      IL_0787:  ldarg.0
      IL_0788:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_078d:  brfalse.s  IL_07a1

      IL_078f:  ldarg.0
      IL_0790:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_0795:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Situation::Exit()
      IL_079a:  ldarg.0
      IL_079b:  ldnull
      IL_079c:  stfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_07a1:  ldarg.0
      IL_07a2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_07a7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_07ac:  ldc.i4     0x10f38d8
      IL_07b1:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_07b6:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kMinEnergyToTeleport
      IL_07bb:  bge.un.s   IL_07f2

      IL_07bd:  ldarg.0
      IL_07be:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_07c3:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StartDreamscapeEffects()
      IL_07c8:  ldarg.0
      IL_07c9:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_07ce:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_07d3:  ldc.i4.s   100
      IL_07d5:  ldarg.0
      IL_07d6:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_07db:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::UpdateBreakage(int32,
                                                                                                                                   class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_07e0:  pop
      IL_07e1:  ldarg.0
      IL_07e2:  call       instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::RunDreamscape()
      IL_07e7:  ldarg.0
      IL_07e8:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_07ed:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StopDreamscapeEffects()
      IL_07f2:  ldarg.0
      IL_07f3:  ldloc.0
      IL_07f4:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine/TeleportBase::DoTeleport(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_07f9:  stloc.1
      IL_07fa:  ldarg.0
      IL_07fb:  ldstr      "ExitGetReady"
      IL_0800:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0805:  ldarg.0
      IL_0806:  ldstr      "routinemachine_store"
      IL_080b:  ldstr      "EnterBeTeleported"
      IL_0810:  ldstr      "x"
      IL_0815:  ldstr      "routinemachine"
      IL_081a:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_081f:  ldarg.0
      IL_0820:  ldc.i4.s   100
      IL_0822:  ldarg.0
      IL_0823:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::TeleportBackEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                            class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0829:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_082e:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddSynchronousOneShotScriptEventHandler(uint32,
                                                                                                                                                        class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0833:  ldarg.0
      IL_0834:  ldstr      "BeTeleported"
      IL_0839:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_083e:  ldarg.0
      IL_083f:  ldstr      "ExitBeTeleported"
      IL_0844:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0849:  ldarg.0
      IL_084a:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit()
      IL_084f:  ldarg.0
      IL_0850:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0855:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StopPortalReturnVisualEffects()
      IL_085a:  ldarg.0
      IL_085b:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_0860:  brfalse.s  IL_086d

      IL_0862:  ldarg.0
      IL_0863:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_0868:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Situation::Exit()
      IL_086d:  ldloc.1
      IL_086e:  ret
    } // end of method TeleportBase::Run

    .method public hidebysig instance void 
            TeleportOutEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                             class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       20 (0x14)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0006:  brfalse.s  IL_0013

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_000e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::FadeOut()
      IL_0013:  ret
    } // end of method TeleportBase::TeleportOutEvent

    .method public hidebysig instance void 
            TeleportBackEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                              class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0006:  brfalse.s  IL_0014

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_000e:  ldc.i4.0
      IL_000f:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::FadeIn(bool)
      IL_0014:  ret
    } // end of method TeleportBase::TeleportBackEvent

    .method public hidebysig instance void 
            ShowerStartEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                             class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       20 (0x14)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  brfalse.s  IL_0013

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_000e:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StartShowerAudio()
      IL_0013:  ret
    } // end of method TeleportBase::ShowerStartEvent

    .method public hidebysig instance void 
            ShowerEndEvent(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                           class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       20 (0x14)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  brfalse.s  IL_0013

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_000e:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StopShowerAudio()
      IL_0013:  ret
    } // end of method TeleportBase::ShowerEndEvent

    .method public hidebysig instance void 
            SatisfyHungerDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                                  valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData loopData) cil managed
    {
      // Code size       109 (0x6d)
      .maxstack  3
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_000b:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0010:  ldc.i4.s   13
      IL_0012:  bne.un.s   IL_0045

      IL_0014:  ldarg.0
      IL_0015:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_001a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_001f:  ldc.i4     0x10f38da
      IL_0024:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_0029:  brfalse.s  IL_0045

      IL_002b:  ldarg.0
      IL_002c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0031:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0036:  ldc.i4     0x10f38da
      IL_003b:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSpeakerFun
      IL_0040:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_0045:  ldarg.0
      IL_0046:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_004b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0050:  ldc.i4     0x10f38d7
      IL_0055:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::IsMax(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_005a:  brfalse.s  IL_006c

      IL_005c:  ldarg.0
      IL_005d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0062:  ldc.i4     0x2000
      IL_0067:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_006c:  ret
    } // end of method TeleportBase::SatisfyHungerDelegate

    .method public hidebysig instance void 
            SatisfyBladderDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData loopData) cil managed
    {
      // Code size       109 (0x6d)
      .maxstack  3
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_000b:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0010:  ldc.i4.s   13
      IL_0012:  bne.un.s   IL_0045

      IL_0014:  ldarg.0
      IL_0015:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_001a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_001f:  ldc.i4     0x10f38da
      IL_0024:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_0029:  brfalse.s  IL_0045

      IL_002b:  ldarg.0
      IL_002c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0031:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0036:  ldc.i4     0x10f38da
      IL_003b:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSpeakerFun
      IL_0040:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_0045:  ldarg.0
      IL_0046:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_004b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0050:  ldc.i4     0x10f38d6
      IL_0055:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::IsMax(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_005a:  brfalse.s  IL_006c

      IL_005c:  ldarg.0
      IL_005d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0062:  ldc.i4     0x2000
      IL_0067:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_006c:  ret
    } // end of method TeleportBase::SatisfyBladderDelegate

    .method public hidebysig instance void 
            SatisfyHygieneDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData loopData) cil managed
    {
      // Code size       109 (0x6d)
      .maxstack  3
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_000b:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0010:  ldc.i4.s   13
      IL_0012:  bne.un.s   IL_0045

      IL_0014:  ldarg.0
      IL_0015:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_001a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_001f:  ldc.i4     0x10f38da
      IL_0024:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_0029:  brfalse.s  IL_0045

      IL_002b:  ldarg.0
      IL_002c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0031:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0036:  ldc.i4     0x10f38da
      IL_003b:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSpeakerFun
      IL_0040:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_0045:  ldarg.0
      IL_0046:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_004b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0050:  ldc.i4     0x10f38d9
      IL_0055:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::IsMax(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_005a:  brfalse.s  IL_006c

      IL_005c:  ldarg.0
      IL_005d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0062:  ldc.i4     0x2000
      IL_0067:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_006c:  ret
    } // end of method TeleportBase::SatisfyHygieneDelegate

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       59 (0x3b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::OutfitHelper
      IL_0006:  brfalse.s  IL_001a

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::OutfitHelper
      IL_000e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Dispose()
      IL_0013:  ldarg.0
      IL_0014:  ldnull
      IL_0015:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::OutfitHelper
      IL_001a:  ldarg.0
      IL_001b:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_0020:  brfalse.s  IL_0034

      IL_0022:  ldarg.0
      IL_0023:  ldfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_0028:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Situation::Exit()
      IL_002d:  ldarg.0
      IL_002e:  ldnull
      IL_002f:  stfld      class Sims3.Store.Objects.RoutineMachine/ShowerPrivacySituation Sims3.Store.Objects.RoutineMachine/TeleportBase::mSituation
      IL_0034:  ldarg.0
      IL_0035:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_003a:  ret
    } // end of method TeleportBase::Cleanup

    .method public hidebysig newslot virtual 
            instance bool  PreTeleport([out] class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject& destinationObject,
                                       [out] class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot& targetLot) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldnull
      IL_0002:  stind.ref
      IL_0003:  ldarg.2
      IL_0004:  ldnull
      IL_0005:  stind.ref
      IL_0006:  ldc.i4.1
      IL_0007:  ret
    } // end of method TeleportBase::PreTeleport

    .method public hidebysig newslot abstract virtual 
            instance bool  DoTeleport(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject gameObject) cil managed
    {
    } // end of method TeleportBase::DoTeleport

    .method public hidebysig instance valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode 
            PickDreamscapeButtons(int32 stringNum,
                                  class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Actor) cil managed
    {
      // Code size       240 (0xf0)
      .maxstack  5
      .locals init (valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode[] V_0,
               class [mscorlib]System.Collections.Generic.List`1<valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode> V_1,
               string V_2,
               string V_3,
               string V_4,
               bool V_5,
               valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode[] V_6,
               object[] V_7)
      IL_0000:  ldc.i4.3
      IL_0001:  newarr     Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_0006:  stloc.s    V_6
      IL_0008:  ldloc.s    V_6
      IL_000a:  ldc.i4.1
      IL_000b:  ldc.i4.1
      IL_000c:  stelem.i4
      IL_000d:  ldloc.s    V_6
      IL_000f:  ldc.i4.2
      IL_0010:  ldc.i4.2
      IL_0011:  stelem.i4
      IL_0012:  ldloc.s    V_6
      IL_0014:  stloc.0
      IL_0015:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode>::.ctor()
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  ldloc.0
      IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode>::AddRange(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0022:  ldloc.1
      IL_0023:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomizeListOfObjects<valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode>(class [mscorlib]System.Collections.Generic.List`1<!!0>)
      IL_0028:  ldstr      "ChoiceDialog"
      IL_002d:  ldarg.1
      IL_002e:  box        [mscorlib]System.Int32
      IL_0033:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_0038:  stloc.2
      IL_0039:  ldstr      "ChoiceDialog"
      IL_003e:  ldtoken    Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_0048:  ldloc.1
      IL_0049:  ldc.i4.0
      IL_004a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode>::get_Item(int32)
      IL_004f:  box        Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_0054:  call       string [mscorlib]System.Enum::GetName(class [mscorlib]System.Type,
                                                                 object)
      IL_0059:  ldarg.1
      IL_005a:  box        [mscorlib]System.Int32
      IL_005f:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_0064:  stloc.3
      IL_0065:  ldstr      "ChoiceDialog"
      IL_006a:  ldtoken    Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_006f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_0074:  ldloc.1
      IL_0075:  ldc.i4.1
      IL_0076:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode>::get_Item(int32)
      IL_007b:  box        Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_0080:  call       string [mscorlib]System.Enum::GetName(class [mscorlib]System.Type,
                                                                 object)
      IL_0085:  ldarg.1
      IL_0086:  box        [mscorlib]System.Int32
      IL_008b:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_0090:  stloc.s    V_4
      IL_0092:  ldarg.2
      IL_0093:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0098:  ldloc.2
      IL_0099:  ldc.i4.1
      IL_009a:  newarr     [mscorlib]System.Object
      IL_009f:  stloc.s    V_7
      IL_00a1:  ldloc.s    V_7
      IL_00a3:  ldc.i4.0
      IL_00a4:  ldarg.2
      IL_00a5:  stelem.ref
      IL_00a6:  ldloc.s    V_7
      IL_00a8:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_00ad:  ldarg.2
      IL_00ae:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_00b3:  ldloc.3
      IL_00b4:  ldc.i4.0
      IL_00b5:  newarr     [mscorlib]System.Object
      IL_00ba:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_00bf:  ldarg.2
      IL_00c0:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_00c5:  ldloc.s    V_4
      IL_00c7:  ldc.i4.0
      IL_00c8:  newarr     [mscorlib]System.Object
      IL_00cd:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_00d2:  call       bool [UI]Sims3.UI.TwoButtonDialog::Show(string,
                                                                   string,
                                                                   string)
      IL_00d7:  ldc.i4.0
      IL_00d8:  ceq
      IL_00da:  stloc.s    V_5
      IL_00dc:  ldloc.s    V_5
      IL_00de:  brfalse.s  IL_00e8

      IL_00e0:  ldloc.1
      IL_00e1:  ldc.i4.1
      IL_00e2:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode>::get_Item(int32)
      IL_00e7:  ret

      IL_00e8:  ldloc.1
      IL_00e9:  ldc.i4.0
      IL_00ea:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode>::get_Item(int32)
      IL_00ef:  ret
    } // end of method TeleportBase::PickDreamscapeButtons

    .method public hidebysig instance void 
            RunDreamscape() cil managed
    {
      // Code size       1258 (0x4ea)
      .maxstack  6
      .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3,
               valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode V_4,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_5,
               string V_6,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_7,
               string V_8,
               string V_9,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_10,
               object[] V_11,
               object[] V_12,
               int32 V_13,
               object[] V_14,
               object[] V_15,
               object[] V_16,
               object[] V_17)
      IL_0000:  ldsfld     int32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kNumExploreEvents
      IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor(int32)
      IL_000a:  stloc.0
      IL_000b:  ldc.i4.0
      IL_000c:  stloc.1
      IL_000d:  br.s       IL_001c

      IL_000f:  ldloc.0
      IL_0010:  ldloc.1
      IL_0011:  ldc.i4.1
      IL_0012:  add
      IL_0013:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
      IL_0018:  ldloc.1
      IL_0019:  ldc.i4.1
      IL_001a:  add
      IL_001b:  stloc.1
      IL_001c:  ldloc.1
      IL_001d:  ldsfld     int32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kNumExploreEvents
      IL_0022:  blt.s      IL_000f

      IL_0024:  ldloc.0
      IL_0025:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomizeListOfObjects<int32>(class [mscorlib]System.Collections.Generic.List`1<!!0>)
      IL_002a:  ldc.i4.0
      IL_002b:  stloc.2
      IL_002c:  ldc.i4.0
      IL_002d:  stloc.3
      IL_002e:  ldarg.0
      IL_002f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0034:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsSelectable()
      IL_0039:  brfalse    IL_04b9

      IL_003e:  ldarg.0
      IL_003f:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMinBetweenEvents
      IL_0044:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMaxBetweenEvents
      IL_0049:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetFloat(float32,
                                                                                                  float32)
      IL_004e:  stfld      float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::mTimeUntilEvent
      IL_0053:  ldarg.0
      IL_0054:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0059:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_005e:  ldarg.0
      IL_005f:  ldc.i4     0xffa9bfff
      IL_0064:  ldarg.0
      IL_0065:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::DreamscapeLoopDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_006b:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_0070:  ldarg.0
      IL_0071:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0076:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                       class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_007b:  brfalse.s  IL_00e9

      IL_007d:  ldarg.0
      IL_007e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0083:  ldarg.0
      IL_0084:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0089:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_008e:  ldstr      "InitialProgressTNS"
      IL_0093:  ldc.i4.1
      IL_0094:  newarr     [mscorlib]System.Object
      IL_0099:  stloc.s    V_11
      IL_009b:  ldloc.s    V_11
      IL_009d:  ldc.i4.0
      IL_009e:  ldarg.0
      IL_009f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00a4:  stelem.ref
      IL_00a5:  ldloc.s    V_11
      IL_00a7:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_00ac:  ldc.i4.3
      IL_00ad:  ldarg.0
      IL_00ae:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_00b3:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
      IL_00b8:  ldarg.0
      IL_00b9:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00be:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
      IL_00c3:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                               valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle,
                                                                                                                                               valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                               valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_00c8:  pop
      IL_00c9:  ldarg.0
      IL_00ca:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMinBetweenEvents
      IL_00cf:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMaxBetweenEvents
      IL_00d4:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetFloat(float32,
                                                                                                  float32)
      IL_00d9:  stfld      float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::mTimeUntilEvent
      IL_00de:  ldarg.0
      IL_00df:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00e4:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_00e9:  ldarg.0
      IL_00ea:  ldc.i4     0xffa9bfff
      IL_00ef:  ldarg.0
      IL_00f0:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::DreamscapeLoopDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_00f6:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_00fb:  ldarg.0
      IL_00fc:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0101:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                       class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_0106:  brfalse.s  IL_0157

      IL_0108:  ldarg.0
      IL_0109:  ldloc.0
      IL_010a:  ldloc.2
      IL_010b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
      IL_0110:  ldarg.0
      IL_0111:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0116:  call       instance valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode Sims3.Store.Objects.RoutineMachine/TeleportBase::PickDreamscapeButtons(int32,
                                                                                                                                                                                    class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_011b:  stloc.s    V_4
      IL_011d:  ldarg.0
      IL_011e:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMinBetweenEvents
      IL_0123:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMaxBetweenEvents
      IL_0128:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetFloat(float32,
                                                                                                  float32)
      IL_012d:  stfld      float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::mTimeUntilEvent
      IL_0132:  ldarg.0
      IL_0133:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0138:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_013d:  ldarg.0
      IL_013e:  ldloc.s    V_4
      IL_0140:  ldsfld     int32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSkillCheck1Required
      IL_0145:  ldloc.0
      IL_0146:  ldloc.2
      IL_0147:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
      IL_014c:  call       instance bool Sims3.Store.Objects.RoutineMachine/TeleportBase::PassedSkillCheck(valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode,
                                                                                                           int32,
                                                                                                           int32)
      IL_0151:  brfalse.s  IL_0157

      IL_0153:  ldloc.3
      IL_0154:  ldc.i4.1
      IL_0155:  add
      IL_0156:  stloc.3
      IL_0157:  ldloc.3
      IL_0158:  ldc.i4.1
      IL_0159:  bne.un     IL_01e9

      IL_015e:  ldarg.0
      IL_015f:  ldc.i4     0xffa9bfff
      IL_0164:  ldarg.0
      IL_0165:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::DreamscapeLoopDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_016b:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_0170:  ldarg.0
      IL_0171:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0176:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                       class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_017b:  brfalse.s  IL_01e9

      IL_017d:  ldarg.0
      IL_017e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0183:  ldarg.0
      IL_0184:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0189:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_018e:  ldstr      "LateProgressTNS"
      IL_0193:  ldc.i4.1
      IL_0194:  newarr     [mscorlib]System.Object
      IL_0199:  stloc.s    V_12
      IL_019b:  ldloc.s    V_12
      IL_019d:  ldc.i4.0
      IL_019e:  ldarg.0
      IL_019f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01a4:  stelem.ref
      IL_01a5:  ldloc.s    V_12
      IL_01a7:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_01ac:  ldc.i4.3
      IL_01ad:  ldarg.0
      IL_01ae:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_01b3:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
      IL_01b8:  ldarg.0
      IL_01b9:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01be:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
      IL_01c3:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                               valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle,
                                                                                                                                               valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                               valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_01c8:  pop
      IL_01c9:  ldarg.0
      IL_01ca:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMinBetweenEvents
      IL_01cf:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMaxBetweenEvents
      IL_01d4:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetFloat(float32,
                                                                                                  float32)
      IL_01d9:  stfld      float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::mTimeUntilEvent
      IL_01de:  ldarg.0
      IL_01df:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01e4:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_01e9:  ldloc.3
      IL_01ea:  ldc.i4.1
      IL_01eb:  bne.un.s   IL_025f

      IL_01ed:  ldarg.0
      IL_01ee:  ldc.i4     0xffa9bfff
      IL_01f3:  ldarg.0
      IL_01f4:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::DreamscapeLoopDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_01fa:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_01ff:  ldarg.0
      IL_0200:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0205:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                       class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_020a:  brfalse.s  IL_025f

      IL_020c:  ldloc.2
      IL_020d:  ldc.i4.1
      IL_020e:  add
      IL_020f:  stloc.2
      IL_0210:  ldarg.0
      IL_0211:  ldloc.0
      IL_0212:  ldloc.2
      IL_0213:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
      IL_0218:  ldarg.0
      IL_0219:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_021e:  call       instance valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode Sims3.Store.Objects.RoutineMachine/TeleportBase::PickDreamscapeButtons(int32,
                                                                                                                                                                                    class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0223:  stloc.s    V_4
      IL_0225:  ldarg.0
      IL_0226:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMinBetweenEvents
      IL_022b:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMaxBetweenEvents
      IL_0230:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetFloat(float32,
                                                                                                  float32)
      IL_0235:  stfld      float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::mTimeUntilEvent
      IL_023a:  ldarg.0
      IL_023b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0240:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ClearExitReasons()
      IL_0245:  ldarg.0
      IL_0246:  ldloc.s    V_4
      IL_0248:  ldsfld     int32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSkillCheck2Required
      IL_024d:  ldloc.0
      IL_024e:  ldloc.2
      IL_024f:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
      IL_0254:  call       instance bool Sims3.Store.Objects.RoutineMachine/TeleportBase::PassedSkillCheck(valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode,
                                                                                                           int32,
                                                                                                           int32)
      IL_0259:  brfalse.s  IL_025f

      IL_025b:  ldloc.3
      IL_025c:  ldc.i4.1
      IL_025d:  add
      IL_025e:  stloc.3
      IL_025f:  ldarg.0
      IL_0260:  ldc.i4     0xffa9bfff
      IL_0265:  ldarg.0
      IL_0266:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::DreamscapeLoopDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_026c:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_0271:  ldarg.0
      IL_0272:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0277:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                       class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_027c:  brfalse    IL_04e9

      IL_0281:  ldarg.0
      IL_0282:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0287:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_028c:  ldc.i8     0x70a900201f452380
      IL_0295:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_029a:  ldarg.0
      IL_029b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_02a0:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_02a5:  ldc.i8     0x70a900201f452340
      IL_02ae:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_02b3:  ldarg.0
      IL_02b4:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_02b9:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_02be:  ldc.i8     0x70a900201f452390
      IL_02c7:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::RemoveElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_02cc:  ldloc.3
      IL_02cd:  stloc.s    V_13
      IL_02cf:  ldloc.s    V_13
      IL_02d1:  switch     ( 
                            IL_045b,
                            IL_0385,
                            IL_02e7)
      IL_02e2:  br         IL_0385

      IL_02e7:  ldarg.0
      IL_02e8:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject Sims3.Store.Objects.RoutineMachine/TeleportBase::GetRareItem()
      IL_02ed:  stloc.s    V_5
      IL_02ef:  ldloc.s    V_5
      IL_02f1:  brtrue.s   IL_02fb

      IL_02f3:  ldarg.0
      IL_02f4:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject Sims3.Store.Objects.RoutineMachine/TeleportBase::GetRandomItem()
      IL_02f9:  stloc.s    V_5
      IL_02fb:  ldloc.s    V_5
      IL_02fd:  brfalse.s  IL_0361

      IL_02ff:  ldarg.0
      IL_0300:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0305:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_030a:  ldloc.s    V_5
      IL_030c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToAdd(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_0311:  brtrue.s   IL_031c

      IL_0313:  ldloc.s    V_5
      IL_0315:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject::Destroy()
      IL_031a:  br.s       IL_0361

      IL_031c:  ldarg.0
      IL_031d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0322:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0327:  ldstr      "FoundItem"
      IL_032c:  ldc.i4.2
      IL_032d:  newarr     [mscorlib]System.Object
      IL_0332:  stloc.s    V_14
      IL_0334:  ldloc.s    V_14
      IL_0336:  ldc.i4.0
      IL_0337:  ldarg.0
      IL_0338:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_033d:  stelem.ref
      IL_033e:  ldloc.s    V_14
      IL_0340:  ldc.i4.1
      IL_0341:  ldloc.s    V_5
      IL_0343:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::GetLocalizedName()
      IL_0348:  stelem.ref
      IL_0349:  ldloc.s    V_14
      IL_034b:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_0350:  stloc.s    V_6
      IL_0352:  ldarg.0
      IL_0353:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0358:  ldloc.s    V_6
      IL_035a:  ldc.i4.2
      IL_035b:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                               valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle)
      IL_0360:  pop
      IL_0361:  ldarg.0
      IL_0362:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0367:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_036c:  ldc.i8     0x70a900201f452380
      IL_0375:  ldc.i8     0x8bfc002118898760
      IL_037e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_0383:  pop
      IL_0384:  ret

      IL_0385:  ldarg.0
      IL_0386:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_038b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0390:  ldc.i8     0x70a900201f452340
      IL_0399:  ldc.i8     0x8bfc002118898760
      IL_03a2:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_03a7:  pop
      IL_03a8:  ldarg.0
      IL_03a9:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_03ae:  ldarg.0
      IL_03af:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_03b4:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_03b9:  ldstr      "GoodMoodlet"
      IL_03be:  ldc.i4.1
      IL_03bf:  newarr     [mscorlib]System.Object
      IL_03c4:  stloc.s    V_15
      IL_03c6:  ldloc.s    V_15
      IL_03c8:  ldc.i4.0
      IL_03c9:  ldarg.0
      IL_03ca:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_03cf:  stelem.ref
      IL_03d0:  ldloc.s    V_15
      IL_03d2:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_03d7:  ldc.i4.2
      IL_03d8:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                               valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle)
      IL_03dd:  pop
      IL_03de:  ldarg.0
      IL_03df:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject Sims3.Store.Objects.RoutineMachine/TeleportBase::GetRandomItem()
      IL_03e4:  stloc.s    V_7
      IL_03e6:  ldloc.s    V_7
      IL_03e8:  brtrue.s   IL_03f2

      IL_03ea:  ldarg.0
      IL_03eb:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject Sims3.Store.Objects.RoutineMachine/TeleportBase::GetRandomItem()
      IL_03f0:  stloc.s    V_7
      IL_03f2:  ldloc.s    V_7
      IL_03f4:  brfalse    IL_04e9

      IL_03f9:  ldarg.0
      IL_03fa:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_03ff:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0404:  ldloc.s    V_7
      IL_0406:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToAdd(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_040b:  brtrue.s   IL_0415

      IL_040d:  ldloc.s    V_7
      IL_040f:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject::Destroy()
      IL_0414:  ret

      IL_0415:  ldarg.0
      IL_0416:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_041b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0420:  ldstr      "FoundItem"
      IL_0425:  ldc.i4.2
      IL_0426:  newarr     [mscorlib]System.Object
      IL_042b:  stloc.s    V_16
      IL_042d:  ldloc.s    V_16
      IL_042f:  ldc.i4.0
      IL_0430:  ldarg.0
      IL_0431:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0436:  stelem.ref
      IL_0437:  ldloc.s    V_16
      IL_0439:  ldc.i4.1
      IL_043a:  ldloc.s    V_7
      IL_043c:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::GetLocalizedName()
      IL_0441:  stelem.ref
      IL_0442:  ldloc.s    V_16
      IL_0444:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_0449:  stloc.s    V_8
      IL_044b:  ldarg.0
      IL_044c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0451:  ldloc.s    V_8
      IL_0453:  ldc.i4.2
      IL_0454:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                               valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle)
      IL_0459:  pop
      IL_045a:  ret

      IL_045b:  ldarg.0
      IL_045c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0461:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0466:  ldstr      "BadMoodlet"
      IL_046b:  ldc.i4.1
      IL_046c:  newarr     [mscorlib]System.Object
      IL_0471:  stloc.s    V_17
      IL_0473:  ldloc.s    V_17
      IL_0475:  ldc.i4.0
      IL_0476:  ldarg.0
      IL_0477:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_047c:  stelem.ref
      IL_047d:  ldloc.s    V_17
      IL_047f:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_0484:  stloc.s    V_9
      IL_0486:  ldarg.0
      IL_0487:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_048c:  ldloc.s    V_9
      IL_048e:  ldc.i4.2
      IL_048f:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                               valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle)
      IL_0494:  pop
      IL_0495:  ldarg.0
      IL_0496:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_049b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_04a0:  ldc.i8     0x70a900201f452390
      IL_04a9:  ldc.i8     0x8bfc002118898760
      IL_04b2:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_04b7:  pop
      IL_04b8:  ret

      IL_04b9:  ldarg.0
      IL_04ba:  ldc.i4     0xffa9bfff
      IL_04bf:  call       instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_04c4:  brfalse.s  IL_04e9

      IL_04c6:  ldarg.0
      IL_04c7:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject Sims3.Store.Objects.RoutineMachine/TeleportBase::GetRandomItem()
      IL_04cc:  stloc.s    V_10
      IL_04ce:  ldarg.0
      IL_04cf:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_04d4:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_04d9:  ldloc.s    V_10
      IL_04db:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToAdd(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_04e0:  brtrue.s   IL_04e9

      IL_04e2:  ldloc.s    V_10
      IL_04e4:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject::Destroy()
      IL_04e9:  ret
    } // end of method TeleportBase::RunDreamscape

    .method public hidebysig instance void 
            DreamscapeLoopDelegate(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData loopData) cil managed
    {
      // Code size       32 (0x20)
      .maxstack  8
      IL_0000:  ldarga.s   loopData
      IL_0002:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData::mLifeTime
      IL_0007:  ldarg.0
      IL_0008:  ldfld      float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::mTimeUntilEvent
      IL_000d:  ble.un.s   IL_001f

      IL_000f:  ldarg.0
      IL_0010:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0015:  ldc.i4     0x2000
      IL_001a:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_001f:  ret
    } // end of method TeleportBase::DreamscapeLoopDelegate

    .method public hidebysig static string 
            LocalizeExploreString(bool isFemale,
                                  string name,
                                  object[] parameters) cil managed
    {
      .param [3]
      .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldstr      "Store/Objects/RoutineMachine/Explore:"
      IL_0006:  ldarg.1
      IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_000c:  ldarg.2
      IL_000d:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(bool,
                                                                                                              string,
                                                                                                              object[])
      IL_0012:  ret
    } // end of method TeleportBase::LocalizeExploreString

    .method public hidebysig instance bool 
            PassedSkillCheck(valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode skill,
                             int32 skillLevel,
                             int32 eventNumber) cil managed
    {
      // Code size       472 (0x1d8)
      .maxstack  6
      .locals init (valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames V_0,
               int32 V_1,
               object[] V_2,
               object[] V_3,
               object[] V_4,
               object[] V_5,
               object[] V_6,
               object[] V_7)
      IL_0000:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChanceOfExploreRandomFailure
      IL_0005:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomChance(float32)
      IL_000a:  brtrue.s   IL_001e

      IL_000c:  ldarg.0
      IL_000d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0012:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.MoodManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_MoodManager()
      IL_0017:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.MoodManager::get_IsInStrongNegativeMood()
      IL_001c:  brfalse.s  IL_007c

      IL_001e:  ldarg.0
      IL_001f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0024:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0029:  ldstr      "ChoiceResultHeader"
      IL_002e:  ldc.i4.1
      IL_002f:  newarr     [mscorlib]System.Object
      IL_0034:  stloc.2
      IL_0035:  ldloc.2
      IL_0036:  ldc.i4.0
      IL_0037:  ldarg.0
      IL_0038:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_003d:  stelem.ref
      IL_003e:  ldloc.2
      IL_003f:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_0044:  ldarg.0
      IL_0045:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_004a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_004f:  ldstr      "GenericFail"
      IL_0054:  ldarg.3
      IL_0055:  box        [mscorlib]System.Int32
      IL_005a:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_005f:  ldc.i4.1
      IL_0060:  newarr     [mscorlib]System.Object
      IL_0065:  stloc.3
      IL_0066:  ldloc.3
      IL_0067:  ldc.i4.0
      IL_0068:  ldarg.0
      IL_0069:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_006e:  stelem.ref
      IL_006f:  ldloc.3
      IL_0070:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_0075:  call       void [UI]Sims3.UI.SimpleMessageDialog::Show(string,
                                                                       string)
      IL_007a:  ldc.i4.0
      IL_007b:  ret

      IL_007c:  ldc.i4.2
      IL_007d:  conv.i8
      IL_007e:  stloc.0
      IL_007f:  ldarg.1
      IL_0080:  ldc.i4.2
      IL_0081:  bne.un.s   IL_0088

      IL_0083:  ldc.i4.3
      IL_0084:  conv.i8
      IL_0085:  stloc.0
      IL_0086:  br.s       IL_008f

      IL_0088:  ldarg.1
      IL_0089:  brtrue.s   IL_008f

      IL_008b:  ldc.i4.s   12
      IL_008d:  conv.i8
      IL_008e:  stloc.0
      IL_008f:  ldarg.2
      IL_0090:  ldarg.0
      IL_0091:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0096:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
      IL_009b:  ldloc.0
      IL_009c:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetSkillLevel(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
      IL_00a1:  ble.s      IL_011c

      IL_00a3:  ldarg.0
      IL_00a4:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00a9:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_00ae:  ldstr      "ChoiceResultHeader"
      IL_00b3:  ldc.i4.1
      IL_00b4:  newarr     [mscorlib]System.Object
      IL_00b9:  stloc.s    V_4
      IL_00bb:  ldloc.s    V_4
      IL_00bd:  ldc.i4.0
      IL_00be:  ldarg.0
      IL_00bf:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00c4:  stelem.ref
      IL_00c5:  ldloc.s    V_4
      IL_00c7:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_00cc:  ldarg.0
      IL_00cd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00d2:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_00d7:  ldtoken    Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_00dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_00e1:  ldarg.1
      IL_00e2:  box        Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_00e7:  call       string [mscorlib]System.Enum::GetName(class [mscorlib]System.Type,
                                                                 object)
      IL_00ec:  ldstr      "Fail"
      IL_00f1:  ldarg.3
      IL_00f2:  box        [mscorlib]System.Int32
      IL_00f7:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_00fc:  ldc.i4.1
      IL_00fd:  newarr     [mscorlib]System.Object
      IL_0102:  stloc.s    V_5
      IL_0104:  ldloc.s    V_5
      IL_0106:  ldc.i4.0
      IL_0107:  ldarg.0
      IL_0108:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_010d:  stelem.ref
      IL_010e:  ldloc.s    V_5
      IL_0110:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_0115:  call       void [UI]Sims3.UI.SimpleMessageDialog::Show(string,
                                                                       string)
      IL_011a:  ldc.i4.0
      IL_011b:  ret

      IL_011c:  ldarg.0
      IL_011d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0122:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0127:  ldstr      "ChoiceResultHeader"
      IL_012c:  ldc.i4.1
      IL_012d:  newarr     [mscorlib]System.Object
      IL_0132:  stloc.s    V_6
      IL_0134:  ldloc.s    V_6
      IL_0136:  ldc.i4.0
      IL_0137:  ldarg.0
      IL_0138:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_013d:  stelem.ref
      IL_013e:  ldloc.s    V_6
      IL_0140:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_0145:  ldarg.0
      IL_0146:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_014b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0150:  ldtoken    Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_0155:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_015a:  ldarg.1
      IL_015b:  box        Sims3.Store.Objects.RoutineMachine/TeleportBase/DreamscapeNode
      IL_0160:  call       string [mscorlib]System.Enum::GetName(class [mscorlib]System.Type,
                                                                 object)
      IL_0165:  ldstr      "Success"
      IL_016a:  ldarg.3
      IL_016b:  box        [mscorlib]System.Int32
      IL_0170:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_0175:  ldc.i4.1
      IL_0176:  newarr     [mscorlib]System.Object
      IL_017b:  stloc.s    V_7
      IL_017d:  ldloc.s    V_7
      IL_017f:  ldc.i4.0
      IL_0180:  ldarg.0
      IL_0181:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0186:  stelem.ref
      IL_0187:  ldloc.s    V_7
      IL_0189:  call       string Sims3.Store.Objects.RoutineMachine/TeleportBase::LocalizeExploreString(bool,
                                                                                                         string,
                                                                                                         object[])
      IL_018e:  call       void [UI]Sims3.UI.SimpleMessageDialog::Show(string,
                                                                       string)
      IL_0193:  ldc.i4.0
      IL_0194:  stloc.1
      IL_0195:  br.s       IL_01d2

      IL_0197:  ldarg.0
      IL_0198:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_019d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_01a2:  ldc.i4     0x10f38d8
      IL_01a7:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::HasMotive(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_01ac:  brfalse.s  IL_01ce

      IL_01ae:  ldarg.0
      IL_01af:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01b4:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_01b9:  ldc.i4     0x10f38d8
      IL_01be:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kDreamscapeEnergyBoost
      IL_01c3:  ldc.r4     5.
      IL_01c8:  div
      IL_01c9:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::ChangeValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            float32)
      IL_01ce:  ldloc.1
      IL_01cf:  ldc.i4.1
      IL_01d0:  add
      IL_01d1:  stloc.1
      IL_01d2:  ldloc.1
      IL_01d3:  ldc.i4.5
      IL_01d4:  blt.s      IL_0197

      IL_01d6:  ldc.i4.1
      IL_01d7:  ret
    } // end of method TeleportBase::PassedSkillCheck

    .method public hidebysig instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject 
            GetRareItem() cil managed
    {
      // Code size       114 (0x72)
      .maxstack  4
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_0,
               string V_1,
               valuetype [SimIFace]Sims3.SimIFace.ProductVersion V_2,
               int32 V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_4)
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChanceOfExploreAwesomeGnome
      IL_0007:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomChance(float32)
      IL_000c:  brfalse.s  IL_0049

      IL_000e:  ldsfld     string [mscorlib]System.String::Empty
      IL_0013:  stloc.1
      IL_0014:  ldc.i4.0
      IL_0015:  stloc.2
      IL_0016:  ldsfld     string[] Sims3.Store.Objects.RoutineMachine/TeleportBase::kGnomeList
      IL_001b:  ldlen
      IL_001c:  conv.i4
      IL_001d:  ldc.i4.1
      IL_001e:  sub
      IL_001f:  call       int32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetInt(int32)
      IL_0024:  stloc.3
      IL_0025:  ldsfld     string[] Sims3.Store.Objects.RoutineMachine/TeleportBase::kGnomeList
      IL_002a:  ldloc.3
      IL_002b:  ldelem.ref
      IL_002c:  stloc.1
      IL_002d:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.ProductVersion[] Sims3.Store.Objects.RoutineMachine/TeleportBase::kGnomeProductVersions
      IL_0032:  ldloc.3
      IL_0033:  ldelem.u4
      IL_0034:  stloc.2
      IL_0035:  ldloc.2
      IL_0036:  call       bool [SimIFace]Sims3.SimIFace.GameUtils::IsInstalled(valuetype [SimIFace]Sims3.SimIFace.ProductVersion)
      IL_003b:  brfalse.s  IL_0016

      IL_003d:  ldloc.1
      IL_003e:  ldloc.2
      IL_003f:  ldnull
      IL_0040:  ldnull
      IL_0041:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.GlobalFunctions::CreateObjectOutOfWorld(string,
                                                                                                                                                                          valuetype [SimIFace]Sims3.SimIFace.ProductVersion,
                                                                                                                                                                          string,
                                                                                                                                                                          class [SimIFace]Sims3.SimIFace.Simulator/ObjectInitParameters)
      IL_0046:  stloc.0
      IL_0047:  br.s       IL_005a

      IL_0049:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kRewardRareGemMinCost
      IL_004e:  ldc.r4     3.4028235e+038
      IL_0053:  ldc.i4.1
      IL_0054:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetalBase::GetGemInPriceRange(float32,
                                                                                                                                                                                 float32,
                                                                                                                                                                                 bool)
      IL_0059:  stloc.0
      IL_005a:  ldloc.0
      IL_005b:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Core.FailureObject
      IL_0060:  stloc.s    V_4
      IL_0062:  ldloc.s    V_4
      IL_0064:  brfalse.s  IL_006e

      IL_0066:  ldloc.0
      IL_0067:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject::Destroy()
      IL_006c:  br.s       IL_0070

      IL_006e:  ldloc.0
      IL_006f:  ret

      IL_0070:  ldnull
      IL_0071:  ret
    } // end of method TeleportBase::GetRareItem

    .method public hidebysig instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject 
            GetRandomItem() cil managed
    {
      // Code size       300 (0x12c)
      .maxstack  4
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_0,
               valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/RewardItemTypes V_1,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Objects.Gardening.PlantRarity V_2,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Objects.Quality V_3,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Objects.Gardening.PlayerDisclosure V_4,
               class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData> V_5,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData V_6,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_7,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient V_8,
               valuetype Sims3.Store.Objects.RoutineMachine/TeleportBase/RewardItemTypes V_9,
               valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData> V_10)
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.3
      IL_0004:  call       int32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetInt(int32,
                                                                                              int32)
      IL_0009:  stloc.1
      IL_000a:  ldloc.1
      IL_000b:  stloc.s    V_9
      IL_000d:  ldloc.s    V_9
      IL_000f:  switch     ( 
                            IL_0026,
                            IL_0037,
                            IL_004d,
                            IL_0077)
      IL_0024:  br.s       IL_0077

      IL_0026:  call       valuetype [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetal [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetalBase::GetRandomMetal()
      IL_002b:  ldc.i4.1
      IL_002c:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetalBase::Make(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetal,
                                                                                                                                                                   bool)
      IL_0031:  stloc.0
      IL_0032:  br         IL_00fd

      IL_0037:  ldc.r4     0.0
      IL_003c:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kRewardRareGemMinCost
      IL_0041:  ldc.i4.1
      IL_0042:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RockGemMetalBase::GetGemInPriceRange(float32,
                                                                                                                                                                                 float32,
                                                                                                                                                                                 bool)
      IL_0047:  stloc.0
      IL_0048:  br         IL_00fd

      IL_004d:  ldc.i4.0
      IL_004e:  ldc.i4.1
      IL_004f:  call       int32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetInt(int32,
                                                                                              int32)
      IL_0054:  stloc.2
      IL_0055:  ldc.i4.4
      IL_0056:  ldc.i4.s   9
      IL_0058:  call       int32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetInt(int32,
                                                                                              int32)
      IL_005d:  stloc.3
      IL_005e:  ldloc.2
      IL_005f:  brfalse.s  IL_0064

      IL_0061:  ldc.i4.1
      IL_0062:  br.s       IL_0065

      IL_0064:  ldc.i4.0
      IL_0065:  stloc.s    V_4
      IL_0067:  ldloc.2
      IL_0068:  ldloc.3
      IL_0069:  ldc.i4.1
      IL_006a:  ldloc.s    V_4
      IL_006c:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Objects.Gardening.PlantHelper::CreateRandomPlantable(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Objects.Gardening.PlantRarity,
                                                                                                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.Objects.Quality,
                                                                                                                                                                                       bool,
                                                                                                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.Objects.Gardening.PlayerDisclosure)
      IL_0071:  stloc.0
      IL_0072:  br         IL_00fd

      IL_0077:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData> [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData::IngredientDataList
      IL_007c:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData>::get_Count()
      IL_0081:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData>::.ctor(int32)
      IL_0086:  stloc.s    V_5
      IL_0088:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData> [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData::IngredientDataList
      IL_008d:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData>::GetEnumerator()
      IL_0092:  stloc.s    V_10
      .try
      {
        IL_0094:  br.s       IL_00d7

        IL_0096:  ldloca.s   V_10
        IL_0098:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData>::get_Current()
        IL_009d:  stloc.s    V_6
        IL_009f:  ldloc.s    V_6
        IL_00a1:  ldfld      bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData::IsAbstract
        IL_00a6:  brtrue.s   IL_00d7

        IL_00a8:  ldloc.s    V_6
        IL_00aa:  ldfld      valuetype [SimIFace]Sims3.SimIFace.ProductVersion [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData::RequiredDataVersion
        IL_00af:  ldc.i4.1
        IL_00b0:  bne.un.s   IL_00d7

        IL_00b2:  ldloc.s    V_6
        IL_00b4:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData::PlantName
        IL_00b9:  call       bool [mscorlib]System.String::IsNullOrEmpty(string)
        IL_00be:  brfalse.s  IL_00d7

        IL_00c0:  ldloc.s    V_6
        IL_00c2:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData::ModelName
        IL_00c7:  call       bool [mscorlib]System.String::IsNullOrEmpty(string)
        IL_00cc:  brtrue.s   IL_00d7

        IL_00ce:  ldloc.s    V_5
        IL_00d0:  ldloc.s    V_6
        IL_00d2:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData>::Add(!0)
        IL_00d7:  ldloca.s   V_10
        IL_00d9:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData>::MoveNext()
        IL_00de:  brtrue.s   IL_0096

        IL_00e0:  leave.s    IL_00f0

      }  // end .try
      finally
      {
        IL_00e2:  ldloca.s   V_10
        IL_00e4:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData>
        IL_00ea:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_00ef:  endfinally
      }  // end handler
      IL_00f0:  ldloc.s    V_5
      IL_00f2:  call       !!0 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetRandomObjectFromList<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData>(class [mscorlib]System.Collections.Generic.List`1<!!0>)
      IL_00f7:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient::Create(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.IngredientData)
      IL_00fc:  stloc.0
      IL_00fd:  ldloc.0
      IL_00fe:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Core.FailureObject
      IL_0103:  stloc.s    V_7
      IL_0105:  ldloc.s    V_7
      IL_0107:  brfalse.s  IL_0111

      IL_0109:  ldloc.0
      IL_010a:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject::Destroy()
      IL_010f:  br.s       IL_012a

      IL_0111:  ldloc.0
      IL_0112:  brfalse.s  IL_0128

      IL_0114:  ldloc.0
      IL_0115:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient
      IL_011a:  stloc.s    V_8
      IL_011c:  ldloc.s    V_8
      IL_011e:  brfalse.s  IL_0128

      IL_0120:  ldloc.s    V_8
      IL_0122:  ldc.i4.1
      IL_0123:  stfld      bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.FoodObjects.Ingredient::IsFresh
      IL_0128:  ldloc.0
      IL_0129:  ret

      IL_012a:  ldnull
      IL_012b:  ret
    } // end of method TeleportBase::GetRandomItem

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMinBetweenEvents
      IL_0006:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMaxBetweenEvents
      IL_000b:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetFloat(float32,
                                                                                                  float32)
      IL_0010:  stfld      float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::mTimeUntilEvent
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::.ctor()
      IL_001b:  ret
    } // end of method TeleportBase::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       302 (0x12e)
      .maxstack  3
      .locals init (string[] V_0,
               valuetype [SimIFace]Sims3.SimIFace.ProductVersion[] V_1)
      IL_0000:  ldc.r4     5.
      IL_0005:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChangeHungerTime
      IL_000a:  ldc.r4     5.
      IL_000f:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChangeBladderTime
      IL_0014:  ldc.r4     10.
      IL_0019:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChangeHygieneTime
      IL_001e:  ldc.r4     50.
      IL_0023:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kThresholdHunger
      IL_0028:  ldc.r4     50.
      IL_002d:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kThresholdBladder
      IL_0032:  ldc.r4     50.
      IL_0037:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kThresholdHygiene
      IL_003c:  ldc.r4     3.
      IL_0041:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSleepyTimeStart
      IL_0046:  ldc.r4     8.
      IL_004b:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSleepyTimeEnd
      IL_0050:  ldc.r4     -20.
      IL_0055:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kMinEnergyToTeleport
      IL_005a:  ldc.r4     3.
      IL_005f:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSpeakerFun
      IL_0064:  ldc.r4     3.
      IL_0069:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMinBetweenEvents
      IL_006e:  ldc.r4     10.
      IL_0073:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kExploreMaxBetweenEvents
      IL_0078:  ldc.i4.5
      IL_0079:  stsfld     int32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kNumExploreEvents
      IL_007e:  ldc.i4.3
      IL_007f:  stsfld     int32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSkillCheck1Required
      IL_0084:  ldc.i4.5
      IL_0085:  stsfld     int32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kSkillCheck2Required
      IL_008a:  ldc.r4     10.
      IL_008f:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChanceOfExploreRandomFailure
      IL_0094:  ldc.r4     20.
      IL_0099:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kChanceOfExploreAwesomeGnome
      IL_009e:  ldc.r4     50.
      IL_00a3:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kDreamscapeEnergyBoost
      IL_00a8:  ldc.r4     200.
      IL_00ad:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportBase::kRewardRareGemMinCost
      IL_00b2:  ldc.i4.8
      IL_00b3:  newarr     [mscorlib]System.String
      IL_00b8:  stloc.0
      IL_00b9:  ldloc.0
      IL_00ba:  ldc.i4.0
      IL_00bb:  ldstr      "GardenGnome"
      IL_00c0:  stelem.ref
      IL_00c1:  ldloc.0
      IL_00c2:  ldc.i4.1
      IL_00c3:  ldstr      "MagicGnome"
      IL_00c8:  stelem.ref
      IL_00c9:  ldloc.0
      IL_00ca:  ldc.i4.2
      IL_00cb:  ldstr      "MagicGnomeEypt"
      IL_00d0:  stelem.ref
      IL_00d1:  ldloc.0
      IL_00d2:  ldc.i4.3
      IL_00d3:  ldstr      "MagicGnomeChina"
      IL_00d8:  stelem.ref
      IL_00d9:  ldloc.0
      IL_00da:  ldc.i4.4
      IL_00db:  ldstr      "MagicGnomeFrance"
      IL_00e0:  stelem.ref
      IL_00e1:  ldloc.0
      IL_00e2:  ldc.i4.5
      IL_00e3:  ldstr      "MagicGnomeInventor"
      IL_00e8:  stelem.ref
      IL_00e9:  ldloc.0
      IL_00ea:  ldc.i4.6
      IL_00eb:  ldstr      "MagicGnomeSculptor"
      IL_00f0:  stelem.ref
      IL_00f1:  ldloc.0
      IL_00f2:  ldc.i4.7
      IL_00f3:  ldstr      "MagicGnomeCelebrity"
      IL_00f8:  stelem.ref
      IL_00f9:  ldloc.0
      IL_00fa:  stsfld     string[] Sims3.Store.Objects.RoutineMachine/TeleportBase::kGnomeList
      IL_00ff:  ldc.i4.8
      IL_0100:  newarr     [SimIFace]Sims3.SimIFace.ProductVersion
      IL_0105:  stloc.1
      IL_0106:  ldloc.1
      IL_0107:  ldc.i4.0
      IL_0108:  ldc.i4.1
      IL_0109:  stelem.i4
      IL_010a:  ldloc.1
      IL_010b:  ldc.i4.1
      IL_010c:  ldc.i4.1
      IL_010d:  stelem.i4
      IL_010e:  ldloc.1
      IL_010f:  ldc.i4.2
      IL_0110:  ldc.i4.2
      IL_0111:  stelem.i4
      IL_0112:  ldloc.1
      IL_0113:  ldc.i4.3
      IL_0114:  ldc.i4.2
      IL_0115:  stelem.i4
      IL_0116:  ldloc.1
      IL_0117:  ldc.i4.4
      IL_0118:  ldc.i4.2
      IL_0119:  stelem.i4
      IL_011a:  ldloc.1
      IL_011b:  ldc.i4.5
      IL_011c:  ldc.i4.8
      IL_011d:  stelem.i4
      IL_011e:  ldloc.1
      IL_011f:  ldc.i4.6
      IL_0120:  ldc.i4.8
      IL_0121:  stelem.i4
      IL_0122:  ldloc.1
      IL_0123:  ldc.i4.7
      IL_0124:  ldc.i4.s   32
      IL_0126:  stelem.i4
      IL_0127:  ldloc.1
      IL_0128:  stsfld     valuetype [SimIFace]Sims3.SimIFace.ProductVersion[] Sims3.Store.Objects.RoutineMachine/TeleportBase::kGnomeProductVersions
      IL_012d:  ret
    } // end of method TeleportBase::.cctor

    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper
            SwitchOutfitHelper()
    {
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::get_SwitchOutfitHelper()
      .set instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::set_SwitchOutfitHelper(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper)
    } // end of property TeleportBase::SwitchOutfitHelper
    .property instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason
            NewClothesStyle()
    {
      .get instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason Sims3.Store.Objects.RoutineMachine/TeleportBase::get_NewClothesStyle()
    } // end of property TeleportBase::NewClothesStyle
  } // end of class TeleportBase

  .class auto ansi nested public beforefieldinit TeleportToWorkOrSchool
         extends Sims3.Store.Objects.RoutineMachine/TeleportBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool>
    {
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       85 (0x55)
        .maxstack  3
        IL_0000:  ldarg.1
        IL_0001:  call       bool Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::IsForOccupation(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_0006:  brfalse.s  IL_001f

        IL_0008:  ldarg.1
        IL_0009:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_000e:  ldstr      "TeleportToWork"
        IL_0013:  ldc.i4.0
        IL_0014:  newarr     [mscorlib]System.Object
        IL_0019:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_001e:  ret

        IL_001f:  ldarg.1
        IL_0020:  call       bool Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::IsForSchool(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_0025:  brfalse.s  IL_003e

        IL_0027:  ldarg.1
        IL_0028:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_002d:  ldstr      "TeleportToSchool"
        IL_0032:  ldc.i4.0
        IL_0033:  newarr     [mscorlib]System.Object
        IL_0038:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_003d:  ret

        IL_003e:  ldarg.1
        IL_003f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0044:  ldstr      "TeleportToWork"
        IL_0049:  ldc.i4.0
        IL_004a:  newarr     [mscorlib]System.Object
        IL_004f:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_0054:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       33 (0x21)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  ldarg.2
        IL_0003:  ldarg.3
        IL_0004:  ldarg.s    greyedOutTooltipCallback
        IL_0006:  call       instance bool class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool>::Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                                 class Sims3.Store.Objects.RoutineMachine,
                                                                                                                                                                                                 bool,
                                                                                                                                                                                                 class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_000b:  brfalse.s  IL_001f

        IL_000d:  ldarg.1
        IL_000e:  call       bool Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::IsForSchool(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_0013:  brtrue.s   IL_001d

        IL_0015:  ldarg.1
        IL_0016:  call       bool Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::IsForOccupation(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_001b:  brfalse.s  IL_001f

        IL_001d:  ldc.i4.1
        IL_001e:  ret

        IL_001f:  ldc.i4.0
        IL_0020:  ret
      } // end of method Definition::Test

    } // end of class Definition

    .field public static float32 kEarlyNoticeDelayInMinutes
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 42 4D 69 6E 75 74 65 73 20 62 65 66 6F 72   // ..BMinutes befor
                                                                                              65 20 54 4E 53 20 61 77 61 72 64 20 66 6F 72 20   // e TNS award for 
                                                                                              62 65 69 6E 67 20 65 61 72 6C 79 20 74 6F 20 77   // being early to w
                                                                                              6F 72 6B 2E 20 20 47 50 45 20 64 65 66 61 75 6C   // ork.  GPE defaul
                                                                                              74 3A 20 34 35 00 00 )                            // t: 45..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kEarlyBonusIfNotNoticed
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 52 43 61 72 65 65 72 20 62 6F 6E 75 73 20   // ..RCareer bonus 
                                                                                              70 65 72 20 68 6F 75 72 20 69 66 20 79 6F 75 20   // per hour if you 
                                                                                              74 65 6C 65 70 6F 72 74 20 65 61 72 6C 79 20 62   // teleport early b
                                                                                              75 74 20 61 72 65 20 6E 6F 74 20 6E 6F 74 69 63   // ut are not notic
                                                                                              65 64 2E 20 20 47 50 45 20 64 65 66 61 75 6C 74   // ed.  GPE default
                                                                                              3A 20 32 2E 35 00 00 )                            // : 2.5..
    .field public static float32 kEarlyBonusIfNoticed
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 4D 43 61 72 65 65 72 20 62 6F 6E 75 73 20   // ..MCareer bonus 
                                                                                              70 65 72 20 68 6F 75 72 20 69 66 20 79 6F 75 20   // per hour if you 
                                                                                              74 65 6C 65 70 6F 72 74 20 65 61 72 6C 79 20 61   // teleport early a
                                                                                              6E 64 20 61 72 65 20 6E 6F 74 69 63 65 64 2E 20   // nd are noticed. 
                                                                                              20 47 50 45 20 64 65 66 61 75 6C 74 3A 20 31 35   //  GPE default: 15
                                                                                              00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kEarlyBonusChanceToBeNoticed
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 49 43 68 61 6E 63 65 20 74 68 61 74 20 63   // ..IChance that c
                                                                                              6F 77 6F 72 6B 65 72 73 20 77 69 6C 6C 20 6E 6F   // oworkers will no
                                                                                              74 69 63 65 20 69 66 20 79 6F 75 20 74 65 6C 65   // tice if you tele
                                                                                              70 6F 72 74 20 65 61 72 6C 79 2E 20 20 47 50 45   // port early.  GPE
                                                                                              20 64 65 66 61 75 6C 74 3A 20 37 35 00 00 )       //  default: 75..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static float32 kEarlyBonusHoursEarlyToGetBonus
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 46 48 6F 77 20 65 61 72 6C 79 20 74 6F 20   // ..FHow early to 
                                                                                              62 65 20 73 6F 20 74 68 65 20 62 6F 73 73 20 77   // be so the boss w
                                                                                              69 6C 6C 20 70 6F 74 65 6E 74 69 61 6C 6C 79 20   // ill potentially 
                                                                                              6E 6F 74 69 63 65 2E 20 20 47 50 45 20 44 65 66   // notice.  GPE Def
                                                                                              61 75 6C 74 3A 20 2E 37 35 00 00 )                // ault: .75..
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig static bool 
            IsForSchool(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001a

      IL_0003:  ldarg.0
      IL_0004:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.School [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_School()
      IL_0009:  brfalse.s  IL_001a

      IL_000b:  ldarg.0
      IL_000c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.School [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_School()
      IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::IsAllowedToWork()
      IL_0016:  brfalse.s  IL_001a

      IL_0018:  ldc.i4.1
      IL_0019:  ret

      IL_001a:  ldc.i4.0
      IL_001b:  ret
    } // end of method TeleportToWorkOrSchool::IsForSchool

    .method public hidebysig static bool 
            IsForOccupation(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor) cil managed
    {
      // Code size       41 (0x29)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.0
      IL_0004:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
      IL_0009:  brfalse.s  IL_0027

      IL_000b:  ldarg.0
      IL_000c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
      IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::IsAllowedToWork()
      IL_0016:  brfalse.s  IL_0027

      IL_0018:  ldarg.0
      IL_0019:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
      IL_001e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::CanTeleportToWork()
      IL_0023:  brfalse.s  IL_0027

      IL_0025:  ldc.i4.1
      IL_0026:  ret

      IL_0027:  ldc.i4.0
      IL_0028:  ret
    } // end of method TeleportToWorkOrSchool::IsForOccupation

    .method public hidebysig specialname virtual 
            instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason 
            get_NewClothesStyle() cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0006:  pop
      IL_0007:  ldarg.0
      IL_0008:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_000d:  call       bool Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::IsForOccupation(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0012:  brtrue.s   IL_0016

      IL_0014:  ldc.i4.6
      IL_0015:  ret

      IL_0016:  ldc.i4.5
      IL_0017:  ret
    } // end of method TeleportToWorkOrSchool::get_NewClothesStyle

    .method public hidebysig instance void 
            WorkEarlyBonus() cil managed
    {
      // Code size       139 (0x8b)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Career V_0,
               string V_1,
               object[] V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Career [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_OccupationAsCareer()
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  brfalse.s  IL_008a

      IL_000f:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyBonusChanceToBeNoticed
      IL_0014:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomChance(float32)
      IL_0019:  brfalse.s  IL_0078

      IL_001b:  ldarg.0
      IL_001c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0021:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0026:  ldstr      "WorkEarlyBonus"
      IL_002b:  ldc.i4.1
      IL_002c:  newarr     [mscorlib]System.Object
      IL_0031:  stloc.2
      IL_0032:  ldloc.2
      IL_0033:  ldc.i4.0
      IL_0034:  ldarg.0
      IL_0035:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_003a:  stelem.ref
      IL_003b:  ldloc.2
      IL_003c:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
      IL_0041:  stloc.1
      IL_0042:  ldarg.0
      IL_0043:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0048:  ldloc.1
      IL_0049:  ldc.i4.3
      IL_004a:  ldarg.0
      IL_004b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0050:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
      IL_0055:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [SimIFace]Sims3.SimIFace.ObjectGuid::InvalidObjectGuid
      IL_005a:  ldstr      "sting_career_positive"
      IL_005f:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSAndPlayStingIfSelectable(string,
                                                                                                                                                           valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle,
                                                                                                                                                           valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                                           valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                                           string)
      IL_0064:  pop
      IL_0065:  ldloc.0
      IL_0066:  dup
      IL_0067:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Careers.Career::PerformanceBonusPerHour
      IL_006c:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyBonusIfNoticed
      IL_0071:  add
      IL_0072:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Careers.Career::PerformanceBonusPerHour
      IL_0077:  ret

      IL_0078:  ldloc.0
      IL_0079:  dup
      IL_007a:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Careers.Career::PerformanceBonusPerHour
      IL_007f:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyBonusIfNotNoticed
      IL_0084:  add
      IL_0085:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Careers.Career::PerformanceBonusPerHour
      IL_008a:  ret
    } // end of method TeleportToWorkOrSchool::WorkEarlyBonus

    .method public hidebysig virtual instance bool 
            DoTeleport(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject gameObject) cil managed
    {
      // Code size       346 (0x15a)
      .maxstack  6
      .locals init (class Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool/Definition V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject V_1,
               bool V_2,
               bool V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition V_4,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_5)
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0006:  isinst     Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool/Definition
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  brfalse.s  IL_001f

      IL_000f:  ldarg.0
      IL_0010:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0015:  brfalse.s  IL_001f

      IL_0017:  ldarg.0
      IL_0018:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_001d:  brtrue.s   IL_0021

      IL_001f:  ldc.i4.0
      IL_0020:  ret

      IL_0021:  ldnull
      IL_0022:  stloc.1
      IL_0023:  ldarg.0
      IL_0024:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0029:  call       bool Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::IsForOccupation(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_002e:  stloc.2
      IL_002f:  ldloc.2
      IL_0030:  brfalse.s  IL_005a

      IL_0032:  ldarg.0
      IL_0033:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0038:  brfalse.s  IL_005a

      IL_003a:  ldarg.0
      IL_003b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0040:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
      IL_0045:  brfalse.s  IL_005a

      IL_0047:  ldarg.0
      IL_0048:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_004d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
      IL_0052:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::get_OfficeLocation()
      IL_0057:  stloc.1
      IL_0058:  br.s       IL_009c

      IL_005a:  ldarg.0
      IL_005b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0060:  call       bool Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::IsForSchool(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0065:  brfalse.s  IL_009c

      IL_0067:  ldarg.0
      IL_0068:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_006d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.School [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_School()
      IL_0072:  brfalse.s  IL_009c

      IL_0074:  ldarg.0
      IL_0075:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_007a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.School [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_School()
      IL_007f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.CareerLocation [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::get_CareerLoc()
      IL_0084:  brfalse.s  IL_009c

      IL_0086:  ldarg.0
      IL_0087:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_008c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.School [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_School()
      IL_0091:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.CareerLocation [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::get_CareerLoc()
      IL_0096:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RabbitHole [Sims3GameplaySystems]Sims3.Gameplay.Careers.CareerLocation::Owner
      IL_009b:  stloc.1
      IL_009c:  ldloc.1
      IL_009d:  brtrue.s   IL_00a1

      IL_009f:  ldc.i4.0
      IL_00a0:  ret

      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_00a7:  ldarg.0
      IL_00a8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00ad:  ldloc.1
      IL_00ae:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine::Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_00b3:  stloc.3
      IL_00b4:  ldloc.3
      IL_00b5:  brfalse.s  IL_010c

      IL_00b7:  ldloc.2
      IL_00b8:  brfalse.s  IL_010c

      IL_00ba:  ldarg.0
      IL_00bb:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00c0:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
      IL_00c5:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::get_HoursUntilWork()
      IL_00ca:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyBonusHoursEarlyToGetBonus
      IL_00cf:  ble.un.s   IL_010c

      IL_00d1:  ldarg.0
      IL_00d2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00d7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
      IL_00dc:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::get_HoursUntilWork()
      IL_00e1:  ldc.r4     12.
      IL_00e6:  bge.un.s   IL_010c

      IL_00e8:  ldarg.0
      IL_00e9:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_00ee:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyNoticeDelayInMinutes
      IL_00f3:  ldc.i4.1
      IL_00f4:  ldarg.0
      IL_00f5:  ldftn      instance void Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::WorkEarlyBonus()
      IL_00fb:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback::.ctor(object,
                                                                                                                  native int)
      IL_0100:  ldstr      "Get to work early bonus"
      IL_0105:  ldc.i4.1
      IL_0106:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddAlarm(float32,
                                                                                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                             class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback,
                                                                                                                                                                             string,
                                                                                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmType)
      IL_010b:  pop
      IL_010c:  ldloc.3
      IL_010d:  brfalse.s  IL_0158

      IL_010f:  ldloc.1
      IL_0110:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RabbitHole
      IL_0115:  brfalse.s  IL_0158

      IL_0117:  ldloc.2
      IL_0118:  brtrue.s   IL_0121

      IL_011a:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Careers.GoToSchoolInRabbitHole::Singleton
      IL_011f:  br.s       IL_0126

      IL_0121:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Careers.WorkInRabbitHole::Singleton
      IL_0126:  stloc.s    V_4
      IL_0128:  ldloc.s    V_4
      IL_012a:  ldloc.1
      IL_012b:  ldarg.0
      IL_012c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0131:  ldarg.0
      IL_0132:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0137:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_013c:  ldc.i4.0
      IL_013d:  ldc.i4.1
      IL_013e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0143:  stloc.s    V_5
      IL_0145:  ldarg.0
      IL_0146:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_014b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0150:  ldloc.s    V_5
      IL_0152:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::AddNext(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0157:  ret

      IL_0158:  ldloc.3
      IL_0159:  ret
    } // end of method TeleportToWorkOrSchool::DoTeleport

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::.ctor()
      IL_0006:  ret
    } // end of method TeleportToWorkOrSchool::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       61 (0x3d)
      .maxstack  8
      IL_0000:  ldc.r4     45.
      IL_0005:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyNoticeDelayInMinutes
      IL_000a:  ldc.r4     2.5
      IL_000f:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyBonusIfNotNoticed
      IL_0014:  ldc.r4     15.
      IL_0019:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyBonusIfNoticed
      IL_001e:  ldc.r4     75.
      IL_0023:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyBonusChanceToBeNoticed
      IL_0028:  ldc.r4     0.75
      IL_002d:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::kEarlyBonusHoursEarlyToGetBonus
      IL_0032:  newobj     instance void Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool/Definition::.ctor()
      IL_0037:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::Singleton
      IL_003c:  ret
    } // end of method TeleportToWorkOrSchool::.cctor

    .property instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason
            NewClothesStyle()
    {
      .get instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::get_NewClothesStyle()
    } // end of property TeleportToWorkOrSchool::NewClothesStyle
  } // end of class TeleportToWorkOrSchool

  .class auto ansi nested public beforefieldinit TeleportToShop
         extends Sims3.Store.Objects.RoutineMachine/TeleportBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToShop>
    {
      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       23 (0x17)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0006:  ldstr      "TeleportToShop"
        IL_000b:  ldc.i4.0
        IL_000c:  newarr     [mscorlib]System.Object
        IL_0011:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_0016:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       52 (0x34)
        .maxstack  8
        IL_0000:  call       uint32 [Sims3GameplaySystems]Sims3.Gameplay.Queries::CountObjects<class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole>()
        IL_0005:  ldc.i4.1
        IL_0006:  bge.un.s   IL_0028

        IL_0008:  ldarg.s    greyedOutTooltipCallback
        IL_000a:  ldarg.1
        IL_000b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0010:  ldstr      "NoShopsFound"
        IL_0015:  ldc.i4.0
        IL_0016:  newarr     [mscorlib]System.Object
        IL_001b:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_0020:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_0025:  stind.ref
        IL_0026:  ldc.i4.0
        IL_0027:  ret

        IL_0028:  ldarg.0
        IL_0029:  ldarg.1
        IL_002a:  ldarg.2
        IL_002b:  ldarg.3
        IL_002c:  ldarg.s    greyedOutTooltipCallback
        IL_002e:  call       instance bool class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToShop>::Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                         class Sims3.Store.Objects.RoutineMachine,
                                                                                                                                                                                         bool,
                                                                                                                                                                                         class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_0033:  ret
      } // end of method Definition::Test

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToShop>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            PreTeleport([out] class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject& destinationObject,
                        [out] class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot& targetLot) cil managed
    {
      // Code size       41 (0x29)
      .maxstack  2
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole[] V_0)
      IL_0000:  call       !!0[] [Sims3GameplaySystems]Sims3.Gameplay.Queries::GetObjects<class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole>()
      IL_0005:  stloc.0
      IL_0006:  ldarg.1
      IL_0007:  ldloc.0
      IL_0008:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole [Sims3GameplaySystems]Sims3.Gameplay.Objects.Vehicles.CarOwnable::ChooseStore(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole[])
      IL_000d:  stind.ref
      IL_000e:  ldarg.1
      IL_000f:  ldind.ref
      IL_0010:  brfalse.s  IL_001d

      IL_0012:  ldarg.2
      IL_0013:  ldarg.1
      IL_0014:  ldind.ref
      IL_0015:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::get_LotCurrent()
      IL_001a:  stind.ref
      IL_001b:  br.s       IL_0020

      IL_001d:  ldarg.2
      IL_001e:  ldnull
      IL_001f:  stind.ref
      IL_0020:  ldarg.1
      IL_0021:  ldind.ref
      IL_0022:  ldnull
      IL_0023:  ceq
      IL_0025:  ldc.i4.0
      IL_0026:  ceq
      IL_0028:  ret
    } // end of method TeleportToShop::PreTeleport

    .method public hidebysig virtual instance bool 
            DoTeleport(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject gameObject) cil managed
    {
      // Code size       86 (0x56)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_000c:  ldarg.1
      IL_000d:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine::Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_0012:  brfalse.s  IL_0054

      IL_0014:  ldarg.1
      IL_0015:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole
      IL_001a:  stloc.0
      IL_001b:  ldloc.0
      IL_001c:  brfalse.s  IL_0054

      IL_001e:  ldloc.0
      IL_001f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole::GetObjectSpecificShoppingInteractionDefinition()
      IL_0024:  stloc.1
      IL_0025:  ldloc.1
      IL_0026:  ldarg.1
      IL_0027:  ldarg.0
      IL_0028:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_002d:  ldarg.0
      IL_002e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0033:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_0038:  ldc.i4.0
      IL_0039:  ldc.i4.1
      IL_003a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_003f:  stloc.2
      IL_0040:  ldarg.0
      IL_0041:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0046:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_004b:  ldloc.2
      IL_004c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::AddNext(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0051:  pop
      IL_0052:  ldc.i4.1
      IL_0053:  ret

      IL_0054:  ldc.i4.0
      IL_0055:  ret
    } // end of method TeleportToShop::DoTeleport

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::.ctor()
      IL_0006:  ret
    } // end of method TeleportToShop::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.RoutineMachine/TeleportToShop/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToShop::Singleton
      IL_000a:  ret
    } // end of method TeleportToShop::.cctor

  } // end of class TeleportToShop

  .class auto ansi nested public beforefieldinit TeleportToGym
         extends Sims3.Store.Objects.RoutineMachine/TeleportBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToGym>
    {
      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       23 (0x17)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0006:  ldstr      "TeleportToGym"
        IL_000b:  ldc.i4.0
        IL_000c:  newarr     [mscorlib]System.Object
        IL_0011:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_0016:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       58 (0x3a)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  call       class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> Sims3.Store.Objects.RoutineMachine/TeleportToGym::GetGymTeleportList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_0006:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>::get_Count()
        IL_000b:  ldc.i4.1
        IL_000c:  bge.s      IL_002e

        IL_000e:  ldarg.s    greyedOutTooltipCallback
        IL_0010:  ldarg.1
        IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0016:  ldstr      "NoGymsFound"
        IL_001b:  ldc.i4.0
        IL_001c:  newarr     [mscorlib]System.Object
        IL_0021:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_0026:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_002b:  stind.ref
        IL_002c:  ldc.i4.0
        IL_002d:  ret

        IL_002e:  ldarg.0
        IL_002f:  ldarg.1
        IL_0030:  ldarg.2
        IL_0031:  ldarg.3
        IL_0032:  ldarg.s    greyedOutTooltipCallback
        IL_0034:  call       instance bool class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToGym>::Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                        class Sims3.Store.Objects.RoutineMachine,
                                                                                                                                                                                        bool,
                                                                                                                                                                                        class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_0039:  ret
      } // end of method Definition::Test

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToGym>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> 
            GetGymTeleportList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a) cil managed
    {
      // Code size       85 (0x55)
      .maxstack  2
      .locals init (class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IMetaObject V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot V_2,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IMetaObject> V_3)
      IL_0000:  ldc.i4.s   10
      IL_0002:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>::.ctor(int32)
      IL_0007:  stloc.0
      IL_0008:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IMetaObject> [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_PublicMetaObjects()
      IL_000d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IMetaObject>::GetEnumerator()
      IL_0012:  stloc.3
      .try
      {
        IL_0013:  br.s       IL_003f

        IL_0015:  ldloc.3
        IL_0016:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IMetaObject>::get_Current()
        IL_001b:  stloc.1
        IL_001c:  ldloc.1
        IL_001d:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot
        IL_0022:  stloc.2
        IL_0023:  ldloc.2
        IL_0024:  brfalse.s  IL_003f

        IL_0026:  ldloc.2
        IL_0027:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot/MetaAutonomyType [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_GetMetaAutonomyType()
        IL_002c:  ldc.i4.7
        IL_002d:  bne.un.s   IL_003f

        IL_002f:  ldloc.2
        IL_0030:  ldarg.0
        IL_0031:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_LotCurrent()
        IL_0036:  beq.s      IL_003f

        IL_0038:  ldloc.0
        IL_0039:  ldloc.2
        IL_003a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>::Add(!0)
        IL_003f:  ldloc.3
        IL_0040:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0045:  brtrue.s   IL_0015

        IL_0047:  leave.s    IL_0053

      }  // end .try
      finally
      {
        IL_0049:  ldloc.3
        IL_004a:  brfalse.s  IL_0052

        IL_004c:  ldloc.3
        IL_004d:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0052:  endfinally
      }  // end handler
      IL_0053:  ldloc.0
      IL_0054:  ret
    } // end of method TeleportToGym::GetGymTeleportList

    .method public hidebysig specialname virtual 
            instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason 
            get_NewClothesStyle() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.7
      IL_0001:  ret
    } // end of method TeleportToGym::get_NewClothesStyle

    .method public hidebysig virtual instance bool 
            PreTeleport([out] class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject& destinationObject,
                        [out] class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot& targetLot) cil managed
    {
      // Code size       76 (0x4c)
      .maxstack  3
      .locals init (class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0006:  call       class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> Sims3.Store.Objects.RoutineMachine/TeleportToGym::GetGymTeleportList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>::get_Count()
      IL_0012:  ldc.i4.1
      IL_0013:  bne.un.s   IL_0029

      IL_0015:  ldarg.1
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.0
      IL_0018:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>::get_Item(int32)
      IL_001d:  stind.ref
      IL_001e:  ldarg.2
      IL_001f:  ldarg.1
      IL_0020:  ldind.ref
      IL_0021:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::get_LotCurrent()
      IL_0026:  stind.ref
      IL_0027:  ldc.i4.1
      IL_0028:  ret

      IL_0029:  ldarg.1
      IL_002a:  ldloc.0
      IL_002b:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Objects.Vehicles.CarOwnable::ChooseLot(class [mscorlib]System.Collections.Generic.ICollection`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>)
      IL_0030:  stind.ref
      IL_0031:  ldarg.1
      IL_0032:  ldind.ref
      IL_0033:  brfalse.s  IL_0040

      IL_0035:  ldarg.2
      IL_0036:  ldarg.1
      IL_0037:  ldind.ref
      IL_0038:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::get_LotCurrent()
      IL_003d:  stind.ref
      IL_003e:  br.s       IL_0043

      IL_0040:  ldarg.2
      IL_0041:  ldnull
      IL_0042:  stind.ref
      IL_0043:  ldarg.1
      IL_0044:  ldind.ref
      IL_0045:  ldnull
      IL_0046:  ceq
      IL_0048:  ldc.i4.0
      IL_0049:  ceq
      IL_004b:  ret
    } // end of method TeleportToGym::PreTeleport

    .method public hidebysig virtual instance bool 
            DoTeleport(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject gameObject) cil managed
    {
      // Code size       86 (0x56)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_000c:  ldarg.1
      IL_000d:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine::Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_0012:  brfalse.s  IL_0054

      IL_0014:  ldarg.1
      IL_0015:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole
      IL_001a:  stloc.0
      IL_001b:  ldloc.0
      IL_001c:  brfalse.s  IL_0054

      IL_001e:  ldloc.0
      IL_001f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole::GetObjectSpecificShoppingInteractionDefinition()
      IL_0024:  stloc.1
      IL_0025:  ldloc.1
      IL_0026:  ldarg.1
      IL_0027:  ldarg.0
      IL_0028:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_002d:  ldarg.0
      IL_002e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0033:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_0038:  ldc.i4.0
      IL_0039:  ldc.i4.1
      IL_003a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_003f:  stloc.2
      IL_0040:  ldarg.0
      IL_0041:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0046:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_004b:  ldloc.2
      IL_004c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::AddNext(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0051:  pop
      IL_0052:  ldc.i4.1
      IL_0053:  ret

      IL_0054:  ldc.i4.0
      IL_0055:  ret
    } // end of method TeleportToGym::DoTeleport

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::.ctor()
      IL_0006:  ret
    } // end of method TeleportToGym::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.RoutineMachine/TeleportToGym/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToGym::Singleton
      IL_000a:  ret
    } // end of method TeleportToGym::.cctor

    .property instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason
            NewClothesStyle()
    {
      .get instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ClothesChangeReason Sims3.Store.Objects.RoutineMachine/TeleportToGym::get_NewClothesStyle()
    } // end of property TeleportToGym::NewClothesStyle
  } // end of class TeleportToGym

  .class auto ansi nested public beforefieldinit TeleportToCommunityLot
         extends Sims3.Store.Objects.RoutineMachine/TeleportBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToCommunityLot>
    {
      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       23 (0x17)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0006:  ldstr      "TeleportToCommunityLot"
        IL_000b:  ldc.i4.0
        IL_000c:  newarr     [mscorlib]System.Object
        IL_0011:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_0016:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       58 (0x3a)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  call       class [mscorlib]System.Collections.Generic.ICollection`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> [Sims3GameplaySystems]Sims3.Gameplay.Objects.Vehicles.CarOwnable::GetValidCommunityLots(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_0006:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>::get_Count()
        IL_000b:  ldc.i4.1
        IL_000c:  bge.s      IL_002e

        IL_000e:  ldarg.s    greyedOutTooltipCallback
        IL_0010:  ldarg.1
        IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0016:  ldstr      "NoShopsFound"
        IL_001b:  ldc.i4.0
        IL_001c:  newarr     [mscorlib]System.Object
        IL_0021:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_0026:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_002b:  stind.ref
        IL_002c:  ldc.i4.0
        IL_002d:  ret

        IL_002e:  ldarg.0
        IL_002f:  ldarg.1
        IL_0030:  ldarg.2
        IL_0031:  ldarg.3
        IL_0032:  ldarg.s    greyedOutTooltipCallback
        IL_0034:  call       instance bool class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToCommunityLot>::Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                                 class Sims3.Store.Objects.RoutineMachine,
                                                                                                                                                                                                 bool,
                                                                                                                                                                                                 class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_0039:  ret
      } // end of method Definition::Test

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToCommunityLot>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            PreTeleport([out] class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject& destinationObject,
                        [out] class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot& targetLot) cil managed
    {
      // Code size       70 (0x46)
      .maxstack  2
      .locals init (class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0006:  call       class [mscorlib]System.Collections.Generic.ICollection`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> [Sims3GameplaySystems]Sims3.Gameplay.Objects.Vehicles.CarOwnable::GetValidCommunityLots(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_000b:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0010:  stloc.0
      IL_0011:  ldarg.0
      IL_0012:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0017:  brfalse.s  IL_0023

      IL_0019:  ldarg.1
      IL_001a:  ldloc.0
      IL_001b:  call       !!0 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetRandomObjectFromList<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>(class [mscorlib]System.Collections.Generic.List`1<!!0>)
      IL_0020:  stind.ref
      IL_0021:  br.s       IL_002b

      IL_0023:  ldarg.1
      IL_0024:  ldloc.0
      IL_0025:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Objects.Vehicles.CarOwnable::ChooseLot(class [mscorlib]System.Collections.Generic.ICollection`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>)
      IL_002a:  stind.ref
      IL_002b:  ldarg.1
      IL_002c:  ldind.ref
      IL_002d:  brfalse.s  IL_003a

      IL_002f:  ldarg.2
      IL_0030:  ldarg.1
      IL_0031:  ldind.ref
      IL_0032:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::get_LotCurrent()
      IL_0037:  stind.ref
      IL_0038:  br.s       IL_003d

      IL_003a:  ldarg.2
      IL_003b:  ldnull
      IL_003c:  stind.ref
      IL_003d:  ldarg.1
      IL_003e:  ldind.ref
      IL_003f:  ldnull
      IL_0040:  ceq
      IL_0042:  ldc.i4.0
      IL_0043:  ceq
      IL_0045:  ret
    } // end of method TeleportToCommunityLot::PreTeleport

    .method public hidebysig virtual instance bool 
            DoTeleport(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject gameObject) cil managed
    {
      // Code size       364 (0x16c)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot V_0,
               bool V_1,
               valuetype [SimIFace]Sims3.SimIFace.Vector3[] V_2,
               int32 V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_4)
      IL_0000:  ldarg.1
      IL_0001:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse    IL_016a

      IL_000d:  ldc.i4.0
      IL_000e:  stloc.1
      IL_000f:  ldc.i4.5
      IL_0010:  newarr     [SimIFace]Sims3.SimIFace.Vector3
      IL_0015:  stloc.2
      IL_0016:  ldloc.2
      IL_0017:  ldc.i4.0
      IL_0018:  ldelema    [SimIFace]Sims3.SimIFace.Vector3
      IL_001d:  ldloc.0
      IL_001e:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Position()
      IL_0023:  stobj      [SimIFace]Sims3.SimIFace.Vector3
      IL_0028:  ldloc.2
      IL_0029:  ldc.i4.1
      IL_002a:  ldelema    [SimIFace]Sims3.SimIFace.Vector3
      IL_002f:  ldloc.0
      IL_0030:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_Corners()
      IL_0035:  ldc.i4.0
      IL_0036:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners::get_Item(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Corner)
      IL_003b:  ldloc.0
      IL_003c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_Corners()
      IL_0041:  ldc.i4.3
      IL_0042:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners::get_Item(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Corner)
      IL_0047:  call       valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::op_Addition(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                    valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_004c:  ldc.r4     2.
      IL_0051:  call       valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::op_Division(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                    float32)
      IL_0056:  stobj      [SimIFace]Sims3.SimIFace.Vector3
      IL_005b:  ldloc.2
      IL_005c:  ldc.i4.2
      IL_005d:  ldelema    [SimIFace]Sims3.SimIFace.Vector3
      IL_0062:  ldloc.0
      IL_0063:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_Corners()
      IL_0068:  ldc.i4.3
      IL_0069:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners::get_Item(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Corner)
      IL_006e:  ldloc.0
      IL_006f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_Corners()
      IL_0074:  ldc.i4.2
      IL_0075:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners::get_Item(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Corner)
      IL_007a:  call       valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::op_Addition(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                    valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_007f:  ldc.r4     2.
      IL_0084:  call       valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::op_Division(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                    float32)
      IL_0089:  stobj      [SimIFace]Sims3.SimIFace.Vector3
      IL_008e:  ldloc.2
      IL_008f:  ldc.i4.3
      IL_0090:  ldelema    [SimIFace]Sims3.SimIFace.Vector3
      IL_0095:  ldloc.0
      IL_0096:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_Corners()
      IL_009b:  ldc.i4.1
      IL_009c:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners::get_Item(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Corner)
      IL_00a1:  ldloc.0
      IL_00a2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_Corners()
      IL_00a7:  ldc.i4.2
      IL_00a8:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners::get_Item(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Corner)
      IL_00ad:  call       valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::op_Addition(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                    valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_00b2:  ldc.r4     2.
      IL_00b7:  call       valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::op_Division(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                    float32)
      IL_00bc:  stobj      [SimIFace]Sims3.SimIFace.Vector3
      IL_00c1:  ldloc.2
      IL_00c2:  ldc.i4.4
      IL_00c3:  ldelema    [SimIFace]Sims3.SimIFace.Vector3
      IL_00c8:  ldloc.0
      IL_00c9:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_Corners()
      IL_00ce:  ldc.i4.0
      IL_00cf:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners::get_Item(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Corner)
      IL_00d4:  ldloc.0
      IL_00d5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_Corners()
      IL_00da:  ldc.i4.1
      IL_00db:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Core.Corners::get_Item(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.Corner)
      IL_00e0:  call       valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::op_Addition(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                    valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_00e5:  ldc.r4     2.
      IL_00ea:  call       valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::op_Division(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                                                    float32)
      IL_00ef:  stobj      [SimIFace]Sims3.SimIFace.Vector3
      IL_00f4:  ldc.i4.0
      IL_00f5:  stloc.3
      IL_00f6:  br.s       IL_011f

      IL_00f8:  ldarg.0
      IL_00f9:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_00fe:  ldarg.0
      IL_00ff:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0104:  ldloc.2
      IL_0105:  ldloc.3
      IL_0106:  ldelema    [SimIFace]Sims3.SimIFace.Vector3
      IL_010b:  ldobj      [SimIFace]Sims3.SimIFace.Vector3
      IL_0110:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine::Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_0115:  brfalse.s  IL_011b

      IL_0117:  ldc.i4.1
      IL_0118:  stloc.1
      IL_0119:  br.s       IL_0125

      IL_011b:  ldloc.3
      IL_011c:  ldc.i4.1
      IL_011d:  add
      IL_011e:  stloc.3
      IL_011f:  ldloc.3
      IL_0120:  ldloc.2
      IL_0121:  ldlen
      IL_0122:  conv.i4
      IL_0123:  blt.s      IL_00f8

      IL_0125:  ldloc.1
      IL_0126:  brfalse.s  IL_016a

      IL_0128:  ldarg.0
      IL_0129:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_012e:  ldloc.0
      IL_012f:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SwitchToFormalOutfitIfCocktail(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot)
      IL_0134:  pop
      IL_0135:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Core.VisitCommunityLot::Singleton
      IL_013a:  ldarg.1
      IL_013b:  ldarg.0
      IL_013c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0141:  ldarg.0
      IL_0142:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0147:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
      IL_014c:  ldc.i4.0
      IL_014d:  ldc.i4.1
      IL_014e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0153:  stloc.s    V_4
      IL_0155:  ldarg.0
      IL_0156:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_015b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0160:  ldloc.s    V_4
      IL_0162:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::AddNext(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0167:  pop
      IL_0168:  ldc.i4.1
      IL_0169:  ret

      IL_016a:  ldc.i4.0
      IL_016b:  ret
    } // end of method TeleportToCommunityLot::DoTeleport

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::.ctor()
      IL_0006:  ret
    } // end of method TeleportToCommunityLot::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.RoutineMachine/TeleportToCommunityLot/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToCommunityLot::Singleton
      IL_000a:  ret
    } // end of method TeleportToCommunityLot::.cctor

  } // end of class TeleportToCommunityLot

  .class auto ansi nested public beforefieldinit TeleportToPointDestination
         extends Sims3.Store.Objects.RoutineMachine/TeleportBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination>
    {
      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       29 (0x1d)
        .maxstack  5
        .locals init (object[] V_0)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0006:  ldstr      "TeleportHere"
        IL_000b:  ldc.i4.1
        IL_000c:  newarr     [mscorlib]System.Object
        IL_0011:  stloc.0
        IL_0012:  ldloc.0
        IL_0013:  ldc.i4.0
        IL_0014:  ldarg.1
        IL_0015:  stelem.ref
        IL_0016:  ldloc.0
        IL_0017:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
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
        IL_0001:  call       instance void class Sims3.Store.Objects.RoutineMachine/TeleportBase/TeleportDefinitionBase`1<class Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static float32 kChanceOfSuccessWhenTeleportAnywhereIncomplete
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 6F 57 68 65 6E 20 74 68 65 79 20 61 74 74   // ..oWhen they att
                                                                                              65 6D 70 74 20 74 6F 20 74 65 6C 65 70 6F 72 74   // empt to teleport
                                                                                              20 62 75 74 20 64 6F 6E 27 74 20 68 61 76 65 20   //  but don't have 
                                                                                              74 68 65 20 6D 61 63 68 69 6E 65 20 75 70 67 72   // the machine upgr
                                                                                              61 64 65 64 2C 20 63 68 61 6E 63 65 20 6F 66 20   // aded, chance of 
                                                                                              73 75 63 63 65 65 64 69 6E 67 20 61 6E 79 77 61   // succeeding anywa
                                                                                              79 2E 20 47 50 45 20 64 65 66 61 75 6C 74 3A 20   // y. GPE default: 
                                                                                              38 35 00 00 )                                     // 85..
    .field public valuetype [SimIFace]Sims3.SimIFace.Vector3 Destination
    .field public bool locationSuccess
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            DoTeleport(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject gameObject) cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (object[] V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      bool Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::locationSuccess
      IL_0006:  brtrue.s   IL_0064

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_000e:  ldarg.0
      IL_000f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0014:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
      IL_0019:  ldstr      "TransporterNotUpgraded"
      IL_001e:  ldc.i4.1
      IL_001f:  newarr     [mscorlib]System.Object
      IL_0024:  stloc.0
      IL_0025:  ldloc.0
      IL_0026:  ldc.i4.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_002d:  stelem.ref
      IL_002e:  ldloc.0
      IL_002f:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
      IL_0034:  ldc.i4.3
      IL_0035:  ldarg.0
      IL_0036:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_003b:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
      IL_0040:  ldarg.0
      IL_0041:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0046:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
      IL_004b:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                               valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle,
                                                                                                                                               valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                               valuetype [SimIFace]Sims3.SimIFace.ObjectGuid)
      IL_0050:  pop
      IL_0051:  ldarg.0
      IL_0052:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0057:  ldarg.0
      IL_0058:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_005d:  ldarg.1
      IL_005e:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine::Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_0063:  ret

      IL_0064:  ldarg.0
      IL_0065:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_006a:  ldarg.0
      IL_006b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0070:  ldarg.0
      IL_0071:  ldfld      valuetype [SimIFace]Sims3.SimIFace.Vector3 Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::Destination
      IL_0076:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine::Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      valuetype [SimIFace]Sims3.SimIFace.Vector3)
      IL_007b:  ret
    } // end of method TeleportToPointDestination::DoTeleport

    .method public hidebysig virtual instance bool 
            PreTeleport([out] class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject& destinationObject,
                        [out] class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot& targetLot) cil managed
    {
      // Code size       115 (0x73)
      .maxstack  2
      .locals init (bool V_0,
               class [mscorlib]System.Collections.Generic.ICollection`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> V_1,
               class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> V_2)
      IL_0000:  ldsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::kChanceOfSuccessWhenTeleportAnywhereIncomplete
      IL_0005:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::RandomChance(float32)
      IL_000a:  stloc.0
      IL_000b:  ldarg.0
      IL_000c:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0011:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_0016:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_001b:  ldc.i4.s   17
      IL_001d:  beq.s      IL_0022

      IL_001f:  ldloc.0
      IL_0020:  brfalse.s  IL_0036

      IL_0022:  ldarg.0
      IL_0023:  ldc.i4.1
      IL_0024:  stfld      bool Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::locationSuccess
      IL_0029:  ldarg.1
      IL_002a:  ldnull
      IL_002b:  stind.ref
      IL_002c:  ldarg.2
      IL_002d:  ldarg.0
      IL_002e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot Sims3.Store.Objects.RoutineMachine/TeleportBase::mTargetLot
      IL_0033:  stind.ref
      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.0
      IL_0038:  stfld      bool Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::locationSuccess
      IL_003d:  ldarg.0
      IL_003e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0043:  call       class [mscorlib]System.Collections.Generic.ICollection`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot> [Sims3GameplaySystems]Sims3.Gameplay.Objects.Vehicles.CarOwnable::GetValidCommunityLots(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0048:  stloc.1
      IL_0049:  ldloc.1
      IL_004a:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_004f:  stloc.2
      IL_0050:  ldarg.1
      IL_0051:  ldloc.2
      IL_0052:  call       !!0 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetRandomObjectFromList<class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot>(class [mscorlib]System.Collections.Generic.List`1<!!0>)
      IL_0057:  stind.ref
      IL_0058:  ldarg.1
      IL_0059:  ldind.ref
      IL_005a:  brfalse.s  IL_0067

      IL_005c:  ldarg.2
      IL_005d:  ldarg.1
      IL_005e:  ldind.ref
      IL_005f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::get_LotCurrent()
      IL_0064:  stind.ref
      IL_0065:  br.s       IL_006a

      IL_0067:  ldarg.2
      IL_0068:  ldnull
      IL_0069:  stind.ref
      IL_006a:  ldarg.1
      IL_006b:  ldind.ref
      IL_006c:  ldnull
      IL_006d:  ceq
      IL_006f:  ldc.i4.0
      IL_0070:  ceq
      IL_0072:  ret
    } // end of method TeleportToPointDestination::PreTeleport

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine/TeleportBase::.ctor()
      IL_0006:  ret
    } // end of method TeleportToPointDestination::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldc.r4     85.
      IL_0005:  stsfld     float32 Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::kChanceOfSuccessWhenTeleportAnywhereIncomplete
      IL_000a:  newobj     instance void Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination/Definition::.ctor()
      IL_000f:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::Singleton
      IL_0014:  ret
    } // end of method TeleportToPointDestination::.cctor

  } // end of class TeleportToPointDestination

  .class auto ansi nested public beforefieldinit Repair
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>
         implements [Sims3GameplaySystems]Sims3.Gameplay.Actors.IOutfitChangeInteraction,
                    [UI]Sims3.UI.Hud.IInteractionInstance
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,class Sims3.Store.Objects.RoutineMachine/Repair>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       44 (0x2c)
        .maxstack  8
        IL_0000:  ldarg.3
        IL_0001:  brfalse.s  IL_0015

        IL_0003:  ldarg.1
        IL_0004:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsPerformingAService()
        IL_0009:  brtrue.s   IL_0015

        IL_000b:  ldarg.1
        IL_000c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsSelectable()
        IL_0011:  brtrue.s   IL_0015

        IL_0013:  ldc.i4.0
        IL_0014:  ret

        IL_0015:  ldarg.2
        IL_0016:  ldarg.1
        IL_0017:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::IsServiceableBySim(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_001c:  brtrue.s   IL_0020

        IL_001e:  ldc.i4.0
        IL_001f:  ret

        IL_0020:  ldarg.2
        IL_0021:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
        IL_0026:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
        IL_002b:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       29 (0x1d)
        .maxstack  5
        .locals init (object[] V_0)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0006:  ldstr      "Repair"
        IL_000b:  ldc.i4.1
        IL_000c:  newarr     [mscorlib]System.Object
        IL_0011:  stloc.0
        IL_0012:  ldloc.0
        IL_0013:  ldc.i4.0
        IL_0014:  ldarg.1
        IL_0015:  stelem.ref
        IL_0016:  ldloc.0
        IL_0017:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
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
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,class Sims3.Store.Objects.RoutineMachine/Repair>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper mSwitchOutfitHelper
    .method public hidebysig virtual instance void 
            ConfigureInteraction() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_000b:  ldarg.0
      IL_000c:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::ConfigureStages(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0011:  ret
    } // end of method Repair::ConfigureInteraction

    .method public hidebysig newslot specialname virtual final 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper 
            get_SwitchOutfitHelper() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/Repair::mSwitchOutfitHelper
      IL_0006:  ret
    } // end of method Repair::get_SwitchOutfitHelper

    .method public hidebysig newslot specialname virtual final 
            instance void  set_SwitchOutfitHelper(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper 'value') cil managed
    {
      // Code size       37 (0x25)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/Repair::mSwitchOutfitHelper
      IL_0006:  ldarg.1
      IL_0007:  bne.un.s   IL_000a

      IL_0009:  ret

      IL_000a:  ldarg.0
      IL_000b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/Repair::mSwitchOutfitHelper
      IL_0010:  brfalse.s  IL_001d

      IL_0012:  ldarg.0
      IL_0013:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/Repair::mSwitchOutfitHelper
      IL_0018:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Dispose()
      IL_001d:  ldarg.0
      IL_001e:  ldarg.1
      IL_001f:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/Repair::mSwitchOutfitHelper
      IL_0024:  ret
    } // end of method Repair::set_SwitchOutfitHelper

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       538 (0x21a)
      .maxstack  5
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent V_0,
               bool V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_000c:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Slot Sims3.Store.Objects.RoutineMachine::kRoutingSlot
      IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlotAndCheckInUse(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_0016:  brtrue.s   IL_0025

      IL_0018:  ldarg.0
      IL_0019:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_001e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayRouteFailure()
      IL_0023:  ldc.i4.0
      IL_0024:  ret

      IL_0025:  ldarg.0
      IL_0026:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_002b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0030:  stloc.0
      IL_0031:  ldloc.0
      IL_0032:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_0037:  brtrue.s   IL_0051

      IL_0039:  ldarg.0
      IL_003a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_003f:  ldarg.0
      IL_0040:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0045:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetThumbnailKey()
      IL_004a:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayRouteFailThoughtBalloon(valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey)
      IL_004f:  ldc.i4.0
      IL_0050:  ret

      IL_0051:  ldarg.0
      IL_0052:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry()
      IL_0057:  ldarg.0
      IL_0058:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_005d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0062:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::DisableLookAts()
      IL_0067:  ldarg.0
      IL_0068:  ldstr      "routinemachine_store"
      IL_006d:  ldstr      "EnterFixMachine"
      IL_0072:  ldstr      "x"
      IL_0077:  ldstr      "routinemachine"
      IL_007c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_0081:  ldloc.0
      IL_0082:  ldarg.0
      IL_0083:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::StartRepair(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0088:  pop
      IL_0089:  ldarg.0
      IL_008a:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_008f:  ldarg.0
      IL_0090:  ldstr      "FixMachine"
      IL_0095:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_009a:  ldarg.0
      IL_009b:  ldc.i4     0xffa9bfff
      IL_00a0:  call       instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_00a5:  stloc.1
      IL_00a6:  ldarg.0
      IL_00a7:  ldloc.1
      IL_00a8:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_00ad:  ldloc.0
      IL_00ae:  ldarg.0
      IL_00af:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00b4:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::StopRepair(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor)
      IL_00b9:  ldloc.0
      IL_00ba:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_00bf:  brtrue.s   IL_00da

      IL_00c1:  ldc.i4.1
      IL_00c2:  stloc.1
      IL_00c3:  ldloc.0
      IL_00c4:  ldarg.0
      IL_00c5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00ca:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::StopRepair(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor)
      IL_00cf:  ldloc.0
      IL_00d0:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::ForceRepaired()
      IL_00d5:  br         IL_01e4

      IL_00da:  ldc.i4.0
      IL_00db:  stloc.1
      IL_00dc:  ldarg.0
      IL_00dd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00e2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_00e7:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsFrankenstein()
      IL_00ec:  brfalse.s  IL_0121

      IL_00ee:  ldarg.0
      IL_00ef:  ldstr      "Shocked"
      IL_00f4:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_00f9:  ldarg.0
      IL_00fa:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00ff:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0104:  ldc.i8     0x6c2a012d6179df90
      IL_010d:  ldc.i8     0x6c2a002d6179dfa0
      IL_0116:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_011b:  pop
      IL_011c:  br         IL_01d3

      IL_0121:  ldarg.0
      IL_0122:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0127:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_012c:  ldc.i8     0x4c800171bd46b80
      IL_0135:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_013a:  brtrue.s   IL_0157

      IL_013c:  ldarg.0
      IL_013d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0142:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_0147:  ldc.i8     0x4c800171bd46b00
      IL_0150:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_0155:  brfalse.s  IL_0196

      IL_0157:  ldarg.0
      IL_0158:  ldc.i8     0x8bfc0021188986d4
      IL_0161:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffSinged::SingeViaInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.IOutfitChangeInteraction,
                                                                                                                  valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_0166:  ldarg.0
      IL_0167:  ldstr      "Shocked"
      IL_016c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0171:  ldarg.0
      IL_0172:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0177:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_017c:  ldc.i8     0x4c800171bd46b80
      IL_0185:  ldc.i8     0x8bfc0021188986d4
      IL_018e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_0193:  pop
      IL_0194:  br.s       IL_01d3

      IL_0196:  ldarg.0
      IL_0197:  ldc.i8     0x8bfc0021188986d4
      IL_01a0:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffSinged::SingeViaInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.IOutfitChangeInteraction,
                                                                                                                  valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_01a5:  ldarg.0
      IL_01a6:  ldstr      "Shocked"
      IL_01ab:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_01b0:  ldarg.0
      IL_01b1:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01b6:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_01bb:  ldc.i8     0x4c800171bd46b80
      IL_01c4:  ldc.i8     0x8bfc0021188986d4
      IL_01cd:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_01d2:  pop
      IL_01d3:  ldarg.0
      IL_01d4:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01d9:  ldarg.0
      IL_01da:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_01df:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Controllers.FireManager::SimShockedBy(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject)
      IL_01e4:  ldarg.0
      IL_01e5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01ea:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_01ef:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_01f4:  ldarg.0
      IL_01f5:  ldstr      "ExitFixMachine"
      IL_01fa:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_01ff:  ldloc.0
      IL_0200:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_0205:  brtrue.s   IL_0212

      IL_0207:  ldarg.0
      IL_0208:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_020d:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::StopBrokenVisualEffects()
      IL_0212:  ldarg.0
      IL_0213:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit()
      IL_0218:  ldloc.1
      IL_0219:  ret
    } // end of method Repair::Run

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  call       instance void Sims3.Store.Objects.RoutineMachine/Repair::set_SwitchOutfitHelper(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper)
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_000d:  ret
    } // end of method Repair::Cleanup

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::.ctor()
      IL_0006:  ret
    } // end of method Repair::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.RoutineMachine/Repair/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/Repair::Singleton
      IL_000a:  ret
    } // end of method Repair::.cctor

    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper
            SwitchOutfitHelper()
    {
      .set instance void Sims3.Store.Objects.RoutineMachine/Repair::set_SwitchOutfitHelper(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper)
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/Repair::get_SwitchOutfitHelper()
    } // end of property Repair::SwitchOutfitHelper
  } // end of class Repair

  .class abstract auto ansi nested public beforefieldinit UpgradeBase
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>
         implements [Sims3GameplaySystems]Sims3.Gameplay.Actors.IOutfitChangeInteraction,
                    [UI]Sims3.UI.Hud.IInteractionInstance
  {
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
              '<BasicTest>b__0'() cil managed
      {
        // Code size       39 (0x27)
        .maxstack  5
        .locals init (object[] V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.RoutineMachine/UpgradeBase/'<>c__DisplayClass2'::a
        IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_000b:  ldstr      "NoUpgradeWhileInUse"
        IL_0010:  ldc.i4.1
        IL_0011:  newarr     [mscorlib]System.Object
        IL_0016:  stloc.0
        IL_0017:  ldloc.0
        IL_0018:  ldc.i4.0
        IL_0019:  ldarg.0
        IL_001a:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.RoutineMachine/UpgradeBase/'<>c__DisplayClass2'::a
        IL_001f:  stelem.ref
        IL_0020:  ldloc.0
        IL_0021:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                       string,
                                                                                       object[])
        IL_0026:  ret
      } // end of method '<>c__DisplayClass2'::'<BasicTest>b__0'

    } // end of class '<>c__DisplayClass2'

    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper mSwitchOutfitHelper
    .method public hidebysig newslot specialname abstract virtual 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning 
            get_TuningParams() cil managed
    {
    } // end of method UpgradeBase::get_TuningParams

    .method public hidebysig newslot specialname abstract virtual 
            instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade 
            get_UpgradeFeature() cil managed
    {
    } // end of method UpgradeBase::get_UpgradeFeature

    .method public hidebysig newslot specialname abstract virtual 
            instance bool  get_IsUpgraded() cil managed
    {
    } // end of method UpgradeBase::get_IsUpgraded

    .method public hidebysig static string 
            GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade upgrade) cil managed
    {
      // Code size       79 (0x4f)
      .maxstack  3
      .locals init (valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  ldc.i4.1
      IL_0004:  beq.s      IL_0036

      IL_0006:  ldloc.0
      IL_0007:  ldc.i4.s   13
      IL_0009:  beq.s      IL_0012

      IL_000b:  ldloc.0
      IL_000c:  ldc.i4.s   17
      IL_000e:  beq.s      IL_0024

      IL_0010:  br.s       IL_0048

      IL_0012:  ldc.i4.0
      IL_0013:  ldstr      "UpgradeSpeakers"
      IL_0018:  ldc.i4.0
      IL_0019:  newarr     [mscorlib]System.Object
      IL_001e:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
      IL_0023:  ret

      IL_0024:  ldc.i4.0
      IL_0025:  ldstr      "UpgradeTeleport"
      IL_002a:  ldc.i4.0
      IL_002b:  newarr     [mscorlib]System.Object
      IL_0030:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ldstr      "Unbreakable"
      IL_003c:  ldc.i4.0
      IL_003d:  newarr     [mscorlib]System.Object
      IL_0042:  call       string Sims3.Store.Objects.RoutineMachine::LocalizeString(bool,
                                                                                     string,
                                                                                     object[])
      IL_0047:  ret

      IL_0048:  ldarg.0
      IL_0049:  call       string [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_004e:  ret
    } // end of method UpgradeBase::GetUpgradeTypeString

    .method public hidebysig static string 
            GetInteractionDefinitionNameHelper(class Sims3.Store.Objects.RoutineMachine target,
                                               valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade newfeature) cil managed
    {
      // Code size       204 (0xcc)
      .maxstack  4
      .locals init (int32 V_0,
               object[] V_1,
               object[] V_2,
               object[] V_3,
               object[] V_4)
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::PercentComplete(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_000c:  stloc.0
      IL_000d:  ldarg.0
      IL_000e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_0013:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0018:  brfalse.s  IL_0084

      IL_001a:  ldloc.0
      IL_001b:  brtrue.s   IL_004c

      IL_001d:  ldstr      "Gameplay/ObjectComponents/Upgrade:ReplaceZero"
      IL_0022:  ldc.i4.2
      IL_0023:  newarr     [mscorlib]System.Object
      IL_0028:  stloc.1
      IL_0029:  ldloc.1
      IL_002a:  ldc.i4.0
      IL_002b:  ldarg.0
      IL_002c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_0031:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0036:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_003b:  stelem.ref
      IL_003c:  ldloc.1
      IL_003d:  ldc.i4.1
      IL_003e:  ldarg.1
      IL_003f:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_0044:  stelem.ref
      IL_0045:  ldloc.1
      IL_0046:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                              object[])
      IL_004b:  ret

      IL_004c:  ldstr      "Gameplay/ObjectComponents/Upgrade:Replace"
      IL_0051:  ldc.i4.3
      IL_0052:  newarr     [mscorlib]System.Object
      IL_0057:  stloc.2
      IL_0058:  ldloc.2
      IL_0059:  ldc.i4.0
      IL_005a:  ldarg.0
      IL_005b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_0060:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0065:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_006a:  stelem.ref
      IL_006b:  ldloc.2
      IL_006c:  ldc.i4.1
      IL_006d:  ldarg.1
      IL_006e:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_0073:  stelem.ref
      IL_0074:  ldloc.2
      IL_0075:  ldc.i4.2
      IL_0076:  ldloc.0
      IL_0077:  box        [mscorlib]System.Int32
      IL_007c:  stelem.ref
      IL_007d:  ldloc.2
      IL_007e:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                              object[])
      IL_0083:  ret

      IL_0084:  ldloc.0
      IL_0085:  brtrue.s   IL_00a3

      IL_0087:  ldstr      "Gameplay/ObjectComponents/Upgrade:CompletePercentZ"
      + "ero"
      IL_008c:  ldc.i4.1
      IL_008d:  newarr     [mscorlib]System.Object
      IL_0092:  stloc.3
      IL_0093:  ldloc.3
      IL_0094:  ldc.i4.0
      IL_0095:  ldarg.1
      IL_0096:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_009b:  stelem.ref
      IL_009c:  ldloc.3
      IL_009d:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                              object[])
      IL_00a2:  ret

      IL_00a3:  ldstr      "Gameplay/ObjectComponents/Upgrade:CompletePercent"
      IL_00a8:  ldc.i4.2
      IL_00a9:  newarr     [mscorlib]System.Object
      IL_00ae:  stloc.s    V_4
      IL_00b0:  ldloc.s    V_4
      IL_00b2:  ldc.i4.0
      IL_00b3:  ldarg.1
      IL_00b4:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_00b9:  stelem.ref
      IL_00ba:  ldloc.s    V_4
      IL_00bc:  ldc.i4.1
      IL_00bd:  ldloc.0
      IL_00be:  box        [mscorlib]System.Int32
      IL_00c3:  stelem.ref
      IL_00c4:  ldloc.s    V_4
      IL_00c6:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                              object[])
      IL_00cb:  ret
    } // end of method UpgradeBase::GetInteractionDefinitionNameHelper

    .method public hidebysig static bool 
            BasicTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                      class Sims3.Store.Objects.RoutineMachine target,
                      bool isAutonomous,
                      class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback,
                      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade newfeature) cil managed
    {
      // Code size       82 (0x52)
      .maxstack  4
      .locals init (class [UI]Sims3.SimIFace.GreyedOutTooltipCallback V_0,
               class Sims3.Store.Objects.RoutineMachine/UpgradeBase/'<>c__DisplayClass2' V_1)
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  newobj     instance void Sims3.Store.Objects.RoutineMachine/UpgradeBase/'<>c__DisplayClass2'::.ctor()
      IL_0007:  stloc.1
      IL_0008:  ldloc.1
      IL_0009:  ldarg.0
      IL_000a:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.RoutineMachine/UpgradeBase/'<>c__DisplayClass2'::a
      IL_000f:  ldarg.1
      IL_0010:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0015:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
      IL_001a:  brtrue.s   IL_002b

      IL_001c:  ldarg.1
      IL_001d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_0022:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0027:  ldarg.s    newfeature
      IL_0029:  bne.un.s   IL_002d

      IL_002b:  ldc.i4.0
      IL_002c:  ret

      IL_002d:  ldarg.1
      IL_002e:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
      IL_0033:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Count()
      IL_0038:  ldc.i4.0
      IL_0039:  ble.s      IL_0050

      IL_003b:  ldarg.3
      IL_003c:  ldloc.0
      IL_003d:  brtrue.s   IL_004c

      IL_003f:  ldloc.1
      IL_0040:  ldftn      instance string Sims3.Store.Objects.RoutineMachine/UpgradeBase/'<>c__DisplayClass2'::'<BasicTest>b__0'()
      IL_0046:  newobj     instance void [UI]Sims3.SimIFace.GreyedOutTooltipCallback::.ctor(object,
                                                                                            native int)
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stind.ref
      IL_004e:  ldc.i4.0
      IL_004f:  ret

      IL_0050:  ldc.i4.1
      IL_0051:  ret
    } // end of method UpgradeBase::BasicTest

    .method public hidebysig virtual instance string 
            GetInteractionName() cil managed
    {
      // Code size       108 (0x6c)
      .maxstack  4
      .locals init (object[] V_0,
               object[] V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_000b:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0010:  brfalse.s  IL_004b

      IL_0012:  ldstr      "Gameplay/ObjectComponents/Upgrade:ReplaceZero"
      IL_0017:  ldc.i4.2
      IL_0018:  newarr     [mscorlib]System.Object
      IL_001d:  stloc.0
      IL_001e:  ldloc.0
      IL_001f:  ldc.i4.0
      IL_0020:  ldarg.0
      IL_0021:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0026:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_002b:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
      IL_0030:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_0035:  stelem.ref
      IL_0036:  ldloc.0
      IL_0037:  ldc.i4.1
      IL_0038:  ldarg.0
      IL_0039:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_UpgradeFeature()
      IL_003e:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_0043:  stelem.ref
      IL_0044:  ldloc.0
      IL_0045:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                              object[])
      IL_004a:  ret

      IL_004b:  ldstr      "Gameplay/ObjectComponents/Upgrade:CompletePercentZ"
      + "ero"
      IL_0050:  ldc.i4.1
      IL_0051:  newarr     [mscorlib]System.Object
      IL_0056:  stloc.1
      IL_0057:  ldloc.1
      IL_0058:  ldc.i4.0
      IL_0059:  ldarg.0
      IL_005a:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_UpgradeFeature()
      IL_005f:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetUpgradeTypeString(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_0064:  stelem.ref
      IL_0065:  ldloc.1
      IL_0066:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                              object[])
      IL_006b:  ret
    } // end of method UpgradeBase::GetInteractionName

    .method public hidebysig newslot specialname virtual final 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper 
            get_SwitchOutfitHelper() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/UpgradeBase::mSwitchOutfitHelper
      IL_0006:  ret
    } // end of method UpgradeBase::get_SwitchOutfitHelper

    .method public hidebysig newslot specialname virtual final 
            instance void  set_SwitchOutfitHelper(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper 'value') cil managed
    {
      // Code size       37 (0x25)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/UpgradeBase::mSwitchOutfitHelper
      IL_0006:  ldarg.1
      IL_0007:  bne.un.s   IL_000a

      IL_0009:  ret

      IL_000a:  ldarg.0
      IL_000b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/UpgradeBase::mSwitchOutfitHelper
      IL_0010:  brfalse.s  IL_001d

      IL_0012:  ldarg.0
      IL_0013:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/UpgradeBase::mSwitchOutfitHelper
      IL_0018:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Dispose()
      IL_001d:  ldarg.0
      IL_001e:  ldarg.1
      IL_001f:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/UpgradeBase::mSwitchOutfitHelper
      IL_0024:  ret
    } // end of method UpgradeBase::set_SwitchOutfitHelper

    .method public hidebysig virtual instance void 
            ConfigureInteraction() cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_000b:  ldarg.0
      IL_000c:  ldarg.0
      IL_000d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_TuningParams()
      IL_0012:  ldarg.0
      IL_0013:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_UpgradeFeature()
      IL_0018:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::ConfigureStages(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                    class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning,
                                                                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
      IL_001d:  ret
    } // end of method UpgradeBase::ConfigureInteraction

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       502 (0x1f6)
      .maxstack  5
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent V_0,
               bool V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_000c:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Slot Sims3.Store.Objects.RoutineMachine::kRoutingSlot
      IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::RouteToSlotAndCheckInUse(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_0016:  brtrue.s   IL_0025

      IL_0018:  ldarg.0
      IL_0019:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_001e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayRouteFailure()
      IL_0023:  ldc.i4.0
      IL_0024:  ret

      IL_0025:  ldarg.0
      IL_0026:  callvirt   instance bool Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_IsUpgraded()
      IL_002b:  brfalse.s  IL_002f

      IL_002d:  ldc.i4.0
      IL_002e:  ret

      IL_002f:  ldarg.0
      IL_0030:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry()
      IL_0035:  ldarg.0
      IL_0036:  ldstr      "routinemachine_store"
      IL_003b:  ldstr      "EnterFixMachine"
      IL_0040:  ldstr      "x"
      IL_0045:  ldstr      "routinemachine"
      IL_004a:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EnterStateMachine(string,
                                                                                                                                  string,
                                                                                                                                  string,
                                                                                                                                  string)
      IL_004f:  ldarg.0
      IL_0050:  ldstr      "FixMachine"
      IL_0055:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_005a:  ldarg.0
      IL_005b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0060:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_UpgradableComponent()
      IL_0065:  stloc.0
      IL_0066:  ldloc.0
      IL_0067:  ldarg.0
      IL_0068:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_UpgradeFeature()
      IL_006d:  ldarg.0
      IL_006e:  ldarg.0
      IL_006f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_TuningParams()
      IL_0074:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradeResult [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::StartUpgrade(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade,
                                                                                                                                                                                                          class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                                                                                          class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning)
      IL_0079:  pop
      IL_007a:  ldarg.0
      IL_007b:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_0080:  ldarg.0
      IL_0081:  ldc.i4     0xffa9bfff
      IL_0086:  call       instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_008b:  stloc.1
      IL_008c:  ldarg.0
      IL_008d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0092:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_0097:  ldarg.0
      IL_0098:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_009d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::UpdateBreakage(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_00a2:  brfalse.s  IL_00a6

      IL_00a4:  ldc.i4.0
      IL_00a5:  stloc.1
      IL_00a6:  ldarg.0
      IL_00a7:  ldloc.1
      IL_00a8:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_00ad:  ldarg.0
      IL_00ae:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_00b3:  callvirt   instance void Sims3.Store.Objects.RoutineMachine::UpdateGeometry()
      IL_00b8:  ldloc.0
      IL_00b9:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradeResult [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::UpgradeResult
      IL_00be:  brfalse    IL_01d5

      IL_00c3:  ldloc.0
      IL_00c4:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradeResult [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::UpgradeResult
      IL_00c9:  ldc.i4.4
      IL_00ca:  bne.un     IL_01bc

      IL_00cf:  ldarg.0
      IL_00d0:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00d5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
      IL_00da:  ldc.i4.s   11
      IL_00dc:  conv.i8
      IL_00dd:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
      IL_00e2:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness
      IL_00e7:  stloc.2
      IL_00e8:  ldloc.2
      IL_00e9:  brfalse.s  IL_00f7

      IL_00eb:  ldloc.2
      IL_00ec:  ldarg.0
      IL_00ed:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_00f2:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness::SimShockedByUpgrade(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject)
      IL_00f7:  ldarg.0
      IL_00f8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_00fd:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_0102:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_IsFrankenstein()
      IL_0107:  brfalse.s  IL_0139

      IL_0109:  ldarg.0
      IL_010a:  ldstr      "Shocked"
      IL_010f:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_0114:  ldarg.0
      IL_0115:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_011a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_011f:  ldc.i8     0x6c2a012d6179df90
      IL_0128:  ldc.i8     0x6c2a002d6179dfa0
      IL_0131:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_0136:  pop
      IL_0137:  br.s       IL_01a9

      IL_0139:  ldarg.0
      IL_013a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_013f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0144:  ldc.i8     0x4c800171bd46b80
      IL_014d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0152:  brtrue.s   IL_016f

      IL_0154:  ldarg.0
      IL_0155:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_015a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_015f:  ldc.i8     0x4c800171bd46b00
      IL_0168:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_016d:  brfalse.s  IL_018f

      IL_016f:  ldarg.0
      IL_0170:  ldstr      "Shocked"
      IL_0175:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_017a:  ldarg.0
      IL_017b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_0180:  ldc.i4.4
      IL_0181:  ldarg.0
      IL_0182:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0187:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::Kill(valuetype [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription/DeathType,
                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject)
      IL_018c:  pop
      IL_018d:  br.s       IL_01a9

      IL_018f:  ldarg.0
      IL_0190:  ldstr      "Shocked"
      IL_0195:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_019a:  ldarg.0
      IL_019b:  ldc.i8     0x8bfc002118898758
      IL_01a4:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffSinged::SingeViaInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.IOutfitChangeInteraction,
                                                                                                                  valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_01a9:  ldarg.0
      IL_01aa:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Actor
      IL_01af:  ldarg.0
      IL_01b0:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_01b5:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Controllers.FireManager::SimShockedBy(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject)
      IL_01ba:  br.s       IL_01d5

      IL_01bc:  ldloc.0
      IL_01bd:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradeResult [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::UpgradeResult
      IL_01c2:  ldc.i4.1
      IL_01c3:  bne.un.s   IL_01d5

      IL_01c5:  ldarg.0
      IL_01c6:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_01cb:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
      IL_01d0:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::BreakObject()
      IL_01d5:  ldarg.0
      IL_01d6:  ldstr      "ExitFixMachine"
      IL_01db:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_01e0:  ldloc.0
      IL_01e1:  ldarg.0
      IL_01e2:  ldarg.0
      IL_01e3:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_TuningParams()
      IL_01e8:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradeResult [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::StopUpgrade(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                                                                                         class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning)
      IL_01ed:  pop
      IL_01ee:  ldarg.0
      IL_01ef:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit()
      IL_01f4:  ldloc.1
      IL_01f5:  ret
    } // end of method UpgradeBase::Run

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  call       instance void Sims3.Store.Objects.RoutineMachine/UpgradeBase::set_SwitchOutfitHelper(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper)
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_000d:  ret
    } // end of method UpgradeBase::Cleanup

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::.ctor()
      IL_0006:  ret
    } // end of method UpgradeBase::.ctor

    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning
            TuningParams()
    {
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_TuningParams()
    } // end of property UpgradeBase::TuningParams
    .property instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade
            UpgradeFeature()
    {
      .get instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_UpgradeFeature()
    } // end of property UpgradeBase::UpgradeFeature
    .property instance bool IsUpgraded()
    {
      .get instance bool Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_IsUpgraded()
    } // end of property UpgradeBase::IsUpgraded
    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper
            SwitchOutfitHelper()
    {
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/UpgradeBase::get_SwitchOutfitHelper()
      .set instance void Sims3.Store.Objects.RoutineMachine/UpgradeBase::set_SwitchOutfitHelper(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper)
    } // end of property UpgradeBase::SwitchOutfitHelper
  } // end of class UpgradeBase

  .class auto ansi nested public beforefieldinit UpgradeSpeakers
         extends Sims3.Store.Objects.RoutineMachine/UpgradeBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,class Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers>
    {
      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       9 (0x9)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  ldc.i4.s   13
        IL_0003:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetInteractionDefinitionNameHelper(class Sims3.Store.Objects.RoutineMachine,
                                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
        IL_0008:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       13 (0xd)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.2
        IL_0002:  ldarg.3
        IL_0003:  ldarg.s    greyedOutTooltipCallback
        IL_0005:  ldc.i4.s   13
        IL_0007:  call       bool Sims3.Store.Objects.RoutineMachine/UpgradeBase::BasicTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                            class Sims3.Store.Objects.RoutineMachine,
                                                                                            bool,
                                                                                            class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&,
                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
        IL_000c:  ret
      } // end of method Definition::Test

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,class Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning kAddSpeakersTuning
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 17 53 70 65 61 6B 65 72 73 20 75 70 67 72   // ...Speakers upgr
                                                                                              61 64 65 20 74 75 6E 69 6E 67 00 00 )             // ade tuning..
    .method public hidebysig specialname virtual 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning 
            get_TuningParams() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers::kAddSpeakersTuning
      IL_0005:  ret
    } // end of method UpgradeSpeakers::get_TuningParams

    .method public hidebysig specialname virtual 
            instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade 
            get_UpgradeFeature() cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      IL_0000:  ldc.i4.s   13
      IL_0002:  ret
    } // end of method UpgradeSpeakers::get_UpgradeFeature

    .method public hidebysig specialname virtual 
            instance bool  get_IsUpgraded() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_000b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_SoupUpSpeakers()
      IL_0010:  ret
    } // end of method UpgradeSpeakers::get_IsUpgraded

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine/UpgradeBase::.ctor()
      IL_0006:  ret
    } // end of method UpgradeSpeakers::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers::Singleton
      IL_000a:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning::.ctor()
      IL_000f:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers::kAddSpeakersTuning
      IL_0014:  ret
    } // end of method UpgradeSpeakers::.cctor

    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning
            TuningParams()
    {
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers::get_TuningParams()
    } // end of property UpgradeSpeakers::TuningParams
    .property instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade
            UpgradeFeature()
    {
      .get instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers::get_UpgradeFeature()
    } // end of property UpgradeSpeakers::UpgradeFeature
    .property instance bool IsUpgraded()
    {
      .get instance bool Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers::get_IsUpgraded()
    } // end of property UpgradeSpeakers::IsUpgraded
  } // end of class UpgradeSpeakers

  .class auto ansi nested public beforefieldinit UpgradeTransporter
         extends Sims3.Store.Objects.RoutineMachine/UpgradeBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,class Sims3.Store.Objects.RoutineMachine/UpgradeTransporter>
    {
      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       9 (0x9)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  ldc.i4.s   17
        IL_0003:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetInteractionDefinitionNameHelper(class Sims3.Store.Objects.RoutineMachine,
                                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
        IL_0008:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       13 (0xd)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.2
        IL_0002:  ldarg.3
        IL_0003:  ldarg.s    greyedOutTooltipCallback
        IL_0005:  ldc.i4.s   17
        IL_0007:  call       bool Sims3.Store.Objects.RoutineMachine/UpgradeBase::BasicTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                            class Sims3.Store.Objects.RoutineMachine,
                                                                                            bool,
                                                                                            class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&,
                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
        IL_000c:  ret
      } // end of method Definition::Test

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,class Sims3.Store.Objects.RoutineMachine/UpgradeTransporter>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning kUpgradeTransporterTuning
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 22 55 70 67 72 61 64 65 20 54 72 61 6E 73   // .."Upgrade Trans
                                                                                              70 6F 72 74 65 72 20 75 70 67 72 61 64 65 20 74   // porter upgrade t
                                                                                              75 6E 69 6E 67 00 00 )                            // uning..
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname virtual 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning 
            get_TuningParams() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeTransporter::kUpgradeTransporterTuning
      IL_0005:  ret
    } // end of method UpgradeTransporter::get_TuningParams

    .method public hidebysig specialname virtual 
            instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade 
            get_UpgradeFeature() cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      IL_0000:  ldc.i4.s   17
      IL_0002:  ret
    } // end of method UpgradeTransporter::get_UpgradeFeature

    .method public hidebysig specialname virtual 
            instance bool  get_IsUpgraded() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_000b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_PerfectTeleportation()
      IL_0010:  ret
    } // end of method UpgradeTransporter::get_IsUpgraded

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine/UpgradeBase::.ctor()
      IL_0006:  ret
    } // end of method UpgradeTransporter::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.RoutineMachine/UpgradeTransporter/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/UpgradeTransporter::Singleton
      IL_000a:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning::.ctor()
      IL_000f:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeTransporter::kUpgradeTransporterTuning
      IL_0014:  ret
    } // end of method UpgradeTransporter::.cctor

    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning
            TuningParams()
    {
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradeTransporter::get_TuningParams()
    } // end of property UpgradeTransporter::TuningParams
    .property instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade
            UpgradeFeature()
    {
      .get instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade Sims3.Store.Objects.RoutineMachine/UpgradeTransporter::get_UpgradeFeature()
    } // end of property UpgradeTransporter::UpgradeFeature
    .property instance bool IsUpgraded()
    {
      .get instance bool Sims3.Store.Objects.RoutineMachine/UpgradeTransporter::get_IsUpgraded()
    } // end of property UpgradeTransporter::IsUpgraded
  } // end of class UpgradeTransporter

  .class auto ansi nested public beforefieldinit UpgradePreventBreakage
         extends Sims3.Store.Objects.RoutineMachine/UpgradeBase
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,class Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage>
    {
      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                                 class Sims3.Store.Objects.RoutineMachine target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  ldc.i4.1
        IL_0002:  call       string Sims3.Store.Objects.RoutineMachine/UpgradeBase::GetInteractionDefinitionNameHelper(class Sims3.Store.Objects.RoutineMachine,
                                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
        IL_0007:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.RoutineMachine target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.2
        IL_0002:  ldarg.3
        IL_0003:  ldarg.s    greyedOutTooltipCallback
        IL_0005:  ldc.i4.1
        IL_0006:  call       bool Sims3.Store.Objects.RoutineMachine/UpgradeBase::BasicTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                            class Sims3.Store.Objects.RoutineMachine,
                                                                                            bool,
                                                                                            class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&,
                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
        IL_000b:  ret
      } // end of method Definition::Test

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine,class Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning kUpgradeUnbreakableTuning
    .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 1A 55 6E 62 72 65 61 6B 61 62 6C 65 20 75   // ...Unbreakable u
                                                                                              70 67 72 61 64 65 20 74 75 6E 69 6E 67 00 00 )    // pgrade tuning..
    .method public hidebysig specialname virtual 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning 
            get_TuningParams() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage::kUpgradeUnbreakableTuning
      IL_0005:  ret
    } // end of method UpgradePreventBreakage::get_TuningParams

    .method public hidebysig specialname virtual 
            instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade 
            get_UpgradeFeature() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } // end of method UpgradePreventBreakage::get_UpgradeFeature

    .method public hidebysig specialname virtual 
            instance bool  get_IsUpgraded() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.RoutineMachine>::Target
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
      IL_000b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_Unbreakable()
      IL_0010:  ret
    } // end of method UpgradePreventBreakage::get_IsUpgraded

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine/UpgradeBase::.ctor()
      IL_0006:  ret
    } // end of method UpgradePreventBreakage::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage::Singleton
      IL_000a:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning::.ctor()
      IL_000f:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage::kUpgradeUnbreakableTuning
      IL_0014:  ret
    } // end of method UpgradePreventBreakage::.cctor

    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning
            TuningParams()
    {
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Handiness/UpgradeTuning Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage::get_TuningParams()
    } // end of property UpgradePreventBreakage::TuningParams
    .property instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade
            UpgradeFeature()
    {
      .get instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage::get_UpgradeFeature()
    } // end of property UpgradePreventBreakage::UpgradeFeature
    .property instance bool IsUpgraded()
    {
      .get instance bool Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage::get_IsUpgraded()
    } // end of property UpgradePreventBreakage::IsUpgraded
  } // end of class UpgradePreventBreakage

  .field public static literal int32 mNumSpeakers = int32(0x00000004)
  .field public static string sLocalizationKey
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mVisualEffectBroken
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mVisualEffectIndicator
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mVisualEffectPortalOut
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mVisualEffectPortalTeleport
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect mVisualEffectDreamscape
  .field public class [SimIFace]Sims3.SimIFace.ObjectSound mDreamscapeMusicHandle
  .field public class [SimIFace]Sims3.SimIFace.ObjectSound mShowerAudioHandle
  .field public class [SimIFace]Sims3.SimIFace.VisualEffect[] mVisualEffectSpeakerUpgrade
  .field public class [SimIFace]Sims3.SimIFace.ObjectSound mSpeakerUpgradeMusicHandle
  .field public static valuetype [SimIFace]Sims3.SimIFace.Slot kRoutingSlot
  .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Controllers.PuddleManager/FlowingPuddleTuning kPuddlesTuning
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 31 46 6C 6F 77 69 6E 67 20 70 75 64 64 6C   // ..1Flowing puddl
                                                                                            65 20 74 75 6E 69 6E 67 20 28 61 6C 73 6F 20 75   // e tuning (also u
                                                                                            73 65 64 20 66 6F 72 20 77 68 65 6E 20 62 72 6F   // sed for when bro
                                                                                            6B 65 6E 29 00 00 )                               // ken)..
  .field public static class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning kRepairableComponentTuning
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 11 52 65 70 61 72 61 69 62 6C 65 20 74 75   // ...Reparaible tu
                                                                                            6E 69 6E 67 00 00 )                               // ning..
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
    IL_0001:  ldsfld     string Sims3.Store.Objects.RoutineMachine::sLocalizationKey
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
  } // end of method RoutineMachine::LocalizeString

  .method public hidebysig specialname instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning 
          get_RepairableTuning() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning Sims3.Store.Objects.RoutineMachine::kRepairableComponentTuning
    IL_0005:  ret
  } // end of method RoutineMachine::get_RepairableTuning

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.4
    IL_0002:  newarr     [SimIFace]Sims3.SimIFace.VisualEffect
    IL_0007:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_000c:  ldarg.0
    IL_000d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::.ctor()
    IL_0012:  ret
  } // end of method RoutineMachine::.ctor

  .method public hidebysig virtual instance void 
          OnCreation() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::OnCreation()
    IL_0006:  ret
  } // end of method RoutineMachine::OnCreation

  .method public hidebysig virtual instance void 
          OnStartup() cil managed
  {
    // Code size       190 (0xbe)
    .maxstack  6
    .locals init (class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade> V_0,
             object[] V_1,
             object[] V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     [mscorlib]System.Object
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.2
    IL_000b:  box        [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/RepairableType
    IL_0010:  stelem.ref
    IL_0011:  ldloc.1
    IL_0012:  ldc.i4.1
    IL_0013:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning Sims3.Store.Objects.RoutineMachine::kRepairableComponentTuning
    IL_0018:  stelem.ref
    IL_0019:  ldloc.1
    IL_001a:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class Sims3.Store.Objects.RoutineMachine/RoutineMachineRepairableComponent>(object[])
    IL_001f:  pop
    IL_0020:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade>::.ctor()
    IL_0025:  stloc.0
    IL_0026:  ldloc.0
    IL_0027:  ldc.i4.1
    IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade>::Add(!0)
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.s   13
    IL_0030:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade>::Add(!0)
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.s   17
    IL_0038:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade>::Add(!0)
    IL_003d:  ldarg.0
    IL_003e:  ldc.i4.2
    IL_003f:  newarr     [mscorlib]System.Object
    IL_0044:  stloc.2
    IL_0045:  ldloc.2
    IL_0046:  ldc.i4.0
    IL_0047:  ldarg.0
    IL_0048:  ldftn      instance void Sims3.Store.Objects.RoutineMachine::UpgradeChangedCallback(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade,
                                                                                                  valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade)
    IL_004e:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent/UpgradeChanged::.ctor(object,
                                                                                                                                       native int)
    IL_0053:  stelem.ref
    IL_0054:  ldloc.2
    IL_0055:  ldc.i4.1
    IL_0056:  ldloc.0
    IL_0057:  stelem.ref
    IL_0058:  ldloc.2
    IL_0059:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent>(object[])
    IL_005e:  pop
    IL_005f:  ldarg.0
    IL_0060:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToWorkOrSchool::Singleton
    IL_0065:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_006a:  ldarg.0
    IL_006b:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToShop::Singleton
    IL_0070:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0075:  ldarg.0
    IL_0076:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToCommunityLot::Singleton
    IL_007b:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0080:  ldarg.0
    IL_0081:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToGym::Singleton
    IL_0086:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_008b:  ldarg.0
    IL_008c:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/Repair::Singleton
    IL_0091:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0096:  ldarg.0
    IL_0097:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/UpgradePreventBreakage::Singleton
    IL_009c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00a1:  ldarg.0
    IL_00a2:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/UpgradeSpeakers::Singleton
    IL_00a7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00ac:  ldarg.0
    IL_00ad:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/UpgradeTransporter::Singleton
    IL_00b2:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00b7:  ldarg.0
    IL_00b8:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateGeometry()
    IL_00bd:  ret
  } // end of method RoutineMachine::OnStartup

  .method public hidebysig instance void 
          UpdateGeometry() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Upgradable()
    IL_0006:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.UpgradableComponent::get_CurrentUpgrade()
    IL_000b:  ldc.i4.s   13
    IL_000d:  bne.un.s   IL_001b

    IL_000f:  ldarg.0
    IL_0010:  ldstr      "Upgrade"
    IL_0015:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetGeometryState(string)
    IL_001a:  ret

    IL_001b:  ldarg.0
    IL_001c:  ldstr      "Default"
    IL_0021:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::SetGeometryState(string)
    IL_0026:  ret
  } // end of method RoutineMachine::UpdateGeometry

  .method public hidebysig instance void 
          UpgradeChangedCallback(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade oldUpgrade,
                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.Upgrade newUpgrade) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateGeometry()
    IL_0006:  ret
  } // end of method RoutineMachine::UpgradeChangedCallback

  .method public hidebysig virtual instance void 
          Dispose() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine::StopPortalTeleportVisualEffects()
    IL_0006:  ldarg.0
    IL_0007:  call       instance void Sims3.Store.Objects.RoutineMachine::StopDreamscapeEffects()
    IL_000c:  ldarg.0
    IL_000d:  call       instance void Sims3.Store.Objects.RoutineMachine::StopShowerAudio()
    IL_0012:  ldarg.0
    IL_0013:  call       instance void Sims3.Store.Objects.RoutineMachine::StopSpeakerEffects()
    IL_0018:  ldarg.0
    IL_0019:  call       instance void Sims3.Store.Objects.RoutineMachine::StopPortalReturnVisualEffects()
    IL_001e:  ldarg.0
    IL_001f:  call       instance void Sims3.Store.Objects.RoutineMachine::StopBrokenVisualEffects()
    IL_0024:  ldarg.0
    IL_0025:  call       instance void Sims3.Store.Objects.RoutineMachine::StopIndicatorVisualEffects()
    IL_002a:  ldarg.0
    IL_002b:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::Dispose()
    IL_0030:  ret
  } // end of method RoutineMachine::Dispose

  .method public hidebysig newslot virtual final 
          instance void  SwitchToNakedClothing(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (class Sims3.Store.Objects.RoutineMachine/TeleportBase V_0)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CurrentInteraction()
    IL_0006:  isinst     Sims3.Store.Objects.RoutineMachine/TeleportBase
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brfalse.s  IL_002c

    IL_000f:  ldloc.0
    IL_0010:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::OutfitHelper
    IL_0015:  ldc.i4.0
    IL_0016:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::Wait(bool)
    IL_001b:  ldloc.0
    IL_001c:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper Sims3.Store.Objects.RoutineMachine/TeleportBase::OutfitHelper
    IL_0021:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/SwitchOutfitHelper::ChangeOutfit()
    IL_0026:  ldarg.1
    IL_0027:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::LoopIdle()
    IL_002c:  ret
  } // end of method RoutineMachine::SwitchToNakedClothing

  .method public hidebysig newslot virtual final 
          instance void  SwitchBackToPreviousClothing(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s   83
    IL_0003:  ldc.i4.2
    IL_0004:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                     valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
    IL_0009:  pop
    IL_000a:  ldarg.1
    IL_000b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::LoopIdle()
    IL_0010:  ldarg.1
    IL_0011:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::SwitchToPreviousOutfitWithSpin()
    IL_0016:  ret
  } // end of method RoutineMachine::SwitchBackToPreviousClothing

  .method public hidebysig newslot virtual final 
          instance void  TeleportMeToDestination(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                                 valuetype [SimIFace]Sims3.SimIFace.Vector3 destination) cil managed
  {
    // Code size       65 (0x41)
    .maxstack  6
    .locals init (class Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination V_0)
    IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::Singleton
    IL_0005:  ldarg.0
    IL_0006:  ldarg.1
    IL_0007:  ldarg.1
    IL_0008:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
    IL_000d:  ldc.i4.0
    IL_000e:  ldc.i4.1
    IL_000f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                      bool,
                                                                                                                                                                                                      bool)
    IL_0014:  isinst     Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.2
    IL_001c:  stfld      valuetype [SimIFace]Sims3.SimIFace.Vector3 Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::Destination
    IL_0021:  ldloc.0
    IL_0022:  ldloc.0
    IL_0023:  ldfld      valuetype [SimIFace]Sims3.SimIFace.Vector3 Sims3.Store.Objects.RoutineMachine/TeleportToPointDestination::Destination
    IL_0028:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Core.LotManager::GetLotAtPoint(valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_002d:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot Sims3.Store.Objects.RoutineMachine/TeleportBase::mTargetLot
    IL_0032:  ldarg.1
    IL_0033:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.1
    IL_003a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                              bool)
    IL_003f:  pop
    IL_0040:  ret
  } // end of method RoutineMachine::TeleportMeToDestination

  .method public hidebysig instance void 
          StartPortalTeleportVisualEffects(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalTeleport
    IL_0006:  pop
    IL_0007:  ldarg.0
    IL_0008:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_000d:  ret
  } // end of method RoutineMachine::StartPortalTeleportVisualEffects

  .method public hidebysig instance void 
          StopPortalTeleportVisualEffects() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalTeleport
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalTeleport
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalTeleport
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalTeleport
    IL_0026:  ldarg.0
    IL_0027:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_002c:  ret
  } // end of method RoutineMachine::StopPortalTeleportVisualEffects

  .method public hidebysig instance void 
          StartDreamscapeEffects() cil managed
  {
    // Code size       103 (0x67)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectDreamscape
    IL_0006:  brtrue.s   IL_0036

    IL_0008:  ldarg.0
    IL_0009:  ldstr      "store_routineMachine_glitch"
    IL_000e:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
    IL_0013:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectDreamscape
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectDreamscape
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4     0x318a282c
    IL_0024:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0029:  pop
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectDreamscape
    IL_0030:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_0035:  pop
    IL_0036:  ldarg.0
    IL_0037:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mDreamscapeMusicHandle
    IL_003c:  brtrue.s   IL_0060

    IL_003e:  ldarg.0
    IL_003f:  ldarg.0
    IL_0040:  call       instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
    IL_0045:  ldstr      "rmachine_dreamscape"
    IL_004a:  newobj     instance void [SimIFace]Sims3.SimIFace.ObjectSound::.ctor(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                   string)
    IL_004f:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mDreamscapeMusicHandle
    IL_0054:  ldarg.0
    IL_0055:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mDreamscapeMusicHandle
    IL_005a:  callvirt   instance bool [SimIFace]Sims3.SimIFace.Sound::StartLoop()
    IL_005f:  pop
    IL_0060:  ldarg.0
    IL_0061:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_0066:  ret
  } // end of method RoutineMachine::StartDreamscapeEffects

  .method public hidebysig instance void 
          StopDreamscapeEffects() cil managed
  {
    // Code size       82 (0x52)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectDreamscape
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectDreamscape
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectDreamscape
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectDreamscape
    IL_0026:  ldarg.0
    IL_0027:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mDreamscapeMusicHandle
    IL_002c:  brfalse.s  IL_004b

    IL_002e:  ldarg.0
    IL_002f:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mDreamscapeMusicHandle
    IL_0034:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Stop()
    IL_0039:  ldarg.0
    IL_003a:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mDreamscapeMusicHandle
    IL_003f:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Dispose()
    IL_0044:  ldarg.0
    IL_0045:  ldnull
    IL_0046:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mDreamscapeMusicHandle
    IL_004b:  ldarg.0
    IL_004c:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_0051:  ret
  } // end of method RoutineMachine::StopDreamscapeEffects

  .method public hidebysig instance void 
          StartShowerAudio() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mShowerAudioHandle
    IL_0006:  brtrue.s   IL_002a

    IL_0008:  ldarg.0
    IL_0009:  ldarg.0
    IL_000a:  call       instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
    IL_000f:  ldstr      "rmachine_shower_appear_lp"
    IL_0014:  newobj     instance void [SimIFace]Sims3.SimIFace.ObjectSound::.ctor(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                   string)
    IL_0019:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mShowerAudioHandle
    IL_001e:  ldarg.0
    IL_001f:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mShowerAudioHandle
    IL_0024:  callvirt   instance bool [SimIFace]Sims3.SimIFace.Sound::StartLoop()
    IL_0029:  pop
    IL_002a:  ret
  } // end of method RoutineMachine::StartShowerAudio

  .method public hidebysig instance void 
          StopShowerAudio() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mShowerAudioHandle
    IL_0006:  brfalse.s  IL_0025

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mShowerAudioHandle
    IL_000e:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Stop()
    IL_0013:  ldarg.0
    IL_0014:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mShowerAudioHandle
    IL_0019:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Dispose()
    IL_001e:  ldarg.0
    IL_001f:  ldnull
    IL_0020:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mShowerAudioHandle
    IL_0025:  ret
  } // end of method RoutineMachine::StopShowerAudio

  .method public hidebysig instance void 
          StartSpeakerEffects() cil managed
  {
    // Code size       125 (0x7d)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0048

    IL_0004:  ldarg.0
    IL_0005:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_000a:  ldloc.0
    IL_000b:  ldelem.ref
    IL_000c:  brtrue.s   IL_0044

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_0014:  ldloc.0
    IL_0015:  ldstr      "store_routineMachine_speaker"
    IL_001a:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
    IL_001f:  stelem.ref
    IL_0020:  ldarg.0
    IL_0021:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_0026:  ldloc.0
    IL_0027:  ldelem.ref
    IL_0028:  ldarg.0
    IL_0029:  ldc.i4     0x318a2828
    IL_002e:  ldloc.0
    IL_002f:  add
    IL_0030:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0035:  pop
    IL_0036:  ldarg.0
    IL_0037:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_003c:  ldloc.0
    IL_003d:  ldelem.ref
    IL_003e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_0043:  pop
    IL_0044:  ldloc.0
    IL_0045:  ldc.i4.1
    IL_0046:  add
    IL_0047:  stloc.0
    IL_0048:  ldloc.0
    IL_0049:  ldc.i4.4
    IL_004a:  blt.s      IL_0004

    IL_004c:  ldarg.0
    IL_004d:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mSpeakerUpgradeMusicHandle
    IL_0052:  brtrue.s   IL_0076

    IL_0054:  ldarg.0
    IL_0055:  ldarg.0
    IL_0056:  call       instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
    IL_005b:  ldstr      "rmachine_futuristic_song"
    IL_0060:  newobj     instance void [SimIFace]Sims3.SimIFace.ObjectSound::.ctor(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                   string)
    IL_0065:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mSpeakerUpgradeMusicHandle
    IL_006a:  ldarg.0
    IL_006b:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mSpeakerUpgradeMusicHandle
    IL_0070:  callvirt   instance bool [SimIFace]Sims3.SimIFace.Sound::StartLoop()
    IL_0075:  pop
    IL_0076:  ldarg.0
    IL_0077:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_007c:  ret
  } // end of method RoutineMachine::StartSpeakerEffects

  .method public hidebysig instance void 
          StopSpeakerEffects() cil managed
  {
    // Code size       102 (0x66)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0036

    IL_0004:  ldarg.0
    IL_0005:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_000a:  ldloc.0
    IL_000b:  ldelem.ref
    IL_000c:  brfalse.s  IL_0032

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_0014:  ldloc.0
    IL_0015:  ldelem.ref
    IL_0016:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_001b:  pop
    IL_001c:  ldarg.0
    IL_001d:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_0022:  ldloc.0
    IL_0023:  ldelem.ref
    IL_0024:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_0029:  ldarg.0
    IL_002a:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect[] Sims3.Store.Objects.RoutineMachine::mVisualEffectSpeakerUpgrade
    IL_002f:  ldloc.0
    IL_0030:  ldnull
    IL_0031:  stelem.ref
    IL_0032:  ldloc.0
    IL_0033:  ldc.i4.1
    IL_0034:  add
    IL_0035:  stloc.0
    IL_0036:  ldloc.0
    IL_0037:  ldc.i4.4
    IL_0038:  blt.s      IL_0004

    IL_003a:  ldarg.0
    IL_003b:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mSpeakerUpgradeMusicHandle
    IL_0040:  brfalse.s  IL_005f

    IL_0042:  ldarg.0
    IL_0043:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mSpeakerUpgradeMusicHandle
    IL_0048:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Stop()
    IL_004d:  ldarg.0
    IL_004e:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mSpeakerUpgradeMusicHandle
    IL_0053:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Dispose()
    IL_0058:  ldarg.0
    IL_0059:  ldnull
    IL_005a:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.RoutineMachine::mSpeakerUpgradeMusicHandle
    IL_005f:  ldarg.0
    IL_0060:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_0065:  ret
  } // end of method RoutineMachine::StopSpeakerEffects

  .method public hidebysig instance void 
          StartPortalReturnVisualEffects(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject actor) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalOut
    IL_0006:  pop
    IL_0007:  ldarg.0
    IL_0008:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_000d:  ret
  } // end of method RoutineMachine::StartPortalReturnVisualEffects

  .method public hidebysig instance void 
          StopPortalReturnVisualEffects() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalOut
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalOut
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalOut
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectPortalOut
    IL_0026:  ldarg.0
    IL_0027:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_002c:  ret
  } // end of method RoutineMachine::StopPortalReturnVisualEffects

  .method public hidebysig instance void 
          StartBrokenVisualEffects() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectBroken
    IL_0006:  brtrue.s   IL_0036

    IL_0008:  ldarg.0
    IL_0009:  ldstr      "store_routineMachine_broken"
    IL_000e:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
    IL_0013:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectBroken
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectBroken
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4     0x318a282c
    IL_0024:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0029:  pop
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectBroken
    IL_0030:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_0035:  pop
    IL_0036:  ldarg.0
    IL_0037:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_003c:  ret
  } // end of method RoutineMachine::StartBrokenVisualEffects

  .method public hidebysig instance void 
          StopBrokenVisualEffects() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectBroken
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectBroken
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectBroken
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectBroken
    IL_0026:  ldarg.0
    IL_0027:  call       instance void Sims3.Store.Objects.RoutineMachine::UpdateVisualEffect()
    IL_002c:  ret
  } // end of method RoutineMachine::StopBrokenVisualEffects

  .method public hidebysig instance void 
          StopIndicatorVisualEffects() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectIndicator
    IL_0006:  brfalse.s  IL_0026

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectIndicator
    IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectIndicator
    IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectIndicator
    IL_0026:  ret
  } // end of method RoutineMachine::StopIndicatorVisualEffects

  .method public hidebysig instance void 
          UpdateVisualEffect() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Sims3.Store.Objects.RoutineMachine::StopIndicatorVisualEffects()
    IL_0006:  ldarg.0
    IL_0007:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
    IL_000c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
    IL_0011:  brfalse.s  IL_001a

    IL_0013:  ldarg.0
    IL_0014:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectIndicator
    IL_0019:  pop
    IL_001a:  ldarg.0
    IL_001b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectIndicator
    IL_0020:  brfalse.s  IL_0040

    IL_0022:  ldarg.0
    IL_0023:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectIndicator
    IL_0028:  ldarg.0
    IL_0029:  ldc.i4     0x318a282d
    IL_002e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                       valuetype [SimIFace]Sims3.SimIFace.Slot)
    IL_0033:  pop
    IL_0034:  ldarg.0
    IL_0035:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.RoutineMachine::mVisualEffectIndicator
    IL_003a:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
    IL_003f:  pop
    IL_0040:  ret
  } // end of method RoutineMachine::UpdateVisualEffect

  .method public hidebysig instance bool 
          Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject destinationObject) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.Vector3 [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::get_Position()
    IL_0008:  call       instance bool Sims3.Store.Objects.RoutineMachine::Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                    valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_000d:  ret
  } // end of method RoutineMachine::Teleport

  .method public hidebysig instance bool 
          Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   valuetype [SimIFace]Sims3.SimIFace.Vector3 destinationPoint) cil managed
  {
    // Code size       209 (0xd1)
    .maxstack  9
    .locals init (valuetype [SimIFace]Sims3.SimIFace.Vector3 V_0,
             class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot V_1,
             class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  call       instance valuetype [SimIFace]Sims3.SimIFace.Vector3 Sims3.Store.Objects.RoutineMachine::TeleportAsExpected(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                    valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Core.LotManager::GetLotAtPoint(valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  ldarg.1
    IL_0012:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LotHome()
    IL_0017:  beq.s      IL_002d

    IL_0019:  ldloc.1
    IL_001a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot::get_IsWorldLot()
    IL_001f:  brtrue.s   IL_0039

    IL_0021:  ldarg.1
    IL_0022:  ldloc.1
    IL_0023:  ldloc.0
    IL_0024:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::IsPointInLotSafelyRoutable(class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot,
                                                                                                                   valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_0029:  brtrue.s   IL_0039

    IL_002b:  ldc.i4.0
    IL_002c:  ret

    IL_002d:  ldloc.0
    IL_002e:  call       valuetype [SimIFace]Sims3.SimIFace.TerrainType [SimIFace]Sims3.SimIFace.World::GetTerrainType(valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_0033:  ldc.i4.s   64
    IL_0035:  bne.un.s   IL_0039

    IL_0037:  ldc.i4.0
    IL_0038:  ret

    IL_0039:  ldloc.0
    IL_003a:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::Zero
    IL_003f:  call       bool [SimIFace]Sims3.SimIFace.Vector3::op_Inequality(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                              valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_0044:  brfalse    IL_00cf

    IL_0049:  ldloc.0
    IL_004a:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::Invalid
    IL_004f:  call       bool [SimIFace]Sims3.SimIFace.Vector3::op_Inequality(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                              valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_0054:  brfalse.s  IL_00cf

    IL_0056:  ldstr      "SocialJigOnePerson"
    IL_005b:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.GlobalFunctions::CreateObjectOutOfWorld(string)
    IL_0060:  stloc.2
    IL_0061:  ldloc.2
    IL_0062:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::AddToWorld()
    IL_0067:  ldloc.2
    IL_0068:  ldloc.0
    IL_0069:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::SetPosition(valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_006e:  ldloc.2
    IL_006f:  ldc.i4.1
    IL_0070:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::SetHiddenFlags(valuetype [SimIFace]Sims3.SimIFace.HiddenFlags)
    .try
    {
      IL_0075:  ldarg.0
      IL_0076:  ldloc.2
      IL_0077:  call       instance void Sims3.Store.Objects.RoutineMachine::StartPortalReturnVisualEffects(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_007c:  ldarg.1
      IL_007d:  ldloc.0
      IL_007e:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::Invalid
      IL_0083:  ldarg.0
      IL_0084:  ldarg.1
      IL_0085:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsActiveSim()
      IL_008a:  ldc.i4.0
      IL_008b:  ldc.i4.0
      IL_008c:  ldc.i4.0
      IL_008d:  ldc.i4.0
      IL_008e:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Core.Terrain::Teleport(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                            valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                            valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ITeleporter,
                                                                                            bool,
                                                                                            bool,
                                                                                            bool,
                                                                                            bool,
                                                                                            bool)
      IL_0093:  ldc.r4     2.
      IL_0098:  ldc.i4.1
      IL_0099:  call       int64 [Sims3GameplaySystems]Sims3.Gameplay.Utilities.SimClock::ConvertToTicks(float32,
                                                                                                         valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit)
      IL_009e:  conv.u4
      IL_009f:  call       void [SimIFace]Sims3.SimIFace.Simulator::Sleep(uint32)
      IL_00a4:  ldarg.0
      IL_00a5:  call       instance void Sims3.Store.Objects.RoutineMachine::StopPortalReturnVisualEffects()
      IL_00aa:  leave.s    IL_00b3

    }  // end .try
    finally
    {
      IL_00ac:  ldloc.2
      IL_00ad:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject::Destroy()
      IL_00b2:  endfinally
    }  // end handler
    IL_00b3:  ldarg.0
    IL_00b4:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
    IL_00b9:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::get_Broken()
    IL_00be:  brtrue.s   IL_00cd

    IL_00c0:  ldarg.0
    IL_00c1:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Repairable()
    IL_00c6:  ldarg.1
    IL_00c7:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent::UpdateBreakage(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
    IL_00cc:  pop
    IL_00cd:  ldc.i4.1
    IL_00ce:  ret

    IL_00cf:  ldc.i4.0
    IL_00d0:  ret
  } // end of method RoutineMachine::Teleport

  .method public hidebysig instance valuetype [SimIFace]Sims3.SimIFace.Vector3 
          TeleportAsExpected(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                             valuetype [SimIFace]Sims3.SimIFace.Vector3 destinationPoint) cil managed
  {
    // Code size       38 (0x26)
    .maxstack  4
    .locals init (valuetype [SimIFace]Sims3.SimIFace.Vector3 V_0,
             valuetype [SimIFace]Sims3.SimIFace.Vector3 V_1)
    IL_0000:  ldarg.2
    IL_0001:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::Invalid
    IL_0006:  call       bool [SimIFace]Sims3.SimIFace.Vector3::op_Inequality(valuetype [SimIFace]Sims3.SimIFace.Vector3,
                                                                              valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_000b:  brfalse.s  IL_0020

    IL_000d:  ldarg.1
    IL_000e:  ldarg.2
    IL_000f:  newobj     instance void [SimIFace]Sims3.SimIFace.World/FindGoodLocationParams::.ctor(valuetype [SimIFace]Sims3.SimIFace.Vector3)
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldloca.s   V_1
    IL_0018:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.GlobalFunctions::FindGoodLocation(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                     valuetype [SimIFace]Sims3.SimIFace.World/FindGoodLocationParams,
                                                                                                     valuetype [SimIFace]Sims3.SimIFace.Vector3&,
                                                                                                     valuetype [SimIFace]Sims3.SimIFace.Vector3&)
    IL_001d:  pop
    IL_001e:  ldloc.0
    IL_001f:  ret

    IL_0020:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Vector3 [SimIFace]Sims3.SimIFace.Vector3::Invalid
    IL_0025:  ret
  } // end of method RoutineMachine::TeleportAsExpected

  .method public hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       66 (0x42)
    .maxstack  3
    IL_0000:  ldstr      "Sims3.Store.Objects"
    IL_0005:  ldc.i4.6
    IL_0006:  callvirt   instance string [mscorlib]System.String::Substring(int32)
    IL_000b:  ldc.i4.s   46
    IL_000d:  ldc.i4.s   47
    IL_000f:  callvirt   instance string [mscorlib]System.String::Replace(char,
                                                                          char)
    IL_0014:  ldstr      "/RoutineMachine"
    IL_0019:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_001e:  stsfld     string Sims3.Store.Objects.RoutineMachine::sLocalizationKey
    IL_0023:  ldc.i4     0x31229a4c
    IL_0028:  stsfld     valuetype [SimIFace]Sims3.SimIFace.Slot Sims3.Store.Objects.RoutineMachine::kRoutingSlot
    IL_002d:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Controllers.PuddleManager/FlowingPuddleTuning::.ctor()
    IL_0032:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Controllers.PuddleManager/FlowingPuddleTuning Sims3.Store.Objects.RoutineMachine::kPuddlesTuning
    IL_0037:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning::.ctor()
    IL_003c:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning Sims3.Store.Objects.RoutineMachine::kRepairableComponentTuning
    IL_0041:  ret
  } // end of method RoutineMachine::.cctor

  .property instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning
          RepairableTuning()
  {
    .get instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.RepairableComponent/Tuning Sims3.Store.Objects.RoutineMachine::get_RepairableTuning()
  } // end of property RoutineMachine::RepairableTuning
} // end of class Sims3.Store.Objects.RoutineMachine


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file D:\Downloads\For Games\Sims3 Downloads\Hacks\Coding\Sims3\Store\0x48C91DDF59415321.res
