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
    public abstract class EnumManagerOptionItem<TManager,TType> : EnumBaseManagerOptionItem<TManager,TType>, IGeneralOption
        where TManager : StoryProgressionObject
    {
        public EnumManagerOptionItem(TType value)
            : base(value, value)
        { }
        public EnumManagerOptionItem(TType value, TType defValue)
            : base(value, defValue)
        { }
    }
}
