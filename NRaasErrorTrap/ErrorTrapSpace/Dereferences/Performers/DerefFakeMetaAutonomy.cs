using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFakeMetaAutonomy : Dereference<FakeMetaAutonomy>
    {
        protected override DereferenceResult Perform(FakeMetaAutonomy reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCreatedSims", field, objects))
            {
                RemoveValues(reference.mCreatedSims, objects);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mSimsAssignedToLots", field, objects))
            {
                if (Performing)
                {
                    foreach (List<Sim> sims in reference.mSimsAssignedToLots.Values)
                    {
                        Remove(sims, objects);
                    }
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
