using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Helpers
{
    public class RoleEx
    {
        public static bool IsSimValidForAnyRole(IMiniSimDescription iSim, Role.RoleType role)
        {
            string reason = null;
            return IsSimValidForAnyRole(iSim, role, out reason);
        }
        public static bool IsSimValidForAnyRole(IMiniSimDescription iSim, Role.RoleType role, out string reason)
        {
            SimDescription sim = iSim as SimDescription;
            if (sim != null)
            {
                if (!sim.IsValidDescription)
                {
                    reason = "IsValidDescription Fail";
                    return false;
                }

                if (SimTypes.InCarPool(sim))
                {
                    reason = "InCarPool";
                    return false;
                }

                if (SimTypes.InServicePool(sim))
                {
                    reason = "InServicePool";
                    return false;
                }

                if (SimTypes.IsServiceAlien(sim))
                {
                    reason = "IsServiceAlien";
                    return false;
                }

                if (SimTypes.IsDead(sim))
                {
                    reason = "Dead";
                    return false;
                }

                if (sim.Household != null)
                {
                    if (sim.Household.IsPreviousTravelerHousehold)
                    {
                        reason = "PreviousTravelerHousehold";
                        return false;
                    }

                    if ((role != Role.RoleType.Tourist) && (role != Role.RoleType.Explorer))
                    {
                        if (sim.Household.IsTouristHousehold)
                        {
                            reason = "Tourist";
                            return false;
                        }
                    }
                }
            }
            else
            {
                MiniSimDescription miniSim = iSim as MiniSimDescription;
                if (miniSim != null)
                {
                    if (!Role.IsSimValidForAnyRole(miniSim))
                    {
                        reason = "IsSimValidForAnyRole Fail";
                        return false;
                    }
                }
                else
                {
                    reason = "MiniSim Fail";
                    return false;
                }
            }

            reason = "Success";
            return true;
        }

        public static bool IsSimGoodForRole(IMiniSimDescription sim, RoleData role, IRoleGiver roleGiver)
        {
            string reason = null;
            return IsSimGoodForRole(sim, role, roleGiver, out reason);
        }
        public static bool IsSimGoodForRole(IMiniSimDescription sim, RoleData role, IRoleGiver roleGiver, out string reason)
        {
            if (role == null)
            {
                reason = "No Role";
                return false;
            }

            if (!IsSimValidForAnyRole(sim, role.Type, out reason)) return false;

            return IsSimGoodForRoleCommonTest(sim, role, roleGiver, out reason);
        }

        private static bool IsSimGoodForRoleCommonTest(IMiniSimDescription desc, RoleData data, IRoleGiver roleGiver, out string reason)
        {
            WorldName homeWorld = desc.HomeWorld;

            /*
            bool isCelebrity = desc.IsCelebrity;
            if (!data.CanBeCelebrity && isCelebrity)
            {
                reason = "Celebrity Fail";
                return false;
            }
            */
            if ((CASUtils.CASAGSAvailabilityFlagsFromCASAgeGenderFlags(desc.Age | desc.Species) & data.AvailableAgeSpecies) == CASAGSAvailabilityFlags.None)
            {
                reason = "Age/Species Fail";
                return false;
            }

            SimDescription description = desc as SimDescription;
            if (((description != null) && (description.CreatedSim == null)) && (description.WillAgeUpOnInstantiation && ((CASUtils.CASAGSAvailabilityFlagsFromCASAgeGenderFlags(AgingState.GetNextOlderAge(desc.Age, desc.Species) | desc.Species) & data.AvailableAgeSpecies) == CASAGSAvailabilityFlags.None)))
            {
                reason = "Age/Species Fail";
                return false;
            }

            if (data.FillRoleFrom == Role.RoleFillFrom.PeopleWhoDontLiveInThisWorld)
            {                
                if ((homeWorld != WorldName.TouristWorld) && (GameUtils.GetWorldType(homeWorld) != WorldType.Vacation) && (GameUtils.GetWorldType(homeWorld) != WorldType.Future))
                {
                    reason = "Vacation World Fail";
                    return false;
                }
                if (homeWorld == GameUtils.GetCurrentWorld())
                {
                    reason = "Home World Fail";
                    return false;
                }
                if (GameUtils.GetWorldType(homeWorld) == WorldType.Future)
                {
                    if (Sims3.Gameplay.Queries.CountObjects<ITimePortal>() == 0)
                    {
                        reason = "Time Portal Fail";
                        return false;
                    }                    
                }
            }
            /*
            else if (data.FillRoleFrom == RoleFillFrom.CustomCreatedSim)
            {
                return false;
            }
            */

            IRoleGiverCustomIsSimGoodTest test = roleGiver as IRoleGiverCustomIsSimGoodTest;
            if ((test != null) && (!test.IsSimGoodForRole(desc)))
            {
                reason = "Role Giver Fail";
                return false;
            }

            reason = "Success";
            return true;
        }

        public static bool IsSimGood(Role role)
        {
            SimDescription sim = role.mSim;
            if (sim == null) return false;

            return IsSimGoodForRole(sim, role.Data, role.RoleGivingObject);
        }
    }
}