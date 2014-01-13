using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRelationship : Dereference<Relationship>
    {
        protected override DereferenceResult Perform(Relationship reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSTC", field, objects))
            {
                RemoveRelationship(reference);

                //Remove(ref reference.mSTC);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mWhatBHasLearnedAboutA", field, objects))
            {
                RemoveRelationship(reference);

                //Remove(ref reference.mWhatBHasLearnedAboutA);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mWhatAHasLearnedAboutB", field, objects))
            {
                RemoveRelationship(reference);

                //Remove(ref reference.mWhatAHasLearnedAboutB);
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimDescriptionA", field, objects))
            {
                RemoveRelationship(reference);

                //Remove(ref reference.SimDescriptionA);
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimDescriptionB", field, objects))
            {
                RemoveRelationship(reference);

                //Remove(ref reference.SimDescriptionB);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }

        protected void RemoveRelationship(Relationship relationship)
        {
            if (!Performing) return;

            bool found = true;

            Dictionary<SimDescription, Relationship> relations;
            if (Relationship.sAllRelationships.TryGetValue(relationship.SimDescriptionA, out relations))
            {
                if (!relations.Remove(relationship.SimDescriptionB))
                {
                    found = false;
                }
            }
            else
            {
                found = false;
            }

            if (Relationship.sAllRelationships.TryGetValue(relationship.SimDescriptionB, out relations))
            {
                if (!relations.Remove(relationship.SimDescriptionA))
                {
                    found = false;
                }
            }
            else
            {
                found = false;
            }

            if (!found)
            {
                foreach (KeyValuePair<SimDescription, Dictionary<SimDescription, Relationship>> pair in Relationship.sAllRelationships)
                {
                    foreach (KeyValuePair<SimDescription, Relationship> relation in pair.Value)
                    {
                        if (object.ReferenceEquals(relation.Value, relationship))
                        {
                            pair.Value.Remove(relation.Key);
                            break;
                        }
                    }
                }
            }
        }
    }
}
