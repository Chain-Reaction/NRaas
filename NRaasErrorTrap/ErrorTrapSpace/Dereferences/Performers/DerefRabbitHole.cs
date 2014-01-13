using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRabbitHole : Dereference<RabbitHole>
    {
        protected override DereferenceResult Perform(RabbitHole reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSlotToSlotInfo", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mSlotToSlotInfo.Clear();
                    }
                    catch
                    { }

                    try
                    {
                        if (!reference.HasBeenDestroyed)
                        {
                            reference.DeriveAndSetupDoors();
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(reference, e);
                    }
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "RabbitHoleProxy", field, objects))
            {
                Remove(ref reference.RabbitHoleProxy);
                if (Performing)
                {
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
