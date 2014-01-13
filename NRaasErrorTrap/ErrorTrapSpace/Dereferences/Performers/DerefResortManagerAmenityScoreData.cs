using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefResortManagerAmenityScoreData : Dereference<ResortManager.AmenityScoreData>
    {
        protected override DereferenceResult Perform(ResortManager.AmenityScoreData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "ScoreObjects", field, objects))
            {
                foreach (KeyValuePair<ResortObjectType, Dictionary<IResortScoreObject, float>> value in reference.ScoreObjects)
                {
                    RemoveKeys(value.Value, objects);
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
