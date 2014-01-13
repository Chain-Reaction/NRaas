using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefInteractionQueue : Dereference<InteractionQueue>
    {
        protected override DereferenceResult Perform(InteractionQueue reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCurrentTransitionInteraction", field, objects))
            {
                if (Performing)
                {
                    //reference.CancelAllInteractions();
                }
                return DereferenceResult.Ignore;
            }

            if (Matches(reference, "mRunningInteractions", field, objects))
            {
                if (Performing)
                {
                    //reference.CancelAllInteractions();
                }
                return DereferenceResult.Ignore;
            }

            if (Matches(reference, "mInteractionList", field, objects))
            {
                if (Performing)
                {
                    //reference.CancelAllInteractions();
                }
                return DereferenceResult.Ignore;
            }

            if (Matches(reference, "mActor", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mActor);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
