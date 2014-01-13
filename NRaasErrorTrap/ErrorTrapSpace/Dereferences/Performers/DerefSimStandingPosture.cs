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
    public class DerefSimStandingPosture : Dereference<Sim.StandingPosture>
    {
        protected override DereferenceResult Perform(Sim.StandingPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActor", field, objects))
            {
                // Cannot be dereferenced due to issue with DoReset()
                //Remove(ref reference.mActor);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
