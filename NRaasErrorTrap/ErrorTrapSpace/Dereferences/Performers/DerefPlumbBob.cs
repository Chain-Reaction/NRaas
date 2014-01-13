using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPlumbBob : Dereference<PlumbBob>
    {
        protected override DereferenceResult Perform(PlumbBob reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;

            if (Matches(reference, "mSelectedActor", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        Remove(ref reference.mSelectedActor);

                        if (PlumbBob.Singleton == reference)
                        {
                            GameStates.TransitionToEditTown();
                        }
                    }
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
