using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
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
    public abstract class DegreeBaseOption : GenericOptionBase.ListedOptionItem<AcademicDegreeNames, AcademicDegreeNames>
    {
        public DegreeBaseOption()
            : base(new List<AcademicDegreeNames>(), new List<AcademicDegreeNames>())
        { }

        protected override AcademicDegreeNames ConvertFromString(string value)
        {
            AcademicDegreeNames result;
            if (!ParserFunctions.TryParseEnum<AcademicDegreeNames>(value, out result, AcademicDegreeNames.Undefined))
            {
                ulong guid;
                if (ulong.TryParse(value, out guid))
                {
                    result = (AcademicDegreeNames)guid;

                    if (AcademicDegreeManager.GetStaticElement(result) == null)
                    {
                        return AcademicDegreeNames.Undefined;
                    }
                }
            }
            return result;
        }

        protected override AcademicDegreeNames ConvertToValue(AcademicDegreeNames value, out bool valid)
        {
            valid = true;
            return value;
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(AcademicDegreeNames value, ref ThumbnailKey icon)
        {
            AcademicDegreeStaticData degree = AcademicDegreeManager.GetStaticElement(value);
            if (degree == null) return null;

            // icon = new ThumbnailKey(ResourceKey.CreatePNGKey(career.CareerIconColored, ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)), ThumbnailSize.Medium);

            return degree.DegreeName;
        }

        protected override IEnumerable<AcademicDegreeNames> GetOptions()
        {
            List<AcademicDegreeNames> results = new List<AcademicDegreeNames>();

            foreach (AcademicDegreeStaticData data in AcademicDegreeManager.sDictionary.Values)
            {
                results.Add(data.Guid);
            }

            return results;
        }
    }
}

