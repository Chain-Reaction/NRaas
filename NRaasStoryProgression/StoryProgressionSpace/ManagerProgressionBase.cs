using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public abstract class ManagerProgressionBase
    {
        public enum PrefixType
        {
            Pure,
            Story,
            Summary,
        }

        private Main mMainManager = null;

        public ManagerProgressionBase(Main manager)
        {
            mMainManager = manager;
        }

        public Main Main
        {
            get 
            { 
                return mMainManager; 
            }
            set
            {
                mMainManager = value;
            }
        }

        public TType GetValue<T, TType>(OccultTypes occult)
            where T : OptionItem, IByOccultOptionItem<TType>, IGeneralOption
        {
            T item = GetInternalOption<T>(null);
            if (item == null) return default(TType);

            return item.GetValue(occult);
        }

        public TType GetValue<T, TType>(CASAgeGenderFlags species)
            where T : OptionItem, IBySpeciesOptionItem<TType>, IGeneralOption
        {
            T item = GetInternalOption<T>(null);
            if (item == null) return default(TType);

            return item.GetValue(species);
        }

        public TType GetValue<T, TType>()
            where T : GenericOptionItem<TType>, IGeneralOption
        {
            T item = GetInternalOption<T>(null);
            if (item == null) return default(TType);

            return item.Value;
        }
        public TType GetValue<T, TType>(string name)
            where T : GenericOptionItem<TType>, INameableOption
        {
            T item = GetOption<T>(name);
            if (item == null) return default(TType);

            return item.Value;
        }

        public bool HasValue<T, TType>(TType value)
            where T : OptionItem, IGenericHasOption<TType>, IGeneralOption
        {
            T item = GetInternalOption<T>(null);
            if (item == null) return false;

            return item.Contains(value);
        }
        public bool HasValue<T, TType>(string name, TType value)
            where T : OptionItem, IGenericHasOption<TType>, INameableOption
        {
            T item = GetOption<T>(name);
            if (item == null) return false;

            return item.Contains(value);
        }

        public void SetValue<T, R>(R value)
            where T : GenericOptionItem<R>, IGeneralOption, new()
        {
            T item = GetInternalOption<T>(null);
            if (item == null) return;

            item.SetValue(value);
        }

        public TType AddValue<T, TType>(TType value)
            where T : OptionItem, IGenericAddOption<TType>, IGeneralOption
        {
            T item = GetInternalOption<T>(null);
            if (item == null) return default(TType);

            return item.AddValue(value);
        }
        public TType AddValue<T, TType>(string name, TType value)
            where T : OptionItem, IGenericAddOption<TType>, INameableOption
        {
            T item = GetOption<T>(name);
            if (item == null) return default(TType);

            return item.AddValue(value);
        }

        public void RemoveValue<T, TType>(TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IGeneralOption
        {
            T item = GetInternalOption<T>(null);
            if (item == null) return;

            item.RemoveValue(value);
        }
        public void RemoveValue<T, TType>(string name, TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, INameableOption
        {
            T item = GetOption<T>(name);
            if (item == null) return;

            item.RemoveValue(value);
        }

        public TSubType GetOption<T, TSubType>(CASAgeGenderFlags species)
            where T : OptionItem, IHasSpeciesSubOption<TSubType>, IGeneralOption
            where TSubType : OptionItem, ISpeciesOption
        {
            T option = GetInternalOption<T>(null);
            if (option == null) return default(TSubType);

            return option.GetSubOption(species);
        }
        public T GetOption<T>()
            where T : OptionItem, IGeneralOption
        {
            return GetInternalOption<T>(null);
        }
        public T GetOption<T>(string name)
            where T : OptionItem, INameableOption
        {
            return GetInternalOption<T>(UnlocalizedName + name);
        }

        protected virtual T GetInternalOption<T>(string name)
            where T : OptionItem, IGeneralOrNameableOption
        {
            return Main.GetInternalOption<T>(name);
        }

        public abstract string GetTitlePrefix(PrefixType type);

        public string UnlocalizedName
        {
            get { return GetTitlePrefix(PrefixType.Pure); }
        }

        public virtual Common.DebugLevel DebuggingLevel
        {
            get { return Common.DebugLevel.Quiet; }
        }

        public virtual bool DebuggingEnabled
        {
            get { return Main.DebuggingEnabled; }
        }

        public virtual ManagerCaste Castes
        {
            get
            {
                return mMainManager.Castes;
            }
            protected set
            {
            }
        }

        public virtual ManagerMoney Money
        {
            get
            {
                return mMainManager.Money;
            }
            protected set
            {
            }
        }

        public virtual ManagerSim Sims
        {
            get
            {
                return mMainManager.Sims;
            }
            protected set
            {
            }
        }

        public virtual ManagerDeath Deaths
        {
            get
            {
                return mMainManager.Deaths;
            }
            protected set
            {
            }
        }

        public virtual ManagerSituation Situations
        {
            get
            {
                return mMainManager.Situations;
            }
            protected set
            {
            }
        }

        public virtual ManagerHousehold Households
        {
            get
            {
                return mMainManager.Households;
            }
            protected set
            {
            }
        }

        public virtual ManagerLot Lots
        {
            get
            {
                return mMainManager.Lots;
            }
            protected set
            {
            }
        }

        public virtual ManagerFriendship Friends
        {
            get
            {
                return mMainManager.Friends;
            }
            protected set
            {
            }
        }

        public virtual ManagerPersonality Personalities
        {
            get
            {
                return mMainManager.Personalities;
            }
            protected set
            {
            }
        }

        public virtual ManagerPregnancy Pregnancies
        {
            get
            {
                return mMainManager.Pregnancies;
            }
            protected set
            {
            }
        }

        public virtual ManagerSkill Skills
        {
            get
            {
                return mMainManager.Skills;
            }
            protected set
            {
            }
        }

        public virtual ManagerFlirt Flirts
        {
            get
            {
                return mMainManager.Flirts;
            }
            protected set
            {
            }
        }

        public virtual ManagerRomance Romances
        {
            get
            {
                return mMainManager.Romances;
            }
            protected set
            {
            }
        }

        public virtual ManagerCareer Careers
        {
            get
            {
                return mMainManager.Careers;
            }
            protected set
            {
            }
        }

        public virtual ManagerStory Stories
        {
            get
            {
                return mMainManager.Stories;
            }
            protected set
            {
            }
        }

        public virtual ManagerScenario Scenarios
        {
            get
            {
                if (mMainManager == null) return null;

                return mMainManager.Scenarios;
            }
            protected set
            {
            }
        }

        public virtual ManagerScoring Scoring
        {
            get
            {
                return mMainManager.Scoring;
            }
            protected set
            {
            }
        }

        public virtual OptionStore Options
        {
            get
            {
                return mMainManager.Options;
            }
            protected set
            {
            }
        }

        public HouseholdOptions GetHouseOptions(Household house)
        {
            return Options.GetHouseOptions(house);
        }

        public LotOptions GetLotOptions(Lot lot)
        {
            return Options.GetLotOptions(lot);
        }

        public SimData GetData(Sim sim)
        {
            return Options.GetSim(sim);
        }
        public T GetData<T>(Sim sim) 
            where T : ElementalSimData, new()
        {
            return Options.GetSim<T>(sim);
        }
        public SimData GetData(SimDescription sim)
        {
            if (sim == null) return null;

            return Options.GetSim(sim);
        }
        public T GetData<T>(SimDescription sim) 
            where T : ElementalSimData, new()
        {
            return Options.GetSim<T>(sim);
        }

        public R GetValue<T, R>(Lot lot)
            where T : GenericOptionItem<R>, IReadLotLevelOption, new()
        {
            if (lot == null)
            {
                return Options.GetValue<T, R>();
            }
            else
            {
                return GetLotOptions(lot).GetValue<T, R>();
            }
        }
        public R GetValue<T, R>(Household house)
            where T : GenericOptionItem<R>, IReadHouseLevelOption, new()
        {
            if (house == null)
            {
                return Options.GetValue<T,R>();
            }
            else
            {
                return GetHouseOptions(house).GetValue<T, R>();
            }
        }
        public R GetValue<T, R>(Sim sim)
            where T : GenericOptionItem<R>, IReadSimLevelOption, new()
        {
            if (sim == null)
            {
                return Options.GetValue<T, R>();
            }
            else
            {
                return GetValue<T, R>(sim.SimDescription);
            }
        }
        public R GetValue<T, R>(SimDescription sim)
            where T : GenericOptionItem<R>, IReadSimLevelOption, new()
        {
            if (sim == null)
            {
                return Options.GetValue<T, R>();
            }
            else
            {
                return Options.GetSim(sim).GetValue<T, R>();
            }
        }

        public int GetElapsedTime<T>(Sim sim)
            where T : GenericOptionItem<int>, IReadSimLevelOption, IElapsedSimLevelOption, new()
        {
            if (sim == null)
            {
                return 0;
            }
            else
            {
                return GetElapsedTime<T>(sim.SimDescription);
            }
        }
        public int GetElapsedTime<T>(SimDescription sim)
            where T : GenericOptionItem<int>, IReadSimLevelOption, IElapsedSimLevelOption, new()
        {
            if (sim == null)
            {
                return 0;
            }
            else
            {
                int value = GetValue<T, int>(sim);
                if (value < 0) return int.MaxValue;

                return (SimClock.ElapsedCalendarDays() - value);
            }
        }

        public int TestElapsedTime<T, R>(SimDescription sim)
            where T : GenericOptionItem<int>, IReadSimLevelOption, IElapsedSimLevelOption, new()
            where R : GenericOptionItem<int>, IGeneralOption
        {
            return GetElapsedTime<T>(sim) - GetValue<R, int>();
        }
        public int TestElapsedTimeSim<T, R>(SimDescription sim)
            where T : GenericOptionItem<int>, IReadSimLevelOption, IElapsedSimLevelOption, new()
            where R : GenericOptionItem<int>, IReadSimLevelOption, new()
        {
            return GetElapsedTime<T>(sim) - GetValue<R, int>(sim);
        }

        public void SetElapsedTime<T>(SimDescription sim)
            where T : GenericOptionItem<int>, IElapsedSimLevelOption, IWriteSimLevelOption, new()
        {
            SetValue<T, int>(sim, SimClock.ElapsedCalendarDays());
        }

        public void SetValue<T, R>(Household house, R value)
            where T : GenericOptionItem<R>, IWriteHouseLevelOption, new()
        {
            if (house == null)
            {
                Options.SetValue<T, R>(value);
            }
            else
            {
                GetHouseOptions(house).SetValue<T, R>(value);
            }
        }
        public void SetValue<T, R>(SimDescription sim, R value)
            where T : GenericOptionItem<R>, IWriteSimLevelOption, new()
        {
            Options.GetSim(sim).SetValue<T, R>(value);
        }

        public bool HasValue<T, TType>(Household house, TType value)
            where T : OptionItem, IGenericHasOption<TType>, IGenericLevelOption, IReadHouseLevelOption, new()
        {
            if (house == null)
            {
                return Options.HasValue<T, TType>(value);
            }
            else
            {
                return GetHouseOptions(house).HasValue<T, TType>(value);
            }
        }

        public bool HasValue<T, TType>(SimDescription sim, TType value)
            where T : OptionItem, IGenericHasOption<TType>, IGenericLevelOption, IReadSimLevelOption, new()
        {
            if (sim == null)
            {
                return Options.HasValue<T, TType>(value);
            }
            else
            {
                return GetData(sim).HasValue<T, TType>(value);
            }
        }

        public bool HasAnyValue<T, TType>(SimDescription sim)
            where T : OptionItem, IGenericHasOption<TType>, IGenericLevelOption, IReadSimLevelOption, new()
        {
            if (sim == null)
            {
                return Options.HasAnyValue<T, TType>();
            }
            else
            {
                return GetData(sim).HasAnyValue<T, TType>();
            }
        }

        public TType AddValue<T, TType>(Household house, TType value)
            where T : OptionItem, IGenericAddOption<TType>, IWriteHouseLevelOption, new()
        {
            if (house == null)
            {
                return Options.AddValue<T, TType>(value);
            }
            else
            {
                return GetHouseOptions(house).AddValue<T, TType>(value);
            }
        }
        public TType AddValue<T, TType>(SimDescription sim, TType value)
            where T : OptionItem, IGenericAddOption<TType>, IWriteSimLevelOption, new()
        {
            if (sim == null)
            {
                return Options.AddValue<T, TType>(value);
            }
            else
            {
                return Options.GetSim(sim).AddValue<T, TType>(value);
            }
        }

        public void RemoveValue<T, TType>(Household house, TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IWriteHouseLevelOption, new()
        {
            if (house == null)
            {
                Options.RemoveValue<T, TType>(value);
            }
            else
            {
                GetHouseOptions(house).RemoveValue<T, TType>(value);
            }
        }
        public void RemoveValue<T, TType>(SimDescription sim, TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IWriteSimLevelOption, new()
        {
            if (sim == null)
            {
                Options.RemoveValue<T, TType>(value);
            }
            else
            {
                Options.GetSim(sim).RemoveValue<T, TType>(value);
            }
        }
    }
}
