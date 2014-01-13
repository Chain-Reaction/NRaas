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
    public class DerefPoliceBreakTeenPartySituation : Dereference<PoliceBreakTeenPartySituation>
    {
        protected override DereferenceResult Perform(PoliceBreakTeenPartySituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Cop", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.Cop);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mTeen", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mTeen);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
