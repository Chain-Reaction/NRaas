using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefReactionBroadcaster : Dereference<ReactionBroadcaster>
    {
        protected override DereferenceResult Perform(ReactionBroadcaster reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOnEnterCallback", field, objects))
            {
                RemoveDelegate<ReactionBroadcaster.BroadcastCallback>(ref reference.mOnEnterCallback, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mOnExitCallback", field, objects))
            {
                RemoveDelegate<ReactionBroadcaster.BroadcastCallback>(ref reference.mOnExitCallback, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mInRadiusSims", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    /*
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }
                    */
                    if (result.Valid)
                    {
                        RemoveValues(reference.mInRadiusSims, objects);
                    }
                }
                return DereferenceResult.End;
            }

            result = new ReferenceWrapper();
            if (Matches(reference, "mSimsReacting", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    /*
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }
                    */
                    if (result.Valid)
                    {
                        RemoveKeys(reference.mSimsReacting, objects);
                        RemoveValues(reference.mSimsReacting, objects);
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mBroadcastingObject", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        // Must be prior to the nulling
                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mBroadcastingObject);
                }
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
