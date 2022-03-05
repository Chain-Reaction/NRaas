using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.Attraction
{
    public interface IAttractionOption : IInteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>
    { }
}
