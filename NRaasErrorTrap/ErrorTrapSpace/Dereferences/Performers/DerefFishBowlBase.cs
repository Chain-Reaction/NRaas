using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFishBowlBase : Dereference<FishBowlBase>
    {
        protected override DereferenceResult Perform(FishBowlBase reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFishInBowl", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mFishInBowl);
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            if (Matches(reference, "mFeedFishAutonomouslyBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mFeedFishAutonomouslyBroadcaster.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mFeedFishAutonomouslyBroadcaster);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
