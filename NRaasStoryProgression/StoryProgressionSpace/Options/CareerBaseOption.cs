using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public abstract class CareerBaseOption : GenericOptionBase.ListedOptionItem<OccupationNames, OccupationNames>
    {
        public CareerBaseOption()
            : base(new List<OccupationNames>(), new List<OccupationNames>())
        { }

        protected override OccupationNames ConvertFromString(string value)
        {
            OccupationNames result;
            if (!ParserFunctions.TryParseEnum<OccupationNames>(value, out result, OccupationNames.Undefined))
            {
                ulong guid;
                if (ulong.TryParse(value, out guid))
                {
                    result = (OccupationNames)guid;

                    if (CareerManager.GetStaticOccupation(result) == null)
                    {
                        return OccupationNames.Undefined;
                    }
                }
            }
            return result;
        }

        protected override OccupationNames ConvertToValue(OccupationNames value, out bool valid)
        {
            valid = true;
            return value;
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(OccupationNames value, ref ThumbnailKey icon)
        {
            Occupation career = CareerManager.GetStaticOccupation(value);
            if (career == null) return null;

            icon = new ThumbnailKey(ResourceKey.CreatePNGKey(career.CareerIconColored, ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)), ThumbnailSize.Medium);

 	        return career.CareerName;
        }

        protected override IEnumerable<OccupationNames> GetOptions()
        {
            List<OccupationNames> results = new List<OccupationNames>();

            foreach(Occupation career in CareerManager.OccupationList)
            {
                results.Add(career.Guid);
            }

            return results;
        }
    }
}

