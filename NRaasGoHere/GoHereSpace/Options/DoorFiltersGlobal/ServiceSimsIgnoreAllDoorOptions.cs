using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFiltersGlobal
{
    public class ServiceSimsIgnoreAllDoorOptions : BooleanSettingOption<GameObject>, IDoorGlobalOption
    {
        protected override bool Value
        {
            get
            {
                return GoHere.Settings.mServiceSimsIgnoreAllDoorOptions;
            }
            set
            {
                GoHere.Settings.mServiceSimsIgnoreAllDoorOptions = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ServiceSimsIgnoreAllDoorOptions";
        }        

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
