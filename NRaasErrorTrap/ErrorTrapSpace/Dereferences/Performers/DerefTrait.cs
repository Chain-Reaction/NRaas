using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTrait : Dereference<Trait>
    {
        protected override DereferenceResult Perform(Trait reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "TraitListener", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.TraitListener.Remove();
                        }
                        catch
                        { }

                        Remove(ref reference.TraitListener);
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
