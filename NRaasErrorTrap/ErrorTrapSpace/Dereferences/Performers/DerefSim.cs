using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSim : Dereference<Sim>
    {
        protected override DereferenceResult Perform(Sim reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "PostureChanged", field, objects))
            {
                Sim.PostureChangedCallback callback = FindLast<Sim.PostureChangedCallback>(objects);
                if (callback != null)
                {
                    reference.PostureChanged -= callback;
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mSimDescription", field, objects))
            {
                if (Performing)
                {
                    //Remove(ref reference.mSimDescription);
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            if (Matches(reference, "PetSittingOnGround", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.PetSittingOnGround.Dispose();
                    }
                    catch
                    { }

                    if (reference.HasBeenDestroyed)
                    {
                        Remove(ref reference.PetSittingOnGround);
                    }
                    else
                    {
                        reference.SacsInit();
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mDeepSnowEffectManager", field, objects))
            {
                Remove(ref reference.mDeepSnowEffectManager);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mBridgeOrigin", field, objects))
            {
                Remove(ref reference.mBridgeOrigin);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mActiveSwitchOutfitHelper", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mActiveSwitchOutfitHelper.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mActiveSwitchOutfitHelper);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mClothingReactionBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mClothingReactionBroadcaster.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mClothingReactionBroadcaster);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "PartyAnimalWooList", field, objects))
            {
                Remove(reference.PartyAnimalWooList, objects);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "SimsReactedToGhost", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.SimsReactedToGhost, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "PostureChanged", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    RemoveDelegate<Sim.PostureChangedCallback>(ref reference.PostureChanged, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "SynchronizationTarget", field, objects))
            {
                Remove(ref reference.SynchronizationTarget);
                return DereferenceResult.End;
            }

            if (Matches(reference, "Bed", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.Bed);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSituationSpecificInteractions", field, objects))
            {
                RemoveKeys(reference.mSituationSpecificInteractions, objects);
                RemoveValues(reference.mSituationSpecificInteractions, objects);

                return DereferenceResult.End;
            }

            if (Matches(reference, "ProgressMeter", field, objects))
            {
                Remove(ref reference.ProgressMeter);
                return DereferenceResult.End;
            }

            if (Matches(reference, "PerformanceMeter", field, objects))
            {
                Remove(ref reference.PerformanceMeter);
                return DereferenceResult.End;
            }

            result = new ReferenceWrapper();
            if (Matches(reference, "ReservedVehicle", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.ReservedVehicle);
                }
                return DereferenceResult.End;
            }

            result = new ReferenceWrapper();
            if (Matches(reference, "ReservedBoat", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.ReservedBoat);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "SynchronizationTarget", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.ClearSynchronizationData();
                    }
                    catch
                    { }

                    Remove(ref reference.SynchronizationTarget);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mBridgeOrigin", field, objects))
            {
                Remove(ref reference.mBridgeOrigin);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPosture", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.Posture = null;
                        }
                        catch
                        { }
                    }
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "smcCarry", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.smcCarry.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.smcCarry);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
