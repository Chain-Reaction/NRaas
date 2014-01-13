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
    public class DerefPregnancy : Dereference<Pregnancy>
    {
        protected override DereferenceResult Perform(Pregnancy reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mMom", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mMom);
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mContractionBroadcast", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mContractionBroadcast);
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mDad", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        if (reference.mDad.SimDescription != null)
                        {
                            reference.DadDescriptionId = reference.mDad.SimDescription.SimDescriptionId;
                        }

                        Remove(ref reference.mDad);
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
