using NRaas.CommonSpace.Helpers;
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
    public class ForceTraitOption : TraitBaseOption, IReadSimLevelOption, IWriteSimLevelOption, IHouseLevelSimOption
    {
        public ForceTraitOption()
        { }

        public override string GetTitlePrefix()
        {
            return "ForceTrait";
        }

        protected override string ValuePrefix
        {
            get { return "Boolean"; }
        }

        protected override bool Allow(TraitNames value)
        {
            if (Traits.IsObjectBaseReward(value)) return false;

            return base.Allow(value);
        }
    }
}

