using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckRoleManager : Check<RoleManager>
    {
        protected override bool PrePerform(RoleManager manager, bool postLoad)
        {
            if (manager == null) return false;

            foreach (KeyValuePair<Role.RoleType, List<Role>> roles in manager.mRoles)
            {
                if (roles.Value == null) continue;

                int index = 0;
                while (index < roles.Value.Count)
                {
                    Role role = roles.Value[index];
                    if (role == null)
                    {
                        roles.Value.RemoveAt(index);
                    }
                    else if (role.mSim == null)
                    {
                        if (role.mRoleGivingObject != null)
                        {
                            role.mRoleGivingObject.CurrentRole = null;
                        }

                        roles.Value.RemoveAt(index);
                    }
                    else if (role.mRoleGivingObject == null)
                    {
                        // EA fail in RoleLocationMerchant:EndRole
                        if (role is RoleLocationMerchant)
                        {
                            roles.Value.RemoveAt(index);
                        }
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            return true;
        }
    }
}
