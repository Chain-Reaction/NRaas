using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimWatchTheStars : Dereference<Sim.WatchTheStars>
    {
        protected override DereferenceResult Perform(Sim.WatchTheStars reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "SimJig", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.Cleanup();
                        }
                        catch
                        { }

                        //Remove(ref reference.SimJig);;
                    }
                }
                
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
