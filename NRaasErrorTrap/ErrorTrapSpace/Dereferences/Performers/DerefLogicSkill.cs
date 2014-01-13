using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefLogicSkill : Dereference<LogicSkill>
    {
        protected override DereferenceResult Perform(LogicSkill reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mChessRankingNextChallenger", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mChessRankingNextChallenger);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mChessHighestRankingDefeatedOpponent", field, objects))
            {
                Remove(ref reference.mChessHighestRankingDefeatedOpponent);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
