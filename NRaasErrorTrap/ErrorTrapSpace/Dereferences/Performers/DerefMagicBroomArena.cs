using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMagicBroomArena : Dereference<MagicBroomArena>
    {
        protected override DereferenceResult Perform(MagicBroomArena reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mTipJar", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mTipJar);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mBroom", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mBroom);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCurrentPerformer", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mCurrentPerformer);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCurrentRider", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mCurrentRider);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mUseBroomSim", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mUseBroomSim);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mLeavingSims", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.mLeavingSims, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
