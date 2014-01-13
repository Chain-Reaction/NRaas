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
    public class DerefHangWithCoworkersTone : Dereference<HangWithCoworkersTone>
    {
        protected override DereferenceResult Perform(HangWithCoworkersTone reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimsInConv", field, objects))
            {
                Remove(reference.mSimsInConv,objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
