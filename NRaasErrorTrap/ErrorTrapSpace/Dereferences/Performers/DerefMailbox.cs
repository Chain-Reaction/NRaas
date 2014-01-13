using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMailbox : Dereference<Mailbox>
    {
        protected override DereferenceResult Perform(Mailbox reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mInvisibleObjectList", field, objects))
            {
                Remove(reference.mInvisibleObjectList,objects);

                if (Performing)
                {
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
