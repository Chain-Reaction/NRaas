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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public abstract class OccultTypeOption : GenericOptionBase.ListedOptionItem<OccultTypes, OccultTypes>
    {
        public OccultTypeOption(OccultTypes[] types)
            : base(new List<OccultTypes>(), new List<OccultTypes>(types))
        { }

        protected override OccultTypes ConvertFromString(string value)
        {
            OccultTypes result;
            ParserFunctions.TryParseEnum<OccultTypes>(value, out result, OccultTypes.None);
            return result;
        }

        protected override OccultTypes ConvertToValue(OccultTypes value, out bool valid)
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
            return "OccultType";
        }

        protected override string GetLocalizedValue(OccultTypes value, ref ThumbnailKey icon)
        {
            return OccultTypeHelper.GetLocalizedName(value);
        }

        protected override string ValuePrefix
        {
            get { return "Boolean"; }
        }

        protected override IEnumerable<OccultTypes> GetOptions()
        {
            List<OccultTypes> results = new List<OccultTypes>();
            results = OccultTypeHelper.CreateListOfMissingOccults(new List<OccultTypes> { OccultTypes.Frankenstein, OccultTypes.Robot }, false);

            return results;
        }
    }
}