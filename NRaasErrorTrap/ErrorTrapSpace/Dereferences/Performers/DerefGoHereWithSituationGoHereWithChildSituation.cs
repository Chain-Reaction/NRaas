using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGoHereWithSituationGoHereWithChildSituation : Dereference<GoHereWithSituation.GoHereWithChildSituation>
    {
        protected override DereferenceResult Perform(GoHereWithSituation.GoHereWithChildSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPushedInteractions", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    RemoveKeys(reference.mPushedInteractions, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
