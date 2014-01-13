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
    public class DerefFoosballTeam : Dereference<FoosballTable.FoosballTeam>
    {
        protected override DereferenceResult Perform(FoosballTable.FoosballTeam reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "TeamMembers", field, objects))
            {
                Remove(reference.TeamMembers, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
