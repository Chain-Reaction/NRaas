using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefShuffleboardPlay : Dereference<Shuffleboard.Play>
    {
        protected override DereferenceResult Perform(Shuffleboard.Play reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPuck", field, objects))
            {
                Remove(ref reference.mPuck);
                return DereferenceResult.ContinueIfReferenced;
            }

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
                }

                Remove(ref reference.mBroadcaster);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mSecondaryPuck", field, objects))
            {
                Remove(ref reference.mSecondaryPuck);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
