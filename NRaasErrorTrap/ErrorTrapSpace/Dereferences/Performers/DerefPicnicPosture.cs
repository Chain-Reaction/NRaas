using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPicnicPosture : Dereference<PicnicPosture>
    {
        protected override DereferenceResult Perform(PicnicPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Sim", field, objects))
            {
                Remove(ref reference.Sim );
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Blanket", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Sim.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.Blanket);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
