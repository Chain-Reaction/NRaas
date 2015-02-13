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
    public class AgeInYearsScalesToSeasonsOption : BooleanSettingOption<GameObject>, ITagDataOption
    {
        protected override bool Value
        {
            get
            {
                return Tagger.Settings.mAgeInYearsScalesToSeasons;
            }
            set
            {
                Tagger.Settings.mAgeInYearsScalesToSeasons = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AgeInYearsSeasons";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP8)) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}