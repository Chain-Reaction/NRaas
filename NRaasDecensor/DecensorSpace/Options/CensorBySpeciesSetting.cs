using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DecensorSpace.Options
{
    public class CensorBySpeciesSetting : AgeSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override Proxy GetList()
        {
            return new ListProxy(Decensor.Settings.mCensorBySpecies);
        }

        public override string GetTitlePrefix()
        {
            return "CensorBySpecies";
        }

        protected override bool Allow(CASAgeGenderFlags value)
        {
            switch (value)
            {
                case CASAgeGenderFlags.Human:
                case CASAgeGenderFlags.LittleDog:
                case CASAgeGenderFlags.Dog:
                case CASAgeGenderFlags.Cat:
                case CASAgeGenderFlags.Horse:
                    return true;
            }

            return false;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
