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
    public abstract class NameBaseOption : GenericOptionBase.EnumOptionItem<NameTakeType>, INameTakeOption
    {
        public NameBaseOption(NameTakeType defValue)
            : base(defValue, defValue)
        { }

        public string GetNameTakeLocalizationValueKey()
        {
            return GetLocalizationValueKey();
        }

        protected override string GetLocalizedValue(NameTakeType value, ref bool matches, ref ThumbnailKey icon)
        {
            matches = (Value == value);

            return LocalizeValue(value.ToString());
        }
    }
}

