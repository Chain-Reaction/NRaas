using NRaas.CommonSpace.Options;
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
    public abstract class CommodityBaseOption<TTarget> : GenericSettingOption<CommodityKind, TTarget>
        where TTarget : class, IGameObject
    {        
        public override void SetImportValue(string value)
        {
            //Value = value;
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            Value = Value;

            Tagger.Settings.mColorByJobPerformance = false;
            Tagger.Settings.mColorTagsByAge = false;
            Tagger.Settings.mColorTagsByRelationship = false;
            Tagger.Settings.mColorTagsByRelationshipStatus = false;
            Tagger.Settings.mColorTagsBySimType = false;
            Tagger.Settings.mColorByCash = false;
            Tagger.Settings.mColorByMood = false;

            Tagger.InitTags(false);

            Common.Notify(ToString());
            return OptionResult.SuccessClose;
        }
    }
}