using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Elevator;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGameObject : Dereference<GameObject>
    {
        protected override DereferenceResult Perform(GameObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActorsUsingMe", field, objects))
            {
                Remove(reference.mActorsUsingMe, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mReferenceList", field, objects))
            {
                Remove(reference.mReferenceList, objects);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "OnFootprintChangedCallback", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (Performing)
                    {
                        GameObject.FootprintChangedDelegate callback = FindLast<GameObject.FootprintChangedDelegate>(objects);
                        if (callback != null)
                        {
                            reference.OnFootprintChangedCallback -= callback;
                        }
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mRoutingReferenceList", field, objects))
            {
                Remove(reference.mRoutingReferenceList, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "ObjectDisposed", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (Performing)
                    {
                        GameObject.ObjectDisposedEvent callback = FindLast<GameObject.ObjectDisposedEvent>(objects);
                        if (callback != null)
                        {
                            reference.ObjectDisposed -= callback;
                        }
                    }
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mObjComponents", field, objects))
            {
                // Do nothing but end the recursion
                return DereferenceResult.Ignore;
            }

            return DereferenceResult.Failure;
        }
    }
}
