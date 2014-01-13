using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRelationshipDictionary : Dereference<Dictionary<SimDescription,Relationship>>
    {
        protected override DereferenceResult Perform(Dictionary<SimDescription, Relationship> reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            return DereferenceResult.Continue;
        }
    }
}
