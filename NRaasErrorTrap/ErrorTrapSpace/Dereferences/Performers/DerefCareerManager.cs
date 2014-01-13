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
    public class DerefCareerManager : Dereference<CareerManager>
    {
        protected override DereferenceResult Perform(CareerManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBossOf", field, objects))
            {
                if (reference.mBossOf != null)
                {
                    RemoveKeys(reference.mBossOf, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSimDescription", field, objects))
            {
                //Remove(ref reference.mSimDescription);
                return DereferenceResult.Ignore;
            }

            return DereferenceResult.Failure;
        }
    }
}
