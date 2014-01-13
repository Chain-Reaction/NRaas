using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSituation : Dereference<Situation>
    {
        protected override DereferenceResult Perform(Situation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mInteractions", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mInteractions, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "OnExit", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    RemoveDelegate(ref reference.OnExit, objects);
                }
                return DereferenceResult.End;
            }

            if (reference.GetType().ToString().Contains("NRaas.StoryProgressionSpace.Situations.FirefighterSituation"))
            {
                // StoryProgression Firefighter Situation
                if (Matches(reference, "mCar", field, objects))
                {
                    if (Performing)
                    {
                        try
                        {
                            reference.Exit();
                        }
                        catch
                        { }
                    }
                    return DereferenceResult.End;
                }
            }

            if (Matches(reference, "mSimsWithInteractions", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    RemoveKeys(reference.mSimsWithInteractions, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mChecks", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    RemoveKeys(reference.mChecks, objects);
                }
                return DereferenceResult.End;                
            }

            if (Matches(reference, "mForcedInteractions", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mForcedInteractions, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
