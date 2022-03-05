using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.DebuggingLevel
{
    public interface IOverallDebuggingOption : IInteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>
    { }
}
