using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Books
{
    public abstract class BooksRead : SimFromList, IBooksOption
    {
        IEnumerable<Item> mChoices = null;

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        public override void Reset()
        {
            mChoices = null;

 	        base.Reset();
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.ReadBookDataList != null);
        }

        protected abstract List<Item> GetOptions(SimDescription me, Dictionary<BookData,bool> lookup);

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                Dictionary<BookData,bool> lookup = new Dictionary<BookData,bool>();

                foreach(ReadBookData data in me.ReadBookDataList.Values)
                {
                    BookData bookData = BookData.GetBookData(BookData.BookType.General, data.BookID);
                    if (bookData == null)
                    {
                        bookData = BookData.GetBookData(BookData.BookType.Toddler, data.BookID);
                        if (bookData == null)
                        {
                            BookWrittenData writtenData;
                            if (BookData.BookWrittenDataList.TryGetValue(data.BookID, out writtenData))
                            {
                                bookData = writtenData;
                            }

                            if (bookData == null) continue;
                        }
                    }

                    lookup.Add(bookData, true);
                }

                CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, GetOptions(me, lookup)).SelectMultiple();
                if ((selection == null) || (selection.Count == 0)) return false;

                CommonSelection<Item>.HandleAllOrNothing(selection);

                mChoices = selection;
            }

            foreach (Item choice in mChoices)
            {
                Perform(me, choice.mBook, !choice.IsSet);
            }

            return true;
        }

        protected virtual void Perform(SimDescription me, BookData book, bool addToList)
        {
            if (me.ReadBookDataList.ContainsKey(book.ID))
            {
                if (addToList) return;

                me.ReadBookDataList.Remove(book.ID);
            }
            else
            {
                if (!addToList) return;

                ReadBookData readData = new ReadBookData();
                readData.BookID = book.ID;
                readData.TimesRead = 1;

                me.ReadBookDataList.Add(book.ID, readData);
            }
        }

        public class Item : CommonOptionItem
        {
            public readonly BookData mBook;

            public readonly string mKey;

            public Item(BookData book, string key, bool read)
                : this(book, read)
            {
                mKey = key;
            }
            public Item(BookData book, bool read)
                : base(book.Title, read ? 1 : 0)
            {
                mThumbnail = new ThumbnailKey(new ResourceKey((ulong)ResourceUtils.XorFoldHashString32("book_standard"), 0x1661233, 0x1), ThumbnailSize.Medium, ResourceUtils.HashString32(book.GeometryState), ResourceUtils.HashString32(book.MaterialState));

                mBook = book;
            }

            public override string DisplayValue
            {
                get { return null; }
            }

            public override string DisplayKey
            {
                get { return "Knows"; }
            }
        }
    }
}
