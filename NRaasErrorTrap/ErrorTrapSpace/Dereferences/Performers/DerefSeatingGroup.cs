using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSeatingGroup : Dereference<SeatingGroup>
    {
        protected override DereferenceResult Perform(SeatingGroup reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSeats", field, objects))
            {
                Remove(reference.mSeats, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
