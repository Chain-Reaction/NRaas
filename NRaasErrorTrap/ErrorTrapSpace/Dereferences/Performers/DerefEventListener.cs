using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefEventListener : Dereference<EventListener>
    {
        protected override DereferenceResult Perform(EventListener reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCompletionEvent", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        EventTracker.RemoveListener(reference);
                    }
                    catch
                    { }
                }

                Remove(ref reference.mCompletionEvent);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mScriptObject", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        EventTracker.RemoveListener(reference);
                    }
                    catch
                    { }
                }

                Remove(ref reference.mScriptObject);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mTargetObject", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        EventTracker.RemoveListener(reference);
                    }
                    catch
                    { }
                }

                Remove(ref reference.mTargetObject);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
