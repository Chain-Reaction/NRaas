// NOTE THIS MOD WILL NOT COMPILE WITHOUT THE DLL'S FROM THE STORE CONTENT IT ALTERS IN THE SIMS3 / COMPILER DIRECTORY
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to auto restock all display cases in the world")]
        protected static bool kAutoRestock = true;

        [Tunable, TunableComment("Whether to restock lots owned by the active family")]
        protected static bool kAffectActive = false;

        [Tunable, TunableComment("Whether to stock the top of cases with wedding cakes")]
        protected static bool kStockWeddingCakes = true;        

        [Tunable, TunableComment("Whether to disble random restock system")]
        protected static bool kDisableRandomAutoRestock = false;   
     
        [Tunable, TunableComment("Whether to only use the first picked recipe for the entire display when on random selection")]
        protected static bool kOneRecipePerDiplayOnRandom = false;

        public bool mAutoRestock = kAutoRestock;
        public bool mAffectActive = kAffectActive;
        public bool mStockWeddingCakes = kStockWeddingCakes;        
        public bool mDisableRandomAutoRestock = kDisableRandomAutoRestock;
        public bool mOneRecipePerDisplayOnRandom = kOneRecipePerDiplayOnRandom;

        public Dictionary<ObjectGuid, Dictionary<int, Dictionary<string, List<Quality>>>> mDisplayRestockSettings = new Dictionary<ObjectGuid, Dictionary<int, Dictionary<string, List<Quality>>>>();
        public List<ObjectGuid> mExemptDisplays = new List<ObjectGuid>();

        public Dictionary<string, List<Quality>> mRandomRestockSettings = new Dictionary<string, List<Quality>>();

        protected bool mDebugging = false;

        public void ValidateObjects()
        {
            List<ObjectGuid> remove = new List<ObjectGuid>();

            foreach (ObjectGuid guid in mDisplayRestockSettings.Keys)
            {
                if (Simulator.GetProxy(guid) != null) continue;

                remove.Add(guid);
            }

            foreach (ObjectGuid guid in remove)
            {
                mDisplayRestockSettings.Remove(guid);

                if (mExemptDisplays.Contains(guid))
                {
                    mExemptDisplays.Remove(guid);
                }
            }
        }

        public Dictionary<string, List<Quality>> GetDisplaySettingsForSlot(ObjectGuid guid, int i)
        {
            if (HasSettings(guid))
            {
                if (SlotHasSettings(guid, i))
                {
                    return mDisplayRestockSettings[guid][i];
                }
            }

            return null;
        }

        public Dictionary<int, Dictionary<string, List<Quality>>> GetDisplaySettings(ObjectGuid guid)
        {
            if (mDisplayRestockSettings.ContainsKey(guid))
            {
                return mDisplayRestockSettings[guid];
            }

            return null;
        }

        public bool IsDisplayExempt(ObjectGuid guid)
        {
            return mExemptDisplays.Contains(guid);
        }

        public void AddDisplayRecipe(ObjectGuid guid, List<int> slots, string recipeKey, bool autoSetQuality)
        {
            if (!mDisplayRestockSettings.ContainsKey(guid))
            {
                mDisplayRestockSettings.Add(guid, new Dictionary<int, Dictionary<string, List<Quality>>>());               
            }            

            foreach (int i in slots)
            {
                if (!mDisplayRestockSettings[guid].ContainsKey(i))
                {
                    mDisplayRestockSettings[guid].Add(i, new Dictionary<string, List<Quality>>());
                }

                if (!mDisplayRestockSettings[guid][i].ContainsKey(recipeKey))
                {
                    mDisplayRestockSettings[guid][i].Add(recipeKey, new List<Quality>());
                }
                else
                {
                    mDisplayRestockSettings[guid][i].Remove(recipeKey);
                }

                if (autoSetQuality)
                {
                    if (!mDisplayRestockSettings[guid][i][recipeKey].Contains(Quality.Any))
                    {
                        mDisplayRestockSettings[guid][i][recipeKey].Add(Quality.Any);
                    }
                }
            }
        }        

        public void SetRecipeQuality(ObjectGuid guid, List<int> slots, Quality quality, bool clear)
        {
            foreach (int i in slots)
            {
                Dictionary<string, List<Quality>> newSettings = new Dictionary<string, List<Quality>>();

                foreach (KeyValuePair<string, List<Quality>> recipe in mDisplayRestockSettings[guid][i])
                {
                    if (!clear && !newSettings.ContainsKey(recipe.Key))
                    {
                        newSettings.Add(recipe.Key, recipe.Value);
                    }
                    else
                    {
                        newSettings.Add(recipe.Key, new List<Quality>());
                    }

                    if (!mDisplayRestockSettings[guid][i][recipe.Key].Contains(quality))
                    {
                        newSettings[recipe.Key].Add(quality);
                    }
                    else
                    {
                        newSettings[recipe.Key].Remove(quality);
                    }
                }

                mDisplayRestockSettings[guid][i] = newSettings;
            }
        }

        public void RemoveDisplaySettings(ObjectGuid guid)
        {
            if (mDisplayRestockSettings.ContainsKey(guid))
            {
                mDisplayRestockSettings.Remove(guid);
            }            
        }

        public void AddExemptStatus(ObjectGuid guid)
        {
            if (!mExemptDisplays.Contains(guid))
            {
                mExemptDisplays.Remove(guid);
            }
        }

        public void ClearExemptStatus(ObjectGuid guid)
        {
            if (mExemptDisplays.Contains(guid))
            {
                mExemptDisplays.Remove(guid);
            }
        }

        public void RemoveDisplaySettingsForSlot(ObjectGuid guid, int slot)
        {
            if (mDisplayRestockSettings.ContainsKey(guid))
            {
                if (mDisplayRestockSettings[guid].ContainsKey(slot))
                {
                    mDisplayRestockSettings[guid].Remove(slot);
                }
            }
        }

        public void SetDisplaySettings(ObjectGuid guid, Dictionary<int, Dictionary<string, List<Quality>>> settings)
        {
            if (mDisplayRestockSettings.ContainsKey(guid))
            {
                RemoveDisplaySettings(guid);
            }

            mDisplayRestockSettings.Add(guid, settings);
        }

        public bool HasSettings(ObjectGuid guid)
        {
            if (!mDisplayRestockSettings.ContainsKey(guid))
            {
                return false;
            }

            if (mDisplayRestockSettings[guid].Count == 0)
            {
                return false;
            }

            return true;
        }

        public bool SlotHasSettings(ObjectGuid guid, int slot)
        {
            if (!mDisplayRestockSettings.ContainsKey(guid))
            {
                return false;
            }

            if (!mDisplayRestockSettings[guid].ContainsKey(slot))
            {
                return false;
            }

            if (mDisplayRestockSettings[guid][slot].Count == 0)
            {
                return false;
            }

            return true;
        }

        public void SetDisplaySettingsForSlot(ObjectGuid guid, Dictionary<string, List<Quality>> settings, int slot)
        {
            if (!HasSettings(guid))
            {
                mDisplayRestockSettings.Add(guid, new Dictionary<int, Dictionary<string, List<Quality>>>());
            }

            if(SlotHasSettings(guid, slot))
            {
                RemoveDisplaySettingsForSlot(guid, slot);
            }

            mDisplayRestockSettings[guid].Add(slot, settings);
        }

        public void AddRandomRestockRecipe(string recipeKey, List<Quality> qualities)
        {
            if (!mRandomRestockSettings.ContainsKey(recipeKey))
            {
                mRandomRestockSettings.Add(recipeKey, qualities);
            }
            else
            {
                mRandomRestockSettings.Remove(recipeKey);
            }
        }

        public void SetRandomRestockRecipeQuality(string recipeKey, Quality quality)
        {
            if (!mRandomRestockSettings.ContainsKey(recipeKey))
            {
                return;                
            }

            if (!mRandomRestockSettings[recipeKey].Contains(quality))
            {
                mRandomRestockSettings[recipeKey].Add(quality);
            }
            else
            {
                mRandomRestockSettings[recipeKey].Remove(quality);
            }            
        }

        // warning, may return non-unique list
        public List<Quality> RandomRestockQualitiesAsList()
        {
            List<Quality> qualities = new List<Quality>();
            foreach (KeyValuePair<string, List<Quality>> values in mRandomRestockSettings)
            {
                qualities.AddRange(values.Value);                
            }

            return qualities;
        }

        public Dictionary<string, string> BuildSlotsWithRecipes(Dictionary<int, Dictionary<string, List<Quality>>> settings)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            Dictionary<string, List<string>> tempValues = new Dictionary<string, List<string>>();

            if (settings == null)
            {
                return values;
            }

            foreach (KeyValuePair<int, Dictionary<string, List<Quality>>> slotConfig in settings)
            {
                foreach (KeyValuePair<string, List<Quality>> recipeConfig in slotConfig.Value)
                {
                    if (!tempValues.ContainsKey(recipeConfig.Key))
                    {
                        tempValues.Add(recipeConfig.Key, new List<string>());
                    }

                    if (!tempValues[recipeConfig.Key].Contains(slotConfig.Key.ToString()))
                    {
                        tempValues[recipeConfig.Key].Add(slotConfig.Key.ToString());
                    }
                }
            }

            foreach(KeyValuePair<string, List<string>> val in tempValues)
            {
                values.Add(val.Key, string.Join(",", val.Value.ToArray()));
            }

            return values;
        }

        public Dictionary<Quality, string> BuildSlotsWithQualities(Dictionary<int, Dictionary<string, List<Quality>>> settings)
        {
            Dictionary<Quality, string> values = new Dictionary<Quality, string>();
            Dictionary<Quality, List<string>> tempValues = new Dictionary<Quality, List<string>>();

            if (settings == null)
            {
                return values;
            }

            foreach (KeyValuePair<int, Dictionary<string, List<Quality>>> slotConfig in settings)
            {
                foreach (KeyValuePair<string, List<Quality>> recipeConfig in slotConfig.Value)
                {
                    foreach (Quality quality in recipeConfig.Value)
                    {
                        if (!tempValues.ContainsKey(quality))
                        {
                            tempValues.Add(quality, new List<string>());
                        }

                        if (!tempValues[quality].Contains(slotConfig.Key.ToString()))
                        {
                            tempValues[quality].Add(slotConfig.Key.ToString());
                        }
                    }
                }
            }

            foreach (KeyValuePair<Quality, List<string>> val in tempValues)
            {
                values.Add(val.Key, string.Join(",", val.Value.ToArray()));
            }

            return values;
        }

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }
    }
}