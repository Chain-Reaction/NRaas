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
    public class DerefRenovationCreationSpec : Dereference<InteriorDesigner.RenovationCreationSpec>
    {
        protected override DereferenceResult Perform(InteriorDesigner.RenovationCreationSpec reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "PrimaryClient", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.PrimaryClient);

                    if (InteriorDesigner.sPendingJobData != null)
                    {
                        InteriorDesigner.sPendingJobData.Remove(reference);
                    }
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "SecondaryClient", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.SecondaryClient);

                    if (InteriorDesigner.sPendingJobData != null)
                    {
                        InteriorDesigner.sPendingJobData.Remove(reference);
                    }
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
