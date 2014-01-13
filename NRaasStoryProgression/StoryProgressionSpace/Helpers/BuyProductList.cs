using NRaas.StoryProgressionSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class BuyProductList
    {
        List<BuildBuyProduct> mProducts = new List<BuildBuyProduct>();

        public BuyProductList(Common.IStatGenerator stats, BuildBuyProduct.eBuyCategory buyCategory, BuildBuyProduct.eBuySubCategory buySubCategory, int minimumPrice, int maximumPrice)
        {
            List<object> products = UserToolUtils.GetObjectProductListFiltered((uint)buyCategory, uint.MaxValue, (ulong)buySubCategory, ulong.MaxValue, ulong.MaxValue, 0x0, uint.MaxValue, 0x0, 0x0);

            stats.IncStat("BuyProduct " + buyCategory);
            stats.IncStat("BuyProduct " + buySubCategory);

            stats.AddStat("BuyProduct Possibles", products.Count);

            foreach (object obj in products)
            {
                BuildBuyProduct product = obj as BuildBuyProduct;
                if (product == null) continue;

                if (!product.ShowInCatalog) continue;

                if (product.Price < minimumPrice) continue;

                if (product.Price > maximumPrice) continue;

                mProducts.Add(product);
            }

            stats.AddStat("BuyProduct Results", mProducts.Count);
        }

        public int Count
        {
            get
            {
                return mProducts.Count;
            }
        }

        public delegate bool TestDelegate<T>(T obj)
            where T : class;

        public T Get<T>(Common.IStatGenerator stats, string name, TestDelegate<T> test, out int price)
            where T : class
        {
            List<BuildBuyProduct> choices = new List<BuildBuyProduct>(mProducts);
            RandomUtil.RandomizeListOfObjects(choices);

            stats.AddStat(name + " Total", choices.Count);

            foreach(BuildBuyProduct choice in choices)
            {
                ResourceKey key = choice.ProductResourceKey;
                
                Hashtable overrides = new Hashtable(0x1);

                IGameObject obj = null;

                try
                {
                    obj = GlobalFunctions.CreateObjectInternal(key, overrides, null);
                }
                catch (Exception e)
                {
                    Common.DebugException(key.ToString(), e);
                    continue;
                }

                T result = obj as T;
                if (result == null) 
                {
                    stats.IncStat(name + " Mismatch " + choice.ObjectInstanceName, Common.DebugLevel.High);
                    stats.IncStat(name + " Wrong Type " + obj.GetType().Name, Common.DebugLevel.High);
                }
                else if ((test != null) && (!test(result)))
                {
                    stats.IncStat(name + " Test Fail");
                }
                else
                {
                    BuildBuyModel model = Sims3.Gameplay.UI.Responder.Instance.BuildBuyModel;

                    List<ResourceKey> presets = new List<ResourceKey>();

                    uint[] objectPresetIdList = model.GetObjectPresetIdList(obj.ObjectId);
                    for (uint i = 0; i < objectPresetIdList.Length; i++)
                    {
                        ThumbnailKey thumbnail = model.GetObjectProductThumbnailKey(obj.ObjectId, ThumbnailSize.Large);
                        if (thumbnail.mDescKey != ResourceKey.kInvalidResourceKey)
                        {
                            uint id = objectPresetIdList[i];
                            thumbnail.mDescKey.GroupId = (thumbnail.mDescKey.GroupId & 0xff000000) | (id & 0xffffff);
                            ResourceKeyContentCategory customContentTypeFromKeyAndPresetIndex = UIUtils.GetCustomContentTypeFromKeyAndPresetIndex(model.GetObjectProductKey(obj.ObjectId), i);
                            if (customContentTypeFromKeyAndPresetIndex == ResourceKeyContentCategory.kInstalled)
                            {
                                customContentTypeFromKeyAndPresetIndex = UIUtils.GetCustomContentType(model.GetObjectProductKey(obj.ObjectId));
                            }
                            string presetString = model.GetPresetString(thumbnail.mDescKey);
                            if (presetString != string.Empty)
                            {
                                KeyValuePair<string, Dictionary<string, Complate>> presetEntryFromPresetString = (KeyValuePair<string, Dictionary<string, Complate>>)SimBuilder.GetPresetEntryFromPresetString(presetString);
                                if ((presetEntryFromPresetString.Value != null) && (presetEntryFromPresetString.Value.Count > 0))
                                {
                                    presets.Add(thumbnail.mDescKey);
                                }
                            }
                        }
                    }

                    if (presets.Count > 0)
                    {
                        ResourceKey preset = RandomUtil.GetRandomObjectFromList(presets);

                        string presetString = model.GetPresetString(preset);
                        if (!string.IsNullOrEmpty(presetString))
                        {
                            object presetEntryFromPresetString = SimBuilder.GetPresetEntryFromPresetString(presetString);
                            if (presetEntryFromPresetString is KeyValuePair<string, Dictionary<string, Complate>>)
                            {
                                FinalizePresetTask.Perform(obj.ObjectId, (KeyValuePair<string, Dictionary<string, Complate>>)presetEntryFromPresetString);
                            }
                        }
                    }

                    stats.IncStat(name + " Found");

                    price = (int)choice.Price;

                    return result;
                }

                obj.Destroy();
            }

            price = 0;
            return null;
        }

        public class FinalizePresetTask : Common.FunctionTask
        {
            ObjectGuid mObject;

            KeyValuePair<string, Dictionary<string, Complate>> mPresetEntry;

            public FinalizePresetTask(ObjectGuid id, KeyValuePair<string, Dictionary<string, Complate>> presetEntry)
            {
                mObject = id;
                mPresetEntry = presetEntry;
            }

            public static void Perform(ObjectGuid id, KeyValuePair<string, Dictionary<string, Complate>> presetEntry)
            {
                new FinalizePresetTask(id, presetEntry).AddToSimulator();
            }

            protected override void OnPerform()
            {
                CompositorUtil.ApplyPreset(mPresetEntry);
                SortedList<string, Complate> patterns = new SortedList<string, Complate>();
                foreach (KeyValuePair<string, Complate> pair2 in mPresetEntry.Value)
                {
                    patterns.Add(pair2.Key, pair2.Value);
                }

                using (DesignModeSwap swap = new DesignModeSwap())
                {
                    if (swap.SetSourceObject(this.mObject))
                    {
                        swap.ClearOldCompositors();
                    }

                    string key = mPresetEntry.Key;
                    Complate complate = null;
                    if (!patterns.TryGetValue(key, out complate))
                    {
                        return;
                    }

                    SortedList<string, bool> enabledStencils = new SortedList<string, bool>();
                    foreach (Complate.Variable variable in complate.Variables)
                    {
                        if (variable.Type == Complate.Variable.Types.Bool)
                        {
                            string str2 = variable.Name.ToLower();
                            if (str2.StartsWith("stencil ") && str2.EndsWith(" enabled"))
                            {
                                enabledStencils.Add(variable.Name, bool.Parse(variable.Value));
                            }
                        }
                    }

                    string presetStringFromPresetEntry = SimBuilder.GetPresetStringFromPresetEntry(mPresetEntry);

                    Complate.SetupDesignSwap(swap, patterns, presetStringFromPresetEntry, true, enabledStencils);

                    swap.ApplyToObject();
                }
            }
        }
    }
}

