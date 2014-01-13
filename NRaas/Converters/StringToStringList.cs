using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Converters
{
    public class StringToStringList : StringToList<string>
    {
        protected override bool PrivateConvert(string value, out string result)
        {
            result = value;
            return true;
        }
    }
}
