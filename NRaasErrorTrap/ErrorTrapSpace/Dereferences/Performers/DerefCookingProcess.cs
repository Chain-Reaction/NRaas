using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCookingProcess : Dereference<CookingProcess>
    {
        protected override DereferenceResult Perform(CookingProcess reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (reference.ObjectClickedOn != null)
            {
                if (Performing)
                {
                    if (reference.ObjectClickedOn.ObjectDisposed != null)
                    {
                        reference.ObjectClickedOn.ObjectDisposed -= reference.ObjectClickedOnDestroyed;
                    }
                }
                Remove(ref reference.ObjectClickedOn);
            }

            DereferenceResult reason = MatchAndRemove(reference, "mStateChangedListener", field, ref reference.mStateChangedListener, objects, DereferenceResult.ContinueIfReferenced);
            if (reason != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    try
                    {
                        reference.Owner.CookingProcess = null;
                    }
                    catch
                    { }
                }
                return reason;
            }

            if (Matches(reference, "ObjectClickedOn", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Owner.CookingProcess = null;
                    }
                    catch
                    { }
                }

                //Remove(ref reference.ObjectClickedOn);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Owner", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Owner.CookingProcess = null;
                    }
                    catch
                    { }
                }

                Remove(ref reference.Owner);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
