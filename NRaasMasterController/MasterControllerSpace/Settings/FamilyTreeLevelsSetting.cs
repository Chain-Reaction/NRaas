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
    public class FamilyTreeLevelsSetting : IntegerSettingOption<GameObject>, ISettingOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.MasterController.Settings.mFamilyTreeLevels;
            }
            set
            {
                NRaas.MasterController.Settings.mFamilyTreeLevels = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "FamilyTreeLevelsSetting";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new SimListingOption(); }
        }
    }
}
