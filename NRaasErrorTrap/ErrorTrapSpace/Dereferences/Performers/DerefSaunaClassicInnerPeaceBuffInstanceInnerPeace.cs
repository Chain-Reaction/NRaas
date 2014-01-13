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
    public class DerefSaunaClassicInnerPeaceBuffInstanceInnerPeace : DereferenceGeneric<BuffInstance>
    {
        protected override bool Matches(string type)
        {
            return (type == "Sims3.Store.Objects.SaunaClassic+InnerPeace+BuffInstanceInnerPeace");
        }

        protected override DereferenceResult Perform(BuffInstance reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            FieldInfo actualField;
            ReferenceWrapper result;
            if (Matches(reference, "mAffectedSim", field, objects, out result, out actualField) != MatchResult.Failure)
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
