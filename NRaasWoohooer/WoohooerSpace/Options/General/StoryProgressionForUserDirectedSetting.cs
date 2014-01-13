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

namespace NRaas.WoohooerSpace.Options.General
{
    public class StoryProgressionForUserDirectedSetting : BooleanSettingOption<GameObject>, IGeneralOption
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mStoryProgressionForUserDirected;
            }
            set
            {
                Woohooer.Settings.mStoryProgressionForUserDirected = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "StoryProgressionForUserDirected";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Woohooer.Settings.mLinkToStoryProgression) return false;

            return base.Allow(parameters);
        }
    }
}
