
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
.assembly extern Sims3GameplayObjects
{
  .ver 1:0:0:0
}
.assembly extern UI
{
  .ver 1:0:0:0
}
.assembly StoreObjectsTablet
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 42 34 37 39 32 38 31 38 2D 41 45 45 30   // ..$B4792818-AEE0
                                                                                                  2D 45 36 38 39 2D 35 34 37 45 2D 43 39 32 44 43   // -E689-547E-C92DC
                                                                                                  42 30 42 34 42 31 33 00 00 )                      // B0B4B13..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1D 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  45 41 2E 63 6F 6D 20 32 30 30 37 2D 32 30 31 30   // EA.com 2007-2010
                                                                                                  00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 10 53 69 6D 73 33 53 74 6F 72 65 54 61 62   // ...Sims3StoreTab
                                                                                                6C 65 74 00 00 )                                  // let..
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 45 41 2E 63 6F 6D 00 00 )                // ...EA.com..
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 10 53 69 6D 73 33 53 74 6F 72 65 54 61 62   // ...Sims3StoreTab
                                                                                              6C 65 74 00 00 )                                  // let..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module StoreObjectsTablet.dll
// MVID: {F8A706FC-DB0B-4D96-B027-6CE62B9E7A69}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00290000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Sims3.Store.Objects.Tablet
       extends [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject
       implements [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable,
                  [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.IBaseCarryable,
                  [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.IHasRouteRadius,
                  [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.IUseCarrySitTransitions,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.ISuctionable,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IScriptObject,
                  [SimIFace]Sims3.SimIFace.IScriptLogic,
                  [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                  [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IObjectUI,
                  [SimIFace]Sims3.SimIFace.CustomContent.IExportableContent
{
  .custom instance void [SimIFace]Sims3.SimIFace.RuntimeExportAttribute::.ctor() = ( 01 00 00 00 ) 
  .class auto ansi nested public beforefieldinit ChooseBookOnTablet
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/ChooseBookOnTablet>
           implements [Sims3GameplaySystems]Sims3.Gameplay.Interactions.IOverridesCalculateScore
    {
      .field public class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book mAutonomyBook
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       22 (0x16)
        .maxstack  3
        .locals init (class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> V_0)
        IL_0000:  ldarg.1
        IL_0001:  ldc.i4.1
        IL_0002:  ldc.i4.0
        IL_0003:  call       class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> Sims3.Store.Objects.Tablet::GetBooksOnMyLot(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                    bool,
                                                                                                                                                                                    bool)
        IL_0008:  stloc.0
        IL_0009:  ldloc.0
        IL_000a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::get_Count()
        IL_000f:  ldc.i4.0
        IL_0010:  bgt.s      IL_0014

        IL_0012:  ldc.i4.0
        IL_0013:  ret

        IL_0014:  ldc.i4.1
        IL_0015:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "ReadBook"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance void 
              PopulatePieMenuPicker(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters& parameters,
                                    [out] class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>& listObjs,
                                    [out] class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>& headers,
                                    [out] int32& NumSelectableRows) cil managed
      {
        // Code size       375 (0x177)
        .maxstack  5
        .locals init (class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/RowInfo> V_0,
                 class [UI]Sims3.SimIFace.GreyedOutTooltipCallback V_1,
                 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> V_2,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book V_3,
                 class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo> V_4,
                 valuetype [SimIFace]Sims3.SimIFace.ResourceKey V_5,
                 valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey V_6,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.MedicalJournalData V_7,
                 class [UI]Sims3.UI.ObjectPicker/RowInfo V_8,
                 class [UI]Sims3.UI.ObjectPicker/TabInfo V_9,
                 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> V_10)
        IL_0000:  ldarg.s    NumSelectableRows
        IL_0002:  ldc.i4.1
        IL_0003:  stind.i4
        IL_0004:  ldarg.3
        IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>::.ctor()
        IL_000a:  stind.ref
        IL_000b:  ldarg.2
        IL_000c:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>::.ctor()
        IL_0011:  stind.ref
        IL_0012:  ldarg.3
        IL_0013:  ldind.ref
        IL_0014:  ldstr      "Ui/Caption/ObjectPicker:Title"
        IL_0019:  ldstr      "Ui/Tooltip/ObjectPicker:Name"
        IL_001e:  ldc.i4     0xfa
        IL_0023:  newobj     instance void [UI]Sims3.UI.ObjectPicker/HeaderInfo::.ctor(string,
                                                                                       string,
                                                                                       int32)
        IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>::Add(!0)
        IL_002d:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/RowInfo>::.ctor()
        IL_0032:  stloc.0
        IL_0033:  ldnull
        IL_0034:  stloc.1
        IL_0035:  ldarg.1
        IL_0036:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters::get_Actor()
        IL_003b:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim
        IL_0040:  ldc.i4.0
        IL_0041:  ldc.i4.1
        IL_0042:  call       class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> Sims3.Store.Objects.Tablet::GetBooksOnMyLot(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                    bool,
                                                                                                                                                                                    bool)
        IL_0047:  stloc.2
        IL_0048:  ldloc.2
        IL_0049:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::GetEnumerator()
        IL_004e:  stloc.s    V_10
        .try
        {
          IL_0050:  br         IL_0134

          IL_0055:  ldloca.s   V_10
          IL_0057:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::get_Current()
          IL_005c:  stloc.3
          IL_005d:  ldloc.3
          IL_005e:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.SheetMusic
          IL_0063:  brtrue     IL_0134

          IL_0068:  ldloc.3
          IL_0069:  ldarg.1
          IL_006a:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters::get_Actor()
          IL_006f:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim
          IL_0074:  ldarg.1
          IL_0075:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters::get_Autonomous()
          IL_007a:  ldloca.s   V_1
          IL_007c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::TestReadBook(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             bool,
                                                                                                             class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
          IL_0081:  brfalse    IL_0134

          IL_0086:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo>::.ctor()
          IL_008b:  stloc.s    V_4
          IL_008d:  ldloca.s   V_5
          IL_008f:  ldstr      "book_standard"
          IL_0094:  call       uint32 [SimIFace]Sims3.SimIFace.ResourceUtils::XorFoldHashString32(string)
          IL_0099:  conv.u8
          IL_009a:  ldc.i4     0x1661233
          IL_009f:  ldc.i4.1
          IL_00a0:  call       instance void [SimIFace]Sims3.SimIFace.ResourceKey::.ctor(uint64,
                                                                                         uint32,
                                                                                         uint32)
          IL_00a5:  ldloca.s   V_6
          IL_00a7:  ldloc.s    V_5
          IL_00a9:  ldc.i4.1
          IL_00aa:  ldloc.3
          IL_00ab:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::Data
          IL_00b0:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData::GeometryState
          IL_00b5:  call       uint32 [SimIFace]Sims3.SimIFace.ResourceUtils::HashString32(string)
          IL_00ba:  ldloc.3
          IL_00bb:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::Data
          IL_00c0:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData::MaterialState
          IL_00c5:  call       uint32 [SimIFace]Sims3.SimIFace.ResourceUtils::HashString32(string)
          IL_00ca:  call       instance void [SimIFace]Sims3.SimIFace.ThumbnailKey::.ctor(valuetype [SimIFace]Sims3.SimIFace.ResourceKey,
                                                                                          valuetype [SimIFace]Sims3.SimIFace.ThumbnailSize,
                                                                                          uint32,
                                                                                          uint32)
          IL_00cf:  ldloc.3
          IL_00d0:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::Data
          IL_00d5:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.MedicalJournalData
          IL_00da:  stloc.s    V_7
          IL_00dc:  ldloc.s    V_7
          IL_00de:  brfalse.s  IL_0109

          IL_00e0:  ldloc.s    V_4
          IL_00e2:  ldloc.s    V_6
          IL_00e4:  ldloc.s    V_7
          IL_00e6:  ldloc.3
          IL_00e7:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.MedicalJournal
          IL_00ec:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Objects.MedicalJournal::mOwner
          IL_00f1:  ldloc.s    V_7
          IL_00f3:  ldfld      int32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.MedicalJournalData::CurrentEdition
          IL_00f8:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.MedicalJournalData::GetTitle(class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription,
                                                                                                                         int32)
          IL_00fd:  newobj     instance void [UI]Sims3.UI.ObjectPicker/ThumbAndTextColumn::.ctor(valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey,
                                                                                                 string)
          IL_0102:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo>::Add(!0)
          IL_0107:  br.s       IL_0122

          IL_0109:  ldloc.s    V_4
          IL_010b:  ldloc.s    V_6
          IL_010d:  ldloc.3
          IL_010e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::Data
          IL_0113:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData::get_Title()
          IL_0118:  newobj     instance void [UI]Sims3.UI.ObjectPicker/ThumbAndTextColumn::.ctor(valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey,
                                                                                                 string)
          IL_011d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo>::Add(!0)
          IL_0122:  ldloc.3
          IL_0123:  ldloc.s    V_4
          IL_0125:  newobj     instance void [UI]Sims3.UI.ObjectPicker/RowInfo::.ctor(object,
                                                                                      class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo>)
          IL_012a:  stloc.s    V_8
          IL_012c:  ldloc.0
          IL_012d:  ldloc.s    V_8
          IL_012f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/RowInfo>::Add(!0)
          IL_0134:  ldloca.s   V_10
          IL_0136:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::MoveNext()
          IL_013b:  brtrue     IL_0055

          IL_0140:  leave.s    IL_0150

        }  // end .try
        finally
        {
          IL_0142:  ldloca.s   V_10
          IL_0144:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>
          IL_014a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          IL_014f:  endfinally
        }  // end handler
        IL_0150:  ldstr      "Coupon"
        IL_0155:  ldstr      "Ui/Caption/ObjectPicker:Books"
        IL_015a:  ldc.i4.0
        IL_015b:  newarr     [mscorlib]System.Object
        IL_0160:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                                object[])
        IL_0165:  ldloc.0
        IL_0166:  newobj     instance void [UI]Sims3.UI.ObjectPicker/TabInfo::.ctor(string,
                                                                                    string,
                                                                                    class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/RowInfo>)
        IL_016b:  stloc.s    V_9
        IL_016d:  ldarg.2
        IL_016e:  ldind.ref
        IL_016f:  ldloc.s    V_9
        IL_0171:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>::Add(!0)
        IL_0176:  ret
      } // end of method Definition::PopulatePieMenuPicker

      .method public hidebysig newslot virtual final 
              instance float32  CalculateScore(class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interactionObjectPair,
                                               class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy autonomy) cil managed
      {
        // Code size       212 (0xd4)
        .maxstack  4
        .locals init (float32 V_0,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book V_1,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookGeneral V_2,
                 class [UI]Sims3.SimIFace.GreyedOutTooltipCallback V_3,
                 float32 V_4,
                 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> V_5)
        IL_0000:  ldc.r4     0.0
        IL_0005:  stloc.0
        IL_0006:  ldarg.0
        IL_0007:  ldnull
        IL_0008:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/ChooseBookOnTablet/Definition::mAutonomyBook
        IL_000d:  ldarg.2
        IL_000e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
        IL_0013:  ldc.i4.0
        IL_0014:  ldc.i4.1
        IL_0015:  call       class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> Sims3.Store.Objects.Tablet::GetBooksOnMyLot(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                                                                    bool,
                                                                                                                                                                                    bool)
        IL_001a:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::GetEnumerator()
        IL_001f:  stloc.s    V_5
        .try
        {
          IL_0021:  br.s       IL_0065

          IL_0023:  ldloca.s   V_5
          IL_0025:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::get_Current()
          IL_002a:  stloc.1
          IL_002b:  ldloc.1
          IL_002c:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookGeneral
          IL_0031:  stloc.2
          IL_0032:  ldnull
          IL_0033:  stloc.3
          IL_0034:  ldloc.2
          IL_0035:  brfalse.s  IL_0065

          IL_0037:  ldloc.2
          IL_0038:  ldarg.2
          IL_0039:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
          IL_003e:  ldc.i4.1
          IL_003f:  ldloca.s   V_3
          IL_0041:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::TestReadBook(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             bool,
                                                                                                             class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
          IL_0046:  brfalse.s  IL_0065

          IL_0048:  ldarg.2
          IL_0049:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
          IL_004e:  ldloc.2
          IL_004f:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::GetInterestInBook(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookGeneral)
          IL_0054:  stloc.s    V_4
          IL_0056:  ldloc.s    V_4
          IL_0058:  ldloc.0
          IL_0059:  ble.un.s   IL_0065

          IL_005b:  ldarg.0
          IL_005c:  ldloc.1
          IL_005d:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/ChooseBookOnTablet/Definition::mAutonomyBook
          IL_0062:  ldloc.s    V_4
          IL_0064:  stloc.0
          IL_0065:  ldloca.s   V_5
          IL_0067:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::MoveNext()
          IL_006c:  brtrue.s   IL_0023

          IL_006e:  leave.s    IL_007e

        }  // end .try
        finally
        {
          IL_0070:  ldloca.s   V_5
          IL_0072:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>
          IL_0078:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          IL_007d:  endfinally
        }  // end handler
        IL_007e:  ldarg.0
        IL_007f:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/ChooseBookOnTablet/Definition::mAutonomyBook
        IL_0084:  brtrue.s   IL_008c

        IL_0086:  ldc.r4     0.0
        IL_008b:  ret

        IL_008c:  ldarg.2
        IL_008d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
        IL_0092:  ldc.i8     0x4c800171bd46a00
        IL_009b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_00a0:  brfalse.s  IL_00ac

        IL_00a2:  ldloc.0
        IL_00a3:  ldsfld     float32 Sims3.Store.Objects.Tablet::kCompWhizMultiplier
        IL_00a8:  mul
        IL_00a9:  stloc.0
        IL_00aa:  br.s       IL_00ca

        IL_00ac:  ldarg.2
        IL_00ad:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
        IL_00b2:  ldc.i8     0x4c800171bd469d0
        IL_00bb:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_00c0:  brfalse.s  IL_00ca

        IL_00c2:  ldloc.0
        IL_00c3:  ldsfld     float32 Sims3.Store.Objects.Tablet::kTechnophobeMultiplier
        IL_00c8:  mul
        IL_00c9:  stloc.0
        IL_00ca:  ldloc.0
        IL_00cb:  ldarg.2
        IL_00cc:  ldarg.1
        IL_00cd:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::CalculateScoreForObjectInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair)
        IL_00d2:  mul
        IL_00d3:  ret
      } // end of method Definition::CalculateScore

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/ChooseBookOnTablet>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig specialname 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book 
            get_AutonomyBook() cil managed
    {
      // Code size       24 (0x18)
      .maxstack  1
      .locals init (class Sims3.Store.Objects.Tablet/ChooseBookOnTablet/Definition V_0)
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0006:  isinst     Sims3.Store.Objects.Tablet/ChooseBookOnTablet/Definition
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  brtrue.s   IL_0011

      IL_000f:  ldnull
      IL_0010:  ret

      IL_0011:  ldloc.0
      IL_0012:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/ChooseBookOnTablet/Definition::mAutonomyBook
      IL_0017:  ret
    } // end of method ChooseBookOnTablet::get_AutonomyBook

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       222 (0xde)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book V_0,
               bool V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition V_2,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_3,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition V_4,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_5)
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/ChooseBookOnTablet::get_AutonomyBook()
      IL_0010:  stloc.0
      IL_0011:  br.s       IL_001f

      IL_0013:  ldarg.0
      IL_0014:  call       instance object [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetSelectedObject()
      IL_0019:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book
      IL_001e:  stloc.0
      IL_001f:  ldloc.0
      IL_0020:  brtrue.s   IL_0024

      IL_0022:  ldc.i4.0
      IL_0023:  ret

      IL_0024:  ldc.i4.0
      IL_0025:  stloc.1
      IL_0026:  ldarg.0
      IL_0027:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_002c:  ldarg.0
      IL_002d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0032:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PopCanePostureIfNecessary()
      IL_0037:  ldarg.0
      IL_0038:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0042:  ldarg.0
      IL_0043:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0048:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_004d:  brfalse.s  IL_0097

      IL_004f:  ldc.i4.1
      IL_0050:  newobj     instance void Sims3.Store.Objects.Tablet/ReadBookOnTablet/Definition::.ctor(bool)
      IL_0055:  stloc.2
      IL_0056:  ldloc.2
      IL_0057:  ldloc.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_005e:  ldarg.0
      IL_005f:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0064:  ldarg.0
      IL_0065:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_006a:  ldc.i4.1
      IL_006b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0070:  stloc.3
      IL_0071:  ldloc.3
      IL_0072:  isinst     Sims3.Store.Objects.Tablet/ReadBookOnTablet
      IL_0077:  ldarg.0
      IL_0078:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_007d:  stfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0082:  ldarg.0
      IL_0083:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0088:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_008d:  ldloc.3
      IL_008e:  ldc.i4.0
      IL_008f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0094:  stloc.1
      IL_0095:  br.s       IL_00d5

      IL_0097:  ldloc.0
      IL_0098:  newobj     instance void Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book)
      IL_009d:  stloc.s    V_4
      IL_009f:  ldloc.s    V_4
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_00a7:  ldarg.0
      IL_00a8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00ad:  ldarg.0
      IL_00ae:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_00b3:  ldarg.0
      IL_00b4:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_00b9:  ldc.i4.1
      IL_00ba:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_00bf:  stloc.s    V_5
      IL_00c1:  ldarg.0
      IL_00c2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00c7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_00cc:  ldloc.s    V_5
      IL_00ce:  ldc.i4.0
      IL_00cf:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_00d4:  stloc.1
      IL_00d5:  ldarg.0
      IL_00d6:  ldloc.1
      IL_00d7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_00dc:  ldc.i4.1
      IL_00dd:  ret
    } // end of method ChooseBookOnTablet::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Run()
      IL_0006:  ret
    } // end of method ChooseBookOnTablet::RunFromInventory

    .method public hidebysig virtual instance bool 
            TargetInventoryChangeIsValid(class [Sims3GameplaySystems]Sims3.Gameplay.Inventory newInventory) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brfalse.s  IL_0011

      IL_0003:  ldarg.1
      IL_0004:  ldarg.0
      IL_0005:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_000f:  bne.un.s   IL_0013

      IL_0011:  ldc.i4.1
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method ChooseBookOnTablet::TargetInventoryChangeIsValid

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method ChooseBookOnTablet::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/ChooseBookOnTablet/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ChooseBookOnTablet::Singleton
      IL_000a:  ret
    } // end of method ChooseBookOnTablet::.cctor

    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book
            AutonomyBook()
    {
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/ChooseBookOnTablet::get_AutonomyBook()
    } // end of property ChooseBookOnTablet::AutonomyBook
  } // end of class ChooseBookOnTablet

  .class auto ansi nested public beforefieldinit PickUpTabletForReading
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForReading>
    {
      .field public class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book mBook
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForReading>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book book) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForReading>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition::mBook
        IL_000d:  ret
      } // end of method Definition::.ctor

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       40 (0x28)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  bne.un.s   IL_000b

        IL_0009:  ldc.i4.0
        IL_000a:  ret

        IL_000b:  ldarg.0
        IL_000c:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition::mBook
        IL_0011:  brfalse.s  IL_0024

        IL_0013:  ldarg.0
        IL_0014:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition::mBook
        IL_0019:  ldarg.1
        IL_001a:  ldarg.3
        IL_001b:  ldarg.s    greyedOutTooltipCallback
        IL_001d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::TestReadBook(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           bool,
                                                                                                           class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_0022:  brtrue.s   IL_0026

        IL_0024:  ldc.i4.0
        IL_0025:  ret

        IL_0026:  ldc.i4.1
        IL_0027:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       90 (0x5a)
        .maxstack  4
        .locals init (object[] V_0,
                 object[] V_1)
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition::mBook
        IL_0007:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::Data
        IL_000c:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData::ID
        IL_0011:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBookData::HasSimFinishedBook(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                string)
        IL_0016:  brfalse.s  IL_0039

        IL_0018:  ldstr      "Reread"
        IL_001d:  ldc.i4.1
        IL_001e:  newarr     [mscorlib]System.Object
        IL_0023:  stloc.0
        IL_0024:  ldloc.0
        IL_0025:  ldc.i4.0
        IL_0026:  ldarg.0
        IL_0027:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition::mBook
        IL_002c:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::get_Title()
        IL_0031:  stelem.ref
        IL_0032:  ldloc.0
        IL_0033:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0038:  ret

        IL_0039:  ldstr      "Read"
        IL_003e:  ldc.i4.1
        IL_003f:  newarr     [mscorlib]System.Object
        IL_0044:  stloc.1
        IL_0045:  ldloc.1
        IL_0046:  ldc.i4.0
        IL_0047:  ldarg.0
        IL_0048:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition::mBook
        IL_004d:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::get_Title()
        IL_0052:  stelem.ref
        IL_0053:  ldloc.1
        IL_0054:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0059:  ret
      } // end of method Definition::GetInteractionName

    } // end of class Definition

    .method public hidebysig specialname 
            instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book 
            get_BookToRead() cil managed
    {
      // Code size       24 (0x18)
      .maxstack  1
      .locals init (class Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition V_0)
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0006:  isinst     Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  brtrue.s   IL_0011

      IL_000f:  ldnull
      IL_0010:  ret

      IL_0011:  ldloc.0
      IL_0012:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading/Definition::mBook
      IL_0017:  ret
    } // end of method PickUpTabletForReading::get_BookToRead

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       29 (0x1d)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PickUp(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable)
      IL_0011:  brtrue.s   IL_0015

      IL_0013:  ldc.i4.0
      IL_0014:  ret

      IL_0015:  ldarg.0
      IL_0016:  ldc.i4.0
      IL_0017:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForReading::PushReadInteraction(bool)
      IL_001c:  ret
    } // end of method PickUpTabletForReading::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForReading::PushReadInteraction(bool)
      IL_0007:  ret
    } // end of method PickUpTabletForReading::RunFromInventory

    .method public hidebysig virtual instance bool 
            TargetInventoryChangeIsValid(class [Sims3GameplaySystems]Sims3.Gameplay.Inventory newInventory) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brfalse.s  IL_0011

      IL_0003:  ldarg.1
      IL_0004:  ldarg.0
      IL_0005:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_000f:  bne.un.s   IL_0013

      IL_0011:  ldc.i4.1
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method PickUpTabletForReading::TargetInventoryChangeIsValid

    .method public hidebysig instance bool 
            PushReadInteraction(bool fromInventory) cil managed
    {
      // Code size       92 (0x5c)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_1)
      IL_0000:  ldarg.1
      IL_0001:  newobj     instance void Sims3.Store.Objects.Tablet/ReadBookOnTablet/Definition::.ctor(bool)
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldarg.0
      IL_0009:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading::get_BookToRead()
      IL_000e:  ldarg.0
      IL_000f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0014:  ldarg.0
      IL_0015:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_001a:  ldarg.0
      IL_001b:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0020:  ldc.i4.1
      IL_0021:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0026:  stloc.1
      IL_0027:  ldloc.1
      IL_0028:  isinst     Sims3.Store.Objects.Tablet/ReadBookOnTablet
      IL_002d:  ldarg.0
      IL_002e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0033:  stfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0043:  ldloc.1
      IL_0044:  ldc.i4.0
      IL_0045:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_004a:  brtrue.s   IL_005a

      IL_004c:  ldarg.0
      IL_004d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0052:  ldc.i4.1
      IL_0053:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PutDown(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SurfaceType)
      IL_0058:  ldc.i4.0
      IL_0059:  ret

      IL_005a:  ldc.i4.1
      IL_005b:  ret
    } // end of method PickUpTabletForReading::PushReadInteraction

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method PickUpTabletForReading::.ctor

    .property instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book
            BookToRead()
    {
      .get instance class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/PickUpTabletForReading::get_BookToRead()
    } // end of property PickUpTabletForReading::BookToRead
  } // end of class PickUpTabletForReading

  .class auto ansi nested public beforefieldinit ReadBookOnTablet
         extends [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBook
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book,class Sims3.Store.Objects.Tablet/ReadBookOnTablet>
    {
      .field public bool mFromInventory
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  stfld      bool Sims3.Store.Objects.Tablet/ReadBookOnTablet/Definition::mFromInventory
        IL_0007:  ldarg.0
        IL_0008:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book,class Sims3.Store.Objects.Tablet/ReadBookOnTablet>::.ctor()
        IL_000d:  ret
      } // end of method Definition::.ctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(bool fromInventory) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  stfld      bool Sims3.Store.Objects.Tablet/ReadBookOnTablet/Definition::mFromInventory
        IL_0007:  ldarg.0
        IL_0008:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book,class Sims3.Store.Objects.Tablet/ReadBookOnTablet>::.ctor()
        IL_000d:  ldarg.0
        IL_000e:  ldarg.1
        IL_000f:  stfld      bool Sims3.Store.Objects.Tablet/ReadBookOnTablet/Definition::mFromInventory
        IL_0014:  ret
      } // end of method Definition::.ctor

      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       51 (0x33)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
        IL_0006:  ldc.i4     0x40f3d22
        IL_000b:  ldarg.2
        IL_000c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
        IL_0011:  brtrue.s   IL_0028

        IL_0013:  ldarg.1
        IL_0014:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
        IL_0019:  ldc.i4     0x40f3d23
        IL_001e:  ldarg.2
        IL_001f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
        IL_0024:  brtrue.s   IL_0028

        IL_0026:  ldc.i4.0
        IL_0027:  ret

        IL_0028:  ldarg.2
        IL_0029:  ldarg.1
        IL_002a:  ldarg.3
        IL_002b:  ldarg.s    greyedOutTooltipCallback
        IL_002d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::TestReadBook(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           bool,
                                                                                                           class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_0032:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interaction) cil managed
      {
        // Code size       75 (0x4b)
        .maxstack  4
        .locals init (object[] V_0,
                 object[] V_1)
        IL_0000:  ldarg.1
        IL_0001:  ldarg.2
        IL_0002:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::Data
        IL_0007:  ldfld      string [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData::ID
        IL_000c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBookData::HasSimFinishedBook(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                string)
        IL_0011:  brfalse.s  IL_002f

        IL_0013:  ldstr      "Reread"
        IL_0018:  ldc.i4.1
        IL_0019:  newarr     [mscorlib]System.Object
        IL_001e:  stloc.0
        IL_001f:  ldloc.0
        IL_0020:  ldc.i4.0
        IL_0021:  ldarg.2
        IL_0022:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::get_Title()
        IL_0027:  stelem.ref
        IL_0028:  ldloc.0
        IL_0029:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_002e:  ret

        IL_002f:  ldstr      "Read"
        IL_0034:  ldc.i4.1
        IL_0035:  newarr     [mscorlib]System.Object
        IL_003a:  stloc.1
        IL_003b:  ldloc.1
        IL_003c:  ldc.i4.0
        IL_003d:  ldarg.2
        IL_003e:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::get_Title()
        IL_0043:  stelem.ref
        IL_0044:  ldloc.1
        IL_0045:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_004a:  ret
      } // end of method Definition::GetInteractionName

    } // end of class Definition

    .field public static literal string kReadTabletVFXName = "store_tablet_read"
    .field public class Sims3.Store.Objects.Tablet mTablet
    .field public class [SimIFace]Sims3.SimIFace.VisualEffect mScreenVFX
    .field public bool isBookWorm
    .method public hidebysig specialname 
            instance bool  get_FromInventory() cil managed
    {
      // Code size       24 (0x18)
      .maxstack  1
      .locals init (class Sims3.Store.Objects.Tablet/ReadBookOnTablet/Definition V_0)
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_InteractionDefinition()
      IL_0006:  isinst     Sims3.Store.Objects.Tablet/ReadBookOnTablet/Definition
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  brtrue.s   IL_0011

      IL_000f:  ldc.i4.0
      IL_0010:  ret

      IL_0011:  ldloc.0
      IL_0012:  ldfld      bool Sims3.Store.Objects.Tablet/ReadBookOnTablet/Definition::mFromInventory
      IL_0017:  ret
    } // end of method ReadBookOnTablet::get_FromInventory

    .method public hidebysig virtual instance void 
            ConfigureInteraction() cil managed
    {
      // Code size       207 (0xcf)
      .maxstack  8
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookSkill V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookSkillData V_1,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames V_2,
               class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage> V_3,
               object[] V_4,
               object[] V_5)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0006:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookSkill
      IL_000b:  stloc.0
      IL_000c:  ldloc.0
      IL_000d:  brfalse    IL_00ce

      IL_0012:  ldarg.0
      IL_0013:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0018:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::Data
      IL_001d:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookSkillData
      IL_0022:  stloc.1
      IL_0023:  ldloc.1
      IL_0024:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookSkillData::SkillGuid
      IL_0029:  stloc.2
      IL_002a:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage>::.ctor()
      IL_002f:  stloc.3
      IL_0030:  ldloc.3
      IL_0031:  ldstr      "SimpleStage"
      IL_0036:  ldc.i4.1
      IL_0037:  newarr     [mscorlib]System.Object
      IL_003c:  stloc.s    V_4
      IL_003e:  ldloc.s    V_4
      IL_0040:  ldc.i4.0
      IL_0041:  ldloc.1
      IL_0042:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData::get_Title()
      IL_0047:  stelem.ref
      IL_0048:  ldloc.s    V_4
      IL_004a:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                             object[])
      IL_004f:  ldc.r4     3.4028235e+038
      IL_0054:  ldarg.0
      IL_0055:  ldftn      instance float32 Sims3.Store.Objects.Tablet/ReadBookOnTablet::DummyTest(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_005b:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.SimpleStage/CompletionTest::.ctor(object,
                                                                                                                             native int)
      IL_0060:  ldc.i4.0
      IL_0061:  ldc.i4.1
      IL_0062:  ldc.i4.0
      IL_0063:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.SimpleStage::.ctor(string,
                                                                                                              float32,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.SimpleStage/CompletionTest,
                                                                                                              bool,
                                                                                                              bool,
                                                                                                              bool)
      IL_0068:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage>::Add(!0)
      IL_006d:  ldloc.3
      IL_006e:  ldstr      "SkillStage"
      IL_0073:  ldc.i4.2
      IL_0074:  newarr     [mscorlib]System.Object
      IL_0079:  stloc.s    V_5
      IL_007b:  ldloc.s    V_5
      IL_007d:  ldc.i4.0
      IL_007e:  ldloc.2
      IL_007f:  ldarg.0
      IL_0080:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0085:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
      IL_008a:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::GetLocalizedSkillName(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames,
                                                                                                           class [UI]Sims3.UI.CAS.ISimDescription)
      IL_008f:  stelem.ref
      IL_0090:  ldloc.s    V_5
      IL_0092:  ldc.i4.1
      IL_0093:  ldloc.1
      IL_0094:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData::get_Title()
      IL_0099:  stelem.ref
      IL_009a:  ldloc.s    V_5
      IL_009c:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                             object[])
      IL_00a1:  ldloc.2
      IL_00a2:  ldarg.0
      IL_00a3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_00a8:  ldloc.1
      IL_00a9:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookData::PagesMinNorm
      IL_00ae:  ldloc.1
      IL_00af:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookSkillData::SkillPointsPerPage
      IL_00b4:  mul
      IL_00b5:  ldc.r4     60.
      IL_00ba:  mul
      IL_00bb:  ldc.i4.1
      IL_00bc:  ldc.i4.1
      IL_00bd:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.SkillStage::.ctor(string,
                                                                                                             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames,
                                                                                                             class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                             float32,
                                                                                                             bool,
                                                                                                             bool)
      IL_00c2:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage>::Add(!0)
      IL_00c7:  ldarg.0
      IL_00c8:  ldloc.3
      IL_00c9:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::set_Stages(class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Stage>)
      IL_00ce:  ret
    } // end of method ReadBookOnTablet::ConfigureInteraction

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       33 (0x21)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0006:  brtrue.s   IL_001a

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_000e:  ldc.i4     0x10000
      IL_0013:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0018:  ldc.i4.0
      IL_0019:  ret

      IL_001a:  ldarg.0
      IL_001b:  call       instance bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::DoReadBookOnTablet()
      IL_0020:  ret
    } // end of method ReadBookOnTablet::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Run()
      IL_0006:  ret
    } // end of method ReadBookOnTablet::RunFromInventory

    .method public hidebysig virtual instance valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey 
            GetIconKey() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0006:  brfalse.s  IL_0014

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_000e:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetThumbnailKey()
      IL_0013:  ret

      IL_0014:  ldarg.0
      IL_0015:  call       instance valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetIconKey()
      IL_001a:  ret
    } // end of method ReadBookOnTablet::GetIconKey

    .method public hidebysig virtual instance void 
            PostureTransitionFailed(bool transitionExitReason) cil managed
    {
      // Code size       207 (0xcf)
      .maxstack  11
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0006:  brtrue.s   IL_001e

      IL_0008:  ldarg.0
      IL_0009:  ldarg.0
      IL_000a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_000f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
      IL_0014:  isinst     Sims3.Store.Objects.Tablet
      IL_0019:  stfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_001e:  ldarg.0
      IL_001f:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0024:  brfalse    IL_00ce

      IL_0029:  ldarg.0
      IL_002a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_002f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0034:  ldarg.0
      IL_0035:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_003a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_003f:  brtrue     IL_00ce

      IL_0044:  ldarg.0
      IL_0045:  call       instance bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::get_FromInventory()
      IL_004a:  brfalse.s  IL_0092

      IL_004c:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.PutInInventory::Singleton
      IL_0051:  ldarg.0
      IL_0052:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0057:  ldarg.0
      IL_0058:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_005d:  ldarg.0
      IL_005e:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0063:  ldc.i4.0
      IL_0064:  ldarg.0
      IL_0065:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_006a:  ldnull
      IL_006b:  ldnull
      IL_006c:  ldarg.0
      IL_006d:  ldftn      instance void Sims3.Store.Objects.Tablet/ReadBookOnTablet::PutInInventoryFailureCallback(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                    float32)
      IL_0073:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback::.ctor(object,
                                                                                                       native int)
      IL_0078:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstanceWithCallbacks(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                                     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                                     bool,
                                                                                                                                                                                                                     bool,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback)
      IL_007d:  stloc.0
      IL_007e:  ldarg.0
      IL_007f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0084:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0089:  ldloc.0
      IL_008a:  ldc.i4.1
      IL_008b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0090:  pop
      IL_0091:  ret

      IL_0092:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_0097:  ldarg.0
      IL_0098:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_009d:  ldarg.0
      IL_009e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_00a3:  ldarg.0
      IL_00a4:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_00a9:  ldarg.0
      IL_00aa:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_00af:  ldarg.0
      IL_00b0:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_00b5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_00ba:  stloc.1
      IL_00bb:  ldarg.0
      IL_00bc:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_00c1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_00c6:  ldloc.1
      IL_00c7:  ldc.i4.1
      IL_00c8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_00cd:  pop
      IL_00ce:  ret
    } // end of method ReadBookOnTablet::PostureTransitionFailed

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       99 (0x63)
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_HasBeenDestroyed()
      IL_000b:  brtrue.s   IL_0026

      IL_000d:  ldarg.0
      IL_000e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0013:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0018:  ldc.i8     0x4c80018653f16e0
      IL_0021:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::UnpauseBuff(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0026:  ldarg.0
      IL_0027:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_002c:  brfalse.s  IL_004c

      IL_002e:  ldarg.0
      IL_002f:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_0034:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
      IL_0039:  pop
      IL_003a:  ldarg.0
      IL_003b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_0040:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
      IL_0045:  ldarg.0
      IL_0046:  ldnull
      IL_0047:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_004c:  ldarg.0
      IL_004d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0052:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0057:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_005c:  ldarg.0
      IL_005d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBook::Cleanup()
      IL_0062:  ret
    } // end of method ReadBookOnTablet::Cleanup

    .method public hidebysig instance class [SimIFace]Sims3.SimIFace.StateMachineClient 
            GetDefaultStateMachine() cil managed
    {
      // Code size       174 (0xae)
      .maxstack  5
      .locals init (class [SimIFace]Sims3.SimIFace.StateMachineClient V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0006:  callvirt   instance class [SimIFace]Sims3.SimIFace.IScriptProxy [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_Proxy()
      IL_000b:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [SimIFace]Sims3.SimIFace.ITask::get_ObjectId()
      IL_0010:  ldstr      "Tablet_store"
      IL_0015:  call       class [SimIFace]Sims3.SimIFace.StateMachineClient [SimIFace]Sims3.SimIFace.StateMachineClient::Acquire(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                  string)
      IL_001a:  stloc.0
      IL_001b:  ldloc.0
      IL_001c:  ldstr      "x"
      IL_0021:  ldarg.0
      IL_0022:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0027:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_002c:  ldloc.0
      IL_002d:  ldstr      "tablet"
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0038:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_003d:  ldloc.0
      IL_003e:  ldstr      "isSitting"
      IL_0043:  ldarg.0
      IL_0044:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0049:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_004e:  ldc.i4     0x40f3d23
      IL_0053:  ldarg.0
      IL_0054:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0059:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_005e:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0063:  ldloc.0
      IL_0064:  ldstr      "inInventory"
      IL_0069:  ldarg.0
      IL_006a:  call       instance bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::get_FromInventory()
      IL_006f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0074:  ldloc.0
      IL_0075:  ldstr      "x"
      IL_007a:  ldstr      "Enter"
      IL_007f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_0084:  ldloc.0
      IL_0085:  ldc.i4.s   104
      IL_0087:  ldarg.0
      IL_0088:  ldftn      instance void Sims3.Store.Objects.Tablet/ReadBookOnTablet::eBookVideoOn(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                   class [SimIFace]Sims3.SimIFace.IEvent)
      IL_008e:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0093:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0098:  ldloc.0
      IL_0099:  ldc.i4.s   105
      IL_009b:  ldarg.0
      IL_009c:  ldftn      instance void Sims3.Store.Objects.Tablet/ReadBookOnTablet::eBookVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                    class [SimIFace]Sims3.SimIFace.IEvent)
      IL_00a2:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_00a7:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_00ac:  ldloc.0
      IL_00ad:  ret
    } // end of method ReadBookOnTablet::GetDefaultStateMachine

    .method public hidebysig instance float32 
            DummyTest(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance 'instance') cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldc.r4     0.0
      IL_0005:  ret
    } // end of method ReadBookOnTablet::DummyTest

    .method public hidebysig instance bool 
            DoReadBookOnTablet() cil managed
    {
      // Code size       455 (0x1c7)
      .maxstack  7
      .locals init (bool V_0,
               bool V_1,
               bool V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0006:  brtrue.s   IL_001a

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_000e:  ldc.i4     0x10000
      IL_0013:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0018:  ldc.i4.0
      IL_0019:  ret

      IL_001a:  ldarg.0
      IL_001b:  call       instance bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::get_FromInventory()
      IL_0020:  brfalse.s  IL_005d

      IL_0022:  ldarg.0
      IL_0023:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0028:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_002d:  ldarg.0
      IL_002e:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0033:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToRemove(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_0038:  brtrue.s   IL_004c

      IL_003a:  ldarg.0
      IL_003b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0040:  ldc.i4     0x400
      IL_0045:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_004a:  ldc.i4.0
      IL_004b:  ret

      IL_004c:  ldarg.0
      IL_004d:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0052:  ldarg.0
      IL_0053:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0058:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_005d:  ldarg.0
      IL_005e:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0063:  ldarg.0
      IL_0064:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0069:  ldarg.0
      IL_006a:  call       instance bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::get_FromInventory()
      IL_006f:  ldarg.0
      IL_0070:  ldarg.0
      IL_0071:  ldftn      instance class [SimIFace]Sims3.SimIFace.StateMachineClient Sims3.Store.Objects.Tablet/ReadBookOnTablet::GetDefaultStateMachine()
      IL_0077:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate::.ctor(object,
                                                                                                                                  native int)
      IL_007c:  ldarg.0
      IL_007d:  ldflda     class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0082:  callvirt   instance void Sims3.Store.Objects.Tablet::StartUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      bool,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate,
                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient&)
      IL_0087:  ldarg.0
      IL_0088:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_008d:  ldarg.0
      IL_008e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0093:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::get_BookId()
      IL_0098:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::HasBeenRead(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                               string)
      IL_009d:  brfalse.s  IL_00af

      IL_009f:  ldarg.0
      IL_00a0:  ldc.i4     0x10f38da
      IL_00a5:  ldsfld     float32 Sims3.Store.Objects.Tablet::kBookRepeatFunRate
      IL_00aa:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdate(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                                     float32)
      IL_00af:  ldarg.0
      IL_00b0:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_00b5:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookSkill
      IL_00ba:  brfalse.s  IL_00d9

      IL_00bc:  ldarg.0
      IL_00bd:  ldc.i4     0x10f38da
      IL_00c2:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::HasCommodityUpdate(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_00c7:  brfalse.s  IL_00d9

      IL_00c9:  ldarg.0
      IL_00ca:  ldc.i4     0x10f38da
      IL_00cf:  ldc.r4     0.0
      IL_00d4:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdate(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                                     float32)
      IL_00d9:  ldarg.0
      IL_00da:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_00df:  ldstr      "Respond to Book"
      IL_00e4:  ldnull
      IL_00e5:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Socializing.ActiveTopic::AddToSim(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                       string,
                                                                                                       class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription)
      IL_00ea:  ldarg.0
      IL_00eb:  ldc.i4.0
      IL_00ec:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry(bool)
      IL_00f1:  ldarg.0
      IL_00f2:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_00f7:  ldarg.0
      IL_00f8:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_00fd:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0102:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::DisableLookAts()
      IL_0107:  ldarg.0
      IL_0108:  call       instance bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::ReadLoop()
      IL_010d:  stloc.0
      IL_010e:  ldarg.0
      IL_010f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0114:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0119:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_011e:  ldarg.0
      IL_011f:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0124:  ldloc.0
      IL_0125:  ldarg.0
      IL_0126:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_012b:  ldarg.0
      IL_012c:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::ReadPostLoop(bool,
                                                                                                         class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                         class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_0131:  ldarg.0
      IL_0132:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0137:  ldc.i8     0xb82d0015b9294190
      IL_0140:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_0145:  brfalse.s  IL_017f

      IL_0147:  ldarg.0
      IL_0148:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_014d:  callvirt   instance class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBookData> [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_ReadBookDataList()
      IL_0152:  brfalse.s  IL_017f

      IL_0154:  ldarg.0
      IL_0155:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_015a:  callvirt   instance class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBookData> [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_ReadBookDataList()
      IL_015f:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBookData>::get_Count()
      IL_0164:  ldsfld     int32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitTipsManager::kBooksReadCountToShowBookwormTraitTip
      IL_0169:  blt.s      IL_017f

      IL_016b:  ldc.i8     0xb82d0015b9294190
      IL_0174:  ldarg.0
      IL_0175:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_017a:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitTipsManager::ShowTraitTip(uint64,
                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_017f:  ldarg.0
      IL_0180:  ldc.i4.1
      IL_0181:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_0186:  ldarg.0
      IL_0187:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_018c:  ldarg.0
      IL_018d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0192:  ldarg.0
      IL_0193:  call       instance bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::get_FromInventory()
      IL_0198:  callvirt   instance bool Sims3.Store.Objects.Tablet::ShouldPutInInventory(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                          bool)
      IL_019d:  stloc.1
      IL_019e:  ldarg.0
      IL_019f:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_01a4:  ldarg.0
      IL_01a5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_01aa:  ldarg.0
      IL_01ab:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_01b0:  ldarg.0
      IL_01b1:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_01b6:  ldloc.1
      IL_01b7:  callvirt   instance bool Sims3.Store.Objects.Tablet::StopUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book,
                                                                                     class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                     bool)
      IL_01bc:  stloc.2
      IL_01bd:  ldarg.0
      IL_01be:  ldc.i4.0
      IL_01bf:  ldc.i4.0
      IL_01c0:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_01c5:  ldloc.2
      IL_01c6:  ret
    } // end of method ReadBookOnTablet::DoReadBookOnTablet

    .method public hidebysig instance bool 
            ReadLoop() cil managed
    {
      // Code size       168 (0xa8)
      .maxstack  5
      .locals init (bool V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldstr      "ReadTablet"
      IL_0006:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_000b:  ldc.i4.0
      IL_000c:  stloc.0
      IL_000d:  ldarg.0
      IL_000e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0013:  ldarg.0
      IL_0014:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0019:  ldarg.0
      IL_001a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::ReadPreLoop(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance)
      IL_001f:  brfalse    IL_00a6

      IL_0024:  ldarg.0
      IL_0025:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_002a:  ldarg.0
      IL_002b:  ldarg.0
      IL_002c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0031:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_0036:  ldc.i8     0xb82d0015b9294190
      IL_003f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_0044:  stfld      bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::isBookWorm
      IL_0049:  ldarg.0
      IL_004a:  ldfld      bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBook::ExitAfterFinishingBookForFirstTimeOrMaxingSkill
      IL_004f:  brfalse.s  IL_0086

      IL_0051:  ldarg.0
      IL_0052:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0057:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookSkill
      IL_005c:  brtrue.s   IL_006b

      IL_005e:  ldarg.0
      IL_005f:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0064:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.BookRecipe
      IL_0069:  brfalse.s  IL_0086

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4     0xffa9ffff
      IL_0071:  ldarg.0
      IL_0072:  ldftn      instance void Sims3.Store.Objects.Tablet/ReadBookOnTablet::LoopDel(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                              valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_0078:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_007d:  ldnull
      IL_007e:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_0083:  stloc.0
      IL_0084:  br.s       IL_009f

      IL_0086:  ldarg.0
      IL_0087:  ldc.i4     0xffa9ffff
      IL_008c:  ldarg.0
      IL_008d:  ldftn      instance void Sims3.Store.Objects.Tablet/ReadBookOnTablet::LoopDel(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                              valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_0093:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_0098:  ldnull
      IL_0099:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient)
      IL_009e:  stloc.0
      IL_009f:  ldarg.0
      IL_00a0:  ldloc.0
      IL_00a1:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_00a6:  ldloc.0
      IL_00a7:  ret
    } // end of method ReadBookOnTablet::ReadLoop

    .method public hidebysig instance void 
            LoopDel(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData loopData) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Target
      IL_0006:  ldarg.1
      IL_0007:  ldarg.0
      IL_0008:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_000d:  ldarg.0
      IL_000e:  ldfld      bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::isBookWorm
      IL_0013:  ldarga.s   loopData
      IL_0015:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData::mDeltaTime
      IL_001a:  ldarga.s   loopData
      IL_001c:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData::mLifeTime
      IL_0021:  ldarg.0
      IL_0022:  ldfld      bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBook::ExitAfterFinishingBookForFirstTimeOrMaxingSkill
      IL_0027:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::LoopDel(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                    class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                    bool,
                                                                                                    float32,
                                                                                                    float32,
                                                                                                    bool)
      IL_002c:  ret
    } // end of method ReadBookOnTablet::LoopDel

    .method public hidebysig instance void 
            eBookVideoOn(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                         class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       73 (0x49)
      .maxstack  3
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_0006:  brfalse.s  IL_0010

      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  ldarg.2
      IL_000b:  call       instance void Sims3.Store.Objects.Tablet/ReadBookOnTablet::eBookVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                    class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0010:  ldarg.0
      IL_0011:  ldstr      "store_tablet_read"
      IL_0016:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
      IL_001b:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_0026:  ldarg.0
      IL_0027:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_002c:  ldstr      "_FX_0_screen"
      IL_0031:  call       valuetype [SimIFace]Sims3.SimIFace.Slot [SimIFace]Sims3.SimIFace.Slots::Hash(string)
      IL_0036:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                         valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_003b:  pop
      IL_003c:  ldarg.0
      IL_003d:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_0042:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
      IL_0047:  pop
      IL_0048:  ret
    } // end of method ReadBookOnTablet::eBookVideoOn

    .method public hidebysig instance void 
            eBookVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                          class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       39 (0x27)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_0006:  brfalse.s  IL_0026

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
      IL_0013:  pop
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
      IL_001f:  ldarg.0
      IL_0020:  ldnull
      IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ReadBookOnTablet::mScreenVFX
      IL_0026:  ret
    } // end of method ReadBookOnTablet::eBookVideoOff

    .method public hidebysig instance void 
            PutInInventoryFailureCallback(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim s,
                                          float32 f) cil managed
    {
      // Code size       99 (0x63)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0006:  brtrue.s   IL_001e

      IL_0008:  ldarg.0
      IL_0009:  ldarg.0
      IL_000a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_000f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
      IL_0014:  isinst     Sims3.Store.Objects.Tablet
      IL_0019:  stfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_001e:  ldarg.0
      IL_001f:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0024:  brfalse.s  IL_0062

      IL_0026:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_002b:  ldarg.0
      IL_002c:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ReadBookOnTablet::mTablet
      IL_0031:  ldarg.0
      IL_0032:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0037:  ldarg.0
      IL_0038:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_003d:  ldarg.0
      IL_003e:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0043:  ldarg.0
      IL_0044:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_0049:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_004e:  stloc.0
      IL_004f:  ldarg.0
      IL_0050:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Actor
      IL_0055:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_005a:  ldloc.0
      IL_005b:  ldc.i4.1
      IL_005c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0061:  pop
      IL_0062:  ret
    } // end of method ReadBookOnTablet::PutInInventoryFailureCallback

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.ReadBook::.ctor()
      IL_0006:  ret
    } // end of method ReadBookOnTablet::.ctor

    .property instance bool FromInventory()
    {
      .get instance bool Sims3.Store.Objects.Tablet/ReadBookOnTablet::get_FromInventory()
    } // end of property ReadBookOnTablet::FromInventory
  } // end of class ReadBookOnTablet

  .class auto ansi nested public beforefieldinit PickUpTabletForShopping
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForShopping>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       47 (0x2f)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  bne.un.s   IL_000b

        IL_0009:  ldc.i4.0
        IL_000a:  ret

        IL_000b:  call       uint32 [Sims3GameplaySystems]Sims3.Gameplay.Queries::CountObjects<class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore>()
        IL_0010:  ldc.i4.0
        IL_0011:  bgt.un.s   IL_002d

        IL_0013:  ldarg.s    greyedOutTooltipCallback
        IL_0015:  ldstr      "NoBookstores"
        IL_001a:  ldc.i4.0
        IL_001b:  newarr     [mscorlib]System.Object
        IL_0020:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0025:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_002a:  stind.ref
        IL_002b:  ldc.i4.0
        IL_002c:  ret

        IL_002d:  ldc.i4.1
        IL_002e:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "ShopForBooks"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForShopping>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PickUp(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable)
      IL_0011:  brtrue.s   IL_0015

      IL_0013:  ldc.i4.0
      IL_0014:  ret

      IL_0015:  ldarg.0
      IL_0016:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForShopping::PushShopInteraction()
      IL_001b:  ret
    } // end of method PickUpTabletForShopping::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForShopping::PushShopInteraction()
      IL_0006:  ret
    } // end of method PickUpTabletForShopping::RunFromInventory

    .method public hidebysig virtual instance bool 
            TargetInventoryChangeIsValid(class [Sims3GameplaySystems]Sims3.Gameplay.Inventory newInventory) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brfalse.s  IL_0011

      IL_0003:  ldarg.1
      IL_0004:  ldarg.0
      IL_0005:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_000f:  bne.un.s   IL_0013

      IL_0011:  ldc.i4.1
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method PickUpTabletForShopping::TargetInventoryChangeIsValid

    .method public hidebysig instance bool 
            PushShopInteraction() cil managed
    {
      // Code size       72 (0x48)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
      IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::Singleton
      IL_0005:  ldarg.0
      IL_0006:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000b:  ldarg.0
      IL_000c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0011:  ldarg.0
      IL_0012:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0017:  ldarg.0
      IL_0018:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_001d:  ldc.i4.1
      IL_001e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0023:  stloc.0
      IL_0024:  ldarg.0
      IL_0025:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_002a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_002f:  ldloc.0
      IL_0030:  ldc.i4.0
      IL_0031:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0036:  brtrue.s   IL_0046

      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003e:  ldc.i4.1
      IL_003f:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PutDown(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SurfaceType)
      IL_0044:  ldc.i4.0
      IL_0045:  ret

      IL_0046:  ldc.i4.1
      IL_0047:  ret
    } // end of method PickUpTabletForShopping::PushShopInteraction

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method PickUpTabletForShopping::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/PickUpTabletForShopping/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PickUpTabletForShopping::Singleton
      IL_000a:  ret
    } // end of method PickUpTabletForShopping::.cctor

  } // end of class PickUpTabletForShopping

  .class auto ansi nested public beforefieldinit ShopForBooksOnTablet
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/ShopForBooksOnTablet>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       61 (0x3d)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  beq.s      IL_0019

        IL_0009:  ldarg.1
        IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
        IL_000f:  ldarg.2
        IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
        IL_0015:  brtrue.s   IL_0019

        IL_0017:  ldc.i4.0
        IL_0018:  ret

        IL_0019:  call       uint32 [Sims3GameplaySystems]Sims3.Gameplay.Queries::CountObjects<class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore>()
        IL_001e:  ldc.i4.0
        IL_001f:  bgt.un.s   IL_003b

        IL_0021:  ldarg.s    greyedOutTooltipCallback
        IL_0023:  ldstr      "NoBookstores"
        IL_0028:  ldc.i4.0
        IL_0029:  newarr     [mscorlib]System.Object
        IL_002e:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0033:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
        IL_0038:  stind.ref
        IL_0039:  ldc.i4.0
        IL_003a:  ret

        IL_003b:  ldc.i4.1
        IL_003c:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "ShopForBooks"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/ShopForBooksOnTablet>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public bool mFromInventory
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  3
      .locals init (class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore[] V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  stfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_0007:  call       !!0[] [Sims3GameplaySystems]Sims3.Gameplay.Queries::GetObjects<class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore>()
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  ldlen
      IL_000f:  conv.i4
      IL_0010:  ldc.i4.0
      IL_0011:  bgt.s      IL_0015

      IL_0013:  ldc.i4.0
      IL_0014:  ret

      IL_0015:  ldarg.0
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.0
      IL_0018:  ldelem.ref
      IL_0019:  call       instance bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::DoShopForBooksOnTablet(class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore)
      IL_001e:  ret
    } // end of method ShopForBooksOnTablet::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  3
      .locals init (class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore[] V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  stfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_0007:  call       !!0[] [Sims3GameplaySystems]Sims3.Gameplay.Queries::GetObjects<class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore>()
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  ldlen
      IL_000f:  conv.i4
      IL_0010:  ldc.i4.0
      IL_0011:  bgt.s      IL_0015

      IL_0013:  ldc.i4.0
      IL_0014:  ret

      IL_0015:  ldarg.0
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.0
      IL_0018:  ldelem.ref
      IL_0019:  call       instance bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::DoShopForBooksOnTablet(class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore)
      IL_001e:  ret
    } // end of method ShopForBooksOnTablet::RunFromInventory

    .method public hidebysig virtual instance void 
            PostureTransitionFailed(bool transitionExitReason) cil managed
    {
      // Code size       175 (0xaf)
      .maxstack  11
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0006:  brtrue.s   IL_0009

      IL_0008:  ret

      IL_0009:  ldarg.0
      IL_000a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0014:  ldarg.0
      IL_0015:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_001a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_001f:  brtrue     IL_00ae

      IL_0024:  ldarg.0
      IL_0025:  ldfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_002a:  brfalse.s  IL_0072

      IL_002c:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.PutInInventory::Singleton
      IL_0031:  ldarg.0
      IL_0032:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0037:  ldarg.0
      IL_0038:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003d:  ldarg.0
      IL_003e:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0043:  ldc.i4.0
      IL_0044:  ldarg.0
      IL_0045:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_004a:  ldnull
      IL_004b:  ldnull
      IL_004c:  ldarg.0
      IL_004d:  ldftn      instance void Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::PutInInventoryFailureCallback(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                        float32)
      IL_0053:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback::.ctor(object,
                                                                                                       native int)
      IL_0058:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstanceWithCallbacks(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                                     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                                     bool,
                                                                                                                                                                                                                     bool,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback)
      IL_005d:  stloc.0
      IL_005e:  ldarg.0
      IL_005f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0064:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0069:  ldloc.0
      IL_006a:  ldc.i4.1
      IL_006b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0070:  pop
      IL_0071:  ret

      IL_0072:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_0077:  ldarg.0
      IL_0078:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_007d:  ldarg.0
      IL_007e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0083:  ldarg.0
      IL_0084:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0089:  ldarg.0
      IL_008a:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_008f:  ldarg.0
      IL_0090:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_0095:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_009a:  stloc.1
      IL_009b:  ldarg.0
      IL_009c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00a1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_00a6:  ldloc.1
      IL_00a7:  ldc.i4.1
      IL_00a8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_00ad:  pop
      IL_00ae:  ret
    } // end of method ShopForBooksOnTablet::PostureTransitionFailed

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       61 (0x3d)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_HasBeenDestroyed()
      IL_000b:  brtrue.s   IL_0026

      IL_000d:  ldarg.0
      IL_000e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0013:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0018:  ldc.i8     0x4c80018653f16e0
      IL_0021:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::UnpauseBuff(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0026:  ldarg.0
      IL_0027:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_002c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0031:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_0036:  ldarg.0
      IL_0037:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_003c:  ret
    } // end of method ShopForBooksOnTablet::Cleanup

    .method public hidebysig instance class [SimIFace]Sims3.SimIFace.StateMachineClient 
            GetDefaultStateMachine() cil managed
    {
      // Code size       134 (0x86)
      .maxstack  5
      .locals init (class [SimIFace]Sims3.SimIFace.StateMachineClient V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance class [SimIFace]Sims3.SimIFace.IScriptProxy [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_Proxy()
      IL_000b:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [SimIFace]Sims3.SimIFace.ITask::get_ObjectId()
      IL_0010:  ldstr      "Tablet_store"
      IL_0015:  call       class [SimIFace]Sims3.SimIFace.StateMachineClient [SimIFace]Sims3.SimIFace.StateMachineClient::Acquire(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                  string)
      IL_001a:  stloc.0
      IL_001b:  ldloc.0
      IL_001c:  ldstr      "x"
      IL_0021:  ldarg.0
      IL_0022:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0027:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_002c:  ldloc.0
      IL_002d:  ldstr      "tablet"
      IL_0032:  ldarg.0
      IL_0033:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0038:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_003d:  ldloc.0
      IL_003e:  ldstr      "isSitting"
      IL_0043:  ldarg.0
      IL_0044:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0049:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_004e:  ldc.i4     0x40f3d23
      IL_0053:  ldarg.0
      IL_0054:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0059:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_005e:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0063:  ldloc.0
      IL_0064:  ldstr      "inInventory"
      IL_0069:  ldarg.0
      IL_006a:  ldfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_006f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0074:  ldloc.0
      IL_0075:  ldstr      "x"
      IL_007a:  ldstr      "Enter"
      IL_007f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_0084:  ldloc.0
      IL_0085:  ret
    } // end of method ShopForBooksOnTablet::GetDefaultStateMachine

    .method public hidebysig instance bool 
            DoShopForBooksOnTablet(class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore bookstore) cil managed
    {
      // Code size       323 (0x143)
      .maxstack  7
      .locals init (bool V_0,
               bool V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_0006:  brfalse.s  IL_0045

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0013:  ldarg.0
      IL_0014:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0019:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToRemove(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_001e:  brtrue.s   IL_0032

      IL_0020:  ldarg.0
      IL_0021:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0026:  ldc.i4     0x400
      IL_002b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0030:  ldc.i4.0
      IL_0031:  ret

      IL_0032:  ldarg.0
      IL_0033:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0043:  br.s       IL_006a

      IL_0045:  ldarg.0
      IL_0046:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_004b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
      IL_0050:  ldarg.0
      IL_0051:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0056:  beq.s      IL_006a

      IL_0058:  ldarg.0
      IL_0059:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_005e:  ldc.i4     0x10000
      IL_0063:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0068:  ldc.i4.0
      IL_0069:  ret

      IL_006a:  ldarg.0
      IL_006b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0070:  ldarg.0
      IL_0071:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0076:  ldarg.0
      IL_0077:  ldfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_007c:  ldarg.0
      IL_007d:  ldarg.0
      IL_007e:  ldftn      instance class [SimIFace]Sims3.SimIFace.StateMachineClient Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::GetDefaultStateMachine()
      IL_0084:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate::.ctor(object,
                                                                                                                                  native int)
      IL_0089:  ldarg.0
      IL_008a:  ldflda     class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_008f:  callvirt   instance void Sims3.Store.Objects.Tablet::StartUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      bool,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate,
                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient&)
      IL_0094:  ldarg.0
      IL_0095:  ldc.i4.0
      IL_0096:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry(bool)
      IL_009b:  ldarg.0
      IL_009c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00a7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_00ac:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::DisableLookAts()
      IL_00b1:  ldarg.0
      IL_00b2:  ldstr      "Shop"
      IL_00b7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_00bc:  ldarg.0
      IL_00bd:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_00c2:  ldarg.0
      IL_00c3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00c8:  ldarg.1
      IL_00c9:  callvirt   instance bool Sims3.Store.Objects.Tablet::ShowShoppingDialog(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                        class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore)
      IL_00ce:  pop
      IL_00cf:  ldarg.0
      IL_00d0:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00d5:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_00da:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_00df:  ldarg.0
      IL_00e0:  ldc.i4.1
      IL_00e1:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_00e6:  ldarg.0
      IL_00e7:  ldarg.0
      IL_00e8:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_00ed:  ldarg.0
      IL_00ee:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00f3:  ldarg.0
      IL_00f4:  ldfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_00f9:  callvirt   instance bool Sims3.Store.Objects.Tablet::ShouldPutInInventory(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                          bool)
      IL_00fe:  stfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_0103:  ldarg.0
      IL_0104:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0109:  ldarg.0
      IL_010a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_010f:  ldnull
      IL_0110:  ldarg.0
      IL_0111:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0116:  ldarg.0
      IL_0117:  ldfld      bool Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::mFromInventory
      IL_011c:  callvirt   instance bool Sims3.Store.Objects.Tablet::StopUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book,
                                                                                     class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                     bool)
      IL_0121:  stloc.0
      IL_0122:  ldarg.0
      IL_0123:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0128:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
      IL_012d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Count()
      IL_0132:  ldc.i4.0
      IL_0133:  ceq
      IL_0135:  ldc.i4.0
      IL_0136:  ceq
      IL_0138:  stloc.1
      IL_0139:  ldarg.0
      IL_013a:  ldloc.1
      IL_013b:  ldloc.1
      IL_013c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_0141:  ldloc.0
      IL_0142:  ret
    } // end of method ShopForBooksOnTablet::DoShopForBooksOnTablet

    .method public hidebysig instance void 
            PutInInventoryFailureCallback(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim s,
                                          float32 f) cil managed
    {
      // Code size       70 (0x46)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0006:  brtrue.s   IL_0009

      IL_0008:  ret

      IL_0009:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_000e:  ldarg.0
      IL_000f:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0014:  ldarg.0
      IL_0015:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_001a:  ldarg.0
      IL_001b:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0020:  ldarg.0
      IL_0021:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0026:  ldarg.0
      IL_0027:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_002c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0031:  stloc.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0038:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_003d:  ldloc.0
      IL_003e:  ldc.i4.1
      IL_003f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0044:  pop
      IL_0045:  ret
    } // end of method ShopForBooksOnTablet::PutInInventoryFailureCallback

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method ShopForBooksOnTablet::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/ShopForBooksOnTablet/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::Singleton
      IL_000a:  ret
    } // end of method ShopForBooksOnTablet::.cctor

  } // end of class ShopForBooksOnTablet

  .class auto ansi nested public beforefieldinit PickUpTabletForAudio
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForAudio>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       37 (0x25)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  bne.un.s   IL_000b

        IL_0009:  ldc.i4.0
        IL_000a:  ret

        IL_000b:  ldarg.1
        IL_000c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
        IL_0011:  ldc.i8     0x9a7f5f1919df86c1
        IL_001a:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::HasElement(uint64)
        IL_001f:  brfalse.s  IL_0023

        IL_0021:  ldc.i4.0
        IL_0022:  ret

        IL_0023:  ldc.i4.1
        IL_0024:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "ListenToAudioProgram"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance void 
              PopulatePieMenuPicker(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters& parameters,
                                    [out] class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>& listObjs,
                                    [out] class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>& headers,
                                    [out] int32& NumSelectableRows) cil managed
      {
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters::get_Actor()
        IL_0006:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim
        IL_000b:  ldarg.1
        IL_000c:  ldarg.2
        IL_000d:  ldarg.3
        IL_000e:  ldarg.s    NumSelectableRows
        IL_0010:  call       void Sims3.Store.Objects.Tablet::PopulateAudioPrograms(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters&,
                                                                                    class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>&,
                                                                                    class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>&,
                                                                                    int32&)
        IL_0015:  ret
      } // end of method Definition::PopulatePieMenuPicker

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForAudio>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PickUp(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable)
      IL_0011:  brtrue.s   IL_0015

      IL_0013:  ldc.i4.0
      IL_0014:  ret

      IL_0015:  ldarg.0
      IL_0016:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForAudio::PushAudioInteraction()
      IL_001b:  ret
    } // end of method PickUpTabletForAudio::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForAudio::PushAudioInteraction()
      IL_0006:  ret
    } // end of method PickUpTabletForAudio::RunFromInventory

    .method public hidebysig virtual instance bool 
            TargetInventoryChangeIsValid(class [Sims3GameplaySystems]Sims3.Gameplay.Inventory newInventory) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brfalse.s  IL_0011

      IL_0003:  ldarg.1
      IL_0004:  ldarg.0
      IL_0005:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_000f:  bne.un.s   IL_0013

      IL_0011:  ldc.i4.1
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method PickUpTabletForAudio::TargetInventoryChangeIsValid

    .method public hidebysig instance bool 
            PushAudioInteraction() cil managed
    {
      // Code size       168 (0xa8)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0,
               class Sims3.Store.Objects.Tablet/ListenToAudioProgram V_1,
               object V_2,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_3,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_4)
      IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ListenToAudioProgram::Singleton
      IL_0005:  ldarg.0
      IL_0006:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000b:  ldarg.0
      IL_000c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0011:  ldarg.0
      IL_0012:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0017:  ldarg.0
      IL_0018:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_001d:  ldc.i4.1
      IL_001e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  isinst     Sims3.Store.Objects.Tablet/ListenToAudioProgram
      IL_002a:  stloc.1
      IL_002b:  ldarg.0
      IL_002c:  call       instance object [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetSelectedObject()
      IL_0031:  stloc.2
      IL_0032:  ldloc.2
      IL_0033:  brfalse.s  IL_0043

      IL_0035:  ldloc.1
      IL_0036:  ldloc.2
      IL_0037:  unbox.any  [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
      IL_003c:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0041:  br.s       IL_007a

      IL_0043:  ldarg.0
      IL_0044:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0049:  brfalse.s  IL_0078

      IL_004b:  ldc.i4.3
      IL_004c:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
      IL_0051:  stloc.s    V_4
      IL_0053:  ldloc.s    V_4
      IL_0055:  ldc.i4.0
      IL_0056:  ldc.i4.8
      IL_0057:  conv.i8
      IL_0058:  stelem.i8
      IL_0059:  ldloc.s    V_4
      IL_005b:  ldc.i4.1
      IL_005c:  ldc.i4.s   12
      IL_005e:  conv.i8
      IL_005f:  stelem.i8
      IL_0060:  ldloc.s    V_4
      IL_0062:  ldc.i4.2
      IL_0063:  ldc.i4.s   21
      IL_0065:  conv.i8
      IL_0066:  stelem.i8
      IL_0067:  ldloc.s    V_4
      IL_0069:  stloc.3
      IL_006a:  ldloc.1
      IL_006b:  ldloc.3
      IL_006c:  call       !!0 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetRandomObjectFromList<valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames>(!!0[])
      IL_0071:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0076:  br.s       IL_007a

      IL_0078:  ldc.i4.0
      IL_0079:  ret

      IL_007a:  ldloc.1
      IL_007b:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0080:  ldc.i4.0
      IL_0081:  conv.i8
      IL_0082:  beq.s      IL_0098

      IL_0084:  ldarg.0
      IL_0085:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_008a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_008f:  ldloc.0
      IL_0090:  ldc.i4.0
      IL_0091:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0096:  brtrue.s   IL_00a6

      IL_0098:  ldarg.0
      IL_0099:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_009e:  ldc.i4.1
      IL_009f:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PutDown(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SurfaceType)
      IL_00a4:  ldc.i4.0
      IL_00a5:  ret

      IL_00a6:  ldc.i4.1
      IL_00a7:  ret
    } // end of method PickUpTabletForAudio::PushAudioInteraction

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method PickUpTabletForAudio::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/PickUpTabletForAudio/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PickUpTabletForAudio::Singleton
      IL_000a:  ret
    } // end of method PickUpTabletForAudio::.cctor

  } // end of class PickUpTabletForAudio

  .class auto ansi nested public beforefieldinit ListenToAudioProgram
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/ListenToAudioProgram>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       83 (0x53)
        .maxstack  5
        .locals init (valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters V_0,
                 class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo> V_1,
                 class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo> V_2,
                 int32 V_3)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  beq.s      IL_0019

        IL_0009:  ldarg.1
        IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
        IL_000f:  ldarg.2
        IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
        IL_0015:  brtrue.s   IL_0019

        IL_0017:  ldc.i4.0
        IL_0018:  ret

        IL_0019:  ldarg.1
        IL_001a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
        IL_001f:  ldc.i8     0x9a7f5f1919df86c1
        IL_0028:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::HasElement(uint64)
        IL_002d:  brfalse.s  IL_0031

        IL_002f:  ldc.i4.0
        IL_0030:  ret

        IL_0031:  ldloca.s   V_0
        IL_0033:  initobj    [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters
        IL_0039:  ldarg.1
        IL_003a:  ldloca.s   V_0
        IL_003c:  ldloca.s   V_1
        IL_003e:  ldloca.s   V_2
        IL_0040:  ldloca.s   V_3
        IL_0042:  call       void Sims3.Store.Objects.Tablet::PopulateAudioPrograms(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters&,
                                                                                    class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>&,
                                                                                    class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>&,
                                                                                    int32&)
        IL_0047:  ldloc.1
        IL_0048:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>::get_Count()
        IL_004d:  brtrue.s   IL_0051

        IL_004f:  ldc.i4.0
        IL_0050:  ret

        IL_0051:  ldc.i4.1
        IL_0052:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "ListenToAudioProgram"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig virtual instance void 
              PopulatePieMenuPicker(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters& parameters,
                                    [out] class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>& listObjs,
                                    [out] class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>& headers,
                                    [out] int32& NumSelectableRows) cil managed
      {
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters::get_Actor()
        IL_0006:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim
        IL_000b:  ldarg.1
        IL_000c:  ldarg.2
        IL_000d:  ldarg.3
        IL_000e:  ldarg.s    NumSelectableRows
        IL_0010:  call       void Sims3.Store.Objects.Tablet::PopulateAudioPrograms(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters&,
                                                                                    class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>&,
                                                                                    class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>&,
                                                                                    int32&)
        IL_0015:  ret
      } // end of method Definition::PopulatePieMenuPicker

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/ListenToAudioProgram>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public bool mFromInventory
    .field public valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames mSkill
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig specialname 
            instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames 
            get_SkillProgram() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0006:  ret
    } // end of method ListenToAudioProgram::get_SkillProgram

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       111 (0x6f)
      .maxstack  3
      .locals init (valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_0,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0006:  ldc.i4.0
      IL_0007:  conv.i8
      IL_0008:  bne.un.s   IL_0061

      IL_000a:  ldarg.0
      IL_000b:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0010:  brfalse.s  IL_003a

      IL_0012:  ldc.i4.3
      IL_0013:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
      IL_0018:  stloc.1
      IL_0019:  ldloc.1
      IL_001a:  ldc.i4.0
      IL_001b:  ldc.i4.8
      IL_001c:  conv.i8
      IL_001d:  stelem.i8
      IL_001e:  ldloc.1
      IL_001f:  ldc.i4.1
      IL_0020:  ldc.i4.s   12
      IL_0022:  conv.i8
      IL_0023:  stelem.i8
      IL_0024:  ldloc.1
      IL_0025:  ldc.i4.2
      IL_0026:  ldc.i4.s   21
      IL_0028:  conv.i8
      IL_0029:  stelem.i8
      IL_002a:  ldloc.1
      IL_002b:  stloc.0
      IL_002c:  ldarg.0
      IL_002d:  ldloc.0
      IL_002e:  call       !!0 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetRandomObjectFromList<valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames>(!!0[])
      IL_0033:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0038:  br.s       IL_0061

      IL_003a:  ldarg.0
      IL_003b:  call       instance object [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetSelectedObject()
      IL_0040:  brtrue.s   IL_0044

      IL_0042:  ldc.i4.0
      IL_0043:  ret

      IL_0044:  ldarg.0
      IL_0045:  ldarg.0
      IL_0046:  call       instance object [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetSelectedObject()
      IL_004b:  unbox.any  [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
      IL_0050:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0055:  ldarg.0
      IL_0056:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_005b:  ldc.i4.0
      IL_005c:  conv.i8
      IL_005d:  bne.un.s   IL_0061

      IL_005f:  ldc.i4.0
      IL_0060:  ret

      IL_0061:  ldarg.0
      IL_0062:  ldc.i4.0
      IL_0063:  stfld      bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::mFromInventory
      IL_0068:  ldarg.0
      IL_0069:  call       instance bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::DoListenToAudioProgram()
      IL_006e:  ret
    } // end of method ListenToAudioProgram::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       108 (0x6c)
      .maxstack  3
      .locals init (object V_0,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_1,
               valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0006:  ldc.i4.0
      IL_0007:  conv.i8
      IL_0008:  bne.un.s   IL_005e

      IL_000a:  ldarg.0
      IL_000b:  call       instance object [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetSelectedObject()
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brfalse.s  IL_002c

      IL_0014:  ldarg.0
      IL_0015:  ldloc.0
      IL_0016:  unbox.any  [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
      IL_001b:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0020:  ldarg.0
      IL_0021:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_0026:  ldc.i4.0
      IL_0027:  conv.i8
      IL_0028:  bne.un.s   IL_005e

      IL_002a:  ldc.i4.0
      IL_002b:  ret

      IL_002c:  ldarg.0
      IL_002d:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0032:  brfalse.s  IL_005c

      IL_0034:  ldc.i4.3
      IL_0035:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
      IL_003a:  stloc.2
      IL_003b:  ldloc.2
      IL_003c:  ldc.i4.0
      IL_003d:  ldc.i4.8
      IL_003e:  conv.i8
      IL_003f:  stelem.i8
      IL_0040:  ldloc.2
      IL_0041:  ldc.i4.1
      IL_0042:  ldc.i4.s   12
      IL_0044:  conv.i8
      IL_0045:  stelem.i8
      IL_0046:  ldloc.2
      IL_0047:  ldc.i4.2
      IL_0048:  ldc.i4.s   21
      IL_004a:  conv.i8
      IL_004b:  stelem.i8
      IL_004c:  ldloc.2
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldloc.1
      IL_0050:  call       !!0 [Sims3GameplaySystems]Sims3.Gameplay.Core.RandomUtil::GetRandomObjectFromList<valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames>(!!0[])
      IL_0055:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::mSkill
      IL_005a:  br.s       IL_005e

      IL_005c:  ldc.i4.0
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldc.i4.1
      IL_0060:  stfld      bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::mFromInventory
      IL_0065:  ldarg.0
      IL_0066:  call       instance bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::DoListenToAudioProgram()
      IL_006b:  ret
    } // end of method ListenToAudioProgram::RunFromInventory

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       61 (0x3d)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_HasBeenDestroyed()
      IL_000b:  brtrue.s   IL_0026

      IL_000d:  ldarg.0
      IL_000e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0013:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0018:  ldc.i8     0x4c80018653f16e0
      IL_0021:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::UnpauseBuff(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0026:  ldarg.0
      IL_0027:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_002c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0031:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_0036:  ldarg.0
      IL_0037:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_003c:  ret
    } // end of method ListenToAudioProgram::Cleanup

    .method public hidebysig instance class [SimIFace]Sims3.SimIFace.StateMachineClient 
            GetDefaultStateMachine() cil managed
    {
      // Code size       154 (0x9a)
      .maxstack  5
      .locals init (class [SimIFace]Sims3.SimIFace.StateMachineClient V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance class [SimIFace]Sims3.SimIFace.IScriptProxy [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_Proxy()
      IL_000b:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [SimIFace]Sims3.SimIFace.ITask::get_ObjectId()
      IL_0010:  ldstr      "Tablet_store"
      IL_0015:  call       class [SimIFace]Sims3.SimIFace.StateMachineClient [SimIFace]Sims3.SimIFace.StateMachineClient::Acquire(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                  string)
      IL_001a:  stloc.0
      IL_001b:  ldloc.0
      IL_001c:  ldstr      "x"
      IL_0021:  ldarg.0
      IL_0022:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0027:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_002c:  ldloc.0
      IL_002d:  ldstr      "tablet"
      IL_0032:  ldarg.0
      IL_0033:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0038:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_003d:  ldloc.0
      IL_003e:  ldstr      "isSitting"
      IL_0043:  ldarg.0
      IL_0044:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0049:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_004e:  ldc.i4     0x40f3d23
      IL_0053:  ldarg.0
      IL_0054:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0059:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_005e:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0063:  ldloc.0
      IL_0064:  ldstr      "inInventory"
      IL_0069:  ldarg.0
      IL_006a:  ldfld      bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::mFromInventory
      IL_006f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0074:  ldloc.0
      IL_0075:  ldstr      "x"
      IL_007a:  ldstr      "Enter"
      IL_007f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_0084:  ldloc.0
      IL_0085:  ldc.i4.s   101
      IL_0087:  ldarg.0
      IL_0088:  ldftn      instance void Sims3.Store.Objects.Tablet/ListenToAudioProgram::StartProgram(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                       class [SimIFace]Sims3.SimIFace.IEvent)
      IL_008e:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0093:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0098:  ldloc.0
      IL_0099:  ret
    } // end of method ListenToAudioProgram::GetDefaultStateMachine

    .method public hidebysig instance bool 
            DoListenToAudioProgram() cil managed
    {
      // Code size       304 (0x130)
      .maxstack  7
      .locals init (bool V_0,
               bool V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::mFromInventory
      IL_0006:  brfalse.s  IL_0045

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0013:  ldarg.0
      IL_0014:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0019:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToRemove(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_001e:  brtrue.s   IL_0032

      IL_0020:  ldarg.0
      IL_0021:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0026:  ldc.i4     0x400
      IL_002b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0030:  ldc.i4.0
      IL_0031:  ret

      IL_0032:  ldarg.0
      IL_0033:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0043:  br.s       IL_006a

      IL_0045:  ldarg.0
      IL_0046:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_004b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
      IL_0050:  ldarg.0
      IL_0051:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0056:  beq.s      IL_006a

      IL_0058:  ldarg.0
      IL_0059:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_005e:  ldc.i4     0x10000
      IL_0063:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0068:  ldc.i4.0
      IL_0069:  ret

      IL_006a:  ldarg.0
      IL_006b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0070:  ldarg.0
      IL_0071:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0076:  ldarg.0
      IL_0077:  ldfld      bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::mFromInventory
      IL_007c:  ldarg.0
      IL_007d:  ldarg.0
      IL_007e:  ldftn      instance class [SimIFace]Sims3.SimIFace.StateMachineClient Sims3.Store.Objects.Tablet/ListenToAudioProgram::GetDefaultStateMachine()
      IL_0084:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate::.ctor(object,
                                                                                                                                  native int)
      IL_0089:  ldarg.0
      IL_008a:  ldflda     class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_008f:  callvirt   instance void Sims3.Store.Objects.Tablet::StartUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      bool,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate,
                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient&)
      IL_0094:  ldarg.0
      IL_0095:  ldc.i4.0
      IL_0096:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry(bool)
      IL_009b:  ldarg.0
      IL_009c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00a7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_00ac:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::DisableLookAts()
      IL_00b1:  ldarg.0
      IL_00b2:  ldstr      "Listen"
      IL_00b7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_00bc:  ldarg.0
      IL_00bd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00c2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_00c7:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_00cc:  ldarg.0
      IL_00cd:  ldc.i4.1
      IL_00ce:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_00d3:  ldarg.0
      IL_00d4:  ldarg.0
      IL_00d5:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_00da:  ldarg.0
      IL_00db:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00e0:  ldarg.0
      IL_00e1:  ldfld      bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::mFromInventory
      IL_00e6:  callvirt   instance bool Sims3.Store.Objects.Tablet::ShouldPutInInventory(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                          bool)
      IL_00eb:  stfld      bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::mFromInventory
      IL_00f0:  ldarg.0
      IL_00f1:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_00f6:  ldarg.0
      IL_00f7:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00fc:  ldnull
      IL_00fd:  ldarg.0
      IL_00fe:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_0103:  ldarg.0
      IL_0104:  ldfld      bool Sims3.Store.Objects.Tablet/ListenToAudioProgram::mFromInventory
      IL_0109:  callvirt   instance bool Sims3.Store.Objects.Tablet::StopUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book,
                                                                                     class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                     bool)
      IL_010e:  stloc.0
      IL_010f:  ldarg.0
      IL_0110:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0115:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
      IL_011a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Count()
      IL_011f:  ldc.i4.0
      IL_0120:  ceq
      IL_0122:  ldc.i4.0
      IL_0123:  ceq
      IL_0125:  stloc.1
      IL_0126:  ldarg.0
      IL_0127:  ldloc.1
      IL_0128:  ldloc.1
      IL_0129:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_012e:  ldloc.0
      IL_012f:  ret
    } // end of method ListenToAudioProgram::DoListenToAudioProgram

    .method public hidebysig instance void 
            StartProgram(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                         class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       29 (0x1d)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_000b:  ldc.i8     0x9a7f5f1919df86c1
      IL_0014:  ldc.i4.0
      IL_0015:  conv.i8
      IL_0016:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElement(uint64,
                                                                                                                   valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
      IL_001b:  pop
      IL_001c:  ret
    } // end of method ListenToAudioProgram::StartProgram

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method ListenToAudioProgram::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/ListenToAudioProgram/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ListenToAudioProgram::Singleton
      IL_000a:  ret
    } // end of method ListenToAudioProgram::.cctor

    .property instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
            SkillProgram()
    {
      .get instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::get_SkillProgram()
    } // end of property ListenToAudioProgram::SkillProgram
  } // end of class ListenToAudioProgram

  .class auto ansi nested public beforefieldinit StopAudioProgram
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet/StopAudioProgram>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       26 (0x1a)
        .maxstack  8
        IL_0000:  ldarg.2
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
        IL_0006:  ldc.i8     0x9a7f5f1919df86c1
        IL_000f:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::HasElement(uint64)
        IL_0014:  brtrue.s   IL_0018

        IL_0016:  ldc.i4.0
        IL_0017:  ret

        IL_0018:  ldc.i4.1
        IL_0019:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "StopAudioProgram"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet/StopAudioProgram>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_000b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_0010:  ldarg.0
      IL_0011:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_0016:  ret
    } // end of method StopAudioProgram::Cleanup

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       197 (0xc5)
      .maxstack  5
      .locals init (class [SimIFace]Sims3.SimIFace.StateMachineClient V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry(bool)
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_000d:  ldarg.0
      IL_000e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0013:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0018:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::DisableLookAts()
      IL_001d:  ldarg.0
      IL_001e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0023:  callvirt   instance class [SimIFace]Sims3.SimIFace.IScriptProxy [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_Proxy()
      IL_0028:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [SimIFace]Sims3.SimIFace.ITask::get_ObjectId()
      IL_002d:  ldstr      "Tablet_store"
      IL_0032:  call       class [SimIFace]Sims3.SimIFace.StateMachineClient [SimIFace]Sims3.SimIFace.StateMachineClient::Acquire(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                  string)
      IL_0037:  stloc.0
      IL_0038:  ldloc.0
      IL_0039:  ldstr      "x"
      IL_003e:  ldarg.0
      IL_003f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0044:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0049:  ldloc.0
      IL_004a:  ldstr      "isSitting"
      IL_004f:  ldarg.0
      IL_0050:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0055:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_005a:  ldc.i4     0x40f3d23
      IL_005f:  ldarg.0
      IL_0060:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Target
      IL_0065:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_006a:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_006f:  ldloc.0
      IL_0070:  ldstr      "x"
      IL_0075:  ldstr      "Enter"
      IL_007a:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_007f:  ldloc.0
      IL_0080:  ldc.i4.s   102
      IL_0082:  ldarg.0
      IL_0083:  ldftn      instance void Sims3.Store.Objects.Tablet/StopAudioProgram::StopProgram(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                  class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0089:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_008e:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0093:  ldloc.0
      IL_0094:  ldc.i4.1
      IL_0095:  ldstr      "x"
      IL_009a:  ldstr      "StopListening"
      IL_009f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(bool,
                                                                                                   string,
                                                                                                   string)
      IL_00a4:  ldarg.0
      IL_00a5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_00aa:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_00af:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.1
      IL_00b6:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_00bb:  ldarg.0
      IL_00bc:  ldc.i4.0
      IL_00bd:  ldc.i4.0
      IL_00be:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_00c3:  ldc.i4.1
      IL_00c4:  ret
    } // end of method StopAudioProgram::Run

    .method public hidebysig instance void 
            StopProgram(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                        class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Actor
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_000b:  ldc.i8     0x9a7f5f1919df86c1
      IL_0014:  callvirt   instance void class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::RemoveElement(uint64)
      IL_0019:  ret
    } // end of method StopAudioProgram::StopProgram

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::.ctor()
      IL_0006:  ret
    } // end of method StopAudioProgram::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/StopAudioProgram/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/StopAudioProgram::Singleton
      IL_000a:  ret
    } // end of method StopAudioProgram::.cctor

  } // end of class StopAudioProgram

  .class auto ansi nested public beforefieldinit StopAudioProgramFromTablet
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/StopAudioProgramFromTablet>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim a,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       26 (0x1a)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
        IL_0006:  ldc.i8     0x9a7f5f1919df86c1
        IL_000f:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::HasElement(uint64)
        IL_0014:  brtrue.s   IL_0018

        IL_0016:  ldc.i4.0
        IL_0017:  ret

        IL_0018:  ldc.i4.1
        IL_0019:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "StopAudioProgram"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/StopAudioProgramFromTablet>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_000b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_0010:  ldarg.0
      IL_0011:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_0016:  ret
    } // end of method StopAudioProgramFromTablet::Cleanup

    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       197 (0xc5)
      .maxstack  5
      .locals init (class [SimIFace]Sims3.SimIFace.StateMachineClient V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry(bool)
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_000d:  ldarg.0
      IL_000e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0013:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0018:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::DisableLookAts()
      IL_001d:  ldarg.0
      IL_001e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0023:  callvirt   instance class [SimIFace]Sims3.SimIFace.IScriptProxy [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_Proxy()
      IL_0028:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [SimIFace]Sims3.SimIFace.ITask::get_ObjectId()
      IL_002d:  ldstr      "Tablet_store"
      IL_0032:  call       class [SimIFace]Sims3.SimIFace.StateMachineClient [SimIFace]Sims3.SimIFace.StateMachineClient::Acquire(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                  string)
      IL_0037:  stloc.0
      IL_0038:  ldloc.0
      IL_0039:  ldstr      "x"
      IL_003e:  ldarg.0
      IL_003f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0044:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_0049:  ldloc.0
      IL_004a:  ldstr      "isSitting"
      IL_004f:  ldarg.0
      IL_0050:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0055:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_005a:  ldc.i4     0x40f3d23
      IL_005f:  ldarg.0
      IL_0060:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0065:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_006a:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_006f:  ldloc.0
      IL_0070:  ldstr      "x"
      IL_0075:  ldstr      "Enter"
      IL_007a:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_007f:  ldloc.0
      IL_0080:  ldc.i4.s   102
      IL_0082:  ldarg.0
      IL_0083:  ldftn      instance void Sims3.Store.Objects.Tablet/StopAudioProgramFromTablet::StopProgram(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                            class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0089:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_008e:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0093:  ldloc.0
      IL_0094:  ldc.i4.1
      IL_0095:  ldstr      "x"
      IL_009a:  ldstr      "StopListening"
      IL_009f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(bool,
                                                                                                   string,
                                                                                                   string)
      IL_00a4:  ldarg.0
      IL_00a5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00aa:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_00af:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.1
      IL_00b6:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_00bb:  ldarg.0
      IL_00bc:  ldc.i4.0
      IL_00bd:  ldc.i4.0
      IL_00be:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_00c3:  ldc.i4.1
      IL_00c4:  ret
    } // end of method StopAudioProgramFromTablet::Run

    .method public hidebysig instance void 
            StopProgram(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                        class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_000b:  ldc.i8     0x9a7f5f1919df86c1
      IL_0014:  callvirt   instance void class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::RemoveElement(uint64)
      IL_0019:  ret
    } // end of method StopAudioProgramFromTablet::StopProgram

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method StopAudioProgramFromTablet::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/StopAudioProgramFromTablet/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/StopAudioProgramFromTablet::Singleton
      IL_000a:  ret
    } // end of method StopAudioProgramFromTablet::.cctor

  } // end of class StopAudioProgramFromTablet

  .class auto ansi nested public beforefieldinit PickUpTabletForGaming
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForGaming>
           implements [Sims3GameplaySystems]Sims3.Gameplay.Interactions.IOverridesCalculateScore
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       41 (0x29)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  bne.un.s   IL_000b

        IL_0009:  ldc.i4.0
        IL_000a:  ret

        IL_000b:  ldarg.3
        IL_000c:  brfalse.s  IL_0027

        IL_000e:  ldarg.1
        IL_000f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_0014:  ldc.i4     0x10f38da
        IL_0019:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
        IL_001e:  ldsfld     float32 Sims3.Store.Objects.Tablet::kPlayGameFunThreshold
        IL_0023:  blt.un.s   IL_0027

        IL_0025:  ldc.i4.0
        IL_0026:  ret

        IL_0027:  ldc.i4.1
        IL_0028:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       119 (0x77)
        .maxstack  2
        IL_0000:  ldarg.1
        IL_0001:  ldc.i8     0xb82d0015b9294320
        IL_000a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_000f:  brfalse.s  IL_0022

        IL_0011:  ldstr      "PlayGameGood"
        IL_0016:  ldc.i4.0
        IL_0017:  newarr     [mscorlib]System.Object
        IL_001c:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0021:  ret

        IL_0022:  ldarg.1
        IL_0023:  ldc.i8     0xb82d0015b92942a0
        IL_002c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_0031:  brfalse.s  IL_0044

        IL_0033:  ldstr      "PlayGameEvil"
        IL_0038:  ldc.i4.0
        IL_0039:  newarr     [mscorlib]System.Object
        IL_003e:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0043:  ret

        IL_0044:  ldarg.1
        IL_0045:  ldc.i8     0xb82d0015b9294260
        IL_004e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_0053:  brfalse.s  IL_0066

        IL_0055:  ldstr      "PlayGameExtreme"
        IL_005a:  ldc.i4.0
        IL_005b:  newarr     [mscorlib]System.Object
        IL_0060:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0065:  ret

        IL_0066:  ldstr      "PlayGame"
        IL_006b:  ldc.i4.0
        IL_006c:  newarr     [mscorlib]System.Object
        IL_0071:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0076:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig newslot virtual final 
              instance float32  CalculateScore(class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interactionObjectPair,
                                               class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy autonomy) cil managed
      {
        // Code size       78 (0x4e)
        .maxstack  3
        .locals init (float32 V_0)
        IL_0000:  ldc.r4     1.
        IL_0005:  stloc.0
        IL_0006:  ldarg.2
        IL_0007:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
        IL_000c:  ldc.i8     0x4c800171bd46a00
        IL_0015:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_001a:  brfalse.s  IL_0026

        IL_001c:  ldloc.0
        IL_001d:  ldsfld     float32 Sims3.Store.Objects.Tablet::kCompWhizMultiplier
        IL_0022:  mul
        IL_0023:  stloc.0
        IL_0024:  br.s       IL_0044

        IL_0026:  ldarg.2
        IL_0027:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
        IL_002c:  ldc.i8     0x4c800171bd469d0
        IL_0035:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_003a:  brfalse.s  IL_0044

        IL_003c:  ldloc.0
        IL_003d:  ldsfld     float32 Sims3.Store.Objects.Tablet::kTechnophobeMultiplier
        IL_0042:  mul
        IL_0043:  stloc.0
        IL_0044:  ldloc.0
        IL_0045:  ldarg.2
        IL_0046:  ldarg.1
        IL_0047:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::CalculateScoreForObjectInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair)
        IL_004c:  mul
        IL_004d:  ret
      } // end of method Definition::CalculateScore

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForGaming>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PickUp(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable)
      IL_0011:  brtrue.s   IL_0015

      IL_0013:  ldc.i4.0
      IL_0014:  ret

      IL_0015:  ldarg.0
      IL_0016:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForGaming::PushGameInteraction()
      IL_001b:  ret
    } // end of method PickUpTabletForGaming::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForGaming::PushGameInteraction()
      IL_0006:  ret
    } // end of method PickUpTabletForGaming::RunFromInventory

    .method public hidebysig virtual instance bool 
            TargetInventoryChangeIsValid(class [Sims3GameplaySystems]Sims3.Gameplay.Inventory newInventory) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brfalse.s  IL_0011

      IL_0003:  ldarg.1
      IL_0004:  ldarg.0
      IL_0005:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_000f:  bne.un.s   IL_0013

      IL_0011:  ldc.i4.1
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method PickUpTabletForGaming::TargetInventoryChangeIsValid

    .method public hidebysig instance bool 
            PushGameInteraction() cil managed
    {
      // Code size       72 (0x48)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
      IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PlayGameOnTablet::Singleton
      IL_0005:  ldarg.0
      IL_0006:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000b:  ldarg.0
      IL_000c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0011:  ldarg.0
      IL_0012:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0017:  ldarg.0
      IL_0018:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_001d:  ldc.i4.1
      IL_001e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0023:  stloc.0
      IL_0024:  ldarg.0
      IL_0025:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_002a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_002f:  ldloc.0
      IL_0030:  ldc.i4.0
      IL_0031:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0036:  brtrue.s   IL_0046

      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003e:  ldc.i4.1
      IL_003f:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PutDown(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SurfaceType)
      IL_0044:  ldc.i4.0
      IL_0045:  ret

      IL_0046:  ldc.i4.1
      IL_0047:  ret
    } // end of method PickUpTabletForGaming::PushGameInteraction

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method PickUpTabletForGaming::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/PickUpTabletForGaming/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PickUpTabletForGaming::Singleton
      IL_000a:  ret
    } // end of method PickUpTabletForGaming::.cctor

  } // end of class PickUpTabletForGaming

  .class auto ansi nested public beforefieldinit PlayGameOnTablet
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PlayGameOnTablet>
           implements [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IWakesLightSleeper,
                      [Sims3GameplaySystems]Sims3.Gameplay.Interactions.IOverridesCalculateScore
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       55 (0x37)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  beq.s      IL_0019

        IL_0009:  ldarg.1
        IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
        IL_000f:  ldarg.2
        IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
        IL_0015:  brtrue.s   IL_0019

        IL_0017:  ldc.i4.0
        IL_0018:  ret

        IL_0019:  ldarg.3
        IL_001a:  brfalse.s  IL_0035

        IL_001c:  ldarg.1
        IL_001d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
        IL_0022:  ldc.i4     0x10f38da
        IL_0027:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::GetValue(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
        IL_002c:  ldsfld     float32 Sims3.Store.Objects.Tablet::kPlayGameFunThreshold
        IL_0031:  blt.un.s   IL_0035

        IL_0033:  ldc.i4.0
        IL_0034:  ret

        IL_0035:  ldc.i4.1
        IL_0036:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       119 (0x77)
        .maxstack  2
        IL_0000:  ldarg.1
        IL_0001:  ldc.i8     0xb82d0015b9294320
        IL_000a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_000f:  brfalse.s  IL_0022

        IL_0011:  ldstr      "PlayGameGood"
        IL_0016:  ldc.i4.0
        IL_0017:  newarr     [mscorlib]System.Object
        IL_001c:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0021:  ret

        IL_0022:  ldarg.1
        IL_0023:  ldc.i8     0xb82d0015b92942a0
        IL_002c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_0031:  brfalse.s  IL_0044

        IL_0033:  ldstr      "PlayGameEvil"
        IL_0038:  ldc.i4.0
        IL_0039:  newarr     [mscorlib]System.Object
        IL_003e:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0043:  ret

        IL_0044:  ldarg.1
        IL_0045:  ldc.i8     0xb82d0015b9294260
        IL_004e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_0053:  brfalse.s  IL_0066

        IL_0055:  ldstr      "PlayGameExtreme"
        IL_005a:  ldc.i4.0
        IL_005b:  newarr     [mscorlib]System.Object
        IL_0060:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0065:  ret

        IL_0066:  ldstr      "PlayGame"
        IL_006b:  ldc.i4.0
        IL_006c:  newarr     [mscorlib]System.Object
        IL_0071:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0076:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig newslot virtual final 
              instance float32  CalculateScore(class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair interactionObjectPair,
                                               class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy autonomy) cil managed
      {
        // Code size       78 (0x4e)
        .maxstack  3
        .locals init (float32 V_0)
        IL_0000:  ldc.r4     1.
        IL_0005:  stloc.0
        IL_0006:  ldarg.2
        IL_0007:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
        IL_000c:  ldc.i8     0x4c800171bd46a00
        IL_0015:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_001a:  brfalse.s  IL_0026

        IL_001c:  ldloc.0
        IL_001d:  ldsfld     float32 Sims3.Store.Objects.Tablet::kCompWhizMultiplier
        IL_0022:  mul
        IL_0023:  stloc.0
        IL_0024:  br.s       IL_0044

        IL_0026:  ldarg.2
        IL_0027:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::get_Actor()
        IL_002c:  ldc.i8     0x4c800171bd469d0
        IL_0035:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
        IL_003a:  brfalse.s  IL_0044

        IL_003c:  ldloc.0
        IL_003d:  ldsfld     float32 Sims3.Store.Objects.Tablet::kTechnophobeMultiplier
        IL_0042:  mul
        IL_0043:  stloc.0
        IL_0044:  ldloc.0
        IL_0045:  ldarg.2
        IL_0046:  ldarg.1
        IL_0047:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Autonomy::CalculateScoreForObjectInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair)
        IL_004c:  mul
        IL_004d:  ret
      } // end of method Definition::CalculateScore

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PlayGameOnTablet>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static literal string kTabletVideoGameVFXName = "store_tablet_game"
    .field public bool mFromInventory
    .field public class [SimIFace]Sims3.SimIFace.VisualEffect mScreenVFX
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  stfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_0007:  ldarg.0
      IL_0008:  call       instance bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::DoPlayGame()
      IL_000d:  ret
    } // end of method PlayGameOnTablet::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  stfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_0007:  ldarg.0
      IL_0008:  call       instance bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::DoPlayGame()
      IL_000d:  ret
    } // end of method PlayGameOnTablet::RunFromInventory

    .method public hidebysig virtual instance void 
            PostureTransitionFailed(bool transitionExitReason) cil managed
    {
      // Code size       175 (0xaf)
      .maxstack  11
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0006:  brtrue.s   IL_0009

      IL_0008:  ret

      IL_0009:  ldarg.0
      IL_000a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0014:  ldarg.0
      IL_0015:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_001a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_001f:  brtrue     IL_00ae

      IL_0024:  ldarg.0
      IL_0025:  ldfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_002a:  brfalse.s  IL_0072

      IL_002c:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.PutInInventory::Singleton
      IL_0031:  ldarg.0
      IL_0032:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0037:  ldarg.0
      IL_0038:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003d:  ldarg.0
      IL_003e:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0043:  ldc.i4.0
      IL_0044:  ldarg.0
      IL_0045:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_004a:  ldnull
      IL_004b:  ldnull
      IL_004c:  ldarg.0
      IL_004d:  ldftn      instance void Sims3.Store.Objects.Tablet/PlayGameOnTablet::PutInInventoryFailureCallback(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                    float32)
      IL_0053:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback::.ctor(object,
                                                                                                       native int)
      IL_0058:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstanceWithCallbacks(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                                     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                                     bool,
                                                                                                                                                                                                                     bool,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback)
      IL_005d:  stloc.0
      IL_005e:  ldarg.0
      IL_005f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0064:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0069:  ldloc.0
      IL_006a:  ldc.i4.1
      IL_006b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0070:  pop
      IL_0071:  ret

      IL_0072:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_0077:  ldarg.0
      IL_0078:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_007d:  ldarg.0
      IL_007e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0083:  ldarg.0
      IL_0084:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0089:  ldarg.0
      IL_008a:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_008f:  ldarg.0
      IL_0090:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_0095:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_009a:  stloc.1
      IL_009b:  ldarg.0
      IL_009c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00a1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_00a6:  ldloc.1
      IL_00a7:  ldc.i4.1
      IL_00a8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_00ad:  pop
      IL_00ae:  ret
    } // end of method PlayGameOnTablet::PostureTransitionFailed

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       99 (0x63)
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_HasBeenDestroyed()
      IL_000b:  brtrue.s   IL_0026

      IL_000d:  ldarg.0
      IL_000e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0013:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
      IL_0018:  ldc.i8     0x4c80018653f16e0
      IL_0021:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::UnpauseBuff(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
      IL_0026:  ldarg.0
      IL_0027:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_002c:  brfalse.s  IL_004c

      IL_002e:  ldarg.0
      IL_002f:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_0034:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
      IL_0039:  pop
      IL_003a:  ldarg.0
      IL_003b:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_0040:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
      IL_0045:  ldarg.0
      IL_0046:  ldnull
      IL_0047:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_004c:  ldarg.0
      IL_004d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0052:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0057:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_005c:  ldarg.0
      IL_005d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_0062:  ret
    } // end of method PlayGameOnTablet::Cleanup

    .method public hidebysig instance class [SimIFace]Sims3.SimIFace.StateMachineClient 
            GetDefaultStateMachine() cil managed
    {
      // Code size       174 (0xae)
      .maxstack  5
      .locals init (class [SimIFace]Sims3.SimIFace.StateMachineClient V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance class [SimIFace]Sims3.SimIFace.IScriptProxy [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_Proxy()
      IL_000b:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [SimIFace]Sims3.SimIFace.ITask::get_ObjectId()
      IL_0010:  ldstr      "Tablet_store"
      IL_0015:  call       class [SimIFace]Sims3.SimIFace.StateMachineClient [SimIFace]Sims3.SimIFace.StateMachineClient::Acquire(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                  string)
      IL_001a:  stloc.0
      IL_001b:  ldloc.0
      IL_001c:  ldstr      "x"
      IL_0021:  ldarg.0
      IL_0022:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0027:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_002c:  ldloc.0
      IL_002d:  ldstr      "tablet"
      IL_0032:  ldarg.0
      IL_0033:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0038:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_003d:  ldloc.0
      IL_003e:  ldstr      "isSitting"
      IL_0043:  ldarg.0
      IL_0044:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0049:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_004e:  ldc.i4     0x40f3d23
      IL_0053:  ldarg.0
      IL_0054:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0059:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_005e:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0063:  ldloc.0
      IL_0064:  ldstr      "inInventory"
      IL_0069:  ldarg.0
      IL_006a:  ldfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_006f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0074:  ldloc.0
      IL_0075:  ldstr      "x"
      IL_007a:  ldstr      "Enter"
      IL_007f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_0084:  ldloc.0
      IL_0085:  ldc.i4.s   103
      IL_0087:  ldarg.0
      IL_0088:  ldftn      instance void Sims3.Store.Objects.Tablet/PlayGameOnTablet::GameVideoOn(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                  class [SimIFace]Sims3.SimIFace.IEvent)
      IL_008e:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0093:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0098:  ldloc.0
      IL_0099:  ldc.i4.s   105
      IL_009b:  ldarg.0
      IL_009c:  ldftn      instance void Sims3.Store.Objects.Tablet/PlayGameOnTablet::GameVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                   class [SimIFace]Sims3.SimIFace.IEvent)
      IL_00a2:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_00a7:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_00ac:  ldloc.0
      IL_00ad:  ret
    } // end of method PlayGameOnTablet::GetDefaultStateMachine

    .method public hidebysig instance bool 
            DoPlayGame() cil managed
    {
      // Code size       379 (0x17b)
      .maxstack  7
      .locals init (bool V_0,
               bool V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_0006:  brfalse.s  IL_0045

      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0013:  ldarg.0
      IL_0014:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0019:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToRemove(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_001e:  brtrue.s   IL_0032

      IL_0020:  ldarg.0
      IL_0021:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0026:  ldc.i4     0x400
      IL_002b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0030:  ldc.i4.0
      IL_0031:  ret

      IL_0032:  ldarg.0
      IL_0033:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0043:  br.s       IL_006a

      IL_0045:  ldarg.0
      IL_0046:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_004b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
      IL_0050:  ldarg.0
      IL_0051:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0056:  beq.s      IL_006a

      IL_0058:  ldarg.0
      IL_0059:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_005e:  ldc.i4     0x10000
      IL_0063:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0068:  ldc.i4.0
      IL_0069:  ret

      IL_006a:  ldarg.0
      IL_006b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0070:  ldarg.0
      IL_0071:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0076:  ldarg.0
      IL_0077:  ldfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_007c:  ldarg.0
      IL_007d:  ldarg.0
      IL_007e:  ldftn      instance class [SimIFace]Sims3.SimIFace.StateMachineClient Sims3.Store.Objects.Tablet/PlayGameOnTablet::GetDefaultStateMachine()
      IL_0084:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate::.ctor(object,
                                                                                                                                  native int)
      IL_0089:  ldarg.0
      IL_008a:  ldflda     class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_008f:  callvirt   instance void Sims3.Store.Objects.Tablet::StartUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      bool,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate,
                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient&)
      IL_0094:  ldarg.0
      IL_0095:  ldc.i4.0
      IL_0096:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry(bool)
      IL_009b:  ldarg.0
      IL_009c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00a1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_TraitManager()
      IL_00a6:  ldc.i8     0x4c800171bd46a00
      IL_00af:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitManager::HasElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_00b4:  brfalse.s  IL_00c6

      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4     0x10f38da
      IL_00bc:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.TuningValues.TraitTuning::get_ComputerWhizTraitGamesFunMultiplier()
      IL_00c1:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdate(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                                     float32)
      IL_00c6:  ldarg.0
      IL_00c7:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_00cc:  ldarg.0
      IL_00cd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00d2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_00d7:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::DisableLookAts()
      IL_00dc:  ldarg.0
      IL_00dd:  ldstr      "PlayGame"
      IL_00e2:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_00e7:  ldarg.0
      IL_00e8:  ldc.i4     0xffa9bfff
      IL_00ed:  ldarg.0
      IL_00ee:  ldftn      instance void Sims3.Store.Objects.Tablet/PlayGameOnTablet::LoopDel(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                              valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_00f4:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_00f9:  ldnull
      IL_00fa:  ldc.r4     1.
      IL_00ff:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                               class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                                                                                                               float32)
      IL_0104:  stloc.0
      IL_0105:  ldarg.0
      IL_0106:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_010b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0110:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_0115:  ldarg.0
      IL_0116:  ldloc.0
      IL_0117:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_011c:  ldarg.0
      IL_011d:  ldarg.0
      IL_011e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0123:  ldarg.0
      IL_0124:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0129:  ldarg.0
      IL_012a:  ldfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_012f:  callvirt   instance bool Sims3.Store.Objects.Tablet::ShouldPutInInventory(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                          bool)
      IL_0134:  stfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_0139:  ldloc.0
      IL_013a:  ldarg.0
      IL_013b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0140:  ldarg.0
      IL_0141:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0146:  ldnull
      IL_0147:  ldarg.0
      IL_0148:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_014d:  ldarg.0
      IL_014e:  ldfld      bool Sims3.Store.Objects.Tablet/PlayGameOnTablet::mFromInventory
      IL_0153:  callvirt   instance bool Sims3.Store.Objects.Tablet::StopUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book,
                                                                                     class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                     bool)
      IL_0158:  and
      IL_0159:  stloc.0
      IL_015a:  ldarg.0
      IL_015b:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0160:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
      IL_0165:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Count()
      IL_016a:  ldc.i4.0
      IL_016b:  ceq
      IL_016d:  ldc.i4.0
      IL_016e:  ceq
      IL_0170:  stloc.1
      IL_0171:  ldarg.0
      IL_0172:  ldloc.1
      IL_0173:  ldloc.1
      IL_0174:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_0179:  ldloc.0
      IL_017a:  ret
    } // end of method PlayGameOnTablet::DoPlayGame

    .method public hidebysig instance void 
            LoopDel(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData loopData) cil managed
    {
      // Code size       161 (0xa1)
      .maxstack  3
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultImaginaryFriend V_0,
               float32 V_1)
      IL_0000:  ldarga.s   loopData
      IL_0002:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData::mDeltaTime
      IL_0007:  ldarga.s   loopData
      IL_0009:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData::mLifeTime
      IL_000e:  bne.un.s   IL_0038

      IL_0010:  ldarg.0
      IL_0011:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0016:  ldloca.s   V_0
      IL_0018:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultImaginaryFriend::TryGetOccultFromSim(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                             class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultImaginaryFriend&)
      IL_001d:  brfalse.s  IL_0038

      IL_001f:  ldloc.0
      IL_0020:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.OccultImaginaryFriend::get_IsReal()
      IL_0025:  brtrue.s   IL_0038

      IL_0027:  ldarg.0
      IL_0028:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_002d:  ldc.i4     0x200
      IL_0032:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0037:  ret

      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003e:  ldc.i8     0x4c800171bd469d0
      IL_0047:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_004c:  brtrue.s   IL_0055

      IL_004e:  ldsfld     float32 Sims3.Store.Objects.Tablet::kGamingDuration
      IL_0053:  br.s       IL_005a

      IL_0055:  ldsfld     float32 [Sims3GameplayObjects]Sims3.Gameplay.Objects.Electronics.Computer::kTechnophobeTraitMaximumChatTime
      IL_005a:  stloc.1
      IL_005b:  ldarga.s   loopData
      IL_005d:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData::mLifeTime
      IL_0062:  ldloc.1
      IL_0063:  bge.s      IL_007c

      IL_0065:  ldarg.0
      IL_0066:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_006b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_0070:  ldc.i4     0x10f38da
      IL_0075:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::IsMax(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
      IL_007a:  brfalse.s  IL_0089

      IL_007c:  ldarg.0
      IL_007d:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0082:  ldc.i4.s   64
      IL_0084:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0089:  ldc.i4     0x10d
      IL_008e:  ldarg.0
      IL_008f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0094:  ldarg.0
      IL_0095:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_009a:  call       class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTracker::SendEvent(valuetype [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTypeId,
                                                                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                 class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_009f:  pop
      IL_00a0:  ret
    } // end of method PlayGameOnTablet::LoopDel

    .method public hidebysig instance void 
            GameVideoOn(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                        class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       73 (0x49)
      .maxstack  3
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_0006:  brfalse.s  IL_0010

      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  ldarg.2
      IL_000b:  call       instance void Sims3.Store.Objects.Tablet/PlayGameOnTablet::GameVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                   class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0010:  ldarg.0
      IL_0011:  ldstr      "store_tablet_game"
      IL_0016:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
      IL_001b:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_0026:  ldarg.0
      IL_0027:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_002c:  ldstr      "_FX_0_screen"
      IL_0031:  call       valuetype [SimIFace]Sims3.SimIFace.Slot [SimIFace]Sims3.SimIFace.Slots::Hash(string)
      IL_0036:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                         valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_003b:  pop
      IL_003c:  ldarg.0
      IL_003d:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_0042:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
      IL_0047:  pop
      IL_0048:  ret
    } // end of method PlayGameOnTablet::GameVideoOn

    .method public hidebysig instance void 
            GameVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                         class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       39 (0x27)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_0006:  brfalse.s  IL_0026

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
      IL_0013:  pop
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
      IL_001f:  ldarg.0
      IL_0020:  ldnull
      IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/PlayGameOnTablet::mScreenVFX
      IL_0026:  ret
    } // end of method PlayGameOnTablet::GameVideoOff

    .method public hidebysig instance void 
            PutInInventoryFailureCallback(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim s,
                                          float32 f) cil managed
    {
      // Code size       70 (0x46)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0006:  brtrue.s   IL_0009

      IL_0008:  ret

      IL_0009:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_000e:  ldarg.0
      IL_000f:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0014:  ldarg.0
      IL_0015:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_001a:  ldarg.0
      IL_001b:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0020:  ldarg.0
      IL_0021:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0026:  ldarg.0
      IL_0027:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_002c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0031:  stloc.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0038:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_003d:  ldloc.0
      IL_003e:  ldc.i4.1
      IL_003f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0044:  pop
      IL_0045:  ret
    } // end of method PlayGameOnTablet::PutInInventoryFailureCallback

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method PlayGameOnTablet::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/PlayGameOnTablet/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PlayGameOnTablet::Singleton
      IL_000a:  ret
    } // end of method PlayGameOnTablet::.cctor

  } // end of class PlayGameOnTablet

  .class auto ansi nested public beforefieldinit PickUpTabletForHomework
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForHomework>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       26 (0x1a)
        .maxstack  5
        .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework V_0)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  bne.un.s   IL_000b

        IL_0009:  ldc.i4.0
        IL_000a:  ret

        IL_000b:  ldnull
        IL_000c:  stloc.0
        IL_000d:  ldarg.1
        IL_000e:  ldloca.s   V_0
        IL_0010:  ldarg.3
        IL_0011:  ldc.i4.1
        IL_0012:  ldarg.s    greyedOutTooltipCallback
        IL_0014:  call       bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::HomeworkTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework&,
                                                                                              bool,
                                                                                              bool,
                                                                                              class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_0019:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "DoHomework"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PickUpTabletForHomework>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PickUp(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ICarryable)
      IL_0011:  brtrue.s   IL_0015

      IL_0013:  ldc.i4.0
      IL_0014:  ret

      IL_0015:  ldarg.0
      IL_0016:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForHomework::PushHomeworkInteraction()
      IL_001b:  ret
    } // end of method PickUpTabletForHomework::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance bool Sims3.Store.Objects.Tablet/PickUpTabletForHomework::PushHomeworkInteraction()
      IL_0006:  ret
    } // end of method PickUpTabletForHomework::RunFromInventory

    .method public hidebysig virtual instance bool 
            TargetInventoryChangeIsValid(class [Sims3GameplaySystems]Sims3.Gameplay.Inventory newInventory) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brfalse.s  IL_0011

      IL_0003:  ldarg.1
      IL_0004:  ldarg.0
      IL_0005:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_000f:  bne.un.s   IL_0013

      IL_0011:  ldc.i4.1
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method PickUpTabletForHomework::TargetInventoryChangeIsValid

    .method public hidebysig instance bool 
            PushHomeworkInteraction() cil managed
    {
      // Code size       72 (0x48)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
      IL_0000:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::Singleton
      IL_0005:  ldarg.0
      IL_0006:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_000b:  ldarg.0
      IL_000c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0011:  ldarg.0
      IL_0012:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0017:  ldarg.0
      IL_0018:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_001d:  ldc.i4.1
      IL_001e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0023:  stloc.0
      IL_0024:  ldarg.0
      IL_0025:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_002a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_002f:  ldloc.0
      IL_0030:  ldc.i4.0
      IL_0031:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0036:  brtrue.s   IL_0046

      IL_0038:  ldarg.0
      IL_0039:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003e:  ldc.i4.1
      IL_003f:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PutDown(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SurfaceType)
      IL_0044:  ldc.i4.0
      IL_0045:  ret

      IL_0046:  ldc.i4.1
      IL_0047:  ret
    } // end of method PickUpTabletForHomework::PushHomeworkInteraction

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method PickUpTabletForHomework::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/PickUpTabletForHomework/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PickUpTabletForHomework::Singleton
      IL_000a:  ret
    } // end of method PickUpTabletForHomework::.cctor

  } // end of class PickUpTabletForHomework

  .class auto ansi nested public beforefieldinit DoHomeworkOnTablet
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/DoHomeworkOnTablet>
           implements [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IWakesLightSleeper
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
                   bool isAutonomous,
                   class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
      {
        // Code size       40 (0x28)
        .maxstack  5
        .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework V_0)
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
        IL_0006:  ldarg.2
        IL_0007:  beq.s      IL_0019

        IL_0009:  ldarg.1
        IL_000a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
        IL_000f:  ldarg.2
        IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
        IL_0015:  brtrue.s   IL_0019

        IL_0017:  ldc.i4.0
        IL_0018:  ret

        IL_0019:  ldnull
        IL_001a:  stloc.0
        IL_001b:  ldarg.1
        IL_001c:  ldloca.s   V_0
        IL_001e:  ldarg.3
        IL_001f:  ldc.i4.1
        IL_0020:  ldarg.s    greyedOutTooltipCallback
        IL_0022:  call       bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::HomeworkTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework&,
                                                                                              bool,
                                                                                              bool,
                                                                                              class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
        IL_0027:  ret
      } // end of method Definition::Test

      .method public hidebysig virtual instance string 
              GetInteractionName(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "DoHomework"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/DoHomeworkOnTablet>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static literal string kTabletHomeworkVFXName = "store_tablet_homework"
    .field public bool mFromInventory
    .field public class [SimIFace]Sims3.SimIFace.VisualEffect mScreenVFX
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework mHomework
    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  stfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_0007:  ldarg.0
      IL_0008:  call       instance bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::DoMyHomework()
      IL_000d:  ret
    } // end of method DoHomeworkOnTablet::Run

    .method public hidebysig virtual instance bool 
            RunFromInventory() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  stfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_0007:  ldarg.0
      IL_0008:  call       instance bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::DoMyHomework()
      IL_000d:  ret
    } // end of method DoHomeworkOnTablet::RunFromInventory

    .method public hidebysig virtual instance void 
            PostureTransitionFailed(bool transitionExitReason) cil managed
    {
      // Code size       175 (0xaf)
      .maxstack  11
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0006:  brtrue.s   IL_0009

      IL_0008:  ret

      IL_0009:  ldarg.0
      IL_000a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0014:  ldarg.0
      IL_0015:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_001a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Contains(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_001f:  brtrue     IL_00ae

      IL_0024:  ldarg.0
      IL_0025:  ldfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_002a:  brfalse.s  IL_0072

      IL_002c:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.PutInInventory::Singleton
      IL_0031:  ldarg.0
      IL_0032:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0037:  ldarg.0
      IL_0038:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_003d:  ldarg.0
      IL_003e:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0043:  ldc.i4.0
      IL_0044:  ldarg.0
      IL_0045:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_004a:  ldnull
      IL_004b:  ldnull
      IL_004c:  ldarg.0
      IL_004d:  ldftn      instance void Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::PutInInventoryFailureCallback(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                      float32)
      IL_0053:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback::.ctor(object,
                                                                                                       native int)
      IL_0058:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstanceWithCallbacks(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                                     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                                     bool,
                                                                                                                                                                                                                     bool,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback,
                                                                                                                                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Callback)
      IL_005d:  stloc.0
      IL_005e:  ldarg.0
      IL_005f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0064:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0069:  ldloc.0
      IL_006a:  ldc.i4.1
      IL_006b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0070:  pop
      IL_0071:  ret

      IL_0072:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_0077:  ldarg.0
      IL_0078:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_007d:  ldarg.0
      IL_007e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0083:  ldarg.0
      IL_0084:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0089:  ldarg.0
      IL_008a:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_008f:  ldarg.0
      IL_0090:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_0095:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_009a:  stloc.1
      IL_009b:  ldarg.0
      IL_009c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00a1:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_00a6:  ldloc.1
      IL_00a7:  ldc.i4.1
      IL_00a8:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_00ad:  pop
      IL_00ae:  ret
    } // end of method DoHomeworkOnTablet::PostureTransitionFailed

    .method public hidebysig virtual instance void 
            Cleanup() cil managed
    {
      // Code size       110 (0x6e)
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_0006:  brfalse.s  IL_0031

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_000e:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
      IL_0013:  ldarg.0
      IL_0014:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0019:  callvirt   instance bool class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::Contains(!0)
      IL_001e:  brfalse.s  IL_0031

      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_0026:  ldarg.0
      IL_0027:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_002c:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveFromUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0031:  ldarg.0
      IL_0032:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_0037:  brfalse.s  IL_0057

      IL_0039:  ldarg.0
      IL_003a:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_003f:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
      IL_0044:  pop
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_004b:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
      IL_0050:  ldarg.0
      IL_0051:  ldnull
      IL_0052:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_0057:  ldarg.0
      IL_0058:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_005d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0062:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_0067:  ldarg.0
      IL_0068:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::Cleanup()
      IL_006d:  ret
    } // end of method DoHomeworkOnTablet::Cleanup

    .method public hidebysig static bool 
            HomeworkTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                         class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework& target,
                         bool isAutonomous) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  5
      .locals init (class [UI]Sims3.SimIFace.GreyedOutTooltipCallback V_0)
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldarg.1
      IL_0004:  ldarg.2
      IL_0005:  ldc.i4.0
      IL_0006:  ldloca.s   V_0
      IL_0008:  call       bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::HomeworkTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework&,
                                                                                            bool,
                                                                                            bool,
                                                                                            class [UI]Sims3.SimIFace.GreyedOutTooltipCallback&)
      IL_000d:  ret
    } // end of method DoHomeworkOnTablet::HomeworkTest

    .method public hidebysig static bool 
            HomeworkTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                         class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework& target,
                         bool isAutonomous,
                         bool setCB,
                         class [UI]Sims3.SimIFace.GreyedOutTooltipCallback& greyedOutTooltipCallback) cil managed
    {
      // Code size       341 (0x155)
      .maxstack  5
      .locals init (class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework> V_0,
               int32 V_1,
               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_2,
               object[] V_3,
               object[] V_4,
               object[] V_5,
               object[] V_6)
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0006:  ldc.i4.0
      IL_0007:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<!!0> [Sims3GameplaySystems]Sims3.Gameplay.Inventory::FindAll<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework>(bool)
      IL_000c:  stloc.0
      IL_000d:  ldarg.1
      IL_000e:  ldnull
      IL_000f:  stind.ref
      IL_0010:  ldc.i4.0
      IL_0011:  stloc.1
      IL_0012:  br.s       IL_0040

      IL_0014:  ldloc.0
      IL_0015:  ldloc.1
      IL_0016:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework>::get_Item(int32)
      IL_001b:  ldarg.0
      IL_001c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework::IsSimsHomework(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0021:  brfalse.s  IL_003c

      IL_0023:  ldloc.0
      IL_0024:  ldloc.1
      IL_0025:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework>::get_Item(int32)
      IL_002a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework::IsComplete()
      IL_002f:  brtrue.s   IL_003c

      IL_0031:  ldarg.1
      IL_0032:  ldloc.0
      IL_0033:  ldloc.1
      IL_0034:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework>::get_Item(int32)
      IL_0039:  stind.ref
      IL_003a:  br.s       IL_0049

      IL_003c:  ldloc.1
      IL_003d:  ldc.i4.1
      IL_003e:  add
      IL_003f:  stloc.1
      IL_0040:  ldloc.1
      IL_0041:  ldloc.0
      IL_0042:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework>::get_Count()
      IL_0047:  blt.s      IL_0014

      IL_0049:  ldarg.1
      IL_004a:  ldind.ref
      IL_004b:  brtrue.s   IL_0076

      IL_004d:  ldarg.0
      IL_004e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0053:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::GetCurrentInteraction()
      IL_0058:  stloc.2
      IL_0059:  ldloc.2
      IL_005a:  brfalse.s  IL_0076

      IL_005c:  ldloc.2
      IL_005d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Target()
      IL_0062:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework
      IL_0067:  brfalse.s  IL_0076

      IL_0069:  ldarg.1
      IL_006a:  ldloc.2
      IL_006b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Target()
      IL_0070:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework
      IL_0075:  stind.ref
      IL_0076:  ldarg.1
      IL_0077:  ldind.ref
      IL_0078:  brtrue.s   IL_00a0

      IL_007a:  ldarg.3
      IL_007b:  brfalse.s  IL_009e

      IL_007d:  ldarg.2
      IL_007e:  brtrue.s   IL_009e

      IL_0080:  ldarg.s    greyedOutTooltipCallback
      IL_0082:  ldstr      "NoHomework"
      IL_0087:  ldc.i4.1
      IL_0088:  newarr     [mscorlib]System.Object
      IL_008d:  stloc.3
      IL_008e:  ldloc.3
      IL_008f:  ldc.i4.0
      IL_0090:  ldarg.0
      IL_0091:  stelem.ref
      IL_0092:  ldloc.3
      IL_0093:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                             object[])
      IL_0098:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
      IL_009d:  stind.ref
      IL_009e:  ldc.i4.0
      IL_009f:  ret

      IL_00a0:  ldarg.1
      IL_00a1:  ldind.ref
      IL_00a2:  ldarg.0
      IL_00a3:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework::IsSimsHomework(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_00a8:  brtrue.s   IL_00d3

      IL_00aa:  ldarg.3
      IL_00ab:  brfalse.s  IL_00d1

      IL_00ad:  ldarg.2
      IL_00ae:  brtrue.s   IL_00d1

      IL_00b0:  ldarg.s    greyedOutTooltipCallback
      IL_00b2:  ldstr      "NoHomework"
      IL_00b7:  ldc.i4.1
      IL_00b8:  newarr     [mscorlib]System.Object
      IL_00bd:  stloc.s    V_4
      IL_00bf:  ldloc.s    V_4
      IL_00c1:  ldc.i4.0
      IL_00c2:  ldarg.0
      IL_00c3:  stelem.ref
      IL_00c4:  ldloc.s    V_4
      IL_00c6:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                             object[])
      IL_00cb:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
      IL_00d0:  stind.ref
      IL_00d1:  ldc.i4.0
      IL_00d2:  ret

      IL_00d3:  ldarg.1
      IL_00d4:  ldind.ref
      IL_00d5:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_InUse()
      IL_00da:  brfalse.s  IL_010f

      IL_00dc:  ldarg.1
      IL_00dd:  ldind.ref
      IL_00de:  ldarg.0
      IL_00df:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::IsActorUsingMe(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_00e4:  brtrue.s   IL_010f

      IL_00e6:  ldarg.3
      IL_00e7:  brfalse.s  IL_010d

      IL_00e9:  ldarg.2
      IL_00ea:  brtrue.s   IL_010d

      IL_00ec:  ldarg.s    greyedOutTooltipCallback
      IL_00ee:  ldstr      "HomeworkInUse"
      IL_00f3:  ldc.i4.1
      IL_00f4:  newarr     [mscorlib]System.Object
      IL_00f9:  stloc.s    V_5
      IL_00fb:  ldloc.s    V_5
      IL_00fd:  ldc.i4.0
      IL_00fe:  ldarg.0
      IL_00ff:  stelem.ref
      IL_0100:  ldloc.s    V_5
      IL_0102:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                             object[])
      IL_0107:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
      IL_010c:  stind.ref
      IL_010d:  ldc.i4.0
      IL_010e:  ret

      IL_010f:  ldarg.1
      IL_0110:  ldind.ref
      IL_0111:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework::IsComplete()
      IL_0116:  brfalse.s  IL_0141

      IL_0118:  ldarg.3
      IL_0119:  brfalse.s  IL_013f

      IL_011b:  ldarg.2
      IL_011c:  brtrue.s   IL_013f

      IL_011e:  ldarg.s    greyedOutTooltipCallback
      IL_0120:  ldstr      "HomeworkCompleted"
      IL_0125:  ldc.i4.1
      IL_0126:  newarr     [mscorlib]System.Object
      IL_012b:  stloc.s    V_6
      IL_012d:  ldloc.s    V_6
      IL_012f:  ldc.i4.0
      IL_0130:  ldarg.0
      IL_0131:  stelem.ref
      IL_0132:  ldloc.s    V_6
      IL_0134:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                             object[])
      IL_0139:  call       class [UI]Sims3.SimIFace.GreyedOutTooltipCallback [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::CreateTooltipCallback(string)
      IL_013e:  stind.ref
      IL_013f:  ldc.i4.0
      IL_0140:  ret

      IL_0141:  ldarg.2
      IL_0142:  brfalse.s  IL_0153

      IL_0144:  ldarg.0
      IL_0145:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Motives()
      IL_014a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.Motives::StressInteractionTest()
      IL_014f:  brfalse.s  IL_0153

      IL_0151:  ldc.i4.0
      IL_0152:  ret

      IL_0153:  ldc.i4.1
      IL_0154:  ret
    } // end of method DoHomeworkOnTablet::HomeworkTest

    .method public hidebysig instance class [SimIFace]Sims3.SimIFace.StateMachineClient 
            GetDefaultStateMachine() cil managed
    {
      // Code size       174 (0xae)
      .maxstack  5
      .locals init (class [SimIFace]Sims3.SimIFace.StateMachineClient V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0006:  callvirt   instance class [SimIFace]Sims3.SimIFace.IScriptProxy [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_Proxy()
      IL_000b:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [SimIFace]Sims3.SimIFace.ITask::get_ObjectId()
      IL_0010:  ldstr      "Tablet_store"
      IL_0015:  call       class [SimIFace]Sims3.SimIFace.StateMachineClient [SimIFace]Sims3.SimIFace.StateMachineClient::Acquire(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                  string)
      IL_001a:  stloc.0
      IL_001b:  ldloc.0
      IL_001c:  ldstr      "x"
      IL_0021:  ldarg.0
      IL_0022:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0027:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_002c:  ldloc.0
      IL_002d:  ldstr      "tablet"
      IL_0032:  ldarg.0
      IL_0033:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0038:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetActor(string,
                                                                                               class [SimIFace]Sims3.SimIFace.IHasScriptProxy)
      IL_003d:  ldloc.0
      IL_003e:  ldstr      "isSitting"
      IL_0043:  ldarg.0
      IL_0044:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0049:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
      IL_004e:  ldc.i4     0x40f3d23
      IL_0053:  ldarg.0
      IL_0054:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0059:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                              class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_005e:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0063:  ldloc.0
      IL_0064:  ldstr      "inInventory"
      IL_0069:  ldarg.0
      IL_006a:  ldfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_006f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                   bool)
      IL_0074:  ldloc.0
      IL_0075:  ldstr      "x"
      IL_007a:  ldstr      "Enter"
      IL_007f:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::EnterState(string,
                                                                                                 string)
      IL_0084:  ldloc.0
      IL_0085:  ldc.i4.s   104
      IL_0087:  ldarg.0
      IL_0088:  ldftn      instance void Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::HomeworkVideoOn(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                        class [SimIFace]Sims3.SimIFace.IEvent)
      IL_008e:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_0093:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_0098:  ldloc.0
      IL_0099:  ldc.i4.s   105
      IL_009b:  ldarg.0
      IL_009c:  ldftn      instance void Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::HomeworkVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                         class [SimIFace]Sims3.SimIFace.IEvent)
      IL_00a2:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                          native int)
      IL_00a7:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                   class [SimIFace]Sims3.SimIFace.SacsEventHandler)
      IL_00ac:  ldloc.0
      IL_00ad:  ret
    } // end of method DoHomeworkOnTablet::GetDefaultStateMachine

    .method public hidebysig instance bool 
            DoMyHomework() cil managed
    {
      // Code size       512 (0x200)
      .maxstack  7
      .locals init (bool V_0,
               bool V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_0007:  ldarg.0
      IL_0008:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000d:  ldarg.0
      IL_000e:  ldflda     class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_0013:  ldarg.0
      IL_0014:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0019:  call       bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::HomeworkTest(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework&,
                                                                                            bool)
      IL_001e:  brtrue.s   IL_0032

      IL_0020:  ldarg.0
      IL_0021:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0026:  ldc.i4     0x400
      IL_002b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0030:  ldc.i4.0
      IL_0031:  ret

      IL_0032:  ldarg.0
      IL_0033:  ldfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_0038:  brfalse.s  IL_0077

      IL_003a:  ldarg.0
      IL_003b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0040:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
      IL_0045:  ldarg.0
      IL_0046:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_004b:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToRemove(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
      IL_0050:  brtrue.s   IL_0064

      IL_0052:  ldarg.0
      IL_0053:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0058:  ldc.i4     0x400
      IL_005d:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_0062:  ldc.i4.0
      IL_0063:  ret

      IL_0064:  ldarg.0
      IL_0065:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_006a:  ldarg.0
      IL_006b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0070:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0075:  br.s       IL_009c

      IL_0077:  ldarg.0
      IL_0078:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_007d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::GetObjectInRightHand()
      IL_0082:  ldarg.0
      IL_0083:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0088:  beq.s      IL_009c

      IL_008a:  ldarg.0
      IL_008b:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0090:  ldc.i4     0x10000
      IL_0095:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_009a:  ldc.i4.0
      IL_009b:  ret

      IL_009c:  ldarg.0
      IL_009d:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_00a2:  ldarg.0
      IL_00a3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00a8:  ldarg.0
      IL_00a9:  ldfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_00ae:  ldarg.0
      IL_00af:  ldarg.0
      IL_00b0:  ldftn      instance class [SimIFace]Sims3.SimIFace.StateMachineClient Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::GetDefaultStateMachine()
      IL_00b6:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate::.ctor(object,
                                                                                                                                  native int)
      IL_00bb:  ldarg.0
      IL_00bc:  ldflda     class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_00c1:  callvirt   instance void Sims3.Store.Objects.Tablet::StartUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                      bool,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate,
                                                                                      class [SimIFace]Sims3.SimIFace.StateMachineClient&)
      IL_00c6:  ldarg.0
      IL_00c7:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_00cc:  ldarg.0
      IL_00cd:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00d2:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddToUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_00d7:  ldarg.0
      IL_00d8:  ldc.i4.0
      IL_00d9:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardEntry(bool)
      IL_00de:  ldarg.0
      IL_00df:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdates()
      IL_00e4:  ldarg.0
      IL_00e5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00ea:  ldc.i8     0xb82d0015b9294690
      IL_00f3:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_00f8:  brfalse.s  IL_010b

      IL_00fa:  ldarg.0
      IL_00fb:  ldc.i4     0x10f38da
      IL_0100:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.TuningValues.TraitTuning::get_WorkaholicTraitHomeworkFunPerHour()
      IL_0105:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.MotiveDelta [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AddMotiveDelta(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                                                                                          float32)
      IL_010a:  pop
      IL_010b:  ldarg.0
      IL_010c:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0111:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
      IL_0116:  ldc.i4.s   24
      IL_0118:  conv.i8
      IL_0119:  ldsfld     float32 Sims3.Store.Objects.Tablet::kHomeworkSkillPPM
      IL_011e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::StartGainAndAddSkillIfNeeded(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames,
                                                                                                                                float32)
      IL_0123:  ldarg.0
      IL_0124:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0129:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_012e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::DisableLookAts()
      IL_0133:  ldarg.0
      IL_0134:  ldstr      "Do Homework"
      IL_0139:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::AnimateSim(string)
      IL_013e:  ldarg.0
      IL_013f:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0144:  ldc.r4     0.0
      IL_0149:  ldc.i4.1
      IL_014a:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Core.ProgressMeter::ShowProgressMeter(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           float32,
                                                                                                           valuetype [Sims3GameplaySystems]Sims3.Gameplay.Core.ProgressMeter/GlowType)
      IL_014f:  ldarg.0
      IL_0150:  ldc.i4     0xffa9bfff
      IL_0155:  ldarg.0
      IL_0156:  ldftn      instance void Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::LoopDel(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData)
      IL_015c:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction::.ctor(object,
                                                                                                                                         native int)
      IL_0161:  ldnull
      IL_0162:  ldc.r4     1.
      IL_0167:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::DoLoop(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason,
                                                                                                                                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/InsideLoopFunction,
                                                                                                                                                                                                               class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                                                                                                                               float32)
      IL_016c:  stloc.0
      IL_016d:  ldarg.0
      IL_016e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0173:  ldloc.0
      IL_0174:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Core.ProgressMeter::HideProgressMeter(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                           bool)
      IL_0179:  ldarg.0
      IL_017a:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_017f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LookAtManager()
      IL_0184:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.LookAtManager::EnableLookAts()
      IL_0189:  ldarg.0
      IL_018a:  ldloc.0
      IL_018b:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::EndCommodityUpdates(bool)
      IL_0190:  ldarg.0
      IL_0191:  ldarg.0
      IL_0192:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0197:  ldarg.0
      IL_0198:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_019d:  ldarg.0
      IL_019e:  ldfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_01a3:  callvirt   instance bool Sims3.Store.Objects.Tablet::ShouldPutInInventory(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                          bool)
      IL_01a8:  stfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_01ad:  ldloc.0
      IL_01ae:  ldarg.0
      IL_01af:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_01b4:  ldarg.0
      IL_01b5:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_01ba:  ldnull
      IL_01bb:  ldarg.0
      IL_01bc:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
      IL_01c1:  ldarg.0
      IL_01c2:  ldfld      bool Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mFromInventory
      IL_01c7:  callvirt   instance bool Sims3.Store.Objects.Tablet::StopUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book,
                                                                                     class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                     bool)
      IL_01cc:  and
      IL_01cd:  stloc.0
      IL_01ce:  ldarg.0
      IL_01cf:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_01d4:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
      IL_01d9:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Count()
      IL_01de:  ldc.i4.0
      IL_01df:  ceq
      IL_01e1:  ldc.i4.0
      IL_01e2:  ceq
      IL_01e4:  stloc.1
      IL_01e5:  ldarg.0
      IL_01e6:  ldloc.1
      IL_01e7:  ldloc.1
      IL_01e8:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::StandardExit(bool,
                                                                                                                             bool)
      IL_01ed:  ldarg.0
      IL_01ee:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_01f3:  ldarg.0
      IL_01f4:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_01f9:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveFromUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_01fe:  ldloc.0
      IL_01ff:  ret
    } // end of method DoHomeworkOnTablet::DoMyHomework

    .method public hidebysig instance void 
            LoopDel(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                    valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData loopData) cil managed
    {
      // Code size       176 (0xb0)
      .maxstack  5
      .locals init (float32 V_0)
      IL_0000:  ldc.i4     0x93
      IL_0005:  ldarg.0
      IL_0006:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000b:  ldnull
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_0012:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.HomeworkEvent::.ctor(valuetype [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTypeId,
                                                                                                                         class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                         class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                         class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework)
      IL_0017:  call       class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.EventTracker::SendEvent(class [Sims3GameplaySystems]Sims3.Gameplay.EventSystem.Event)
      IL_001c:  pop
      IL_001d:  ldarg.0
      IL_001e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_0023:  ldarg.0
      IL_0024:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0029:  ldc.i4.0
      IL_002a:  ldc.i4.0
      IL_002b:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework::GetCompletionRate(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                                   bool,
                                                                                                                                   bool)
      IL_0030:  ldsfld     float32 Sims3.Store.Objects.Tablet::kHomeworkRateMultiplier
      IL_0035:  mul
      IL_0036:  stloc.0
      IL_0037:  ldarg.0
      IL_0038:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_003d:  ldarg.0
      IL_003e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0043:  ldloc.0
      IL_0044:  ldarga.s   loopData
      IL_0046:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance/LoopData::mDeltaTime
      IL_004b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework::UpdateProgress(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                             float32,
                                                                                                                             float32)
      IL_0050:  ldarg.0
      IL_0051:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_0056:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework::IsComplete()
      IL_005b:  brfalse.s  IL_00af

      IL_005d:  ldarg.0
      IL_005e:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0063:  ldc.i8     0xb82d0015b9294560
      IL_006c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
      IL_0071:  brfalse.s  IL_0082

      IL_0073:  ldarg.0
      IL_0074:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0079:  ldc.i4.s   50
      IL_007b:  ldc.i4.4
      IL_007c:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
      IL_0081:  pop
      IL_0082:  ldarg.0
      IL_0083:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mHomework
      IL_0088:  ldarg.0
      IL_0089:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_008e:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Miscellaneous.Homework::IsSimsHomework(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0093:  brfalse.s  IL_00a2

      IL_0095:  ldarg.0
      IL_0096:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_009b:  ldc.i4.7
      IL_009c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.ChildAndTeenUpdates.Punishment::ApplyAbsolvingActionToSim(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                                               valuetype [Sims3GameplaySystems]Sims3.Gameplay.ChildAndTeenUpdates.Punishment/AbsolvingActionType)
      IL_00a1:  pop
      IL_00a2:  ldarg.0
      IL_00a3:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_00a8:  ldc.i4.s   64
      IL_00aa:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::AddExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
      IL_00af:  ret
    } // end of method DoHomeworkOnTablet::LoopDel

    .method public hidebysig instance void 
            HomeworkVideoOn(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                            class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       73 (0x49)
      .maxstack  3
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_0006:  brfalse.s  IL_0010

      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  ldarg.2
      IL_000b:  call       instance void Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::HomeworkVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                                         class [SimIFace]Sims3.SimIFace.IEvent)
      IL_0010:  ldarg.0
      IL_0011:  ldstr      "store_tablet_homework"
      IL_0016:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
      IL_001b:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_0026:  ldarg.0
      IL_0027:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_002c:  ldstr      "_FX_0_screen"
      IL_0031:  call       valuetype [SimIFace]Sims3.SimIFace.Slot [SimIFace]Sims3.SimIFace.Slots::Hash(string)
      IL_0036:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                         valuetype [SimIFace]Sims3.SimIFace.Slot)
      IL_003b:  pop
      IL_003c:  ldarg.0
      IL_003d:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_0042:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
      IL_0047:  pop
      IL_0048:  ret
    } // end of method DoHomeworkOnTablet::HomeworkVideoOn

    .method public hidebysig instance void 
            HomeworkVideoOff(class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                             class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
    {
      // Code size       39 (0x27)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_0006:  brfalse.s  IL_0026

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
      IL_0013:  pop
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
      IL_001f:  ldarg.0
      IL_0020:  ldnull
      IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::mScreenVFX
      IL_0026:  ret
    } // end of method DoHomeworkOnTablet::HomeworkVideoOff

    .method public hidebysig instance void 
            PutInInventoryFailureCallback(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim s,
                                          float32 f) cil managed
    {
      // Code size       70 (0x46)
      .maxstack  6
      .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0006:  brtrue.s   IL_0009

      IL_0008:  ret

      IL_0009:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_000e:  ldarg.0
      IL_000f:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0014:  ldarg.0
      IL_0015:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_001a:  ldarg.0
      IL_001b:  call       instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::GetPriority()
      IL_0020:  ldarg.0
      IL_0021:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Autonomous()
      IL_0026:  ldarg.0
      IL_0027:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_CancellableByPlayer()
      IL_002c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                        bool,
                                                                                                                                                                                                        bool)
      IL_0031:  stloc.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_0038:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_003d:  ldloc.0
      IL_003e:  ldc.i4.1
      IL_003f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                                bool)
      IL_0044:  pop
      IL_0045:  ret
    } // end of method DoHomeworkOnTablet::PutInInventoryFailureCallback

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method DoHomeworkOnTablet::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/DoHomeworkOnTablet/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::Singleton
      IL_000a:  ret
    } // end of method DoHomeworkOnTablet::.cctor

  } // end of class DoHomeworkOnTablet

  .class auto ansi nested public beforefieldinit PutTabletDown
         extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>
  {
    .class auto ansi nested public beforefieldinit Definition
           extends class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PutTabletDown>
    {
      .method public hidebysig virtual instance bool 
              Test(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                   class Sims3.Store.Objects.Tablet target,
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
                                 class Sims3.Store.Objects.Tablet target,
                                 class [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.InteractionObjectPair iop) cil managed
      {
        // Code size       17 (0x11)
        .maxstack  8
        IL_0000:  ldstr      "PutDown"
        IL_0005:  ldc.i4.0
        IL_0006:  newarr     [mscorlib]System.Object
        IL_000b:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                               object[])
        IL_0010:  ret
      } // end of method Definition::GetInteractionName

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition`3<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet,class Sims3.Store.Objects.Tablet/PutTabletDown>::.ctor()
        IL_0006:  ret
      } // end of method Definition::.ctor

    } // end of class Definition

    .field public static class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Singleton
    .method public hidebysig virtual instance bool 
            Run() cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_0006:  ldarg.0
      IL_0007:  ldfld      !0 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Actor
      IL_000c:  callvirt   instance void Sims3.Store.Objects.Tablet::PutTabletAway(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_0011:  ldc.i4.1
      IL_0012:  ret
    } // end of method PutTabletDown::Run

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.Interaction`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::.ctor()
      IL_0006:  ret
    } // end of method PutTabletDown::.ctor

    .method public hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Sims3.Store.Objects.Tablet/PutTabletDown/Definition::.ctor()
      IL_0005:  stsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
      IL_000a:  ret
    } // end of method PutTabletDown::.cctor

  } // end of class PutTabletDown

  .class auto ansi nested public beforefieldinit ListeningToAudioProgramBuffStore
         extends [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff
  {
    .class auto ansi nested public beforefieldinit BuffInstanceLTAPBS
           extends [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance
    {
      .field public static literal string kEarbudMaleVFXName = "store_tablet_earbudM"
      .field public static literal string kEarbudFemaleVFXName = "store_tablet_earbudF"
      .field public static literal string kEarbudChildMaleVFXName = "store_tablet_earbudCM"
      .field public static literal string kEarbudChildFemaleVFXName = "store_tablet_earbudCF"
      .field public static literal string kEarbudAudioName = "tablet_listen_lp"
      .field public class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim mSim
      .field public class Sims3.Store.Objects.Tablet mTablet
      .field public valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames mSkill
      .field public int32 mNewLevel
      .field public class [SimIFace]Sims3.SimIFace.VisualEffect mEarbudVFX
      .field public valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle mStillValidAlarm
      .field public class [SimIFace]Sims3.SimIFace.ObjectSound mSpeakerAudioHandle
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_0006:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mStillValidAlarm
        IL_000b:  ldarg.0
        IL_000c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::.ctor()
        IL_0011:  ret
      } // end of method BuffInstanceLTAPBS::.ctor

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
        IL_0006:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mStillValidAlarm
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
      } // end of method BuffInstanceLTAPBS::.ctor

      .method public hidebysig specialname virtual 
              instance object  get_DescriptionParameter3() cil managed
      {
        // Code size       28 (0x1c)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_0006:  box        [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
        IL_000b:  callvirt   instance string [mscorlib]System.Object::ToString()
        IL_0010:  ldc.i4.0
        IL_0011:  newarr     [mscorlib]System.Object
        IL_0016:  call       string Sims3.Store.Objects.Tablet::LocalizeAudioProgram(string,
                                                                                     object[])
        IL_001b:  ret
      } // end of method BuffInstanceLTAPBS::get_DescriptionParameter3

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
        IL_0018:  newobj     instance void Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                                 valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                                 int32,
                                                                                                                                 float32)
        IL_001d:  ret
      } // end of method BuffInstanceLTAPBS::Clone

      .method public hidebysig instance void 
              Setup(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim sim) cil managed
      {
        // Code size       457 (0x1c9)
        .maxstack  9
        .locals init (class Sims3.Store.Objects.Tablet/ListenToAudioProgram V_0,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill V_1,
                 int32 V_2,
                 class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill V_3,
                 float32 V_4,
                 object[] V_5)
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_000d:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
        IL_0012:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::GetCurrentInteraction()
        IL_0017:  isinst     Sims3.Store.Objects.Tablet/ListenToAudioProgram
        IL_001c:  stloc.0
        IL_001d:  ldloc.0
        IL_001e:  brtrue.s   IL_002c

        IL_0020:  ldarg.0
        IL_0021:  ldc.r4     0.0
        IL_0026:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_002b:  ret

        IL_002c:  ldarg.0
        IL_002d:  ldloc.0
        IL_002e:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
        IL_0033:  stfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mTablet
        IL_0038:  ldarg.0
        IL_0039:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mTablet
        IL_003e:  brtrue.s   IL_004c

        IL_0040:  ldarg.0
        IL_0041:  ldc.r4     0.0
        IL_0046:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_004b:  ret

        IL_004c:  ldarg.0
        IL_004d:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mTablet
        IL_0052:  ldarg.0
        IL_0053:  stfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
        IL_0058:  ldarg.0
        IL_0059:  ldloc.0
        IL_005a:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListenToAudioProgram::get_SkillProgram()
        IL_005f:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_0064:  ldarg.0
        IL_0065:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_006a:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetStaticSkill(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
        IL_006f:  stloc.1
        IL_0070:  ldarg.0
        IL_0071:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0076:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
        IL_007b:  ldarg.0
        IL_007c:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_0081:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetSkillLevel(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
        IL_0086:  stloc.2
        IL_0087:  ldloc.2
        IL_0088:  ldloc.1
        IL_0089:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::get_MaxSkillLevel()
        IL_008e:  blt.s      IL_009c

        IL_0090:  ldarg.0
        IL_0091:  ldc.r4     0.0
        IL_0096:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_009b:  ret

        IL_009c:  ldarg.0
        IL_009d:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_00a2:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
        IL_00a7:  ldarg.0
        IL_00a8:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_00ad:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::AddElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
        IL_00b2:  pop
        IL_00b3:  ldarg.0
        IL_00b4:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_00b9:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
        IL_00be:  ldarg.0
        IL_00bf:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_00c4:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
        IL_00c9:  stloc.3
        IL_00ca:  ldloc.3
        IL_00cb:  brtrue.s   IL_00d9

        IL_00cd:  ldarg.0
        IL_00ce:  ldc.r4     0.0
        IL_00d3:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_00d8:  ret

        IL_00d9:  ldarg.0
        IL_00da:  ldloc.3
        IL_00db:  ldfld      int32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::SkillLevel
        IL_00e0:  ldc.i4.1
        IL_00e1:  add
        IL_00e2:  stfld      int32 Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mNewLevel
        IL_00e7:  ldarg.0
        IL_00e8:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_00ed:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
        IL_00f2:  ldarg.0
        IL_00f3:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_00f8:  ldsfld     float32 Sims3.Store.Objects.Tablet::kSkillGainRate
        IL_00fd:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::TimeToGainLevel(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames,
                                                                                                                        float32)
        IL_0102:  stloc.s    V_4
        IL_0104:  ldarg.0
        IL_0105:  ldloc.s    V_4
        IL_0107:  ldc.r4     0.0
        IL_010c:  call       float32 [mscorlib]System.Math::Max(float32,
                                                                float32)
        IL_0111:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_0116:  ldarg.0
        IL_0117:  ldc.i4.4
        IL_0118:  newarr     [mscorlib]System.Object
        IL_011d:  stloc.s    V_5
        IL_011f:  ldloc.s    V_5
        IL_0121:  ldc.i4.0
        IL_0122:  ldsfld     string Sims3.Store.Objects.Tablet::sLocalizationKeyAudio
        IL_0127:  stelem.ref
        IL_0128:  ldloc.s    V_5
        IL_012a:  ldc.i4.1
        IL_012b:  ldstr      ":"
        IL_0130:  stelem.ref
        IL_0131:  ldloc.s    V_5
        IL_0133:  ldc.i4.2
        IL_0134:  ldarg.0
        IL_0135:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_013a:  box        [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
        IL_013f:  stelem.ref
        IL_0140:  ldloc.s    V_5
        IL_0142:  ldc.i4.3
        IL_0143:  ldstr      "Description"
        IL_0148:  stelem.ref
        IL_0149:  ldloc.s    V_5
        IL_014b:  call       string [mscorlib]System.String::Concat(object[])
        IL_0150:  stfld      string [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mDescription
        IL_0155:  ldarg.0
        IL_0156:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_015b:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/StopAudioProgram::Singleton
        IL_0160:  ldc.i4.1
        IL_0161:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition,
                                                                                                                     bool)
        IL_0166:  ldarg.0
        IL_0167:  ldarg.0
        IL_0168:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_016d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsFemale()
        IL_0172:  ldarg.0
        IL_0173:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0178:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
        IL_017d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_Child()
        IL_0182:  call       instance void Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::AttachEarbud(bool,
                                                                                                                                        bool)
        IL_0187:  ldarg.0
        IL_0188:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mStillValidAlarm
        IL_018d:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_0192:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::op_Equality(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle,
                                                                                                          valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0197:  brfalse.s  IL_01c8

        IL_0199:  ldarg.0
        IL_019a:  ldarg.0
        IL_019b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_01a0:  ldc.r4     1.
        IL_01a5:  ldc.i4.1
        IL_01a6:  ldarg.0
        IL_01a7:  ldftn      instance void Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::StillValidAlarmCallback()
        IL_01ad:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback::.ctor(object,
                                                                                                                    native int)
        IL_01b2:  ldc.r4     1.
        IL_01b7:  ldc.i4.1
        IL_01b8:  ldstr      "Cannot listen anymore"
        IL_01bd:  ldc.i4.1
        IL_01be:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddAlarmRepeating(float32,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                                        class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmTimerCallback,
                                                                                                                                                                                        float32,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.TimeUnit,
                                                                                                                                                                                        string,
                                                                                                                                                                                        valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmType)
        IL_01c3:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mStillValidAlarm
        IL_01c8:  ret
      } // end of method BuffInstanceLTAPBS::Setup

      .method public hidebysig instance void 
              Shutdown() cil managed
      {
        // Code size       169 (0xa9)
        .maxstack  2
        .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0006:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/StopAudioProgram::Singleton
        IL_000b:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveInteractionByType(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
        IL_0010:  ldarg.0
        IL_0011:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0016:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
        IL_001b:  ldarg.0
        IL_001c:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_0021:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
        IL_0026:  stloc.0
        IL_0027:  ldloc.0
        IL_0028:  brfalse.s  IL_004b

        IL_002a:  ldarg.0
        IL_002b:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
        IL_0030:  ldc.r4     0.0
        IL_0035:  bgt.un.s   IL_004b

        IL_0037:  ldloc.0
        IL_0038:  ldfld      int32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::SkillLevel
        IL_003d:  ldarg.0
        IL_003e:  ldfld      int32 Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mNewLevel
        IL_0043:  bge.s      IL_004b

        IL_0045:  ldloc.0
        IL_0046:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::ForceGainPointsForLevelUp()
        IL_004b:  ldarg.0
        IL_004c:  call       instance void Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::DetachEarbud()
        IL_0051:  ldarg.0
        IL_0052:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mStillValidAlarm
        IL_0057:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_005c:  call       bool [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::op_Inequality(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle,
                                                                                                            valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0061:  brfalse.s  IL_007f

        IL_0063:  ldarg.0
        IL_0064:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0069:  ldarg.0
        IL_006a:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mStillValidAlarm
        IL_006f:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveAlarm(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle)
        IL_0074:  ldarg.0
        IL_0075:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle::kInvalidHandle
        IL_007a:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Utilities.AlarmHandle Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mStillValidAlarm
        IL_007f:  ldarg.0
        IL_0080:  ldnull
        IL_0081:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0086:  ldarg.0
        IL_0087:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mTablet
        IL_008c:  ldnull
        IL_008d:  stfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
        IL_0092:  ldarg.0
        IL_0093:  ldnull
        IL_0094:  stfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mTablet
        IL_0099:  ldarg.0
        IL_009a:  ldc.i4.0
        IL_009b:  conv.i8
        IL_009c:  stfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_00a1:  ldarg.0
        IL_00a2:  ldc.i4.0
        IL_00a3:  stfld      int32 Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mNewLevel
        IL_00a8:  ret
      } // end of method BuffInstanceLTAPBS::Shutdown

      .method public hidebysig instance void 
              AttachEarbud(bool isFemale,
                           bool isChild) cil managed
      {
        // Code size       154 (0x9a)
        .maxstack  4
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_0006:  brfalse.s  IL_000e

        IL_0008:  ldarg.0
        IL_0009:  call       instance void Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::DetachEarbud()
        IL_000e:  ldarg.2
        IL_000f:  brfalse.s  IL_002d

        IL_0011:  ldarg.0
        IL_0012:  ldarg.1
        IL_0013:  brtrue.s   IL_001c

        IL_0015:  ldstr      "store_tablet_earbudCM"
        IL_001a:  br.s       IL_0021

        IL_001c:  ldstr      "store_tablet_earbudCF"
        IL_0021:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
        IL_0026:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_002b:  br.s       IL_0047

        IL_002d:  ldarg.0
        IL_002e:  ldarg.1
        IL_002f:  brtrue.s   IL_0038

        IL_0031:  ldstr      "store_tablet_earbudM"
        IL_0036:  br.s       IL_003d

        IL_0038:  ldstr      "store_tablet_earbudF"
        IL_003d:  call       class [SimIFace]Sims3.SimIFace.VisualEffect [SimIFace]Sims3.SimIFace.VisualEffect::Create(string)
        IL_0042:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_0047:  ldarg.0
        IL_0048:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_004d:  ldarg.0
        IL_004e:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0053:  ldsfld     valuetype [SimIFace]Sims3.SimIFace.Slot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/ContainmentSlots::RightEarring
        IL_0058:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::ParentTo(class [SimIFace]Sims3.SimIFace.IHasScriptProxy,
                                                                                           valuetype [SimIFace]Sims3.SimIFace.Slot)
        IL_005d:  pop
        IL_005e:  ldarg.0
        IL_005f:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_0064:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Start()
        IL_0069:  pop
        IL_006a:  ldarg.0
        IL_006b:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSpeakerAudioHandle
        IL_0070:  brtrue.s   IL_0099

        IL_0072:  ldarg.0
        IL_0073:  ldarg.0
        IL_0074:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0079:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
        IL_007e:  ldstr      "tablet_listen_lp"
        IL_0083:  newobj     instance void [SimIFace]Sims3.SimIFace.ObjectSound::.ctor(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                       string)
        IL_0088:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSpeakerAudioHandle
        IL_008d:  ldarg.0
        IL_008e:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSpeakerAudioHandle
        IL_0093:  callvirt   instance bool [SimIFace]Sims3.SimIFace.Sound::StartLoop()
        IL_0098:  pop
        IL_0099:  ret
      } // end of method BuffInstanceLTAPBS::AttachEarbud

      .method public hidebysig instance void 
              DetachEarbud() cil managed
      {
        // Code size       76 (0x4c)
        .maxstack  2
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_0006:  brfalse.s  IL_0026

        IL_0008:  ldarg.0
        IL_0009:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_000e:  callvirt   instance bool [SimIFace]Sims3.SimIFace.VisualEffect::Stop()
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  ldfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_001a:  callvirt   instance void [SimIFace]Sims3.SimIFace.VisualEffect::Dispose()
        IL_001f:  ldarg.0
        IL_0020:  ldnull
        IL_0021:  stfld      class [SimIFace]Sims3.SimIFace.VisualEffect Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mEarbudVFX
        IL_0026:  ldarg.0
        IL_0027:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSpeakerAudioHandle
        IL_002c:  brfalse.s  IL_004b

        IL_002e:  ldarg.0
        IL_002f:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSpeakerAudioHandle
        IL_0034:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Stop()
        IL_0039:  ldarg.0
        IL_003a:  ldfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSpeakerAudioHandle
        IL_003f:  callvirt   instance void [SimIFace]Sims3.SimIFace.Sound::Dispose()
        IL_0044:  ldarg.0
        IL_0045:  ldnull
        IL_0046:  stfld      class [SimIFace]Sims3.SimIFace.ObjectSound Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSpeakerAudioHandle
        IL_004b:  ret
      } // end of method BuffInstanceLTAPBS::DetachEarbud

      .method public hidebysig instance void 
              StillValidAlarmCallback() cil managed
      {
        // Code size       186 (0xba)
        .maxstack  3
        .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill V_0,
                 float32 V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0006:  brtrue.s   IL_0009

        IL_0008:  ret

        IL_0009:  ldarg.0
        IL_000a:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_000f:  ldc.i4.0
        IL_0010:  conv.i8
        IL_0011:  beq.s      IL_006b

        IL_0013:  ldarg.0
        IL_0014:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0019:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
        IL_001e:  ldarg.0
        IL_001f:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
        IL_0024:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
        IL_0029:  stloc.0
        IL_002a:  ldloc.0
        IL_002b:  brfalse.s  IL_006b

        IL_002d:  ldsfld     float32 Sims3.Store.Objects.Tablet::kSkillGainRate
        IL_0032:  ldc.r4     60.
        IL_0037:  div
        IL_0038:  stloc.1
        IL_0039:  ldloc.0
        IL_003a:  callvirt   instance int32[] [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::get_PointsForNextLevel()
        IL_003f:  ldloc.0
        IL_0040:  ldfld      int32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::SkillLevel
        IL_0045:  ldelem.i4
        IL_0046:  conv.r4
        IL_0047:  ldloc.0
        IL_0048:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::SkillPoints
        IL_004d:  sub
        IL_004e:  ldc.r4     2.
        IL_0053:  ldloc.1
        IL_0054:  mul
        IL_0055:  blt.un.s   IL_0062

        IL_0057:  ldloc.0
        IL_0058:  ldloc.1
        IL_0059:  ldc.i4.0
        IL_005a:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::AddPoints(float32,
                                                                                                        bool)
        IL_005f:  pop
        IL_0060:  br.s       IL_006b

        IL_0062:  ldloc.0
        IL_0063:  ldloc.1
        IL_0064:  ldc.i4.1
        IL_0065:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::AddPoints(float32,
                                                                                                        bool)
        IL_006a:  pop
        IL_006b:  ldarg.0
        IL_006c:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0071:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_HasBeenDestroyed()
        IL_0076:  brtrue.s   IL_00a0

        IL_0078:  ldarg.0
        IL_0079:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mTablet
        IL_007e:  brfalse.s  IL_00a0

        IL_0080:  ldarg.0
        IL_0081:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mTablet
        IL_0086:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_HasBeenDestroyed()
        IL_008b:  brtrue.s   IL_00a0

        IL_008d:  ldarg.0
        IL_008e:  ldfld      class Sims3.Store.Objects.Tablet Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mTablet
        IL_0093:  ldarg.0
        IL_0094:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0099:  callvirt   instance bool Sims3.Store.Objects.Tablet::CanListenToAudioProgram(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
        IL_009e:  brtrue.s   IL_00b9

        IL_00a0:  ldarg.0
        IL_00a1:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_00a6:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
        IL_00ab:  ldc.i8     0x9a7f5f1919df86c1
        IL_00b4:  callvirt   instance void class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::RemoveElement(uint64)
        IL_00b9:  ret
      } // end of method BuffInstanceLTAPBS::StillValidAlarmCallback

      .method public hidebysig instance bool 
              RemoveMe() cil managed
      {
        // Code size       37 (0x25)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0006:  brtrue.s   IL_000a

        IL_0008:  ldc.i4.0
        IL_0009:  ret

        IL_000a:  ldarg.0
        IL_000b:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSim
        IL_0010:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
        IL_0015:  ldc.i8     0x9a7f5f1919df86c1
        IL_001e:  callvirt   instance void class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::RemoveElement(uint64)
        IL_0023:  ldc.i4.1
        IL_0024:  ret
      } // end of method BuffInstanceLTAPBS::RemoveMe

      .property instance object DescriptionParameter3()
      {
        .get instance object Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::get_DescriptionParameter3()
      } // end of property BuffInstanceLTAPBS::DescriptionParameter3
    } // end of class BuffInstanceLTAPBS

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff/BuffData info) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff/BuffData)
      IL_0007:  ret
    } // end of method ListeningToAudioProgramBuffStore::.ctor

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
      IL_0013:  newobj     instance void Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::.ctor(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff,
                                                                                                                               valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                               int32,
                                                                                                                               float32)
      IL_0018:  ret
    } // end of method ListeningToAudioProgramBuffStore::CreateBuffInstance

    .method public hidebysig virtual instance bool 
            ShouldAdd(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                      valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.MoodAxis axisEffected,
                      int32 moodValue) cil managed
    {
      // Code size       68 (0x44)
      .maxstack  4
      .locals init (class Sims3.Store.Objects.Tablet/ListenToAudioProgram V_0,
               class Sims3.Store.Objects.Tablet V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  ldarg.3
      IL_0004:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::ShouldAdd(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager,
                                                                                                           valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.MoodAxis,
                                                                                                           int32)
      IL_0009:  brtrue.s   IL_000d

      IL_000b:  ldc.i4.0
      IL_000c:  ret

      IL_000d:  ldarg.1
      IL_000e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::get_Actor()
      IL_0013:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
      IL_0018:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::GetCurrentInteraction()
      IL_001d:  isinst     Sims3.Store.Objects.Tablet/ListenToAudioProgram
      IL_0022:  stloc.0
      IL_0023:  ldloc.0
      IL_0024:  brtrue.s   IL_0028

      IL_0026:  ldc.i4.0
      IL_0027:  ret

      IL_0028:  ldloc.0
      IL_0029:  ldfld      !1 class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance`2<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,class Sims3.Store.Objects.Tablet>::Target
      IL_002e:  stloc.1
      IL_002f:  ldloc.1
      IL_0030:  brfalse.s  IL_0040

      IL_0032:  ldloc.1
      IL_0033:  ldarg.1
      IL_0034:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::get_Actor()
      IL_0039:  callvirt   instance bool Sims3.Store.Objects.Tablet::CanListenToAudioProgram(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_003e:  brtrue.s   IL_0042

      IL_0040:  ldc.i4.0
      IL_0041:  ret

      IL_0042:  ldc.i4.1
      IL_0043:  ret
    } // end of method ListeningToAudioProgramBuffStore::ShouldAdd

    .method public hidebysig virtual instance void 
            OnAddition(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                       class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance bi,
                       bool travelReaddition) cil managed
    {
      // Code size       43 (0x2b)
      .maxstack  4
      .locals init (class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  ldarg.3
      IL_0004:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::OnAddition(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager,
                                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,
                                                                                                            bool)
      IL_0009:  ldarg.2
      IL_000a:  isinst     Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS
      IL_000f:  stloc.0
      IL_0010:  ldloc.0
      IL_0011:  brtrue.s   IL_001e

      IL_0013:  ldarg.2
      IL_0014:  ldc.r4     0.0
      IL_0019:  stfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance::mTimeoutCount
      IL_001e:  ldloc.0
      IL_001f:  ldarg.1
      IL_0020:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::get_Actor()
      IL_0025:  callvirt   instance void Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::Setup(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
      IL_002a:  ret
    } // end of method ListeningToAudioProgramBuffStore::OnAddition

    .method public hidebysig virtual instance void 
            OnRemoval(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager bm,
                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance bi) cil managed
    {
      // Code size       25 (0x19)
      .maxstack  3
      .locals init (class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS V_0)
      IL_0000:  ldarg.2
      IL_0001:  isinst     Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0010

      IL_000a:  ldloc.0
      IL_000b:  callvirt   instance void Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::Shutdown()
      IL_0010:  ldarg.0
      IL_0011:  ldarg.1
      IL_0012:  ldarg.2
      IL_0013:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Buff::OnRemoval(class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager,
                                                                                                           class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance)
      IL_0018:  ret
    } // end of method ListeningToAudioProgramBuffStore::OnRemoval

  } // end of class ListeningToAudioProgramBuffStore

  .class auto ansi nested public beforefieldinit '<>c__DisplayClass2'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book b
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass2'::.ctor

    .method public hidebysig instance bool 
            '<GetBooksOnMyLot>b__0'(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book toTest) cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::get_BookId()
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/'<>c__DisplayClass2'::b
      IL_000c:  callvirt   instance string [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::get_BookId()
      IL_0011:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0016:  ret
    } // end of method '<>c__DisplayClass2'::'<GetBooksOnMyLot>b__0'

  } // end of class '<>c__DisplayClass2'

  .field public static literal uint64 kListeningToAudioProgramBuffStore = uint64(0x9A7F5F1919DF86C1)
  .field public static literal uint64 kHypnotizedBuffStore = uint64(0x9A7F5F1919DF86C2)
  .field public static string sLocalizationKey
  .field public static string sLocalizationKeyAudio
  .field public static string kRoutineMachineRepairKey
  .field public static valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] kSkilledAudioPrograms
  .field public class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS mAudioProgram
  .field public static float32 kSkillGainRate
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 5F 52 61 6E 67 65 3A 20 50 6F 73 69 74 69   // .._Range: Positi
                                                                                            76 65 20 66 6C 6F 61 74 2E 20 44 65 73 63 72 69   // ve float. Descri
                                                                                            70 74 69 6F 6E 3A 20 53 6B 69 6C 6C 20 70 6F 69   // ption: Skill poi
                                                                                            6E 74 73 20 67 61 69 6E 65 64 20 70 65 72 20 68   // nts gained per h
                                                                                            6F 75 72 20 6F 72 20 6C 69 73 74 65 6E 69 6E 67   // our or listening
                                                                                            20 74 6F 20 61 75 64 69 6F 20 70 72 6F 67 72 61   //  to audio progra
                                                                                            6D 2E 00 00 )                                     // m...
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kGamingDuration
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 64 52 61 6E 67 65 3A 20 50 6F 73 69 74 69   // ..dRange: Positi
                                                                                            76 65 20 66 6C 6F 61 74 2E 20 44 65 73 63 72 69   // ve float. Descri
                                                                                            70 74 69 6F 6E 3A 20 44 75 72 61 74 69 6F 6E 20   // ption: Duration 
                                                                                            69 6E 20 6D 69 6E 75 74 65 73 20 27 50 6C 61 79   // in minutes 'Play
                                                                                            20 47 61 6D 65 20 6F 6E 20 54 61 62 6C 65 74 27   //  Game on Tablet'
                                                                                            20 69 6E 74 65 72 61 63 74 69 6F 6E 20 77 69 6C   //  interaction wil
                                                                                            6C 20 6C 61 73 74 2E 00 00 )                      // l last...
  .field public static float32 kCompWhizMultiplier
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 7B 52 61 6E 67 65 3A 20 41 6C 6C 20 46 6C   // ..{Range: All Fl
                                                                                            6F 61 74 73 2E 20 44 65 73 63 72 69 70 74 69 6F   // oats. Descriptio
                                                                                            6E 3A 20 41 75 74 6F 6E 6F 6D 6F 75 73 20 4D 75   // n: Autonomous Mu
                                                                                            6C 74 69 70 6C 69 65 72 20 75 73 65 64 20 66 6F   // ltiplier used fo
                                                                                            72 20 61 6C 6C 20 54 61 62 6C 65 74 20 69 6E 74   // r all Tablet int
                                                                                            65 72 61 63 74 69 6F 6E 73 20 66 6F 72 20 53 69   // eractions for Si
                                                                                            6D 73 20 77 69 74 68 20 27 43 6F 6D 70 75 74 65   // ms with 'Compute
                                                                                            72 20 57 68 69 7A 27 20 74 72 61 69 74 2E 00 00 ) // r Whiz' trait...
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kTechnophobeMultiplier
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 79 52 61 6E 67 65 3A 20 41 6C 6C 20 46 6C   // ..yRange: All Fl
                                                                                            6F 61 74 73 2E 20 44 65 73 63 72 69 70 74 69 6F   // oats. Descriptio
                                                                                            6E 3A 20 41 75 74 6F 6E 6F 6D 6F 75 73 20 4D 75   // n: Autonomous Mu
                                                                                            6C 74 69 70 6C 69 65 72 20 75 73 65 64 20 66 6F   // ltiplier used fo
                                                                                            72 20 61 6C 6C 20 54 61 62 6C 65 74 20 69 6E 74   // r all Tablet int
                                                                                            65 72 61 63 74 69 6F 6E 73 20 66 6F 72 20 53 69   // eractions for Si
                                                                                            6D 73 20 77 69 74 68 20 27 54 65 63 68 6E 6F 70   // ms with 'Technop
                                                                                            68 6F 62 65 27 20 74 72 61 69 74 2E 00 00 )       // hobe' trait...
  .field public static float32 kPlayGameFunThreshold
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 72 52 61 6E 67 65 3A 20 41 6C 6C 20 46 6C   // ..rRange: All Fl
                                                                                            6F 61 74 73 2E 20 44 65 73 63 72 69 70 74 69 6F   // oats. Descriptio
                                                                                            6E 3A 20 46 75 6E 20 6D 6F 74 69 76 65 20 54 68   // n: Fun motive Th
                                                                                            72 65 73 68 6F 6C 64 20 62 65 6C 6F 77 20 77 68   // reshold below wh
                                                                                            69 63 68 20 27 50 6C 61 79 20 47 61 6D 65 27 20   // ich 'Play Game' 
                                                                                            62 65 63 6F 6D 65 73 20 61 76 61 69 6C 61 62 6C   // becomes availabl
                                                                                            65 20 74 6F 20 61 75 74 6F 6E 6F 6D 6F 75 73 20   // e to autonomous 
                                                                                            53 69 6D 73 2E 00 00 )                            // Sims...
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kHomeworkRateMultiplier
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 5D 52 61 6E 67 65 3A 20 50 6F 73 69 74 69   // ..]Range: Positi
                                                                                            76 65 20 66 6C 6F 61 74 2E 20 44 65 73 63 72 69   // ve float. Descri
                                                                                            70 74 69 6F 6E 3A 20 4D 75 6C 74 69 70 6C 69 65   // ption: Multiplie
                                                                                            72 20 61 70 70 6C 69 65 64 20 74 6F 20 6E 6F 72   // r applied to nor
                                                                                            6D 61 6C 20 72 61 74 65 20 6F 66 20 68 6F 6D 65   // mal rate of home
                                                                                            77 6F 72 6B 20 63 6F 6D 70 6C 65 74 69 6F 6E 2E   // work completion.
                                                                                            00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kHomeworkSkillPPM
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 5A 52 61 6E 67 65 3A 20 50 6F 73 69 74 69   // ..ZRange: Positi
                                                                                            76 65 20 66 6C 6F 61 74 2E 20 44 65 73 63 72 69   // ve float. Descri
                                                                                            70 74 69 6F 6E 3A 20 54 68 65 20 6E 75 6D 62 65   // ption: The numbe
                                                                                            72 20 6F 66 20 68 6F 6D 65 77 6F 72 6B 20 73 6B   // r of homework sk
                                                                                            69 6C 6C 20 70 6F 69 6E 74 73 20 67 61 69 6E 65   // ill points gaine
                                                                                            64 20 70 65 72 20 6D 69 6E 75 74 65 2E 00 00 )    // d per minute...
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public static float32 kBookRepeatFunRate
  .custom instance void [SimIFace]Sims3.SimIFace.TunableAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [SimIFace]Sims3.SimIFace.TunableCommentAttribute::.ctor(string) = ( 01 00 6D 52 61 6E 67 65 3A 20 50 6F 73 69 74 76   // ..mRange: Positv
                                                                                            65 20 66 6C 6F 61 74 2E 20 44 65 73 63 72 69 70   // e float. Descrip
                                                                                            74 69 6F 6E 3A 20 4D 75 6C 74 69 70 6C 69 65 72   // tion: Multiplier
                                                                                            20 66 6F 72 20 68 6F 77 20 6D 75 63 68 20 66 75   //  for how much fu
                                                                                            6E 20 61 20 73 69 6D 20 68 61 73 20 72 65 2D 72   // n a sim has re-r
                                                                                            65 61 64 69 6E 67 20 61 20 62 6F 6F 6B 2E 20 20   // eading a book.  
                                                                                            47 50 45 20 44 65 66 61 75 6C 74 3A 20 30 2E 35   // GPE Default: 0.5
                                                                                            00 00 ) 
  .method public hidebysig static string 
          LocalizeString(string name,
                         object[] parameters) cil managed
  {
    .param [2]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  ldsfld     string Sims3.Store.Objects.Tablet::sLocalizationKey
    IL_0005:  ldstr      ":"
    IL_000a:  ldarg.0
    IL_000b:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0010:  ldarg.1
    IL_0011:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                            object[])
    IL_0016:  ret
  } // end of method Tablet::LocalizeString

  .method public hidebysig static string 
          LocalizeAudioProgram(string name,
                               object[] parameters) cil managed
  {
    .param [2]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  ldsfld     string Sims3.Store.Objects.Tablet::sLocalizationKeyAudio
    IL_0005:  ldstr      ":"
    IL_000a:  ldarg.0
    IL_000b:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0010:  ldarg.1
    IL_0011:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                            object[])
    IL_0016:  ret
  } // end of method Tablet::LocalizeAudioProgram

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::.ctor()
    IL_0006:  ret
  } // end of method Tablet::.ctor

  .method public hidebysig virtual instance void 
          OnCreation() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::OnCreation()
    IL_0006:  ret
  } // end of method Tablet::OnCreation

  .method public hidebysig virtual instance void 
          OnStartup() cil managed
  {
    // Code size       229 (0xe5)
    .maxstack  4
    .locals init (object[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  newarr     [mscorlib]System.Object
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Type> [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.ItemComponent::SimInventoryItem
    IL_000f:  stelem.ref
    IL_0010:  ldloc.0
    IL_0011:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddComponent<class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.ItemComponent>(object[])
    IL_0016:  pop
    IL_0017:  ldarg.0
    IL_0018:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.ItemComponent [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ItemComp()
    IL_001d:  ldarg.0
    IL_001e:  ldftn      instance bool Sims3.Store.Objects.Tablet::CanAddToInventory(class [Sims3GameplaySystems]Sims3.Gameplay.Inventory)
    IL_0024:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.ItemComponent/CanAddToInventoryCallback::.ctor(object,
                                                                                                                                            native int)
    IL_0029:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.ItemComponent/CanAddToInventoryCallback [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.ItemComponent::CanAddToInventoryDelegate
    IL_002e:  ldarg.0
    IL_002f:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::OnStartup()
    IL_0034:  ldarg.0
    IL_0035:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ChooseBookOnTablet::Singleton
    IL_003a:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_003f:  ldarg.0
    IL_0040:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PickUpTabletForShopping::Singleton
    IL_0045:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_004a:  ldarg.0
    IL_004b:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::Singleton
    IL_0050:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0055:  ldarg.0
    IL_0056:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PickUpTabletForAudio::Singleton
    IL_005b:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0060:  ldarg.0
    IL_0061:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ListenToAudioProgram::Singleton
    IL_0066:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_006b:  ldarg.0
    IL_006c:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PickUpTabletForGaming::Singleton
    IL_0071:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0076:  ldarg.0
    IL_0077:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PlayGameOnTablet::Singleton
    IL_007c:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0081:  ldarg.0
    IL_0082:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PickUpTabletForHomework::Singleton
    IL_0087:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_008c:  ldarg.0
    IL_008d:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::Singleton
    IL_0092:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_0097:  ldarg.0
    IL_0098:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/StopAudioProgramFromTablet::Singleton
    IL_009d:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00a2:  ldarg.0
    IL_00a3:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ChooseBookOnTablet::Singleton
    IL_00a8:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInventoryInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00ad:  ldarg.0
    IL_00ae:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ShopForBooksOnTablet::Singleton
    IL_00b3:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInventoryInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00b8:  ldarg.0
    IL_00b9:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/ListenToAudioProgram::Singleton
    IL_00be:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInventoryInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00c3:  ldarg.0
    IL_00c4:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PlayGameOnTablet::Singleton
    IL_00c9:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInventoryInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00ce:  ldarg.0
    IL_00cf:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/DoHomeworkOnTablet::Singleton
    IL_00d4:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInventoryInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00d9:  ldarg.0
    IL_00da:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/StopAudioProgramFromTablet::Singleton
    IL_00df:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::AddInventoryInteraction(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition)
    IL_00e4:  ret
  } // end of method Tablet::OnStartup

  .method public hidebysig virtual instance void 
          DoReset(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/ResetInformation resetInformation) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_0006:  brfalse.s  IL_0014

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_000e:  callvirt   instance bool Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::RemoveMe()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldarg.1
    IL_0016:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::DoReset(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject/ResetInformation)
    IL_001b:  ret
  } // end of method Tablet::DoReset

  .method public hidebysig virtual instance void 
          Dispose() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_0006:  brfalse.s  IL_0014

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_000e:  callvirt   instance bool Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::RemoveMe()
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::Dispose()
    IL_001a:  ret
  } // end of method Tablet::Dispose

  .method public hidebysig specialname virtual 
          instance string  get_CatalogName() cil managed
  {
    // Code size       134 (0x86)
    .maxstack  5
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_0,
             string V_1,
             class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill V_2,
             string V_3,
             object[] V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_0006:  brfalse.s  IL_007f

    IL_0008:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_ActiveActor()
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  brfalse.s  IL_007f

    IL_0011:  ldarg.0
    IL_0012:  call       instance string [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_CatalogName()
    IL_0017:  stloc.1
    IL_0018:  ldloc.0
    IL_0019:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
    IL_001e:  ldarg.0
    IL_001f:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_0024:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
    IL_0029:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
    IL_002e:  stloc.2
    IL_002f:  ldarg.0
    IL_0030:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_0035:  ldfld      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::mSkill
    IL_003a:  ldloc.0
    IL_003b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
    IL_0040:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::GetLocalizedSkillName(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames,
                                                                                                         class [UI]Sims3.UI.CAS.ISimDescription)
    IL_0045:  stloc.3
    IL_0046:  ldstr      "CatalogNameWhenGainingSkill"
    IL_004b:  ldc.i4.4
    IL_004c:  newarr     [mscorlib]System.Object
    IL_0051:  stloc.s    V_4
    IL_0053:  ldloc.s    V_4
    IL_0055:  ldc.i4.0
    IL_0056:  ldloc.0
    IL_0057:  stelem.ref
    IL_0058:  ldloc.s    V_4
    IL_005a:  ldc.i4.1
    IL_005b:  ldloc.1
    IL_005c:  stelem.ref
    IL_005d:  ldloc.s    V_4
    IL_005f:  ldc.i4.2
    IL_0060:  ldloc.3
    IL_0061:  stelem.ref
    IL_0062:  ldloc.s    V_4
    IL_0064:  ldc.i4.3
    IL_0065:  ldloc.2
    IL_0066:  callvirt   instance float32 [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::get_Progress()
    IL_006b:  ldc.r4     100.
    IL_0070:  mul
    IL_0071:  box        [mscorlib]System.Single
    IL_0076:  stelem.ref
    IL_0077:  ldloc.s    V_4
    IL_0079:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                           object[])
    IL_007e:  ret

    IL_007f:  ldarg.0
    IL_0080:  call       instance string [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_CatalogName()
    IL_0085:  ret
  } // end of method Tablet::get_CatalogName

  .method public hidebysig specialname virtual 
          instance bool  get_InUse() cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_0006:  brfalse.s  IL_000a

    IL_0008:  ldc.i4.1
    IL_0009:  ret

    IL_000a:  ldarg.0
    IL_000b:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_InUse()
    IL_0010:  ret
  } // end of method Tablet::get_InUse

  .method public hidebysig newslot specialname virtual final 
          instance float32  get_CarryRouteToObjectRadius() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldc.r4     0.44999999
    IL_0005:  ret
  } // end of method Tablet::get_CarryRouteToObjectRadius

  .method public hidebysig newslot specialname virtual final 
          instance string  get_CarryModelName() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      "book"
    IL_0005:  ret
  } // end of method Tablet::get_CarryModelName

  .method public hidebysig newslot virtual final 
          instance bool  OnSuctioned(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                     bool isOutOfControl) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ret
  } // end of method Tablet::OnSuctioned

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> 
          GetBooksOnMyLot(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                          bool justFirst,
                          bool noDuplicates) cil managed
  {
    // Code size       133 (0x85)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> V_0,
             class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot V_1,
             class [mscorlib]System.Predicate`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> V_2,
             class Sims3.Store.Objects.Tablet/'<>c__DisplayClass2' V_3,
             class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book> V_4,
             class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book[] V_5,
             int32 V_6)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldarg.0
    IL_0007:  brtrue.s   IL_000b

    IL_0009:  ldloc.0
    IL_000a:  ret

    IL_000b:  ldarg.0
    IL_000c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LotHome()
    IL_0011:  stloc.1
    IL_0012:  ldnull
    IL_0013:  stloc.2
    IL_0014:  newobj     instance void Sims3.Store.Objects.Tablet/'<>c__DisplayClass2'::.ctor()
    IL_0019:  stloc.3
    IL_001a:  call       !!0[] [Sims3GameplaySystems]Sims3.Gameplay.Queries::GetObjects<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>()
    IL_001f:  stloc.s    V_5
    IL_0021:  ldc.i4.0
    IL_0022:  stloc.s    V_6
    IL_0024:  br.s       IL_0078

    IL_0026:  ldloc.3
    IL_0027:  ldloc.s    V_5
    IL_0029:  ldloc.s    V_6
    IL_002b:  ldelem.ref
    IL_002c:  stfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/'<>c__DisplayClass2'::b
    IL_0031:  ldloc.3
    IL_0032:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/'<>c__DisplayClass2'::b
    IL_0037:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::GetOwnerLot()
    IL_003c:  ldloc.1
    IL_003d:  bne.un.s   IL_0072

    IL_003f:  ldarg.1
    IL_0040:  brtrue.s   IL_005e

    IL_0042:  ldarg.2
    IL_0043:  brfalse.s  IL_005e

    IL_0045:  ldloc.0
    IL_0046:  ldloc.2
    IL_0047:  brtrue.s   IL_0056

    IL_0049:  ldloc.3
    IL_004a:  ldftn      instance bool Sims3.Store.Objects.Tablet/'<>c__DisplayClass2'::'<GetBooksOnMyLot>b__0'(class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book)
    IL_0050:  newobj     instance void class [mscorlib]System.Predicate`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::.ctor(object,
                                                                                                                                          native int)
    IL_0055:  stloc.2
    IL_0056:  ldloc.2
    IL_0057:  callvirt   instance bool class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Exists(class [mscorlib]System.Predicate`1<!0>)
    IL_005c:  brtrue.s   IL_0072

    IL_005e:  ldloc.0
    IL_005f:  ldloc.3
    IL_0060:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book Sims3.Store.Objects.Tablet/'<>c__DisplayClass2'::b
    IL_0065:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book>::Add(!0)
    IL_006a:  ldarg.1
    IL_006b:  brfalse.s  IL_0072

    IL_006d:  ldloc.0
    IL_006e:  stloc.s    V_4
    IL_0070:  leave.s    IL_0082

    IL_0072:  ldloc.s    V_6
    IL_0074:  ldc.i4.1
    IL_0075:  add
    IL_0076:  stloc.s    V_6
    IL_0078:  ldloc.s    V_6
    IL_007a:  ldloc.s    V_5
    IL_007c:  ldlen
    IL_007d:  conv.i4
    IL_007e:  blt.s      IL_0026

    IL_0080:  ldloc.0
    IL_0081:  ret

    IL_0082:  ldloc.s    V_4
    IL_0084:  ret
  } // end of method Tablet::GetBooksOnMyLot

  .method public hidebysig instance void 
          StartUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                           bool bFromInventory,
                           class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance ii,
                           class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate del,
                           class [SimIFace]Sims3.SimIFace.StateMachineClient& smcRef) cil managed
  {
    // Code size       258 (0x102)
    .maxstack  7
    .locals init (uint64[] V_0,
             int32[] V_1,
             uint64[] V_2,
             int32[] V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_0006:  brfalse.s  IL_0014

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS Sims3.Store.Objects.Tablet::mAudioProgram
    IL_000e:  callvirt   instance bool Sims3.Store.Objects.Tablet/ListeningToAudioProgramBuffStore/BuffInstanceLTAPBS::RemoveMe()
    IL_0013:  pop
    IL_0014:  ldarg.1
    IL_0015:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
    IL_001a:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim/StandingPosture
    IL_001f:  brfalse.s  IL_009f

    IL_0021:  ldarg.2
    IL_0022:  brtrue.s   IL_0048

    IL_0024:  ldarg.1
    IL_0025:  ldsfld     float32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::kReadStandingWanderDistanceMin
    IL_002a:  ldsfld     float32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::kReadStandingWanderDistanceMax
    IL_002f:  ldc.i4.3
    IL_0030:  ldc.i4.1
    IL_0031:  newarr     [mscorlib]System.Int32
    IL_0036:  stloc.1
    IL_0037:  ldloc.1
    IL_0038:  ldc.i4.0
    IL_0039:  ldarg.1
    IL_003a:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_RoomId()
    IL_003f:  stelem.i4
    IL_0040:  ldloc.1
    IL_0041:  ldc.i4.0
    IL_0042:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::Wander(float32,
                                                                                               float32,
                                                                                               valuetype [SimIFace]Sims3.SimIFace.RouteDistancePreference,
                                                                                               int32[],
                                                                                               bool)
    IL_0047:  pop
    IL_0048:  ldc.i4.1
    IL_0049:  newarr     [mscorlib]System.UInt64
    IL_004e:  stloc.2
    IL_004f:  ldloc.2
    IL_0050:  ldc.i4.0
    IL_0051:  ldarg.1
    IL_0052:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
    IL_0057:  ldfld      uint64 [SimIFace]Sims3.SimIFace.ObjectGuid::mValue
    IL_005c:  stelem.i8
    IL_005d:  ldloc.2
    IL_005e:  stloc.0
    IL_005f:  ldarg.3
    IL_0060:  ldarg.1
    IL_0061:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CurrentInteraction()
    IL_0066:  bne.un.s   IL_009f

    IL_0068:  ldarg.1
    IL_0069:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ObjectGuid [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::get_ObjectId()
    IL_006e:  ldloc.0
    IL_006f:  ldsfld     float32 [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::kPortalIntersectRadius
    IL_0074:  call       valuetype [SimIFace]Sims3.SimIFace.RouteIsOkToIdleFlags [SimIFace]Sims3.SimIFace.Route::IsOkToIdleAtCurrentLocation(valuetype [SimIFace]Sims3.SimIFace.ObjectGuid,
                                                                                                                                             uint64[],
                                                                                                                                             float32)
    IL_0079:  brfalse.s  IL_009f

    IL_007b:  ldarg.1
    IL_007c:  ldsfld     float32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::kReadStandingWanderDistanceMin
    IL_0081:  ldsfld     float32 [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::kReadStandingWanderDistanceMax
    IL_0086:  ldc.i4.3
    IL_0087:  ldc.i4.1
    IL_0088:  newarr     [mscorlib]System.Int32
    IL_008d:  stloc.3
    IL_008e:  ldloc.3
    IL_008f:  ldc.i4.0
    IL_0090:  ldarg.1
    IL_0091:  callvirt   instance int32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_RoomId()
    IL_0096:  stelem.i4
    IL_0097:  ldloc.3
    IL_0098:  ldc.i4.0
    IL_0099:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::Wander(float32,
                                                                                               float32,
                                                                                               valuetype [SimIFace]Sims3.SimIFace.RouteDistancePreference,
                                                                                               int32[],
                                                                                               bool)
    IL_009e:  pop
    IL_009f:  ldarg.s    smcRef
    IL_00a1:  ldind.ref
    IL_00a2:  brtrue.s   IL_00b5

    IL_00a4:  ldarg.s    smcRef
    IL_00a6:  ldarg.1
    IL_00a7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
    IL_00ac:  ldc.i4.1
    IL_00ad:  ldarg.s    del
    IL_00af:  callvirt   instance class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::GetPostureStateMachine(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.PostureInteractions,
                                                                                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture/GetStateMachineDelegate)
    IL_00b4:  stind.ref
    IL_00b5:  ldarg.1
    IL_00b6:  ldc.i8     0x4c800171bd469d0
    IL_00bf:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
    IL_00c4:  brfalse.s  IL_0101

    IL_00c6:  ldarg.1
    IL_00c7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
    IL_00cc:  ldc.i8     0x4c80018653f16e0
    IL_00d5:  ldc.i8     0xfd3d00247892e320
    IL_00de:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::AddElementPaused(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,
                                                                                                                       valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Origin)
    IL_00e3:  pop
    IL_00e4:  ldarg.3
    IL_00e5:  ldc.i4     0x10f38da
    IL_00ea:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::HasCommodityUpdate(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind)
    IL_00ef:  brfalse.s  IL_0101

    IL_00f1:  ldarg.3
    IL_00f2:  ldc.i4     0x10f38da
    IL_00f7:  ldc.r4     0.0
    IL_00fc:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::BeginCommodityUpdate(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                                                   float32)
    IL_0101:  ret
  } // end of method Tablet::StartUsingTablet

  .method public hidebysig instance bool 
          StopUsingTablet(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                          class [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book bookBeingRead,
                          class [SimIFace]Sims3.SimIFace.StateMachineClient smc,
                          bool bPutInInventory) cil managed
  {
    // Code size       284 (0x11c)
    .maxstack  6
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.Opportunity V_0,
             class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_1,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.Opportunity> V_2)
    IL_0000:  ldarg.3
    IL_0001:  ldstr      "gainSkill"
    IL_0006:  ldc.i4.0
    IL_0007:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                 bool)
    IL_000c:  ldarg.2
    IL_000d:  brfalse.s  IL_0073

    IL_000f:  ldarg.2
    IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Objects.Book::get_IsOneShotDelete()
    IL_0015:  brfalse.s  IL_0073

    IL_0017:  ldarg.1
    IL_0018:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
    IL_001d:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_TeenOrAbove()
    IL_0022:  brfalse.s  IL_0030

    IL_0024:  ldarg.3
    IL_0025:  ldstr      "gainSkill"
    IL_002a:  ldc.i4.0
    IL_002b:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                 bool)
    IL_0030:  ldarg.1
    IL_0031:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.OpportunityManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_OpportunityManager()
    IL_0036:  brfalse.s  IL_006d

    IL_0038:  ldarg.1
    IL_0039:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.OpportunityManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_OpportunityManager()
    IL_003e:  callvirt   instance class [mscorlib]System.Collections.Generic.ICollection`1<!2> class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.OpportunityNames,class [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.Opportunity,class [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.Opportunity>::get_List()
    IL_0043:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.Opportunity>::GetEnumerator()
    IL_0048:  stloc.2
    .try
    {
      IL_0049:  br.s       IL_0059

      IL_004b:  ldloc.2
      IL_004c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.Opportunity>::get_Current()
      IL_0051:  stloc.0
      IL_0052:  ldloc.0
      IL_0053:  ldarg.0
      IL_0054:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Opportunities.Opportunity::DetachObject(class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject)
      IL_0059:  ldloc.2
      IL_005a:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_005f:  brtrue.s   IL_004b

      IL_0061:  leave.s    IL_006d

    }  // end .try
    finally
    {
      IL_0063:  ldloc.2
      IL_0064:  brfalse.s  IL_006c

      IL_0066:  ldloc.2
      IL_0067:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_006c:  endfinally
    }  // end handler
    IL_006d:  ldarg.2
    IL_006e:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ScriptObject::Destroy()
    IL_0073:  ldarg.3
    IL_0074:  ldstr      "inInventory"
    IL_0079:  ldarg.s    bPutInInventory
    IL_007b:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::SetParameter(string,
                                                                                                 bool)
    IL_0080:  ldarg.s    bPutInInventory
    IL_0082:  brfalse.s  IL_00b4

    IL_0084:  ldarg.3
    IL_0085:  ldc.i4.s   100
    IL_0087:  ldarg.0
    IL_0088:  ldftn      instance void Sims3.Store.Objects.Tablet::PutTabletInInventoryHandler(class [SimIFace]Sims3.SimIFace.StateMachineClient,
                                                                                               class [SimIFace]Sims3.SimIFace.IEvent)
    IL_008e:  newobj     instance void [SimIFace]Sims3.SimIFace.SacsEventHandler::.ctor(object,
                                                                                        native int)
    IL_0093:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::AddOneShotScriptEventHandler(uint32,
                                                                                                                 class [SimIFace]Sims3.SimIFace.SacsEventHandler)
    IL_0098:  ldarg.1
    IL_0099:  ldnull
    IL_009a:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::set_CarryStateMachine(class [SimIFace]Sims3.SimIFace.StateMachineClient)
    IL_009f:  ldarg.0
    IL_00a0:  call       instance class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_ActorsUsingMe()
    IL_00a5:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim>::get_Count()
    IL_00aa:  ldc.i4.0
    IL_00ab:  ble.s      IL_00b4

    IL_00ad:  ldarg.0
    IL_00ae:  ldarg.1
    IL_00af:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::RemoveFromUseList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
    IL_00b4:  ldarg.3
    IL_00b5:  ldstr      "x"
    IL_00ba:  ldstr      "Exit"
    IL_00bf:  callvirt   instance void [SimIFace]Sims3.SimIFace.StateMachineClient::RequestState(string,
                                                                                                 string)
    IL_00c4:  ldarg.s    bPutInInventory
    IL_00c6:  brtrue.s   IL_00eb

    IL_00c8:  ldsfld     class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition Sims3.Store.Objects.Tablet/PutTabletDown::Singleton
    IL_00cd:  ldarg.0
    IL_00ce:  ldarg.1
    IL_00cf:  ldarg.1
    IL_00d0:  callvirt   instance valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::InheritedPriority()
    IL_00d5:  ldc.i4.0
    IL_00d6:  ldc.i4.1
    IL_00d7:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionDefinition::CreateInstance(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject,
                                                                                                                                                                                                      class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IActor,
                                                                                                                                                                                                      valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionPriority,
                                                                                                                                                                                                      bool,
                                                                                                                                                                                                      bool)
    IL_00dc:  stloc.1
    IL_00dd:  ldarg.1
    IL_00de:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
    IL_00e3:  ldloc.1
    IL_00e4:  ldc.i4.1
    IL_00e5:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::PushAsContinuation(class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance,
                                                                                                                              bool)
    IL_00ea:  pop
    IL_00eb:  ldarg.1
    IL_00ec:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
    IL_00f1:  ldc.i8     0x4c80018653f16e0
    IL_00fa:  callvirt   instance void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager::UnpauseBuff(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames)
    IL_00ff:  ldarg.1
    IL_0100:  ldc.i8     0x4c800171bd469d0
    IL_0109:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
    IL_010e:  brfalse.s  IL_011a

    IL_0110:  ldarg.1
    IL_0111:  ldc.i4.0
    IL_0112:  ldarg.0
    IL_0113:  ldc.i4.4
    IL_0114:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::PlayReaction(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionTypes,
                                                                                                     class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject,
                                                                                                     valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.ReactionSpeed)
    IL_0119:  pop
    IL_011a:  ldc.i4.1
    IL_011b:  ret
  } // end of method Tablet::StopUsingTablet

  .method public hidebysig instance void 
          PutTabletInInventoryHandler(class [SimIFace]Sims3.SimIFace.StateMachineClient sender,
                                      class [SimIFace]Sims3.SimIFace.IEvent evt) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Parent()
    IL_0006:  brfalse.s  IL_0027

    IL_0008:  ldarg.0
    IL_0009:  call       instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Parent()
    IL_000e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject::get_Inventory()
    IL_0013:  ldarg.0
    IL_0014:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Inventory::TryToAdd(class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
    IL_0019:  pop
    IL_001a:  ldarg.0
    IL_001b:  call       instance bool [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::UnParent()
    IL_0020:  pop
    IL_0021:  ldarg.0
    IL_0022:  call       instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::ResetBindPose()
    IL_0027:  ret
  } // end of method Tablet::PutTabletInInventoryHandler

  .method public hidebysig instance bool 
          ShouldPutInInventory(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                               bool fromInventory) cil managed
  {
    // Code size       148 (0x94)
    .maxstack  3
    .locals init (bool V_0,
             class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_1)
    IL_0000:  ldarg.2
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  brtrue.s   IL_0039

    IL_0005:  ldarg.1
    IL_0006:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_LotCurrent()
    IL_000b:  ldarg.1
    IL_000c:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Core.Lot [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_LotHome()
    IL_0011:  bne.un.s   IL_0039

    IL_0013:  ldarg.1
    IL_0014:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
    IL_0019:  ldc.i4     0x40f3d24
    IL_001e:  ldarg.1
    IL_001f:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture::Satisfies(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Autonomy.CommodityKind,
                                                                                                            class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject)
    IL_0024:  brfalse.s  IL_002a

    IL_0026:  ldc.i4.1
    IL_0027:  stloc.0
    IL_0028:  br.s       IL_0039

    IL_002a:  ldarg.1
    IL_002b:  ldc.i4     0x800
    IL_0030:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasExitReason(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Actors.ExitReason)
    IL_0035:  brtrue.s   IL_0039

    IL_0037:  ldc.i4.1
    IL_0038:  stloc.0
    IL_0039:  ldarg.1
    IL_003a:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
    IL_003f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::GetNextInteraction()
    IL_0044:  stloc.1
    IL_0045:  ldloc.1
    IL_0046:  brfalse.s  IL_0092

    IL_0048:  ldloc.1
    IL_0049:  isinst     Sims3.Store.Objects.Tablet/ChooseBookOnTablet
    IL_004e:  brtrue.s   IL_0090

    IL_0050:  ldloc.1
    IL_0051:  isinst     Sims3.Store.Objects.Tablet/PickUpTabletForShopping
    IL_0056:  brtrue.s   IL_0090

    IL_0058:  ldloc.1
    IL_0059:  isinst     Sims3.Store.Objects.Tablet/ShopForBooksOnTablet
    IL_005e:  brtrue.s   IL_0090

    IL_0060:  ldloc.1
    IL_0061:  isinst     Sims3.Store.Objects.Tablet/PickUpTabletForAudio
    IL_0066:  brtrue.s   IL_0090

    IL_0068:  ldloc.1
    IL_0069:  isinst     Sims3.Store.Objects.Tablet/ListenToAudioProgram
    IL_006e:  brtrue.s   IL_0090

    IL_0070:  ldloc.1
    IL_0071:  isinst     Sims3.Store.Objects.Tablet/PickUpTabletForGaming
    IL_0076:  brtrue.s   IL_0090

    IL_0078:  ldloc.1
    IL_0079:  isinst     Sims3.Store.Objects.Tablet/PlayGameOnTablet
    IL_007e:  brtrue.s   IL_0090

    IL_0080:  ldloc.1
    IL_0081:  isinst     Sims3.Store.Objects.Tablet/PickUpTabletForHomework
    IL_0086:  brtrue.s   IL_0090

    IL_0088:  ldloc.1
    IL_0089:  isinst     Sims3.Store.Objects.Tablet/DoHomeworkOnTablet
    IL_008e:  brfalse.s  IL_0092

    IL_0090:  ldc.i4.1
    IL_0091:  stloc.0
    IL_0092:  ldloc.0
    IL_0093:  ret
  } // end of method Tablet::ShouldPutInInventory

  .method public hidebysig instance void 
          PutTabletAway(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_CarryStateMachine()
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.1
    IL_0009:  ldc.i4.1
    IL_000a:  ldc.i4.1
    IL_000b:  call       void [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.CarrySystem::PutDown(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                     valuetype [Sims3GameplaySystems]Sims3.Gameplay.ObjectComponents.SurfaceType,
                                                                                                     bool)
    IL_0010:  ret
  } // end of method Tablet::PutTabletAway

  .method public hidebysig instance bool 
          CanAddToInventory(class [Sims3GameplaySystems]Sims3.Gameplay.Inventory inventory) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  1
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Inventory::Owner
    IL_0006:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_0011

    IL_000f:  ldc.i4.0
    IL_0010:  ret

    IL_0011:  ldloc.0
    IL_0012:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
    IL_0017:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_ChildOrAbove()
    IL_001c:  ret
  } // end of method Tablet::CanAddToInventory

  .method public hidebysig instance bool 
          ShowShoppingDialog(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                             class [Sims3GameplayObjects]Sims3.Gameplay.Objects.RabbitHoles.Bookstore bookstore) cil managed
  {
    // Code size       245 (0xf5)
    .maxstack  12
    .locals init (float32 V_0)
    IL_0000:  ldc.r4     1.
    IL_0005:  stloc.0
    IL_0006:  ldarg.1
    IL_0007:  ldc.i8     0x24ff001c42b6bacd
    IL_0010:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
    IL_0015:  brfalse.s  IL_002b

    IL_0017:  ldloc.0
    IL_0018:  ldc.r4     1.
    IL_001d:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.TuningValues.TraitTuning::get_BookshopBargainerSalePercentAdd()
    IL_0022:  ldc.r4     100.
    IL_0027:  div
    IL_0028:  sub
    IL_0029:  mul
    IL_002a:  stloc.0
    IL_002b:  ldarg.1
    IL_002c:  ldc.i8     0x24ff001c42b6bad1
    IL_0035:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::HasTrait(valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.TraitNames)
    IL_003a:  brfalse.s  IL_0050

    IL_003c:  ldloc.0
    IL_003d:  ldc.r4     1.
    IL_0042:  call       float32 [Sims3GameplaySystems]Sims3.Gameplay.TuningValues.TraitTuning::get_HagglerSalePercentAdd()
    IL_0047:  ldc.r4     100.
    IL_004c:  div
    IL_004d:  sub
    IL_004e:  mul
    IL_004f:  stloc.0
    IL_0050:  ldarg.1
    IL_0051:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
    IL_0056:  brfalse.s  IL_0092

    IL_0058:  ldarg.1
    IL_0059:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
    IL_005e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.CareerLocation [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::get_CareerLoc()
    IL_0063:  brfalse.s  IL_0092

    IL_0065:  ldarg.1
    IL_0066:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Occupation()
    IL_006b:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Careers.CareerLocation [Sims3GameplaySystems]Sims3.Gameplay.Careers.Occupation::get_CareerLoc()
    IL_0070:  ldfld      class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.RabbitHole [Sims3GameplaySystems]Sims3.Gameplay.Careers.CareerLocation::Owner
    IL_0075:  ldarg.2
    IL_0076:  bne.un.s   IL_0092

    IL_0078:  ldloc.0
    IL_0079:  ldc.r4     1.
    IL_007e:  ldarg.2
    IL_007f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole/Tuning [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole::get_TuningShopping()
    IL_0084:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole/Tuning::EmployeeDiscountPercent
    IL_0089:  ldc.r4     100.
    IL_008e:  div
    IL_008f:  sub
    IL_0090:  mul
    IL_0091:  stloc.0
    IL_0092:  ldloc.0
    IL_0093:  ldc.r4     1.
    IL_0098:  ldarg.2
    IL_0099:  ldfld      float32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole::SalePercentage
    IL_009e:  ldc.r4     100.
    IL_00a3:  div
    IL_00a4:  sub
    IL_00a5:  mul
    IL_00a6:  stloc.0
    IL_00a7:  ldc.r4     1.
    IL_00ac:  ldloc.0
    IL_00ad:  sub
    IL_00ae:  ldc.r4     100.
    IL_00b3:  mul
    IL_00b4:  stloc.0
    IL_00b5:  ldarg.1
    IL_00b6:  ldarg.2
    IL_00b7:  callvirt   instance class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.StoreItem>> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole::get_ItemDictionary()
    IL_00bc:  ldarg.2
    IL_00bd:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole/Tuning [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole::get_TuningShopping()
    IL_00c2:  ldfld      int32 [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole/Tuning::PercentPriceModifier
    IL_00c7:  conv.r4
    IL_00c8:  ldloc.0
    IL_00c9:  conv.i4
    IL_00ca:  conv.r4
    IL_00cb:  ldc.i4.0
    IL_00cc:  ldnull
    IL_00cd:  ldarg.1
    IL_00ce:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Inventory [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.GameObject::get_Inventory()
    IL_00d3:  ldnull
    IL_00d4:  ldarg.0
    IL_00d5:  ldftn      instance void Sims3.Store.Objects.Tablet::FinishedCallBack(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                    bool)
    IL_00db:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole/ShoppingFinished::.ctor(object,
                                                                                                                                 native int)
    IL_00e0:  ldarg.2
    IL_00e1:  dup
    IL_00e2:  ldvirtftn  instance class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ISellableUIItem> [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole::CreateSellableObjectsList(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim)
    IL_00e8:  newobj     instance void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole/CreateSellableCallback::.ctor(object,
                                                                                                                                       native int)
    IL_00ed:  ldc.i4.1
    IL_00ee:  call       void [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole::StartShopping(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim,
                                                                                                               class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [mscorlib]System.Collections.Generic.List`1<class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.StoreItem>>,
                                                                                                               float32,
                                                                                                               float32,
                                                                                                               int32,
                                                                                                               string,
                                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Inventory,
                                                                                                               class [mscorlib]System.Collections.Generic.List`1<string>,
                                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole/ShoppingFinished,
                                                                                                               class [Sims3GameplaySystems]Sims3.Gameplay.Abstracts.ShoppingRabbitHole/CreateSellableCallback,
                                                                                                               bool)
    IL_00f3:  ldc.i4.1
    IL_00f4:  ret
  } // end of method Tablet::ShowShoppingDialog

  .method public hidebysig instance void 
          FinishedCallBack(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim customer,
                           bool boughtItemForFamilyInventory) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_001e

    IL_0003:  ldarg.2
    IL_0004:  brfalse.s  IL_001e

    IL_0006:  ldarg.1
    IL_0007:  ldstr      "PurchaseForFamilyInventory"
    IL_000c:  ldc.i4.0
    IL_000d:  newarr     [mscorlib]System.Object
    IL_0012:  call       string Sims3.Store.Objects.Tablet::LocalizeString(string,
                                                                           object[])
    IL_0017:  ldc.i4.3
    IL_0018:  callvirt   instance class [UI]Sims3.UI.StyledNotification [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::ShowTNSIfSelectable(string,
                                                                                                                                             valuetype [UI]Sims3.UI.StyledNotification/NotificationStyle)
    IL_001d:  pop
    IL_001e:  ret
  } // end of method Tablet::FinishedCallBack

  .method public hidebysig instance bool 
          CanListenToAudioProgram(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor) cil managed
  {
    // Code size       214 (0xd6)
    .maxstack  3
    .locals init (class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance V_0)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsAtWork()
    IL_0006:  brtrue.s   IL_003b

    IL_0008:  ldarg.1
    IL_0009:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsSleeping()
    IL_000e:  brtrue.s   IL_003b

    IL_0010:  ldarg.1
    IL_0011:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_IsSocializing()
    IL_0016:  brtrue.s   IL_003b

    IL_0018:  ldarg.1
    IL_0019:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_BuffManager()
    IL_001e:  ldc.i8     0x9a7f5f1919df86c2
    IL_0027:  callvirt   instance bool class [Sims3GameplaySystems]Sims3.Gameplay.Utilities.GenericManager`3<valuetype [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffNames,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance,class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.BuffInstance>::HasElement(uint64)
    IL_002c:  brtrue.s   IL_003b

    IL_002e:  ldarg.1
    IL_002f:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.Posture [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_Posture()
    IL_0034:  isinst     [Sims3GameplaySystems]Sims3.Gameplay.Pools.SwimmingInPool
    IL_0039:  brfalse.s  IL_003d

    IL_003b:  ldc.i4.0
    IL_003c:  ret

    IL_003d:  ldarg.1
    IL_003e:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_InteractionQueue()
    IL_0043:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance [Sims3GameplaySystems]Sims3.Gameplay.ActorSystems.InteractionQueue::GetCurrentInteraction()
    IL_0048:  stloc.0
    IL_0049:  ldloc.0
    IL_004a:  brfalse    IL_00d4

    IL_004f:  ldloc.0
    IL_0050:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Shower/PutOutSelf
    IL_0055:  brtrue.s   IL_0094

    IL_0057:  ldloc.0
    IL_0058:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Shower/TakeShower
    IL_005d:  brtrue.s   IL_0094

    IL_005f:  ldloc.0
    IL_0060:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Shower/WooHooInShower
    IL_0065:  brtrue.s   IL_0094

    IL_0067:  ldloc.0
    IL_0068:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Bathtub/TakeBath
    IL_006d:  brtrue.s   IL_0094

    IL_006f:  ldloc.0
    IL_0070:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.Plumbing.Bathtub/TakeBubbleBath
    IL_0075:  brtrue.s   IL_0094

    IL_0077:  ldloc.0
    IL_0078:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.Electronics.Computer/ChatRandom
    IL_007d:  brtrue.s   IL_0094

    IL_007f:  ldloc.0
    IL_0080:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.Electronics.Computer/ChatSpecific
    IL_0085:  brtrue.s   IL_0094

    IL_0087:  ldloc.0
    IL_0088:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Interfaces.IGameObject [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::get_Target()
    IL_008d:  isinst     [Sims3GameplayObjects]Sims3.Gameplay.Objects.Electronics.PhoneCell
    IL_0092:  brfalse.s  IL_0096

    IL_0094:  ldc.i4.0
    IL_0095:  ret

    IL_0096:  ldloc.0
    IL_0097:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
    IL_009c:  brfalse.s  IL_00d4

    IL_009e:  ldloc.0
    IL_009f:  ldfld      class [SimIFace]Sims3.SimIFace.StateMachineClient [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstance::mCurrentStateMachine
    IL_00a4:  callvirt   instance string [SimIFace]Sims3.SimIFace.StateMachineClient::get_Name()
    IL_00a9:  ldstr      "routinemachine_store"
    IL_00ae:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00b3:  brfalse.s  IL_00d4

    IL_00b5:  ldloc.0
    IL_00b6:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00bb:  ldsfld     string Sims3.Store.Objects.Tablet::kRoutineMachineRepairKey
    IL_00c0:  ldc.i4.0
    IL_00c1:  newarr     [mscorlib]System.Object
    IL_00c6:  call       string [Sims3GameplaySystems]Sims3.Gameplay.Utilities.Localization::LocalizeString(string,
                                                                                                            object[])
    IL_00cb:  call       bool [mscorlib]System.String::op_Inequality(string,
                                                                     string)
    IL_00d0:  brfalse.s  IL_00d4

    IL_00d2:  ldc.i4.0
    IL_00d3:  ret

    IL_00d4:  ldc.i4.1
    IL_00d5:  ret
  } // end of method Tablet::CanListenToAudioProgram

  .method public hidebysig static void  PopulateAudioPrograms(class [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim actor,
                                                              valuetype [Sims3GameplaySystems]Sims3.Gameplay.Interactions.InteractionInstanceParameters& parameters,
                                                              [out] class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>& listObjs,
                                                              [out] class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>& headers,
                                                              [out] int32& NumSelectableRows) cil managed
  {
    // Code size       354 (0x162)
    .maxstack  5
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/RowInfo> V_0,
             bool V_1,
             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_2,
             class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames> V_3,
             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames V_4,
             class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill V_5,
             class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill V_6,
             class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo> V_7,
             valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey V_8,
             class [UI]Sims3.UI.ObjectPicker/RowInfo V_9,
             class [UI]Sims3.UI.ObjectPicker/TabInfo V_10,
             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_11,
             valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_12,
             int32 V_13)
    IL_0000:  ldarg.s    NumSelectableRows
    IL_0002:  ldc.i4.1
    IL_0003:  stind.i4
    IL_0004:  ldarg.3
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>::.ctor()
    IL_000a:  stind.ref
    IL_000b:  ldarg.2
    IL_000c:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>::.ctor()
    IL_0011:  stind.ref
    IL_0012:  ldarg.3
    IL_0013:  ldind.ref
    IL_0014:  ldsfld     string Sims3.Store.Objects.Tablet::sLocalizationKeyAudio
    IL_0019:  ldstr      ":Title"
    IL_001e:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0023:  ldstr      "Ui/Tooltip/ObjectPicker:Name"
    IL_0028:  ldc.i4     0xfa
    IL_002d:  newobj     instance void [UI]Sims3.UI.ObjectPicker/HeaderInfo::.ctor(string,
                                                                                   string,
                                                                                   int32)
    IL_0032:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/HeaderInfo>::Add(!0)
    IL_0037:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/RowInfo>::.ctor()
    IL_003c:  stloc.0
    IL_003d:  ldc.i4.0
    IL_003e:  stloc.1
    IL_003f:  ldarg.0
    IL_0040:  brfalse.s  IL_0051

    IL_0042:  ldarg.0
    IL_0043:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SimDescription()
    IL_0048:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.CAS.SimDescription::get_Child()
    IL_004d:  brfalse.s  IL_0051

    IL_004f:  ldc.i4.1
    IL_0050:  stloc.1
    IL_0051:  ldc.i4.6
    IL_0052:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
    IL_0057:  stloc.s    V_11
    IL_0059:  ldloc.s    V_11
    IL_005b:  ldc.i4.0
    IL_005c:  ldc.i4.3
    IL_005d:  conv.i8
    IL_005e:  stelem.i8
    IL_005f:  ldloc.s    V_11
    IL_0061:  ldc.i4.1
    IL_0062:  ldc.i4.5
    IL_0063:  conv.i8
    IL_0064:  stelem.i8
    IL_0065:  ldloc.s    V_11
    IL_0067:  ldc.i4.2
    IL_0068:  ldc.i4.s   9
    IL_006a:  conv.i8
    IL_006b:  stelem.i8
    IL_006c:  ldloc.s    V_11
    IL_006e:  ldc.i4.3
    IL_006f:  ldc.i4.s   10
    IL_0071:  conv.i8
    IL_0072:  stelem.i8
    IL_0073:  ldloc.s    V_11
    IL_0075:  ldc.i4.4
    IL_0076:  ldc.i4.s   11
    IL_0078:  conv.i8
    IL_0079:  stelem.i8
    IL_007a:  ldloc.s    V_11
    IL_007c:  ldc.i4.5
    IL_007d:  ldc.i4.s   34
    IL_007f:  conv.i8
    IL_0080:  stelem.i8
    IL_0081:  ldloc.s    V_11
    IL_0083:  stloc.2
    IL_0084:  ldloc.2
    IL_0085:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_008a:  stloc.3
    IL_008b:  ldsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] Sims3.Store.Objects.Tablet::kSkilledAudioPrograms
    IL_0090:  stloc.s    V_12
    IL_0092:  ldc.i4.0
    IL_0093:  stloc.s    V_13
    IL_0095:  br         IL_0130

    IL_009a:  ldloc.s    V_12
    IL_009c:  ldloc.s    V_13
    IL_009e:  ldelem.i8
    IL_009f:  stloc.s    V_4
    IL_00a1:  ldloc.1
    IL_00a2:  brfalse.s  IL_00ae

    IL_00a4:  ldloc.3
    IL_00a5:  ldloc.s    V_4
    IL_00a7:  callvirt   instance bool class [mscorlib]System.Collections.Generic.List`1<valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames>::Contains(!0)
    IL_00ac:  brtrue.s   IL_012a

    IL_00ae:  ldloc.s    V_4
    IL_00b0:  call       class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetStaticSkill(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
    IL_00b5:  stloc.s    V_5
    IL_00b7:  ldloc.s    V_5
    IL_00b9:  brfalse.s  IL_012a

    IL_00bb:  ldarg.0
    IL_00bc:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager [Sims3GameplaySystems]Sims3.Gameplay.Actors.Sim::get_SkillManager()
    IL_00c1:  ldloc.s    V_4
    IL_00c3:  callvirt   instance class [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillManager::GetElement(valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames)
    IL_00c8:  stloc.s    V_6
    IL_00ca:  ldloc.s    V_6
    IL_00cc:  brfalse.s  IL_00d7

    IL_00ce:  ldloc.s    V_6
    IL_00d0:  callvirt   instance bool [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::ReachedMaxLevel()
    IL_00d5:  brtrue.s   IL_012a

    IL_00d7:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo>::.ctor()
    IL_00dc:  stloc.s    V_7
    IL_00de:  ldloca.s   V_8
    IL_00e0:  ldloc.s    V_5
    IL_00e2:  callvirt   instance valuetype [SimIFace]Sims3.SimIFace.ResourceKey [Sims3GameplaySystems]Sims3.Gameplay.Skills.Skill::get_IconKey()
    IL_00e7:  ldc.i4.1
    IL_00e8:  call       instance void [SimIFace]Sims3.SimIFace.ThumbnailKey::.ctor(valuetype [SimIFace]Sims3.SimIFace.ResourceKey,
                                                                                    valuetype [SimIFace]Sims3.SimIFace.ThumbnailSize)
    IL_00ed:  ldloc.s    V_7
    IL_00ef:  ldloc.s    V_8
    IL_00f1:  ldloc.s    V_4
    IL_00f3:  box        [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
    IL_00f8:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00fd:  ldc.i4.0
    IL_00fe:  newarr     [mscorlib]System.Object
    IL_0103:  call       string Sims3.Store.Objects.Tablet::LocalizeAudioProgram(string,
                                                                                 object[])
    IL_0108:  newobj     instance void [UI]Sims3.UI.ObjectPicker/ThumbAndTextColumn::.ctor(valuetype [SimIFace]Sims3.SimIFace.ThumbnailKey,
                                                                                           string)
    IL_010d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo>::Add(!0)
    IL_0112:  ldloc.s    V_4
    IL_0114:  box        [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
    IL_0119:  ldloc.s    V_7
    IL_011b:  newobj     instance void [UI]Sims3.UI.ObjectPicker/RowInfo::.ctor(object,
                                                                                class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/ColumnInfo>)
    IL_0120:  stloc.s    V_9
    IL_0122:  ldloc.0
    IL_0123:  ldloc.s    V_9
    IL_0125:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/RowInfo>::Add(!0)
    IL_012a:  ldloc.s    V_13
    IL_012c:  ldc.i4.1
    IL_012d:  add
    IL_012e:  stloc.s    V_13
    IL_0130:  ldloc.s    V_13
    IL_0132:  ldloc.s    V_12
    IL_0134:  ldlen
    IL_0135:  conv.i4
    IL_0136:  blt        IL_009a

    IL_013b:  ldstr      "Coupon"
    IL_0140:  ldstr      "TabName"
    IL_0145:  ldc.i4.0
    IL_0146:  newarr     [mscorlib]System.Object
    IL_014b:  call       string Sims3.Store.Objects.Tablet::LocalizeAudioProgram(string,
                                                                                 object[])
    IL_0150:  ldloc.0
    IL_0151:  newobj     instance void [UI]Sims3.UI.ObjectPicker/TabInfo::.ctor(string,
                                                                                string,
                                                                                class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/RowInfo>)
    IL_0156:  stloc.s    V_10
    IL_0158:  ldarg.2
    IL_0159:  ldind.ref
    IL_015a:  ldloc.s    V_10
    IL_015c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [UI]Sims3.UI.ObjectPicker/TabInfo>::Add(!0)
    IL_0161:  ret
  } // end of method Tablet::PopulateAudioPrograms

  .method public hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       218 (0xda)
    .maxstack  3
    .locals init (valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] V_0)
    IL_0000:  ldstr      "Sims3.Store.Objects"
    IL_0005:  ldc.i4.6
    IL_0006:  callvirt   instance string [mscorlib]System.String::Substring(int32)
    IL_000b:  ldc.i4.s   46
    IL_000d:  ldc.i4.s   47
    IL_000f:  callvirt   instance string [mscorlib]System.String::Replace(char,
                                                                          char)
    IL_0014:  ldstr      "/Tablet"
    IL_0019:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_001e:  stsfld     string Sims3.Store.Objects.Tablet::sLocalizationKey
    IL_0023:  ldsfld     string Sims3.Store.Objects.Tablet::sLocalizationKey
    IL_0028:  ldstr      "/AudioProgram"
    IL_002d:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0032:  stsfld     string Sims3.Store.Objects.Tablet::sLocalizationKeyAudio
    IL_0037:  ldstr      "Store/Objects/RoutineMachine:Repair"
    IL_003c:  stsfld     string Sims3.Store.Objects.Tablet::kRoutineMachineRepairKey
    IL_0041:  ldc.i4.s   10
    IL_0043:  newarr     [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames
    IL_0048:  stloc.0
    IL_0049:  ldloc.0
    IL_004a:  ldc.i4.0
    IL_004b:  ldc.i4.3
    IL_004c:  conv.i8
    IL_004d:  stelem.i8
    IL_004e:  ldloc.0
    IL_004f:  ldc.i4.1
    IL_0050:  ldc.i4.5
    IL_0051:  conv.i8
    IL_0052:  stelem.i8
    IL_0053:  ldloc.0
    IL_0054:  ldc.i4.2
    IL_0055:  ldc.i4.s   11
    IL_0057:  conv.i8
    IL_0058:  stelem.i8
    IL_0059:  ldloc.0
    IL_005a:  ldc.i4.3
    IL_005b:  ldc.i4.s   12
    IL_005d:  conv.i8
    IL_005e:  stelem.i8
    IL_005f:  ldloc.0
    IL_0060:  ldc.i4.4
    IL_0061:  ldc.i4.8
    IL_0062:  conv.i8
    IL_0063:  stelem.i8
    IL_0064:  ldloc.0
    IL_0065:  ldc.i4.5
    IL_0066:  ldc.i4.s   9
    IL_0068:  conv.i8
    IL_0069:  stelem.i8
    IL_006a:  ldloc.0
    IL_006b:  ldc.i4.6
    IL_006c:  ldc.i4.s   34
    IL_006e:  conv.i8
    IL_006f:  stelem.i8
    IL_0070:  ldloc.0
    IL_0071:  ldc.i4.7
    IL_0072:  ldc.i4.s   21
    IL_0074:  conv.i8
    IL_0075:  stelem.i8
    IL_0076:  ldloc.0
    IL_0077:  ldc.i4.8
    IL_0078:  ldc.i4.s   30
    IL_007a:  conv.i8
    IL_007b:  stelem.i8
    IL_007c:  ldloc.0
    IL_007d:  ldc.i4.s   9
    IL_007f:  ldc.i4.s   10
    IL_0081:  conv.i8
    IL_0082:  stelem.i8
    IL_0083:  ldloc.0
    IL_0084:  stsfld     valuetype [Sims3GameplaySystems]Sims3.Gameplay.Skills.SkillNames[] Sims3.Store.Objects.Tablet::kSkilledAudioPrograms
    IL_0089:  ldc.r4     1000.
    IL_008e:  stsfld     float32 Sims3.Store.Objects.Tablet::kSkillGainRate
    IL_0093:  ldc.r4     120.
    IL_0098:  stsfld     float32 Sims3.Store.Objects.Tablet::kGamingDuration
    IL_009d:  ldc.r4     2.
    IL_00a2:  stsfld     float32 Sims3.Store.Objects.Tablet::kCompWhizMultiplier
    IL_00a7:  ldc.r4     0.25
    IL_00ac:  stsfld     float32 Sims3.Store.Objects.Tablet::kTechnophobeMultiplier
    IL_00b1:  ldc.r4     75.
    IL_00b6:  stsfld     float32 Sims3.Store.Objects.Tablet::kPlayGameFunThreshold
    IL_00bb:  ldc.r4     2.
    IL_00c0:  stsfld     float32 Sims3.Store.Objects.Tablet::kHomeworkRateMultiplier
    IL_00c5:  ldc.r4     2.
    IL_00ca:  stsfld     float32 Sims3.Store.Objects.Tablet::kHomeworkSkillPPM
    IL_00cf:  ldc.r4     0.5
    IL_00d4:  stsfld     float32 Sims3.Store.Objects.Tablet::kBookRepeatFunRate
    IL_00d9:  ret
  } // end of method Tablet::.cctor

  .property instance string CatalogName()
  {
    .get instance string Sims3.Store.Objects.Tablet::get_CatalogName()
  } // end of property Tablet::CatalogName
  .property instance bool InUse()
  {
    .get instance bool Sims3.Store.Objects.Tablet::get_InUse()
  } // end of property Tablet::InUse
  .property instance float32 CarryRouteToObjectRadius()
  {
    .get instance float32 Sims3.Store.Objects.Tablet::get_CarryRouteToObjectRadius()
  } // end of property Tablet::CarryRouteToObjectRadius
  .property instance string CarryModelName()
  {
    .get instance string Sims3.Store.Objects.Tablet::get_CarryModelName()
  } // end of property Tablet::CarryModelName
} // end of class Sims3.Store.Objects.Tablet


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file D:\Downloads\For Games\Sims3 Downloads\Hacks\Coding\Sims3\Store\0xA0572B47188D6B5A.res
