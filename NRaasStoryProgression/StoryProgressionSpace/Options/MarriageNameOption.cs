using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class MarriageNameOption : NameBaseOption, IReadSimLevelOption, IWriteSimLevelOption, IHouseLevelSimOption
    {
        public MarriageNameOption()
            : base(NameTakeType.FemMale)
        { }

        public override string GetTitlePrefix()
        {
            return "MarriageName";
        }

        protected override string GetLocalizationValueKey()
        {
            return "NameTakeType";
        }
    }
}

