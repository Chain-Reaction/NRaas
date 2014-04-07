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
    public abstract class CareerLevelBaseOption : GenericOptionBase.ListedOptionItem<string, string>
    {
        public CareerLevelBaseOption()
            : base(new List<string>(), new List<string>())
        { }

        protected override string ConvertFromString(string value)
        {            
            return value;
        }

        protected override string ConvertToValue(string value, out bool valid)
        {           
            valid = true;
            return value;
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(string value, ref ThumbnailKey icon)
        {
            string[] split = value.Split('-');            

            if (split.Length < 2)
            {                
                split[0] = ""; // will cause TryParseEnum to fail
            }

            OccupationNames result;
            Occupation career;            
            if (!ParserFunctions.TryParseEnum<OccupationNames>(split[0], out result, OccupationNames.Undefined))
            {                
                ulong guid;
                if (ulong.TryParse(split[0], out guid))
                {                    
                    result = (OccupationNames)guid;
                }
                else
                {                   
                    return OccupationNames.Undefined + " 0";
                }
            }            

            career = CareerManager.GetStaticOccupation(result);            

            if (career == null)
            {                
                return OccupationNames.Undefined + " 0";
            }

            icon = new ThumbnailKey(ResourceKey.CreatePNGKey(career.CareerIconColored, ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)), ThumbnailSize.Medium);
           
            return career.CareerName + " " + split[1];                
        }

        protected override string GetLocalizationValueKey()
        {
            return "CareerLevel";
        }

        protected override string ValuePrefix
        {
            get { return "Boolean"; }
        }

        protected override IEnumerable<string> GetOptions()
        {
            List<string> results = new List<string>();

            foreach(Occupation career in CareerManager.OccupationList)
            {
                int start = 1;
                while (start <= career.HighestLevel)
                {
                    results.Add(career.Guid + "-" + start);
                    start++;
                }
            }

            return results;
        }
    }
}
