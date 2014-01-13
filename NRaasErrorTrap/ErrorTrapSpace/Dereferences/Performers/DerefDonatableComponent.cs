using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDonatableComponent : Dereference<DonatableComponent>
    {
        protected override DereferenceResult Perform(DonatableComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mNumberOfSpotsLeftCallback", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (Performing)
                    {
                        DonatableComponent.NumberOfSpotsLeftCallback callback = FindLast<DonatableComponent.NumberOfSpotsLeftCallback>(objects);
                        if (callback != null)
                        {
                            reference.mNumberOfSpotsLeftCallback -= callback;
                        }
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mIsObjectDonatableCallback", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (Performing)
                    {
                        DonatableComponent.IsObjectDonatableCallback callback = FindLast<DonatableComponent.IsObjectDonatableCallback>(objects);
                        if (callback != null)
                        {
                            reference.mIsObjectDonatableCallback -= callback;
                        }
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "OnReservationChanged", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (Performing)
                    {
                        DonatableComponent.OnReservationChangedHandler callback = FindLast<DonatableComponent.OnReservationChangedHandler>(objects);
                        if (callback != null)
                        {
                            reference.OnReservationChanged -= callback;
                        }
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
