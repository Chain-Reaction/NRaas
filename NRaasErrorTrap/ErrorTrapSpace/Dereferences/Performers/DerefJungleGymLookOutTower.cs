using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefJungleGymLookOutTower : Dereference<JungleGymLookOutTower>
    {
        protected override DereferenceResult Perform(JungleGymLookOutTower reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSlotContended", field, objects))
            {
                Remove(reference.mSlotContended,objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSlotOccupied", field, objects))
            {
                Remove(reference.mSlotOccupied, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
