using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.RetunerSpace.Booters;
using NRaas.RetunerSpace.Helpers.Stores;
using NRaas.RetunerSpace.Options.Tunable;
using NRaas.RetunerSpace.Options.Tunable.Fields;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace
{
    [Persistable]
    public class SeasonSettings : IPersistence
    {
        SettingsKey mKey;

        Dictionary<string, ITUNSettings> mSettings = new Dictionary<string, ITUNSettings>();

        Dictionary<string, ActionDataSetting> mActionData = new Dictionary<string, ActionDataSetting>();

        List<TunableStore> mTunables = new List<TunableStore>();

        [Persistable(false)]
        Dictionary<Type, List<TunableStore>> mTunableLookup = null;

        public SeasonSettings()
        { }
        public SeasonSettings(SettingsKey key)
        {
            mKey = key;
        }

        protected Dictionary<Type, List<TunableStore>> Tunables
        {
            get
            {
                if (mTunableLookup == null)
                {
                    mTunableLookup = new Dictionary<Type, List<TunableStore>>();

                    for (int i = mTunables.Count - 1; i >= 0; i--)
                    {
                        TunableStore store = mTunables[i];

                        if ((store == null) || (store.ParentType == null))
                        {
                            mTunables.RemoveAt(i);
                        }
                        else
                        {
                            List<TunableStore> list;
                            if (!mTunableLookup.TryGetValue(store.ParentType, out list))
                            {
                                list = new List<TunableStore>();
                                mTunableLookup.Add(store.ParentType, list);
                            }

                            list.Add(store);
                        }
                    }
                }

                return mTunableLookup;
            }
        }

        public bool HasSettings
        {
            get
            {
                if (mSettings.Count > 0) return true;

                if (mTunables.Count > 0) return true;

                if (mActionData.Count > 0) return true;

                return false;
            }
        }

        public SettingsKey Key
        {
            get { return mKey; }
        }

        public bool IsEqual(SettingsKey key)
        {
            return mKey.IsEqual(key);
        }

        public void SetKey(SettingsKey key)
        {
            mKey = key;
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public void Import(Persistence.Lookup settings)
        {
            mKey = settings.GetChild<SettingsKey>("Key");

            mSettings.Clear();
            foreach (ITUNSettings setting in settings.GetList<ITUNSettings>("Settings"))
            {
                mSettings[setting.mName] = setting;
            }

            mTunables = settings.GetList<TunableStore>("Tunables");
            mTunableLookup = null;

            mActionData.Clear();
            foreach (ActionDataSetting setting in settings.GetList<ActionDataSetting>("ActionData"))
            {
                mActionData[setting.mName] = setting;
            }

            Apply();
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.AddChild("Key", mKey);

            settings.Add("Settings", mSettings.Values);
            settings.Add("Tunables", mTunables);
            settings.Add("ActionData", mActionData.Values);
        }

        public void StoreDefaultSettings(ActionData data)
        {
            PrivateGetSettings(data, true, true);
        }
        public ActionDataSetting GetSettings(ActionData data, bool create)
        {
            return PrivateGetSettings(data, create, false);
        }

        protected ActionDataSetting PrivateGetSettings(ActionData data, bool create, bool isDefault)
        {
            ActionDataSetting settings;
            if (!mActionData.TryGetValue(data.Key, out settings))
            {
                if (create)
                {
                    settings = new ActionDataSetting(data, isDefault);
                    mActionData.Add(data.Key, settings);
                }
                else
                {
                    return null;
                }
            }

            return settings;
        }

        public void ApplyLegacySetting(PersistedSettings.Settings oldSetting)
        {
            // Convert from old class to new class
            mSettings[oldSetting.mName] = new ITUNSettings(oldSetting);
        }

        public void StoreDefaultSettings(InteractionTuning data)
        {
            PrivateGetSettings(data, true, true);
        }
        public ITUNSettings GetSettings(InteractionTuning data, bool create)
        {
            return PrivateGetSettings(data, create, false);
        }

        protected ITUNSettings PrivateGetSettings(InteractionTuning tuning, bool create, bool isDefault)
        {
            string name = Retuner.TuningName(tuning.FullObjectName, tuning.FullInteractionName);

            ITUNSettings settings;
            if (!mSettings.TryGetValue(name, out settings)) 
            {
                if (create)
                {
                    settings = new ITUNSettings(name, tuning, isDefault);
                    mSettings.Add(name, settings);
                }
                else
                {
                    return null;
                }
            }

            return settings;
        }

        public void SetToDefault()
        {
            List<ITUNSettings> itunSettings = InteractionBooter.GetSettings(mKey);

            foreach (ITUNSettings setting in itunSettings)
            {
                mSettings[setting.mName] = setting;
            }

            Dictionary<string, bool> matched = new Dictionary<string, bool>();

            foreach (InteractionTuning tuning in InteractionTuning.sAllTunings.Values)
            {
                ITUNSettings setting = GetSettings(tuning, false);
                if (setting == null) continue;

                matched.Add(setting.mName, true);
            }

            foreach (ITUNSettings setting in itunSettings)
            {
                if (matched.ContainsKey(setting.mName)) continue;

                BooterLogger.AddError("Unmatched ITUN:" + Common.NewLine + " ObjectName: " + setting.mName.Replace("|", Common.NewLine + " InteractionName:"));
            }

            foreach(TunableStore setting in TuningBooter.GetSettings(mKey))
            {
                AddTunable(setting, true);
            }

            foreach (ActionDataSetting setting in SocialBooter.GetSettings(mKey))
            {
                ActionData data = ActionData.Get(setting.mName);
                if (data != null)
                {
                    mActionData[data.Key] = setting;
                }
            }
        }

        public void Apply()
        {
            foreach (InteractionTuning tuning in InteractionTuning.sAllTunings.Values)
            {
                ITUNSettings setting = GetSettings(tuning, false);
                if (setting == null) continue;

                setting.Apply(Key, tuning);
            }

            for (int i = mTunables.Count - 1; i >= 0; i--)
            {
                TunableStore setting = mTunables[i];

                if (!setting.Valid)
                {
                    mTunables.RemoveAt(i);
                }
                else
                {
                    setting.Apply(Key);
                }
            }

            List<string> remove = new List<string>();
            
            foreach(ActionDataSetting setting in mActionData.Values)
            {
                ActionData data = ActionData.Get(setting.mName);
                if (data != null)
                {
                    setting.Apply(Key, data);
                }
                else
                {
                    remove.Add(setting.mName);
                }
            }

            foreach (string key in remove)
            {
                mActionData.Remove(key);
            }
        }

        public TunableStore GetTunable(TunableStore setting)
        {
            List<TunableStore> list;
            if (Tunables.TryGetValue(setting.ParentType, out list))
            {
                foreach (TunableStore store in list)
                {
                    if (setting.IsEqual(store))
                    {
                        return store;
                    }
                }
            }

            return null;
        }

        public void AddTunable(TunableStore newSetting, bool replace)
        {
            List<TunableStore> list;
            if (!Tunables.TryGetValue(newSetting.ParentType, out list))
            {
                list = new List<TunableStore>();
                Tunables.Add(newSetting.ParentType, list);
            }

            for (int i=list.Count-1; i>=0; i--)
            {
                TunableStore oldSetting = list[i];

                if (oldSetting.IsEqual(newSetting))
                {
                    if (!replace) return;

                    mTunables.Remove(oldSetting);

                    list.RemoveAt(i);
                    break;
                }
            }

            list.Add(newSetting);

            mTunables.Add(newSetting);
        }

        public static string ToXMLString(IEnumerable<SeasonSettings> settings)
        {
            string result = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

            result += Common.NewLine + "<Tuning>";
            result += Common.NewLine + "  <ITUN>";
            result += Common.NewLine + "    <!-- This is the default set, do not alter or remove -->";
            result += Common.NewLine + "    <StartHour></StartHour>";
            result += Common.NewLine + "    <EndHour></EndHour>";
            result += Common.NewLine + "    <Season></Season>";
            result += Common.NewLine + "    <ObjectName></ObjectName>";
            result += Common.NewLine + "    <InteractionName></InteractionName>";
            result += Common.NewLine + "  </ITUN>";

            foreach (SeasonSettings setting in settings)
            {
                foreach (ITUNSettings itunSetting in setting.mSettings.Values)
                {
                    result += Common.NewLine + itunSetting.ToXMLString(setting.Key);
                }
            }

            result += Common.NewLine + "  <Social>";
            result += Common.NewLine + "    <!-- This is the default set, do not alter or remove -->";
            result += Common.NewLine + "    <StartHour></StartHour>";
            result += Common.NewLine + "    <EndHour></EndHour>";
            result += Common.NewLine + "    <Season></Season>";
            result += Common.NewLine + "    <Name></Name>";
            result += Common.NewLine + "  </Social>";

            foreach (SeasonSettings setting in settings)
            {
                foreach (ActionDataSetting adSetting in setting.mActionData.Values)
                {
                    result += Common.NewLine + adSetting.ToXMLString(setting.Key);
                }
            }

            result += Common.NewLine + "  <XML>";
            result += Common.NewLine + "    <!-- This is the default set, do not alter or remove -->";
            result += Common.NewLine + "    <StartHour></StartHour>";
            result += Common.NewLine + "    <EndHour></EndHour>";
            result += Common.NewLine + "    <Season></Season>";
            result += Common.NewLine + "    <FullClassName></FullClassName>";
            result += Common.NewLine + "    <FieldName></FieldName>";
            result += Common.NewLine + "    <SubFieldName></SubFieldName>";
            result += Common.NewLine + "    <Index></Index>";
            result += Common.NewLine + "    <Value></Value>";
            result += Common.NewLine + "  </XML>";

            foreach (SeasonSettings setting in settings)
            {
                foreach (TunableStore tuneSetting in setting.mTunables)
                {
                    result += Common.NewLine + tuneSetting.ToXMLString(setting.Key);
                }
            }

            result += Common.NewLine + "</Tuning>";

            return result;
        }

        [Persistable]
        public class ActionDataSetting : IPersistence
        {
            public enum Flags : uint
            {
                None = 0x0,
                Autonomous = 0x01,
                UserDirected = 0x02,
                ActorAgeSpecies = 0x04,
                TargetAgeSpecies = 0x08,
                AllowPregnant = 0x10,
                All = 0x1f,
            }

            public string mName;

            Flags mFlags = Flags.None;

            bool mAutonomous;
            bool mUserDirected;
            bool mAllowPregnant;

            CASAGSAvailabilityFlags mActorAgeSpecies = CASAGSAvailabilityFlags.None;

            [Persistable(false)]
            List<CASAGSAvailabilityFlags> mActorAgeSpeciesList = null;

            CASAGSAvailabilityFlags mTargetAgeSpecies = CASAGSAvailabilityFlags.None;

            [Persistable(false)]
            List<CASAGSAvailabilityFlags> mTargetAgeSpeciesList = null;

            public ActionDataSetting()
            { }
            protected ActionDataSetting(string name)
            {
                mName = name;
            }
            public ActionDataSetting(ActionData data, bool isDefault)
                : this(data.Key)
            {
                mAutonomous = data.Autonomous;
                mUserDirected = data.UserDirectedOnly;
                mAllowPregnant = !data.DisallowedIfPregnant;
                mActorAgeSpecies = data.ActorAgeSpeciesAllowed;
                mTargetAgeSpecies = data.TargetAgeSpeciesAllowed;

                if (isDefault)
                {
                    mFlags = Flags.All;
                }
            }
            public ActionDataSetting(string name, XmlDbRow row)
                : this(name)
            {
                mFlags = Flags.None;

                if (row.Exists("Autonomous"))
                {
                    mAutonomous = row.GetBool("Autonomous");
                    mFlags |= Flags.Autonomous;
                }

                if (row.Exists("UserDirected"))
                {
                    mUserDirected = row.GetBool("UserDirected");
                    mFlags |= Flags.UserDirected;
                }

                if (row.Exists("AllowPregnant"))
                {
                    mAllowPregnant = row.GetBool("AllowPregnant");
                    mFlags |= Flags.AllowPregnant;
                }

                if (row.Exists("ActorAgeSpecies"))
                {
                    mActorAgeSpecies = row.GetEnum<CASAGSAvailabilityFlags>("ActorAgeSpecies", CASAGSAvailabilityFlags.All);
                    if (mActorAgeSpecies == CASAGSAvailabilityFlags.All)
                    {
                        BooterLogger.AddError("Unknown ActorAgeSpecies: " + row.GetString("ActorAgeSpecies"));
                    }
                    else
                    {
                        mFlags |= Flags.ActorAgeSpecies;

                        mActorAgeSpeciesList = null;
                    }
                }

                if (row.Exists("TargetAgeSpecies"))
                {
                    mTargetAgeSpecies = row.GetEnum<CASAGSAvailabilityFlags>("TargetAgeSpecies", CASAGSAvailabilityFlags.All);
                    if (mTargetAgeSpecies == CASAGSAvailabilityFlags.All)
                    {
                        BooterLogger.AddError("Unknown TargetAgeSpecies: " + row.GetString("TargetAgeSpecies"));
                    }
                    else
                    {
                        mFlags |= Flags.TargetAgeSpecies;

                        mTargetAgeSpeciesList = null;
                    }
                }
            }

            public void Import(Persistence.Lookup settings)
            {
                mName = settings.GetString("Name");
                if (settings.Exists("Autonomous"))
                {
                    mAutonomous = settings.GetBool("Autonomous", false);
                    mFlags |= Flags.Autonomous;
                }

                if (settings.Exists("UserDirected"))
                {
                    mUserDirected = settings.GetBool("UserDirected", false);
                    mFlags |= Flags.UserDirected;
                }

                if (settings.Exists("AllowPregnant"))
                {
                    mAllowPregnant = settings.GetBool("AllowPregnant", false);
                    mFlags |= Flags.AllowPregnant;
                }

                if (settings.Exists("ActorAgeSpecies"))
                {
                    mActorAgeSpecies = settings.GetEnum<CASAGSAvailabilityFlags>("ActorAgeSpecies", CASAGSAvailabilityFlags.None);
                    mFlags |= Flags.ActorAgeSpecies;

                    mActorAgeSpeciesList = null;
                }

                if (settings.Exists("TargetAgeSpecies"))
                {
                    mTargetAgeSpecies = settings.GetEnum<CASAGSAvailabilityFlags>("TargetAgeSpecies", CASAGSAvailabilityFlags.None);
                    mFlags |= Flags.TargetAgeSpecies;

                    mTargetAgeSpeciesList = null;
                }
            }

            public void Export(Persistence.Lookup settings)
            {
                settings.Add("Name", mName);

                if ((mFlags & Flags.Autonomous) == Flags.Autonomous)
                {
                    settings.Add("Autonomous", mAutonomous);
                }

                if ((mFlags & Flags.UserDirected) == Flags.UserDirected)
                {
                    settings.Add("UserDirected", mUserDirected);
                }

                if ((mFlags & Flags.AllowPregnant) == Flags.AllowPregnant)
                {
                    settings.Add("AllowPregnant", mAllowPregnant);
                }

                if ((mFlags & Flags.ActorAgeSpecies) == Flags.ActorAgeSpecies)
                {
                    settings.Add("ActorAgeSpecies", mActorAgeSpecies.ToString());
                }

                if ((mFlags & Flags.TargetAgeSpecies) == Flags.TargetAgeSpecies)
                {
                    settings.Add("TargetAgeSpecies", mTargetAgeSpecies.ToString());
                }
            }

            public string PersistencePrefix
            {
                get { return mName; }
            }

            public bool GetAutonomous(out bool result)
            {
                result = mAutonomous;

                return ((mFlags & Flags.Autonomous) != Flags.None);
            }

            public bool GetUserDirected(out bool result)
            {
                result = mUserDirected;

                return ((mFlags & Flags.UserDirected) != Flags.None);
            }

            public bool GetAllowPregnant(out bool result)
            {
                result = mAllowPregnant;

                return ((mFlags & Flags.AllowPregnant) != Flags.None);
            }

            public bool GetActorAgeSpecies(out List<CASAGSAvailabilityFlags> result)
            {
                result = ActorAgeSpecies;

                return ((mFlags & Flags.ActorAgeSpecies) != Flags.None);
            }

            protected List<CASAGSAvailabilityFlags> ActorAgeSpecies
            {
                get 
                {
                    if (mActorAgeSpeciesList == null)
                    {
                        mActorAgeSpeciesList = Retuner.AgeSpeciesToList(mActorAgeSpecies);
                    }

                    return mActorAgeSpeciesList; 
                }
            }

            public bool GetTargetAgeSpecies(out List<CASAGSAvailabilityFlags> result)
            {
                result = TargetAgeSpecies;

                return ((mFlags & Flags.TargetAgeSpecies) != Flags.None);
            }

            protected List<CASAGSAvailabilityFlags> TargetAgeSpecies
            {
                get 
                {
                    if (mTargetAgeSpeciesList == null)
                    {
                        mTargetAgeSpeciesList = Retuner.AgeSpeciesToList(mTargetAgeSpecies);
                    }

                    return mTargetAgeSpeciesList; 
                }
            }

            public void SetUserDirected(SettingsKey key, ActionData data, bool value)
            {
                mUserDirected = value;

                mFlags |= Flags.UserDirected;

                if (Retuner.StoreDefault(key, data))
                {
                    data.mUserDirectedOnly = value;
                }
            }

            public void SetAutonomous(SettingsKey key, ActionData data, bool value)
            {
                mAutonomous = value;

                mFlags |= Flags.Autonomous;

                if (Retuner.StoreDefault(key, data))
                {
                    data.mAutonomous = value;
                }
            }

            public void SetAllowPregnant(SettingsKey key, ActionData data, bool value)
            {
                mAllowPregnant = value;

                mFlags |= Flags.AllowPregnant;

                if (Retuner.StoreDefault(key, data))
                {
                    // Inverted
                    if (!value)
                    {
                        data.mDataBits |= ActionDataBase.ActionDataBits.DisallowedIfPregnant;
                    }
                    else
                    {
                        data.mDataBits &= ~ActionDataBase.ActionDataBits.DisallowedIfPregnant;
                    }
                }
            }

            public void SetActorAgeSpecies(SettingsKey key, ActionData data, List<CASAGSAvailabilityFlags> ageSpeciesList)
            {
                mActorAgeSpeciesList = null;

                mFlags |= Flags.ActorAgeSpecies;

                mActorAgeSpecies = CASAGSAvailabilityFlags.None;
                foreach (CASAGSAvailabilityFlags ageSpecies in ageSpeciesList)
                {
                    mActorAgeSpecies |= ageSpecies;
                }

                if (Retuner.StoreDefault(key, data))
                {
                    data.mActorAgeAllowed = mActorAgeSpecies;
                }
            }

            public void SetTargetAgeSpecies(SettingsKey key, ActionData data, List<CASAGSAvailabilityFlags> ageSpeciesList)
            {
                mTargetAgeSpeciesList = null;

                mFlags |= Flags.TargetAgeSpecies;

                mTargetAgeSpecies = CASAGSAvailabilityFlags.None;
                foreach (CASAGSAvailabilityFlags ageSpecies in ageSpeciesList)
                {
                    mTargetAgeSpecies |= ageSpecies;
                }

                if (Retuner.StoreDefault(key, data))
                {
                    data.mTargetAgeAllowed = mTargetAgeSpecies;
                }
            }

            public void Apply(SettingsKey key, ActionData data)
            {
                if ((mFlags & Flags.Autonomous) != Flags.None)
                {
                    SetAutonomous(key, data, mAutonomous);
                }

                if ((mFlags & Flags.UserDirected) != Flags.None)
                {
                    SetUserDirected(key, data, mUserDirected);
                }

                if ((mFlags & Flags.AllowPregnant) != Flags.None)
                {
                    SetAllowPregnant(key, data, mAllowPregnant);
                }

                if ((mFlags & Flags.ActorAgeSpecies) != Flags.None)
                {
                    SetActorAgeSpecies(key, data, ActorAgeSpecies);
                }

                if ((mFlags & Flags.TargetAgeSpecies) != Flags.None)
                {
                    SetTargetAgeSpecies(key, data, TargetAgeSpecies);
                }
            }

            public override string ToString()
            {
                return ToXMLString(null);
            }

            public string ToXMLString(SettingsKey key)
            {
                string result = null;

                result += "  <Social>";

                if (key != null)
                {
                    result += key.ToXMLString();
                }

                result += Common.NewLine + "    <Name>" + mName + "</Name>";

                if ((mFlags & Flags.Autonomous) != Flags.None)
                {
                    result += Common.NewLine + "    <Autonomous>" + mAutonomous + "</Autonomous>";
                }

                if ((mFlags & Flags.UserDirected) != Flags.None)
                {
                    result += Common.NewLine + "    <UserDirected>" + mUserDirected + "</UserDirected>";
                }

                if ((mFlags & Flags.AllowPregnant) != Flags.None)
                {
                    result += Common.NewLine + "    <AllowPregnant>" + mAllowPregnant + "</AllowPregnant>";
                }

                if ((mFlags & Flags.ActorAgeSpecies) != Flags.None)
                {
                    result += Common.NewLine + "    <ActorAgeSpecies>" + mActorAgeSpecies + "</ActorAgeSpecies>";
                }

                if ((mFlags & Flags.TargetAgeSpecies) != Flags.None)
                {
                    result += Common.NewLine + "    <TargetAgeSpecies>" + mTargetAgeSpecies + "</TargetAgeSpecies>";
                }

                result += Common.NewLine + "  </Social>";

                return result;
            }
        }

        [Persistable]
        public class ITUNSettings : IPersistence
        {
            public enum Flags : uint
            {
                None = 0x0,
                Autonomous = 0x01,
                UserDirected = 0x02,
                AgeSpecies = 0x04,
                Availability = 0x08,
                All = 0x0f,
            }

            public string mName;

            Flags mFlags = Flags.None;

            bool mAutonomous;
            bool mUserDirected;

            Availability.FlagField mAvailability = Availability.FlagField.None;

            CASAGSAvailabilityFlags mAgeSpecies = CASAGSAvailabilityFlags.None;

            [Persistable(false)]
            List<CASAGSAvailabilityFlags> mAgeSpeciesList = null;

            Dictionary<CommodityKind, float> mAdvertisedOutputs = new Dictionary<CommodityKind, float>();

            public ITUNSettings()
            { }
            protected ITUNSettings(string name)
            {
                mName = name;
            }
            public ITUNSettings(string name, InteractionTuning tuning, bool isDefault)
                : this(name)
            {
                mAutonomous = !tuning.HasFlags(InteractionTuning.FlagField.DisallowAutonomous);
                mUserDirected = !tuning.HasFlags(InteractionTuning.FlagField.DisallowUserDirected);
                mAgeSpecies = tuning.Availability.AgeSpeciesAvailabilityFlags;
                mAvailability = tuning.Availability.mFlags;

                if (isDefault)
                {
                    mFlags = Flags.All;
                }
            }
            public ITUNSettings(ITUNSettings setting)
                : this(setting.mName)
            {
                mAutonomous = setting.mAutonomous;
                mUserDirected = setting.mUserDirected;
                mAgeSpecies = setting.mAgeSpecies;
                mAvailability = setting.mAvailability;
                mAdvertisedOutputs = new Dictionary<CommodityKind, float>(mAdvertisedOutputs);
                mFlags = setting.mFlags;
            }
            public ITUNSettings(string name, XmlDbRow row)
                : this(name)
            {
                mFlags = Flags.None;

                if (row.Exists("Autonomous"))
                {
                    mAutonomous = row.GetBool("Autonomous");
                    mFlags |= Flags.Autonomous;
                }

                if (row.Exists("UserDirected"))
                {
                    mUserDirected = row.GetBool("UserDirected");
                    mFlags |= Flags.UserDirected;
                }

                if (row.Exists("AgeSpecies"))
                {
                    mAgeSpecies = row.GetEnum<CASAGSAvailabilityFlags>("AgeSpecies", CASAGSAvailabilityFlags.None);
                    if (mAgeSpecies == CASAGSAvailabilityFlags.None)
                    {
                        BooterLogger.AddError("Unknown AgeSpecies: " + row.GetString("AgeSpecies"));
                    }
                    else
                    {
                        mFlags |= Flags.AgeSpecies;

                        mAgeSpeciesList = null;
                    }
                }

                if (row.Exists("Availability"))
                {
                    mAvailability = row.GetEnum<Availability.FlagField>("Availability", Availability.FlagField.None);
                    if (mAvailability == Availability.FlagField.None)
                    {
                        BooterLogger.AddError("Unknown Availability: " + row.GetString("Availability"));
                    }
                    else
                    {
                        mFlags |= Flags.Availability;
                    }
                }

                if (row.Exists("Advertised"))
                {
                    ParseAdvertised(row.GetStringList("Advertised", ','));
                }
            }

            protected void ParseAdvertised(IList<string> list)
            {
                foreach (string advertised in list)
                {
                    string[] pair = advertised.Split(':');
                    if (pair.Length != 2) continue;

                    CommodityKind kind;
                    if (!ParserFunctions.TryParseEnum<CommodityKind>(pair[0], out kind, CommodityKind.None))
                    {
                        BooterLogger.AddError("Unknown Advertised Commodity: " + pair[0]);
                    }
                    else
                    {
                        float value;
                        if (!float.TryParse(pair[1], out value))
                        {
                            BooterLogger.AddError("Unparsable Advertised Value: " + pair[1]);
                        }
                        else
                        {
                            if (mAdvertisedOutputs.ContainsKey(kind))
                            {
                                BooterLogger.AddError("Duplicate Advertised Commodity: " + pair[0]);
                            }
                            else
                            {
                                mAdvertisedOutputs.Add(kind, value);
                            }
                        }
                    }
                }
            }

            public void Import(Persistence.Lookup settings)
            {
                mName = settings.GetString("Name");
                if (settings.Exists("Autonomous"))
                {
                    mAutonomous = settings.GetBool("Autonomous", false);
                    mFlags |= Flags.Autonomous;
                }

                if (settings.Exists("UserDirected"))
                {
                    mUserDirected = settings.GetBool("UserDirected", false);
                    mFlags |= Flags.UserDirected;
                }

                if (settings.Exists("AgeSpecies"))
                {
                    mAgeSpecies = settings.GetEnum<CASAGSAvailabilityFlags>("AgeSpecies", CASAGSAvailabilityFlags.None);
                    mFlags |= Flags.AgeSpecies;

                    mAgeSpeciesList = null;
                }

                if (settings.Exists("Availability"))
                {
                    mAvailability = settings.GetEnum<Availability.FlagField>("Availability", Availability.FlagField.None);
                    mFlags |= Flags.Availability;
                }

                if (settings.Exists("Advertised"))
                {
                    mAdvertisedOutputs.Clear();
                    ParseAdvertised(settings.GetStringList("Advertised"));
                }
            }

            public void Export(Persistence.Lookup settings)
            {
                settings.Add("Name", mName);

                if ((mFlags & Flags.Autonomous) == Flags.Autonomous)
                {
                    settings.Add("Autonomous", mAutonomous);
                }

                if ((mFlags & Flags.UserDirected) == Flags.UserDirected)
                {
                    settings.Add("UserDirected", mUserDirected);
                }

                if ((mFlags & Flags.AgeSpecies) == Flags.AgeSpecies)
                {
                    settings.Add("AgeSpecies", mAgeSpecies.ToString());
                }

                if ((mFlags & Flags.Availability) == Flags.Availability)
                {
                    settings.Add("Availability", mAvailability.ToString());
                }

                settings.Add("Advertised", AdvertisedToString());
            }

            protected string AdvertisedToString()
            {
                string advertised = null;
                foreach (KeyValuePair<CommodityKind, float> pair in mAdvertisedOutputs)
                {
                    if (advertised != null)
                    {
                        advertised += ',';
                    }

                    advertised += pair.Key + ":" + pair.Value;
                }

                return advertised;
            }

            public string PersistencePrefix
            {
                get { return mName; }
            }

            public bool GetAutonomous(out bool result)
            {
                result = mAutonomous;

                return ((mFlags & Flags.Autonomous) != Flags.None);
            }

            public bool GetUserDirected(out bool result)
            {
                result = mUserDirected;

                return ((mFlags & Flags.UserDirected) != Flags.None);
            }

            public bool GetAgeSpecies(out List<CASAGSAvailabilityFlags> result)
            {
                result = AgeSpecies;

                return ((mFlags & Flags.AgeSpecies) != Flags.None);
            }

            protected List<CASAGSAvailabilityFlags> AgeSpecies
            {
                get 
                {
                    if (mAgeSpeciesList == null)
                    {
                        mAgeSpeciesList = Retuner.AgeSpeciesToList(mAgeSpecies);
                    }

                    return mAgeSpeciesList; 
                }
            }

            public bool GetAdvertised(CommodityKind kind, out float result)
            {
                return mAdvertisedOutputs.TryGetValue(kind, out result);
            }

            public void SetAdvertised(CommodityKind kind, float value)
            {
                mAdvertisedOutputs[kind] = value;
            }

            public bool HasAvailability(Availability.FlagField flag, out bool result)
            {
                result = ((mAvailability & flag) == flag);

                return ((mFlags & Flags.Availability) != Flags.None);
            }

            public void SetUserDirected(SettingsKey key, InteractionTuning tuning, bool value)
            {
                mUserDirected = value;

                mFlags |= Flags.UserDirected;

                if (Retuner.StoreDefault(key, tuning))
                {
                    if (value)
                    {
                        tuning.RemoveFlags(InteractionTuning.FlagField.DisallowUserDirected);
                    }
                    else
                    {
                        tuning.AddFlags(InteractionTuning.FlagField.DisallowUserDirected);
                    }
                }
            }

            public void SetAutonomous(SettingsKey key, InteractionTuning tuning, bool value)
            {
                mAutonomous = value;

                mFlags |= Flags.Autonomous;

                if (Retuner.StoreDefault(key, tuning))
                {
                    if (value)
                    {
                        tuning.RemoveFlags(InteractionTuning.FlagField.DisallowAutonomous);
                    }
                    else
                    {
                        tuning.AddFlags(InteractionTuning.FlagField.DisallowAutonomous);
                    }
                }
            }

            public void SetAgeSpecies(SettingsKey key, InteractionTuning tuning, List<CASAGSAvailabilityFlags> ageSpeciesList)
            {
                mAgeSpeciesList = null;

                mFlags |= Flags.AgeSpecies;

                mAgeSpecies = CASAGSAvailabilityFlags.None;
                foreach (CASAGSAvailabilityFlags ageSpecies in ageSpeciesList)
                {
                    mAgeSpecies |= ageSpecies;
                }

                if (Retuner.StoreDefault(key, tuning))
                {
                    tuning.Availability.AgeSpeciesAvailabilityFlags = mAgeSpecies;
                }
            }

            public void SetAvailability(SettingsKey key, InteractionTuning tuning, Availability.FlagField availability, bool set)
            {
                if ((mFlags & Flags.Availability) == Flags.None)
                {
                    mAvailability = tuning.Availability.mFlags;
                }

                mFlags |= Flags.Availability;

                if (set)
                {
                    mAvailability |= availability;
                }
                else
                {
                    mAvailability &= ~availability;
                }

                if (Retuner.StoreDefault(key, tuning))
                {
                    if (set)
                    {
                        tuning.Availability.AddFlags(availability);
                    }
                    else
                    {
                        tuning.Availability.RemoveFlags(availability);
                    }
                }
            }
            protected void SetAvailability(SettingsKey key, InteractionTuning tuning, Availability.FlagField availability)
            {
                mAvailability = availability;

                mFlags |= Flags.Availability;

                if (Retuner.StoreDefault(key, tuning))
                {
                    tuning.Availability.mFlags = mAvailability;
                }
            }

            protected void SetAdvertised(SettingsKey key, InteractionTuning tuning, Dictionary<CommodityKind, float> advertised)
            {
                if (Retuner.StoreDefault(key, tuning))
                {
                    foreach (KeyValuePair<CommodityKind, float> pair in advertised)
                    {
                        foreach (CommodityChange change in tuning.mTradeoff.mOutputs)
                        {
                            if (change.Commodity == pair.Key)
                            {
                                change.mConstantChange = pair.Value;
                            }
                        }
                    }
                }
            }

            public void Apply(SettingsKey key, InteractionTuning tuning)
            {
                if ((mFlags & Flags.Autonomous) != Flags.None)
                {
                    SetAutonomous(key, tuning, mAutonomous);
                }

                if ((mFlags & Flags.UserDirected) != Flags.None)
                {
                    SetUserDirected(key, tuning, mUserDirected);
                }

                // Not Persisted, so rebuild now
                mAgeSpeciesList = null;

                if ((mFlags & Flags.AgeSpecies) != Flags.None)
                {
                    SetAgeSpecies(key, tuning, AgeSpecies);
                }

                if ((mFlags & Flags.Availability) != Flags.None)
                {
                    SetAvailability(key, tuning, mAvailability);
                }

                SetAdvertised(key, tuning, mAdvertisedOutputs);
            }

            public override string ToString()
            {
                return ToXMLString(null);
            }

            public string ToXMLString(SettingsKey key)
            {
                string result = null;

                string[] names = mName.Split('|');

                result += "  <ITUN>";

                if (key != null)
                {
                    result += key.ToXMLString();
                }

                result += Common.NewLine + "    <ObjectName>" + names[0] + "</ObjectName>";
                result += Common.NewLine + "    <InteractionName>" + names[1] + "</InteractionName>";

                if ((mFlags & Flags.Autonomous) != Flags.None)
                {
                    result += Common.NewLine + "    <Autonomous>" + mAutonomous + "</Autonomous>";
                }

                if ((mFlags & Flags.UserDirected) != Flags.None)
                {
                    result += Common.NewLine + "    <UserDirected>" + mUserDirected + "</UserDirected>";
                }

                if ((mFlags & Flags.AgeSpecies) != Flags.None)
                {
                    result += Common.NewLine + "    <AgeSpecies>" + mAgeSpecies + "</AgeSpecies>";
                }

                if ((mFlags & Flags.Availability) != Flags.None)
                {
                    result += Common.NewLine + "    <Availability>" + mAvailability + "</Availability>";
                }

                if (mAdvertisedOutputs.Count > 0)
                {
                    result += Common.NewLine + "    <Advertised>" + AdvertisedToString() + "</Advertised>";
                }

                result += Common.NewLine + "  </ITUN>";

                return result;
            }
        }
    }
}
