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
    public class DerefWelcomeWagonSituationPrepare : Dereference<WelcomeWagonSituation.Prepare>
    {
        protected override DereferenceResult Perform(WelcomeWagonSituation.Prepare reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFrontDoor", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch (Exception e)
                    {
                        Common.DebugException("DerefWelcomeWagonSituationPrepare", e);
                    }

                    Remove(ref reference.mFrontDoor);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
