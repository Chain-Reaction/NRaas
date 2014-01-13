using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBuffInstanceSmelly : Dereference<BuffSmelly.BuffInstanceSmelly>
    {
        protected override DereferenceResult Perform(BuffSmelly.BuffInstanceSmelly reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Irb", field, objects))
            {
                try
                {
                    reference.Irb.Dispose();
                }
                catch
                { }

                Remove(ref reference.Irb);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
