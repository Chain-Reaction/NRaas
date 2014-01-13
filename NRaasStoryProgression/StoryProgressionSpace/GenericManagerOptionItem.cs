using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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
    public abstract class GenericManagerOptionItem<TManager, TType> : GenericOptionItem<TType>, IInstallable<TManager>, IGeneralOption, INotExportableOption
        where TManager : StoryProgressionObject
        where TType : class
    {
        TManager mManager;

        public GenericManagerOptionItem(TType value, TType defValue)
            : base(value, defValue)
        { }

        public TManager Manager
        {
            get { return mManager; }
            set { mManager = value; }
        }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        public virtual bool HasRequiredVersion()
        {
            return true;
        }

        public string LocalizedSettingPath
        {
            get { return mManager.GetLocalizedName(); }
        }

        public override object PersistValue
        {
            set
            {
                TType list = value as TType;
                if (list != null)
                {
                    SetValue(list);
                }
            }
        }

        public virtual bool Install(TManager manager, bool initial)
        {
            if (!HasRequiredVersion()) return false;

            if (Manager == null)
            {
                Manager = manager;
            }

            return Manager.AddOption(this);
        }

        protected virtual void PrivateStartup()
        { }

        protected virtual void PrivateUpdate(bool fullUpdate, bool initialPass)
        { }

        public virtual void Startup()
        {
            PrivateStartup();
        }

        public void Shutdown()
        {
            PrivateShutdown();
        }

        protected virtual void PrivateShutdown()
        { }

        public void Update(bool fullUpdate, bool initialPass)
        {
            PrivateUpdate(fullUpdate, initialPass);
        }

        public override string GetStoreKey()
        {
            if (Manager == null) return null;

            string text = base.GetStoreKey();
            if (string.IsNullOrEmpty(text)) return null;

            return Manager.UnlocalizedName + text;
        }

        public virtual bool Progressed
        {
            get { return true; }
        }

        public override bool ShouldDisplay()
        {
            if (Progressed)
            {
                return Manager.ProgressionEnabled;
            }
            else
            {
                return true;
            }
        }
    }
}
