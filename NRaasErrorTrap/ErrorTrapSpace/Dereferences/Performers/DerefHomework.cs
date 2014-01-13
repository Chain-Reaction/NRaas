using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHomework : Dereference<Homework>
    {
        protected override DereferenceResult Perform(Homework reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "OwningSimDescription", field, objects))
            {
                if (Performing)
                {
                    ErrorTrap.AddToBeDeleted(reference, true);

                    Remove(ref reference.OwningSimDescription);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "HomeworkSoloJig", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.HomeworkSoloJig.Destroy();
                    }
                    catch
                    { }

                    Remove(ref reference.HomeworkSoloJig);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
