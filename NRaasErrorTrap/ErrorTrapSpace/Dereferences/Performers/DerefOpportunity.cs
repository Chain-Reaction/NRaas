using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOpportunity : Dereference<Opportunity>
    {
        protected override DereferenceResult Perform(Opportunity reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            Sim actor = reference.Actor;

            DereferenceResult reason = MatchAndRemove(reference, "mTargetDeletedListener", field, ref reference.mTargetDeletedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    Cancel(actor, reference);
                }

                return reason;
            }

            reason = MatchAndRemove(reference, "mTimeoutListener", field, ref reference.mTimeoutListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    Cancel(actor, reference);
                }

                return reason;
            }

            reason = MatchAndRemove(reference, "mCompletionListener", field, ref reference.mCompletionListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    Cancel(actor, reference);
                }

                return reason;
            }

            reason = MatchAndRemove(reference, "mSimDiedListener", field, ref reference.mSimDiedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    Cancel(actor, reference);
                }

                return reason;
            }

            reason = MatchAndRemove(reference, "mSimDescriptionDisposedListener", field, ref reference.mSimDescriptionDisposedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    Cancel(actor, reference);
                }

                return reason;
            }

            reason = MatchAndRemove(reference, "mLossListener", field, ref reference.mLossListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    Cancel(actor, reference);
                }

                return reason;
            }

            reason = MatchAndRemove(reference, "mSimCreatedListener", field, ref reference.mSimCreatedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    Cancel(actor, reference);
                }

                return reason;
            }

            if (Matches(reference, "mListenerList", field, objects))
            {
                if (Performing)
                {
                    Remove(reference.mListenerList, objects);

                    Cancel(actor, reference);
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            ReferenceWrapper result;
            if (Matches(reference, "mTarget", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (result.mObject is Sim)
                    {
                        // Sims assigned to opportunities can be hibernated, leaving a broken reference
                        return DereferenceResult.Ignore;
                    }

                    Remove(ref reference.mTarget);

                    Cancel(actor, reference);
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mSource", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (result.mObject is Sim)
                    {
                        // Sims assigned to opportunities can be hibernated, leaving a broken reference
                        return DereferenceResult.Ignore;
                    }

                    Remove(ref reference.mSource);

                    Cancel(actor, reference);
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "SetupObject", field, objects))
            {
                Remove(ref reference.SetupObject );

                Cancel(actor, reference);
                return DereferenceResult.End;
            }

            if (Matches(reference, "Actor", field, objects))
            {
                Remove(ref reference.Actor );

                Cancel(actor, reference);
                return DereferenceResult.End;
            }

            if (Matches(reference, "CustomSource", field, objects))
            {
                Remove(ref reference.CustomSource );

                Cancel(actor, reference);
                return DereferenceResult.End;
            }

            if (Matches(reference, "CustomTarget", field, objects))
            {
                Remove(ref reference.CustomTarget );

                Cancel(actor, reference);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }

        protected void Cancel(Sim actor, Opportunity reference)
        {
            if (Performing)
            {
                try
                {
                    if ((actor != null) && (actor.OpportunityManager != null))
                    {
                        actor.OpportunityManager.CancelOpportunity(reference);
                    }
                }
                catch
                { }
            }
        }
    }
}
