using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCelebrity : Dereference<Celebrity>
    {
        protected override DereferenceResult Perform(Celebrity reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mBroadcaster.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mBroadcaster);

                    if ((reference.mOwner != null) && (!reference.mOwner.HasBeenDestroyed))
                    {
                        reference.StartBroadcaster();
                    }
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mWaitingToReact", field, objects))
            {
                Remove(reference.mWaitingToReact, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mOwner", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mOwner);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
