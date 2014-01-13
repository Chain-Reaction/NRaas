using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefInteractionInstance : Dereference<InteractionInstance>
    {
        protected override DereferenceResult Perform(InteractionInstance reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mLinkedInteractionInstance", field, objects))
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

                //Remove(ref reference.mLinkedInteractionInstance );
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "InteractionObjectPair", field, objects))
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

                //Remove(ref reference.InteractionObjectPair);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mAdditionalObjcectsToClearInUse", field, objects))
            {
                Remove(reference.mAdditionalObjcectsToClearInUse, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSelectedObjects", field, objects))
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

                Remove(reference.mSelectedObjects, objects);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "Actor", field, objects))
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

                // Do nothing but pass it up to InteractionQueue
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mStages", field, objects))
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

                Remove(reference.mStages, objects);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mCurrentStateMachine", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    try
                    {
                        reference.mCurrentStateMachine.Dispose();
                    }
                    catch
                    { }
                }

                Remove(ref reference.mCurrentStateMachine );
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Target", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    try
                    {
                        if ((reference.InteractionObjectPair != null) && (reference.InteractionObjectPair.Tuning != null))
                        {
                            reference.DetachFromTarget();
                        }
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(reference as IGameObject, e);
                    }

                    try
                    {
                        if (reference.InteractionObjectPair != null)
                        {
                            reference.InteractionObjectPair.ClearCallbacks();
                            reference.InteractionObjectPair.ClearTarget();
                        }
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(reference as IGameObject, e);
                    }
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mInstanceActor", field, objects))
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

                Remove(ref reference.mInstanceActor );
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
