using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMoodManager : Dereference<MoodManager>
    {
        protected override DereferenceResult Perform(MoodManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Actor", field, objects))
            {
                Remove(ref reference.Actor);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
