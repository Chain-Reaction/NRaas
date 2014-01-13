using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class TimeControlEx
    {
        public static TimeControl sThs;

        public static void OnMinuteChanged()
        {
            try
            {
                string timeString = SimClockUtils.GetTextDayOfWeekAbrev() + " " + SimClockUtils.GetDisplayText();

                sThs.mClockDisplay.Caption = timeString;// sThs.mHudModel.TimeString;
                sThs.mClockDisplay.TooltipText = TimeTooltipString();
            }
            catch (Exception e)
            {
                Common.Exception("OnMinuteChanged", e);
            }
        }

        public static string TimeTooltipString()
        {
            /*
            if (GameUtils.IsOnVacation())
            {
                return Localization.LocalizeString("Gameplay/Utilities/SimClock:DayOfVacation", new object[] { this.CurrentTripDay, this.MaxTripDays });
            }
            */

            int week = SimClock.ElapsedCalendarWeeks() + 0x1;
            int day = (SimClock.ElapsedCalendarDays() + 0x1) - (0x7 * (week - 0x1));
            long priorWorldTicksPlayed = GameStates.PriorWorldTicksPlayed;
            if (priorWorldTicksPlayed != 0x0L)
            {
                int num4 = (int)SimClock.ConvertFromTicks(priorWorldTicksPlayed, TimeUnit.Weeks);
                int num5 = ((int)SimClock.ConvertFromTicks(priorWorldTicksPlayed, TimeUnit.Days)) - (0x7 * num4);
                week += num4;
                day += num5;
                if (day > 0x7)
                {
                    week++;
                    day -= 0x7;
                }
            }

            return Localization.LocalizeString("Gameplay/Utilities/SimClock:TimePlayed", new object[] { week, day });
        }
    }
}
