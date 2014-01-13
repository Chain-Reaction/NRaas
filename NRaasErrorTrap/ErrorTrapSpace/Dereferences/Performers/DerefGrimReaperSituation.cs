using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGrimReaperSituation : Dereference<GrimReaperSituation>
    {
        protected override DereferenceResult Perform(GrimReaperSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SMCDeath", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.SMCDeath.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.SMCDeath);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
