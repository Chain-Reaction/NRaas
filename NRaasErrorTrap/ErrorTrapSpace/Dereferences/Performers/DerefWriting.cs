using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefWriting : Dereference<Writing>
    {
        protected override DereferenceResult Perform(Writing reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRoyaltyAlarm", field, objects))
            {
                try
                {
                    reference.mRoyaltyAlarm.RemoveRoyaltyAlarm();
                }
                catch
                { }

                Remove(ref reference.mRoyaltyAlarm);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mOpportunityBiographySubject", field, objects))
            {
                Remove(ref reference.mOpportunityBiographySubject);
                return DereferenceResult.End;
            }

            if (Matches(reference, "WrittenBookDataList", field, objects))
            {
                RemoveValues(reference.WrittenBookDataList, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
