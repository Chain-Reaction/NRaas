using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRolePaparazziFollowCelebrity : Dereference<RolePaparazzi.FollowCelebrity>
    {
        protected override DereferenceResult Perform(RolePaparazzi.FollowCelebrity reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mJig", field, objects))
            {
                //Remove(ref reference.mJig);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
