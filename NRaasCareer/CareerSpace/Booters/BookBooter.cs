using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Tasks;
using NRaas.CareerSpace.OmniSpace;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.CareerSpace.Booters
{
    public class BookBooter : BooterHelper.ListingBooter, Common.IPreLoad
    {
        static List<XmlDbData> sDelayedSkillBooks = new List<XmlDbData>();

        static bool sObjectGroupCreated = false;

        public BookBooter()
            : this(VersionStamp.sNamespace + ".Books", true)
        { }
        public BookBooter(string reference, bool testDirect)
            : base("BookFile", "Books", reference, testDirect)
        { }

        public void OnPreLoad()
        {
            LoadSaveManager.ObjectGroupCreated += OnObjectGroupCreated;
        }

        public void OnObjectGroupCreated(IScriptObjectGroup group)
        {
            try
            {
                if (sObjectGroupCreated) return;
                sObjectGroupCreated = true;

                if (sDelayedSkillBooks.Count > 0)
                {
                    BooterLogger.AddTrace("OnWorldLoadStarted: " + sDelayedSkillBooks.Count);

                    foreach (XmlDbData bookData in sDelayedSkillBooks)
                    {
                        LoadSkillBookData(bookData, "BookSkill");
                    }

                    sDelayedSkillBooks.Clear();

                    Bookstore.mItemDictionary.Clear();
                    Bookstore.LoadData();
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnWorldLoadStarted", exception);
            }
        }

        protected static bool LoadSkillBookData(XmlDbData spreadsheet, string bookSheet)
        {
            if (spreadsheet.Tables.ContainsKey(bookSheet))
            {
                XmlDbTable table = spreadsheet.Tables[bookSheet];
                int rowIndex = 0x0;
                foreach (XmlDbRow row in table.Rows)
                {
                    try
                    {
                        XmlDbData.XmlDbRowFast data = row as XmlDbData.XmlDbRowFast;
                        if (data != null)
                        {
                            if (row.Exists("AltSkill"))
                            {
                                data.mData.Remove("Skill");
                                data.mData.Add("Skill", row.GetString("AltSkill"));
                            }
                        }

                        BooterLogger.AddTrace("Skill Book Found: " + row.GetString("Skill"));

                        if (SkillManager.GetStaticSkill(SkillManager.sSkillEnumValues.ParseEnumValue(row.GetString("Skill"))) == null)
                        {
                            return false;
                        }

                        BookSkillData book = new BookSkillData(row, rowIndex);

                        string msg = book.ID;

                        msg += Common.NewLine + "  AllowedWorldTypes " + new ListToString<WorldType>().Convert(book.AllowedWorldTypes);

                        msg += Common.NewLine + "  AllowedWorlds " + new ListToString<WorldName>().Convert(book.AllowedWorlds);

                        BooterLogger.AddTrace("Book Loaded: " + msg);
                    }
                    catch(Exception e)
                    {
                        Common.Exception("Title: " + row["Title"], e);
                    }

                    rowIndex++;
                }
            }
            return true;
        }

        protected override void PerformFile(BooterHelper.BootFile file)
        {
            BooterHelper.DataBootFile dataFile = file as BooterHelper.DataBootFile;
            if (dataFile == null) return;

            sDelayedSkillBooks.Add(dataFile.Data);

            BookData.LoadBookData(dataFile.Data, "BookGeneral", BookData.BookType.General);
            BookData.LoadBookData(dataFile.Data, "BookRecipe", BookData.BookType.Recipe);
            BookData.LoadBookData(dataFile.Data, "MedicalJournal", BookData.BookType.MedicalJournal);
            BookData.LoadBookData(dataFile.Data, "SheetMusic", BookData.BookType.SheetMusic);
            BookData.LoadBookData(dataFile.Data, "BookToddler", BookData.BookType.Toddler);
            BookData.LoadBookData(dataFile.Data, "BookAlchemyRecipe", BookData.BookType.AlchemyRecipe);

            LoadWrittenBookTitles(dataFile.Data);

            LoadWrittenBookMaterials(dataFile.Data);

            BookData.LoadBookData(dataFile.Data, "BookFish", BookData.BookType.FishBook);

            XmlDbTable table = dataFile.GetTable("OmniJournal");
            if (table != null)
            {
                BooterLogger.AddTrace(file.ToString() + ": Found OmniJournal = " + table.Rows.Count.ToString());

                int rowIndex = 0;
                foreach (XmlDbRow row in table.Rows)
                {
                    new OmniJournalData(row, rowIndex);
                    rowIndex++;
                }
            }
            else
            {
                BooterLogger.AddTrace(file.ToString() + ": No OmniJournal");
            }

            Bookstore.mItemDictionary.Clear();
            Bookstore.LoadData();
        }

        protected static void LoadWrittenBookTitles(XmlDbData spreadsheet)
        {
            if (spreadsheet.Tables.ContainsKey("WrittenBookTitles"))
            {
                XmlDbTable table = spreadsheet.Tables["WrittenBookTitles"];

                int count = 0;

                foreach (XmlDbRow row in table.Rows)
                {
                    try
                    {
                        string item = row.GetString("Trashy");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Trashy].Add(item);
                        }
                        item = row.GetString("Drama");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Drama].Add(item);
                        }
                        item = row.GetString("SciFi");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.SciFi].Add(item);
                        }
                        item = row.GetString("Fantasy");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Fantasy].Add(item);
                        }
                        item = row.GetString("Humor");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Humor].Add(item);
                        }
                        item = row.GetString("Satire");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Satire].Add(item);
                        }
                        item = row.GetString("Mystery");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Mystery].Add(item);
                        }
                        item = row.GetString("Romance");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Romance].Add(item);
                        }
                        item = row.GetString("Historical");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Historical].Add(item);
                        }
                        item = row.GetString("Childrens");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Childrens].Add(item);
                        }
                        item = row.GetString("Vaudeville");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Vaudeville].Add(item);
                        }
                        item = row.GetString("Biography");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Biography].Add(item);
                        }
                        item = row.GetString("Article");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Article].Add(item);
                        }
                        item = row.GetString("Autobiography");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Autobiography].Add(item);
                        }
                        item = row.GetString("Masterpiece");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Masterpiece].Add(item);
                        }
                        item = row.GetString("PoliticalMemoir");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.PoliticalMemoir].Add(item);
                        }
                        item = row.GetString("LifeStory");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.LifeStory].Add(item);
                        }
                        item = row.GetString("Fiction");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.Fiction].Add(item);
                        }
                        item = row.GetString("NonFiction");
                        if (item != "")
                        {
                            BookData.WrittenBookTitles[BookData.BookGenres.NonFiction].Add(item);
                        }

                        if (GameUtils.IsInstalled(ProductVersion.EP3))
                        {
                            item = row.GetString("ActionScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookTitles[BookData.BookGenres.ActionScreenplay].Add(item);
                            }
                            item = row.GetString("RomanticComedyScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookTitles[BookData.BookGenres.RomanticComedyScreenplay].Add(item);
                            }
                            item = row.GetString("DramaScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookTitles[BookData.BookGenres.DramaScreenplay].Add(item);
                            }
                            item = row.GetString("SciFiScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookTitles[BookData.BookGenres.SciFiScreenplay].Add(item);
                            }
                            item = row.GetString("IndieScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookTitles[BookData.BookGenres.IndieScreenplay].Add(item);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception("Entry: " + count, e);
                    }

                    count++;
                }
            }
        }

        protected static void LoadWrittenBookMaterials(XmlDbData spreadsheet)
        {
            if (spreadsheet.Tables.ContainsKey("WrittenBookMaterials"))
            {
                XmlDbTable table = spreadsheet.Tables["WrittenBookMaterials"];

                int count = 0;

                foreach (XmlDbRow row in table.Rows)
                {
                    try
                    {
                        string item = row.GetString("Trashy");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Trashy].Add(item);
                        }
                        item = row.GetString("Drama");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Drama].Add(item);
                        }
                        item = row.GetString("SciFi");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.SciFi].Add(item);
                        }
                        item = row.GetString("Fantasy");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Fantasy].Add(item);
                        }
                        item = row.GetString("Humor");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Humor].Add(item);
                        }
                        item = row.GetString("Satire");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Satire].Add(item);
                        }
                        item = row.GetString("Mystery");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Mystery].Add(item);
                        }
                        item = row.GetString("Romance");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Romance].Add(item);
                        }
                        item = row.GetString("Historical");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Historical].Add(item);
                        }
                        item = row.GetString("Childrens");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Childrens].Add(item);
                        }
                        item = row.GetString("Vaudeville");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Vaudeville].Add(item);
                        }
                        item = row.GetString("Biography");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Biography].Add(item);
                        }
                        item = row.GetString("Article");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Article].Add(item);
                        }
                        item = row.GetString("Autobiography");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Autobiography].Add(item);
                        }
                        item = row.GetString("Masterpiece");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Masterpiece].Add(item);
                        }
                        item = row.GetString("PoliticalMemoir");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.PoliticalMemoir].Add(item);
                        }
                        item = row.GetString("LifeStory");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.LifeStory].Add(item);
                        }
                        item = row.GetString("Fiction");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.Fiction].Add(item);
                        }
                        item = row.GetString("NonFiction");
                        if (item != "")
                        {
                            BookData.WrittenBookMaterials[BookData.BookGenres.NonFiction].Add(item);
                        }

                        if (GameUtils.IsInstalled(ProductVersion.EP3))
                        {
                            item = row.GetString("ActionScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookMaterials[BookData.BookGenres.ActionScreenplay].Add(item);
                            }
                            item = row.GetString("RomanticComedyScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookMaterials[BookData.BookGenres.RomanticComedyScreenplay].Add(item);
                            }
                            item = row.GetString("DramaScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookMaterials[BookData.BookGenres.DramaScreenplay].Add(item);
                            }
                            item = row.GetString("SciFiScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookMaterials[BookData.BookGenres.SciFiScreenplay].Add(item);
                            }
                            item = row.GetString("IndieScreenplay");
                            if (item != "")
                            {
                                BookData.WrittenBookMaterials[BookData.BookGenres.IndieScreenplay].Add(item);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception("Entry: " + count, e);
                    }

                    count++;
                }
            }
        }
    }
}
