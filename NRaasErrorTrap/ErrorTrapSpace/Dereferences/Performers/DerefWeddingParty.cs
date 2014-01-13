using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefWeddingParty : Dereference<WeddingParty>
    {
        protected override DereferenceResult Perform(WeddingParty reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mEventListeners", field, objects))
            {
                if (Performing)
                {
                    Remove(reference.mEventListeners, objects);
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            if (Matches(reference, "mWeddingCandidates", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mWeddingCandidates, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
