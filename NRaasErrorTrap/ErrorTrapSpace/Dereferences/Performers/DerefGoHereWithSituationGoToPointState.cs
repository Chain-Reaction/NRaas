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
    public class DerefGoHereWithSituationGoToPointState : Dereference<GoHereWithSituation.GoToPointState>
    {
        protected override DereferenceResult Perform(GoHereWithSituation.GoToPointState reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimsRouting", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mSimsRouting, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSimsToPutInCar", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mSimsToPutInCar, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCar", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mCar);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
