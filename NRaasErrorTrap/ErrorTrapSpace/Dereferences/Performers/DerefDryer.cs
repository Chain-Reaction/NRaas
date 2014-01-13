using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDryer : Dereference<Dryer>
    {
        protected override DereferenceResult Perform(Dryer reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDryerStateMachine", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mDryerStateMachine.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mDryerStateMachine);
                    reference.mCurDryerState = Dryer.DryerState.Empty;
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
