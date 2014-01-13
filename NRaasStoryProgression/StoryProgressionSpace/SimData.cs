using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Situations;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    // For legacy purposes, this class must always be at this level in the namespace 

    [Persistable]
    public class SimData : GenericOptionBase
    {
        SimID mSimID;

        List<string> mClans = new List<string>();

        [Persistable(false)]
        NonPersistSimData mOther = null;

        public SimData() // required for persistence
        { }
        public SimData(SimDescription sim)
        {
            mSimID = new SimID(sim);

            mOther = new NonPersistSimData(sim);
        }

        public override string Name
        {
            get { return "Sim: " + SimDescription.FullName; }
        }

        public bool HasCastes
        {
            get { return (Other.SimCastes.Count > 0); }
        }

        public IEnumerable<CasteOptions> Castes
        {
            get 
            {
                return Other.SimCastes; 
            }
        }

        public bool Contains(CasteOptions caste)
        {
            return Other.SimCastes.Contains(caste);
        }
        public bool HasCaste(ulong caste)
        {
            foreach (CasteOptions option in Other.SimCastes)
            {
                if (option.ID == caste) return true;
            }

            return false;
        }

        public override void InvalidateCache()
        {
            base.InvalidateCache();

            Other.InvalidateCastes();
        }

        public bool Disallowed<T>(SimData sim, bool checkOtherSide)
            where T : DisallowCasteBaseOption
        {
            T lOption = GetInternalOption<T>();
            if ((lOption != null) && (lOption.Count > 0))
            {
                if (lOption.Contains(sim.Castes)) return true;
            }

            if (checkOtherSide)
            {
                T rOption = sim.GetInternalOption<T>();
                if ((rOption != null) && (rOption.Count > 0))
                {
                    if (rOption.Contains(Castes)) return true;
                }
            }

            return false;
        }

        public bool Allowed<T>(SimData sim, bool checkOtherSide)
            where T : AllowCasteBaseOption
        {
            T lOption = GetInternalOption<T>();
            if ((lOption != null) && (lOption.Count > 0))
            {
                if (!sim.HasCastes) return false;

                if (!lOption.Contains(sim.Castes)) return false;
            }

            if (checkOtherSide)
            {
                T rOption = sim.GetInternalOption<T>();
                if ((rOption != null) && (rOption.Count > 0))
                {
                    if (!HasCastes) return false;

                    if (!rOption.Contains(Castes)) return false;
                }
            }

            return true;
        }

        public R GetValue<T, R>()
            where T : GenericOptionItem<R>, IReadSimLevelOption, new()
        {
            return GetInternalValue<T, R>();
        }

        public bool SetValue<T, R>(R value)
            where T : GenericOptionItem<R>, IWriteSimLevelOption, new()
        {
            return SetInternalValue<T, R>(value);
        }

        public TType AddValue<T, TType>(TType value)
            where T : OptionItem, IGenericAddOption<TType>, IWriteSimLevelOption, new()
        {
            return AddInternalValue<T, TType>(value);
        }

        public void RemoveValue<T, TType>(TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IWriteSimLevelOption, new()
        {
            RemoveInternalValue<T, TType>(value);
        }

        public bool HasValue<T, TType>(TType value)
            where T : OptionItem, IGenericHasOption<TType>, IReadSimLevelOption, new()
        {
            return HasInternalValue<T, TType>(value);
        }

        public bool HasAnyValue<T, TType>()
            where T : OptionItem, IGenericHasOption<TType>, IReadSimLevelOption, new()
        {
            return HasAnyInternalValue<T, TType>();
        }

        public bool Contains<T>(IEnumerable<CasteOptions> castes)
            where T : CasteListOption
        {
            T option = GetInternalOption<T>();
            if (option == null) return false;

            return option.Contains(castes);
        }

        public string HandleName<T>(ManagerSim manager, SimDescription b, out bool wasEither)
            where T : NameBaseOption, new()
        {
            T option = GetInternalOption<T>();
            if (option == null)
            {
                option = new T();
            }

            return manager.HandleName(option, SimDescription, b, out wasEither);
        }

        public override void UpdateInheritors(IGenericLevelOption option)
        {
            base.UpdateInheritors(option);

            if (option is ISimCasteOption)
            {
                // Hierarchy changed, invalidate all stored options
                InvalidateCache();
            }

            /*
            SimDescription sim = SimDescription;
            if (sim != null)
            {
                foreach (SimDescription child in IsChildScoring.GetFor(sim))
                {
                    if (child.TeenOrAbove) continue;

                    if (child.Household != sim.Household) continue;

                    if ((remove) || (!IsValidOption(option)))
                    {
                        StoryProgression.Main.GetData(child).RemoveOption(option);
                    }
                }
            }
            */
        }

        public override bool IsValidOption(IGenericLevelOption option)
        {
            return option is IReadSimLevelOption;
        }

        public void GetAllCasteOptions(List<ParentItem> options, bool preHousehold, bool testApplyHouse)
        {
            using (NetWorthOption.CacheValue cache = new NetWorthOption.CacheValue(this, StoryProgression.Main.GetValue<NetWorthOption, int>(House)))
            {
                foreach (CasteOptions option in Other.SimCastes)
                {
                    if (preHousehold)
                    {
                        if (option.Priority >= 0) continue;
                    }
                    else
                    {
                        if (option.Priority < 0) continue;
                    }

                    if (testApplyHouse)
                    {
                        if (!option.GetValue<CasteApplyToHouseOption, bool>()) continue;
                    }

                    options.Add(new ParentItem(option, DefaultingLevel.Castes));
                }
            }
        }

        protected override void GetParentOptions(List<ParentItem> options, DefaultingLevel level)
        {
            SimDescription sim = SimDescription;
            if (sim != null)
            {
                if (SimTypes.HeadOfFamily(sim.Household) == sim)
                {
                    level &= ~DefaultingLevel.HeadOfFamily;
                }

                if ((level & DefaultingLevel.Castes) == DefaultingLevel.Castes)
                {
                    if ((level & DefaultingLevel.HeadOfFamily) == DefaultingLevel.HeadOfFamily)
                    {
                        SimData head = StoryProgression.Main.GetData(SimTypes.HeadOfFamily(House));
                        if (head != null)
                        {
                            head.GetAllCasteOptions(options, true, true);
                        }
                    }

                    GetAllCasteOptions(options, true, false);
                }

                HouseholdOptions houseOptions = StoryProgression.Main.GetHouseOptions(sim.Household);
                if (houseOptions != null)
                {
                    options.Add(new ParentItem(houseOptions, level));
                }

                if ((level & DefaultingLevel.Castes) == DefaultingLevel.Castes)
                {
                    GetAllCasteOptions(options, false, false);
                }
            }
        }

        protected NonPersistSimData Other
        {
            get
            {
                if (mOther == null)
                {
                    mOther = new NonPersistSimData(this);
                }
                return mOther;
            }
        }

        public T Get<T>() where T : ElementalSimData, new()
        {
            return Other.GetData<T>();
        }

        public List<T> GetList<T>() where T : class
        {
            return Other.GetList<T>();
        }

        public SimID SimID
        {
            get { return mSimID; }
        }

        public override Household House
        {
            get 
            {
                SimDescription sim = SimDescription;
                if (sim == null) return null;

                return sim.Household;
            }
        }

        public override Lot Lot
        {
            get
            {
                Household house = House;
                if (house == null) return null;

                return house.LotHome;
            }
        }

        public override SimDescription SimDescription
        {
            get
            {
                if (mSimID == null)
                {
                    return null;
                }
                else
                {
                    return Other.Sim;
                    //return mSimID.SimDescription;
                }
            }
        }

        public void SetSimDescription(SimDescription value)
        {
            if (!SimID.Matches(mSimID, value)) return;

            Other.Sim = value;
        }

        public IEnumerable<string> Clans
        {
            get { return mClans; }
        }
        
        public void AddClan(SimPersonality clan)
        {
            string name = clan.UnlocalizedName.ToLower();

            if (mClans.Contains(name)) return;

            ScoringLookup.UnloadCaches<IClanScoring>();

            mClans.Add(name);
        }

        public bool IsClan(SimPersonality clan)
        {
            if (clan == null) return false;

            return mClans.Contains(clan.UnlocalizedName.ToLower());
        }

        public bool RemoveClan(string clan)
        {
            ScoringLookup.UnloadCaches<IClanScoring>();

            return mClans.Remove(clan.ToLower());
        }
        public bool RemoveClan(SimPersonality clan)
        {
            return RemoveClan(clan.UnlocalizedName);
        }
        
        public bool Update()
        {
            if (SimDescription == null) return false;

            return Other.Update();
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder();

            SimDescription sim = SimDescription;
            if (sim == null)
            {
                text.AddXML("SimID", "Null");
            }
            else
            {
                text.AddXML ("SimName", sim.FullName);
                text.AddXML ("SimID", sim.SimDescriptionId);
            }

            foreach (string clan in mClans)
            {
                text.AddXML("Clan", clan);
            }

            text.AddXML("CurrentDay", SimClock.ElapsedCalendarDays());

            if (Other != null)
            {
                text += Common.NewLine + Other.ToString();
            }
            else
            {
                text += Common.NewLine + "<NoOther/>";
            }

            text += Common.NewLine + base.ToString();

            return text.ToString();
        }

        [Persistable(false)]
        public class NonPersistSimData : ManagerProgressionBase
        {
            SimDescription mClanLeader;
            SimDescription mSim;

            static int sNextID = 0;

            int mID = 0;

            int mDelay = 0;
            
            Dictionary<Type, ElementalSimData> mData = new Dictionary<Type, ElementalSimData>();

            List<CasteOptions> mCastes = null;

            public NonPersistSimData(SimData data)
                : this(data.mSimID.SimDescription)
            { }
            public NonPersistSimData(SimDescription sim)
                : base (StoryProgression.Main)
            {
                Sim = sim;

                mID = sNextID;
                sNextID++;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                return "SimData";
            }

            public void InvalidateCastes()
            {
                mCastes = null;
            }

            public List<CasteOptions> SimCastes
            {
                get
                {
                    if (mCastes == null)
                    {
                        mCastes = Castes.GetMatching(mSim, StoryProgression.Main.GetValue<NetWorthOption, int>(mSim.Household));

                        List<ulong> disallowed = Options.GetSim(mSim).GetValue<DisallowCasteOption,List<ulong>>();

                        for (int i = mCastes.Count - 1; i >= 0; i--)
                        {
                            if (!disallowed.Contains(mCastes[i].ID)) continue;

                            mCastes.RemoveAt(i);
                        }
                    }

                    return mCastes;
                }
            }

            public SimDescription Sim
            {
                get 
                { 
                    //if ((mSim != null) && (!mSim.IsValidDescription)) return null;

                    return mSim; 
                }
                set
                {
                    if (mSim == value) return;

                    mSim = value;

                    mData.Clear();
                }
            }

            public SimDescription ClanLeader
            {
                get
                {
                    if ((mClanLeader != null) && (!mClanLeader.IsValidDescription)) return null;

                    return mClanLeader;
                }
                set
                {
                    mClanLeader = value;
                }
            }

            public override string ToString()
            {
                Common.StringBuilder text = new Common.StringBuilder("<ID>" + mID + "</ID>");

                foreach (ElementalSimData data in mData.Values)
                {
                    text += Common.NewLine + "<" + data.GetType().Name + ">";
                    text += Common.NewLine + data.ToString();
                    text += Common.NewLine + "</" + data.GetType().Name + ">";
                }

                foreach (CasteOptions option in SimCastes)
                {
                    text += Common.NewLine + "<Caste>" + option.Name + "</Caste>";
                }

                return text.ToString();
            }

            public List<T> GetList<T>() where T : class
            {
                List<T> list = new List<T>();

                if (mData != null)
                {
                    foreach (ElementalSimData obj in mData.Values)
                    {
                        T item = obj as T;
                        if (item == null) continue;

                        list.Add(item);
                    }
                }

                return list;
            }

            public T GetData<T>() where T : ElementalSimData, new()
            {
                T element = null;

                Type key = typeof(T);

                ElementalSimData obj;
                if (mData.TryGetValue(key, out obj))
                {
                    element = obj as T;

                    if (element == null)
                    {
                        mData.Remove(key);
                    }
                }

                if (element == null)
                {
                    element = new T();
                    element.Init(this, Sim);

                    mData.Add(key, element);
                }

                return element;
            }

            public bool Update ()
            {
                if (mSim == null) return false;

                if (!mSim.IsValidDescription)
                {
                    return (mSim.Household != null);
                }

                bool updateDelayed = (mDelay <= 0);

                foreach (OnDemandSimData data in GetList<OnDemandSimData>())
                {
                    if (data.Delayed)
                    {
                        if (!updateDelayed) continue;
                    }

                    //using (TestSpan test = new TestSpan(NRaas.StoryProgression.Main.Scenarios, "Reset: " + data.GetType().Name))
                    {
                        data.Reset();
                    }
                }

                if (mDelay <= 0)
                {
                    mDelay = 144;
                }
                else
                {
                    mDelay--;
                }

                return true;
            }
        }
    }
}

