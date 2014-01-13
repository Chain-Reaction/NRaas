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
    public abstract class StringManagerOptionItem<TManager> : StringOptionItem, IStoryProgressionUpdater, IInstallable<TManager>
        where TManager : StoryProgressionObject
    {
        TManager mManager;

        public StringManagerOptionItem(string value)
            : base(value)
        { }

        public TManager Manager
        {
            get { return mManager; }
            set { mManager = value; }
        }

        public virtual bool HasRequiredVersion()
        {
            return true;
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

        public void Startup()
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
