using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefVisitSituationHostOutside : Dereference<VisitSituation.HostOutside>
    {
        protected override DereferenceResult Perform(VisitSituation.HostOutside reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOutdoorHost", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mOutdoorHost);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
