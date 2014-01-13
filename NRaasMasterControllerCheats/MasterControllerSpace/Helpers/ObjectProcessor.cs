using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public abstract class ObjectProcessor
    {
        string mLocalizeKey;

        public ObjectProcessor(string localizeKey)
        {
            mLocalizeKey = localizeKey;
        }

        public string LocalizeKey
        {
            get
            {
                return mLocalizeKey;
            }
        }

        protected abstract void GetObjects(Dictionary<string, Item> results);

        protected Item AddItem(Dictionary<string, Item> hash, string name, IGameObject obj)
        {
            Item item = new Item(name, obj);

            hash.Add(name, item);

            return item;
        }

        protected List<Item> GetObjects(string header)
        {
            Dictionary<string, Item> hash = new Dictionary<string, Item>();
            GetObjects(hash);

            {
                List<Item> options = new List<Item>();


                int count = 0;
                foreach (Item item in hash.Values)
                {
                    options.Add(item);

                    count += item.Count;
                }

                List<Item> allOptions = new List<Item>(options);
                allOptions.Add(new Item("(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")", null));

                CommonSelection<Item>.Results selection = new CommonSelection<Item>(header, allOptions).SelectMultiple();

                if ((selection != null) && (selection.Count > 0))
                {
                    foreach (Item list in selection)
                    {
                        if (list.mObjects.Count == 0)
                        {
                            return options;
                        }
                    }

                    Item all = new Item("(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")", null);

                    hash.Clear();

                    foreach (Item list in selection)
                    {
                        all.mObjects.AddRange(list.mObjects);

                        all.IncCount(list.mObjects.Count);

                        if (list.mObjects.Count == 0)
                        {
                            return options;
                        }
                        else
                        {
                            string name = null;

                            int count2 = 0;
                            foreach (IGameObject obj in list.mObjects)
                            {
                                name = GetName(obj, false);

                                Item item;
                                if (!hash.TryGetValue(name, out item))
                                {
                                    item = AddItem(hash, name, obj);
                                    if (item != null)
                                    {
                                        item.mCatalogName = list.Name;
                                    }

                                    count2++;
                                }
                                else
                                {
                                    item.Add(obj);
                                }
                            }

                            if (count2 > 1)
                            {
                                name = "(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ": " + list.Name + ")";

                                hash.Add(name, new Item(name, list.Name, list.mObjects));
                            }
                        }
                    }

                    options.Clear();

                    if (all.Count > 1)
                    {
                        options.Add(all);
                    }

                    foreach (Item item in hash.Values)
                    {
                        options.Add(item);
                    }

                    return new List<Item>(new CommonSelection<Item>(header, options).SelectMultiple());
                }
            }

            return null;
        }

        protected abstract bool Initialize();

        public delegate bool ProcessObject(IGameObject obj);

        public OptionResult Perform(ProcessObject onProcess)
        {
            if (!Initialize()) return OptionResult.Failure;

            Perform(GetObjects(Common.Localize(LocalizeKey + ":Header")), onProcess);
            return OptionResult.SuccessRetain;
        }

        protected void Perform(List<Item> selection, ProcessObject onProcess)
        {
            if ((selection != null) && (selection.Count > 0))
            {
                string msg = null;
                if (selection.Count == 1)
                {
                    msg = Common.Localize(LocalizeKey + ":Single", false, new object[] { selection[0].Name });
                }
                else
                {
                    msg = Common.Localize(LocalizeKey + ":Multiple", false, new object[] { selection.Count });
                }

                if (AcceptCancelDialog.Show(msg))
                {
                    int changed = 0;

                    Dictionary<IGameObject, bool> objs = new Dictionary<IGameObject, bool>();

                    foreach (Item subitem in selection)
                    {
                        Item item = subitem as Item;

                        foreach (IGameObject obj in item.mObjects)
                        {
                            if (objs.ContainsKey(obj)) continue;

                            objs.Add(obj, true);
                        }
                    }

                    foreach (IGameObject obj in objs.Keys)
                    {
                        try
                        {
                            if (onProcess(obj))
                            {
                                changed++;
                            }
                        }
                        catch
                        { }
                    }

                    StyledNotification.Format format = new StyledNotification.Format(Common.Localize(LocalizeKey + ":Success", false, new object[] { changed }), ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage);
                    format.mTNSCategory = NotificationManager.TNSCategory.Lessons;
                    StyledNotification.Show(format);
                }
            }
        }

        protected static string GetName(IGameObject obj, bool classname)
        {
            if ((!classname) && (obj is GameObject))
            {
                if (obj is Sim)
                {
                    return (obj as Sim).Name;
                }
                else
                {
                    try
                    {
                        string result = (obj as GameObject).ToTooltipString();
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(obj, e);
                    }
                }
            }

            string name = null;

            name = obj.GetType().Namespace + "." + obj.GetType().Name;
            name = name.Replace("Sims3.Gameplay.", "");

            return name;
        }

        public class Item : CommonOptionItem
        {
            public string mCatalogName;

            public List<IGameObject> mObjects = new List<IGameObject>();

            public Item()
            { }
            public Item(string name, string catalogName, List<IGameObject> objs)
                : base(name, objs.Count)
            {
                mCatalogName = catalogName;
                mObjects.AddRange(objs);
            }
            public Item(string name, IGameObject obj)
                : base(name, 0, GetThumbnailKey(obj))
            {
                Add(obj);
            }

            public override string DisplayValue
            {
                get { return null; }
            }

            protected static ThumbnailKey GetThumbnailKey(IGameObject obj)
            {
                if (obj != null)
                {
                    try
                    {
                        return obj.GetThumbnailKey();
                    }
                    catch
                    { }
                }

                return ThumbnailKey.kInvalidThumbnailKey;
            }

            public void Add(IGameObject obj)
            {
                if (obj == null) return;
                
                if (obj is PlumbBob)
                {
                    if (PlumbBob.Singleton == obj) return;
                }
                
                if (mObjects.Contains(obj)) return;

                mObjects.Add(obj);
                IncCount();
            }
        }
    }
}
