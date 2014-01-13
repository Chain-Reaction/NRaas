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
    public abstract class DerefPlayBandInstrument<T> : Dereference<BandInstrument.PlayBandInstrument<T>>
        where T : BandInstrument
    {
        protected override DereferenceResult Perform(BandInstrument.PlayBandInstrument<T> reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mJig", field, objects, out result) != MatchResult.Failure)
            {
                //Remove(ref reference.mJig );
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }

    public class DerefPlayPiano : DerefPlayBandInstrument<Piano>
    { }

    public class DerefPlayDrums : DerefPlayBandInstrument<Drums>
    { }

    public class DerefPlayGuitar : DerefPlayBandInstrument<Guitar>
    { }

    public class DerefPlayBassGuitar : DerefPlayBandInstrument<BassGuitar>
    { }
}
