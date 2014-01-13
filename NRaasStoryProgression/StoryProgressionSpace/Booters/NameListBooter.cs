using NRaas.CommonSpace.Booters;
using NRaas.StoryProgressionSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Booters
{
    public class FirstNameListBooter : NameListBooter
    {
        static NameList sNames = new NameList();

        public FirstNameListBooter()
            : base("Names", VersionStamp.sNamespace + ".NameList")
        { }

        public override void OnWorldLoadFinished()
        {
            sNames.Clear();

            LoadNames(true);
            LoadNames(false);
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            sNames.Load(row);
        }

        protected static void LoadNames(bool isFemale)
        {
            Dictionary<string, bool> names = new Dictionary<string, bool>();
            LoadHumanFirstNames(isFemale, names);
            sNames.AddNames(CASAgeGenderFlags.Human, isFemale, names);

            foreach (CASAgeGenderFlags species in new CASAgeGenderFlags[] { CASAgeGenderFlags.Cat, CASAgeGenderFlags.Dog, CASAgeGenderFlags.LittleDog, CASAgeGenderFlags.Horse })
            {
                names.Clear();
                LoadPetFirstNames(isFemale, species, names);
                sNames.AddNames(species, isFemale, names);
            }

            //BooterLogger.AddError(sNames.ToString());
        }

        private static void LoadPetFirstNames(bool isFemale, CASAgeGenderFlags species, Dictionary<string, bool> names)
        {
            for (int i = 0; i < 1000; i++)
            {
                string name = SimUtils.GetRandomPetName(!isFemale, species, true);

                names[name] = true;
            }
        }

        public static string GetRandomName(bool fullList, CASAgeGenderFlags species, bool isFemale)
        {
            return sNames.GetRandomName(fullList, species, isFemale);
        }

        private static void LoadHumanFirstNames(bool isFemale, Dictionary<string, bool> names)
        {
            bool isMale = !isFemale;

            string key;
            int total = 0;

            switch (GameUtils.GetCurrentWorld())
            {
                case WorldName.Egypt:
                    key = isMale ? "Gameplay/SimNames/MaleName/Egypt:Name" : "Gameplay/SimNames/FemaleName/Egypt:Name";
                    total = isMale ? SimUtils.kLocaleSpecificNumberMaleGivenNames[0x2] : SimUtils.kLocaleSpecificNumberFemaleGivenNames[0x2];
                    break;
                case WorldName.China:
                    key = isMale ? "Gameplay/SimNames/MaleName/China:Name" : "Gameplay/SimNames/FemaleName/China:Name";
                    total = isMale ? SimUtils.kLocaleSpecificNumberMaleGivenNames[0x1] : SimUtils.kLocaleSpecificNumberFemaleGivenNames[0x1];
                    break;
                case WorldName.France:
                    key = isMale ? "Gameplay/SimNames/MaleName/France:Name" : "Gameplay/SimNames/FemaleName/France:Name";
                    total = isMale ? SimUtils.kLocaleSpecificNumberMaleGivenNames[0x3] : SimUtils.kLocaleSpecificNumberFemaleGivenNames[0x3];
                    break;
                default:
                    key = isMale ? "Gameplay/SimNames/MaleName:Name" : "Gameplay/SimNames/FemaleName:Name";
                    total = isMale ? SimUtils.kLocaleSpecificNumberMaleGivenNames[0x0] : SimUtils.kLocaleSpecificNumberFemaleGivenNames[0x0];
                    break;
            }

            for (int i = 0; i < total; i++)
            {
                string name = Common.LocalizeEAString(key + i);

                names[name] = true;
            }
        }

        public static string StaticToString()
        {
            return sNames.ToString();
        }
    }

    public class LastNameListBooter : NameListBooter
    {
        static NameList sNames = new NameList();

        public LastNameListBooter()
            : base("LastNames", VersionStamp.sNamespace + ".NameList")
        { }

        public override void OnWorldLoadFinished()
        {
            sNames.Clear();

            LoadNames(true);
            LoadNames(false);
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            sNames.Load(row);
        }

        public static string GetRandomName(bool fullList, CASAgeGenderFlags species, bool isFemale)
        {
            return sNames.GetRandomName(fullList, species, isFemale);
        }

        protected static void LoadNames(bool isFemale)
        {
            Dictionary<string, bool> names = new Dictionary<string, bool>();
            LoadLastNames(isFemale, names);
            sNames.AddNames(CASAgeGenderFlags.Human, isFemale, names);

            foreach (CASAgeGenderFlags species in new CASAgeGenderFlags[] { CASAgeGenderFlags.Cat, CASAgeGenderFlags.Dog, CASAgeGenderFlags.LittleDog, CASAgeGenderFlags.Horse })
            {
                names.Clear();
                LoadLastNames(isFemale, names);
                sNames.AddNames(species, isFemale, names);
            }
        }

        private static void LoadLastNames(bool isFemale, Dictionary<string, bool> names)
        {
            int total;
            string key;

            switch (GameUtils.GetCurrentWorld())
            {
                case WorldName.Egypt:
                    key = "Gameplay/SimNames/FamilyName/Egypt:Name";
                    total = SimUtils.kLocaleSpecificNumberFamilyNames[0x2];
                    break;
                case WorldName.China:
                    key = "Gameplay/SimNames/FamilyName/China:Name";
                    total = SimUtils.kLocaleSpecificNumberFamilyNames[0x1];
                    break;
                case WorldName.France:
                    key = "Gameplay/SimNames/FamilyName/France:Name";
                    total = SimUtils.kLocaleSpecificNumberFamilyNames[0x3];
                    break;
                default:
                    key = "Gameplay/SimNames/FamilyName:Name";
                    total = SimUtils.kLocaleSpecificNumberFamilyNames[0x0];
                    break;
            }

            for (int i = 0; i < total; i++)
            {
                string name = Common.LocalizeEAString(key + i);

                names[name] = true;
            }
        }

        public static string StaticToString()
        {
            return sNames.ToString();
        }
    }

    public abstract class NameListBooter : BooterHelper.ByRowBooter, Common.IWorldLoadFinished
    {
        protected static bool sCustomNames = false;

        public NameListBooter(string table, string reference)
            : base(table, reference)
        { }

        public abstract void OnWorldLoadFinished();

        public static bool HasCustomNames()
        {
            return sCustomNames;
        }

        public class Genders
        {
            List<string> mMales = new List<string>();            
            List<string> mFemales = new List<string>();

            Dictionary<string, bool> mMaleExclusions = new Dictionary<string, bool>();
            Dictionary<string, bool> mFemaleExclusions = new Dictionary<string, bool>();

            public Genders()
            { }

            public List<string> GetNames(bool isFemale)
            {
                if (isFemale)
                {
                    return mFemales;
                }
                else
                {
                    return mMales;
                }
            }

            public Dictionary<string, bool> GetExclusions(bool isFemale)
            {
                if (isFemale)
                {
                    return mFemaleExclusions;
                }
                else
                {
                    return mMaleExclusions;
                }
            }

            public override string ToString()
            {
                Common.StringBuilder results = new Common.StringBuilder();

                results += Common.NewLine + "Male: ";
                foreach (string name in mMales)
                {
                    results += "," + name;
                }

                results += Common.NewLine + "Female: ";
                foreach (string name in mFemales)
                {
                    results += "," + name;
                }

                results += Common.NewLine + "MaleExclusions: ";
                foreach (string name in mMaleExclusions.Keys)
                {
                    results += "," + name;
                }

                results += Common.NewLine + "FemaleExclusions: ";
                foreach (string name in mFemaleExclusions.Keys)
                {
                    results += "," + name;
                }

                return results.ToString();
            }
        }

        public class NameList
        {
            Dictionary<CASAgeGenderFlags, Genders> mLoadedNames = new Dictionary<CASAgeGenderFlags, Genders>();

            Dictionary<CASAgeGenderFlags, Genders> mNames = new Dictionary<CASAgeGenderFlags, Genders>();

            public void Clear()
            {
                mNames.Clear();
            }

            public override string ToString()
            {
                Common.StringBuilder results = new Common.StringBuilder();

                results += "LoadedNames";
                foreach (KeyValuePair<CASAgeGenderFlags, Genders> loadedNames in mLoadedNames)
                {
                    results += Common.NewLine + "Species: " + loadedNames.Key;
                    results += Common.NewLine + loadedNames.Value;
                }

                results += Common.NewLine + "Names";
                foreach (KeyValuePair<CASAgeGenderFlags, Genders> names in mNames)
                {
                    results += Common.NewLine + "Species: " + names.Key;
                    results += Common.NewLine + names.Value;
                }

                return results.ToString();
            }

            public string GetRandomName(bool fullList, CASAgeGenderFlags species, bool isFemale)
            {
                Genders genders = null;
                if (!fullList)
                {
                    if (!mLoadedNames.TryGetValue(species, out genders))
                    {
                        genders = null;
                    }
                }

                if (genders == null)
                {
                    if (!mNames.TryGetValue(species, out genders)) return "";
                }

                List<string> names = genders.GetNames(isFemale);
                if (names.Count == 0) return "";

                return RandomUtil.GetRandomObjectFromList(names);
            }

            public void AddNames(CASAgeGenderFlags species, bool isFemale, Dictionary<string, bool> names)
            {
                Genders genders;
                if (!mNames.TryGetValue(species, out genders))
                {
                    genders = new Genders();
                    mNames.Add(species, genders);

                    Genders loadedGenders;
                    if (mLoadedNames.TryGetValue(species, out loadedGenders))
                    {
                        genders.GetNames(isFemale).AddRange(loadedGenders.GetNames(isFemale));

                        foreach (string name in loadedGenders.GetExclusions(isFemale).Keys)
                        {
                            genders.GetExclusions(isFemale).Add(name, true);
                        }
                    }
                }

                foreach (string name in names.Keys)
                {
                    if (genders.GetExclusions(isFemale).ContainsKey(name)) continue;

                    genders.GetNames(isFemale).Add(name);
                }

                names.Clear();
            }

            public void Load(XmlDbRow row)
            {
                sCustomNames = true;

                CASAGSAvailabilityFlags specieFlags = ParserFunctions.ParseAllowableAgeSpecies(row, "Species");

                string totalName = row.GetString("Name");
                if (string.IsNullOrEmpty(totalName)) return;

                //BooterLogger.AddError("Names Found: " + totalName);

                string[] names = totalName.Split(',');

                bool exclusion = row.GetBool("Exclude");

                bool isMale = false;
                bool isFemale = false;

                {
                    CASAgeGenderFlags gender = row.GetEnum<CASAgeGenderFlags>("Gender", CASAgeGenderFlags.Male | CASAgeGenderFlags.Female);

                    if ((gender & CASAgeGenderFlags.Male) == CASAgeGenderFlags.Male)
                    {
                        isMale = true;
                    }

                    if ((gender & CASAgeGenderFlags.Female) == CASAgeGenderFlags.Female)
                    {
                        isFemale = true;
                    }
                }

                List<CASAgeGenderFlags> species = new List<CASAgeGenderFlags>();

                if ((specieFlags & CASAGSAvailabilityFlags.AllCatsMask) != CASAGSAvailabilityFlags.None)
                {
                    species.Add(CASAgeGenderFlags.Cat);
                }

                if ((specieFlags & CASAGSAvailabilityFlags.AllDogsMask) != CASAGSAvailabilityFlags.None)
                {
                    species.Add(CASAgeGenderFlags.Dog);
                }

                if ((specieFlags & CASAGSAvailabilityFlags.AllLittleDogsMask) != CASAGSAvailabilityFlags.None)
                {
                    species.Add(CASAgeGenderFlags.LittleDog);
                }

                if ((specieFlags & CASAGSAvailabilityFlags.AllHorsesMask) != CASAGSAvailabilityFlags.None)
                {
                    species.Add(CASAgeGenderFlags.Horse);
                }

                if ((specieFlags & CASAGSAvailabilityFlags.HumanAgeMask) != CASAGSAvailabilityFlags.None)
                {
                    species.Add(CASAgeGenderFlags.Human);
                }

                foreach (CASAgeGenderFlags specie in species)
                {
                    Genders genderList;
                    if (!mLoadedNames.TryGetValue(specie, out genderList))
                    {
                        genderList = new Genders();
                        mLoadedNames.Add(specie, genderList);
                    }

                    for (int i = 0; i < names.Length; i++)
                    {
                        string name = names[i].Trim();

                        if (isMale)
                        {
                            if (exclusion)
                            {
                                genderList.GetExclusions(false).Add(name, true);
                            }
                            else
                            {
                                genderList.GetNames(false).Add(name);
                            }
                        }

                        if (isFemale)
                        {
                            if (exclusion)
                            {
                                genderList.GetExclusions(true).Add(name, true);
                            }
                            else
                            {
                                genderList.GetNames(true).Add(name);
                            }
                        }
                    }
                }
            }
        }
    }
}

