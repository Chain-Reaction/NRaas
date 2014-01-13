using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimReadToSleepGetBook : Dereference<Sim.Sim_ReadToSleep_GetBook>
    {
        protected override DereferenceResult Perform(Sim.Sim_ReadToSleep_GetBook reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Book", field, objects))
            {
                Remove(ref reference.Book);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
