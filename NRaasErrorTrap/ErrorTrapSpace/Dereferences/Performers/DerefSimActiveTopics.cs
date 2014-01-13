using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimActiveTopics : Dereference<SimActiveTopics>
    {
        protected override DereferenceResult Perform(SimActiveTopics reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mForgettableTopics", field, objects))
            {
                ActiveTopic topic = FindLast<ActiveTopic>(objects);
                if (topic != null)
                {
                    if (Performing)
                    {
                        reference.mForgettableTopics.Remove(topic);
                    }
                    return DereferenceResult.End;
                }
            }

            if (Matches(reference, "mUnforgettableTopics", field, objects))
            {
                Remove(reference.mUnforgettableTopics, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mActor", field, objects))
            {
                Remove(ref reference.mActor);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mTraits", field, objects))
            {
                Remove(reference.mTraits, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
