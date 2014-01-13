using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Managers
{
    public abstract class Manager : StoryProgressionObject
    {
        public enum AllowCheck
        {
            None = 0x00,
            Active = 0x01,
            UserDirected = 0x02,
        }

        public delegate bool OnAllow(IScoringGenerator stats, SimData settings, AllowCheck check);

        public delegate bool OnDualAllowFunc(IScoringGenerator stats, SimData actor, SimData target, AllowCheck check);

        IDumpStatsBaseOption mDumpStats = null;

        int mStatCycles = 0;

        StatBin mStats = null;

        StatBin mTotalStats = null;

        public Manager(Main manager)
            : base (manager)
        { }

        protected void RemoveStats()
        {
            mStats = null;

            mTotalStats = null;
        }

        public abstract void InstallOptions(bool initial);

        public void SetDebuggingLevel(Common.DebugLevel value)
        {
            if (mDebugLevel != null)
            {
                mDebugLevel.SetValue(value);
            }
        }

        public override void Startup(PersistentOptionBase options)
        {
            base.Startup(options);

            mStats = new StatBin(UnlocalizedName);

            mTotalStats = new StatBin("Total " + UnlocalizedName);
        }

        public event OnAllow OnPrivateAllow;

        public event OnDualAllowFunc OnDualAllow;

        protected bool DualAllow(IScoringGenerator stats, SimData actorData, SimData targetData, AllowCheck check)
        {
            if (OnDualAllow != null)
            {
                foreach (OnDualAllowFunc del in OnDualAllow.GetInvocationList())
                {
                    if (!del(stats, actorData, targetData, check)) return false;
                }
            }

            return true;
        }

        protected virtual bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (OnPrivateAllow != null) 
            {
                foreach (OnAllow del in OnPrivateAllow.GetInvocationList())
                {
                    if (!del(stats, settings, check)) return false;
                }
            }

            return true;
        }

        protected virtual string IsOnActiveLot(SimDescription sim, bool testViewLot)
        {
            if (sim.Household == Household.ActiveHousehold)
            {
                if (SimTypes.IsSelectable(sim))
                {
                    return "ActiveSelectable";
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return IsOnActiveLot(sim.CreatedSim, testViewLot);
            }
        }
        private static string IsOnActiveLot(Sim sim, bool testViewLot)
        {
            if (sim == null) return null;

            if (sim.LotCurrent == null) return null;

            if (Household.ActiveHousehold != null)
            {
                if (Occupation.DoesLotHaveAnyActiveJobs(sim.LotCurrent))
                {
                    return "ActiveJob";
                }

                if ((sim.LotCurrent != null) && (sim.LotCurrent.CanSimTreatAsHome(Sim.ActiveActor)))
                {
                    return "ActiveHouse";
                }

                foreach (Sim member in HouseholdsEx.AllSims(Household.ActiveHousehold))
                {
                    if (sim.LotCurrent == member.LotCurrent)
                    {
                        return "ActiveOnLot";
                    }
                }

                if (testViewLot)
                {
                    if ((!CameraController.IsMapViewModeEnabled()) && (sim.LotCurrent == LotManager.GetLotAtPoint(CameraController.GetLODInterestPosition())))
                    {
                        return "ViewLot";
                    }
                }
            }

            return null;
        }

        public virtual void GetStoryPrefixes(List<string> prefixes)
        {
            prefixes.Add(UnlocalizedName);
        }

        protected bool PrivateAllow(IScoringGenerator stats, Sim sim)
        {
            if (sim == null) return false;

            return PrivateAllow(stats, sim.SimDescription, AllowCheck.Active);
        }
        protected bool PrivateAllow(IScoringGenerator stats, Sim sim, AllowCheck check)
        {
            if (sim == null) return false;

            return PrivateAllow(stats, sim.SimDescription, check);
        }
        protected bool PrivateAllow(IScoringGenerator stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim, AllowCheck.Active);
        }
        protected virtual bool PrivateAllow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (sim == null) return false;

            if (SimTypes.IsPassporter(sim))
            {
                stats.IncStat("Allow: Simport");
                return false;
            }

            if ((check & AllowCheck.Active) == AllowCheck.Active)
            {
                if (SimTypes.IsDead(sim))
                {
                    stats.IncStat("Allow: Dead");
                    return false;
                }
                else if ((SimTypes.IsService(sim)) && (SimTypes.IsOccult(sim, OccultTypes.ImaginaryFriend)))
                {
                    stats.IncStat("Allow: Imaginary Friend");
                    return false;
                }

                string reason = IsOnActiveLot(sim, true);
                if (!string.IsNullOrEmpty(reason))
                {
                    stats.IncStat("Allow: " + reason);
                    return false;
                }

                if (GetValue<ManagerSim.ProgressActiveNPCOption,bool>())
                {
                    if (SimTypes.IsSelectable(sim))
                    {
                        stats.IncStat("Allow: Selectable");
                        return false;
                    }
                } 
                else 
                {
                    if (sim.Household == Household.ActiveHousehold)
                    {
                        stats.IncStat("Allow: Active Family");
                        return false;
                    }
                }
            }

            return PrivateAllow(stats, sim, GetData(sim), check);
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            base.PrivateUpdate(fullUpdate, initialPass);

            if (fullUpdate)
            {
                IncStat("Full Update");

                mStatCycles++;
                if ((mDumpStats != null) && (mDumpStats.Value != 0) && (mStatCycles >= mDumpStats.Value))
                {
                    DumpStats();
                    mStatCycles = 0;
                }
            }
        }

        public override void Shutdown()
        {
            if ((DebuggingEnabled) && (mTotalStats != null))
            {
                mTotalStats.Dump(null, false);
            }

            base.Shutdown();

            mStats = null;
        }

        public virtual Scenario GetImmigrantRequirement(ImmigrationRequirement requirement)
        {
            return null;
        }

        public override float AddStat(string stat, float val, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return val;

            if ((DebuggingEnabled) && (mTotalStats != null))
            {
                mTotalStats.AddStat(stat, val);
            }

            if ((minLevel == Common.DebugLevel.High) && (mStats != null) && (DebuggingLevel >= minLevel))
            {
                SimpleMessageDialog.Show(UnlocalizedName, stat);
                minLevel = Common.DebugLevel.Low;
            }

            if ((mStats != null) && (DebuggingLevel >= minLevel))
            {
                mStats.AddStat(stat, val);
            }

            return val;
        }

        public override void IncStat(string stat, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return;

            if ((DebuggingEnabled) && (mTotalStats != null))
            {
                mTotalStats.IncStat(stat);
            }

            if ((minLevel == Common.DebugLevel.High) && (mStats != null) && (DebuggingLevel >= minLevel))
            {
                Common.Notify(stat);

                minLevel = Common.DebugLevel.Low;
            }

            if ((mStats != null) && (DebuggingLevel >= minLevel))
            {
                mStats.IncStat(stat);
            }
        }

        public void DumpStats()
        {
            if ((mStats != null) && (DebuggingLevel >= Common.DebugLevel.Quiet))
            {
                mStats.Dump(DebuggingLevel.ToString (), GetValue<ManagerStory.MaxDumpLengthOption,int>());
            }
        }

        public abstract class CooldownOptionItem<TManager> : IntegerManagerOptionItem<TManager>, ICooldownOptionItem where TManager : Manager
        {
            public CooldownOptionItem(int defValue)
                : base(defValue)
            { }

            public virtual bool AdjustsForAgeSpan
            {
                get { return true; }
            }

            public virtual bool AdjustsForSpeed
            {
                get { return false; }
            }
        }

        public abstract class DumpStatsBaseOption<TManager> : IntegerManagerOptionItem<TManager>, IDumpStatsBaseOption, IDebuggingOption
            where TManager : Manager
        {
            public DumpStatsBaseOption(int value)
                : base(value)
            { }

            public override bool Install(TManager main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                Manager.mDumpStats = this;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "CyclesPerStats";
            }
        }

        public abstract class DisposableManagerToggle<TManager,TValue> : IDisposable
            where TManager : StoryProgressionObject
        {
            TManager mManager;

            protected TValue mOriginal;

            public DisposableManagerToggle(TManager manager, TValue original)
            {
                mManager = manager;
                mOriginal = original;
            }

            public TManager Manager
            {
                get { return mManager; }
                set { mManager = value; }
            }

            public abstract void Dispose();
        }

        public abstract class ImmigrationPressureBaseOption<TManager> : IntegerManagerOptionItem<TManager>
            where TManager : Manager
        {
            public ImmigrationPressureBaseOption(int defValue)
                : base(defValue)
            { }

            public override string GetTitlePrefix()
            {
                if (Manager == null) return null;

                return Manager.UnlocalizedName + "ImmigrationPressure";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class ImmigrationRequirement
        {
            public bool mRequired = false;

            public bool mNeedMale;
            public bool mNeedFemale;

            public bool mFertile;
            public bool mSingle;

            public SimDescription mMate;

            public SimDescription mCareerSim;

            public CareerLocation CareerLoc
            {
                get
                {
                    if (mCareerSim == null) return null;

                    return mCareerSim.Occupation.CareerLoc;
                }
            }

            public CareerLevel CareerLevel
            {
                get
                {
                    if (mCareerSim == null) return null;

                    Career career = mCareerSim.Occupation as Career;
                    if (career == null) return null;

                    return career.CurLevel;
                }
            }

            public CASAgeGenderFlags MateGender
            {
                get
                {
                    if (mMate == null) return CASAgeGenderFlags.None;

                    if (mMate.mGenderPreferenceFemale > 0)
                    {
                        return CASAgeGenderFlags.Female;
                    }
                    else if (mMate.mGenderPreferenceMale > 0)
                    {
                        return CASAgeGenderFlags.Male;
                    }
                    else if (mMate.IsMale)
                    {
                        return CASAgeGenderFlags.Female;
                    }
                    else
                    {
                        return CASAgeGenderFlags.Male;
                    }
                }
            }
        }

        public abstract class DumpScoringBaseOption<TManager> : IntegerManagerOptionItem<TManager>, INotPersistableOption, IDebuggingOption
            where TManager : Manager
        {
            public DumpScoringBaseOption()
                : base(0)
            { }

            public override int Value
            {
                get
                {
                    return Manager.mTotalStats.Count;
                }
            }

            public override object PersistValue
            {
                get
                {
                    return null;
                }
                set
                { }
            }

            protected override string GetLocalizationValueKey()
            {
                return null;
            }

            public override string GetTitlePrefix()
            {
                return "DumpScoring";
            }

            protected override bool PrivatePerform()
            {
                Manager.mTotalStats.Dump(null, false);
                return true;
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public abstract class Task<TManager> : Common.FunctionTask
            where TManager : Manager
        {
            TManager mManager;

            public Task(TManager manager)
            {
                mManager = manager;
            }

            protected TManager Manager
            {
                get { return mManager; }
            }
        }
    }
}
