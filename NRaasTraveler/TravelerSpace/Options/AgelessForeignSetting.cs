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

namespace NRaas.TravelerSpace.Options
{
    public class AgelessForeignSetting : WorldNameSetting<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "AgelessForeign";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override Proxy GetList()
        {
            return new DictionaryProxy(Traveler.Settings.mAgelessForeign);
        }

        protected override void PrivatePerform(IEnumerable<ListedSettingOption<WorldName, GameObject>.Item> results)
        {
            base.PrivatePerform(results);

            Traveler.UpdateAgeForeign();
        }
    }
}
