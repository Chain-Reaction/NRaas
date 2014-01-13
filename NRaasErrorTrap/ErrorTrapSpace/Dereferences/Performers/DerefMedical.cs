using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMedical : Dereference<Medical>
    {
        protected override DereferenceResult Perform(Medical reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mHouseCallSim", field, objects))
            {
                Remove(ref reference.mHouseCallSim );
                return DereferenceResult.End;
            }

            if (Matches(reference, "mBeeper", field, objects))
            {
                Remove(ref reference.mBeeper );
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mJournal", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mJournal);
                }

                return DereferenceResult.End;
            }

            DereferenceResult reason = MatchAndRemove(reference, "mNewLotListener", field, ref reference.mNewLotListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            return DereferenceResult.Failure;
        }
    }
}
