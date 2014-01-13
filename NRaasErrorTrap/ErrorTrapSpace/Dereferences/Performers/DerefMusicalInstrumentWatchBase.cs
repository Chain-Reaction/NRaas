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
    public abstract class DerefMusicalInstrumentWatchBase<T> : Dereference<MusicalInstrument.WatchBase<T>>
        where T : MusicalInstrument
    {
        protected override DereferenceResult Perform(MusicalInstrument.WatchBase<T> reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPlayInstance", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    Remove(ref reference.mPlayInstance);
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mPlayer", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    Remove(ref reference.mPlayer);
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
