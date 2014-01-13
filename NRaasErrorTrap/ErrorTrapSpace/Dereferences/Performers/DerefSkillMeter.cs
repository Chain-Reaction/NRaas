using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSkillMeterS : Dereference<SkillMeter>
    {
        protected override DereferenceResult Perform(SkillMeter reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBoneMoveHelper", field, objects))
            {
                Remove(ref reference.mBoneMoveHelper);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSim", field, objects))
            {
                Remove(ref reference.mSim);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSmc", field, objects))
            {
                // Causes script error in SkillMeter:HideMeter
                //Remove(ref reference.mSmc);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
