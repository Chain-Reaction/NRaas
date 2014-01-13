using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class WrittenWord : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static WrittenWord()
        {
            Bootstrap();
        }

        public static ObjectGuid CreateBookWrittenCallback(object customData, ref Simulator.ObjectInitParameters initData, Quality quality)
        {
            BookWrittenData data = customData as BookWrittenData;
            if (data == null) return ObjectGuid.InvalidObjectGuid;

            try
            {
                Notify(data.Title);

                BookWritten book = GlobalFunctions.CreateObjectOutOfWorld("BookWritten") as BookWritten;
                if (book == null)
                {
                    return ObjectGuid.InvalidObjectGuid;
                }

                book.InitBookCommon(data);

                return book.ObjectId;
            }
            catch(Exception e)
            {
                Common.Exception(data.ID, e);
                return ObjectGuid.InvalidObjectGuid;
            }
        }

        public static void ProcessBookWrittenCallback(object customData, IGameObject book)
        {
            BookWrittenData data = customData as BookWrittenData;
            if (data == null) return;

            try
            {
                BookWritten.ProcessCallback(data, book as BookWritten);
            }
            catch (Exception e)
            {
                Common.Exception(data.ID, e);
            }
        }

        protected static void RemoveWrittenData(string type)
        {
            List<StoreItem> oldData = null;
            if (!Bookstore.mItemDictionary.TryGetValue(type, out oldData))
            {
                return;
            }

            List<StoreItem> newData = new List<StoreItem>();
            foreach (StoreItem item in oldData)
            {
                BookWrittenData data = item.CustomData as BookWrittenData;
                if (data != null) continue;

                newData.Add(item);
            }

            Bookstore.mItemDictionary.Remove(type);
            Bookstore.mItemDictionary.Add(type, newData);
        }

        public void OnWorldLoadFinished()
        {
            Dictionary<BookWrittenData, bool> existing = new Dictionary<BookWrittenData, bool>();

            foreach (BookWritten book in Sims3.Gameplay.Queries.GetObjects<BookWritten>())
            {
                BookWrittenData bookData = book.Data as BookWrittenData;
                if (bookData == null) continue;

                existing[bookData] = true;
            }

            List<string> remove = new List<string>();

            foreach (SimDescription sim in SimListing.GetResidents(true).Values)
            {
                if (sim.SkillManager == null) continue;

                Writing skill = sim.SkillManager.GetSkill<Writing>(SkillNames.Writing);
                if (skill == null) continue;

                if (skill.WrittenBookDataList == null) continue;

                foreach (WrittenBookData book in skill.WrittenBookDataList.Values)
                {
                    book.Author = sim.FullName;

                    string id = book.Title + book.Author;

                    BookWrittenData bookData;
                    if (!BookData.BookWrittenDataList.TryGetValue(id, out bookData))
                    {
                        // Constructor auto-adds to dictionary
                        bookData = new BookWrittenData(book, true);
                    }

                    existing[bookData] = true;
                }
            }

            RemoveWrittenData("General");
            RemoveWrittenData("All");

            List<StoreItem> general = null, all = null;
            Bookstore.mItemDictionary.TryGetValue("General", out general);
            Bookstore.mItemDictionary.TryGetValue("All", out all);

            List<WorldType> worldTypes = new List<WorldType>();
            worldTypes.Add(GameUtils.GetCurrentWorldType());

            Writing stateSkill = new Writing();

            foreach (KeyValuePair<string,BookWrittenData> pair in BookData.BookWrittenDataList)
            {
                BookWrittenData data = pair.Value;

                if (!existing.ContainsKey(pair.Value))
                {
                    remove.Add(pair.Key);
                    continue;
                }
                else if ((string.IsNullOrEmpty(data.Title)) || (string.IsNullOrEmpty(data.Author)))
                {
                    remove.Add(pair.Key);
                    continue;
                }

                string geoState, materialState;
                stateSkill.GetGeoAndMaterialStates(data.Genre, out geoState, out materialState);

                if (string.IsNullOrEmpty(data.GeometryState))
                {
                    data.GeometryState = geoState;
                }

                if (string.IsNullOrEmpty(data.MaterialState))
                {
                    data.MaterialState = materialState;
                }

                ThumbnailKey thumb = new ThumbnailKey(new ResourceKey((ulong)ResourceUtils.XorFoldHashString32("book_standard"), 0x1661233, 0x1), ThumbnailSize.Medium, ResourceUtils.HashString32(data.GeometryState), ResourceUtils.HashString32(data.MaterialState));

                BookGeneralStoreItem item = new BookGeneralStoreItem(data.Title + " - " + data.Author, (float)data.Value, data, thumb, data.GenerateUIStoreItemID(), new CreateObjectCallback(CreateBookWrittenCallback), new ProcessObjectCallback(ProcessBookWrittenCallback), data.AllowedWorlds, worldTypes, data.Author, data.Title, data.Length, data.GenreString);

                general.Add(item);
                all.Add(item);
            }

            foreach (string id in remove)
            {
                BookData.BookWrittenDataList.Remove(id);

                BooterLogger.AddTrace("Removed: " + id);
            }
        }
    }
}
