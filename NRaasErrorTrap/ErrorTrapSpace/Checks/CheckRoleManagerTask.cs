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
    public class CheckRoleManagerTask : Check<RoleManagerTask>
    {
        protected override bool PrePerform(RoleManagerTask obj, bool postLoad)
        {
            foreach (Role.RoleType type in Enum.GetValues(typeof(Role.RoleType)))
            {
                List<Role> roles = RoleManager.GetRolesOfType(type);
                if (roles == null) continue;

                for (int i = roles.Count - 1; i >= 0; i--)
                {
                    if (roles[i] == null)
                    {
                        roles.RemoveAt(i);

                        LogCorrection("Bad Role Dropped");
                    }
                }
            }

            return true;
        }
    }
}
