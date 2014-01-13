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
    public abstract class TraitBaseOption : GenericOptionBase.ListedOptionItem<TraitNames, TraitNames>
    {
        public TraitBaseOption()
            : base(new List<TraitNames>(), new List<TraitNames>())
        { }

        protected override TraitNames ConvertFromString(string value)
        {
            TraitNames result;
            ParserFunctions.TryParseEnum<TraitNames>(value, out result, TraitNames.Unknown);
            return result;
        }

        protected override bool Allow(TraitNames value)
        {
            Trait traitFromDictionary = TraitManager.GetTraitFromDictionary(value);
            if (traitFromDictionary == null)
            {
                return false;
            }

            if (!GameUtils.IsInstalled(traitFromDictionary.ProductVersion))
            {
                return false;
            }

            return base.Allow(value);
        }

        protected override TraitNames ConvertToValue(TraitNames value, out bool valid)
        {
            valid = true;
            return value;
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(TraitNames value, ref ThumbnailKey icon)
        {
            Trait trait = TraitManager.GetTraitFromDictionary(value);
            if (trait == null) return null;

            icon = new ThumbnailKey(ResourceKey.CreatePNGKey(trait.ThumbnailIcon, ResourceUtils.ProductVersionToGroupId(trait.ProductVersion)), ThumbnailSize.Medium);

            return Traits.ProperName(value, false);
        }

        protected override IEnumerable<TraitNames> GetOptions()
        {
            List<TraitNames> results = new List<TraitNames>();

            foreach (Trait trait in TraitManager.GetDictionaryTraits)
            {
                results.Add(trait.Guid);
            }

            return results;
        }
    }
}

