using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
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
                        // I'm not sure why this is required but it's causing a build up of JIGS. Rather than killing it, let's destroy the JIGs and see
                        // how that goes
                        foreach(KeyValuePair<Slot, RabbitHole.SlotInfo> slots in reference.mSlotToSlotInfo)
                        {
                            if (slots.Value.Footprint != null && !slots.Value.Footprint.InUse)
                            {
                                slots.Value.Footprint.Destroy();
                            }
                        }

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
