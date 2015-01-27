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

namespace NRaas.TaggerSpace.Options.ColorOptions.SimTags
{
    public class ColorTagsByRelationshipStatus : BooleanSettingOption<GameObject>, ISimTagColorOption
    {
        protected override bool Value
        {
            get
            {
                return Tagger.Settings.mColorTagsByRelationshipStatus;
            }
            set
            {
                Tagger.Settings.mColorTagsByRelationshipStatus = value;

                Tagger.Settings.mColorTagsByOrientation = false;
                Tagger.Settings.mColorTagsByRelationship = false;
                Tagger.Settings.mColorTagsByAge = false;
                Tagger.Settings.mColorTagsBySimType = false;
                Tagger.Settings.mColorByCash = false;
                Tagger.Settings.mColorByJobPerformance = false;
                Tagger.Settings.mColorByMood = false;
                Tagger.Settings.mColorByCommodity = CommodityKind.None;
                Tagger.InitTags(false);
            }
        }

        public override string GetTitlePrefix()
        {
            return "ColorTagsByRelationshipStatus";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}