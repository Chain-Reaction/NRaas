using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMailCarrier : Dereference<MailCarrier>
    {
        protected override DereferenceResult Perform(MailCarrier reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDogChaser", field, objects))
            {
                Remove(ref reference.mDogChaser);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
