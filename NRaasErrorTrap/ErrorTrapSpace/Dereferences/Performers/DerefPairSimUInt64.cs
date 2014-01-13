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
    public class DerefPairSimUInt64 : Dereference<Pair<Sim,ulong>>
    {
        protected override DereferenceResult Perform(Pair<Sim, ulong> reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "First", field, objects))
            {
                //Remove(ref reference.First );
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
