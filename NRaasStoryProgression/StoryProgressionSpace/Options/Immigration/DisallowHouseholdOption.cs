using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options.Immigration
{
    public class DisallowHouseholdOption<TManager> : MultiListedManagerOptionItem<TManager, string>, ISimFromBinOption<TManager>
        where TManager : StoryProgressionObject, ISimFromBinManager
    {
        SimFromBinController mController;

        public DisallowHouseholdOption()
            : base(new List<string>())
        { }

        public override string GetTitlePrefix()
        {
            return "DisallowImmigrantHousehold";
        }

        public SimFromBinController Controller
        {
            set { mController = value; }
        }

        public override bool ShouldDisplay()
        {
            if (!mController.ShouldDisplayImmigrantOptions()) return false;

            return base.ShouldDisplay();
        }

        public override string ValuePrefix
        {
            get { return "Disallowed"; }
        }

        protected override bool PersistCreate(ref string defValue, string value)
        {
            defValue = value;
            return true;
        }

        protected override List<IGenericValueOption<string>> GetAllOptions()
        {
            List<IGenericValueOption<string>> results = new List<IGenericValueOption<string>>();

            BinModel.Singleton.PopulateExportBin();

            foreach (ExportBinContents content in BinModel.Singleton.ExportBinContents)
            {
                if (content.HouseholdName == null) continue;

                if (content.HouseholdName.Contains(".Settings.")) continue;

                if (content.HouseholdSims == null) continue;

                if (content.HouseholdSims.Count > Manager.GetValue<MaximumBinHouseSizeOption<TManager>, int>()) continue;

                results.Add(new ListItem(this, content.HouseholdName, content.PackageName));
            }

            return results;
        }

        public class ListItem : BaseListItem<DisallowHouseholdOption<TManager>>
        {
            public ListItem(DisallowHouseholdOption<TManager> option, string name, string value)
                : base(option, value)
            {
                mName = name;
            }

            public override string Name
            {
                get { return mName; }
            }
        }
    }
}

