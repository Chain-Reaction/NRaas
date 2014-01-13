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

namespace NRaas.DresserSpace.Options.Settings.Rotation
{
    public class VacationOutfitIndexSetting : IntegerSettingOption<GameObject>, IRotationOption
    {
        protected override int Value
        {
            get
            {
                return Dresser.Settings.mVacationOutfitIndex;
            }
            set
            {
                Dresser.Settings.mVacationOutfitIndex = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "VacationOutfitIndex";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Dresser.Settings.mRotationAffectActive) return false;

            return base.Allow(parameters);
        }
    }
}
