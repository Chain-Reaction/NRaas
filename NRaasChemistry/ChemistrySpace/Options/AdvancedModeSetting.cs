using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options
{
    public class AdvancedModeSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Chemistry.Settings.mAdvancedMode;
            }
            set
            {
                Chemistry.Settings.mAdvancedMode = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AdvancedMode";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
