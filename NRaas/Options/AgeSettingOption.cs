using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public abstract class AgeSettingOption<TTarget> : ListedSettingOption<CASAgeGenderFlags, TTarget>
        where TTarget : class, IGameObject
    {
        public override string GetLocalizedValue(CASAgeGenderFlags value)
        {
            return Common.LocalizeEAString("UI/Feedback/CAS:" + value);
        }

        protected override bool Allow(CASAgeGenderFlags value)
        {
            if ((value & CASAgeGenderFlags.AgeMask) == CASAgeGenderFlags.None) return false;

            if (value == CASAgeGenderFlags.AgeMask) return false;

            return base.Allow(value);
        }

        public override bool ConvertFromString(string value, out CASAgeGenderFlags newValue)
        {
            return ParserFunctions.TryParseEnum<CASAgeGenderFlags>(value, out newValue, CASAgeGenderFlags.None);
        }

        public override string ConvertToString(CASAgeGenderFlags value)
        {
            return value.ToString();
        }
    }
}
