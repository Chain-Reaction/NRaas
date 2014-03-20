using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public abstract class OperationSettingNestledOption<TTarget> : OperationSettingOption<TTarget>
        where TTarget : class, IGameObject
    {
        public OperationSettingNestledOption()
        { }
        public OperationSettingNestledOption(string name)
            : base(name, -1)
        { }

        public abstract ITitlePrefixOption ParentListingOption
        {
            get;
        }
    }
}