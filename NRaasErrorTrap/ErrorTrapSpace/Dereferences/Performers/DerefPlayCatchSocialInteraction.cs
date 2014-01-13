using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPlayCatchSocialInteraction : Dereference<PlayCatchSocialInteraction>
    {
        protected override DereferenceResult Perform(PlayCatchSocialInteraction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Ball", field, objects))
            {
                //Remove(ref reference.Ball );
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }

    public class DerefPlayCatchSocialInteractionDefinition : Dereference<PlayCatchSocialInteraction.Definition>
    {
        protected override DereferenceResult Perform(PlayCatchSocialInteraction.Definition reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Ball", field, objects))
            {
                Remove(ref reference.Ball);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
