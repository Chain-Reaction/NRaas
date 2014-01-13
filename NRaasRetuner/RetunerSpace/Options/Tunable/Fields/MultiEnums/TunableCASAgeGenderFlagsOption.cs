using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Helpers.FieldInfos;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options.Tunable.Fields.MultiEnums
{
    public class TunableCASAgeGenderFlagsOption : TunableMultiEnumOption<CASAgeGenderFlags>
    {
        public TunableCASAgeGenderFlagsOption()
        { }
        public TunableCASAgeGenderFlagsOption(TunableFieldInfo field)
            : base(field)
        { }

        public override string GetTitlePrefix()
        {
            return "TunableCASAgeGenderFlags";
        }

        protected override void PrivatePerform(List<CASAgeGenderFlags> values)
        {
            Value = CASAgeGenderFlags.None;

            foreach (CASAgeGenderFlags value in values)
            {
                Value |= value;
            }
        }

        public override ITunableConvertOption Clone(TunableFieldInfo field)
        {
            return new TunableCASAgeGenderFlagsOption(field);
        }
    }
}
