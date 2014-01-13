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
    public class DerefBandInstrumentSituationJoinJammingSession : Dereference<BandInstrumentSituation.JoinJammingSession>
    {
        protected override DereferenceResult Perform(BandInstrumentSituation.JoinJammingSession reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mInstrumentToUse", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    if (result.Valid)
                    {
                        Remove(ref reference.mInstrumentToUse);
                    }
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            result = new ReferenceWrapper();
            if (Matches(reference, "mInstrumentToJoin", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    if (result.Valid)
                    {
                        Remove(ref reference.mInstrumentToJoin);
                    }
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
