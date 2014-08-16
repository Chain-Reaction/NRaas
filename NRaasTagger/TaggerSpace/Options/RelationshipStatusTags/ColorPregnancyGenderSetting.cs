using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.RelationshipStatusTags
{
    public class ColorPregnancyGenderSetting : BooleanSettingOption<GameObject>, IRelationshipStatusTagOption
    {
        SimType mData;

        public ColorPregnancyGenderSetting(SimType data)
        {
            mData = data;
        }

        protected override bool Value
        {
            get
            {
                return Tagger.Settings.mColorPregnancyTag;
            }
            set
            {
                Tagger.Settings.mColorPregnancyTag = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ColorPregnancyTag";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return mData == SimType.Pregnant;
        }
    }
}