using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.TagData
{
    public class AgeInYearsCustomLengthOption : IntegerSettingOption<GameObject>, ITagDataOption
    {
        protected override int Validate(int value)
        {
            return value < 0 ? 0 : value;
        }

        protected override int Value
        {
            get
            {
                return Tagger.Settings.mAgeInYearsCustomLength;
            }
            set
            {
                Tagger.Settings.mAgeInYearsCustomLength = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AgeInYearsLength";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (Tagger.Settings.mAgeInYearsScalesToSeasons && GameUtils.IsInstalled(ProductVersion.EP8)) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}