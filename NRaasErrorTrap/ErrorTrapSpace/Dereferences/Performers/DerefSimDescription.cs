using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimDescription : Dereference<SimDescription>
    {
        protected override DereferenceResult Perform(SimDescription reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mInventoryItemsWhileInPassport", field, objects))
            {
                Remove(reference.mInventoryItemsWhileInPassport, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "GameObjectRelationships", field, objects))
            {
                Remove(reference.GameObjectRelationships, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPartner", field, objects))
            {
                Remove(ref reference.mPartner);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mTraitManager", field, objects))
            {
                // For some reason trait managers tend to load with a different sim assigned to them
                reference.mTraitManager.mSimDescription = reference;
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mSim", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.CreatedSim = null;
                        }
                        catch
                        { }

                        Remove(ref reference.mSim);
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
