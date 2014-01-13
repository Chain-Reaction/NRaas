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
    public class DerefConversation : Dereference<Conversation>
    {
        protected override DereferenceResult Perform(Conversation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mWhoseTurn", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mWhoseTurn);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mDiscourageRegion", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    //Remove(ref reference.mDiscourageRegion);
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mActiveTopics", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.mActiveTopics, objects);
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mWhoTalkedLast", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mWhoTalkedLast);
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mMembers", field, objects))
            {
                Remove(reference.mMembers, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mTopicalSocialsAlreadyUsed", field, objects))
            {
                RemoveKeys(reference.mTopicalSocialsAlreadyUsed, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
