using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public abstract class SimTypeOption : GenericOptionBase.ListedOptionItem<SimType, SimType>
    {
        public SimTypeOption(SimType[] types)
            : base(new List<SimType>(), new List<SimType>(types))
        { }

        protected override SimType ConvertFromString(string value)
        {
            SimType result;
            ParserFunctions.TryParseEnum<SimType>(value, out result, SimType.None);
            return result;
        }

        protected override SimType ConvertToValue(SimType value, out bool valid)
        {
            valid = true;
            return value;
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizationValueKey()
        {
            return "SimType";
        }

        protected override string GetLocalizedValue(SimType value, ref ThumbnailKey icon)
        {
            return SimTypes.GetLocalizedName(value);
        }

        protected override string ValuePrefix
        {
            get { return "Boolean"; }
        }

        protected override IEnumerable<SimType> GetOptions()
        {
            List<SimType> results = new List<SimType>();

            foreach(SimType type in Enum.GetValues(typeof(SimType)))
            {
                switch (type)
                {
                    case SimType.None:
                    case SimType.Frankenstein:
                    case SimType.TimeTraveler:
                    case SimType.Deer:
                    case SimType.Raccoon:
                    case SimType.Local:                    
                    case SimType.MiniSim:
                        continue;
                }

                results.Add(type);
            }

            return results;
        }
    }
}

