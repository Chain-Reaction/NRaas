using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace NRaas.CommonSpace.Booters
{
    public class BooterHelper : Common.IStartupApp
    {
        public static readonly string sBootStrapFile = "NRaas.BootStrap";

        static List<IBooter> sBooters = null;

        public static void Add(IBooter booter)
        {
            if (sBooters == null)
            {
                sBooters = new List<IBooter>();
            }

            sBooters.Add(booter);
        }

        public static void AddFirst(IBooter booter)
        {
            if (sBooters == null)
            {
                sBooters = new List<IBooter>();
            }

            sBooters.Insert(0, booter);
        }

        public static void AddLast(IBooter booter)
        {
            Add(booter);
        }

        public void OnStartupApp()
        {
            if (sBooters != null)
            {
                foreach (IBooter booter in sBooters)
                {
                    try
                    {
                        BooterLogger.AddTrace("Try: " + booter.ToString());

                        booter.Perform(true);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(booter.ToString(), e);
                    }
                }
            }

            Booter.ClearFiles();
        }

        public interface IBooter
        {
            void Perform(bool testDirect);
        }

        public abstract class Booter : IBooter
        {
            readonly string mReference;

            static Dictionary<string, List<BootFile>> sFiles = new Dictionary<string, List<BootFile>>();

            public Booter()
                : this(sBootStrapFile)
            {}
            public Booter(string reference)
            {
                mReference = reference;
            }

            public static void ClearFiles()
            {
                sFiles.Clear();
            }

            public abstract BootFile GetBootFile(string reference, string name, bool primary);

            public virtual void Perform(bool testDirect)
            {
                try
                {
                    BootFile straightFile = null;
                    if (testDirect)
                    {
                        straightFile = GetBootFile(mReference, mReference, true);
                    }

                    if ((straightFile != null) && (straightFile.IsValid))
                    {
                        Perform(straightFile);
                    }
                    else
                    {
                        List<BootFile> files;
                        if (!sFiles.TryGetValue(mReference, out files))
                        {
                            files = new List<BootFile>();
                            sFiles.Add(mReference, files);

                            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                if (assembly.GetType(mReference) == null) continue;

                                BootFile file = GetBootFile(mReference, assembly.GetName().Name + ".dll", true);
                                if (file.IsValid)
                                {
                                    files.Add(file);
                                }
                            }
                        }

                        foreach (BootFile file in files)
                        {
                            Perform(file);
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mReference, e);
                }
            }

            protected abstract bool Perform(BootFile file);

            public override string ToString()
            {
                string result = GetType().Name;
                result += Common.NewLine + " Reference: " + mReference;

                return result;
            }
        }

        /*
         * Booter from module listing for use when no individual field is available
         */
        public abstract class TableBooter : Booter
        {
            readonly string mListing;

            readonly bool mTestDirect;

            public TableBooter(string listing, string reference, bool testDirect)
                : base(reference)
            {
                mListing = listing;
                mTestDirect = testDirect;
            }

            public override void Perform(bool testDirect)
            {
                base.Perform(mTestDirect);
            }

            protected override bool Perform(BootFile file)
            {
                DataBootTable table = new DataBootTable(file as DataBootFile, mListing);
                if (!table.IsValid) return false;

                table.Load(Perform);
                return true;
            }

            protected abstract void Perform(BootFile file, XmlDbRow row);

            public override string ToString()
            {
                string result =base.ToString();
                result += Common.NewLine + " Listing: " + mListing;
                result += Common.NewLine + " TestDirect: " + mTestDirect;

                return result;
            }
        }

        /*
         * Booter from module listing to separate XML
         */
        public abstract class ListingBooter : Booter
        {
            readonly string mListing;

            readonly string mField;

            readonly bool mTestDirect;

            public ListingBooter(string listing, string field, string reference, bool testDirect)
                : base(reference)
            {
                mListing = listing;
                mField = field;
                mTestDirect = testDirect;
            }

            public override BootFile GetBootFile(string reference, string name, bool primary)
            {
                return new DataBootFile(reference, name, primary);
            }

            public override void Perform(bool testDirect)
            {
                base.Perform(mTestDirect);
            }

            protected sealed override bool Perform(BootFile file)
            {
                DataBootTable table = new DataBootTable(file as DataBootFile, mListing);
                if (table.IsValid)
                {
                    Loader results = new Loader(this, mField);

                    table.Load(results.Load);

                    foreach (BootFile result in results.mFiles)
                    {
                        if (!result.IsValid) continue;

                        Perform(result);
                    }
                }
                else
                {
                    PerformFile(file);
                }

                return true;
            }

            protected abstract void PerformFile(BootFile file);

            public class Loader
            {
                public List<BootFile> mFiles = new List<BootFile>();

                Booter mBooter;

                string mField;

                public Loader(Booter booter, string field)
                {
                    mBooter = booter;
                    mField = field;
                }

                public void Load(BootFile file, XmlDbRow row)
                {
                    mFiles.Add(mBooter.GetBootFile(file.ToString(), row.GetString(mField), false));
                }
            }

            public override string ToString()
            {
                string result = base.ToString();
                result += Common.NewLine + " Listing: " + mListing;
                result += Common.NewLine + " Field: " + mField;
                result += Common.NewLine + " TestDirect: " + mTestDirect;

                return result;
            }
        }

        public abstract class ByRowBooter : Booter
        {
            readonly string mTable;

            public ByRowBooter(string table, string reference)
                : base(reference)
            {
                mTable = table;
            }

            public override BootFile GetBootFile(string reference, string name, bool primary)
            {
                return new DataBootFile(reference, name, primary);
            }

            protected sealed override bool Perform(BootFile file)
            {
                DataBootTable table = new DataBootTable(file as DataBootFile, mTable);
                if (!table.IsValid)
                {
                    if (file.mPrimary)
                    {
                        BooterLogger.AddTrace(file + ": No " + mTable + " Table");
                    }
                    else
                    {
                        BooterLogger.AddError(file + ": No " + mTable + " Table");
                    }
                    return false;
                }

                table.Load(Perform);
                return true;
            }

            protected abstract void Perform(BootFile file, XmlDbRow row);

            public override string ToString()
            {
                string result = base.ToString();
                result += Common.NewLine + " Table: " + mTable;

                return result;
            }
        }

        public abstract class ByRowListingBooter : ListingBooter
        {
            readonly string mTable;

            public ByRowListingBooter(string table, string listing, string field, string reference)
                : this(table, listing, field, reference, false)
            { }
            public ByRowListingBooter(string table, string listing, string field, string reference, bool testDirect)
                : base(listing, field, reference, testDirect)
            {
                mTable = table;
            }

            protected sealed override void PerformFile(BootFile file)
            {
                DataBootTable table = new DataBootTable(file as DataBootFile, mTable);
                if (!table.IsValid)
                {
                    if (file.mPrimary)
                    {
                        BooterLogger.AddTrace(file + ": No " + mTable + " Table");
                    }
                    else
                    {
                        BooterLogger.AddError(file + ": No " + mTable + " Table");
                    }
                    return;
                }

                table.Load(Perform);
            }

            protected abstract void Perform(BootFile file, XmlDbRow row);

            public override string ToString()
            {
                string result = base.ToString();
                result += Common.NewLine + " Table: " + mTable;

                return result;
            }
        }

        public abstract class BootFile
        {
            protected readonly string mName;

            public readonly bool mPrimary;

            public BootFile(string name, bool primary)
            {
                mName = name;

                mPrimary = primary;
            }

            public abstract bool IsValid
            {
                get;
            }

            public override string ToString()
            {
                return mName;
            }
        }

        public class DocumentBootFile : BootFile
        {
            readonly XmlDocument mDocument;

            public DocumentBootFile(string reference, string name, bool primary)
                : base(name, primary)
            {
                try
                {
                    mDocument = Simulator.LoadXML(mName);

                    if (IsValid)
                    {
                        BooterLogger.AddTrace("Found: " + mName);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mName, e);

                    BooterLogger.AddError(reference + Common.NewLine + mName + ": XML Format Error");
                }
            }

            public override bool IsValid
            {
                get
                {
                    return (mDocument != null);
                }
            }

            public XmlDocument Document
            {
                get
                {
                    return mDocument;
                }
            }

        }

        public class DataBootFile : BootFile
        {
            readonly XmlDbData mFile;

            public DataBootFile(string reference, string name, bool primary)
                : base(name, primary)
            {
                if (!string.IsNullOrEmpty(mName))
                {
                    try
                    {
                        mFile = XmlDbData.ReadData(mName);

                        if (IsValid)
                        {
                            BooterLogger.AddTrace("Found: " + mName);
                        }
                    }
                    catch (Exception e)
                    {
                        BooterLogger.AddError(mName + " Parse fail");

                        Common.Exception(reference + Common.NewLine + mName, e);
                    }
                }
            }

            public override bool IsValid
            {
                get
                {
                    if (mFile == null) return false;

                    return (mFile.Tables != null);
                }
            }

            public XmlDbData Data
            {
                get
                {
                    return mFile;
                }
            }

            public XmlDbTable GetTable(string table)
            {
                if (!IsValid) return null;

                if (!mFile.Tables.ContainsKey(table)) return null;

                return mFile.Tables[table];
            }

            public void Load(string listing, string field, List<BootFile> files)
            {
                XmlDbTable table = GetTable(listing);
                if (table != null)
                {
                    if ((table.Rows == null) || (table.Rows.Count == 0))
                    {
                        BooterLogger.AddError(mName + ": " + table + " table empty");
                    }
                    else
                    {
                        BooterLogger.AddTrace(mName + ": Found " + listing + " = " + table.Rows.Count);

                        foreach (XmlDbRow row in table.Rows)
                        {
                            BootFile file = new DataBootFile(mName, row.GetString(field), false);
                            if (file.IsValid)
                            {
                                files.Add(file);
                            }
                        }
                    }
                }
                else
                {
                    BooterLogger.AddTrace(mName + ": No " + listing + " Table");
                }
            }
        }

        public class DataBootTable
        {
            readonly XmlDbTable mTable = null;

            readonly DataBootFile mFile = null;

            readonly string mName = null;

            public DataBootTable(DataBootFile file, string table)
            {
                mName = table;

                if (file != null)
                {
                    mFile = file;
                    mTable = file.GetTable(table);
                }
            }

            public bool IsValid
            {
                get
                {
                    if (mTable == null) return false;

                    return (mTable.Rows != null);
                }
            }

            public delegate void LoadFunc(BootFile file, XmlDbRow row);

            public bool Load(LoadFunc func)
            {
                if (!IsValid) return false;

                BooterLogger.AddTrace(mName + ": Rows = " + mTable.Rows.Count);

                for (int i=0; i<mTable.Rows.Count; i++)
                {
                    BooterLogger.AddTrace("Row: " + i);

                    func(mFile, mTable.Rows[i]);
                }

                return true;
            }

            public override string ToString()
            {
                string name = mName;

                if (mFile != null)
                {
                    name = mFile.ToString() + " " + name;
                }

                return name;
            }
        }
    }
}