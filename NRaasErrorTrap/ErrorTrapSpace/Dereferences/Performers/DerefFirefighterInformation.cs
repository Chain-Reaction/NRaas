using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFirefighterInformation : Dereference<Firefighter.FirefighterInformation>
    {
        protected override DereferenceResult Perform(Firefighter.FirefighterInformation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "FirefightersActiveOnLot", field, objects))
            {
                Remove(reference.FirefightersActiveOnLot,objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimsThatAreBurning", field, objects))
            {
                Remove(reference.SimsThatAreBurning, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimThatRequestedService", field, objects))
            {
                Remove(ref reference.SimThatRequestedService);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
