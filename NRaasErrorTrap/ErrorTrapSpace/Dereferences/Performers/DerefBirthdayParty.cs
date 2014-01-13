using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBirthdayParty : Dereference<BirthdayParty>
    {
        protected override DereferenceResult Perform(BirthdayParty reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mEventListeners", field, objects))
            {
                if (Performing)
                {
                    Remove(reference.mEventListeners, objects);
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
