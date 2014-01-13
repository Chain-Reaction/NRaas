using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefItemTestFunction : Dereference<ItemTestFunction>
    {
        protected override DereferenceResult Perform(ItemTestFunction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            GameObject gameObject = FindLast<GameObject>(objects);
            if (gameObject != null)
            {
                if (object.ReferenceEquals(gameObject.TypeCompareDelegate, reference))
                {
                    return DereferenceResult.Ignore;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
