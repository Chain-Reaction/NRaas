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
    public class DerefGrimReaperSituationReapSoul : Dereference<GrimReaperSituation.ReapSoul>
    {
        protected override DereferenceResult Perform(GrimReaperSituation.ReapSoul reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSMCDeath", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    try
                    {
                        reference.mSMCDeath.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mSMCDeath);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
