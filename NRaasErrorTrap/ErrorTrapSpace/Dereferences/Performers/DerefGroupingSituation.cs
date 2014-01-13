using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGroupingSituation : Dereference<GroupingSituation>
    {
        protected override DereferenceResult Perform(GroupingSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;

            if (Matches(reference, "mLeader", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.Exit();
                        }
                        catch
                        { }

                        //Remove(ref reference.mLeader );
                    }
                }
                return DereferenceResult.Ignore;
            }

            if (Matches(reference, "mParticipants", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    RemoveKeys(reference.mParticipants, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
