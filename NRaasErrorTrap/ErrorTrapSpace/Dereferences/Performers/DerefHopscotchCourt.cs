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
    public class DerefHopscotchCourt : Dereference<HopscotchCourt>
    {
        protected override DereferenceResult Perform(HopscotchCourt reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPlayerQueue", field, objects))
            {
                if (Performing)
                {
                    reference.mPlayerQueue.Clear();
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mWinningPlayers", field, objects))
            {
                Remove(reference.mWinningPlayers, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPlayersWhoHaveHopped", field, objects))
            {
                Remove(reference.mPlayersWhoHaveHopped, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
