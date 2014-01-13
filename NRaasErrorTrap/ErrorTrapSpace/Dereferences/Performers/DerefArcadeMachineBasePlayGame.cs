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
    public class DerefArcadeMachineBasePlayGame : Dereference<ArcadeMachineBase.PlayGame>
    {
        protected override DereferenceResult Perform(ArcadeMachineBase.PlayGame reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mStool", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    Remove(ref reference.mStool);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
