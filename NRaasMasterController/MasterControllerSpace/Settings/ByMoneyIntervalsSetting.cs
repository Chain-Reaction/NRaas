using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class ByMoneyIntervalsSetting : IntegerSettingOption<GameObject>, ISettingOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.MasterController.Settings.mByMoneyIntervals;
            }
            set
            {
                NRaas.MasterController.Settings.mByMoneyIntervals = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ByMoneyIntervalsSetting";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new SimListingOption(); }
        }
    }
}
