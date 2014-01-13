using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefInventionWorkbench : Dereference<InventionWorkbench>
    {
        protected override DereferenceResult Perform(InventionWorkbench reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDummyModel", field, objects))
            {
                Remove(ref reference.mDummyModel);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
