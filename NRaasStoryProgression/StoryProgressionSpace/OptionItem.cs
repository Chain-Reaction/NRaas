using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public interface IOptionItem : ICommonOptionItem
    {
        bool Perform();

        bool ShouldDisplay();
    }

    public abstract class OptionItem : ILocalizer, IOptionItem
    {
        public OptionItem()
        { }

        protected string LocalizeValue(string key)
        {
            return LocalizeValue(key, new object[0]);
        }
        protected virtual string LocalizeValue(string key, object[] parameters)
        {
            string prefix = GetLocalizationValueKey();
            if (string.IsNullOrEmpty(prefix)) return key;

            return Common.Localize(prefix + ":" + key, IsFemaleLocalization(), parameters);
        }

        public string Localize(string key)
        {
            return Localize(key, new object[0]);
        }
        public string Localize(string key, object[] parameters)
        {
            return Localize(key, IsFemaleLocalization(), parameters);
        }
        public bool Localize(string key, object[] parameters, out string story)
        {
            return Localize(key, IsFemaleLocalization(), parameters, out story);
        }
        public bool Localize(string key, bool female, object[] parameters, out string story)
        {
            string localizationKey = GetLocalizationKey();
            if (localizationKey == null)
            {
                if ((Common.kDebugging) && (!(this is IDebuggingOption)))
                {
                    story = "NoKey: " + key;
                }
                else
                {
                    story = "Debug: " + key;
                }
                return false;
            }

            if (Common.Localize(localizationKey + ":" + key, female, parameters, out story))
            {
                return true;
            }
            else
            {
                if ((Common.kDebugging) && (!(this is IDebuggingOption)))
                {
                    story = "NoTrans: " + localizationKey + ":" + key;
                }
                else
                {
                    story = "Debug: " + localizationKey + ":" + key;
                }
                return false;
            }
        }
        public string Localize(string key, bool female, object[] parameters)
        {
            string story;
            Localize(key, female, parameters, out story);
            return story;
        }

        public abstract string GetTitlePrefix();

        protected virtual bool IsFemaleLocalization()
        {
            return false;
        }

        public int ValueWidth
        {
            get { return 0; }
        }

        public virtual string Name
        {
            get
            {
                string name = Localize("MenuName");

                if (!ShouldDisplay())
                {
                    name = "(" + name + ")";
                }
                return name;
            }
        }

        public ThumbnailKey Thumbnail
        {
            get { return ThumbnailKey.kInvalidThumbnailKey; }
        }

        public virtual bool UsingCount
        {
            get { return (Count > 0); }
        }

        public virtual int Count
        {
            get { return 0; }
            set { }
        }

        public string DisplayKey
        {
            get { return null; }
        }

        public virtual string GetLocalizationKey()
        {
            return GetTitlePrefix();
        }

        protected abstract string GetLocalizationValueKey();

        public string DisplayValue
        {
            get { return GetUIValue(false); }
        }

        public abstract string GetUIValue(bool pure);

        public abstract bool ShouldDisplay();

        protected abstract bool PrivatePerform ();

        public override string ToString()
        {
            return Name + "=" + DisplayValue;
        }

        public virtual ICommonOptionItem Clone()
        {
            return MemberwiseClone() as ICommonOptionItem;
        }

        public virtual void NotifyAfterChange(string originalUIValue)
        {
            string uiValue = DisplayValue;
            if ((!string.IsNullOrEmpty(Name)) && (!string.IsNullOrEmpty(uiValue)) && (uiValue != originalUIValue))
            {
                Common.Notify(Name + " = " + uiValue);
            }
        }

        public bool Perform()
        {
            string originalUIValue = DisplayValue;

            if (!PrivatePerform()) return false;

            NotifyAfterChange(originalUIValue);

            Persist();
            return true;
        }

        public virtual string GetStoreKey()
        {
            return GetTitlePrefix();
        }

        public string ExportName
        {
            get
            {
                if (Store == null) return null;

                return GetStoreKey();
            }
        }

        public abstract object PersistValue { get;  set; }

        public string GetExportValue()
        {
            object value = PersistValue;
            if (value == null) return null;
            
            return value.ToString();
        }

        public void SetImportValue(string value)
        {
            PersistValue = value;
        }

        protected virtual PersistentOptionBase Store 
        {
            get
            {
                if (StoryProgression.Main == null) return null;

                return StoryProgression.Main.Options;
            }
        }

        public virtual bool Persist()
        {
            PersistentOptionBase options = Store;
            if (options == null) return false;

            return options.Store(this);
        }

        public static List<T> ListOptions<T>(List<T> allOptions, string name, int maxSelection, out bool okayed)
            where T : class, ICommonOptionItem
        {
            if ((allOptions == null) || (allOptions.Count == 0))
            {
                okayed = false;

                return new List<T>();
            }

            CommonSelection<T>.Results results = new CommonSelection<T>(name, allOptions).SelectMultiple(maxSelection);

            okayed = results.mOkayed;

            return new List<T>(results);
        }
    }
}
