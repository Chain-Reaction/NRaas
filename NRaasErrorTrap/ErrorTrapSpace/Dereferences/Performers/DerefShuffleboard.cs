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
    public class DerefShuffleboard : Dereference<Shuffleboard>
    {
        protected override DereferenceResult Perform(Shuffleboard reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTeams", field, objects))
            {
                Remove(reference.mTeams, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCurrentPlayer", field, objects))
            {
                Remove(ref reference.mCurrentPlayer);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPucks", field, objects))
            {
                Remove(reference.mPucks,objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
