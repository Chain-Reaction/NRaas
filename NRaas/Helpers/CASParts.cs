using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class CASParts
    {
        static List<BodyTypes> sAllTypes = new List<BodyTypes>();

        static List<BodyTypes> sBodyHairTypes = null;
        static List<BodyTypes> sGeneticBodyTypes = null;

        public static Key sPrimary = new Key(OutfitCategories.Everyday, 0);

        public static readonly List<BodyTypes> sAccessories = new List<BodyTypes>(new BodyTypes[] { BodyTypes.Accessories, BodyTypes.Armband, BodyTypes.Bracelet, BodyTypes.Glasses, BodyTypes.Gloves, BodyTypes.LeftEarring, BodyTypes.LeftGarter, BodyTypes.Necklace, BodyTypes.NoseRing, BodyTypes.RightEarring, BodyTypes.RightGarter, BodyTypes.Socks, BodyTypes.PetBreastCollar, BodyTypes.PetBridle, BodyTypes.PetBlanket, BodyTypes.Earrings });

        public static readonly List<BodyTypes> sMakeup = new List<BodyTypes>(new BodyTypes[] { BodyTypes.Blush, BodyTypes.CostumeMakeup, BodyTypes.EyeLiner, BodyTypes.EyeShadow, BodyTypes.Lipstick, BodyTypes.Mascara });

        static CASParts()
        {
            foreach (BodyTypes type in Enum.GetValues(typeof(BodyTypes)))
            {
                switch (type)
                {
                    case BodyTypes.Last:
                    case BodyTypes.None:
                    case BodyTypes.TattooTemplate:
                        break;
                }

                sAllTypes.Add(type);
            }
        }

        public static ICollection<BodyTypes> AllTypes
        {
            get
            {
                return sAllTypes;
            }
        }

        public static ICollection<BodyTypes> BodyHairTypes
        {
            get
            {
                if (sBodyHairTypes == null)
                {
                    sBodyHairTypes = new List<BodyTypes>(Sim.kHairBodyTypes);
                }
                return sBodyHairTypes;
            }
        }

        public static ICollection<BodyTypes> GeneticBodyTypes
        {
            get
            {
                if (sGeneticBodyTypes == null)
                {
                    sGeneticBodyTypes = new List<BodyTypes>();
                    sGeneticBodyTypes.AddRange(BodyHairTypes);
                    sGeneticBodyTypes.Add(BodyTypes.BasePeltLayer);
                    sGeneticBodyTypes.Add(BodyTypes.BirthMark);
                    sGeneticBodyTypes.Add(BodyTypes.Freckles);
                    sGeneticBodyTypes.Add(BodyTypes.EyeColor);                    
                    sGeneticBodyTypes.Add(BodyTypes.Moles);
                    sGeneticBodyTypes.Add(BodyTypes.PeltLayer);
                    sGeneticBodyTypes.Add(BodyTypes.PetBody);
                    sGeneticBodyTypes.Add(BodyTypes.PetEars);
                    sGeneticBodyTypes.Add(BodyTypes.PetHorn);
                    sGeneticBodyTypes.Add(BodyTypes.PetHooves);
                    sGeneticBodyTypes.Add(BodyTypes.PetMane);
                    sGeneticBodyTypes.Add(BodyTypes.PetTail);
                }
                return sGeneticBodyTypes;
            }
        }

        public delegate bool TestPart(CASParts.Wrapper part);

        public static bool IsMakeup(BodyTypes type)
        {
            return sMakeup.Contains(type);
        }

        public static List<CASParts.Wrapper> GetParts(TestPart onTest)
        {
            List<CASParts.Wrapper> results = new List<CASParts.Wrapper>();

            PartSearch search = new PartSearch();
            foreach (CASPart part in search)
            {
                CASParts.Wrapper newPart = new CASParts.Wrapper(part);

                if ((onTest != null) && (!onTest(newPart))) continue;

                results.Add(newPart);
            }

            search.Reset();

            return results;
        }

        public static string PartToString(CASPart part)
        {
            Common.StringBuilder msg = new Common.StringBuilder();

            msg += Common.NewLine + "Body Type: " + part.BodyType;
            msg += Common.NewLine + "  Instance: 0x" + part.Key.InstanceId.ToString("X16");
            msg += Common.NewLine + "  Group: 0x" + part.Key.GroupId.ToString("X8");
            msg += Common.NewLine + "  Ages: " + (part.AgeGenderSpecies & CASAgeGenderFlags.AgeMask);
            msg += Common.NewLine + "  Genders: " + (part.AgeGenderSpecies & CASAgeGenderFlags.GenderMask);
            msg += Common.NewLine + "  Species: " + (part.AgeGenderSpecies & CASAgeGenderFlags.SpeciesMask);
            msg += Common.NewLine + "  Categories: ";

            foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
            {
                if (category == OutfitCategories.None) continue;

                if ((part.CategoryFlags & (uint)category) == (uint)category)
                {
                    msg += category + ",";
                }
            }

            msg += Common.NewLine + "  Extended: ";

            foreach (OutfitCategoriesExtended category in Enum.GetValues(typeof(OutfitCategoriesExtended)))
            {
                if ((part.CategoryFlags & (uint)category) == (uint)category)
                {
                    msg += category + ",";
                }
            }

            msg += Common.NewLine + "  BotPart: " + OutfitUtils.IsBotPart(part);

            return msg.ToString();
        }

        public static string GetOutfitName(SimDescriptionCore sim, OutfitCategories category, bool maternity)
        {
            return GetOutfitName(sim, new Key(category, sim.GetOutfitCount(category)), maternity);
        }
        public static string GetOutfitName(SimDescriptionCore sim, Key key, bool maternity)
        {
            string name = key.ToString() + Simulator.TicksElapsed();

            if (maternity)
            {
                name += "Maternity";
            }

            SimDescription simDesc = sim as SimDescription;
            if (simDesc != null)
            {
                return simDesc.SimDescriptionId.ToString() + name;
            }
            else
            {
                return sim.FullName + name;
            }
        }

        public static ArrayList GetOutfits(SimDescriptionCore sim, OutfitCategories category, bool alternate)
        {
            if (alternate)
            {
                return sim.Outfits[category] as ArrayList;
            }
            else
            {
                return sim.GetCurrentOutfits()[category] as ArrayList;
            }
        }

        public static int GetOutfitCount(SimDescriptionCore sim, OutfitCategories category, bool alternate)
        {
            ArrayList outfits = CASParts.GetOutfits(sim, OutfitCategories.Special, alternate);
            if (outfits == null)
            {
                return 0;
            }
            else
            {
                return outfits.Count;
            }
        }

        public static SimOutfit GetOutfit(SimDescriptionCore sim, Key key, bool alternate)
        {
            if (sim == null) return null;

            ArrayList outfits = GetOutfits(sim, key.mCategory, alternate);
            if (outfits == null) return null;

            int index = key.GetIndex(sim, alternate);
            if (index == -1) return null;

            if (index < outfits.Count)
            {
                return outfits[index] as SimOutfit;
            }
            else
            {
                return null;
            }
        }

        public static int AddOutfit(SimDescriptionCore sim, OutfitCategories category, SimBuilder builder, bool alternate)
        {
            return ReplaceOutfit(sim, new Key(category, -1), builder, alternate);
        }

        public static void RemoveOutfit(SimDescriptionCore sim, Key key, bool alternate)
        {
            bool maternity = ((sim.IsUsingMaternityOutfits) && (!alternate));

            sim.RemoveOutfit(key.mCategory, key.GetIndex(sim, alternate), true, maternity);

            switch (key.mCategory)
            {
                case OutfitCategories.Special:
                    if (sim.mSpecialOutfitIndices != null)
                    {
                        int index = key.GetIndex(sim, alternate);

                        sim.RemoveSpecialOutfitAtIndex(index);

                        foreach (uint num in new List<uint>(sim.mSpecialOutfitIndices.Keys))
                        {
                            int oldIndex = sim.mSpecialOutfitIndices[num];
                            if (oldIndex > index)
                            {
                                sim.mSpecialOutfitIndices[num] = oldIndex - 1;
                            }
                        }
                    }
                    break;
                case OutfitCategories.Career:
                    CheckIndex(sim, null);
                    break;
                case OutfitCategories.Everyday:
                    SimDescription simDesc = sim as SimDescription;
                    if (simDesc != null)
                    {
                        simDesc.mDefaultOutfitKey = sim.GetOutfit(OutfitCategories.Everyday, 0).Key;
                    }
                    break;
            }            
        }

        public delegate void Logger(string msg);

        public static void CheckIndex(SimDescriptionCore sim, Logger log)
        {
            int count = sim.GetOutfitCount(OutfitCategories.Career);
            if (count <= 0)
            {
                count = 1;
            }

            if ((sim.CareerOutfitIndex < 0) || (sim.CareerOutfitIndex >= count))
            {
                sim.CareerOutfitIndex = count - 1;

                if (log != null)
                {
                    log(" Index Reset: " + sim.FullName);
                }
            }
        }

        public static void RemoveOutfits(SimDescriptionCore sim, OutfitCategories category, bool alternate)
        {
            for (int i = GetOutfitCount(sim, category, alternate)-1; i >= 0; i--)
            {
                RemoveOutfit(sim, new Key(category, i), alternate);
            }
        }

        public static SimOutfit CreateOutfit(SimDescriptionCore sim, Key key, SimBuilder builder, ulong components, bool alternate)
        {
            builder.UseCompression = true;

            bool maternity = ((sim.IsUsingMaternityOutfits) && (!alternate));

            string outfitName = null;

            if ((key.mCategory == OutfitCategories.Special) || (key.GetIndex() != -1))
            {
                outfitName = GetOutfitName(sim, key, maternity);
            }
            else
            {
                outfitName = GetOutfitName(sim, key.mCategory, maternity);
            }

            return new SimOutfit(builder.CacheOutfit(outfitName, components, false));
        }

        public static int ReplaceOutfit(SimDescriptionCore sim, Key key, SimBuilder builder, bool alternate)
        {
            return ReplaceOutfit(sim, key, builder, ulong.MaxValue, alternate);
        }
        public static int ReplaceOutfit(SimDescriptionCore sim, Key key, SimBuilder builder, ulong components, bool alternate)
        {
            return ReplaceOutfit(sim, key, CreateOutfit(sim, key, builder, components, alternate), alternate);
        }
        public static int ReplaceOutfit(SimDescriptionCore sim, Key key, SimOutfit newOutfit, bool alternate)
        {
            bool maternity = ((sim.IsUsingMaternityOutfits) && (!alternate));

            ArrayList outfits = GetOutfits(sim, key.mCategory, alternate);
            if (outfits == null)
            {
                outfits = new ArrayList();
                if (maternity)
                {
                    sim.mMaternityOutfits[key.mCategory] = outfits;
                }
                else
                {
                    sim.Outfits[key.mCategory] = outfits;
                }
            }

            int index = key.GetIndex(sim, alternate);

            if ((index == -1) || (index >= outfits.Count))
            {
                outfits.Add(newOutfit);

                index = outfits.Count - 1;
            }
            else if (index < outfits.Count)
            {
                SimOutfit oldOutfit = outfits[index] as SimOutfit;
                if (oldOutfit != null)
                {
                    bool inUse = false;
                    foreach (OutfitCategories categories in sim.GetCurrentOutfits().Keys)
                    {
                        ArrayList list2 = sim.GetOutfits(categories);
                        if (list2 != null)
                        {
                            foreach (SimOutfit outfit in list2)
                            {
                                if (outfit.Key == oldOutfit.Key)
                                {
                                    inUse = true;
                                    break;
                                }
                            }
                        }

                        if (inUse) break;
                    }

                    if (!inUse)
                    {
                        oldOutfit.Uncache();
                    }
                }

                outfits[index] = newOutfit;
            }
            else
            {
                outfits.Insert(index, newOutfit);
            }

            switch (key.mCategory)
            {
                case OutfitCategories.Special:
                    if (!string.IsNullOrEmpty(key.mSpecialKey))
                    {
                        if (sim.mSpecialOutfitIndices == null)
                        {
                            sim.mSpecialOutfitIndices = new Dictionary<uint, int>();
                        }

                        sim.mSpecialOutfitIndices[ResourceUtils.HashString32(key.mSpecialKey)] = index;
                    }
                    break;
                case OutfitCategories.Everyday:
                    SimDescription simDesc = sim as SimDescription;
                    if (simDesc != null)
                    {
                        simDesc.mDefaultOutfitKey = sim.GetOutfit(OutfitCategories.Everyday, 0).Key;
                    }
                    break;
            }

            return index;
        }

        public class PartPreset : CASPartPreset
        {
            public PartPreset(CASPart part)
            {
                mPart = part;
                mPresetId = uint.MaxValue;

                ResourceKey resKey = new ResourceKey(part.Key.InstanceId, 0x333406c, part.Key.GroupId);
                mPresetString = Simulator.LoadXMLString(resKey);
            }
            public PartPreset(CASPart part, string preset)
                : base(part, preset)
            { }
            public PartPreset(CASPart part, SimOutfit sourceOutfit)
                : base(part, sourceOutfit.GetPartPreset(part.Key))
            { }
            public PartPreset(CASPart part, uint index)
                : base(part, CASUtils.PartDataGetPresetId(part.Key, index), CASUtils.PartDataGetPreset(part.Key, index))
            { }

            public bool Apply(SimBuilder builder)
            {
                if (builder.AddPart(mPart))
                {
                    if (!string.IsNullOrEmpty(mPresetString))
                    {
                        OutfitUtils.ApplyPresetStringToPart(builder, mPart, mPresetString);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [Persistable]
        public class Key
        {
            public readonly OutfitCategories mCategory;
            
            readonly int mIndex;

            public readonly string mSpecialKey;

            protected Key()
            {
                mCategory = OutfitCategories.None;
                mIndex = -1;
                mSpecialKey = null;
            }
            public Key(OutfitCategories category, int index)
            {
                mCategory = category;
                mIndex = index;
                mSpecialKey = null;
            }
            public Key(OutfitCategories category, SimDescriptionCore sim)
            {
                mCategory = category;
                mIndex = sim.GetOutfitCount(category);
                mSpecialKey = null;
            }
            public Key(Sim sim)
            {
                mCategory = OutfitCategories.None;
                mIndex = 0;
                mSpecialKey = null;

                if (sim != null)
                {
                    try
                    {
                        mCategory = sim.CurrentOutfitCategory;
                        mIndex = sim.CurrentOutfitIndex;
                    }
                    catch
                    { }
                }
            }
            public Key(string specialKey)
            {
                mCategory = OutfitCategories.Special;
                mIndex = -1;
                mSpecialKey = specialKey;
            }

            public override bool Equals(object obj)
            {
                Key key = obj as Key;
                if (key == null) return false;

                if (mCategory != key.mCategory) return false;

                if (mIndex != key.mIndex) return false;

                return (mSpecialKey == key.mSpecialKey);
            }

            public string GetLocalizedName(SimDescription sim, bool alternate)
            {
                return Common.LocalizeEAString(sim.IsFemale, "Ui/Caption/ObjectPicker:" + mCategory) + " " + EAText.GetNumberString(GetIndex(sim, alternate) + 1);
            }

            public override int GetHashCode()
            {
                return (int)ResourceUtils.HashString32(mCategory.ToString() + mIndex + mSpecialKey);
            }

            public int GetIndex()
            {
                return mIndex;
            }
            public int GetIndex(SimDescriptionCore sim, bool alternate)
            {
                if ((mCategory == OutfitCategories.Special) && (!string.IsNullOrEmpty(mSpecialKey)))
                {
                    return sim.GetSpecialOutfitIndexFromKey(ResourceUtils.HashString32(mSpecialKey));
                }
                else
                {
                    return mIndex;
                }
            }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(mSpecialKey))
                {
                    return mCategory.ToString() + mIndex;
                }
                else
                {
                    return mSpecialKey;
                }
            }
        }

        public class OutfitBuilder : IDisposable
        {
            SimBuilder mBuilder;

            SimOutfit mOutfit;

            SimDescriptionCore mSim;

            Key mKey;

            bool mAlternate = false;

            ulong mComponents = ulong.MaxValue;

            public OutfitBuilder(SimDescriptionCore sim, Key key, SimOutfit outfit)
                : this(sim, key, outfit, false)
            { }
            public OutfitBuilder(SimDescriptionCore sim, Key key, SimOutfit outfit, bool alternate)
            {
                mBuilder = new SimBuilder();

                mSim = sim;
                mKey = key;
                mAlternate = alternate;

                mOutfit = outfit;
                if (mOutfit != null)
                {
                    OutfitUtils.SetOutfit(mBuilder, mOutfit, sim);
                }
            }
            public OutfitBuilder(SimDescriptionCore sim, Key key)
                : this(sim, key, false)
            { }
            public OutfitBuilder(SimDescriptionCore sim, Key key, bool alternate)
                : this(sim, key, GetOutfit(sim, key, alternate), alternate)
            { }

            public SimBuilder Builder
            {
                get
                {
                    return mBuilder;
                }
            }

            public ulong Components
            {
                set { mComponents = value; }
            }

            public SimOutfit Outfit
            {
                get
                {
                    return mOutfit;
                }
            }

            public bool OutfitValid
            {
                get
                {
                    return (mOutfit != null);
                }
            }

            public void Invalidate()
            {
                mOutfit = null;
            }

            public void CopyGeneticParts(SimOutfit sourceOutfit)
            {
                CopyGeneticParts(mBuilder, sourceOutfit);
            }
            public static void CopyGeneticParts(SimBuilder builder, SimOutfit sourceOutfit)
            {
                builder.RemoveParts(new List<BodyTypes>(CASParts.GeneticBodyTypes).ToArray());

                foreach (CASPart part in sourceOutfit.Parts)
                {
                    if (!CASParts.GeneticBodyTypes.Contains(part.BodyType)) continue;

                    if ((part.Age & builder.Age) != builder.Age) continue;

                    if ((part.Gender & builder.Gender) != builder.Gender) continue;

                    new PartPreset(part, sourceOutfit).Apply(builder);
                }

                builder.SetSecondaryNormalMapWeights(sourceOutfit.SecondaryNormalMapWeights);

                builder.FurMap = sourceOutfit.FurMap;
                builder.NumCurls = sourceOutfit.NumCurls;
                builder.CurlPixelRadius = sourceOutfit.CurlPixelRadius;
            }

            public static PartPreset GetPartPreset(CASPart part, SimOutfit sourceOutfit)
            {
                return new PartPreset(part, sourceOutfit.GetPartPreset(part.Key));
            }
            public static bool GetPartPreset(BodyTypes type, SimOutfit sourceOutfit, ref PartPreset result)
            {
                foreach (CASPart part in sourceOutfit.Parts)
                {
                    if (part.BodyType != type) continue;

                    result = new PartPreset(part, sourceOutfit.GetPartPreset(part.Key));
                    return true;
                }

                return false;
            }

            public bool ApplyPartPreset(CASPart part, SimOutfit sourceOutfit)
            {
                return new PartPreset(part, sourceOutfit).Apply(mBuilder);
            }
            public bool ApplyPartPreset(PartPreset part)
            {
                return part.Apply(mBuilder);
            }

            public void Dispose()
            {
                if (mOutfit != null)
                {
                    int index = ReplaceOutfit(mSim, mKey, mBuilder, mComponents, mAlternate);

                    mOutfit = mSim.GetOutfit(mKey.mCategory, index);

                    SimDescription sim = mSim as SimDescription;
                    if ((sim != null) && (sim.CreatedSim != null))
                    {
                        try
                        {
                            sim.CreatedSim.RefreshCurrentOutfit(false);
                        }
                        catch (Exception e)
                        {
                            Common.DebugException(sim, e);
                        }
                    }
                }

                mBuilder.Dispose();

                SpeedTrap.Sleep();
            }
        }

        public class Wrapper
        {
            public CASPart mPart;

            ResourceKeyContentCategory mType;

            public Wrapper(CASPart part)
            {
                mPart = part;

                mType = UIUtils.GetCustomContentType(mPart.Key);
            }

            public CASAgeGenderFlags Age
            {
                get { return mPart.Age; }
            }

            public CASAgeGenderFlags Gender
            {
                get { return mPart.Gender; }
            }

            public CASAgeGenderFlags Species
            {
                get { return mPart.Species; }
            }

            public CASAgeGenderFlags AgeGenderSpecies
            {
                get { return mPart.AgeGenderSpecies; }
            }

            public BodyTypes BodyType
            {
                get { return mPart.BodyType; }
            }

            public OutfitCategories Category
            {
                get { return (OutfitCategories)mPart.CategoryFlags; }
            }

            public OutfitCategoriesExtended ExtendedCategory
            {
                get { return (OutfitCategoriesExtended)mPart.CategoryFlags; }
            }

            public ResourceKey Key
            {
                get { return mPart.Key; }
            }

            public bool ValidFor(SimDescription sim)
            {
                if ((mPart.Age & sim.Age) != sim.Age) return false;

                if ((mPart.Gender & sim.Gender) != sim.Gender) return false;

                if (mPart.Species != sim.Species) return false;

                return true;
            }

            public ProductVersion GetVersion()
            {
                return UIUtils.ExtractProductVersion(UIUtils.GetCustomContentType(Key));
            }

            public List<CASParts.PartPreset> GetPresets()
            {
                List<CASParts.PartPreset> presets = new List<CASParts.PartPreset>();

                CASParts.PartPreset preset = new CASParts.PartPreset(mPart);
                if (preset.Valid)
                {
                    presets.Add(preset);
                }

                for (uint index = 0; index < CASUtils.PartDataNumPresets(mPart.Key); index++)
                {
                    preset = new CASParts.PartPreset(mPart, index);
                    if (preset.Valid)
                    {
                        presets.Add(preset);
                    }
                }

                return presets;
            }
            public CASParts.PartPreset GetRandomPreset()
            {
                List<CASParts.PartPreset> presets = GetPresets();
                if (presets.Count > 0)
                {
                    return RandomUtil.GetRandomObjectFromList(presets);
                }
                else
                {
                    return null;
                }
            }

            public static List<Wrapper> CreateList(IEnumerable<CASPart> parts)
            {
                List<Wrapper> results = new List<Wrapper>();

                foreach (CASPart part in parts)
                {
                    results.Add(new Wrapper(part));
                }

                return results;
            }

            public int CompareToCustomBottom(Wrapper wrapper)
            {
                ProductVersion left = UIUtils.ExtractProductVersion(mType);
                ProductVersion right = UIUtils.ExtractProductVersion(wrapper.mType);

                int result = -left.CompareTo(right);
                if (result != 0) return result;

                result = -UIUtils.IsInstalledContent(mType).CompareTo(UIUtils.IsInstalledContent(wrapper.mType));
                if (result != 0) return result;

                result = UIUtils.IsCustomFiltered(mType).CompareTo(UIUtils.IsCustomFiltered(wrapper.mType));
                if (result != 0) return result;

                return mPart.DisplayIndex.CompareTo(wrapper.mPart.DisplayIndex);
            }

            public int CompareTo(Wrapper wrapper)
            {
                int result = -UIUtils.IsCustomFiltered(mType).CompareTo(UIUtils.IsCustomFiltered(wrapper.mType));
                if (result != 0) return result;

                result = UIUtils.IsInstalledContent(mType).CompareTo(UIUtils.IsInstalledContent(wrapper.mType));
                if (result != 0) return result;

                ProductVersion left = UIUtils.ExtractProductVersion(mType);
                ProductVersion right = UIUtils.ExtractProductVersion(wrapper.mType);

                result = -left.CompareTo(right);
                if (result != 0) return result;

                return mPart.DisplayIndex.CompareTo(wrapper.mPart.DisplayIndex);
            }

            public override bool Equals(object o)
            {
                Wrapper wrapper = o as Wrapper;

                return mPart.Equals(wrapper.mPart);
            }

            public override int GetHashCode()
            {
                return mPart.GetHashCode();
            }

            public override string ToString()
            {
                return PartToString(mPart);
            }
        }
    }
}

