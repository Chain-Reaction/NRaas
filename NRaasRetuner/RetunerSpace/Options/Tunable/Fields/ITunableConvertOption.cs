using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Helpers.FieldInfos;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options.Tunable.Fields
{
    public interface ITunableConvertOption : ITunableFieldOption
    {
        Type FieldType
        {
            get;
        }

        ITunableConvertOption Clone(TunableFieldInfo field);

        object Convert(string value, bool fireError);
    }
}
