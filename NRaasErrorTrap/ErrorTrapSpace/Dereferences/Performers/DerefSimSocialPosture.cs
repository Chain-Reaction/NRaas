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
    public class DerefSimSocializingPosture: Dereference<Sim.SocializingPosture>
    {
        protected override DereferenceResult Perform(Sim.SocializingPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Actor", field, objects))
            {
                Remove(ref reference.Actor);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
