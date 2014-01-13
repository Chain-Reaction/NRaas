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
    public class DerefHouseholdMembers : Dereference<Household.Members>
    {
        protected override DereferenceResult Perform(Household.Members reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimDescriptions", field, objects))
            {
                //Remove(reference.mSimDescriptions, objects);
                if (Performing)
                {
                    return DereferenceResult.Ignore;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            if (Matches(reference, "mPetSimDescriptions", field, objects))
            {
                //Remove(reference.mPetSimDescriptions, objects);
                if (Performing)
                {
                    return DereferenceResult.Ignore;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            if (Matches(reference, "mAllSimDescriptions", field, objects))
            {
                //Remove(reference.mSimDescriptions, objects);
                if (Performing)
                {
                    return DereferenceResult.Ignore;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
