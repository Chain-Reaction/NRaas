using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.Profiles.Filters.Criteria
{
    public interface ICriteriaSettingOption : IInteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>
    { }
}
