using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class HandleLunarCycle : DelayedLoadupOption
    {
        static LunarCycleManager.LunarPhase[] sTwoStage = new LunarCycleManager.LunarPhase[] { LunarCycleManager.LunarPhase.kNewMoon, LunarCycleManager.LunarPhase.kFullMoon };
        static LunarCycleManager.LunarPhase[] sFourStage = new LunarCycleManager.LunarPhase[] { LunarCycleManager.LunarPhase.kNewMoon, LunarCycleManager.LunarPhase.kFirstQuarter, LunarCycleManager.LunarPhase.kFullMoon, LunarCycleManager.LunarPhase.kThirdQuarter };
        static LunarCycleManager.LunarPhase[] sSixStage = new LunarCycleManager.LunarPhase[] { LunarCycleManager.LunarPhase.kNewMoon, LunarCycleManager.LunarPhase.kWaxingCrescent, LunarCycleManager.LunarPhase.kWaxingGibbous, LunarCycleManager.LunarPhase.kFullMoon, LunarCycleManager.LunarPhase.kWaningGibbous, LunarCycleManager.LunarPhase.kWaningCrescent };
        static LunarCycleManager.LunarPhase[] sEightStage = new LunarCycleManager.LunarPhase[] { LunarCycleManager.LunarPhase.kNewMoon, LunarCycleManager.LunarPhase.kWaxingCrescent, LunarCycleManager.LunarPhase.kFirstQuarter, LunarCycleManager.LunarPhase.kWaxingGibbous, LunarCycleManager.LunarPhase.kFullMoon, LunarCycleManager.LunarPhase.kWaningGibbous, LunarCycleManager.LunarPhase.kThirdQuarter, LunarCycleManager.LunarPhase.kWaningCrescent };
        static LunarCycleManager.LunarPhase[] sTenStage = new LunarCycleManager.LunarPhase[] { LunarCycleManager.LunarPhase.kNewMoon, LunarCycleManager.LunarPhase.kWaxingCrescent, LunarCycleManager.LunarPhase.kFirstQuarter, LunarCycleManager.LunarPhase.kFirstQuarter, LunarCycleManager.LunarPhase.kWaxingGibbous, LunarCycleManager.LunarPhase.kFullMoon, LunarCycleManager.LunarPhase.kWaningGibbous, LunarCycleManager.LunarPhase.kThirdQuarter, LunarCycleManager.LunarPhase.kThirdQuarter, LunarCycleManager.LunarPhase.kWaningCrescent };

        public override void OnDelayedWorldLoadFinished()
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP7)) return;

            Overwatch.Log("HandleLunarCycle");

            float hourOfDay = World.GetSunriseTime() + 2.6f;

            HudModel model = Sims3.UI.Responder.Instance.HudModel as HudModel;

            AlarmManager.Global.RemoveAlarm(model.mLunarUpdateAlram);
            model.mLunarUpdateAlram = AlarmManager.Global.AddAlarmDay(hourOfDay, ~DaysOfTheWeek.None, OnLunarUpdate, "LunarUpdateAlarm", AlarmType.NeverPersisted, null);

            if (Overwatch.Settings.mCurrentPhaseIndex < 0)
            {
                InitializeLunarPhase();
            }

            LunarUpdate(false);

            if (SimClock.IsNightTime())
            {
                // Reactivates any alarms that were not persisted to save
                EventTracker.SendEvent(new LunarCycleEvent(EventTypeId.kMoonRise, (LunarCycleManager.LunarPhase)World.GetLunarPhase()));

                if (SimClock.HoursPassedOfDay >= (World.GetSunsetTime() + LunarCycleManager.kLunarEffectsDelayHours))
                {
                    if (!Overwatch.Settings.mDisableFullMoonLighting)
                    {
                        LunarCycleManager.TriggerLunarLighting();
                    }
                }
            }

            if (Overwatch.Settings.mDisableFullMoonLighting)
            {
                AlarmManager.Global.RemoveAlarm(LunarCycleManager.mLunarEffectsAlarm);
                LunarCycleManager.mLunarEffectsAlarm = AlarmHandle.kInvalidHandle;
            }

            foreach (MoonDial dial in Sims3.Gameplay.Queries.GetObjects<MoonDial>())
            {
                try
                {
                    if (dial.mLunarFXLookUp.Length > Overwatch.Settings.mCurrentPhaseIndex)
                    {
                        dial.StartLunarFX(dial.mLunarFXLookUp[Overwatch.Settings.mCurrentPhaseIndex]);
                    }
                }
                catch
                { }
            }

            Sims3.UI.Responder.Instance.OptionsModel.OptionsChanged += OnOptionsChanged;
        }

        protected static void InitializeLunarPhase()
        {
            LunarCycleManager.LunarPhase phase = (LunarCycleManager.LunarPhase)World.GetLunarPhase();

            LunarCycleManager.LunarPhase[] choices = GetCycle();

            if ((Overwatch.Settings.mCurrentPhaseIndex < 0) || 
                (Overwatch.Settings.mCurrentPhaseIndex >= choices.Length) || 
                (choices[Overwatch.Settings.mCurrentPhaseIndex] != phase))
            {
                Overwatch.Settings.mCurrentPhaseIndex = 0;

                for (int i = 0; i < choices.Length; i++)
                {
                    if (choices[i] == phase)
                    {
                        Overwatch.Settings.mCurrentPhaseIndex = i;
                        break;
                    }
                }
            }
        }

        public static void OnOptionsChanged()
        {
            try
            {
                InitializeLunarPhase();

                LunarUpdate(false);
            }
            catch (Exception e)
            {
                Common.Exception("OnOptionsChanged", e);
            }
        }

        protected static void OnLunarUpdate()
        {
            try
            {
                LunarUpdate(true);
            }
            catch (Exception e)
            {
                Common.Exception("OnLunarUpdate", e);
            }
        }

        protected static LunarCycleManager.LunarPhase[] GetCycle()
        {
            switch (World.GetLunarCycle())
            {
                case 0:
                    return sTwoStage;
                case 1:
                    return sFourStage;
                case 2:
                    return sSixStage;
                case 3:
                    return sEightStage;
                default:
                    return sTenStage;
            }
        }

        protected static void OnDewolf()
        {
            OnDewolf(true);
        }
        protected static void OnDewolf(bool forFullMoon)
        {
            foreach (Sim sim in LotManager.Actors)
            {
                if (sim.BuffManager == null) continue;

                if (sim.OccultManager == null) continue;

                sim.BuffManager.RemoveElement(BuffNames.FeralChange);

                if (forFullMoon)
                {
                    if (SimTypes.IsSelectable(sim)) continue;

                    OccultWerewolf.ForceHumanformation(sim);
                }
            }
        }

        protected static void LunarUpdate(bool change)
        {
            HudModel model = Sims3.UI.Responder.Instance.HudModel as HudModel;

            LunarCycleManager.LunarPhase[] choices = null;

            uint result;
            OptionsModel.GetOptionSetting("EnableLunarPhase", out result);
            if (result == 0x0)
            {
                choices = GetCycle();

                if (change)
                {
                    Overwatch.Settings.mCurrentPhaseIndex++;
                }

                if (Overwatch.Settings.mCurrentPhaseIndex >= choices.Length)
                {
                    Overwatch.Settings.mCurrentPhaseIndex = 0;
                }

                World.ForceLunarPhase((int)choices[Overwatch.Settings.mCurrentPhaseIndex]);
            }

            LunarCycleManager.LunarPhase phase = (LunarCycleManager.LunarPhase)World.GetLunarPhase();
            if (phase != LunarCycleManager.LunarPhase.kFullMoon)
            {
                if ((LunarCycleManager.sFullMoonZombies == null) || (LunarCycleManager.sFullMoonZombies.Count == 0))
                {
                    foreach (Sim sim in LotManager.Actors)
                    {
                        if (sim.LotHome != null) continue;

                        if (sim.BuffManager == null) continue;

                        sim.BuffManager.RemoveElement(BuffNames.Zombie);
                    }
                }

                OnDewolf(false);
            }
            else
            {
                new Common.AlarmTask(Common.AlarmTask.TimeTo(World.GetSunriseTime()), TimeUnit.Hours, OnDewolf);
            }

            EventTracker.SendEvent(new MoonRiseEvent((uint)World.GetLunarPhase()));
            if (model.LunarUpdate != null)
            {
                model.LunarUpdate((uint)World.GetLunarPhase());
            }

            if (Overwatch.Settings.mDisableFullMoonLighting)
            {
                if (LunarCycleManager.mLunarEffectsAlarm != AlarmHandle.kInvalidHandle)
                {
                    AlarmManager.Global.RemoveAlarm(LunarCycleManager.mLunarEffectsAlarm);
                    LunarCycleManager.mLunarEffectsAlarm = AlarmHandle.kInvalidHandle;
                }
            }

            foreach (MoonDial dial in Sims3.Gameplay.Queries.GetObjects<MoonDial>())
            {
                try
                {
                    if (dial.mLunarFXLookUp.Length > Overwatch.Settings.mCurrentPhaseIndex)
                    {
                        dial.StartLunarFX(dial.mLunarFXLookUp[Overwatch.Settings.mCurrentPhaseIndex]);
                    }
                }
                catch
                { }
            }

            if (choices != null)
            {
                SimDisplay display = SimDisplay.Instance;
                if (display != null)
                {
                    int numDaysUntilFullMoon = 0;

                    int index = Overwatch.Settings.mCurrentPhaseIndex;
                    while ((choices[index] != LunarCycleManager.LunarPhase.kFullMoon) && (numDaysUntilFullMoon < choices.Length))
                    {
                        numDaysUntilFullMoon++;
                        index++;
                        if (index >= choices.Length)
                        {
                            index = 0;
                        }
                    }

                    if (display.mLunarCycleIcon != null)
                    {
                        string entryKey = "UI/LunarCycle:MoonString" + ((uint)World.GetLunarPhase() + 0x1);
                        display.mLunarCycleIcon.TooltipText = Common.LocalizeEAString(entryKey);

                        if (numDaysUntilFullMoon > 0x0)
                        {
                            display.mLunarCycleIcon.TooltipText = display.mLunarCycleIcon.TooltipText + "\n";
                            display.mLunarCycleIcon.TooltipText = display.mLunarCycleIcon.TooltipText + Common.LocalizeEAString(false, "UI/LunarCycle:NextFullMoon", new object[] { numDaysUntilFullMoon });
                        }
                    }
                }
            }
        }
    } 
}
