using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSnowWaterFight : Dereference<SnowWaterFight>
    {
        protected override DereferenceResult Perform(SnowWaterFight reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "LockedPlayers", field, objects))
            {
                Remove(reference.LockedPlayers, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "PlayerSlots", field, objects))
            {
                Remove(reference.PlayerSlots, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "PlayersWhoShouldMove", field, objects))
            {
                RemoveKeys(reference.PlayersWhoShouldMove, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
