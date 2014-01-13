using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Booters
{
    public abstract class InvalidPartBooter
    {
        static Dictionary<ResourceKey, List<InvalidPartBase>> sValidPartsByKey = new Dictionary<ResourceKey, List<InvalidPartBase>>();
        static Dictionary<ResourceKey, List<InvalidPartBase>> sInvalidPartsByKey = new Dictionary<ResourceKey, List<InvalidPartBase>>();

        static Dictionary<BodyTypes, List<InvalidPartBase>> sValidPartsByType = new Dictionary<BodyTypes, List<InvalidPartBase>>();
        static Dictionary<BodyTypes, List<InvalidPartBase>> sInvalidPartsByType = new Dictionary<BodyTypes, List<InvalidPartBase>>();

        public static void ParseParts<TYPE>()
            where TYPE : InvalidPartBase, new()
        {
            ParseValidParts<TYPE>();
            ParseInvalidParts<TYPE>();
        }

        public static IEnumerable<ResourceKey> InvalidPartKeys
        {
            get { return sInvalidPartsByKey.Keys; }
        }

        public static IEnumerable<ResourceKey> ValidPartKeys
        {
            get { return sValidPartsByKey.Keys; }
        }

        public static void ParseValidParts<TYPE>()
            where TYPE : InvalidPartBase, new()
        {
            ParseParts<TYPE>("ValidParts", sValidPartsByKey, sValidPartsByType);
        }

        public static void ParseInvalidParts<TYPE>()
            where TYPE : InvalidPartBase, new()
        {
            ParseParts<TYPE>("InvalidParts", sInvalidPartsByKey, sInvalidPartsByType);
        }

        public static bool HasInvalidParts
        {
            get
            {
                if (sInvalidPartsByKey.Count > 0) return true;

                if (sInvalidPartsByType.Count > 0) return true;

                return false;
            }
        }

        public static int InvalidPartsCount
        {
            get
            {
                return (sInvalidPartsByKey.Count + sInvalidPartsByType.Count);
            }
        }

        public static bool HasValidParts
        {
            get
            {
                if (sValidPartsByKey.Count > 0) return true;

                return (sValidPartsByType.Count > 0);
            }
        }

        public static InvalidPartBase.Reason Allow(CASParts.Wrapper part, CASAgeGenderFlags age, CASAgeGenderFlags gender, CASAgeGenderFlags species, bool maternity, OutfitCategories category)
        {
            OutfitCategoriesExtended extended = (OutfitCategoriesExtended)0;

            if ((part.ExtendedCategory & OutfitCategoriesExtended.IsHat) == OutfitCategoriesExtended.IsHat)
            {
                extended |= OutfitCategoriesExtended.IsHat;
            }

            if (maternity)
            {
                extended |= OutfitCategoriesExtended.ValidForMaternity;
            }

            if (species == CASAgeGenderFlags.Human)
            {
                switch (part.BodyType)
                {
                    case BodyTypes.FullBody:
                    case BodyTypes.LowerBody:
                    case BodyTypes.UpperBody:
                    case BodyTypes.Shoes:
                    case BodyTypes.Hair:
                        switch (age)
                        {
                            case CASAgeGenderFlags.Baby:
                            case CASAgeGenderFlags.Toddler:
                            case CASAgeGenderFlags.Child:
                                if (part.Age != age)
                                {
                                    return InvalidPartBase.Reason.ImproperAge;
                                }
                                break;
                            default:
                                if ((part.Age & (CASAgeGenderFlags.Baby | CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Child)) != CASAgeGenderFlags.None)
                                {
                                    return InvalidPartBase.Reason.ImproperAge;
                                }
                                break;
                        }
                        break;
                }
            }

            InvalidPartBase.Reason reason = InvalidPartBase.Reason.None;

            if ((sValidPartsByKey.Count > 0) || (sValidPartsByType.Count > 0))
            {
                List<InvalidPartBase> tests;
                if (sValidPartsByKey.TryGetValue(part.Key, out tests))
                {
                    foreach (InvalidPartBase test in tests)
                    {
                        if (test.Allow(age, gender, species, category, extended, out reason)) return InvalidPartBase.Reason.None;
                    }
                }

                if (sValidPartsByType.TryGetValue(part.BodyType, out tests))
                {
                    foreach (InvalidPartBase test in tests)
                    {
                        if (test.Allow(age, gender, species, category, extended, out reason)) return InvalidPartBase.Reason.None;
                    }
                }

                return InvalidPartBase.Reason.ValidFail;
            }
            else
            {
                List<InvalidPartBase> tests;
                if (sInvalidPartsByKey.TryGetValue(part.Key, out tests))
                {
                    foreach (InvalidPartBase test in tests)
                    {
                        if (!test.Allow(age, gender, species, category, extended, out reason)) return reason;
                    }
                }

                if (sInvalidPartsByType.TryGetValue(part.BodyType, out tests))
                {
                    foreach (InvalidPartBase test in tests)
                    {
                        if (!test.Allow(age, gender, species, category, extended, out reason)) return reason;
                    }
                }

                return InvalidPartBase.Reason.None;
            }
        }

        public static void AddInvalidPart(ResourceKey key, InvalidPartBase part)
        {
            List<InvalidPartBase> tests;
            if (!sInvalidPartsByKey.TryGetValue(key, out tests))
            {
                tests = new List<InvalidPartBase>();
                sInvalidPartsByKey.Add(key, tests);
            }

            tests.Add(part);
        }

        protected static void ParseParts<TYPE>(string suffix, Dictionary<ResourceKey, List<InvalidPartBase>> partsByKey, Dictionary<BodyTypes, List<InvalidPartBase>> partsByType)
            where TYPE : InvalidPartBase, new()
        {
            partsByKey.Clear();
            partsByType.Clear();

            BooterLogger.AddTrace(suffix + ":OnPreLoad");

            string name = VersionStamp.sNamespace + "." + suffix;

            XmlDbData data = null;
            try
            {
                data = XmlDbData.ReadData(name);
                if ((data == null) || (data.Tables == null))
                {
                    BooterLogger.AddTrace(name + " Missing");
                    return;
                }
            }
            catch (Exception e)
            {
                BooterLogger.AddTrace(name + " Formatting Error");

                Common.Exception(name, e);
                return;
            }

            XmlDbTable table = data.Tables[suffix];
            if ((table != null) && (table.Rows != null))
            {
                Dictionary<ResourceKey, bool> allParts = new Dictionary<ResourceKey, bool>();

                BooterLogger.AddTrace(name + " PartSearch");

                PartSearch search = new PartSearch();

                foreach (CASPart part in search)
                {
                    if (allParts.ContainsKey(part.Key)) continue;

                    allParts.Add(part.Key, true);
                }

                search.Reset();

                BooterLogger.AddTrace(name + " Rows");

                foreach (XmlDbRow row in table.Rows)
                {
                    try
                    {
                        OutfitCategories categories;
                        if (!row.TryGetEnum<OutfitCategories>("Categories", out categories, OutfitCategories.All))
                        {
                            BooterLogger.AddError(suffix + " Unknown Categories: " + row.GetString("Categories"));
                            continue;
                        }

                        /*
                        ProductVersion productVersion = ProductVersion.Undefined;
                        if (row.Exists("ProductVersion"))
                        {
                            if (!row.TryGetEnum<ProductVersion>(row["ProductVersion"], out productVersion, ProductVersion.Undefined))
                            {
                                BooterLogger.AddError(suffix + " Unknown WorldTypes: " + row.GetString("WorldTypes"));
                                continue;
                            }
                        }
                        */
                        List<WorldType> worldTypes = new List<WorldType>();
                        if (row.Exists("WorldTypes"))
                        {
                            if (!ParserFunctions.TryParseCommaSeparatedList<WorldType>(row["WorldTypes"], out worldTypes, WorldType.Undefined))
                            {
                                BooterLogger.AddError(suffix + " Unknown WorldTypes: " + row.GetString("WorldTypes"));
                                continue;
                            }
                        }

                        OutfitCategoriesExtended extended = OutfitCategoriesExtended.ValidForRandom;
                        if (row.Exists("Extended"))
                        {
                            if (!row.TryGetEnum<OutfitCategoriesExtended>("Extended", out extended, OutfitCategoriesExtended.ValidForRandom))
                            {
                                BooterLogger.AddError(suffix + " Unknown Extended: " + row.GetString("Extended"));
                                continue;
                            }
                        }

                        CASAgeGenderFlags age;
                        if (!row.TryGetEnum<CASAgeGenderFlags>("Age", out age, CASAgeGenderFlags.AgeMask))
                        {
                            BooterLogger.AddError(suffix + " Unknown Age: " + row.GetString("Age"));
                            continue;
                        }

                        CASAgeGenderFlags gender;
                        if (!row.TryGetEnum<CASAgeGenderFlags>("Gender", out gender, CASAgeGenderFlags.GenderMask))
                        {
                            BooterLogger.AddError(suffix + " Unknown Gender: " + row.GetString("Gender"));
                            continue;
                        }

                        CASAgeGenderFlags species = CASAgeGenderFlags.Human;

                        if (row.Exists("Species"))
                        {
                            if (!row.TryGetEnum<CASAgeGenderFlags>("Species", out species, CASAgeGenderFlags.Human))
                            {
                                BooterLogger.AddError(suffix + " Unknown Species: " + row.GetString("Species"));
                                continue;
                            }
                        }

                        BodyTypes type = BodyTypes.None;

                        if (!string.IsNullOrEmpty(row.GetString("BodyType")))
                        {
                            if (!row.TryGetEnum<BodyTypes>("BodyType", out type, BodyTypes.None))
                            {
                                BooterLogger.AddError(suffix + " Unknown BodyTypes: " + row.GetString("BodyType"));
                                continue;
                            }
                        }

                        ResourceKey key = ResourceKey.kInvalidResourceKey;

                        List<InvalidPartBase> tests = null;

                        if (type == BodyTypes.None)
                        {
                            ulong instance = row.GetUlong("Instance");
                            if (instance == 0)
                            {
                                BooterLogger.AddError(suffix + " Invalid Instance " + row.GetString("Key"));
                                continue;
                            }

                            uint group = row.GetUInt("Group");

                            key = new ResourceKey(instance, 0x034aeecb, group);

                            if (!allParts.ContainsKey(key))
                            {
                                BooterLogger.AddError(suffix + " Key not found: " + key);
                                continue;
                            }

                            if (!partsByKey.TryGetValue(key, out tests))
                            {
                                tests = new List<InvalidPartBase>();
                                partsByKey.Add(key, tests);
                            }
                        }
                        else
                        {
                            if (!partsByType.TryGetValue(type, out tests))
                            {
                                tests = new List<InvalidPartBase>();
                                partsByType.Add(type, tests);
                            }
                        }

                        TYPE newPart = new TYPE();
                        newPart.Set(categories, worldTypes, extended, age, gender, species);

                        tests.Add(newPart);
                    }
                    catch (Exception e)
                    {
                        string setDump = suffix;
                        foreach (string column in row.ColumnNames)
                        {
                            setDump += Common.NewLine + column + " = " + row.GetString(column);
                        }

                        Common.Exception(setDump, e);
                    }
                }

                BooterLogger.AddTrace(suffix + " Parts By Key Added: " + partsByKey.Count);
                BooterLogger.AddTrace(suffix + " Parts By Type Added: " + partsByType.Count);
            }
        }
    }
}
