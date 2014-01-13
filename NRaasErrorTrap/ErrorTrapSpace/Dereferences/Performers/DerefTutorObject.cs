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
    public class DerefTutorObject : Dereference<TutorObject>
    {
        protected override DereferenceResult Perform(TutorObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Tutor", field, objects))
            {
                Remove(ref reference.Tutor);
                return DereferenceResult.End;
            }

            if (Matches(reference, "Student", field, objects))
            {
                Remove(ref reference.Student);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "TutorJig", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.TutorJig);
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
