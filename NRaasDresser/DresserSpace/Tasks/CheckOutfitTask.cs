using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NRaas.DresserSpace.Tasks
{
    public class CheckOutfitTask : Common.FunctionTask
    {
        static Common.MethodStore sMasterControllerAllow = new Common.MethodStore("NRaasMasterController", "NRaas.MasterController", "Allow", new Type[] { typeof(CASPart), typeof(CASAgeGenderFlags), typeof(CASAgeGenderFlags), typeof(CASAgeGenderFlags), typeof(bool), typeof(OutfitCategories) });

        public enum ProcessOptions
        {
            None = 0x0000,
            Invalid = 0x0001,
            Accessories = 0x0002,
            Hair = 0x0004,
            Reroll = 0x0008,
            WriteLog = 0x0010,
            CurrentOutfit = 0x0020,
            Makeup = 0x0040,
            Beard = 0x0080,
            BodyHair = 0x0100,
            Protected = 0x0200,
        }

        static readonly List<OutfitCategories> sCategories = new List<OutfitCategories> (new OutfitCategories[] { OutfitCategories.Naked, OutfitCategories.Everyday, OutfitCategories.Formalwear, OutfitCategories.Sleepwear, OutfitCategories.Swimwear, OutfitCategories.Athletic, OutfitCategories.Career, OutfitCategories.Bridle, OutfitCategories.Jumping, OutfitCategories.Racing, OutfitCategories.Outerwear });

        static Dictionary<BodyTypes, Dictionary<ResourceKey, CASParts.Wrapper>> sAllParts = null;

        SimDescription mSim;

        ProcessOptions mOptions;

        static Dictionary<BodyTypes, ProcessOptions> sTypeLookup;

        protected CheckOutfitTask(SimDescription sim, ProcessOptions options)
        {
            mSim = sim;
            mOptions = options;
        }

        public static Dictionary<BodyTypes, ProcessOptions> TypeLookup
        {
            get
            {
                if (sTypeLookup == null)
                {
                    sTypeLookup = new Dictionary<BodyTypes, ProcessOptions>();

                    foreach (BodyTypes type in CASParts.BodyHairTypes)
                    {
                        sTypeLookup.Add(type, ProcessOptions.BodyHair);
                    }

                    sTypeLookup.Add(BodyTypes.Beard, ProcessOptions.Beard);
                    sTypeLookup.Add(BodyTypes.Hair, ProcessOptions.Hair);

                    foreach (BodyTypes type in CASParts.sAccessories)
                    {
                        sTypeLookup.Add(type, ProcessOptions.Accessories);
                    }

                    foreach (BodyTypes type in CASParts.sMakeup)
                    {
                        sTypeLookup.Add(type, ProcessOptions.Makeup);
                    }
                }

                return sTypeLookup;
            }
        }

        protected static Dictionary<BodyTypes, Dictionary<ResourceKey, CASParts.Wrapper>> AllParts
        {
            get
            {
                if (sAllParts == null)
                {
                    sAllParts = new Dictionary<BodyTypes, Dictionary<ResourceKey, CASParts.Wrapper>>();

                    PartSearch search = new PartSearch();
                    foreach (CASPart part in search)
                    {
                        OutfitCategoriesExtended partCategory = (OutfitCategoriesExtended)part.CategoryFlags;

                        Dictionary<ResourceKey, CASParts.Wrapper> parts;
                        if (!sAllParts.TryGetValue(part.BodyType, out parts))
                        {
                            parts = new Dictionary<ResourceKey, CASParts.Wrapper>();
                            sAllParts.Add(part.BodyType, parts);
                        }

                        if (!parts.ContainsKey(part.Key))
                        {
                            parts.Add(part.Key, new CASParts.Wrapper(part));
                        }
                    }

                    search.Reset();
                }

                return sAllParts;
            }
        }

        public static ProcessOptions GetType(BodyTypes type)
        {
            ProcessOptions option;
            if (TypeLookup.TryGetValue(type, out option))
            {
                return option;
            }
            else
            {
                return ProcessOptions.None;
            }
        }

        protected static bool IsValidProductVersion(CASParts.Wrapper part)
        {
            ProductVersion productVersion = part.GetVersion();
            if (Dresser.Settings.mInvalidProductVersionGeneral.Contains(productVersion)) return false;

            switch (GetType(part.BodyType))
            {
                case ProcessOptions.BodyHair:
                case ProcessOptions.Beard:
                    if (Dresser.Settings.mInvalidProductVersionBodyHair.Contains(productVersion)) return false;
                    break;
                case ProcessOptions.Accessories:
                    if (Dresser.Settings.mInvalidProductVersionAccessories.Contains(productVersion)) return false;
                    break;
                case ProcessOptions.Hair:
                    if (Dresser.Settings.mInvalidProductVersionHair.Contains(productVersion)) return false;
                    break;
                case ProcessOptions.Makeup:
                    if (Dresser.Settings.mInvalidProductVersionMakeup.Contains(productVersion)) return false;
                    break;
            }

            return true;
        }

        protected static void Add(Dictionary<string, int> reasons, string reason)
        {
            int value;
            if (!reasons.TryGetValue(reason, out value))
            {
                reasons.Add(reason, 1);
            }
            else
            {
                reasons[reason] = value + 1;
            }
        }

        protected static bool Allow(CASParts.Wrapper part, CASAgeGenderFlags age, CASAgeGenderFlags gender, CASAgeGenderFlags species, bool maternity, OutfitCategories category, out string reason)
        {
            if (!IsValidProductVersion(part))
            {
                reason = "ProductVersion Fail";
                return false;
            }

            if (Dresser.Settings.mUseMasterControllerBlackList)
            {
                if (sMasterControllerAllow.Valid)
                {
                    if (!sMasterControllerAllow.Invoke<bool>(new object[] { part.mPart, age, gender, species, maternity, category }))
                    {
                        reason = "MasterController Blacklist Fail";
                        return false;
                    }
                }
                else
                {
                    Common.Notify("sMasterControllerAllow.Valid = False");
                }
            }

            InvalidPartBase.Reason result = InvalidPartBooter.Allow(part, age, gender, species, maternity, category);
            if (result != InvalidPartBase.Reason.None)
            {
                reason = "Dresser Blacklist Fail: " + result;
                return false;
            }

            if ((part.ExtendedCategory & OutfitCategoriesExtended.IsHat) == OutfitCategoriesExtended.IsHat)
            {
                if ((!Dresser.Settings.mAllowHats) || (Dresser.Settings.mInvalidHatCategories.Contains(category)))
                {
                    reason = "Hat Category Fail";
                    return false;
                }
            }

            reason = null;
            return true;
        }

        protected static CASParts.PartPreset GetRandomPreset(BodyTypes location, OutfitCategories category, CASAgeGenderFlags age, CASAgeGenderFlags gender, CASAgeGenderFlags species, bool maternity, StringBuilder log)
        {
            try
            {
                log.Append(Common.NewLine + "  Location: " + location);
                log.Append(Common.NewLine + "  Category: " + category);
                log.Append(Common.NewLine + "  Age: " + age);
                log.Append(Common.NewLine + "  Gender: " + gender);
                log.Append(Common.NewLine + "  Species: " + species);
                log.Append(Common.NewLine + "  Maternity: " + maternity);

                Dictionary<ResourceKey, CASParts.Wrapper> parts;
                if (AllParts.TryGetValue(location, out parts))
                {
                    log.Append(Common.NewLine + "  Parts Found: " + parts.Count);

                    List<CASParts.Wrapper> choices = new List<CASParts.Wrapper>();

                    Dictionary<string,int> reasons = new Dictionary<string,int>();

                    foreach (CASParts.Wrapper test in parts.Values)
                    {
                        ProcessOptions type = GetType(test.mPart.BodyType);

                        if ((type != ProcessOptions.BodyHair) || (!Dresser.Settings.mIgnoreBodyHairGender))
                        {
                            if ((test.Gender & gender) != gender)
                            {
                                Add(reasons, "Gender Fail");
                                continue;
                            }
                        }

                        if ((test.Age & age) != age)
                        {
                            Add(reasons, "Age Fail");
                            continue;
                        }

                        if (test.Species != species)
                        {
                            Add(reasons, "Species Fail");
                            continue;
                        }

                        if ((test.Category & category) != category)
                        {
                            Add(reasons, "Category Fail");
                            continue;
                        }

                        string reason = null;
                        if (!Allow(test, age, gender, species, maternity, category, out reason))
                        {
                            Add(reasons, reason);
                            continue;
                        }

                        if ((test.ExtendedCategory & OutfitCategoriesExtended.IsHiddenInCAS) != 0x0)
                        {
                            Add(reasons, "HiddenInCAS Fail");
                            continue;
                        }

                        bool testIgnore = false;
                        switch(type)
                        {
                            case ProcessOptions.Beard:
                            case ProcessOptions.BodyHair:
                                testIgnore = !Dresser.Settings.mIgnoreValidForRandomBodyHair;
                                break;
                            case ProcessOptions.Hair:
                                testIgnore = !Dresser.Settings.mIgnoreValidForRandomHair;
                                break;
                            case ProcessOptions.Accessories:
                                testIgnore = !Dresser.Settings.mIgnoreValidForRandomAccessories;
                                break;
                            case ProcessOptions.Makeup:
                                testIgnore = !Dresser.Settings.mSameMakeupForAllCategories;
                                break;
                            default:
                                testIgnore = !Dresser.Settings.mIgnoreValidForRandomClothing;
                                break;
                        }

                        if ((testIgnore) && (test.ExtendedCategory & OutfitCategoriesExtended.ValidForRandom) == 0)
                        {
                            Add(reasons, "ValidForRandom Fail");
                            continue;
                        }

                        if (maternity)
                        {
                            if ((test.ExtendedCategory & OutfitCategoriesExtended.ValidForMaternity) == 0)
                            {
                                Add(reasons, "ValidForMaternity Fail");
                                continue;
                            }
                        }

                        choices.Add(test);
                    }

                    foreach (KeyValuePair<string, int> reason in reasons)
                    {
                        log.Append(Common.NewLine + "   " + reason.Key + ": " + reason.Value);
                    }

                    log.Append(Common.NewLine + "  Final Count: " + choices.Count + Common.NewLine);

                    if (choices.Count > 0)
                    {
                        int tries = 0;
                        while (tries < 10)
                        {
                            tries++;

                            CASParts.PartPreset preset = RandomUtil.GetRandomObjectFromList(choices).GetRandomPreset();
                            if (preset != null)
                            {
                                return preset;
                            }
                        }
                    }
                }
                else
                {
                    log.Append(Common.NewLine + "  No Parts Found" + Common.NewLine);
                }
            }
            catch (Exception e)
            {
                Common.Exception(location.ToString(), e);
            }

            return null;
        }

        protected static List<CASParts.PartPreset> GetFromNakedOutfit(SimDescription sim, BodyTypes location)
        {
            SimOutfit outfit = sim.GetOutfit(OutfitCategories.Naked, 0);
            if (outfit == null) return new List<CASParts.PartPreset>();

            List<CASParts.PartPreset> results = new List<CASParts.PartPreset>();

            foreach (CASPart part in outfit.Parts)
            {
                if (location == BodyTypes.FullBody)
                {
                    if ((part.BodyType == BodyTypes.UpperBody) || (part.BodyType == BodyTypes.LowerBody))
                    {
                        results.Add(new CASParts.PartPreset(part, outfit));
                    }
                }
                else if (location == part.BodyType)
                {
                    results.Add(new CASParts.PartPreset(part, outfit));
                }
            }

            return results;
        }

        public static ProcessOptions GetCheck(SimOutfit outfit)
        {
            ProcessOptions results = ProcessOptions.None;

            foreach (CASPart part in outfit.Parts)
            {
                if (CASParts.sAccessories.Contains(part.BodyType))
                {
                    results |= ProcessOptions.Accessories;
                }
                else if (CASParts.IsMakeup(part.BodyType))
                {
                    results |= ProcessOptions.Makeup;
                }
                else if (part.BodyType == BodyTypes.Beard)
                {
                    results |= ProcessOptions.Beard;
                }
                else if (CASParts.BodyHairTypes.Contains(part.BodyType))
                {
                    results |= ProcessOptions.BodyHair;
                }
            }

            return results;
        }

        public static string ConvertToString(CASAgeGenderFlags flags)
        {
            string results = null;

            foreach (CASAgeGenderFlags flag in Enum.GetValues(typeof(CASAgeGenderFlags)))
            {
                switch (flag)
                {
                    case CASAgeGenderFlags.None:
                    case CASAgeGenderFlags.AgeMask:
                    case CASAgeGenderFlags.GenderMask:
                        continue;
                }

                if ((flags & flag) == flag)
                {
                    results += ", " + flag.ToString();
                }
            }

            return results;
        }
        public static string ConvertToString(OutfitCategories flags)
        {
            string results = null;

            foreach (OutfitCategories flag in Enum.GetValues(typeof(OutfitCategories)))
            {
                switch (flag)
                {
                    case OutfitCategories.None:
                        continue;
                }

                if ((flags & flag) == flag)
                {
                    results += ", " + flag.ToString();
                }
            }

            return results;
        }
        public static string ConvertToString(ProcessOptions flags)
        {
            string results = null;

            foreach (ProcessOptions flag in Enum.GetValues(typeof(ProcessOptions)))
            {
                switch (flag)
                {
                    case ProcessOptions.None:
                        continue;
                }

                if ((flags & flag) == flag)
                {
                    results += ", " + flag.ToString();
                }
            }

            return results;
        }

        protected static void Clone(Color[] dst, Color[] src)
        {
            if ((dst != null) && (src != null))
            {
                int length = Math.Min(src.Length, dst.Length);

                for (int j = 0; j < length; j++)
                {
                    dst[j] = src[j];
                }
            }
        }

        protected override void OnPerform()
        {
            StringBuilder replacementLog = new StringBuilder();
            
            replacementLog.Append("Replacement Log");

            try
            {
                replacementLog.Append(Common.NewLine + "Options: " + ConvertToString(mOptions));
                replacementLog.Append(Common.NewLine + "Name: " + mSim.FullName);
                replacementLog.Append(Common.NewLine + "Age: " + mSim.Age);
                replacementLog.Append(Common.NewLine + "Gender: " + mSim.Gender);
                replacementLog.Append(Common.NewLine + "Species: " + mSim.Species);

                if (!mSim.IsValidDescription)
                {
                    replacementLog.Append(Common.NewLine + "Invalid");
                    return;
                }
                else if (SimTypes.IsSkinJob(mSim))
                {
                    replacementLog.Append(Common.NewLine + "Skin Job");
                    return;
                }

                OutfitCategories currentCategory = OutfitCategories.None;
                int currentIndex = 0;

                try
                {
                    if (mSim.CreatedSim != null)
                    {
                        currentCategory = mSim.CreatedSim.CurrentOutfitCategory;
                        currentIndex = mSim.CreatedSim.CurrentOutfitIndex;
                    }
                }
                catch
                { }

                Dictionary<SimOutfit, List<BodyTypes>> replaceOutfits = new Dictionary<SimOutfit, List<BodyTypes>>();

                bool testExisted = false;
                ProcessOptions testResults = ProcessOptions.None;

                if ((mOptions & ProcessOptions.CurrentOutfit) == ProcessOptions.None)
                {
                    testResults = Dresser.Settings.GetTested(mSim, out testExisted);
                }

                bool processBeard = false, processBodyHair = false, processAccessories = false;

                foreach (OutfitCategories category in sCategories)
                {
                    if ((mOptions & ProcessOptions.CurrentOutfit) != ProcessOptions.None)
                    {
                        if (category != currentCategory) continue;
                    }

                    ArrayList outfits = mSim.GetCurrentOutfits()[category] as ArrayList;
                    if (outfits == null) continue;

                    for (int i = 0; i < outfits.Count; i++)
                    {
                        if ((mOptions & ProcessOptions.CurrentOutfit) != ProcessOptions.None)
                        {
                            if (i != currentIndex) continue;
                        }

                        SimOutfit outfit = outfits[i] as SimOutfit;

                        List<BodyTypes> replace = new List<BodyTypes>();

                        if (!testExisted)
                        {
                            testResults |= GetCheck(outfit);
                        }

                        bool lowerBodyFound = false, upperBodyFound = false;

                        foreach (CASParts.Wrapper part in CASParts.Wrapper.CreateList(outfit.Parts))
                        {
                            switch (part.BodyType)
                            {
                                case BodyTypes.PeltLayer:
                                    continue;
                                case BodyTypes.FullBody:
                                case BodyTypes.PetBody:
                                    lowerBodyFound = true;
                                    upperBodyFound = true;
                                    break;
                                case BodyTypes.LowerBody:
                                    lowerBodyFound = true;
                                    break;
                                case BodyTypes.UpperBody:
                                    upperBodyFound = true;
                                    break;
                            }

                            if ((mOptions & ProcessOptions.Reroll) != ProcessOptions.None)
                            {
                                switch (GetType(part.BodyType))
                                {
                                    case ProcessOptions.Beard:
                                    case ProcessOptions.BodyHair:
                                    case ProcessOptions.Hair:
                                    case ProcessOptions.Accessories:
                                    case ProcessOptions.Makeup:
                                        break;
                                    default:
                                        switch (part.BodyType)
                                        {
                                            case BodyTypes.BirthMark:
                                            case BodyTypes.Dental:
                                            case BodyTypes.Eyebrows:
                                            case BodyTypes.Face:
                                            case BodyTypes.FirstFace:
                                            case BodyTypes.Freckles:
                                            case BodyTypes.EyeColor:
                                            case BodyTypes.Moles:
                                            case BodyTypes.Scalp:
                                            case BodyTypes.Tattoo:
                                            case BodyTypes.TattooTemplate:
                                            case BodyTypes.WeddingRing:                                           
                                                break;
                                            case BodyTypes.UpperBody:
                                            case BodyTypes.LowerBody:
                                            case BodyTypes.FullBody:
                                                if (RandomUtil.CoinFlip())
                                                {
                                                    if (!replace.Contains(BodyTypes.UpperBody))
                                                    {
                                                        replace.Add(BodyTypes.FullBody);
                                                    }
                                                }
                                                else
                                                {
                                                    if (!replace.Contains(BodyTypes.FullBody))
                                                    {
                                                        replace.Add(BodyTypes.UpperBody);
                                                        replace.Add(BodyTypes.LowerBody);
                                                    }
                                                }
                                                replacementLog.Append(Common.NewLine + part.Key + " : BodyType reroll For " + part.BodyType);
                                                break;
                                            default:
                                                replace.Add(part.BodyType);
                                                replacementLog.Append(Common.NewLine + part.Key + " : BodyType reroll For " + part.BodyType);
                                                break;
                                        }                                        
                                        break;
                                }
                            }
                            else
                            {
                                Dictionary<ResourceKey, CASParts.Wrapper> parts;
                                if (!AllParts.TryGetValue(part.BodyType, out parts))
                                {
                                    replace.Add(part.BodyType);

                                    replacementLog.Append(Common.NewLine + part.Key + " : BodyType not found For " + part.BodyType);
                                }
                                else if (!parts.ContainsKey(part.Key))
                                {
                                    replace.Add(part.BodyType);

                                    replacementLog.Append(Common.NewLine + part.Key + " : Key not found For " + part.BodyType);
                                }
                                else if ((GetType(part.BodyType) == ProcessOptions.Accessories) && (!Dresser.Settings.AllowAccessories(mSim)))
                                {
                                    replace.Add(part.BodyType);

                                    replacementLog.Append(Common.NewLine + part.Key + " : Accessories Denied For " + part.BodyType);
                                }
                                else
                                {
                                    string reason = null;
                                    if (!Allow(part, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, category, out reason))
                                    {
                                        replace.Add(part.BodyType);

                                        replacementLog.Append(Common.NewLine + part.Key + " : Invalid Part For " + category);
                                        replacementLog.Append(Common.NewLine + "  " + part.BodyType);
                                        replacementLog.Append(Common.NewLine + "  " + reason);
                                        replacementLog.Append(Common.NewLine + "  " + ConvertToString(part.AgeGenderSpecies));
                                        replacementLog.Append(Common.NewLine + "  " + ConvertToString(part.Category));
                                    }
                                }
                            }
                        }

                        if ((!upperBodyFound) && (!lowerBodyFound))
                        {
                            replace.Add(BodyTypes.FullBody);

                            replacementLog.Append(Common.NewLine + " UpperLower Missing For " + category);
                        }
                        else if (!upperBodyFound)
                        {
                            replace.Add(BodyTypes.UpperBody);

                            replacementLog.Append(Common.NewLine + " Upper Body Missing For " + category);
                        }
                        else if (!lowerBodyFound)
                        {
                            replace.Add(BodyTypes.LowerBody);

                            replacementLog.Append(Common.NewLine + " Lower Body Missing For " + category);
                        }

                        if (replace.Count > 0)
                        {
                            foreach(BodyTypes type in replace)
                            {
                                switch (GetType(type))
                                {
                                    case ProcessOptions.BodyHair:
                                        processBodyHair = true;
                                        break;
                                    case ProcessOptions.Accessories:
                                        processAccessories = true;
                                        break;
                                    case ProcessOptions.Beard:
                                        processBeard = true;
                                        break;
                                }
                            }

                            if (!replaceOutfits.ContainsKey(outfit))
                            {
                                replaceOutfits.Add(outfit, replace);
                            }
                        }
                    }
                }

                if (((mOptions & ProcessOptions.BodyHair) != ProcessOptions.None) && ((testResults & ProcessOptions.BodyHair) == ProcessOptions.None))
                {
                    processBodyHair = true;
                }

                replacementLog.Append(Common.NewLine + " Test Results: " + ConvertToString(testResults));

                bool process = ((mOptions & ProcessOptions.Reroll) != ProcessOptions.None);

                bool processHair = ((mOptions & ProcessOptions.Hair) != ProcessOptions.None);

                if (((mOptions & ProcessOptions.Accessories) != ProcessOptions.None) && ((testResults & ProcessOptions.Accessories) == ProcessOptions.None))
                {
                    process = true;

                    processAccessories = true;
                }
                else if (processBodyHair)
                {
                    mSim.BeardUsesHairColor = true;
                    mSim.BodyHairUsesHairColor = true;
                    mSim.EyebrowsUseHairColor = true;

                    process = true;
                }
                else if (((mOptions & ProcessOptions.Makeup) != ProcessOptions.None) && ((testResults & ProcessOptions.Makeup) == ProcessOptions.None))
                {
                    process = true;
                }
                else if (((mOptions & ProcessOptions.Invalid) != ProcessOptions.None) && (replaceOutfits.Count > 0))
                {
                    process = true;
                }
                else if (processHair)
                {
                    process = true;
                }

                if (!Dresser.Settings.AllowAccessories(mSim))
                {
                    processAccessories = false;
                }

                List<CASParts.PartPreset> bodyHair = new List<CASParts.PartPreset>();

                CASParts.PartPreset beardPart = null;
                if (mSim.IsHuman)
                {
                    if (((mOptions & ProcessOptions.Beard) != ProcessOptions.None) && ((testResults & ProcessOptions.Beard) == ProcessOptions.None))
                    {
                        if (RandomUtil.RandomChance(Dresser.Settings.mRandomBeardChance))
                        {
                            beardPart = GetRandomPreset(BodyTypes.Beard, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog);
                            processBeard = true;

                            replacementLog.Append(Common.NewLine + " Beard Chance Success");
                        }
                        else
                        {
                            replacementLog.Append(Common.NewLine + " Beard Chance Fail");
                        }

                        process = true;
                    }

                    if (processBodyHair)
                    {
                        List<Pair<BodyTypes,CASParts.Wrapper>> bodyHairs = new List<Pair<BodyTypes,CASParts.Wrapper>>();

                        int numBodyHairs = 0;

                        if (Dresser.Settings.mInheritBodyHair)
                        {
                            Dictionary<BodyTypes,CASParts.Wrapper> parts = new Dictionary<BodyTypes,CASParts.Wrapper>();
                            foreach (SimDescription parent in Relationships.GetParents(mSim))
                            {
                                if (parent.Gender != mSim.Gender) continue;

                                SimOutfit outfit = parent.GetOutfit(OutfitCategories.Everyday, 0);
                                if (outfit != null)
                                {
                                    foreach (CASPart part in outfit.Parts)
                                    {
                                        if (GetType(part.BodyType) == ProcessOptions.BodyHair)
                                        {
                                            bodyHairs.Add(new Pair<BodyTypes,CASParts.Wrapper>(part.BodyType, new CASParts.Wrapper(part)));
                                        }
                                    }
                                }
                            }

                            numBodyHairs = bodyHairs.Count;

                            replacementLog.Append(Common.NewLine + " Inherited Body Hair Count " + numBodyHairs);
                        }

                        if (bodyHairs.Count == 0)
                        {
                            StringBuilder testLog = new StringBuilder();
                            foreach (BodyTypes type in CASParts.BodyHairTypes)
                            {
                                if (GetRandomPreset(type, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, testLog) == null) continue;

                                bodyHairs.Add(new Pair<BodyTypes,CASParts.Wrapper>(type, null));
                            }
                        
                            numBodyHairs = Dresser.Settings.mRandomBodyHair.GetRandomAmount(mSim.IsFemale, mSim.Age);
                            if (numBodyHairs > bodyHairs.Count)
                            {
                                numBodyHairs = bodyHairs.Count;
                            }

                            replacementLog.Append(Common.NewLine + " Random Body Hair Count " + numBodyHairs);
                        }

                        Dictionary<BodyTypes, bool> choices = new Dictionary<BodyTypes, bool>();

                        RandomUtil.RandomizeListOfObjects(bodyHairs);

                        for (int j = 0; j < numBodyHairs; j++)
                        {
                            Pair<BodyTypes,CASParts.Wrapper> pair = bodyHairs[j];

                            if (choices.ContainsKey(pair.First)) continue;
                            choices.Add(pair.First, true);

                            CASParts.Wrapper part = pair.Second;
                            if (part != null)
                            {
                                if ((part.mPart.Age & mSim.Age) == mSim.Age)
                                {
                                    bodyHair.Add(part.GetRandomPreset());

                                    replacementLog.Append(Common.NewLine + " Inherited Part " + pair.First);
                                }
                                else
                                {
                                    bodyHair.Add(GetRandomPreset(pair.First, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog));

                                    replacementLog.Append(Common.NewLine + " Inherited Position " + pair.First);
                                }
                            }
                            else
                            {
                                if (pair.First == BodyTypes.BodyHairFullBack)
                                {
                                    if (RandomUtil.CoinFlip())
                                    {
                                        bodyHair.Add(GetRandomPreset(BodyTypes.BodyHairUpperBack, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog));
                                        bodyHair.Add(GetRandomPreset(BodyTypes.BodyHairLowerBack, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog));
                                    }
                                    else
                                    {
                                        bodyHair.Add(GetRandomPreset(BodyTypes.BodyHairFullBack, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog));
                                    }
                                }
                                else
                                {
                                    bodyHair.Add(GetRandomPreset(pair.First, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog));
                                }
                            }
                        }

                        process = true;
                    }
                }

                CASParts.PartPreset hair = null;
                if ((processHair) && (Dresser.Settings.mSameHairForAllCategories))
                {
                    hair = GetRandomPreset(BodyTypes.Hair, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog);

                    replacementLog.Append(Common.NewLine + " Same Hair");
                }

                List<CASParts.PartPreset> makeup = new List<CASParts.PartPreset>();

                bool processIndividualMakeup = false;
                if (((mOptions & ProcessOptions.Makeup) != ProcessOptions.None) && ((testResults & ProcessOptions.Makeup) == ProcessOptions.None))
                {
                    if (Dresser.Settings.mSameMakeupForAllCategories)
                    {
                        int processMakeup = Dresser.Settings.mRandomMakeup.GetRandomAmount(mSim.IsFemale, mSim.Age);

                        replacementLog.Append(Common.NewLine + " Makeup Count " + processMakeup);

                        List<BodyTypes> selection = new List<BodyTypes>(CASParts.sMakeup);
                        selection.Remove(BodyTypes.CostumeMakeup);

                        while ((selection.Count > 0) && (makeup.Count < processMakeup))
                        {
                            BodyTypes type = RandomUtil.GetRandomObjectFromList(selection);
                            selection.Remove(type);

                            CASParts.PartPreset part = GetRandomPreset(type, OutfitCategories.Everyday, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog);
                            if (part == null) continue;

                            makeup.Add(part);
                        }
                    }
                    else
                    {
                        processIndividualMakeup = true;
                    }
                }

                if (process)
                {
                    int replaced = 0, accessoriesAdded = 0, bodyHairAdded = 0, makeupAdded = 0;

                    foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                    {
                        switch (category)
                        {
                            case OutfitCategories.All:
                            case OutfitCategories.None:
                            case OutfitCategories.Special:
                            case OutfitCategories.Supernatural:
                            case OutfitCategories.PrimaryCategories:
                            case OutfitCategories.PrimaryHorseCategories:
                            case OutfitCategories.CategoryMask:
                                continue;
                        }

                        if ((mOptions & ProcessOptions.CurrentOutfit) != ProcessOptions.None)
                        {
                            if (category != currentCategory) continue;
                        }

                        bool primary = sCategories.Contains(category);
                        if ((!primary) && (!processHair) && (!processBeard) && (bodyHair.Count == 0)) continue;

                        replacementLog.Append(Common.NewLine + "Category: " + category);

                        for (int i = 0x0; i < mSim.GetOutfitCount(category); i++)
                        {
                            if ((mOptions & ProcessOptions.CurrentOutfit) != ProcessOptions.None)
                            {
                                if (i != currentIndex) continue;
                            }

                            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(mSim, new CASParts.Key(category, i)))
                            {
                                List<BodyTypes> replace;
                                if (!replaceOutfits.TryGetValue(builder.Outfit, out replace))
                                {
                                    replace = new List<BodyTypes>();
                                }

                                if (processHair)
                                {
                                    if (!replace.Contains(BodyTypes.Hair))
                                    {
                                        replace.Add(BodyTypes.Hair);
                                    }
                                }

                                if (processBeard)
                                {
                                    replace.Add(BodyTypes.Beard);
                                }

                                foreach (CASParts.PartPreset preset in bodyHair)
                                {
                                    if (preset == null) continue;

                                    replace.Add(preset.mPart.BodyType);
                                }

                                if ((primary) && (category != OutfitCategories.Naked))
                                {
                                    if (processAccessories)
                                    {
                                        if ((category != OutfitCategories.MartialArts) || (Dresser.Settings.mMartialArtsAccessories))
                                        {
                                            Dictionary<BodyTypes, bool> choices = new Dictionary<BodyTypes, bool>();

                                            int numAccessories = Dresser.Settings.mRandomAccessories.GetRandomAmount(mSim.IsFemale, mSim.Age);

                                            replacementLog.Append(Common.NewLine + " Accessories Count " + numAccessories);

                                            List<BodyTypes> selection = new List<BodyTypes>(CASParts.sAccessories);
                                            while ((selection.Count > 0) && (choices.Count < numAccessories))
                                            {
                                                BodyTypes type = RandomUtil.GetRandomObjectFromList(selection);
                                                selection.Remove(type);

                                                if (GetRandomPreset(type, category, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog) == null)
                                                {
                                                    continue;
                                                }

                                                choices[type] = true;
                                            }

                                            replace.AddRange(choices.Keys);
                                        }
                                    }

                                    if (makeup != null)
                                    {
                                        foreach (CASParts.PartPreset preset in makeup)
                                        {
                                            if (preset == null) continue;

                                            replace.Add(preset.mPart.BodyType);
                                        }
                                    }
                                    else if (processIndividualMakeup)
                                    {
                                        Dictionary<BodyTypes, bool> choices = new Dictionary<BodyTypes, bool>();

                                        int processMakeup = Dresser.Settings.mRandomMakeup.GetRandomAmount(mSim.IsFemale, mSim.Age);

                                        replacementLog.Append(Common.NewLine + " Makeup Count " + processMakeup);

                                        List<BodyTypes> selection = new List<BodyTypes>(CASParts.sMakeup);
                                        selection.Remove(BodyTypes.CostumeMakeup);

                                        while ((selection.Count > 0) && (choices.Count < processMakeup))
                                        {
                                            BodyTypes type = RandomUtil.GetRandomObjectFromList(selection);
                                            selection.Remove(type);

                                            if (GetRandomPreset(type, category, mSim.Age, mSim.Gender, mSim.Species, mSim.IsUsingMaternityOutfits, replacementLog) == null)
                                            {
                                                continue;
                                            }

                                            choices[type] = true;
                                        }

                                        replace.AddRange(choices.Keys);
                                    }
                                }

                                replacementLog.Append(Common.NewLine + "Index: " + i + " " + replace.Count);

                                bool changed = false;

                                if (replace.Count > 0)
                                {
                                    Dictionary<ResourceKey, bool> existing = new Dictionary<ResourceKey, bool>();
                                    foreach (CASPart part in builder.Outfit.Parts)
                                    {
                                        existing[part.Key] = true;
                                    }

                                    OutfitUtils.ExtractOutfitHairColorAndDyeUsage(mSim, builder.Builder);

                                    foreach (BodyTypes location in replace)
                                    {
                                        List<CASParts.PartPreset> randomParts = new List<CASParts.PartPreset>();

                                        bool allowEmpty = false;

                                        bool randomProcess = true;

                                        switch(GetType(location))
                                        {
                                            case ProcessOptions.Accessories:
                                                if (!Dresser.Settings.AllowAccessories(mSim))
                                                {
                                                    allowEmpty = true;
                                                    randomProcess = false;
                                                }

                                                break;
                                            case ProcessOptions.Beard:
                                                if (beardPart != null)
                                                {
                                                    replacementLog.Append(Common.NewLine + " Same Beard");

                                                    randomParts.Add(beardPart);
                                                }

                                                allowEmpty = true;

                                                randomProcess = false;
                                                break;
                                            case ProcessOptions.Hair:
                                                if (hair != null)
                                                {
                                                    replacementLog.Append(Common.NewLine + " Same Hair");

                                                    randomParts.Add(hair);

                                                    randomProcess = false;
                                                }
                                                break;
                                            case ProcessOptions.Makeup:
                                                if (makeup.Count > 0)
                                                {
                                                    foreach (CASParts.PartPreset preset in makeup)
                                                    {
                                                        if (preset == null) continue;

                                                        if (preset.mPart.BodyType == location)
                                                        {
                                                            replacementLog.Append(Common.NewLine + " Same Makeup");

                                                            randomParts.Add(preset);

                                                            break;
                                                        }
                                                    }

                                                    randomProcess = false;
                                                }
                                                break;
                                            case ProcessOptions.BodyHair:
                                                foreach (CASParts.PartPreset preset in bodyHair)
                                                {
                                                    if (preset == null) continue;

                                                    if (preset.mPart.BodyType == location)
                                                    {
                                                        replacementLog.Append(Common.NewLine + " Same BodyHair");

                                                        randomParts.Add(preset);

                                                        break;
                                                    }
                                                }

                                                allowEmpty = true;

                                                randomProcess = false;
                                                break;
                                        }

                                        if (randomProcess)
                                        {
                                            bool naked = true;

                                            bool[] maternityChoices = null;
                                            if (mSim.IsUsingMaternityOutfits)
                                            {
                                                maternityChoices = new bool[] { mSim.IsUsingMaternityOutfits, false };
                                            }
                                            else
                                            {
                                                maternityChoices = new bool[] { false };
                                            }

                                            foreach (bool maternity in maternityChoices)
                                            {
                                                CASParts.PartPreset randomPart = GetRandomPreset(location, category, mSim.Age, mSim.Gender, mSim.Species, maternity, replacementLog);
                                                if (randomPart != null)
                                                {
                                                    replacementLog.Append(Common.NewLine + " RandomPart For " + location);

                                                    randomParts.Add(randomPart);
                                                    naked = false;
                                                }
                                                else if (location == BodyTypes.FullBody)
                                                {
                                                    CASParts.PartPreset upperPart = GetRandomPreset(BodyTypes.UpperBody, category, mSim.Age, mSim.Gender, mSim.Species, maternity, replacementLog);
                                                    CASParts.PartPreset lowerPart = GetRandomPreset(BodyTypes.LowerBody, category, mSim.Age, mSim.Gender, mSim.Species, maternity, replacementLog);
                                                    if ((upperPart != null) && (lowerPart != null))
                                                    {
                                                        replacementLog.Append(Common.NewLine + " UpperLower For " + location);

                                                        randomParts.Add(upperPart);
                                                        randomParts.Add(lowerPart);

                                                        naked = false;
                                                    }
                                                }

                                                if (!naked) break;
                                            }

                                            if (naked)
                                            {
                                                switch (location)
                                                {
                                                    case BodyTypes.FullBody:
                                                    case BodyTypes.LowerBody:
                                                    case BodyTypes.UpperBody:
                                                    case BodyTypes.Shoes:
                                                        replacementLog.Append(Common.NewLine + " NakedPart For " + location);

                                                        randomParts.AddRange(GetFromNakedOutfit(mSim, location));
                                                        break;
                                                    default:
                                                        allowEmpty = ((CASParts.sAccessories.Contains(location)) || (location == BodyTypes.Beard) || (CASParts.sMakeup.Contains(location)) || (CASParts.BodyHairTypes.Contains(location)));
                                                        break;
                                                }
                                            }
                                        }

                                        if (randomParts.Count > 0)
                                        {
                                            foreach (CASParts.PartPreset newPart in randomParts)
                                            {
                                                if (existing.ContainsKey(newPart.mPart.Key)) continue;

                                                switch(GetType(newPart.mPart.BodyType))
                                                {
                                                    case ProcessOptions.Accessories:
                                                        accessoriesAdded++;
                                                        break;
                                                    case ProcessOptions.Beard:
                                                    case ProcessOptions.BodyHair:
                                                        bodyHairAdded++;
                                                        break;
                                                    case ProcessOptions.Makeup:
                                                        makeupAdded++;
                                                        break;
                                                    default:
                                                        replaced++;
                                                        break;
                                                }

                                                switch (newPart.mPart.BodyType)
                                                {
                                                    case BodyTypes.UpperBody:
                                                    case BodyTypes.LowerBody:
                                                        builder.Builder.RemoveParts(BodyTypes.FullBody);
                                                        break;
                                                    case BodyTypes.FullBody:
                                                        builder.Builder.RemoveParts(BodyTypes.LowerBody);
                                                        builder.Builder.RemoveParts(BodyTypes.UpperBody);
                                                        break;
                                                    case BodyTypes.BodyHairFullBack:
                                                        builder.Builder.RemoveParts(BodyTypes.BodyHairUpperBack);
                                                        builder.Builder.RemoveParts(BodyTypes.BodyHairLowerBack);
                                                        break;
                                                    case BodyTypes.BodyHairUpperBack:
                                                    case BodyTypes.BodyHairLowerBack:
                                                        builder.Builder.RemoveParts(BodyTypes.BodyHairFullBack);
                                                        break;
                                                    case BodyTypes.Earrings:
                                                        builder.Builder.RemoveParts(BodyTypes.LeftEarring);
                                                        builder.Builder.RemoveParts(BodyTypes.RightEarring);
                                                        break;
                                                    case BodyTypes.LeftEarring:
                                                    case BodyTypes.RightEarring:
                                                        builder.Builder.RemoveParts(BodyTypes.Earrings);
                                                        break;
                                                }

                                                replacementLog.Append(Common.NewLine + " Parts Removed: " + newPart.mPart.BodyType);

                                                builder.Builder.RemoveParts(newPart.mPart.BodyType);

                                                replacementLog.Append(Common.NewLine + " Parts Added: " + newPart.mPart.Key);

                                                builder.ApplyPartPreset(newPart);

                                                OutfitUtils.AdjustPresetForHairColor(builder.Builder, newPart.mPart, mSim);

                                                changed = true;
                                            }
                                        }
                                        else if (allowEmpty)
                                        {
                                            bool found = false;
                                            foreach (CASPart part in builder.Outfit.Parts)
                                            {
                                                if (part.BodyType == location)
                                                {
                                                    found = true;
                                                    break;
                                                }
                                            }

                                            if (found)
                                            {
                                                replacementLog.Append(Common.NewLine + " Parts Removed: " + location);

                                                builder.Builder.RemoveParts(location);
                                                replaced++;

                                                changed = true;
                                            }
                                        }
                                    }
                                }

                                if (!changed)
                                {
                                    builder.Invalidate();
                                }
                            }
                        }
                    }

                    if ((replaced + accessoriesAdded + bodyHairAdded + makeupAdded) > 0)
                    {
                        if (mSim.CreatedSim != null)
                        {
                            mSim.CreatedSim.UpdateOutfitInfo();

                            mSim.CreatedSim.RefreshCurrentOutfit(false);
                        }

                        SimOutfit currentOutfit = mSim.GetOutfit(OutfitCategories.Everyday, 0);
                        if (currentOutfit != null)
                        {
                            mSim.mDefaultOutfitKey = currentOutfit.Key;

                            ThumbnailManager.GenerateHouseholdSimThumbnail(currentOutfit.Key, currentOutfit.Key.InstanceId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, mSim.AgeGenderSpecies);
                        }

                        if (((mOptions & ProcessOptions.WriteLog) == ProcessOptions.WriteLog) ||
                            (((Common.kDebugging) || (mSim.LotHome != null)) && (Dresser.Settings.mNotifyOnCheckOutfits)))
                        {
                            Common.Notify(mSim, Common.Localize("CheckOutfit:Success", mSim.IsFemale, new object[] { mSim, replaced, accessoriesAdded, bodyHairAdded, makeupAdded }));
                        }
                    }
                }
                else
                {
                    replacementLog.Append(Common.NewLine + " No Change");
                }

                Dresser.Settings.AddTested(mSim, testResults | mOptions);
            }
            catch (Exception e)
            {
                Common.Exception(mSim, null, replacementLog.ToString(), e);
            }
            finally
            {
                if (((mOptions & ProcessOptions.WriteLog) == ProcessOptions.WriteLog) || ((Common.kDebugging) && (Dresser.Settings.mNotifyOnCheckOutfits)))
                {
                    Common.WriteLog(replacementLog.ToString());
                }

                Controller.SetToCompleted();
            }
        }

        public class Controller : Common.IWorldLoadFinished, Common.IWorldQuit
        {
            public class QueueItem
            {
                public readonly SimDescription mSim;
                
                public ProcessOptions mOptions;

                public QueueItem(SimDescription sim, ProcessOptions options)
                {
                    mSim = sim;
                    mOptions = options;
                }
            }

            static Queue<QueueItem> sQueue = new Queue<QueueItem>();
            static bool sProcessing = false;

            static Type sBinCommon = typeof(BinCommon);

            public void OnWorldLoadFinished()
            {
                sQueue.Clear();

                new Common.DelayedEventListener(EventTypeId.kSimAgeTransition, OnAgeUp);
                new Common.DelayedEventListener(EventTypeId.kSimInstantiated, OnInstantiated);
                new Common.DelayedEventListener(EventTypeId.kGotBuff, OnGotBuff);
            }

            public void OnWorldQuit()
            {
                sQueue.Clear();
            }

            public static void OnInstantiated(Event e)
            {
                if (GameStates.IsTravelling) return;

                Sim sim = e.TargetObject as Sim;
                if (sim != null)
                {
                    if (!Dresser.Settings.mCheckOutfitsOnReset && Dresser.sSimsReset.Contains(sim.SimDescription.SimDescriptionId))
                    {
                        return;
                    }

                    if (Dresser.Settings.ShouldCheck(sim.SimDescription))
                    {
                        AddToQueue(sim.SimDescription, ProcessOptions.Invalid | ProcessOptions.Accessories | ProcessOptions.Makeup | ProcessOptions.BodyHair | ProcessOptions.Beard);
                    }
                }
            }

            public static void OnAgeUp(Event e)
            {
                SimDescriptionEvent simEvent = e as SimDescriptionEvent;
                if ((simEvent == null) || (simEvent.SimDescription == null)) return;

                if (Dresser.Settings.ShouldCheck(simEvent.SimDescription))
                {
                    ProcessOptions options = ProcessOptions.Invalid;
                    if (simEvent.SimDescription.Adult)
                    {
                        if (Dresser.Settings.mFullAdultReroll)
                        {
                            options |= ProcessOptions.Reroll;
                        }

                        if (Dresser.Settings.mAllowRollAgeupHair)
                        {
                            options |= ProcessOptions.Hair;
                        }

                        if (Dresser.Settings.mAllowRollAgeupMakeup)
                        {
                            options |= ProcessOptions.Makeup;
                        }

                        if (Dresser.Settings.mAllowRollAgeupBodyHair)
                        {
                            options |= ProcessOptions.BodyHair | ProcessOptions.Beard;
                        }

                        if (Dresser.Settings.mAllowRollAgeupAccessories)
                        {
                            options |= ProcessOptions.Accessories;
                        }
                    }
                    else
                    {
                        if (Dresser.Settings.mAllowRollAgeupHairGeneral)
                        {
                            options |= ProcessOptions.Hair;
                        }

                        if (Dresser.Settings.mAllowRollAgeupBodyHairGeneral)
                        {
                            options |= ProcessOptions.BodyHair;
                        }

                        options |= ProcessOptions.Makeup | ProcessOptions.Beard | ProcessOptions.Accessories;
                    }

                    Dresser.Settings.RemoveTested(simEvent.SimDescription, options);

                    AddToQueue(simEvent.SimDescription, options);
                }
            }

            public static void PerformCheck(SimDescription sim)
            {
                AddToQueue(sim, ProcessOptions.Invalid | ProcessOptions.Accessories | ProcessOptions.Makeup);
            }

            public static void OnGotBuff(Event e)
            {
                HasGuidEvent<BuffNames> buffEvent = e as HasGuidEvent<BuffNames>;
                if ((buffEvent != null) && (buffEvent.Guid == BuffNames.Pregnant))
                {
                    Sim sim = e.TargetObject as Sim;
                    if (sim != null)
                    {
                        if (Dresser.Settings.ShouldCheck(sim.SimDescription))
                        {
                            PerformCheck(sim.SimDescription);
                        }
                    }
                }
            }

            public static void AddToQueue(SimDescription sim, ProcessOptions options)
            {
                bool found = false;
                foreach (QueueItem item in sQueue)
                {
                    if (item.mSim == sim)
                    {
                        item.mOptions |= options;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    sQueue.Enqueue(new QueueItem (sim, options));
                }

                ProcessQueue();
            }

            public static void SetToCompleted()
            {
                sProcessing = false;
                ProcessQueue();
            }

            protected static void ProcessQueue()
            {
                if (sProcessing) return;

                if (sQueue.Count == 0) return;

                QueueItem item = sQueue.Dequeue();

                sProcessing = true;
                new CheckOutfitTask(item.mSim, item.mOptions).AddToSimulator();
            }
        }
    }
}


