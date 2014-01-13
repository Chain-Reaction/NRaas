using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Options.General
{
    public class ShakedownRelationChangeSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mShakedownRelationChange;
            }
            set
            {
                NRaas.Careers.Settings.mShakedownRelationChange = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ShakedownRelationChange";
        }

        protected override int Validate(int value)
        {
            if (value < 0)
            {
                value = -value;
            }

            return base.Validate(value);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
