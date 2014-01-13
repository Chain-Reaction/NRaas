using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CareerSpace;
using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interactions;
using NRaas.CareerSpace.Interfaces;
using NRaas.CareerSpace.OmniSpace;
using NRaas.Gameplay.OmniSpace;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.CareerSpace.OmniSpace;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.Careers
{
    [Persistable]
    public class OmniCareer : Career, Common.IWorldLoadFinished, ICoworkerPool, ILotDesignator
    {
        Dictionary<Type, OmniValue> mValues = new Dictionary<Type, OmniValue>();

        protected int mNumJournalsRead = 0;

        protected int mNumRecruits = 0;

        protected int mShakedownFunds = 0;

        protected OmniJournal mJournal = null;

        [Persistable(false)]
        AlarmHandle mStipendAlarm;

        [Persistable(false)]
        protected NonPersistableData mOther = new NonPersistableData();

        static OmniCareer()
        { }

        public OmniCareer()
        { }
        public OmniCareer(XmlDbRow myRow, XmlDbTable levelTable, XmlDbTable eventDataTable)
            : this(myRow, levelTable, eventDataTable, null)
        {
            try
            {
                string transferCareer = myRow.GetString("TransferCareer");
                if ((transferCareer != null) && (transferCareer != ""))
                {
                    ParserFunctions.TryParseEnum<OccupationNames>(transferCareer, out mOther.mFullTimeEquivalent, OccupationNames.Undefined);

                    if (mOther.mFullTimeEquivalent == OccupationNames.Undefined)
                    {
                        mOther.mFullTimeEquivalent = unchecked((OccupationNames)ResourceUtils.HashString64(transferCareer));
                    }
                }

                if (myRow.Exists("LotDesignator"))
                {
                    mOther.mLotDesignator = myRow.GetString("LotDesignator");
                }

                if (myRow.Exists("BranchOnlyCoworkers"))
                {
                    mOther.mBranchOnlyCoworkers = myRow.GetBool("BranchOnlyCoworkers");
                }

                if (myRow.Exists("CoworkerPool"))
                {
                    mOther.mCoworkerPool = myRow.GetString("CoworkerPool");
                }

                if (myRow.Exists("CanPrank"))
                {
                    mOther.mCanPrank = myRow.GetBool("CanPrank");
                }

                if (!string.IsNullOrEmpty(myRow.GetString("Ages")))
                {
                    mOther.mAges = ParserFunctions.ParseAllowableAges(myRow, "Ages");
                }
                else
                {
                    if (IsPartTime)
                    {
                        mOther.mAges |= CASAgeGenderFlags.Teen;
                    }
                }

                if (!string.IsNullOrEmpty(myRow.GetString("Genders")))
                {
                    ParserFunctions.TryParseEnum<CASAgeGenderFlags>(myRow.GetString("Genders"), out mOther.mGenders, CASAgeGenderFlags.GenderMask);
                }

                for (int i = 1; i < 10; i++)
                {
                    string skillName = myRow.GetString("SkillNamePrerequisite" + i.ToString());
                    if (string.IsNullOrEmpty(skillName)) continue;

                    SkillNames skill = SkillNames.None;

                    try
                    {
                        skill = GenericManager<SkillNames, Skill, Skill>.ParseGuid(skillName);
                    }
                    catch
                    { }

                    if (skill == SkillNames.None)
                    {
                        skill = unchecked((SkillNames)ResourceUtils.HashString64(skillName));
                    }

                    int minSkill = myRow.GetInt("SkillValuePrerequisite" + i.ToString());
                    if (minSkill <= 0) continue;

                    if (mOther.mSkillPrerequisites.ContainsKey(skill))
                    {
                        mOther.mSkillPrerequisites[skill] = minSkill;
                    }
                    else
                    {
                        mOther.mSkillPrerequisites.Add(skill, minSkill);
                    }
                }

                foreach (XmlDbRow levelRow in levelTable.Rows)
                {
                    int level = levelRow.GetInt("Level");

                    string branch = levelRow.GetString("BranchName");

                    CareerLevel careerLevel = null;

                    Dictionary<int, CareerLevel> careerLevels;
                    if (SharedData.CareerLevels.TryGetValue(branch, out careerLevels))
                    {
                        careerLevels.TryGetValue(level, out careerLevel);
                    }

                    if (careerLevel == null) continue;

                    if (levelRow.Exists("OutfitMaleVersion"))
                    {
                        ProductVersion version;
                        levelRow.TryGetEnum<ProductVersion>("OutfitMaleVersion", out version, ProductVersion.BaseGame);

                        string outfit = levelRow.GetString("OutfitMale");
                        if (!string.IsNullOrEmpty(outfit))
                        {
                            careerLevel.OutfitResourceKeyMale = careerLevel.GetResourceKey(outfit, version);
                        }
                    }

                    if (levelRow.Exists("OutfitMaleElderVersion"))
                    {
                        ProductVersion version;
                        levelRow.TryGetEnum<ProductVersion>("OutfitMaleElderVersion", out version, ProductVersion.BaseGame);

                        string outfit = levelRow.GetString("OutfitMaleElder");
                        if (!string.IsNullOrEmpty(outfit))
                        {
                            careerLevel.OutfitResourceKeyMaleElder = careerLevel.GetResourceKey(outfit, version);
                        }
                    }

                    if (levelRow.Exists("OutfitFemaleVersion"))
                    {
                        ProductVersion version;
                        levelRow.TryGetEnum<ProductVersion>("OutfitFemaleVersion", out version, ProductVersion.BaseGame);

                        string outfit = levelRow.GetString("OutfitFemale");
                        if (!string.IsNullOrEmpty(outfit))
                        {
                            careerLevel.OutfitResourceKeyFemale = careerLevel.GetResourceKey(outfit, version);
                        }
                    }

                    if (levelRow.Exists("OutfitFemaleElderVersion"))
                    {
                        ProductVersion version;
                        levelRow.TryGetEnum<ProductVersion>("OutfitFemaleElderVersion", out version, ProductVersion.BaseGame);

                        string outfit = levelRow.GetString("OutfitFemaleElder");
                        if (!string.IsNullOrEmpty(outfit))
                        {
                            careerLevel.OutfitResourceKeyFemaleElder = careerLevel.GetResourceKey(outfit, version);
                        }
                    }

                    mOther.mLevels.Add(careerLevel, new NonPersistableData.CareerLevelData(levelRow));
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
        protected OmniCareer(XmlDbRow myRow, XmlDbTable levelTable, XmlDbTable eventDataTable, CareerSharedData sharedData)
        {
            try
            {
                mCareerLocOwnerGuid = ObjectGuid.InvalidObjectGuid;
                mCurLevelVal = -1;
                mWhenCurLevelStarted = SimClock.CurrentTime();
                LastTimeCalculated = new DateAndTime();
                mSkippedDayOfWork = true;
                mRegularWorkDayHourAfterStartHandle = AlarmHandle.kInvalidHandle;
                mRegularWorkDayFastWalkHandle = AlarmHandle.kInvalidHandle;
                mRegularWorkDayRunHandle = AlarmHandle.kInvalidHandle;
                LastTimeDemandedRaise = SimClock.CurrentTime();
                WorkaholicInteractionLocked = true;
                if ((myRow != null) && (levelTable != null))
                {
                    if (sharedData != null)
                    {
                        SharedData = sharedData;
                    }
                    else
                    {
                        SharedData = new CareerSharedData();
                    }
                    ParserFunctions.TryParseEnum<OccupationNames>(myRow.GetString("Guid"), out mCareerGuid, OccupationNames.Undefined);
                    SharedData.Name = myRow.GetString("CareerName");
                    SharedData.Name = "Gameplay/Excel/Careers/CareerList:" + SharedData.Name;
                    SharedData.OvertimeHours = myRow.GetFloat("OvertimeHours");
                    SharedData.MissWorkPerfPerHour = myRow.GetFloat("MissWorkPerf");
                    SharedData.OvertimePerfPerHour = myRow.GetFloat("OvertimePerf");
                    SharedData.MaxPerfFlowPerHour = myRow.GetFloat("MaxPerfFlow");
                    SharedData.MinPerfFlowPerHour = myRow.GetFloat("MinPerfFlow");
                    SharedData.MaxPerfFlowPerHourMaxLevel = myRow.GetFloat("MaxPerfFlowMaxLevel");
                    SharedData.MinPerfFlowPerHourMaxLevel = myRow.GetFloat("MinPerfFlowMaxLevel");
                    SharedData.BonusAmountInHours = myRow.GetFloat("BonusAmountInHours");
                    SharedData.RaisePercent = myRow.GetFloat("RaisePercent");
                    SharedData.RaisePercentMaxLevel = myRow.GetFloat("RaisePercentMaxLevel");
                    SharedData.RaiseChanceMin = myRow.GetFloat("RaiseChanceMin");
                    SharedData.RaiseChanceMax = myRow.GetFloat("RaiseChanceMax");
                    SharedData.FunPerHourWhileWorking = myRow.GetFloat("FunStressPerHour");
                    SharedData.MinCoworkers = myRow.GetInt("MinCoworkers");
                    SharedData.MaxCoworkers = myRow.GetInt("MaxCoworkers");
                    SharedData.Text_JobOffer = myRow.GetString("Text_JobOffer");
                    SharedData.Text_JobOffer = "Gameplay/Excel/Careers/CareerList:" + SharedData.Text_JobOffer;
                    SharedData.Text_Retirement = myRow.GetString("Text_Retirement");
                    SharedData.Text_Retirement = "Gameplay/Excel/Careers/CareerList:" + SharedData.Text_Retirement;
                    SharedData.Text_BranchOffer = myRow.GetString("Text_BranchOffer");
                    if (SharedData.Text_BranchOffer != string.Empty)
                    {
                        SharedData.Text_BranchOffer = "Gameplay/Excel/Careers/CareerList:" + SharedData.Text_BranchOffer;
                        SharedData.Text_Branch1 = myRow.GetString("Text_BranchName1");
                        SharedData.Text_Branch1 = "Gameplay/Excel/Careers/CareerList:" + SharedData.Text_Branch1;
                        SharedData.Text_Branch2 = myRow.GetString("Text_BranchName2");
                        SharedData.Text_Branch2 = "Gameplay/Excel/Careers/CareerList:" + SharedData.Text_Branch2;
                    }
                    SharedData.SpeechBalloonImage = myRow.GetString("SpeechBaloonImage");
                    SharedData.WorkInteractionIcon = myRow.GetString("WorkInteractionIcon");
                    SharedData.Topic = myRow.GetString("Topic");
                    SharedData.LearnedInfoText = myRow.GetString("LearnedInfoText");
                    AvailableInFutureWorld = myRow.GetBool("AvailableInFuture");
                    myRow.TryGetEnum<ProductVersion>("ProductVersion", out SharedData.ProductVersion, ProductVersion.BaseGame);
                    myRow.TryGetEnum<CareerCategory>("Category", out SharedData.Category, CareerCategory.FullTime);
                    SharedData.DreamsAndPromisesIcon = myRow.GetString("DreamsAndPromisesIcon");
                    SharedData.ToneDefinitions = new List<CareerToneDefinition>();
                    string name = myRow.GetString("WorkHardTone");
                    if (name.Length > 0x0)
                    {
                        name = "Gameplay/Excel/Careers/CareerList:" + name;
                        SharedData.ToneDefinitions.Add(new WorkHardTone.Definition(name));
                    }
                    name = myRow.GetString("TakeItEasyTone");
                    if (name.Length > 0x0)
                    {
                        name = "Gameplay/Excel/Careers/CareerList:" + name;
                        SharedData.ToneDefinitions.Add(new TakeItEasyTone.Definition(name));
                    }
                    name = myRow.GetString("MeetCoworkersTone");
                    if (name.Length > 0x0)
                    {
                        name = "Gameplay/Excel/Careers/CareerList:" + name;
                        if (mCareerGuid == (OccupationNames.Undefined | OccupationNames.Music))
                        {
                            SharedData.ToneDefinitions.Add(new Music.MeetBandTone.Definition(name));
                        }
                        else
                        {
                            SharedData.ToneDefinitions.Add(new MeetCoworkersTone.Definition(name));
                        }
                    }
                    name = myRow.GetString("HangWithCoworkersTone");
                    if (name.Length > 0x0)
                    {
                        name = "Gameplay/Excel/Careers/CareerList:" + name;
                        if (mCareerGuid == (OccupationNames.Undefined | OccupationNames.Music))
                        {
                            SharedData.ToneDefinitions.Add(new Music.ChillWithBandTone.Definition(name));
                        }
                        else
                        {
                            SharedData.ToneDefinitions.Add(new HangWithCoworkersTone.Definition(name));
                        }
                    }
                    name = myRow.GetString("SuckUpToBossTone");
                    if (name.Length > 0x0)
                    {
                        name = "Gameplay/Excel/Careers/CareerList:" + name;
                        SharedData.ToneDefinitions.Add(new SuckUpToBossTone.Definition(name));
                    }
                    name = myRow.GetString("SleepAtWorkTone");
                    if (name.Length > 0x0)
                    {
                        name = "Gameplay/Excel/Careers/CareerList:" + name;
                        SharedData.ToneDefinitions.Add(new SleepAtWorkTone.Definition(name));
                    }
                    else if (((mCareerGuid != OccupationNames.Medical) && (mCareerGuid != OccupationNames.SchoolElementary)) && (mCareerGuid != OccupationNames.SchoolHigh))
                    {
                        name = "Gameplay/Excel/Careers/CareerList:SleepAtWork";
                        SharedData.ToneDefinitions.Add(new SleepAtWorkTone.Definition(name));
                    }

                    SharedData.CareerLevels = new Dictionary<string, Dictionary<int, CareerLevel>>();
                    foreach (XmlDbRow row in levelTable.Rows)
                    {
                        CareerLevel level = new CareerLevel(row, myRow.GetString("TableName"), SharedData.ProductVersion);

                        try
                        {
                            if (CareerLevels.ContainsKey(level.BranchName))
                            {
                                CareerLevels[level.BranchName].Add(level.Level, level);
                                CareerLevels[level.BranchName][level.Level - 0x1].NextLevels.Add(level);
                                level.LastLevel = CareerLevels[level.BranchName][level.Level - 0x1];
                            }
                            else
                            {
                                Dictionary<int, CareerLevel> dictionary = new Dictionary<int, CareerLevel>();
                                dictionary.Add(level.Level, level);
                                CareerLevels.Add(level.BranchName, dictionary);
                                foreach (Dictionary<int, CareerLevel> dictionary2 in CareerLevels.Values)
                                {
                                    int key = level.Level - 0x1;
                                    if (dictionary2.ContainsKey(key) && level.BranchSource.Equals(dictionary2[key].BranchName))
                                    {
                                        dictionary2[key].NextLevels.Add(level);
                                        level.LastLevel = dictionary2[key];
                                        break;
                                    }
                                }
                            }
                            if ((Level1 == null) && (level.Level == 0x1))
                            {
                                SharedData.Level1 = level;
                            }
                            if (level.Level > HighestLevel)
                            {
                                SharedData.HighestLevelOfThisCareer = level.Level;
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(level.BranchName + "," + level.BranchSource + "," + level.Level, e);
                        }
                    }

                    SharedData.Locations = new List<CareerLocation>();
                    SharedData.CareerEventList = new List<EventDaily>();
                    SharedData.CareerEventList.Add(EventRaise.Instance);
                    if (!CareerEventManager.sAllEvents.ContainsKey(CareerEventType.Raise))
                    {
                        CareerEventManager.sAllEvents.Add(CareerEventType.Raise, EventRaise.Instance);
                    }
                    SharedData.CareerEventList.Add(EventLoser.Instance);
                    if (!CareerEventManager.sAllEvents.ContainsKey(CareerEventType.Loser))
                    {
                        CareerEventManager.sAllEvents.Add(CareerEventType.Loser, EventLoser.Instance);
                    }
                    if (eventDataTable != null)
                    {
                        foreach (XmlDbRow row2 in eventDataTable.Rows)
                        {
                            ProductVersion version;
                            row2.TryGetEnum<ProductVersion>("ProductVersion", out version, ProductVersion.Undefined);
                            if (GameUtils.IsInstalled(version))
                            {
                                Type classType;
                                string str3 = row2.GetString("EventName");
                                string str4 = row2.GetString("OpportunityName");
                                bool flag = row2.GetInt("SureShotEvent") == 0x1;
                                if ((str4 != string.Empty) && (str3 == string.Empty))
                                {
                                    classType = typeof(EventOpportunity);
                                }
                                else
                                {
                                    classType = row2.GetClassType("EventName");
                                }
                                Type[] types = new Type[] { typeof(XmlDbRow), typeof(Dictionary<string, Dictionary<int, CareerLevel>>), typeof(string) };
                                object obj2 = classType.GetConstructor(types).Invoke(new object[] { row2, SharedData.CareerLevels, eventDataTable.Name });
                                if (flag)
                                {
                                    SharedData.SureShotEvent = (EventDaily) obj2;
                                }
                                else
                                {
                                    EventDaily item = (EventDaily) obj2;
                                    SharedData.CareerEventList.Add(item);
                                    try
                                    {
                                        CareerEventManager.sAllEvents.Add(item.EventType, item);
                                        continue;
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }

        public string CoworkerPool
        {
            get { return mOther.mCoworkerPool; }
        }

        public string LotDesignator
        {
            get { return mOther.mLotDesignator; }
        }

        protected NonPersistableData Data
        {
            get { return mOther; }
        }

        public int NumJournalsRead
        {
            get { return mNumJournalsRead; }
        }

        public int NumRecruits
        {
            get { return mNumRecruits; }
        }

        public int ShakedownFunds
        {
            get { return mShakedownFunds; }
        }

        public void AddShakedownFunds(int funds)
        {
            mShakedownFunds += funds;
        }

        protected void SetupSeveranceAlarm()
        {
            if ((OwnerDescription.Occupation == this) && (mStipendAlarm == AlarmHandle.kInvalidHandle))
            {
                mStipendAlarm = AlarmManager.Global.AddAlarmDay(12f, DaysOfTheWeek.All, StipendAlarmCallback, "Stipend Alarm", AlarmType.NeverPersisted, OwnerDescription);
            }
        }

        protected virtual int Stipend
        {
            get
            {
                return GetCurLevelData().mStipend;
            }
        }

        protected virtual void PayStipend(int stipend)
        {
            if (stipend != 0)
            {
                if (SimTypes.IsSelectable(OwnerDescription))
                {
                    string msg = OmniCareer.LocalizeString(OwnerDescription, "StipendNotice", "NRaas.Careers.Stipend:GenericNotice", new object[] { OwnerDescription, stipend });

                    Common.Notify(OwnerDescription.CreatedSim, msg);
                }

                PayOwnerSim(stipend, GotPaidEvent.PayType.kCareerRetirementPay);
            }
        }

        protected void StipendAlarmCallback()
        {
            try
            {
                PayStipend(Stipend);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override int CalculateBoostedStartingLevel()
        {
            Dictionary<int, CareerLevel> levels;
            if (!CareerLevels.TryGetValue("Base", out levels)) return -1;

            int maxBaseLevel = 0;

            foreach (int levelValue in levels.Keys)
            {
                if (maxBaseLevel < levelValue)
                {
                    maxBaseLevel = levelValue;
                }
            }

            int level = base.CalculateBoostedStartingLevel();

            return Math.Min(level, maxBaseLevel);
        }

        public static string Localize(Career career, string key, object[] parameters)
        {
            if (career.SharedData == null) return null;

            string localKey = career.SharedData.Name + ":" + key;

            if (!Localization.HasLocalizationString(localKey)) return null;

            if (career.OwnerDescription == null)
            {
                return Common.LocalizeEAString(false, localKey, parameters);
            }
            else
            {
                return Common.LocalizeEAString(career.OwnerDescription.IsFemale, localKey, parameters);
            }
        }

        public void AddToRecruits()
        {
            mNumRecruits++;
        }

        public bool CanUseDisguise()
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data == null) return false;

            return data.mDisguise;
        }

        public bool CanUseReaper()
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data == null) return false;

            return data.mReaper;
        }

        public void CreateJournal()
        {
            OmniJournalData data = OmniJournalData.GetJournalData(SharedData.Name, CareerLevel);

            if (data != null)
            {
                mJournal = OmniJournal.CreateOutOfWorld(data, base.OwnerDescription);

                AlarmTimerCallback callback = delegate
                {
                    if (mJournal != null)
                    {
                        mJournal.mUpdatedToday = false;
                    }
                };

                if (mJournal != null)
                {
                    mJournal.mJournalUpdate = mJournal.AddAlarmDay(0f, ~DaysOfTheWeek.None, callback, "NRaas Reset Journal Update", AlarmType.AlwaysPersisted);

                    SendJournalToInventory();
                }
            }
        }

        protected bool SendJournalToInventory()
        {
            if ((OwnerDescription.CreatedSim != null) && !OwnerDescription.CreatedSim.Inventory.TryToMove(mJournal))
            {
                DestroyJournal(false);
                return false;
            }
            else
            {
                mJournal.RemoveFromWorld();
                return true;
            }
        }

        protected bool MoveJournalToInventory()
        {
            if (OwnerDescription.CreatedSim == null) 
            {
                return false;
            }

            if (mJournal == null)
            {
                mJournal = OwnerDescription.CreatedSim.Inventory.Find<OmniJournal>();
            }
            
            if (mJournal != null)
            {
                if (!SendJournalToInventory())
                {
                    CreateJournal();
                }

                return true;
            }

            return false;
        }

        protected void DestroyJournal(bool showPoofFx)
        {
            if (mJournal != null)
            {
                mJournal.RemoveAlarm(mJournal.mJournalUpdate);
                mJournal.Destroy();
                mJournal = null;
            }
        }

        public void FinishedJournal()
        {
            mNumJournalsRead++;
        }

        public static string LocalizeString(SimDescription sim, string key, string defaultKey, object[] parameters)
        {
            Career career = sim.Occupation as Career;
            if (career != null)
            {
                string result = OmniCareer.Localize(career, key, parameters);
                if (result != null) return result;

                if (defaultKey == null)
                {
                    return key;
                }
            }

            return Common.LocalizeEAString(sim.IsFemale, defaultKey, parameters);
        }

        public override bool CanRetire()
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data != null)
            {
                return data.mCanRetire;
            }
            else
            {
                return true;
            }
        }

        public override Occupation Clone()
        {
            OmniCareer career = base.Clone() as OmniCareer;

            mValues = new Dictionary<Type, OmniValue>();

            return career;
        }

        public override bool ExportContent(IPropertyStreamWriter writer)
        {
            return true;
        }

        public override bool ImportContent(IPropertyStreamReader reader)
        {
            return true;
        }

        public static bool HasMetric<T>(Occupation occupation)
            where T : PerfMetric
        {
            Career career = occupation as Career;
            if (career == null) return false;

            if (career.CurLevel == null) return false;

            foreach (PerfMetric metric in career.CurLevel.Metrics)
            {
                if (metric is T)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasMetric<T>()
            where T : PerfMetric
        {
            return HasMetric<T>(this);
        }

        public static T Career<T>(Occupation career)
            where T : Career, new()
        {
            if (career is T)
            {
                return (career as T);
            }

            OmniCareer omniCareer = career as OmniCareer;
            if (omniCareer == null) return default(T);

            return omniCareer.GetCareer<T>();
        }

        public T GetCareer<T>()
            where T : Career, new()
        {
            return GetValue<CareerOmniValue<T>>().TCareer;
        }

        public T GetValue<T>()
            where T : OmniValue, new()
        {
            OmniValue value;
            if (!mValues.TryGetValue(typeof(T), out value))
            {
                value = new T();

                mValues.Add(typeof(T), value);

                value.Init(this, false);
            }

            value.Set(this);

            return value as T;
        }

        public override void AddCoworker(SimDescription coworker)
        {
            try
            {
                if (coworker == OwnerDescription) return;

                foreach (OmniValue value in mValues.Values)
                {
                    if (value.Career is LawEnforcement)
                    {
                        value.Set(this);

                        LawEnforcement police = value.Career as LawEnforcement;

                        police.Partner = coworker;

                        value.Reset(this);
                    }
                }

                base.AddCoworker(coworker);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public static bool AddCoworker(Career ths)
        {
            ths.RemoveInvalidCoworkers();

            if (ths.CareerLoc == null) return false;

            List<SimDescription> possibles = new List<SimDescription> (ths.CareerLoc.Workers);

            ICoworkerPool pool = ths as ICoworkerPool;
            if ((pool != null) && (!string.IsNullOrEmpty(pool.CoworkerPool)))
            {
                possibles = new List<SimDescription>();

                foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
                {
                    if (hole.CareerLocations == null) continue;

                    foreach (CareerLocation loc in hole.CareerLocations.Values)
                    {
                        ICoworkerPool career = loc.Career as ICoworkerPool;
                        if (career == null) continue;

                        if (career.CoworkerPool != pool.CoworkerPool) continue;

                        possibles.AddRange(loc.Workers);
                    }
                }
            }

            Dictionary<SimDescription, bool> candidateCoworkers = new Dictionary<SimDescription, bool>();

            foreach (SimDescription description in possibles)
            {
                if (description.Household == ths.OwnerDescription.Household) continue;

                if (!description.IsValidDescription) continue;

                if (description.CareerManager == null) continue;

                if (!ths.ShouldConsiderAsCoworker(description)) continue;

                if (candidateCoworkers.ContainsKey(description)) continue;
                candidateCoworkers.Add(description, true);
            }

            if (ths.Boss != null)
            {
                candidateCoworkers.Remove(ths.Boss);
            }

            foreach (SimDescription description2 in ths.Coworkers)
            {
                candidateCoworkers.Remove(description2);
            }

            if (candidateCoworkers.Count > 0)
            {
                ths.AddCoworker(RandomUtil.GetRandomObjectFromList(new List<SimDescription>(candidateCoworkers.Keys)));
                return true;
            }

            candidateCoworkers.Clear();
            foreach (CareerLocation location in ths.Locations)
            {
                if (location == ths.CareerLoc) continue;

                location.CheckWorkers();
                foreach (SimDescription desc in location.Workers)
                {
                    if ((!desc.CareerManager.IsBoss && !desc.CareerManager.IsCoworker) && (desc.Household != ths.OwnerDescription.Household))
                    {
                        if (candidateCoworkers.ContainsKey(desc)) continue;
                        candidateCoworkers.Add(desc, true);
                    }
                }
            }

            return ths.ChooseCoworkerFromSet(candidateCoworkers);
        }

        public override bool AddCoworker()
        {
            try
            {
                if (OmniCareer.AddCoworker(this)) return true;

                if (Common.AssemblyCheck.IsInstalled("NRaasStoryProgression")) return false;

                return base.AddCoworker();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
                return false;
            }
        }

        public override ResourceKey GetWorkOutfitForToday(CASAgeGenderFlags age, CASAgeGenderFlags gender)
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    ResourceKey key = value.Career.GetWorkOutfitForToday(age, gender);
                    if (key != ResourceKey.kInvalidResourceKey)
                    {
                        return key;
                    }
                }

                NonPersistableData.CareerLevelData data = GetCurLevelData();
                if (data == null)
                {
                    return base.GetWorkOutfitForToday(age, gender);
                }
                else
                {
                    return data.mOutfits.GetWorkOutfitForToday(age, gender, base.GetWorkOutfitForToday);
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return ResourceKey.kInvalidResourceKey;
        }

        public override void RegularWorkDayEndAlarmHandler()
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.RegularWorkDayEndAlarmHandler();

                    value.Reset(this);
                }

                base.RegularWorkDayEndAlarmHandler();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void SetCoworkersAndBoss()
        {
            try
            {
                if (CareerLoc == null) return;

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.SetCoworkersAndBoss();

                    value.Reset(this);
                }

                base.SetCoworkersAndBoss();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override bool IsWorkHour(DateAndTime queryTime)
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    if (value.Career.IsWorkHour(queryTime)) return true;
                }

                return base.IsWorkHour(queryTime);
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return false;
        }

        public override bool ShouldConsiderAsCoworker(SimDescription candidate)
        {
            try
            {
                if (OwnerDescription == candidate) return false;

                if (Data.mBranchOnlyCoworkers)
                {
                    string myBranch = CurLevel.BranchName;
                    if ((candidate.Occupation != null) && 
                        (candidate.Occupation.CurLevelBranchName != myBranch))
                    {
                        return false;
                    }
                }

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    if (value.Career.ShouldConsiderAsCoworker(candidate)) return true;
                }

                return base.ShouldConsiderAsCoworker(candidate);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return false;
        }

        public override bool CanInterview()
        {
            try
            {
                if (HasMetric<MetricReportsWritten> ()) return true;

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    if (value.Career.CanInterview()) return true;
                }

                return base.CanInterview();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return false;
       }

        public override bool CanWriteReport()
        {
            try
            {
                if (HasMetric<MetricReportsWritten> ()) return true;

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    try
                    {
                        if (value.Career.CanWriteReport()) return true;
                    }
                    catch
                    {
                        value.Career.OnStartup();
                    }
                }

                return base.CanWriteReport();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return false;
        }

        public override Car GetCareerCar()
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    Car car = value.Career.GetCareerCar();
                    if (car != null) return car;
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return null;
        }

        public override float GetCurrentReportCompletion()
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    float result = value.Career.GetCurrentReportCompletion();
                    if (result > 0) return result;
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return 0;
        }

        public override SimDescription GetCurrentReportSubject()
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    SimDescription sim = value.Career.GetCurrentReportSubject();
                    if (sim != null) return sim;
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return null;
        }

        public override void DemoteSim()
        {
            try
            {
                base.DemoteSim();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void GiveRaise(bool fromDemand)
        {
            try
            {
                Sim sim = OwnerDescription.CreatedSim;
                OwnerDescription.CreatedSim = null;

                float payPerHourExtra = mPayPerHourExtra;

                try
                {
                    foreach (OmniValue value in mValues.Values)
                    {
                        value.Set(this);

                        value.Career.GiveRaise(fromDemand);

                        value.Reset(this);
                    }
                }
                finally
                {
                    mPayPerHourExtra = payPerHourExtra;
                }

                OwnerDescription.CreatedSim = sim;

                if (base.CurLevel.NextLevels.Count == 0)
                {
                    mNumJournalsRead = 0;
                    mNumRecruits = 0;
                }

                base.GiveRaise(fromDemand);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override bool CanInterview(SimDescription interviewee)
        {
            try
            {
                if (HasMetric<MetricReportsWritten>())
                {
                    return true;
                }

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    if (value.Career.CanInterview(interviewee))
                    {
                        return true;
                    }
                }

                if (base.CanInterview(interviewee))
                {
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                Common.Exception(OwnerDescription, exception);
            }

            return false;
       }

        public override bool OnRejoiningQuitCareer()
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.OnRejoiningQuitCareer();

                    value.Reset(this);
                }

                return base.OnRejoiningQuitCareer();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
                return false;
            }
        }

        public override void SetReportSubject(SimDescription subject)
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.SetReportSubject(subject);

                    value.Reset(this);
                }

                base.SetReportSubject(subject);
            }
            catch (Exception exception)
            {
                Common.Exception(OwnerDescription, exception);
            }
        }

        public override void SimInterviewed(SimDescription interviewee)
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.SimInterviewed(interviewee);

                    value.Reset(this);
                }

                base.SimInterviewed(interviewee);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override bool UpdateReport()
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    bool success = value.Career.UpdateReport();

                    value.Reset(this);

                    if (success)
                    {
                        SetReportSubject(null);
                        return true;
                    }
                }

                return base.UpdateReport();
            }
            catch (Exception exception)
            {
                Common.Exception(OwnerDescription, exception);
            }

            return false;
        }

        public override List<SimDescription> ValidNegativeReportSubjects()
        {
            List<SimDescription> subjects = new List<SimDescription>();

            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    List<SimDescription> list = null;

                    try
                    {
                        list = value.Career.ValidNegativeReportSubjects();
                    }
                    catch
                    {
                        value.Career.OnStartup();
                    }

                    if (list != null)
                    {
                        subjects.AddRange(list);
                    }
                }
                
                List<SimDescription> list2 = base.ValidNegativeReportSubjects();
                if (list2 != null)
                {
                    subjects.AddRange(list2);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(OwnerDescription, exception);
            }

            return subjects;
        }

        public override List<SimDescription> ValidPositiveReportSubjects()
        {
            List<SimDescription> subjects = new List<SimDescription>();

            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    List<SimDescription> list = null;

                    try
                    {
                        list = value.Career.ValidPositiveReportSubjects();
                    }
                    catch
                    {
                        value.Career.OnStartup();
                    }

                    if (list != null)
                    {
                        subjects.AddRange(list);
                    }
                }

                List<SimDescription> list2 = base.ValidPositiveReportSubjects();
                if (list2 != null)
                {
                    subjects.AddRange(list2);
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return subjects;
        }

        private new void CreateRewardObject(CareerLevel newLevel, string rewardObjectName)
        {
            rewardObjectName = rewardObjectName.Replace("0x", "");

            ulong instance = 0;
            if (!ulong.TryParse(rewardObjectName, System.Globalization.NumberStyles.HexNumber, null, out instance))
            {
                return;
            }

            Simulator.ObjectInitParameters initParamsForRewardObj = GetInitParamsForRewardObj(newLevel, rewardObjectName);

            GameObject obj = ObjectCreation.CreateObject(instance, ProductVersion.BaseGame, initParamsForRewardObj) as GameObject;

            if ((OwnerDescription.Household != null) && (OwnerDescription.Household.SharedFamilyInventory != null))
            {
                OwnerDescription.Household.SharedFamilyInventory.Inventory.TryToAdd(obj, false);

                ShowOccupationTNS(LocalizeCareerString(OwnerDescription.IsFemale, "ObjectReward", new object[] { newLevel.GetLocalizedName(OwnerDescription), obj }), false);
            }
        }

        public override void GivePromotionRewardObjects(CareerLevel newLevel)
        {
            try
            {
                base.GivePromotionRewardObjects(newLevel);

                NonPersistableData.CareerLevelData data = mOther.GetLevelData(newLevel);
                if ((data != null) && (data.mPromotionRewardData != null))
                {
                    string promotionRewardData = data.mPromotionRewardData;
                    if (newLevel.PromotionRewardData.Contains(","))
                    {
                        string[] list = null;
                        ParserFunctions.ParseCommaSeparatedString(newLevel.PromotionRewardData, out list);
                        promotionRewardData = RandomUtil.GetRandomStringFromList(list);
                    }

                    TraitNames trait = TraitNames.Unknown;
                    if (ParserFunctions.TryParseEnum<TraitNames>(promotionRewardData, out trait, TraitNames.Unknown))
                    {
                        OwnerDescription.TraitManager.AddHiddenElement(trait);
                    }
                    else
                    {
                        CreateRewardObject(newLevel, promotionRewardData);
                    }
                }

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.GivePromotionRewardObjects(newLevel);

                    value.Reset(this);
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void RemoveCoworker(SimDescription coworker)
        {
            try
            {
                base.RemoveCoworker(coworker);

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void LeaveJobNow(Career.LeaveJobReason reason)
        {
            try
            {
                List<SimDescription> coworkers = new List<SimDescription>(Coworkers);
                foreach (SimDescription sim in coworkers)
                {
                    if (sim.Occupation == null) continue;

                    if ((sim.Occupation == this) || (sim.Occupation.Coworkers == Coworkers))
                    {
                        Coworkers.Remove(sim);
                    }
                }

                // Required to stop a script error in ScheduleOpportunityCall
                if (OwnerDescription.CelebrityManager == null)
                {
                    OwnerDescription.CelebrityManager = new CelebrityManager(OwnerDescription.SimDescriptionId);
                }

                base.LeaveJobNow(reason);

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.LeaveJobNow(reason);

                    value.Reset(this);
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void OnDispose()
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.OnDispose();
                }

                base.OnDispose();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        protected NonPersistableData.CareerLevelData GetCurLevelData()
        {
            return mOther.GetLevelData(CurLevel);
        }

        public bool CanShakedown()
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data == null) return false;

            return data.mCanShakedown;
        }

        public bool CanBreakIntoHouses()
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data == null) return false;

            return data.mCanBreakIntoHouses;
        }

        public bool CanSwipe()
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data == null) return false;

            return data.mCanSwipe;
        }

        public bool PaidForConcerts()
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data == null) return false;

            return data.mPayForConcerts;
        }

        public override bool CanGiveAutograph(AutographType autographType)
        {
            try
            {
                if (base.CanGiveAutograph(autographType)) return true;

                NonPersistableData.CareerLevelData data = GetCurLevelData();
                if (data != null)
                {
                    if (data.mAutograph) return true;
                }

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    bool success = value.Career.CanGiveAutograph(autographType);

                    value.Reset(this);

                    if (success) return true;
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return false;
        }

        public override bool PromoteIfShould()
        {
            try
            {
                if ((ShouldPromote || (Performance >= 100f)) && (CurLevel.NextLevels.Count > 0x0))
                {
                    ShouldPromote = false;
                    CareerLevel newLevel = null;
                    CareerLevel curLevel = CurLevel;

                    NonPersistableData.CareerLevelData careerData = GetCurLevelData();
                    if (careerData != null)
                    {
                        string branchRedirection = careerData.mBranchRedirection;

                        int levelRedirection = careerData.mLevelRedirection;

                        if (levelRedirection != 0)
                        {
                            if (string.IsNullOrEmpty(branchRedirection))
                            {
                                branchRedirection = CurLevel.BranchName;
                            }

                            Dictionary<int, CareerLevel> levels;
                            if (!CareerLevels.TryGetValue(branchRedirection, out levels))
                            {
                                Common.DebugNotify("Redirection Branch Missing");
                                return false;
                            }

                            if (!levels.TryGetValue(levelRedirection, out newLevel))
                            {
                                Common.DebugNotify("Redirection Level Missing");
                                return false;
                            }
                        }
                    }
                    
                    if (newLevel == null)
                    {
                        List<CareerLevel> nextLevels = CurLevel.NextLevels;
                        if (nextLevels.Count > 0x1)
                        {
                            CareerLevel level3 = nextLevels[0x0];
                            CareerLevel level4 = nextLevels[0x1];
                            bool flag = true;
                            if (ShouldShowNotification())
                            {
                                flag = Occupation.ShowCareerOptionDialog(Common.LocalizeEAString(OwnerDescription.IsFemale, SharedData.Text_BranchOffer, new object[] { OwnerDescription }), Common.LocalizeEAString(OwnerDescription.IsFemale, SharedData.Text_Branch1, new object[0x0]), Common.LocalizeEAString(OwnerDescription.IsFemale, SharedData.Text_Branch2, new object[0x0]));
                            }
                            else
                            {
                                flag = RandomUtil.CoinFlip();
                            }
                            newLevel = flag ? level3 : level4;
                        }
                        else if (nextLevels.Count == 0x1)
                        {
                            newLevel = nextLevels[0x0];
                        }
                    }

                    int bonusAmount = GivePromotionBonus();
                    GivePromotionRewardObjectsIfShould(newLevel);
                    SetLevel(newLevel);
                    OnPromoteDemote(curLevel, newLevel);
                    if (OwnerDescription.CreatedSim != null)
                    {
                        SetTones(OwnerDescription.CreatedSim.CurrentInteraction);
                    }
                    if (ShouldShowNotification())
                    {
                        string displayText = GeneratePromotionText(bonusAmount);
                        Audio.StartSound("sting_career_positive");
                        ShowOccupationTNS(displayText);
                    }
                    return true;
                }

                if (ShouldPromote && (CurLevel.NextLevels.Count == 0x0))
                {
                    ShouldPromote = false;
                }
                else if ((Performance >= 100f) && (CurLevel.NextLevels.Count == 0x0))
                {
                    GiveRaise(false);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                // Added for extra detail regarding a script error
                //   (Feb 2011)
                string errorMessage = null;
                errorMessage = "CurLevel = " + CurLevel;

                if (CurLevel != null)
                {
                    errorMessage += Common.NewLine + "NextLevels = " + CurLevel.NextLevels;
                }

                Common.Exception(errorMessage + Common.NewLine + OwnerDescription, e);
                return false;
            }
        }

        public override void OnPromoteDemote(CareerLevel oldLevel, CareerLevel newLevel)
        {
            try
            {
                base.OnPromoteDemote(oldLevel, newLevel);

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    try
                    {
                        value.Career.OnPromoteDemote(oldLevel, newLevel);
                    }
                    catch
                    {
                        value.Career.OnStartup();
                    }

                    value.Reset(this);
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override bool OnLoadFixup(bool isQuitCareer)
        {
            bool result = false;

            try
            {
                SetupSeveranceAlarm();

                Tones = new List<CareerTone>();

                if (Coworkers == null)
                {
                    Coworkers = new List<SimDescription>();
                }

                List<SimDescription> coworkers = new List<SimDescription>(Coworkers);

                foreach (SimDescription coworker in coworkers)
                {
                    if ((coworker.Household == OwnerDescription.Household) || (!ShouldConsiderAsCoworker(coworker)))
                    {
                        Coworkers.Remove(coworker);
                    }
                }

                if ((mJournal != null) && (mJournal.HasBeenDestroyed))
                {
                    mJournal = null;
                }

                OmniCareer staticCareer = CareerManager.GetStaticCareer(Guid) as OmniCareer;
                if (staticCareer != null)
                {
                    mOther = staticCareer.mOther;
                }

                result = base.OnLoadFixup(isQuitCareer);

                List<Type> remove = new List<Type>();

                if (CurLevel != null)
                {
                    foreach (KeyValuePair<Type, OmniValue> value in mValues)
                    {
                        if (value.Value.Career.OwnerDescription != OwnerDescription)
                        {
                            remove.Add(value.Key);
                        }
                        else
                        {
                            value.Value.Init(this, isQuitCareer);
                        }
                    }
                }

                // Cleanup for mess made by not having the Clone() operation set up properly
                //   (Mar 2011)
                foreach (Type value in remove)
                {
                    mValues.Remove(value);
                }

                return result;
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription + Common.NewLine + "Result: " + result, e);
                return false;
            }
        }

        public override void OnStartup()
        {
            try
            {
                base.OnStartup();

                foreach (OmniValue value in mValues.Values)
                {
                    value.Init(this, false);
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override bool ShouldBeAtWork()
        {
            SetupSeveranceAlarm();

            return base.ShouldBeAtWork();
        }
     
        private void GameDayHourBeforeStartAlarmHandler()
        {
            ProSports job = GetCareer<ProSports>();

            if (job.mPlayInNextGame && !base.IsDayOff)
            {
                Sim createdSim = OwnerDescription.CreatedSim;
                if ((createdSim != null) && ((!createdSim.IsSelectable || !mbCarpoolEnabled) || !CurLevel.HasCarpool))
                {
                    createdSim.ShowTNSIfSelectable(LocalizeString(OwnerDescription, "ProfessionalSports:GoingToGame", "Gameplay/Careers/ProfessionalSports:GoingToGame", new object[] { createdSim }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, createdSim.ObjectId);
                    createdSim.InteractionQueue.AddNext(PlayGame.Singleton.CreateInstance(createdSim.CareerLocation, createdSim, new InteractionPriority(InteractionPriorityLevel.High), true, true));
                }
            }
        }

        public override void SetLevel(CareerLevel level)
        {
            try
            {
                base.SetLevel(level);

                if (!level.HasMetricType(typeof(MetricJournals)))
                {
                    DestroyJournal(true);
                }

                mNumJournalsRead = 0;
                mNumRecruits = 0;

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    value.Career.SetLevel(level);

                    CareerOmniValue<ProSports> proSports = value as CareerOmniValue<ProSports>;
                    if (proSports != null)
                    {
                        DaysOfTheWeek days = DaysOfTheWeek.None;
                        foreach (DaysOfTheWeek week2 in ProSports.kDaysToPlayGame)
                        {
                            days |= week2;
                        }

                        AlarmManager.Global.RemoveAlarm(proSports.TCareer.mGameDayHourBeforeStartHandle);

                        proSports.TCareer.mGameDayHourBeforeStartHandle = AlarmManager.Global.AddAlarmDay(ProSports.kGameStartTime - 1f, days, new AlarmTimerCallback(GameDayHourBeforeStartAlarmHandler), "Career: hour before regular starting time", AlarmType.AlwaysPersisted, OwnerDescription);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void StartWorking()
        {
            try
            {
                if ((CareerLoc == null) || (CurLevel == null)) return;

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    try
                    {
                        value.Career.StartWorking();
                    }
                    catch
                    { }

                    value.Reset(this);
                }

                if ((mJournal == null) && CurLevel.HasMetricType(typeof(MetricJournals)))
                {
                    if (!MoveJournalToInventory())
                    {
                        CreateJournal();
                    }
                }
                else if ((mJournal != null) && !CurLevel.HasMetricType(typeof(MetricJournals)))
                {
                    DestroyJournal(true);
                }
                else if (mJournal != null)
                {
                    if ((OwnerDescription.CreatedSim != null) && (!OwnerDescription.CreatedSim.Inventory.Contains(mJournal)))
                    {
                        MoveJournalToInventory();
                    }

                    if (mJournal != null)
                    {
                        mJournal.UpdateJournal(this);
                    }
                }

                // Fix for script error in Career::get_CarpoolLotEndLocation
                bool carpoolEnabled = mbCarpoolEnabled;
                if ((CareerLoc == null) || (CareerLoc.Owner == null) || (CareerLoc.Owner.RabbitHoleProxy.LotCurrent == null))
                {
                    mbCarpoolEnabled = false;
                }

                try
                {
                    base.StartWorking();
                }
                finally
                {
                    mbCarpoolEnabled = carpoolEnabled;
                }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void FinishWorking()
        {
            try
            {
                bool rolledForStolenObject = RolledForStolenObject;
                RolledForStolenObject = true;

                Sim sim = OwnerDescription.CreatedSim;
                OwnerDescription.CreatedSim = null;

                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    // Stops the function from paying the sim
                    bool changed = false;
                    float original = 0;

                    try
                    {
                        if ((value.Career != null) && (value.Career.CurLevel != null))
                        {
                            original = value.Career.mPayPerHourExtra;
                            value.Career.mPayPerHourExtra = -value.Career.CurLevel.PayPerHourBase;
                            changed = true;
                        }

                        if (value.Career is ProSports)
                        {
                            sim.ResetShapeDelta();
                            //value.Career.FinishWorking();
                        }
                        else
                        {
                            value.Career.FinishWorking();
                        }
                    }
                    finally
                    {
                        if (changed)
                        {
                            value.Career.mPayPerHourExtra = original;
                        }
                    }

                    value.Reset(this);
                }

                OwnerDescription.CreatedSim = sim;

                RolledForStolenObject = rolledForStolenObject;

                try
                {
                    base.FinishWorking();
                }
                catch
                { }
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override bool CareerAgeTest(SimDescription sim)
        {
            foreach (KeyValuePair<SkillNames, int> value in mOther.mSkillPrerequisites)
            {
                if (sim.SkillManager.GetSkillLevel(value.Key) < value.Value)
                {
                    return false;
                }
            }

            if ((Data.mGenders & sim.Gender) != sim.Gender) return false;

            return ((Data.mAges & sim.Age) == sim.Age);
        }

        public override Simulator.ObjectInitParameters GetInitParamsForRewardObj(CareerLevel newLevel, string rewardObjectName)
        {
            try
            {
                foreach (OmniValue value in mValues.Values)
                {
                    value.Set(this);

                    Simulator.ObjectInitParameters param = value.Career.GetInitParamsForRewardObj(newLevel, rewardObjectName);
                    if (param != null) return param;
                }

                return base.GetInitParamsForRewardObj(newLevel, rewardObjectName);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }

            return null;
        }

        protected virtual OccupationNames GetTransferCareer()
        {
            return mOther.mFullTimeEquivalent;
        }

        protected static void OnJobTransfer(Sims3.Gameplay.EventSystem.Event e)
        {
            TransferCareerEvent transfer = e as TransferCareerEvent;
            if (transfer != null)
            {
                Career newJob = transfer.NewCareer as Career;

                OmniCareer oldJob = transfer.OldCareer as OmniCareer;
                if ((oldJob != null) && (newJob != null))
                {
                    OccupationNames transferCareer = oldJob.GetTransferCareer();

                    if ((transferCareer != OccupationNames.Undefined) &&
                        (transfer.NewCareer.mCareerGuid == transferCareer))
                    {
                        if (oldJob.CareerLevel > 1)
                        {
                            CareerLevel newLevel = newJob.Level1;
                            if (newLevel != null)
                            {
                                int iOldLevel = oldJob.CareerLevel;

                                // Compensate for Rehire droppage
                                if (kPercentLevelRehired != 0f)
                                {
                                    iOldLevel = (int)Math.Ceiling((float)(iOldLevel / kPercentLevelRehired));
                                }

                                Household house = null;
                                if (transfer.NewCareer.OwnerDescription != null)
                                {
                                    house = transfer.NewCareer.OwnerDescription.Household;
                                }

                                for (int i = 1; i < iOldLevel; i++)
                                {
                                    if (newLevel.NextLevels.Count > 0)
                                    {
                                        newLevel = newLevel.NextLevels[0];

                                        newJob.SetLevelBasic(newLevel);

                                        if ((house != null) && (i < oldJob.CareerLevel))
                                        {
                                            if (newJob.HighestCareerLevelAchieved == null)
                                            {
                                                newJob.mHighestLevelAchieved = newLevel;
                                            }

                                            newJob.GivePromotionBonus();
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            if ((newJob.OwnerDescription != null) &&
                                (newJob.OwnerDescription.CreatedSim != null) &&
                                (newJob.OwnerDescription.Household == Household.ActiveHousehold))
                            {
                                StyledNotification.Show(new StyledNotification.Format(Common.Localize("TransferCareer:Message", newJob.IsOwnerFemale, new object[] { newJob.OwnerDescription }), ObjectGuid.InvalidObjectGuid, newJob.OwnerDescription.CreatedSim.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive), newJob.CareerIconColored);
                            }
                        }

                        newJob.mPerformance = oldJob.mPerformance;
                        newJob.mPayPerHourExtra += oldJob.mPayPerHourExtra;
                    }
                }
            }
        }

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kReadBook, OnReadBook);
            new Common.DelayedEventListener(EventTypeId.kCareerTransferJob, OnJobTransfer);
        }

        public bool CanPrank()
        {
            return Data.mCanPrank;
        }

        protected static void OnReadBook(Sims3.Gameplay.EventSystem.Event e)
        {
            OmniJournal book = e.TargetObject as OmniJournal;
            if (book != null)
            {
                Sim sim = e.Actor as Sim;
                if (sim != null)
                {
                    OmniCareer career = sim.Occupation as OmniCareer;
                    if ((career != null) && (career.HasMetric<MetricJournals>()))
                    {
                        ReadBookData data;
                        if (sim.ReadBookDataList.TryGetValue(book.Data.ID, out data))
                        {
                            if (data.TimesRead == 1)
                            {
                                career.FinishedJournal();
                            }
                        }
                    }
                }
            }
        }

        public override void AddAnyUniformsForCurrentLevelToWardrobe()
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data == null)
            {
                base.AddAnyUniformsForCurrentLevelToWardrobe();
            }
            else
            {
                data.mOutfits.AddAnyUniformsForCurrentLevelToWardrobe(this, base.AddAnyUniformsForCurrentLevelToWardrobe);
            }
        }

        public override bool TryGetUniformForCurrentLevel(CASAgeGenderFlags age, CASAgeGenderFlags gender, out SimOutfit uniform)
        {
            NonPersistableData.CareerLevelData data = GetCurLevelData();
            if (data == null)
            {
                return base.TryGetUniformForCurrentLevel(age, gender, out uniform);
            }
            else
            {
                return data.mOutfits.TryGetUniformForCurrentLevel(age, gender, out uniform, base.TryGetUniformForCurrentLevel);
            }
        }

        [Persistable]
        public abstract class OmniValue
        {
            public OmniValue()
            { }

            public abstract Career Career
            {
                get;
            }

            public abstract void Init(OmniCareer career, bool isQuitCareer);
            public abstract void Set(OmniCareer career);
            public abstract void Reset(OmniCareer career);
        }

        [Persistable]
        public class CareerOmniValue<T> : OmniValue
            where T : Career, new()
        {
            T mCareer = null;

            public CareerOmniValue()
            {
                mCareer = new T();
            }

            public T TCareer
            {
                get { return mCareer; }
            }

            public override Career Career
            {
                get { return mCareer; }
            }

            protected void RemoveAlarms()
            {
                AlarmManager.Global.RemoveAlarm(mCareer.mHungerSolverHandle);
                AlarmManager.Global.RemoveAlarm(mCareer.mRegularWorkDayEndHandle);
                AlarmManager.Global.RemoveAlarm(mCareer.mRegularWorkDayFastWalkHandle);
                AlarmManager.Global.RemoveAlarm(mCareer.mRegularWorkDayGoToWorkHandle);
                AlarmManager.Global.RemoveAlarm(mCareer.mRegularWorkDayHourAfterStartHandle);
                AlarmManager.Global.RemoveAlarm(mCareer.mRegularWorkDayRunHandle);
                AlarmManager.Global.RemoveAlarm(mCareer.mRegularWorkDayStartHandle);
                AlarmManager.Global.RemoveAlarm(mCareer.mRegularWorkDayTwoHoursBeforeStartHandle);

                mCareer.mHungerSolverHandle = AlarmHandle.kInvalidHandle;
                mCareer.mRegularWorkDayEndHandle = AlarmHandle.kInvalidHandle;
                mCareer.mRegularWorkDayFastWalkHandle = AlarmHandle.kInvalidHandle;
                mCareer.mRegularWorkDayGoToWorkHandle = AlarmHandle.kInvalidHandle;
                mCareer.mRegularWorkDayHourAfterStartHandle = AlarmHandle.kInvalidHandle;
                mCareer.mRegularWorkDayRunHandle = AlarmHandle.kInvalidHandle;
                mCareer.mRegularWorkDayStartHandle = AlarmHandle.kInvalidHandle;
                mCareer.mRegularWorkDayTwoHoursBeforeStartHandle = AlarmHandle.kInvalidHandle;
            }

            public override void Init(OmniCareer career, bool isQuitCareer)
            {
                //mCareer.SharedData = career.SharedData;
                mCareer.Tones = new List<CareerTone>();
                mCareer.WorkaholicInteractionLocked = career.WorkaholicInteractionLocked;
                mCareer.mAgeWhenJobFirstStarted = career.mAgeWhenJobFirstStarted;
                mCareer.mCareerGuid = career.mCareerGuid;
                mCareer.mDateHired = career.mDateHired;

                Set(career);

                mCareer.OnLoadFixup(isQuitCareer);
                mCareer.OnStartup();

                RemoveAlarms();
            }

            public override void Set(OmniCareer career)
            {
                mCareer.Boss = career.Boss;
                mCareer.Coworkers = career.Coworkers;
                mCareer.FormerBoss = career.FormerBoss;
                mCareer.CareerEventManager = career.CareerEventManager;
                mCareer.mHighestLevelAchieved = career.mHighestLevelAchieved;
                mCareer.mHighestLevelAchievedBranchName = career.mHighestLevelAchievedBranchName;
                mCareer.mHighestLevelAchievedVal = career.mHighestLevelAchievedVal;
                mCareer.mHourSpecialWorkTimeEnds = career.mHourSpecialWorkTimeEnds;
                mCareer.mHoursToReachWork = career.mHoursToReachWork;
                mCareer.mHoursUntilWork = career.mHoursUntilWork;
                mCareer.mAmbitiousPerformance = 0;// career.mAmbitiousPerformance;
                mCareer.mCurLevel = career.mCurLevel;
                mCareer.mCurLevelBranchName = career.mCurLevelBranchName;
                mCareer.mCurLevelVal = career.mCurLevelVal;
                mCareer.LastTone = career.LastTone;
                mCareer.mbCarpoolEnabled = false;

                mCareer.mCareerLoc = career.mCareerLoc;
                mCareer.mCareerLocOwnerGuid = career.mCareerLocOwnerGuid;

                mCareer.LastMetricAverageCalculated = career.LastMetricAverageCalculated;
                mCareer.LastPerfChange = career.LastPerfChange;
                mCareer.LastTimeCalculated = career.LastTimeCalculated;
                mCareer.LastTimeDemandedRaise = career.LastTimeDemandedRaise;

                mCareer.mDemoteEndOfNextShift = career.mDemoteEndOfNextShift;

                mCareer.mDemotionImminent = career.mDemotionImminent;
                if (mCareer.mDemotionImminent)
                {
                    mCareer.mDemotionHandle = career.mDemotionHandle;
                }
                else
                {
                    mCareer.mDemotionHandle = AlarmHandle.kInvalidHandle;
                }

                mCareer.mIsAtWork = career.mIsAtWork;
                mCareer.mIsSpecialWorkTime = career.mIsSpecialWorkTime;
                mCareer.mLeaveJobOnExit = LeaveJobReason.kNone; //career.mLeaveJobOnExit;
                mCareer.mPayPerHourExtra = career.mPayPerHourExtra;
                mCareer.mDaysOff = career.mDaysOff;
                mCareer.mUnpaidDaysOff = career.mUnpaidDaysOff;
                mCareer.mPaySimForTimeOff = false;// career.mPaySimForTimeOff;
                mCareer.mPerformance = career.mPerformance;
                mCareer.mPerformanceAtBeginningOfData = career.mPerformanceAtBeginningOfData;

                mCareer.mPickUpCarpool = null;
                mCareer.mTakeHomeCarpool = null;

                RemoveAlarms();

                mCareer.mSkippedDayOfWork = career.mSkippedDayOfWork;
                mCareer.mTimeElapsedSincePerfUpdate = career.mTimeElapsedSincePerfUpdate;
                mCareer.mTimeStartedWork = career.mTimeStartedWork;
                mCareer.mWhenCurLevelStarted = career.mWhenCurLevelStarted;
                mCareer.OwnerDescription = career.OwnerDescription;
                mCareer.PerformanceBonusPerHour = career.PerformanceBonusPerHour;
                mCareer.RolledForStolenObject = career.RolledForStolenObject;

                mCareer.ShouldDemote = career.ShouldDemote;
                mCareer.ShouldPromote = career.ShouldPromote;

                mCareer.mTookDayOff = career.mTookDayOff;
                mCareer.mUnpaidTimeOffCooldownTimeStamp = career.mUnpaidTimeOffCooldownTimeStamp;
            }

            public override void Reset(OmniCareer career)
            {
                career.LastMetricAverageCalculated = mCareer.LastMetricAverageCalculated;
                career.LastPerfChange = mCareer.LastPerfChange;
                career.LastTimeCalculated = mCareer.LastTimeCalculated;
                career.LastTimeDemandedRaise = mCareer.LastTimeDemandedRaise;
                career.mDemoteEndOfNextShift = mCareer.mDemoteEndOfNextShift;

                if (mCareer.mDemotionImminent != career.mDemotionImminent)
                {
                    if (mCareer.mDemotionImminent)
                    {
                        career.mDemotionHandle = mCareer.mDemotionHandle;
                    }
                    else
                    {
                        AlarmManager.Global.RemoveAlarm(career.mDemotionHandle);
                        career.mDemotionHandle = AlarmHandle.kInvalidHandle;
                    }
                }

                career.mLeaveJobOnExit = mCareer.mLeaveJobOnExit;
                career.mPayPerHourExtra = mCareer.mPayPerHourExtra;
                career.mDaysOff = mCareer.mDaysOff;
                career.mUnpaidDaysOff = mCareer.mUnpaidDaysOff;
                //career.mPaySimForTimeOff = mCareer.mPaySimForTimeOff;
                career.mPerformance = mCareer.mPerformance;

                RemoveAlarms();

                career.PerformanceBonusPerHour = mCareer.PerformanceBonusPerHour;
                career.RolledForStolenObject = mCareer.RolledForStolenObject;
                career.ShouldDemote = mCareer.ShouldDemote;
                career.ShouldPromote = mCareer.ShouldPromote;

                career.mTookDayOff = mCareer.mTookDayOff;
                career.mUnpaidTimeOffCooldownTimeStamp = mCareer.mUnpaidTimeOffCooldownTimeStamp;
            }
        }

        public class OutfitData
        {
            public ResourceKey mChildMaleOutfit;
            public ResourceKey mChildFemaleOutfit;

            public ResourceKey mTeenMaleOutfit;
            public ResourceKey mTeenFemaleOutfit;

            public delegate void OnAddAnyUniformsForCurrentLevelToWardrobe();

            public delegate ResourceKey OnGetWorkOutfitForToday(CASAgeGenderFlags age, CASAgeGenderFlags gender);

            public delegate bool OnTryGetUniformForCurrentLevel(CASAgeGenderFlags age, CASAgeGenderFlags gender, out SimOutfit uniform);

            public OutfitData()
            { }
            public OutfitData(XmlDbRow row)
            {
                ProductVersion version;
                row.TryGetEnum<ProductVersion>("OutfitMaleChildVersion", out version, ProductVersion.BaseGame);

                string outfit = row.GetString("OutfitMaleChild");
                if (!string.IsNullOrEmpty(outfit))
                {
                    mChildMaleOutfit = GetResourceKey(outfit, version);
                }

                row.TryGetEnum<ProductVersion>("OutfitFemaleChildVersion", out version, ProductVersion.BaseGame);

                outfit = row.GetString("OutfitFemaleChild");
                if (!string.IsNullOrEmpty(outfit))
                {
                    mChildFemaleOutfit = GetResourceKey(outfit, version);
                }

                row.TryGetEnum<ProductVersion>("OutfitMaleTeenVersion", out version, ProductVersion.BaseGame);

                outfit = row.GetString("OutfitMaleTeen");
                if (!string.IsNullOrEmpty(outfit))
                {
                    mTeenMaleOutfit = GetResourceKey(outfit, version);
                }

                row.TryGetEnum<ProductVersion>("OutfitFemaleTeenVersion", out version, ProductVersion.BaseGame);

                outfit = row.GetString("OutfitFemaleTeen");
                if (!string.IsNullOrEmpty(outfit))
                {
                    mTeenFemaleOutfit = GetResourceKey(outfit, version);
                }
            }

            public void AddAnyUniformsForCurrentLevelToWardrobe(Career ths, OnAddAnyUniformsForCurrentLevelToWardrobe baseFunc)
            {
                baseFunc();

                Household household = ths.OwnerDescription.Household;
                if (household != null)
                {
                    household.AddOutfitToWardrobe(GetOutfit(CASAgeGenderFlags.Child, CASAgeGenderFlags.Male));
                    household.AddOutfitToWardrobe(GetOutfit(CASAgeGenderFlags.Child, CASAgeGenderFlags.Female));
                    household.AddOutfitToWardrobe(GetOutfit(CASAgeGenderFlags.Teen, CASAgeGenderFlags.Male));
                    household.AddOutfitToWardrobe(GetOutfit(CASAgeGenderFlags.Teen, CASAgeGenderFlags.Female));
                }
            }

            public ResourceKey GetWorkOutfitForToday(CASAgeGenderFlags age, CASAgeGenderFlags gender, OnGetWorkOutfitForToday baseFunc)
            {
                ResourceKey key = GetOutfit(age, gender);
                if (key != ResourceKey.kInvalidResourceKey)
                {
                    return key;
                }

                return baseFunc(age, gender);
            }

            public bool TryGetUniformForCurrentLevel(CASAgeGenderFlags age, CASAgeGenderFlags gender, out SimOutfit uniform, OnTryGetUniformForCurrentLevel baseFunc)
            {
                ResourceKey key = GetOutfit(age, gender);
                if (key != ResourceKey.kInvalidResourceKey)
                {
                    return OutfitUtils.TryGenerateSimOutfit(key, out uniform);
                }

                return baseFunc(age, gender, out uniform);
            }

            public ResourceKey GetOutfit(CASAgeGenderFlags age, CASAgeGenderFlags gender)
            {
                if (age == CASAgeGenderFlags.Child)
                {
                    if (gender == CASAgeGenderFlags.Female)
                    {
                        return mChildFemaleOutfit;
                    }
                    else
                    {
                        return mChildMaleOutfit;
                    }
                }
                else if (age == CASAgeGenderFlags.Teen)
                {
                    if (gender == CASAgeGenderFlags.Female)
                    {
                        return mTeenFemaleOutfit;
                    }
                    else
                    {
                        return mTeenMaleOutfit;
                    }
                }
                else
                {
                    return ResourceKey.kInvalidResourceKey;
                }
            }

            private static ResourceKey GetResourceKey(string outfit, ProductVersion version)
            {
                if (!string.IsNullOrEmpty(outfit))
                {
                    return ResourceKey.CreateOutfitKeyFromProductVersion(outfit, version);
                }
                return ResourceKey.kInvalidResourceKey;
            }
        }

        [Persistable(false)]
        protected class NonPersistableData
        {
            public OccupationNames mFullTimeEquivalent = OccupationNames.Undefined;

            public bool mBranchOnlyCoworkers = false;

            public CASAgeGenderFlags mGenders = CASAgeGenderFlags.GenderMask;

            public CASAgeGenderFlags mAges = CASAgeGenderFlags.YoungAdult|CASAgeGenderFlags.Adult|CASAgeGenderFlags.Elder;

            public Dictionary<CareerLevel, CareerLevelData> mLevels = new Dictionary<CareerLevel, CareerLevelData>();

            public string mCoworkerPool = null;

            public string mLotDesignator = null;

            public bool mCanPrank = false;

            public Dictionary<SkillNames, int> mSkillPrerequisites = new Dictionary<SkillNames, int>();

            public CareerLevelData GetLevelData(CareerLevel level)
            {
                CareerLevelData data;
                if (mLevels.TryGetValue(level, out data))
                {
                    return data;
                }
                else
                {
                    return null;
                }
            }

            public class CareerLevelData
            {
                public OutfitData mOutfits;

                public bool mAutograph;

                public bool mDisguise;

                public bool mReaper;

                public string mPromotionRewardData;

                public bool mCanRetire = true;

                public bool mCanShakedown = false;

                public bool mPayForConcerts = false;

                public bool mCanBreakIntoHouses = false;

                public bool mCanSwipe = false;

                public int mLevelRedirection = 0;
                public string mBranchRedirection = null;

                public int mStipend = 0;

                public CareerLevelData()
                { }
                public CareerLevelData(XmlDbRow levelRow)
                {
                    Parse(levelRow);
                }

                public void Parse(XmlDbRow levelRow)
                {
                    mOutfits = new OutfitData(levelRow);

                    mStipend = levelRow.GetInt("Stipend", 0);

                    if (levelRow.Exists("LevelRedirection"))
                    {
                        mLevelRedirection = levelRow.GetInt("LevelRedirection");
                    }

                    if (levelRow.Exists("BranchRedirection"))
                    {
                        mBranchRedirection = levelRow.GetString("BranchRedirection");
                    }

                    mCanBreakIntoHouses = levelRow.GetBool("CanBreakIntoHouses");

                    mCanSwipe = levelRow.GetBool("CanSwipe");

                    // Default is true
                    if (!string.IsNullOrEmpty(levelRow.GetString("CanRetire")))
                    {
                        mCanRetire = levelRow.GetBool("CanRetire");
                    }

                    mPayForConcerts = levelRow.GetBool("PayForConcerts");

                    mAutograph = levelRow.GetBool("AllowAutograph");

                    mDisguise = levelRow.GetBool("AllowDisguise");

                    mReaper = levelRow.GetBool("AllowReaper");

                    mCanShakedown = levelRow.GetBool("CanShakedown");

                    mPromotionRewardData = levelRow.GetString("OmniPromotionalRewardData");
                }
            }
        }
    }
}
