using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBuffInstanceBeeAttack : Dereference<BuffBeeAttack.BuffInstanceBeeAttack>
    {
        protected override DereferenceResult Perform(BuffBeeAttack.BuffInstanceBeeAttack reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "BeingAttackedAlarmDelegate", field, objects))
            {
                RemoveDelegate<BuffBeeAttack.BeeAttackAlarmDelegate>(ref reference.BeingAttackedAlarmDelegate, objects);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
