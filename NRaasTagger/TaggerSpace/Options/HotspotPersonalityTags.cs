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

namespace NRaas.TaggerSpace.Options
{
    public class HotspotPersonalityTags : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Tagger.Settings.mHotspotPersonalityTags;
            }
            set
            {
                Tagger.Settings.mHotspotPersonalityTags = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HotspotPersonalityTags";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.AssemblyCheck.IsInstalled("NRaasStoryProgression")) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}