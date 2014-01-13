using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRole : Dereference<Role>
    {
        protected override DereferenceResult Perform(Role reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRoleGivingObject", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.RemoveSimFromRole();
                    }
                    catch
                    { }

                    try
                    {
                        RoleManager.sRoleManager.RemoveRole(reference);
                    }
                    catch
                    { }

                    Remove(ref reference.mRoleGivingObject);
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mSim", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.RemoveSimFromRole();
                    }
                    catch
                    { }

                    try
                    {
                        RoleManager.sRoleManager.RemoveRole(reference);
                    }
                    catch
                    { }
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
