using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    // For legacy purposes, this class must always be at this level in the namespace 

    [Persistable]
    public class OptionStore : GenericOptionBase
    {
        List<SimData> mSimData = new List<SimData>();

        List<HouseholdOptions> mHouseholdOptions = new List<HouseholdOptions>();

        List<LotOptions> mLotOptions = new List<LotOptions>();

        List<CasteOptions> mCasteOptions = new List<CasteOptions>();

        HouseholdOptions mImmigrantOptions = new HouseholdOptions();

        [Persistable(false)]
        ulong mNextCasteID = 0;

        [Persistable(false)]
        Dictionary<ulong, LotOptions> mLotOptionsLookup = null;

        [Persistable(false)]
        Dictionary<ulong, HouseholdOptions> mHouseholdOptionsLookup = null;

        [Persistable(false)]
        Dictionary<ulong, CasteOptions> mCasteOptionsLookup = null;

        [Persistable(false)]
        Dictionary<SimID, SimData> mSimLookup = null;

        public OptionStore() // required for persistence
        { }

        public override string Name
        {
            get { return "Town Options"; }
        }

        public bool HasCastes
        {
            get { return (Castes.Count > 0); }
        }

        public IEnumerable<CasteOptions> AllCastes
        {
            get { return Castes.Values; }
        }

        protected Dictionary<ulong, CasteOptions> Castes
        {
            get
            {
                if (mCasteOptionsLookup == null)
                {
                    mCasteOptionsLookup = new Dictionary<ulong, CasteOptions>();

                    int index = 0;
                    while (index < mCasteOptions.Count)
                    {
                        CasteOptions oldOptions = mCasteOptions[index];

                        if (!mCasteOptionsLookup.ContainsKey(oldOptions.ID))
                        {
                            mCasteOptionsLookup.Add(oldOptions.ID, oldOptions);
                            index++;
                        }
                        else
                        {
                            mCasteOptions.RemoveAt(index);
                        }
                    }
                }

                return mCasteOptionsLookup;
            }
        }

        protected Dictionary<ulong, LotOptions> Lots
        {
            get
            {
                if (mLotOptionsLookup == null)
                {
                    mLotOptionsLookup = new Dictionary<ulong, LotOptions>();

                    int index = 0;
                    while (index < mLotOptions.Count)
                    {
                        LotOptions oldOptions = mLotOptions[index];

                        if ((LotManager.GetLot(oldOptions.LotId) == null) || (mLotOptionsLookup.ContainsKey(oldOptions.LotId)))
                        {
                            mLotOptions.RemoveAt(index);
                        }
                        else
                        {
                            mLotOptionsLookup.Add(oldOptions.LotId, oldOptions);
                            index++;
                        }
                    }
                }
                return mLotOptionsLookup;
            }
        }

        protected Dictionary<ulong, HouseholdOptions> Households
        {
            get
            {
                if (mHouseholdOptionsLookup == null)
                {
                    mHouseholdOptionsLookup = new Dictionary<ulong, HouseholdOptions>();

                    int index = 0;
                    while (index < mHouseholdOptions.Count)
                    {
                        HouseholdOptions oldOptions = mHouseholdOptions[index];

                        if ((Household.Find(oldOptions.HouseholdId) == null) || (mHouseholdOptionsLookup.ContainsKey(oldOptions.HouseholdId)))
                        {
                            mHouseholdOptions.RemoveAt(index);
                        }
                        else
                        {
                            mHouseholdOptionsLookup.Add(oldOptions.HouseholdId, oldOptions);
                            index++;
                        }
                    }
                }
                return mHouseholdOptionsLookup;
            }
        }

        protected Dictionary<SimID, SimData> Sims
        {
            get
            {
                if (mSimLookup == null)
                {
                    mSimLookup = new Dictionary<SimID, SimData>();

                    Dictionary<SimID, int> countLookup = new Dictionary<SimID, int>();

                    int count = 0;
                    int index = 0;
                    while (index < mSimData.Count)
                    {
                        count++;

                        SimData oldData = mSimData[index];

                        if (oldData.SimDescription == null)
                        {
                            mSimData.RemoveAt(index);
                        }
                        else
                        {
                            if (mSimLookup.ContainsKey(oldData.SimID))
                            {
                                int priorCount = -1;
                                if (countLookup.ContainsKey(oldData.SimID))
                                {
                                    priorCount = countLookup[oldData.SimID];
                                }

                                mSimData.RemoveAt(index);
                            }
                            else
                            {
                                countLookup.Add(oldData.SimID, count);

                                mSimLookup.Add(oldData.SimID, oldData);

                                index++;
                            }
                        }
                    }
                }
                return mSimLookup;
            }
        }

        public R GetValue<T, R>()
            where T : GenericOptionItem<R>, IGenericLevelOption, new()
        {
            return GetInternalValue<T, R>();
        }

        public bool SetValue<T, R>(R value)
            where T : GenericOptionItem<R>, IGenericLevelOption, new()
        {
            return SetInternalValue<T, R>(value);
        }

        public TType AddValue<T, TType>(TType value)
            where T : OptionItem, IGenericAddOption<TType>, IGenericLevelOption, new()
        {
            return AddInternalValue<T, TType>(value);
        }

        public void RemoveValue<T, TType>(TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IGenericLevelOption, new()
        {
            RemoveInternalValue<T, TType>(value);
        }

        public bool HasValue<T, TType>(TType value)
            where T : OptionItem, IGenericHasOption<TType>, IGenericLevelOption, new()
        {
            return HasInternalValue<T, TType>(value);
        }

        public bool HasAnyValue<T, TType>()
            where T : OptionItem, IGenericHasOption<TType>, IGenericLevelOption, new()
        {
            return HasAnyInternalValue<T, TType>();
        }

        public override SimDescription SimDescription
        {
            get { return null; }
        }

        public override Household House
        {
            get { return null; }
        }

        public override Lot Lot
        {
            get { return null; }
        }

        public void ExportCastes(Persistence.Lookup settings)
        {
            settings.Add("Castes", mCasteOptions);
        }

        public void ImportCastes(Persistence.Lookup settings)
        {
            List<CasteOptions> casteOptions = mCasteOptions;

            mCasteOptions = settings.GetList<CasteOptions>("Castes");
            mCasteOptionsLookup = null;

            if ((mCasteOptions == null) || (mCasteOptions.Count == 0))
            {
                mCasteOptions = casteOptions;
            }
            else
            {
                mNextCasteID = 0;

                ValidateCasteOptions(Castes);
            }
        }

        public override void UpdateInheritors(IGenericLevelOption option)
        {
            base.UpdateInheritors(option);

            foreach (Household house in Household.sHouseholdList)
            {
                HouseholdOptions options = GetHouseOptions(house);

                if (!IsValidOption(option))
                {
                    options.RemoveOption(option);
                }

                options.UpdateInheritors(option);
            }
        }

        public override bool IsValidOption(IGenericLevelOption option)
        {
            if (option is INotGlobalLevelOption) return false;

            if (option is IWriteHouseLevelOption) return true;

            return (option is IHouseLevelSimOption);
        }

        protected override void GetParentOptions(List<ParentItem> options, DefaultingLevel level)
        { }

        public void RemoveCaste(CasteOptions caste)
        {
            mCasteOptions.Remove(caste);

            Castes.Remove(caste.ID);

            ValidateCasteOptions(Castes);

            InvalidateCache();
        }

        public void RemoveSim(Sim sim)
        {
            RemoveSim(sim.SimDescription);
        }
        public void RemoveSim(SimDescription sim)
        {
            Sims.Remove(new SimID(sim));
        }

        public override void InvalidateCache()
        {
            base.InvalidateCache();

            foreach (SimData sim in mSimData)
            {
                sim.InvalidateCache();
            }

            foreach (HouseholdOptions house in mHouseholdOptions)
            {
                house.InvalidateCache();
            }

            foreach (LotOptions lot in mLotOptions)
            {
                lot.InvalidateCache();
            }
        }

        public SimData GetSim(Sim sim)
        {
            return GetSim(sim.SimDescription);
        }
        public SimData GetSim(SimDescription sim)
        {
            SimID id = new SimID(sim);

            SimData data;
            if (!Sims.TryGetValue(id, out data))
            {
                data = new SimData(sim);

                Sims.Add(id, data);

                mSimData.Add(data);
            }

            data.SetSimDescription(sim);

            return data;
        }
        public T GetSim<T>(Sim sim) 
            where T : ElementalSimData, new()
        {
            return GetSim(sim).Get<T>();
        }
        public T GetSim<T>(SimDescription sim) 
            where T : ElementalSimData, new()
        {
            return GetSim(sim).Get<T>();
        }

        public bool ShowOptions ()
        {
            return new Managers.Main.MasterListingOption(NRaas.StoryProgression.Main).Perform();
        }

        public bool ShowImmigrantOptions(Manager manager, string localizedTitle)
        {
            return mImmigrantOptions.ShowOptions(manager, localizedTitle);
        }

        public List<DefaultableOption> GetImmigrantOptions(Manager manager)
        {
            return mImmigrantOptions.GetOptions(manager, "Immigrant", false);
        }

        public void StampImmigrantHousehold(Household house)
        {
            GetHouseOptions(house).CopyOptions(mImmigrantOptions);
        }

        public void UpdateSimData(SimUpdateScenario scenario, ScenarioFrame frame)
        {
            // This list can change size during the loop, cannot use "foreach"
            int i=0;
            while (i < mSimData.Count)
            {
                scenario.Perform(mSimData[i], frame);
                i++;
            }
        }

        public override void ValidateCasteOptions(Dictionary<ulong, CasteOptions> castes)
        {
            base.ValidateCasteOptions(castes);

            foreach (SimData option in Sims.Values)
            {
                option.ValidateCasteOptions(castes);
            }

            foreach (CasteOptions option in Castes.Values)
            {
                option.ValidateCasteOptions(castes);
            }

            mImmigrantOptions.ValidateCasteOptions(castes);

            foreach (HouseholdOptions option in Households.Values)
            {
                option.ValidateCasteOptions(castes);
            }

            foreach (LotOptions option in Lots.Values)
            {
                option.ValidateCasteOptions(castes);
            }
        }

        public override void Restore()
        {
            base.Restore();

            foreach (SimData option in Sims.Values)
            {
                option.Restore();
            }

            foreach (CasteOptions option in Castes.Values)
            {
                option.Restore();
            }

            mImmigrantOptions.Restore();

            foreach (HouseholdOptions option in Households.Values)
            {
                option.Restore();
            }

            foreach (LotOptions option in Lots.Values)
            {
                option.Restore();
            }
        }

        protected override bool RetainAllOptions
        {
            get { return true; }
        }

        /*
        public void RemoveSim(ulong sim)
        {
            Sims.Remove(new SimID(sim));

            int index = 0;
            while (index < mSimData.Count)
            {
                SimData data = mSimData[index];

                if (SimID.Matches (data.SimID, sim))
                {
                    mSimData.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }
        */

        public CasteOptions GetNewCasteOptions(string defaultName, string name, out bool created)
        {
            created = false;

            if (!string.IsNullOrEmpty(defaultName))
            {
                ManagerCaste.AddDefaultCaste(defaultName);

                foreach (CasteOptions option in AllCastes)
                {
                    if (option.DefaultName == defaultName)
                    {
                        StoryProgression.Main.GetOption<ManagerCaste.CreatedCastesOption>().AddValue(defaultName);

                        return option;
                    }
                }

                if (StoryProgression.Main.GetValue<ManagerCaste.CreatedCastesOption, List<string>>().Contains(defaultName)) return null;
            }

            if (mNextCasteID == 0)
            {
                foreach (ulong id in Castes.Keys)
                {
                    if (mNextCasteID < id)
                    {
                        mNextCasteID = id;
                    }
                }
            }

            mNextCasteID++;

            CasteOptions options = GetCasteOptions(mNextCasteID);
            if (options == null) return null;

            options.SetValue<CasteNameOption, string>(name);

            if (!string.IsNullOrEmpty(defaultName))
            {
                options.SetValue<CasteDefaultNameOption, string>(defaultName);

                StoryProgression.Main.GetOption<ManagerCaste.CreatedCastesOption>().AddValue(defaultName);
            }

            created = true;
            return options;
        }

        public CasteOptions GetCasteOptions(ulong id)
        {
            CasteOptions result;
            if (!Castes.TryGetValue(id, out result))
            {
                result = new CasteOptions(id);

                mCasteOptions.Add(result);

                Castes.Add(id, result);
            }

            return result;
        }

        public LotOptions GetLotOptions(Lot lot)
        {
            if (lot == null)
            {
                return null;
            }
            else
            {
                LotOptions options;
                if (!Lots.TryGetValue(lot.LotId, out options))
                {
                    options = new LotOptions(lot);

                    mLotOptions.Add(options);

                    Lots.Add(lot.LotId, options);
                }

                return options;
            }
        }

        public HouseholdOptions GetHouseOptions(Household house)
        {
            if (house == null)
            {
                return null;
            }
            else
            {
                HouseholdOptions options;
                if (!Households.TryGetValue(house.HouseholdId, out options))
                {
                    options = new HouseholdOptions(house);

                    mHouseholdOptions.Add(options);

                    Households.Add(house.HouseholdId, options);
                }

                return options;
            }
        }

        public override string ToString()
        {
            Common.StringBuilder result = new Common.StringBuilder("<TownOptions>");

            result += base.ToString();

            result += Common.NewLine + "</TownOptions>";

            return result.ToString();
        }
    }
}

