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
    public class DerefShortTermContext : Dereference<ShortTermContext>
    {
        protected override DereferenceResult Perform(ShortTermContext reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPerceivedSim", field, objects))
            {
                //Remove(ref reference.mPerceivedSim);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mAsymmetricStcProgress", field, objects))
            {
                SimDescription sim = FindLast<SimDescription>(objects);
                if (sim != null)
                foreach(KeyValuePair<CommodityTypes,Dictionary<SimDescription,float>> pair in reference.mAsymmetricStcProgress)
                {
                    pair.Value.Remove(sim);
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "SimA", field, objects))
            {
                //Remove(ref reference.SimA);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "SimB", field, objects))
            {
                //Remove(ref reference.SimB);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
