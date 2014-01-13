using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMagicWandCastSpell : Dereference<MagicWand.CastSpell>
    {
        protected override DereferenceResult Perform(MagicWand.CastSpell reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mWand", field, objects))
            {
                try
                {
                    reference.Cleanup();
                }
                catch
                { }

                Remove(ref reference.mWand);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
