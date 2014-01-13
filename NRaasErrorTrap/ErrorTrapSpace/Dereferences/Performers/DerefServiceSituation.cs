using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefServiceSituation : Dereference<ServiceSituation>
    {
        protected override DereferenceResult Perform(ServiceSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Worker", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.Worker);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCar", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mCar);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
