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
    public class DerefHorseshoeCourt : Dereference<HorseshoeCourt>
    {
        protected override DereferenceResult Perform(HorseshoeCourt reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPlayersWhoHaveThrown", field, objects))
            {
                Remove(reference.mPlayersWhoHaveThrown, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mScores", field, objects))
            {
                RemoveKeys(reference.mScores, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
