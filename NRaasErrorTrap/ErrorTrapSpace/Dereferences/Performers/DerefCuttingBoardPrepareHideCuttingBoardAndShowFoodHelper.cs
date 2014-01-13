using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCuttingBoardPrepareHideCuttingBoardAndShowFoodHelper : Dereference<CuttingBoard_Prepare.HideCuttingBoardAndShowFoodHelper>
    {
        protected override DereferenceResult Perform(CuttingBoard_Prepare.HideCuttingBoardAndShowFoodHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCuttingBoard", field, objects))
            {
                Remove(ref reference.mCuttingBoard);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
