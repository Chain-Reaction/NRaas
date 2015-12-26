using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
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

namespace NRaas.TaggerSpace.Options
{
    public class DefaultFilterSetting : FilterSetting<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "DefaultFilter";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override Proxy GetList()
        {
            return new DictionaryProxy(Tagger.Settings.mDefaultFilter);
        }

        protected override void PrivatePerform(IEnumerable<ListedSettingOption<string, GameObject>.Item> results)
        {
            base.PrivatePerform(results);

            Tagger.InitTags(false);
        }
    }
}