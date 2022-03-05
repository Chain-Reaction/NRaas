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
    using NRaas;
    using NRaas.CommonSpace.Options;
    using NRaas.TaggerSpace.Options.CustomTitles;
    using Sims3.Gameplay.Abstracts;
    using Sims3.Gameplay.Interfaces;
    using System;
    using System.Runtime.InteropServices;

    public class ModifyCustomTitles : ListedSettingOption<ICustomTitleOption, GameObject>, ISimOption
    {
        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        public override bool ConvertFromString(string value, out ICustomTitleOption newValue)
        {
            newValue = null;
            return true;
        }

        public override string ConvertToString(ICustomTitleOption value)
        {
            return "";
        }

        protected override ListedSettingOption<ICustomTitleOption, GameObject>.Proxy GetList()
        {
            return null;
        }

        public override string GetTitlePrefix()
        {
            return "ModifyCustomTitles";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return new InteractionOptionList<ICustomTitleOption, GameObject>.AllList(this.Name, false).Perform(new GameHitParameters<GameObject>(parameters.mTarget as IActor, parameters.mActor as GameObject, parameters.mHit));
        }

        public override string DisplayValue
        {
            get
            {
                return null;
            }
        }

        public override string Name
        {
            get
            {
                return Common.Localize("ModifyCustomTitles:MenuName");
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get
            {
                return null;
            }
        }
    }
}

