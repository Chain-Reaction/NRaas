using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Interactions
{
    public class ReadToddlerBookshelfEx : Common.IPreLoad, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Bookshelf, Bookshelf_ReadToToddler.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Bookshelf, Bookshelf_ReadToToddler.Definition, Definition>(false);

            Bookshelf_ReadToToddler.Singleton = Singleton;
        }

        public class Definition : Bookshelf_ReadToToddler.Definition
        {
            public Definition()
            { }
            public Definition(Sim toddler, string menuText, string[] menuPath)
                : base (toddler, menuText, menuPath)
            { }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Bookshelf target, List<InteractionObjectPair> results)
            {
                if (actor.SimDescription.TeenOrAbove && (target.Inventory.AmountIn<BookToddler>() != 0x0))
                {
                    List<Sim> list = new List<Sim>();
                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim.SimDescription.Toddler)
                        {
                            bool found = false;
                            foreach (BookToddler book in Inventories.QuickDuoFind<BookToddler,Book>(target.Inventory))
                            {
                                if (!ReadBookData.HasSimFinishedBook(sim, book.Data.ID))
                                {
                                    found = true;
                                }
                            }

                            if (found)
                            {
                                list.Add(sim);
                            }
                        }
                    }
                    if (list.Count != 0x0)
                    {
                        if (list.Count == 0x1)
                        {
                            results.Add(new InteractionObjectPair(new Definition(list[0x0], Bookshelf_ReadToToddler.LocalizeString("ReadWith", new object[] { list[0x0] }), new string[0x0]), target));
                        }
                        else
                        {
                            foreach (Sim sim2 in list)
                            {
                                results.Add(new InteractionObjectPair(new Definition(sim2, sim2.SimDescription.FirstName, new string[] { Common.LocalizeEAString("Gameplay/Objects/BookToddler:ReadWithMenuText") }), target));
                            }
                        }
                    }
                }
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 0x1;

                headers = null;
                listObjs = null;

                try
                {
                    IGameObject target = parameters.Target;
                    headers = new List<ObjectPicker.HeaderInfo>();
                    listObjs = new List<ObjectPicker.TabInfo>();
                    headers.Add(new ObjectPicker.HeaderInfo("Ui/Caption/ObjectPicker:Title", "Ui/Tooltip/ObjectPicker:Name", 0xfa));
                    List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
                    foreach (BookToddler book in Inventories.QuickDuoFind<BookToddler,Book>(parameters.Target.Inventory))
                    {
                        if (ReadBookData.HasSimFinishedBook(Toddler, book.Data.ID)) continue;

                        List<ObjectPicker.ColumnInfo> columnInfo = new List<ObjectPicker.ColumnInfo>();
                        ThumbnailKey thumbnail = new ThumbnailKey(new ResourceKey((ulong)ResourceUtils.XorFoldHashString32("book_standard"), 0x1661233, 0x1), ThumbnailSize.Medium, ResourceUtils.HashString32(book.Data.GeometryState), ResourceUtils.HashString32(book.Data.MaterialState));
                        columnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, book.Data.Title));
                        ObjectPicker.RowInfo info = new ObjectPicker.RowInfo(book, columnInfo);
                        rowInfo.Add(info);
                    }

                    // Custom code
                    if (rowInfo.Count == 0)
                    {
                        listObjs = null;
                    }
                    else
                    {
                        ObjectPicker.TabInfo item = new ObjectPicker.TabInfo("Coupon", Common.LocalizeEAString("Ui/Caption/ObjectPicker:ToddlerBooks"), rowInfo);
                        listObjs.Add(item);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(parameters.Actor, parameters.Target, e);
                }
            }
        }
    }
}


