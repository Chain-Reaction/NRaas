using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    [Persistable]
    public class SavedOutfit
    {
        List<SavedPart> mParts = new List<SavedPart>();

        ColorStore mStore;

        protected SavedOutfit() // Persistable
        { }
        public SavedOutfit(SimOutfit outfit)
        {
            if (outfit != null)
            {
                foreach (CASPart oldPart in outfit.Parts)
                {
                    string oldPreset = outfit.GetPartPreset(oldPart.Key);

                    mParts.Add(new SavedPart(oldPart, oldPreset));
                }
            }

            mStore = new ColorStore(this);
        }
        public SavedOutfit(SimBuilder builder)
        {
            foreach (CASPart oldPart in builder.mCASParts.Values)
            {
                string oldPreset = builder.GetDesignPresetString(oldPart);

                mParts.Add(new SavedPart(oldPart, oldPreset));
            }

            mStore = new ColorStore(this);
        }

        public bool HasParts
        {
            get { return (mParts.Count > 0); }
        }

        public IEnumerable<BodyTypes> UsedSingleBodyTypes
        {
            get
            {
                List<BodyTypes> result = new List<BodyTypes>();

                foreach(SavedPart part in mParts)
                {
                    if (!CASLogic.BodyTypeCanHaveMultiples(part.mPart.BodyType))
                    {
                        result.Add(part.mPart.BodyType);
                    }
                }

                return result;
            }
        }

        public List<SavedPart> GetParts(List<BodyTypes> parts)
        {
            List<SavedPart> results = new List<SavedPart>();

            foreach(SavedPart part in mParts)
            {
                if (parts.Contains(part.mPart.BodyType))
                {
                    results.Add(part);
                }
            }

            return results;
        }

        /*
        public void DebugLogColor()
        {
            if (!Common.kDebugging) return;

            Common.StringBuilder msg = new Common.StringBuilder(null;

            msg += "Hair";

            Color[] color = GetPresetColor(BodyTypes.Hair);
            if (color != null)
            {
                foreach (Color item in color)
                {
                    msg += Common.NewLine + item;
                }
            }

            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                msg += Common.NewLine + e.StackTrace;
            }

            Common.WriteLog(msg);
        }
        */

        public string GetPreset(BodyTypes type)
        {
            foreach (SavedPart part in mParts)
            {
                if (part.mPart.BodyType == type)
                {
                    if (!string.IsNullOrEmpty(part.mPreset))
                    {
                        return part.mPreset;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        public void SetPreset(BodyTypes type, string preset)
        {
            foreach (SavedPart part in mParts)
            {
                if (part.mPart.BodyType == type)
                {
                    part.mPreset = preset;
                }
            }
        }

        /*
        public void Apply(SimBuilder builder, bool applyHairColor)
        {
            List<BodyTypes> types = new List<BodyTypes>();

            foreach (SavedPart part in mParts)
            {
                if (types.Contains(part.mPart.BodyType)) continue;

                types.Add(part.mPart.BodyType);
            }

            Apply(builder, applyHairColor, types, null);
        }
        */
        public void Apply(CASParts.OutfitBuilder builder, bool applyHairColor)
        {
            Apply(builder, applyHairColor, UsedSingleBodyTypes, null);
        }
        public void Apply(CASParts.OutfitBuilder builder, bool applyHairColor, IEnumerable<BodyTypes> argTypes, IEnumerable<BodyTypes> argNotTypes)
        {
            Apply(builder.Builder, applyHairColor, argTypes, argNotTypes);
        }
        public void Apply(SimBuilder builder, bool applyHairColor, IEnumerable<BodyTypes> argTypes, IEnumerable<BodyTypes> argNotTypes)
        {
            List<BodyTypes> types = null;
            if (argTypes != null)
            {
                types = new List<BodyTypes>(argTypes);
            }

            List<BodyTypes> notTypes = null;
            if (argNotTypes != null)
            {
                notTypes = new List<BodyTypes>(argNotTypes);
            }

            if ((types == null) || (types.Count == 0))
            {
                types = null;
            }
            else if (notTypes != null)
            {
                foreach(BodyTypes type in types)
                {
                    notTypes.Remove(type);
                }
            }

            ColorStore store = mStore;
            if (!applyHairColor)
            {
                store = new ColorStore(builder);
            }

            if (types != null)
            {
                if ((types.Contains(BodyTypes.UpperBody)) || (types.Contains(BodyTypes.LowerBody)))
                {
                    builder.RemoveParts(new BodyTypes[] { BodyTypes.FullBody });
                }

                if (types.Contains(BodyTypes.FullBody))
                {
                    builder.RemoveParts(new BodyTypes[] { BodyTypes.UpperBody, BodyTypes.LowerBody });
                }

                builder.RemoveParts(types.ToArray());
            }
            else 
            {
                List<BodyTypes> allTypes = new List<BodyTypes>(CASParts.AllTypes);

                if (notTypes != null)
                {
                    foreach(BodyTypes type in notTypes)
                    {
                        allTypes.Remove(type);
                    }
                }

                builder.RemoveParts(allTypes.ToArray());
            }

            foreach (SavedPart part in mParts)
            {
                if (types != null)
                {
                    if (!types.Contains(part.mPart.BodyType)) continue;
                }

                if (notTypes != null)
                {
                    if (notTypes.Contains(part.mPart.BodyType)) continue;
                }

                builder.AddPart(part.mPart);

                if ((part.mPreset != null) && CASUtils.ApplyPresetToPart(builder, part.mPart, part.mPreset))
                {
                    builder.SetPartPreset(part.mPart.Key, null, part.mPreset);
                }
            }

            store.Apply(builder);
        }

        public void Replace(List<SavedPart> parts)
        {
            List<BodyTypes> types = new List<BodyTypes>();

            foreach (SavedPart part in parts)
            {
                if (types.Contains(part.mPart.BodyType)) continue;
                types.Add(part.mPart.BodyType);
            }

            for (int i = mParts.Count - 1; i >= 0; i--)
            {
                if (types.Contains(mParts[i].mPart.BodyType))
                {
                    mParts.RemoveAt(i);
                }
            }

            mParts.AddRange(parts);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (SavedPart part in mParts)
            {
                result.Append(Common.NewLine + part.ToString());
            }

            return result.ToString();
        }

        [Persistable]
        public class SavedPart
        {
            public readonly CASPart mPart;
            public string mPreset;

            protected SavedPart() // Persistable
            { }
            public SavedPart(CASPart part, string preset)
            {
                mPart = part;
                mPreset = preset;
            }

            public override string ToString()
            {
                return CASParts.PartToString(mPart);
            }
        }

        public class Cache
        {
            Dictionary<OutfitCategories, Dictionary<int, SavedOutfit>> mOutfits = new Dictionary<OutfitCategories, Dictionary<int, SavedOutfit>>();

            Cache mAltOutfits;

            public Cache()
            { }
            public Cache(SimDescriptionCore sim)
                : this(sim, false)
            { }
            private Cache(SimDescriptionCore sim, bool alternate)
            {
                if ((alternate) && (!sim.IsUsingMaternityOutfits)) return;

                foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                {
                    if (category == OutfitCategories.Supernatural) continue;

                    ArrayList outfits = CASParts.GetOutfits(sim, category, alternate);

                    if (outfits != null)
                    {
                        for (int i = 0x0; i < outfits.Count; i++)
                        {
                            SimOutfit outfit = outfits[i] as SimOutfit;
                            if (outfit == null) continue;

                            Store(new CASParts.Key(category, i), new SavedOutfit(outfit));
                        }
                    }
                }

                if (!alternate)
                {
                    mAltOutfits = new Cache(sim, true);
                }
            }

            public static int CountOutfits(SimDescriptionCore sim, bool alternate)
            {
                int count = 0;

                foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                {
                    if (category == OutfitCategories.Supernatural) continue;

                    count += sim.GetOutfitCount(category);
                }

                return count;
            }

            public List<Key> AltOutfits
            {
                get
                {
                    if (mAltOutfits == null)
                    {
                        return new List<Key>();
                    }
                    else
                    {
                        return mAltOutfits.Outfits;
                    }
                }
            }

            public int GetOutfitCount(OutfitCategories category, bool alternate)
            {
                Dictionary<int, SavedOutfit> outfits;

                if (alternate)
                {
                    if (mAltOutfits == null) return 0;

                    if (!mAltOutfits.mOutfits.TryGetValue(category, out outfits)) return 0;
                }
                else
                {
                    if (!mOutfits.TryGetValue(category, out outfits)) return 0;
                }

                int maximum = 0;
                foreach (KeyValuePair<int, SavedOutfit> index in outfits)
                {
                    if (maximum < (index.Key + 1))
                    {
                        maximum = index.Key + 1;
                    }
                }

                return maximum;
            }

            public List<Key> Outfits
            {
                get
                {
                    List<Key> results = new List<Key>();

                    foreach(KeyValuePair<OutfitCategories,Dictionary<int, SavedOutfit>> category in mOutfits)
                    {
                        foreach (KeyValuePair<int,SavedOutfit> index in category.Value)
                        {
                            results.Add(new Key(new CASParts.Key(category.Key, index.Key), index.Value));
                        }
                    }

                    return results;
                }
            }

            public bool ApplyColors(SimDescriptionCore sim, CASParts.Key key)
            {
                SavedOutfit outfit = Load(key);
                if (outfit == null) return false;

                outfit.mStore.Apply(sim);
                return true;
            }

            /*
            public bool Apply(SimBuilder builder, OutfitCategories category, int index, bool applyHairColor)
            {
                return Apply(builder, category, index, applyHairColor, null, null);
            }
            */
            public bool Apply(CASParts.OutfitBuilder builder, CASParts.Key key, bool applyHairColor, IEnumerable<BodyTypes> types, IEnumerable<BodyTypes> notTypes)
            {
                return Apply(builder.Builder, key, applyHairColor, types, notTypes);
            }
            public bool Apply(SimBuilder builder, CASParts.Key key, bool applyHairColor, IEnumerable<BodyTypes> types, IEnumerable<BodyTypes> notTypes)
            {
                SavedOutfit outfit = Load(key);
                if (outfit == null) return false;

                outfit.Apply(builder, applyHairColor, types, notTypes);
                return true;
            }

            /*
            public bool ApplyAlt(SimBuilder builder, CASParts.Key key, bool applyHairColor)
            {
                return ApplyAlt(builder, key, applyHairColor, null, null);
            }
            */
            public bool ApplyAlt(SimBuilder builder, CASParts.Key key, bool applyHairColor, IEnumerable<BodyTypes> types, IEnumerable<BodyTypes> notTypes)
            {
                if (mAltOutfits == null) return false;

                SavedOutfit outfit = mAltOutfits.Load(key);
                if (outfit == null) return false;

                outfit.Apply(builder, applyHairColor, types, notTypes);
                return true;
            }

            public SavedOutfit Replace(CASParts.Key key, SimBuilder builder, bool applyHairColor)
            {
                if (!applyHairColor)
                {
                    SavedOutfit oldOutfit = Load(key);
                    if (oldOutfit != null)
                    {
                        oldOutfit.mStore.Apply(builder);
                    }
                }

                return Store(key, new SavedOutfit(builder));
            }

            public SavedOutfit Load(CASParts.Key key)
            {
                Dictionary<int, SavedOutfit> indices;
                if (!mOutfits.TryGetValue(key.mCategory, out indices))
                {
                    return null;
                }

                SavedOutfit outfit;
                if (!indices.TryGetValue(key.GetIndex(), out outfit))
                {
                    return null;
                }

                return outfit;
            }

            public SavedOutfit Store(CASParts.Key key, SavedOutfit outfit)
            {
                Dictionary<int, SavedOutfit> indices;
                if (!mOutfits.TryGetValue(key.mCategory, out indices))
                {
                    indices = new Dictionary<int, SavedOutfit>();
                    mOutfits.Add(key.mCategory, indices);
                }

                indices[key.GetIndex()] = outfit;
                return outfit;
            }

            public bool PropagateGenetics(SimDescriptionCore sim, CASParts.Key geneKey)
            {
                SimOutfit origOutfit = CASParts.GetOutfit(sim, geneKey, false);
                if (origOutfit == null) return false;

                foreach (Key outfit in Outfits)
                {
                    if (outfit.mKey == geneKey) continue;

                    using(CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, outfit.mKey, origOutfit))
                    {
                        outfit.Apply(builder, true, null, CASParts.GeneticBodyTypes);
                    }
                }

                if (mAltOutfits != null)
                {
                    foreach (Key outfit in mAltOutfits.Outfits)
                    {
                        if (outfit.mKey == geneKey) continue;

                        using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, outfit.mKey, origOutfit))
                        {
                            outfit.Apply(builder, true, null, CASParts.GeneticBodyTypes);
                        }
                    }
                }

                SimDescription simDesc = sim as SimDescription;
                if ((simDesc != null) && (simDesc.CreatedSim != null))
                {
                    simDesc.CreatedSim.RefreshCurrentOutfit(false);
                }

                SpeedTrap.Sleep();

                return true;
            }

            public override string ToString()
            {
                StringBuilder result = new StringBuilder();

                foreach (Key key in Outfits)
                {
                    result.Append(Common.NewLine + "Key: " + key);
                    result.Append(Common.NewLine + key.mOutfit);
                }

                return result.ToString();
            }

            public class Key
            {
                public readonly CASParts.Key mKey;

                public readonly SavedOutfit mOutfit;

                public Key(CASParts.Key key, SavedOutfit outfit)
                {
                    mKey = key;
                    mOutfit = outfit;
                }

                public void Apply(CASParts.OutfitBuilder builder, bool applyHairColor, IEnumerable<BodyTypes> argTypes, IEnumerable<BodyTypes> argNotTypes)
                {
                    mOutfit.Apply(builder, applyHairColor, argTypes, argNotTypes);
                }
                public void Apply(SimBuilder builder, bool applyHairColor, IEnumerable<BodyTypes> argTypes, IEnumerable<BodyTypes> argNotTypes)
                {
                    mOutfit.Apply(builder, applyHairColor, argTypes, argNotTypes);
                }

                public OutfitCategories Category
                {
                    get { return mKey.mCategory; }
                }

                public int Index
                {
                    get { return mKey.GetIndex(); }
                }

                public override string ToString()
                {
                    return mKey.ToString();
                }
            }
        }

        [Persistable]
        public class ColorStore
        {
            string mHairPreset;
            string mBeardPreset;
            string mEyebrowsPreset;
            string mBodyHairPreset;

            protected ColorStore() // Persistable
            { }
            public ColorStore(SavedOutfit outfit)
            {
                if (outfit != null)
                {
                    mHairPreset = outfit.GetPreset(BodyTypes.Hair);
                    mBeardPreset = outfit.GetPreset(BodyTypes.Beard);
                    mEyebrowsPreset = outfit.GetPreset(BodyTypes.Eyebrows);
                    mBodyHairPreset = outfit.GetPreset(BodyTypes.BodyHairUpperChest);
                }
            }
            public ColorStore(SimBuilder builder)
            {
                if (builder != null)
                {
                    foreach (CASPart part in builder.mCASParts.Values)
                    {
                        switch (part.BodyType)
                        {
                            case BodyTypes.Hair:
                                mHairPreset = builder.GetDesignPresetString(part);
                                break;
                            case BodyTypes.Beard:
                                mBeardPreset = builder.GetDesignPresetString(part);
                                break;
                            case BodyTypes.Eyebrows:
                                mEyebrowsPreset = builder.GetDesignPresetString(part);
                                break;
                            default:
                                if (CASParts.BodyHairTypes.Contains(part.BodyType))
                                {
                                    mBodyHairPreset = builder.GetDesignPresetString(part);
                                }
                                break;
                        }
                    }
                }
            }

            private static Color[] ExtractHairColor(string preset)
            {
                if (string.IsNullOrEmpty(preset)) return null;

                return CASUtils.ExtractHairColor(preset);
            }

            private static Color[] ExtractEyebrowColor(string preset)
            {
                if (string.IsNullOrEmpty(preset)) return null;

                return CASUtils.ExtractEyebrowColor(preset);
            }

            public void Apply(SimDescriptionCore sim)
            {
                Color[] color = ExtractHairColor(mHairPreset);
                if (color != null)
                {
                    sim.ActiveHairColors = color;
                }

                color = ExtractHairColor(mBeardPreset);
                if (color != null)
                {
                    sim.ActiveFacialHairColors = color;
                }

                /*
                color = ExtractEyebrowColor(mEyebrowsPreset);
                if ((color != null) && (color.Length > 0))
                {
                    sim.ActiveEyebrowColor = color[0];
                }

                color = ExtractEyebrowColor(mBodyHairPreset);
                if ((color != null) && (color.Length > 0))
                {
                    sim.ActiveBodyHairColor = color[0];
                }
                */
            }
            public void Apply(SimBuilder builder)
            {
                Color[] color = ExtractHairColor(mHairPreset);
                if (color != null)
                {
                    OutfitUtils.InjectHairColor(builder, color, BodyTypes.Hair);
                }

                color = ExtractHairColor(mBeardPreset);
                if (color != null)
                {
                    OutfitUtils.InjectHairColor(builder, color, BodyTypes.Beard);
                }

                /*
                color = ExtractEyebrowColor(mEyebrowsPreset);
                if ((color != null) && (color.Length > 0))
                {
                    OutfitUtils.InjectEyeBrowHairColor(builder, color[0]);
                }

                color = ExtractEyebrowColor(mBodyHairPreset);
                if ((color != null) && (color.Length > 0))
                {
                    OutfitUtils.InjectBodyHairColor(builder, color[0]);
                }
                */
            }
        }
    }
}
