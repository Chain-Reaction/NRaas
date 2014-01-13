using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interfaces
{
    public interface IPersonalityOption : IInstallable<SimPersonality>, INameableOption
    {
        bool Parse(XmlDbRow row, SimPersonality personality, ref string error);
    }
}

