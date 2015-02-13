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

namespace NRaas.TaggerSpace.Options.ColorOptions.LotTags
{
    public class ColorLotTagsByCash : BooleanSettingOption<GameObject>, ILotTagColorOption
    {
        protected override bool Value
        {
            get
            {
                return Tagger.Settings.mColorLotTagsByCash;
            }
            set
            {
                Tagger.Settings.mColorLotTagsByRelationship = false;
                Tagger.Settings.mColorLotTagsByCash = value;

                Tagger.InitTags(true);
            }
        }

        public override string GetTitlePrefix()
        {
            return "ColorLotTagsByCash";
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