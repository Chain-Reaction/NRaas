using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Dialogs;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace
{
    [Persistable]
    public abstract class OptionList<T> : InteractionOptionList<T, GameObject>, IInteractionOptionItem<IActor, GameObject, GameHitParameters< GameObject>>
        where T : class, IInteractionOptionItem<IActor, GameObject, GameHitParameters< GameObject>>
    {
        public OptionList()
        { }
    }
}
