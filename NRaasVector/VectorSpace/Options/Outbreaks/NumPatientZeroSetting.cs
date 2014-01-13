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

namespace NRaas.VectorSpace.Options.Outbreaks
{
    public class NumPatientZeroSetting : IntegerSettingOption<GameObject>, IOutbreaksOption
    {
        protected override int Value
        {
            get
            {
                return Vector.Settings.mNumPatientZero;
            }
            set
            {
                Vector.Settings.mNumPatientZero = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "NumPatientZero";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
