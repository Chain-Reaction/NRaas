using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSingerCareer : Dereference<SingerCareer>
    {
        protected override DereferenceResult Perform(SingerCareer reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSteadyNotifier", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mSteadyNotifier.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mSteadyNotifier);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mGuitar", field, objects))
            {
                Remove(ref reference.mGuitar);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
