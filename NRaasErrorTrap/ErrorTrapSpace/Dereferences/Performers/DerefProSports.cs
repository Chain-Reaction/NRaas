using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefProSports : Dereference<ProSports>
    {
        protected override DereferenceResult Perform(ProSports reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBooReactionBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mBooReactionBroadcaster.Dispose();
                    }
                    catch
                    { }
                }

                Remove(ref reference.mBooReactionBroadcaster);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
