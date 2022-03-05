using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options
{
    public class DebuggingSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Chemistry.Settings.Debugging;
            }
            set
            {
                Chemistry.Settings.Debugging = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "Debugging";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
