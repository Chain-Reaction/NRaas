using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefUpgradableComponent : Dereference<UpgradableComponent>
    {
        protected override DereferenceResult Perform(UpgradableComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mUpgradeChanged", field, objects))
            {
                RemoveDelegate<UpgradableComponent.UpgradeChanged>(ref reference.mUpgradeChanged, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mHandySkill", field, objects))
            {
                Remove(ref reference.mHandySkill);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
