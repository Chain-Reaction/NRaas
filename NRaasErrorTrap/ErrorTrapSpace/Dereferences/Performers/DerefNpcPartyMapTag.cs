using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefNpcPartyMapTag : Dereference<NpcPartyMapTag>
    {
        protected override DereferenceResult Perform(NpcPartyMapTag reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mNpcParty", field, objects))
            {
                Remove(ref reference.mNpcParty);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
