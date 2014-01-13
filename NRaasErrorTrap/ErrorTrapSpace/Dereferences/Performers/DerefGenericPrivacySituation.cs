using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGenericPrivacySituation : Dereference<GenericPrivacySituation>
    {
        protected override DereferenceResult Perform(GenericPrivacySituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "AlarmHost", field, objects))
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }

                //Remove(ref reference.AlarmHost );
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Participants", field, objects))
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }

                Remove(reference.Participants, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
