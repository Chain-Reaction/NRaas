using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefJournalism : Dereference<Journalism>
    {
        protected override DereferenceResult Perform(Journalism reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SimsInterviewed", field, objects))
            {
                Remove(reference.SimsInterviewed, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "StorySubject", field, objects))
            {
                Remove(ref reference.StorySubject);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
