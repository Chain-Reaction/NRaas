using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefLotSavedData : Dereference<Lot.SavedData>
    {
        protected override DereferenceResult Perform(Lot.SavedData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDoorsToVirtualResidentDictionary", field, objects))
            {
                RemoveValues(reference.mDoorsToVirtualResidentDictionary, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mBroadcastersWithSims", field, objects))
            {
                Remove(reference.mBroadcastersWithSims, objects);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mLastDiedSim", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mLastDiedSim);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mReactions", field, objects))
            {
                Remove(reference.mReactions, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mProprietor", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mProprietor);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
