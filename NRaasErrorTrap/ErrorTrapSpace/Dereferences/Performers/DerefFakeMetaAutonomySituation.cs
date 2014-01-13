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
    public class DerefFakeMetaAutonomySituation : Dereference<FakeMetaAutonomySituation>
    {
        protected override DereferenceResult Perform(FakeMetaAutonomySituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Service", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.Service);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "Worker", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.Worker);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
