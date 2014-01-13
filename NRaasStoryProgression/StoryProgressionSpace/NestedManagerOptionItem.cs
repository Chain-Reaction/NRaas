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
    public abstract class NestingManagerOptionItem<TManager,TOption> : NestingOptionItem<TOption>, IInstallable<TManager>
        where TManager : StoryProgressionObject
        where TOption : class, IOptionItem
    {
        TManager mManager;

        public NestingManagerOptionItem()
        { }
        public NestingManagerOptionItem(TManager manager)
        {
            mManager = manager;
        }

        public TManager Manager
        {
            get { return mManager; }
            set { mManager = value; }
        }

        public override StoryProgressionObject GetManager()
        {
            return mManager;
        }

        public override string GetStoreKey()
        {
            if (Manager == null) return null;

            string text = base.GetStoreKey();
            if (string.IsNullOrEmpty(text)) return null;

            return Manager.UnlocalizedName + text;
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
    }
}
