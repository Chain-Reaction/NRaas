using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBotShopRegisterConsignedObject : Dereference<BotShopRegister.ConsignedObject>
    {
        protected override DereferenceResult Perform(BotShopRegister.ConsignedObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mObject", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mObject);

                    foreach (List<BotShopRegister.ConsignedObject> list in BotShopRegister.sConsignedObjects.Values)
                    {
                        if (list.Remove(reference)) break;
                    }

                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
