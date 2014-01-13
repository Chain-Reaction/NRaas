using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPlayCatchObject : Dereference<PlayCatchObject>
    {
        protected override DereferenceResult Perform(PlayCatchObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "CurCatcher", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mCurCatcher );
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "CurThrower", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mCurThrower );
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "Player1", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mPlayer1 );
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "Player2", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mPlayer2);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
