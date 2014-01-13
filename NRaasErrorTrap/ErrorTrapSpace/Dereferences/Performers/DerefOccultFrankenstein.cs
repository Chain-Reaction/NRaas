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
    public class DerefOccultFrankenstein : Dereference<OccultFrankenstein>
    {
        protected override DereferenceResult Perform(OccultFrankenstein reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mOwningSim", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mOwningSim);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "ReactionBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.ReactionBroadcaster.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.ReactionBroadcaster);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
