using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPicnicSituationBeAtPicnic : Dereference<PicnicSituation.BeAtPicnic>
    {
        protected override DereferenceResult Perform(PicnicSituation.BeAtPicnic reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSims", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mSims, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
