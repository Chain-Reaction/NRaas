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
    public class DerefPromSituation : Dereference<PromSituation>
    {
        protected override DereferenceResult Perform(PromSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mParticipants", field, objects))
            {
                Remove(reference.mParticipants, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mDates", field, objects))
            {
                RemoveKeys(reference.mDates, objects);
                RemoveValues(reference.mDates, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSimsAskedToSealTheDeal", field, objects))
            {
                Remove(reference.mSimsAskedToSealTheDeal, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mLocation", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mLocation);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
