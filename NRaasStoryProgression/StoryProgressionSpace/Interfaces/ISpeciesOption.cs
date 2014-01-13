using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interfaces
{
    public interface ISpeciesOption : IDelayedInstallable, INotRootLevelOption
    {
        CASAgeGenderFlags Species
        {
            get;
            set;
        }

        void InitDefaultValue();
    }
}

