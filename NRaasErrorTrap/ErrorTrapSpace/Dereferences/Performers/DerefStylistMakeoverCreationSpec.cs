using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefStylistMakeoverCreationSpec : Dereference<Stylist.MakeoverCreationSpec>
    {
        protected override DereferenceResult Perform(Stylist.MakeoverCreationSpec reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Client", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.Client);

                    if (Stylist.sPendingJobData != null)
                    {
                        Stylist.sPendingJobData.Remove(reference);
                    }
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
