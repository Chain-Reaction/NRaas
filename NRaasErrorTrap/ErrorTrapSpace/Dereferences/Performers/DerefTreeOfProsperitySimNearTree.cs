using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTreeOfProsperitySimNearTree : DereferenceGeneric<GameObject>
    {
        protected override bool Matches(string type)
        {
            return (type == "Sims3.Store.Objects.TreeOfProsperity+SimNearTree");
        }

        protected override DereferenceResult Perform(GameObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            FieldInfo actualField;
            ReferenceWrapper result;
            if (Matches(reference, "mSim", field, objects, out result, out actualField) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(actualField, ref reference);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
