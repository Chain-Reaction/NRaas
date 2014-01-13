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
    public class DerefBuffManager : Dereference<BuffManager>
    {
        protected override DereferenceResult Perform(BuffManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mValues", field, objects))
            {
                if (Performing)
                {
                    BuffInstance buff = Find<BuffInstance>(objects);
                    if (buff != null)
                    {
                        try
                        {
                            reference.RemoveElement(buff.Guid);
                        }
                        catch
                        { }
                    }

                    RemoveValues(reference.mValues, objects);
                }
                return DereferenceResult.End;
            }

            ReferenceWrapper result;

            if (Matches(reference, "mRecentBuff", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mRecentBuff);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mActor", field, objects))
            {
                Remove(ref reference.mActor);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
