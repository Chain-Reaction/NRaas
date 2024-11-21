﻿using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.ColorOptions.SimTags.Commodity
{
    public class CommodityOption : CommodityBaseOption<GameObject>, ISimTagCommodityColorOption
    {
        CommodityKind mType;

        public CommodityOption(CommodityKind type)
        {            
            mType = type;
            mName = Tagger.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:Motive" + mType.ToString());
        }

        protected override CommodityKind Value
        {
            get
            {                
                return Tagger.Settings.mColorByCommodity;
            }
            set
            {
                Tagger.Settings.mColorByCommodity = mType;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }
        
        public override string DisplayValue
        {
            get
            {
                return (Value == mType ? "True" : "False");
            }
        }
    }
}