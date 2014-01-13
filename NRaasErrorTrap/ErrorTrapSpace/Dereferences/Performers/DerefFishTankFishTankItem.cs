using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFishTankFishTankItem : Dereference<FishTank.FishTankItem>
    {
        protected override DereferenceResult Perform(FishTank.FishTankItem reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFishTank", field, objects))
            {
                Remove(ref reference.mFishTank);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mFish", field, objects))
            {
                //Remove(ref reference.mFish);
                if (Performing)
                {
                    return DereferenceResult.Ignore;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
