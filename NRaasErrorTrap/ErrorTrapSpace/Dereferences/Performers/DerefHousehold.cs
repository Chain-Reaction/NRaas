using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHousehold : Dereference<Household>
    {
        protected override DereferenceResult Perform(Household reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mSharedFamilyInventory", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        reference.mSharedFamilyInventory = SharedFamilyInventory.Create(reference);
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSharedFridgeInventory", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        reference.mSharedFridgeInventory = SharedFridgeInventory.Create(reference);
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "ComeAndSeeOutOfWorldObjects", field, objects))
            {
                Remove(reference.ComeAndSeeOutOfWorldObjects, objects);

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
