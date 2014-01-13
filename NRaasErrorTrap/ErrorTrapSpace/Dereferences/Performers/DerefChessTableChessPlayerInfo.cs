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
    public class DerefChessTableChessPlayerInfo : Dereference<ChessTable.ChessPlayerInfo>
    {
        protected override DereferenceResult Perform(ChessTable.ChessPlayerInfo reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "GrandMaster", field, objects))
            {
                Remove(ref reference.GrandMaster);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
