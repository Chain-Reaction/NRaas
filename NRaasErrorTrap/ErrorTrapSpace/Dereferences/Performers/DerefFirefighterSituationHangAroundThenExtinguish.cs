using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFirefighterSituationHangAroundThenExtinguish : Dereference<FirefighterSituation.HangAroundThenExtinguish>
    {
        protected override DereferenceResult Perform(FirefighterSituation.HangAroundThenExtinguish reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBurningObject", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mBurningObject);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
