using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefStateMachineClient : Dereference<StateMachineClient>
    {
        protected override DereferenceResult Perform(StateMachineClient reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mEventHandlers", field, objects))
            {
                try
                {
                    reference.Dispose();
                }
                catch
                { }

                Remove(reference.mEventHandlers, objects);

                return DereferenceResult.ContinueIfReferenced;
            }


            if (Matches(reference, "mActors", field, objects))
            {
                try
                {
                    reference.Dispose();
                }
                catch
                { }

                RemoveValues(reference.mActors, objects);

                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mVirtualAddRefs", field, objects))
            {
                try
                {
                    reference.Dispose();
                }
                catch
                { }

                RemoveValues(reference.mVirtualAddRefs, objects);

                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
