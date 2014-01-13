using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interfaces
{
    public abstract class ExpansionInstaller<TInstaller> : IInstallable<TInstaller>
        where TInstaller : IInstallationBase
    {
        public virtual string Name
        {
            get { return GetType().ToString(); }
        }

        public TInstaller Manager
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool HasRequiredVersion()
        {
            return true;
        }

        protected abstract bool PrivateInstall(TInstaller main, bool initial);

        public bool Install(TInstaller main, bool initial)
        {
            if (!HasRequiredVersion()) return false;

            return PrivateInstall(main, initial);
        }
    }
}

