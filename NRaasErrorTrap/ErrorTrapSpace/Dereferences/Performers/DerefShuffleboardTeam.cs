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
    public class DerefShuffleboardTeam : Dereference<Shuffleboard.Team>
    {
        protected override DereferenceResult Perform(Shuffleboard.Team reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "CurrentPlayer", field, objects))
            {
                Remove(ref reference.CurrentPlayer);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Sims", field, objects))
            {
                Remove(reference.Sims, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
