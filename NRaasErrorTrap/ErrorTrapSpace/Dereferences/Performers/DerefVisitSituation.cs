using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefVisitSituation : Dereference<VisitSituation>
    {
        protected override DereferenceResult Perform(VisitSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;

            if (Matches(reference, "mLastHostRemoved", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.Exit();
                        }
                        catch
                        { }

                        Remove(ref reference.mLastHostRemoved);
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mMostFriendlyHost", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    if (result.Valid)
                    {
                        Remove(ref reference.mMostFriendlyHost);
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPreviousHost", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    if (result.Valid)
                    {
                        Remove(ref reference.mPreviousHost);
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mHosts", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mHosts, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mGuest", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mGuest);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mEventListeners", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    List<List<EventListener>> events = RemoveKeys(reference.mEventListeners, objects);

                    foreach (List<EventListener> list in events)
                    {
                        foreach (EventListener e in list)
                        {
                            EventTracker.RemoveListener(e);
                        }
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
