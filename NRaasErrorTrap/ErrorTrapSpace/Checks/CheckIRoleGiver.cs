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
    public class CheckIRoleGiver : Check<IRoleGiver>
    {
        protected override bool PrePerform(IRoleGiver giver, bool postLoad)
        {
            if (giver.CurrentRole != null)
            {
                if (giver.CurrentRole.mSim == null)
                {
                    try
                    {
                        RoleManager.sRoleManager.RemoveRole(giver.CurrentRole);
                    }
                    catch
                    { }

                    giver.CurrentRole = null;
                }
            }

            return true;
        }
    }
}
