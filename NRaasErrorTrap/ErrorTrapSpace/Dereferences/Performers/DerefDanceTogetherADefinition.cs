using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDanceTogetherADefinition : Dereference<Stereo.DanceTogetherA.Definition>
    {
        protected override DereferenceResult Perform(Stereo.DanceTogetherA.Definition reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTargetDanceObject", field, objects))
            {
                Remove(ref reference.mTargetDanceObject);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
