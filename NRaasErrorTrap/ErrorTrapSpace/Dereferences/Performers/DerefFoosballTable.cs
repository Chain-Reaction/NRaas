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
    public class DerefFoosballTable : Dereference<FoosballTable>
    {
        protected override DereferenceResult Perform(FoosballTable reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "TeamA", field, objects))
            {
                if (Performing)
                {
                    reference.TeamA = new FoosballTable.FoosballTeam("TeamA");
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "TeamB", field, objects))
            {
                if (Performing)
                {
                    reference.TeamB = new FoosballTable.FoosballTeam("TeamB");
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "Players", field, objects))
            {
                Remove(reference.Players,objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
