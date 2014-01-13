using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTraitListener : Dereference<TraitListener>
    {
        protected override DereferenceResult Perform(TraitListener reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSim", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Remove();
                    }
                    catch
                    { }

                    Remove(ref reference.mSim);
                }
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mListeners", field, objects))
            {
                Remove(reference.mListeners, objects);

                if (Performing)
                {
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
