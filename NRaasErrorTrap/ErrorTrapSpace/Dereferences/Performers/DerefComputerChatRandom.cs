using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefComputerChatRandom : Dereference<Computer.ChatRandom>
    {
        protected override DereferenceResult Perform(Computer.ChatRandom reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "simToChat", field, objects))
            {
                Remove(ref reference.simToChat);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
