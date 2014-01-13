using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;

namespace NRaas.CommonSpace.Helpers
{
    public class Roles
    {
        public static string GetLocalizedName(Role role)
        {
            string result = role.TooltipTitleKey;
            if (!string.IsNullOrEmpty(result))
            {
                string roleName;
                if (!Localization.GetLocalizedString(result, out roleName))
                {
                    return roleName;
                }

                return result;
            }

            ShoppingRegister register = role.RoleGivingObject as ShoppingRegister;
            if ((register != null) && (role.mSim != null))
            {
                return register.RegisterRoleName(role.mSim.IsFemale);
            }
            else
            {
                result = role.CareerTitleKey;

                string roleName;
                if (!Localization.GetLocalizedString(result, out roleName))
                {
                    return roleName;
                }

                return result;
            }
        }

        public static bool GetRoleHours(SimDescription sim, ref DateAndTime start, ref DateAndTime end)
        {
            if ((sim != null) && (sim.AssignedRole != null))
            {
                IRoleGiverExtended roleGivingObject = sim.AssignedRole.RoleGivingObject as IRoleGiverExtended;
                if (roleGivingObject != null)
                {
                    float startTime;
                    float endTime;
                    roleGivingObject.GetRoleTimes(out startTime, out endTime);

                    start = new DateAndTime(SimClock.ConvertToTicks((float)SimClock.DayToInt(SimClock.CurrentDayOfWeek), TimeUnit.Days));
                    end = new DateAndTime(SimClock.ConvertToTicks((float)SimClock.DayToInt(SimClock.CurrentDayOfWeek), TimeUnit.Days));
                    if (SimClock.HoursPassedOfDay >= endTime)
                    {
                        start= SimClock.Add(start, TimeUnit.Hours, startTime);
                        end = SimClock.Add(end, TimeUnit.Hours, endTime + 24f);
                    }
                    else
                    {
                        start = SimClock.Add(start, TimeUnit.Hours, startTime);
                        end = SimClock.Add(end, TimeUnit.Hours, endTime);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
