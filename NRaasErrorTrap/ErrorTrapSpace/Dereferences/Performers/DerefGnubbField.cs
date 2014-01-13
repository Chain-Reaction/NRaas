using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGnubbField : Dereference<GnubbField>
    {
        protected override DereferenceResult Perform(GnubbField reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult result = DereferenceResult.Failure;

            if (Matches(reference, "mKing", field, objects))
            {
                Remove(ref reference.mKing);

                result = DereferenceResult.End;
            }

            if (Matches(reference, "mKnights", field, objects))
            {
                Remove(reference.mKnights, objects);
                result = DereferenceResult.End;
            }

            if (result != DereferenceResult.Failure)
            {
                if (Performing)
                {
                    if (reference.mKnights != null)
                    {
                        foreach (GnubbField.Spot spot in reference.mKnights)
                        {
                            try
                            {
                                if (spot.Knight != null)
                                {
                                    spot.Knight.Destroy();
                                }
                            }
                            catch
                            { }
                        }

                        reference.mKnights = null;
                    }

                    if (reference.mKing != null)
                    {
                        try
                        {
                            reference.mKing.Knight.Destroy();
                        }
                        catch
                        { }

                        reference.mKing = null;
                    }

                    try
                    {
                        reference.CreateKnights();
                    }
                    catch (Exception e)
                    {
                        Common.Exception(reference, e);
                    }
                }
            }

            return result;
        }
    }
}
