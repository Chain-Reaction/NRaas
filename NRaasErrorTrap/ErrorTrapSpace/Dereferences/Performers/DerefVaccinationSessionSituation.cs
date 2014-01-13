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
    public class DerefVaccinationSessionSituation : Dereference<VaccinationSessionSituation>
    {
        protected override DereferenceResult Perform(VaccinationSessionSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult reason = MatchAndRemove(reference, "mLeaveConversationListener", field, ref reference.mLeaveConversationListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            if (Matches(reference, "Vaccinator", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.Vaccinator);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mIgnoreList", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(reference.mIgnoreList, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mInterruptedVaccinationSeekers", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    RemoveKeys(reference.mInterruptedVaccinationSeekers, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
