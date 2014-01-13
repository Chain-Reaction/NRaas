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
    public class DerefDartboard : Dereference<Dartboard>
    {
        protected override DereferenceResult Perform(Dartboard reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "CheatingSim", field, objects))
            {
                Remove(ref reference.CheatingSim);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "CurrentPlayer", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.CurrentPlayer);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "Darts", field, objects))
            {
                Remove(reference.Darts, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
