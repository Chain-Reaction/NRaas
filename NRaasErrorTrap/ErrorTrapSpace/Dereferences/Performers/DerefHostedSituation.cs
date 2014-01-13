using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHostedSituation : Dereference<HostedSituation>
    {
        protected override DereferenceResult Perform(HostedSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Host", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    // Cannot be dereferenced due to corruption issues regarding Sim.OnLotChangedEvent
                    //Remove(ref reference.Host);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "GuestDescriptions", field, objects))
            {
                Remove(reference.GuestDescriptions, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "Guests", field, objects))
            {
                Remove(reference.Guests, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "OtherHosts", field, objects))
            {
                Remove(reference.OtherHosts, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mNumRouteFailures", field, objects))
            {
                RemoveKeys(reference.mNumRouteFailures, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimsWhoLeftEarly", field, objects))
            {
                Remove(reference.SimsWhoLeftEarly, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimsWhoLeftParty", field, objects))
            {
                Remove(reference.SimsWhoLeftParty, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
