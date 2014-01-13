using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDrillHole : Dereference<DrillHole>
    {
        protected override DereferenceResult Perform(DrillHole reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDriller", field, objects))
            {
                Remove(ref reference.mDriller);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mMinerExclusionJig", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mMinerExclusionJig);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
