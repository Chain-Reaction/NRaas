using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Options.CustomTitles;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
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
    public class ModifyCustomTitles : ListedSettingOption<ICustomTitleOption, GameObject>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "ModifyCustomTitles";
        }

        public override string Name
        {
            get
            {
                return Tagger.Localize("ModifyCustomTitles:MenuName");
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

        protected override ListedSettingOption<ICustomTitleOption, GameObject>.Proxy GetList()
        {
            return null;
        }

        public override string ConvertToString(ICustomTitleOption value)
        {
            return "";
        }

        public override bool ConvertFromString(string value, out ICustomTitleOption newValue)
        {
            newValue = null;
            return true;
        }
        
        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return new InteractionOptionList<ICustomTitleOption, GameObject>.AllList(Name, false).Perform(new GameHitParameters<GameObject>(parameters.mTarget as IActor, parameters.mActor as GameObject, parameters.mHit));
        }
    }
}