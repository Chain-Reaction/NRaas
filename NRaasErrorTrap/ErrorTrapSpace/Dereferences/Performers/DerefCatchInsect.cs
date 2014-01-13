using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCatchInsect : Dereference<InsectJig.CatchInsect>
    {
        protected override DereferenceResult Perform(InsectJig.CatchInsect reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDummyIkJig", field, objects))
            {
                Remove(ref reference.mDummyIkJig);

                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mTempInsect", field, objects))
            {
                Remove(ref reference.mTempInsect);

                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
