using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
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

namespace NRaas.MasterControllerSpace.Settings.CAS.Blacklist
{
    public class ExportBlacklist : OptionItem, IBlacklistOption, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "ExportBlacklist";
        }

        public void Import(Persistence.Lookup settings)
        {
            MasterController.Settings.ImportBlacklist(settings);
        }

        public void Export(Persistence.Lookup settings)
        {
            MasterController.Settings.ExportBlacklist(settings);
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (MasterController.Settings.BlacklistPartsCount == 0)
            {
                Common.Notify(Common.Localize(GetTitlePrefix() + ":Failure"));
                return OptionResult.Failure;
            }

            Common.WriteLog(MasterController.Settings.GetBlacklistParts(), false);

            Common.Notify(Common.Localize(GetTitlePrefix() + ":Success"));
            return OptionResult.SuccessClose;
        }
    }
}
