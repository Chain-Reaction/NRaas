using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPlayInSandJig : Dereference<PlayInSandJig>
    {
        protected override DereferenceResult Perform(PlayInSandJig reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Part", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Destroy();
                    }
                    catch
                    { }

                    Remove(ref reference.Part);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Sculpture", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Destroy();
                    }
                    catch
                    { }

                    Remove(ref reference.Sculpture);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
