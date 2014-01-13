using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefActorTrailerWooHoo : Dereference<ActorTrailer.WooHoo>
    {
        protected override DereferenceResult Perform(ActorTrailer.WooHoo reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "WooHooer", field, objects))
            {
                try
                {
                    reference.Cleanup();
                }
                catch
                { }

                Remove(ref reference.WooHooer);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "WooHooee", field, objects))
            {
                try
                {
                    reference.Cleanup();
                }
                catch
                { }

                Remove(ref reference.WooHooee);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
