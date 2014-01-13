using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBoardingSchoolInfo : Dereference<BoardingSchool.BoardingSchoolInfo>
    {
        protected override DereferenceResult Perform(BoardingSchool.BoardingSchoolInfo reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mInventoryItems", field, objects))
            {
                Remove(reference.mInventoryItems, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
