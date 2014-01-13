using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTrendsetterRecordData : Dereference<TrendsetterRecordData>
    {
        protected override DereferenceResult Perform(TrendsetterRecordData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult reason = MatchAndRemove(reference, "mBoughtOutfitListener", field, ref reference.mBoughtOutfitListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            return DereferenceResult.Failure;
        }
    }
}
