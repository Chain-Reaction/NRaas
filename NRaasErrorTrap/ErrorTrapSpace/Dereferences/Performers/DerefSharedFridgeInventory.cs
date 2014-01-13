using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSharedFridgeInventory : Dereference<SharedFridgeInventory>
    {
        protected override DereferenceResult Perform(SharedFridgeInventory reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SpoiledFood", field, objects))
            {
                Remove(reference.SpoiledFood, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mLastOpenedFridge", field, objects))
            {
                Remove(ref reference.mLastOpenedFridge);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCurrOpenedFridge", field, objects))
            {
                Remove(ref reference.mCurrOpenedFridge);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
