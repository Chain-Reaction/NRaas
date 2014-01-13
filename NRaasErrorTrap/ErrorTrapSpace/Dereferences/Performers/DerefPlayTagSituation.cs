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
    public class DerefPlayTagSituation : Dereference<PlayTagSituation>
    {
        protected override DereferenceResult Perform(PlayTagSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Players", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    RemoveKeys(reference.Players, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mHunter", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mHunter);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPrey", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mPrey);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
