using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHousePartyHappening : Dereference<HouseParty.Happening>
    {
        protected override DereferenceResult Perform(HouseParty.Happening reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "partyMotives", field, objects))
            {
                RemoveKeys(reference.partyMotives, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
