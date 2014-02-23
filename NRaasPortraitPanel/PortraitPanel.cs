using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.PortraitPanelSpace;
using NRaas.PortraitPanelSpace.Dialogs;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class PortraitPanel : Common, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        protected static PersistedSettings sSettings = null;

        protected static DelayedEventListener sLotChangedListener = null;

        static PortraitPanel()
        {
            Bootstrap();
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;

            SkewerEx.Instance.PopulateSkewers();
        }

        public void OnWorldLoadFinished()
        {
            DreamCatcher.OnWorldLoadFinishedDreams();

            Common.FunctionTask.Perform(OnInitialize);

            new AlarmTask(5f, TimeUnit.Minutes, SkewerEx.OnUpdate, 5f, TimeUnit.Minutes);

            new Common.DelayedEventListener(EventTypeId.kEventSimSelected, OnSimSelected);

            InitializeWatchers();
        }

        public void OnWorldQuit()
        {
            try
            {
                SkewerEx.Unload();

                ShutdownWatchers();
            }
            catch (Exception exception)
            {
                Common.Exception("OnWorldQuit", exception);
            }
        }

        protected static void OnInitialize()
        {
            if (SkewerEx.Instance != null) return;

            try
            {
                while ((Skewer.Instance == null) || (Skewer.Instance.mDoubleClickTimer == null))
                {
                    SpeedTrap.Sleep();
                }

                while (!GameStates.IsLiveState)
                {
                    SpeedTrap.Sleep();
                }

                SkewerEx.Load();

                while (Skewer.Instance != null)
                {
                    if (Skewer.Instance.mHouseholdItems.Length == 0) break;

                    if (Skewer.Instance.mHouseholdItems[0].mContainer == null) break;

                    if (Skewer.Instance.mHouseholdItems[0].mContainer.Visible)
                    {
                        SkewerEx.HideSkewer(ref Skewer.Instance.mHouseholdItems);
                    }

                    SpeedTrap.Sleep();
                }
            }
            catch (Exception exception)
            {
                Common.Exception("Simulate", exception);
            }
        }

        protected static void OnSimSelected(Event e)
        {
            if ((SkewerEx.Instance != null) && (sSettings != null))
            {
                if ((Settings.InUse(SkewerEx.VisibilityType.ActiveSimLot)) ||
                    (Settings.InUse(SkewerEx.VisibilityType.ActiveFamilyLot)) ||
                    (Settings.InUse(SkewerEx.VisibilityType.ActiveHousehold)) ||
                    (Settings.InUse(SkewerEx.VisibilityType.ActiveHomeLot)))
                {
                    Common.FunctionTask.Perform(SkewerEx.Instance.PopulateSkewers);
                }
            }
        }

        // Externalized to StoryProgression
        public static bool IsSimListed(SimDescription sim)
        {
            if (sim == null)
            {
                return false;
            }

            try
            {
                return SkewerEx.Instance.IsSimListed(sim);
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return false;
            }
        }

        public static void InitializeWatchers()
        {
            ShutdownWatchers();

            LotCheckTask.Initialize();

            if ((Settings.InUse(SkewerEx.VisibilityType.ActiveSimLot)) ||
                (Settings.InUse(SkewerEx.VisibilityType.ActiveHomeLot)) ||
                (Settings.InUse(SkewerEx.VisibilityType.ActiveFamilyLot)))
            {
                sLotChangedListener = new Common.DelayedEventListener(EventTypeId.kEventSimMovedFromLot, OnLotChanged);
            }
        }

        public static void ShutdownWatchers()
        {
            LotCheckTask.Shutdown();

            if (sLotChangedListener != null)
            {
                sLotChangedListener.Dispose();
                sLotChangedListener = null;
            }
        }

        protected static void OnLotChanged(Event e)
        {
            Sim sim = e.Actor as Sim;
            Lot oldLot = e.TargetObject as Lot;
            Lot newLot = e.Actor.LotCurrent;

            if ((SkewerEx.Instance != null) && (sSettings != null))
            {
                bool changed = false;
                if (Settings.InUse(SkewerEx.VisibilityType.ActiveSimLot))
                {
                    Sim active = Sim.ActiveActor;
                    if (active != null)
                    {
                        if (sim == active)
                        {
                            changed = true;
                        }
                        else if ((oldLot == active.LotCurrent) || (newLot == active.LotCurrent))
                        {
                            changed = true;
                        }
                    }
                }
                else if (Settings.InUse(SkewerEx.VisibilityType.ActiveFamilyLot))
                {
                    if (Household.ActiveHousehold != null)
                    {
                        if (sim.Household == Household.ActiveHousehold)
                        {
                            changed = true;
                        }
                        else
                        {
                            foreach (Sim member in Household.ActiveHousehold.AllActors)
                            {
                                if ((oldLot == member.LotCurrent) || (newLot == member.LotCurrent))
                                {
                                    changed = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (Settings.InUse(SkewerEx.VisibilityType.ActiveHomeLot))
                {
                    if (Household.ActiveHousehold != null)
                    {
                        if ((oldLot == Household.ActiveHousehold.LotHome) || (newLot == Household.ActiveHousehold.LotHome))
                        {
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    Common.FunctionTask.Perform(SkewerEx.Instance.PopulateSkewers);
                }
            }
        }

        public class LotCheckTask : RepeatingTask
        {
            static LotCheckTask sLotCheckTask = null;

            Lot mViewedLot = null;

            protected static void Startup()
            {
                if (sLotCheckTask != null) return;

                sLotCheckTask = new LotCheckTask();
                sLotCheckTask.AddToSimulator();
            }

            public static void Shutdown()
            {
                if (sLotCheckTask == null) return;

                sLotCheckTask.Dispose();
                sLotCheckTask = null;
            }

            public static void Initialize()
            {
                if (Settings.InUse(SkewerEx.VisibilityType.ViewedLot))
                {
                    Startup();
                }
                else
                {
                    Shutdown();
                }
            }

            protected override int Delay
            {
                get { return 1000; }
            }

            protected override bool OnPerform()
            {
                if ((SkewerEx.Instance != null) && (sSettings != null))
                {
                    if (Settings.InUse(SkewerEx.VisibilityType.ViewedLot))
                    {
                        Lot viewedLot = LotManager.GetLotAtPoint(CameraController.GetLODInterestPosition());

                        if (mViewedLot != viewedLot)
                        {
                            mViewedLot = viewedLot;

                            Common.FunctionTask.Perform(SkewerEx.Instance.PopulateSkewers);
                        }
                    }
                }

                return true;
            }
        }

        public class Logger : Common.Logger<Logger>, IAddStatGenerator
        {
            public readonly static Logger sLogger = new Logger();

            StatBin.StatEntry mEntry = new StatBin.StatEntry("PopulateSkewers");

            public static void Append(string msg)
            {
                if (!Common.kDebugging) return;

                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Log"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }

            public Common.DebugLevel DebuggingLevel
            {
                get { return DebugLevel.Stats; }
            }

            public float AddStat(string stat, float val)
            {
                mEntry.Add(val);
                return val;
            }
            public float AddStat(string stat, float val, Common.DebugLevel minLevel)
            {
                mEntry.Add(val);
                return val;
            }

            protected override int PrivateLog(Common.StringBuilder builder)
            {
                if (mEntry.Count > 0)
                {
                    Append(mEntry.ToString());
                }

                return base.PrivateLog(builder);
            }
        }
    }
}
