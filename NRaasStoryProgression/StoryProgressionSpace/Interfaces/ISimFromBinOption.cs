using NRaas.StoryProgressionSpace.Helpers;
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
    public interface ISimFromBinOption<TManager> : ManagerSim.IImmigrationEmigrationOption, ManagerLot.IImmigrationEmigrationOption, IGeneralOption, IInstallable<TManager>
        where TManager : StoryProgressionObject, ISimFromBinManager
    {
        SimFromBinController Controller
        {
            set;
        }
    }
}

