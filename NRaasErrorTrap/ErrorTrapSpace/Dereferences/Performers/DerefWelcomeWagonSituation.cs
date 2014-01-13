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
    public class DerefWelcomeWagonSituation : Dereference<WelcomeWagonSituation>
    {
        protected override DereferenceResult Perform(WelcomeWagonSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Wagon", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.Wagon, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
