using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Converters
{
    public class StringToSpeciesList : StringToList<CASAgeGenderFlags>
    {
        public string mError;

        protected override bool PrivateConvert(string value, out CASAgeGenderFlags result)
        {
            if (ParserFunctions.TryParseEnum<CASAgeGenderFlags>(value, out result, CASAgeGenderFlags.None)) return true;

            mError = "Unknown Species " + value;
            return false;
        }
    }
}
