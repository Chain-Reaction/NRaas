using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCarRelationship : Dereference<CarRelationship>
    {
        protected override DereferenceResult Perform(CarRelationship reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimDesc", field, objects))
            {
                try
                {
                    reference.mGameObjectDesc.GameObject.ObjectDisposed -= reference.GameObjectDisposed;

                    reference.RemoveRelationship();
                }
                catch
                { }

                //Remove(ref reference.mSimDesc);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mGameObjectDesc", field, objects))
            {
                try
                {
                    reference.RemoveRelationship();
                }
                catch
                { }

                Remove(ref reference.mGameObjectDesc);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
