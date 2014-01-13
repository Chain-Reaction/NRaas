using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
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
    public abstract class MultiListedManagerOptionItem<TManager, TType> : MultiListedBaseManagerOptionItem<TManager,TType>, IGeneralOption
        where TManager : StoryProgressionObject
    {
        public MultiListedManagerOptionItem(TType[] value)
            : base(value)
        { }
        public MultiListedManagerOptionItem(List<TType> value)
            : base(value)
        { }
    }
}
