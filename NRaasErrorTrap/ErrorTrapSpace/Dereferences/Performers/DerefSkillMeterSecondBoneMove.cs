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
    public class DerefSkillMeterSecondBoneMove : Dereference<SkillMeter.SecondBoneMove>
    {
        protected override DereferenceResult Perform(SkillMeter.SecondBoneMove reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mMeter", field, objects))
            {
                //Remove(ref reference.mMeter);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
