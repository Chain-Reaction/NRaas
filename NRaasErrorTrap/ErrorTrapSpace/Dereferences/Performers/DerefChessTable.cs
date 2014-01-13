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
    public class DerefChessTable : Dereference<ChessTable>
    {
        protected override DereferenceResult Perform(ChessTable reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mChessPlayers", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        // Doing so will reset the mChessPlayers list
                        reference.SetObjectToReset();
                    }
                    catch
                    { }

                    //Remove(reference.mChessPlayers, objects);
                }
                return DereferenceResult.Ignore;
            }

            if (Matches(reference, "mSimsPlayingForChessRank", field, objects))
            {
                Remove(reference.mSimsPlayingForChessRank, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSimThatJustWon", field, objects))
            {
                Remove(ref reference.mSimThatJustWon);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
