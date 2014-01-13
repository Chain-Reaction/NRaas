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
    public class DerefSculptingStationCreateSculpture : Dereference<SculptingStation.CreateSculpture>
    {
        protected override DereferenceResult Perform(SculptingStation.CreateSculpture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mListOfScrapInUse", field, objects))
            {
                Remove(reference.mListOfScrapInUse, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
