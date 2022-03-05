using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.Performance
{
    public interface IPerformanceOption : IInteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>
    { }
}
