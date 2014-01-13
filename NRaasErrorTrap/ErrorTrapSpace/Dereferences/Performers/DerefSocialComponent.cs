using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSocialComponent : Dereference<SocialComponent>
    {
        protected override DereferenceResult Perform(SocialComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSim", field, objects))
            {
                Remove(ref reference.mSim);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mActiveTopics", field, objects))
            {
                //Remove(ref reference.mActiveTopics);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mShortTermDesireToSocializeWith", field, objects))
            {
                RemoveKeys(reference.mShortTermDesireToSocializeWith, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPlayerIntendedSocialCommodityTypes", field, objects))
            {
                RemoveKeys(reference.mPlayerIntendedSocialCommodityTypes, objects);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mLastTalkingTo", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mLastTalkingTo);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
