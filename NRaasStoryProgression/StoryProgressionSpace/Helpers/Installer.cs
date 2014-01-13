using NRaas.StoryProgressionSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class Installer<TInstaller>
        where TInstaller : IInstallationBase
    {
        TInstaller mManager;

        public Installer(TInstaller manager)
        {
            mManager = manager;
        }

        public static bool Install(TInstaller manager, IInstallable<TInstaller> install, bool initial)
        {
            try
            {
                IHasDefaultOption defaultOption = install as IHasDefaultOption;
                if (defaultOption != null)
                {
                    defaultOption.InitDefaultValue();
                }

                return install.Install(manager, initial);
            }
            catch (Exception e)
            {
                string name = null;

                try
                {
                    name = install.Name;
                }
                catch
                {
                    name = install.GetType().ToString();
                }

                Common.Exception(name, e);
                return false;
            }
        }

        public void Perform(bool initial)
        {
            List<IInstallable<TInstaller>> list = Common.DerivativeSearch.Find<IInstallable<TInstaller>>(Common.DerivativeSearch.Caching.NoCache);

            foreach (IInstallable<TInstaller> install in list)
            {
                if (install is IDelayedInstallable) continue;

                Install(mManager, install, initial);
            }
        }
    }
}

