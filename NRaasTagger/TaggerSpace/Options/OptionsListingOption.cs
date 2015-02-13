using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options
{
    // this lists the default options under all sources but the computer due to the quick switch
    // filters overriding the primary interaction in those cases
    public class OptionsListingOption : ListedSettingOption<IPrimaryOption<GameObject>, GameObject>, ICityHallOption
    {
        public override string GetTitlePrefix()
        {
            return "Options";
        }

        public override string Name
        {
            get
            {
                return Tagger.Localize("Options:MenuName");
            }
        }

        public override string DisplayValue
        {
            get
            {
                return null;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        protected override ListedSettingOption<IPrimaryOption<GameObject>, GameObject>.Proxy GetList()
        {
            return null;
        }

        public override string ConvertToString(IPrimaryOption<GameObject> value)
        {
            return "";
        }

        public override bool ConvertFromString(string value, out IPrimaryOption<GameObject> newValue)
        {
            newValue = null;
            return true;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return new InteractionOptionList<IPrimaryOption<GameObject>, GameObject>.AllList(Name, false).Perform(new GameHitParameters< GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }
    }
}