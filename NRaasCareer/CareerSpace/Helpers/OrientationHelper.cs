using Sims3.Gameplay;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.CareerSpace.Helpers
{
    public class OrientationHelper : Common.IDelayedWorldLoadFinished
    {
        public void OnDelayedWorldLoadFinished()
        {
            if (OrientationSituation.sStartOrientationAlarm != AlarmHandle.kInvalidHandle)
            {
                AlarmManager.Global.RemoveAlarm(OrientationSituation.sStartOrientationAlarm);
                OrientationSituation.sStartOrientationAlarm = AlarmHandle.kInvalidHandle;
            }

            if (GameUtils.IsInstalled(ProductVersion.EP9))
            {
                OrientationSituation.sStartOrientationAlarm = AlarmManager.Global.AddAlarmDay(OrientationSituation.kScheduleHour, ~DaysOfTheWeek.None, ScheduleTodaysOrientation, "Orientation Startup", AlarmType.NeverPersisted, null);
            }
        }

        private static void ScheduleTodaysOrientation()
        {
            Common.DebugNotify("ScheduleTodaysOrientation A");

            if (GameUtils.IsUniversityWorld())
            {
                if (GameStates.CurrentDayOfTrip != 0x1) return;
            }
            else
            {
                if (!AcademicHelper.HasFirstDayActiveStudents()) return;
            }

            Common.DebugNotify("ScheduleTodaysOrientation B");

            Lot lot = null;

            foreach (EventLotMarker marker in Sims3.Gameplay.Queries.GetObjects<EventLotMarker>())
            {
                if (marker.LotCurrent == null) continue;

                if (!marker.LotCurrent.IsCommunityLot) continue;

                lot = marker.LotCurrent;
                break;
            }

            if (lot == null)
            {
                lot = LotManager.GetLot(OrientationSituation.kOrientationLotId);
            }

            Common.DebugNotify("ScheduleTodaysOrientation " + lot.LotId);

            if (lot != null)
            {
                OrientationSituation.Create(lot);
            }
        }
    }
}
