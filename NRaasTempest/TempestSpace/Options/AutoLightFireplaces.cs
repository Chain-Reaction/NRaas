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

namespace NRaas.TempestSpace.Options
{
    public class AutoLightFireplaces : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Tempest.Settings.mAutoLightFireplaces;
            }
            set
            {
                Tempest.Settings.mAutoLightFireplaces = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AutoLightFireplaces";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}