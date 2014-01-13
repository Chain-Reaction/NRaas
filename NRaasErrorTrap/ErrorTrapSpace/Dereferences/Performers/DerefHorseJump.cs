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
    public class DerefHorseJump : Dereference<HorseJump>
    {
        protected override DereferenceResult Perform(HorseJump reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mNextConnectingJump", field, objects))
            {
                Remove(ref reference.mNextConnectingJump);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPreviousConnectingJump", field, objects))
            {
                Remove(ref reference.mPreviousConnectingJump);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
