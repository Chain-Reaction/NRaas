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
    public class DerefPoolTable : Dereference<PoolTable>
    {
        protected override DereferenceResult Perform(PoolTable reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mCurrentShooter", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mCurrentShooter);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mExpectedPlayers", field, objects))
            {
                Remove(reference.mExpectedPlayers, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPlayerIdling", field, objects))
            {
                RemoveKeys(reference.mPlayerIdling, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mWatchers", field, objects))
            {
                RemoveKeys(reference.mWatchers, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mShootingJig", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mShootingJig);
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
