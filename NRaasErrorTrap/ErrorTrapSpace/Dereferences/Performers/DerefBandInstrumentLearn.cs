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
    public abstract class DerefBandInstrumentLearn<T> : Dereference<BandInstrument.Learn<T>>
        where T : BandInstrument
    {
        protected override DereferenceResult Perform(BandInstrument.Learn<T> reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "MusicToLearn", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }
                }

                Remove(ref reference.MusicToLearn);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }

    public class DerefPianoLearn : DerefBandInstrumentLearn<Piano>
    { }

    public class DerefDrumsLearn : DerefBandInstrumentLearn<Drums>
    { }

    public class DerefGuitarLearn : DerefBandInstrumentLearn<Guitar>
    { }

    public class DerefBassGuitarLearn : DerefBandInstrumentLearn<BassGuitar>
    { }
}
