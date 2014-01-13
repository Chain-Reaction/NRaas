using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGoHereWithSituationG : Dereference<GoHereWithSituation>
    {
        protected override DereferenceResult Perform(GoHereWithSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimsQueued", field, objects))
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }

                Remove(reference.mSimsQueued, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mFailedSims", field, objects))
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }

                Remove(reference.mFailedSims, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "FollowerInteractions", field, objects))
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }

                RemoveKeys(reference.FollowerInteractions,objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "Owner", field, objects))
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }

                //reference.Owner = null;
                return DereferenceResult.Ignore;
            }

            if (Matches(reference, "Leader", field, objects))
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }

                //reference.Leader = null;
                return DereferenceResult.Ignore;
            }

            if (Matches(reference, "Followers", field, objects))
            {
                try
                {
                    reference.Exit();
                }
                catch
                { }

                Remove(reference.Followers, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "LeaderInteraction", field, objects))
            {
                try
                {
                    reference.LeaderInteraction.Exiting = true;
                    reference.Exit();
                }
                catch
                { }

                reference.LeaderInteraction = null;
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
