using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
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
    public class CasteZodiacFilterOption : GenericOptionBase.ListedOptionItem<Zodiac, Zodiac>, IReadCasteLevelOption, IWriteCasteLevelOption, ISimCasteOption, ICasteFilterOption
    {
        public CasteZodiacFilterOption()
            : base(new List<Zodiac>(), new List<Zodiac>())
        { }

        public override string GetTitlePrefix()
        {
            return "CasteZodiacFilter";
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(Zodiac value, ref ThumbnailKey icon)
        {           
            return Common.LocalizeEAString("Ui/Caption/HUD/KnownInfoDialog:" + value.ToString());
        }

        protected override Zodiac ConvertFromString(string value)
        {
            Zodiac result;
            ParserFunctions.TryParseEnum<Zodiac>(value, out result, Zodiac.Unset);
            return result;
        }

        protected override Zodiac ConvertToValue(Zodiac value, out bool valid)
        {
            valid = true;
            return value;
        }

        protected override string ValuePrefix
        {
            get { return "Boolean"; }
        }

        protected override IEnumerable<Zodiac> GetOptions()
        {
            List<Zodiac> results = new List<Zodiac>();

            foreach (Zodiac flags in Enum.GetValues(typeof(Zodiac)))
            {
                switch (flags)
                {
                    case Zodiac.Unset:                    
                        break;
                    default:
                        results.Add(flags);
                        break;
                }
            }

            return results;
        }

        public override bool ShouldDisplay()
        {
            if ((!GetValue<CasteAutoOption, bool>()) && (!GetValue<CasteInheritedOption, bool>())) return false;

            return base.ShouldDisplay();
        }
    }
}
