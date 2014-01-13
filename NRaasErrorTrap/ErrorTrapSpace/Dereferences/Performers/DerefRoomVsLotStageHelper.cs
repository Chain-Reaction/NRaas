using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRoomVsLotStageHelper : Dereference<RoomVsLotStageHelper<IGameObject>>
    {
        protected override DereferenceResult Perform(RoomVsLotStageHelper<IGameObject> reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mObjects", field, objects))
            {
                Remove(reference.mObjects, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mIgnored", field, objects))
            {
                Remove(ref reference.mIgnored);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
