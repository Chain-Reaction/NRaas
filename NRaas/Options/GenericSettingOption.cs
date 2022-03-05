using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Interfaces;
using System;

namespace NRaas.CommonSpace.Options
{
    public abstract class GenericSettingOption<TType, TTarget> : InteractionOptionItem<IActor, TTarget, GameHitParameters<TTarget>>, ICloseDialogOption, IPersistence, ITitlePrefixOption
        where TTarget : class, IGameObject
    {
        public GenericSettingOption()
        {}
        public GenericSettingOption(string name)
            : base(name, -1)
        { }

        protected abstract TType Value
        {
            get;
            set;
        }

        public abstract ITitlePrefixOption ParentListingOption
        {
            get;
        }

        public virtual string ExportName
        {
            get
            {
                string name = GetTitlePrefix();

                ITitlePrefixOption parent = ParentListingOption;
                if (parent != null)
                {
                    name = parent.ExportName + name;
                }

                return name;
            }
        }

        public override string DisplayValue
        {
            get
            {
                string key = DisplayKey;
                if (!string.IsNullOrEmpty(key))
                {
                    return Common.Localize(key + ":" + Value.ToString());
                }
                else
                {
                    return Value.ToString();
                }
            }
        }

        protected virtual string GetPrompt()
        {
            string value;
            if (!Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[0], out value)) return null;

            return value;
        }

        public virtual string GetExportValue()
        {
            TType value = Value;
            if (value == null) return null;
            
            return value.ToString();
        }

        public abstract void SetImportValue(string value);

        public virtual string PersistencePrefix
        {
            get { return ""; }
        }

        public virtual void Import(Persistence.Lookup settings)
        {
            string name = GetType().ToString();
            try
            {
                string value = null;
                if (!settings.GetString(ExportName, out value)) return;

                SetImportValue(value);
            }
            catch (Exception e)
            {
                Common.Exception(name, e);
            }
        }

        public virtual void Export(Persistence.Lookup settings)
        {
            string name = GetType().ToString();

            try
            {
                settings.Add(ExportName, GetExportValue());
            }
            catch (Exception e)
            {
                Common.Exception(name, e);
            }
        }
    }
}
