using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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
    public enum OverrideStyle : int
    {
        None = 0x00,
        CopyData = 0x01,
        ClearSet = 0x02,
        MergeSet = 0x04,
    }

    public interface IGenericLevelOption : IInstallable<GenericOptionBase>, ICommonOptionItem
    {
        void Set(IGenericLevelOption option, bool persist);

        new string Name
        {
            get;
        }

        object PersistValue
        {
            get;
            set;
        }

        string GetStoreKey();

        bool Perform();

        string GetUIValue(bool pure);

        bool ShouldDisplay();

        void ApplyOverride(IGenericLevelOption option, OverrideStyle style);

        bool Persist();

        void InitDefaultValue();
    }
}

