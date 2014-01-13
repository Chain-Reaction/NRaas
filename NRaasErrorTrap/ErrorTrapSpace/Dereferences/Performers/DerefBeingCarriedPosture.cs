using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBeingCarriedPosture : Dereference<BeingCarriedPosture>
    {
        protected override DereferenceResult Perform(BeingCarriedPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Child", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Child.Posture = null;
                    }
                    catch
                    { }

                    try
                    {
                        reference.Parent.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.Child);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Parent", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Parent.Posture = null;
                    }
                    catch
                    { }

                    try
                    {
                        reference.Child.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.Parent);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
