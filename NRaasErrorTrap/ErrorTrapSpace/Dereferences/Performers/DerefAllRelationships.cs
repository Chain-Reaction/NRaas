using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefAllRelationships : Dereference<Dictionary<SimDescription,Dictionary<SimDescription,Relationship>>>
    {
        protected override DereferenceResult Perform(Dictionary<SimDescription, Dictionary<SimDescription, Relationship>> reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (reference == Relationship.sAllRelationships)
            {
                SimDescription sim = FindLast<SimDescription>(objects);

                if (Performing)
                {
                    if (reference.ContainsKey(sim))
                    {
                        reference.Remove(sim);
                    }

                    foreach (Dictionary<SimDescription, Relationship> relations in reference.Values)
                    {
                        relations.Remove(sim);
                    }
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
