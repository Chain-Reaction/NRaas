using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefInformationLearnedAboutSim : Dereference<InformationLearnedAboutSim>
    {
        protected override DereferenceResult Perform(InformationLearnedAboutSim reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mLearner", field, objects))
            {
                //Remove(ref reference.mLearner);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
