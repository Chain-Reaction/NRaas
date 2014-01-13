using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBurglarSituation : Dereference<BurglarSituation>
    {
        protected override DereferenceResult Perform(BurglarSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult result = DereferenceResult.Failure;

            if (Matches(reference, "IgnoreObjects", field, objects))
            {
                Remove(reference.IgnoreObjects, objects);
                result = DereferenceResult.Continue;
            }

            if (Matches(reference, "StolenObjects", field, objects))
            {
                Remove(reference.StolenObjects, objects);
                result = DereferenceResult.Continue;
            }

            if (Matches(reference, "mReactingSims", field, objects))
            {
                Remove(reference.mReactingSims, objects);
                result = DereferenceResult.Continue;
            }

            if (result != DereferenceResult.Failure)
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }
            }

            return result;
        }
    }
}
