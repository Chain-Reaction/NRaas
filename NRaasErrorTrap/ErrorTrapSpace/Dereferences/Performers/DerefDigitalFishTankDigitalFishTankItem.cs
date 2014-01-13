using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDigitalFishTankDigitalFishTankItem : Dereference<DigitalFishTank.DigitalFishTankItem>
    {
        protected override DereferenceResult Perform(DigitalFishTank.DigitalFishTankItem reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFishTank", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mFishTank.mItems.Remove(reference);
                    }
                    catch
                    { }

                    Remove(ref reference.mFishTank);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
