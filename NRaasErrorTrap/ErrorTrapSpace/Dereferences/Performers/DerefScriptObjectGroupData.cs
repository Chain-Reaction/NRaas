using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefScriptObjectGroupData : Dereference<ScriptCore.ScriptObjectGroupData>
    {
        protected override DereferenceResult Perform(ScriptCore.ScriptObjectGroupData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mObjects", field, objects))
            {
                Remove(reference.mObjects, objects);

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
