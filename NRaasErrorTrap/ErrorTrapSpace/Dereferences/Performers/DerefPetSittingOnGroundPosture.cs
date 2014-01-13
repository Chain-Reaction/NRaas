using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPetSittingOnGroundPosture : Dereference<PetSittingOnGroundPosture>
    {
        protected override DereferenceResult Perform(PetSittingOnGroundPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActor", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    //Remove(ref reference.mActor);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPostureSwitcher", field, objects))
            {
                DereferenceResult endResult = DereferenceResult.ContinueIfReferenced;

                if (Performing)
                {
                    try
                    {
                        if (reference.mActor.Posture != reference)
                        {
                            endResult = DereferenceResult.End;
                        }

                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mPostureSwitcher);
                }
                return endResult;
            }

            return DereferenceResult.Failure;
        }
    }
}
