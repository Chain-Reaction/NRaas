using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerStory : Manager
    {
        public enum SummaryPerSim
        {
            Yes,
            No
        }

        public enum StoryLogging : int
        {
            None = 0x00,
            Summary = 0x01,
            Log = 0x02,
            Notice = 0x04,
            Full = Summary|Log|Notice,
        }

        public enum PerformFormat
        {
            Yes,
            No
        }

        private List<Story> mStories = new List<Story>();

        private List<string> mSummary = new List<string> ();

        private Dictionary<string, List<AvailableStory>> mValidStories = new Dictionary<string, List<AvailableStory>>();

        public ManagerStory(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Stories";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerStory>(this).Perform(initial);
        }

        public static Common.StringBuilder CreateNameString(List<SimDescription> sims, SimDescription exclude)
        {
            Common.StringBuilder names = new Common.StringBuilder ();
            foreach (SimDescription sim in sims)
            {
                if (sim == exclude) continue;

                names += Common.NewLine + sim.FullName;
            }

            return names;
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (fullUpdate)
            {
                if (mStories.Count > 0)
                {
                    do
                    {
                        Story story = mStories[0];
                        mStories.RemoveAt(0);

                        story.Delay -= 10;

                        if (story.Delay > 0)
                        {
                            mStories.Add(story);
                        }
                        else if (story.Show())
                        {
                            if (!DebuggingEnabled)
                            {
                                int chance = mStories.Count * 3;
                                if (chance > 100)
                                {
                                    chance = 100;
                                }

                                if (!RandomUtil.RandomChance(chance))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    while (mStories.Count > 0);

                    if (Common.kDebugging)
                    {
                        foreach (Story story2 in mStories)
                        {
                            IncStat("Total Stories", Common.DebugLevel.Stats);

                            IncStat(story2.Manager.UnlocalizedName);
                        }
                    }
                }

                if ((GetValue<ShowSummaryOption, int>() > 0) && (mSummary.Count >= GetValue<ShowSummaryOption, int>()))
                {
                    PrintSummary();
                }
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public void Clear()
        {
            mStories.Clear();
            mSummary.Clear();
        }

        public override void Shutdown()
        {
            Clear();

            base.Shutdown();
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            for (int index = mStories.Count - 1; index >= 0; index--)
            {
                if (mStories[index].UsesSim(sim))
                {
                    mStories.RemoveAt(index);
                }
            }
        }

        public Story PrintFormattedSummary(Manager manager, string storyKey, string summaryKey, SimDescription sim, List<SimDescription> sims, string[] extended, StoryLogging logging)
        {
            List<object> parameters = new List<object>();
            
            parameters.Add(sim);

            foreach (SimDescription other in sims)
            {
                if (sim == other) continue;

                parameters.Add(other);
            }

            string nameString = ManagerStory.CreateNameString(sims, sim).ToString();

            List<object> objs = AddGenderNouns(sim);
            objs.Add(nameString);

            parameters.Add(nameString);

            return PrintFormattedStory(manager, manager.Localize(storyKey + "Summary", false, objs.ToArray()), summaryKey, parameters.ToArray(), extended, logging);
        }

        public void AddSummary(StoryProgressionObject spObject, SimDescription sim, string msg, string[] extended, StoryLogging logging)
        {
            if (string.IsNullOrEmpty(msg)) return;

            string sType = null;

            bool notify = true;

            if (sim != null)
            {
                notify = MatchesAlertLevel(sim);
                if (notify)
                {
                    if (!GetValue<AllowSummaryOption, bool>(sim))
                    {
                        notify = false;
                    }
                }

                if (DebuggingEnabled)
                {
                    if (sim.LotHome != null)
                    {
                        sType = "(R) ";
                    }
                    else if (SimTypes.IsService(sim))
                    {
                        sType = "(S) ";
                    }
                    else if (SimTypes.IsTourist(sim))
                    {
                        sType = "(T) ";
                    }
                    else
                    {
                        sType = "(V) ";
                    }
                }
            }

            AddSummary(spObject, sType + FullName(sim), msg, extended, notify, logging);
        }
        protected void AddSummary(StoryProgressionObject spObject, string name, string msg, string[] extended, bool notify, StoryLogging logging)
        {
            if (string.IsNullOrEmpty(name)) return;

            Manager manager = null;
            if (spObject is SimPersonality)
            {
                manager = Personalities;
            }
            else
            {
                manager = spObject as Manager;
            }

            if (manager != null)
            {
                manager.IncStat("Summary: " + msg);
            }

            string translated = msg;
            if (spObject != null)
            {
                ulong key = ParserFunctions.ParseUlong(msg, 0);

                if (key != 0)
                {
                    translated = Localization.LocalizeString(key);
                }
                else
                {
                    translated = spObject.Localize(msg);
                }

                if (string.IsNullOrEmpty(translated))
                {
                    translated = "(D) " + spObject.UnlocalizedName + "." + msg;
                }
            }

            if ((notify) && ((logging & StoryLogging.Summary) == StoryLogging.Summary) && (GetValue<ShowSummaryOption, int>() > 0))
            {
                mSummary.Add(translated + ": " + name);
            }

            if (((logging & StoryLogging.Log) == StoryLogging.Log) && ((notify) || (GetValue<FullStoryLogOption, bool>())))
            {
                AddSummaryToLog(translated + ": " + name, extended);
            }
        }

        public void AddSummaryToLog(string localizedText, string[] extended)
        {
            Common.StringBuilder extendedText = new Common.StringBuilder();

            extendedText += EAText.GetNumberString(SimClock.ElapsedCalendarDays()) + " ";
            extendedText += SimClock.CurrentTime().ToString() + ": ";
            extendedText += localizedText;

            if (extended != null)
            {
                extendedText += " (";

                bool first = true;
                foreach (string text in extended)
                {
                    if (!first)
                    {
                        extendedText += " ";
                    }

                    extendedText += text;
                    first = false;
                }

                extendedText += ")";
            }

            AddValue<DumpStoryLogOption, string>(extendedText.ToString());
        }

        protected void PrintSummary()
        {
            Common.StringBuilder msg = new Common.StringBuilder(Localize("SummaryTitle"));

            int count = 0;
            while ((mSummary.Count > 0) && (count < GetValue<ShowSummaryOption, int>()))
            {
                msg += Common.NewLine + mSummary[0];
                mSummary.RemoveAt(0);

                count++;
            }

            Common.Notify(msg);
        }

        public List<object> AddGenderNouns(SimDescription sim)
        {
            List<object> results = new List<object>();
            results.Add(sim);
            AddGenderNouns(sim, results);
            return results;
        }
        public void AddGenderNouns (SimDescription sim, List<object> parameters)
        {
            string value = null;

            for (int i = 1; i <= 8; i++)
            {
                if (!Localize(sim.Species + "Pronoun" + i, sim.IsFemale, new object[0], out value))
                {
                    value = "";
                }

                parameters.Add(value);
            }

            if (!Localize(sim.Species.ToString() + sim.Age.ToString(), sim.IsFemale, new object[0], out value))
            {
                value = "";
            }

            parameters.Add(value);
        }

        public Story PrintStory(StoryProgressionObject manager, string key, Household house, string[] extended, StoryLogging logging)
        {
            List<object> parameters = new List<object>();

            SimDescription head = SimTypes.HeadOfFamily(house, false);
            if (head != null)
            {
                parameters.Add(head);

                if (house.LotHome != null)
                {
                    string name = house.LotHome.Name.Trim();
                    if (string.IsNullOrEmpty(name))
                    {
                        name = Localize("SomeHome");
                    }
                    parameters.Add(name);

                    name = house.LotHome.Address.Trim();
                    if (string.IsNullOrEmpty(name))
                    {
                        name = Localize("SomeAddress");
                    }
                    parameters.Add(name);
                }
                else
                {
                    parameters.Add(null);
                    parameters.Add(null);
                }

                foreach (SimDescription sim in house.AllSimDescriptions)
                {
                    if (sim == head) continue;

                    parameters.Add(sim);
                }
            }

            return PrintInternalStory(manager, key, key, parameters.ToArray(), extended, PerformFormat.Yes, false, SummaryPerSim.No, logging);
        }
        public Story PrintStory(StoryProgressionObject manager, string key, object[] parameters, string[] extended)
        {
            return PrintStory(manager, key, parameters, extended, false, StoryLogging.Full);
        }
        public Story PrintStory(StoryProgressionObject manager, string key, object[] parameters, string[] extended, bool forceNotify, StoryLogging logging)
        {
            return PrintInternalStory(manager, key, key, parameters, extended, PerformFormat.Yes, forceNotify, SummaryPerSim.Yes, logging);
        }

        public Story PrintFormattedStory(StoryProgressionObject manager, string localizedStory, string summaryKey, object[] parameters, string[] extended, StoryLogging logging)
        {
            return PrintInternalStory(manager, localizedStory, summaryKey, parameters, extended, PerformFormat.No, false, SummaryPerSim.Yes, logging);
        }

        public class AvailableStory
        {
            protected string mMale;
            protected string mFemale;
            protected string mMaleMale;
            protected string mMaleFemale;
            protected string mFemaleMale;
            protected string mFemaleFemale;

            public AvailableStory(ILocalizer spObject, string key, object[] parameters)
            {
                string primary = Check(spObject, key, parameters, null);

                mMale = Check(spObject, key + "_Male", parameters, primary);
                mFemale = Check(spObject, key + "_Female", parameters, primary);

                string uniMaleDefaultMale = Check(spObject, key + "_UniMale", parameters, mMale);
                string uniFemaleDefaultMale = Check(spObject, key + "_UniFemale", parameters, mMale);

                string uniMaleDefaultFemale = Check(spObject, key + "_UniMale", parameters, mFemale);
                string uniFemaleDefaultFemale = Check(spObject, key + "_UniFemale", parameters, mFemale);

                mMaleMale = Check(spObject, key + "_MaleMale", parameters, uniMaleDefaultMale);
                mMaleFemale = Check(spObject, key + "_MaleFemale", parameters, uniFemaleDefaultMale);

                mFemaleMale = Check(spObject, key + "_FemaleMale", parameters, uniMaleDefaultFemale);
                mFemaleFemale = Check(spObject, key + "_FemaleFemale", parameters, uniFemaleDefaultFemale);
            }

            protected string Check(ILocalizer spObject, string key, object[] parameters, string primary)
            {
                string story;
                if ((spObject.Localize(key, false, parameters, out story)) && (!story.StartsWith("NRaas.")))
                {
                    return key;
                }
                else
                {
                    return primary;
                }
            }

            public bool IsValid
            {
                get
                {
                    return (mMale != null);
                }
            }

            public string Localize(ILocalizer spObject, CASAgeGenderFlags first, CASAgeGenderFlags second, object[] parameters)
            {
                return spObject.Localize(GetKey(first, second), false, parameters);
            }

            protected string GetKey(CASAgeGenderFlags first, CASAgeGenderFlags second)
            {
                switch(first)
                {
                    case CASAgeGenderFlags.Male:
                        switch(second)
                        {
                            case CASAgeGenderFlags.Male:
                                return mMaleMale;
                            case CASAgeGenderFlags.Female:
                                return mMaleFemale;
                            default:
                                return mMale;
                        }
                    case CASAgeGenderFlags.Female:
                        switch(second)
                        {
                            case CASAgeGenderFlags.Male:
                                return mFemaleMale;
                            case CASAgeGenderFlags.Female:
                                return mFemaleFemale;
                            default:
                                return mFemale;
                        }
                    default:
                        return mMale;
                }
            }
        }

        protected Story PrintInternalStory(StoryProgressionObject manager, string storyKey, string summaryKey, object[] oldParameters, string[] extended, PerformFormat format, bool notify, SummaryPerSim summaryPerSim, StoryLogging logging)
        {
            bool matchesAlert = notify, foundSim = false, foundLot = false;

            Story.Element element1 = null;
            Story.Element element2 = null;

            CASAgeGenderFlags firstGender = CASAgeGenderFlags.None;
            CASAgeGenderFlags secondGender = CASAgeGenderFlags.None;

            List<object> newParameters = new List<object>();

            if (!matchesAlert)
            {
                if (oldParameters.Length == 0)
                {
                    matchesAlert = true;
                }
                else if (manager != null)
                {
                    foreach (object obj in oldParameters)
                    {
                        SimDescription sim = null;
                        if (obj is Sim)
                        {
                            sim = (obj as Sim).SimDescription;
                        }
                        else
                        {
                            sim = obj as SimDescription;
                        }

                        if (sim != null)
                        {
                            if (manager.MatchesAlertLevel(sim))
                            {
                                matchesAlert = true;
                                break;
                            }
                        }
                        else
                        {
                            Lot lot = obj as Lot;
                            if ((lot != null) && (lot.Household != null))
                            {
                                if (manager.MatchesAlertLevel(lot.Household.AllSimDescriptions))
                                {
                                    matchesAlert = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            foreach (object obj in oldParameters)
            {
                SimDescription sim = null;
                if (obj is Sim)
                {
                    sim = (obj as Sim).SimDescription;
                }
                else
                {
                    sim = obj as SimDescription;
                }

                if (sim != null)
                {
                    newParameters.Add(obj);

                    if ((summaryPerSim == SummaryPerSim.Yes) || (!foundSim))
                    {
                        AddSummary(manager, sim, summaryKey, extended, logging);
                    }

                    AddGenderNouns(sim, newParameters);

                    foundSim = true;

                    if (element1 == null)
                    {
                        firstGender = sim.Gender;

                        element1 = new Story.Element(sim);
                    }
                    else if (element2 == null)
                    {
                        secondGender = sim.Gender;

                        element2 = new Story.Element(sim);
                    }
                }
                else
                {
                    Lot lot = obj as Lot;
                    if (lot != null)
                    {
                        string name = lot.Name.Trim();
                        if (string.IsNullOrEmpty(name))
                        {
                            name = Localize("SomeHome");
                        }
                        newParameters.Add(name);

                        name = lot.Address.Trim();
                        if (string.IsNullOrEmpty(name))
                        {
                            name = Localize("SomeAddress");
                        }
                        newParameters.Add(name);

                        foundLot = true;

                        if (!string.IsNullOrEmpty(lot.Name))
                        {
                            AddSummary(manager, lot.Name, summaryKey, null, matchesAlert, logging);
                        }

                        AddSummary(manager, lot.Address, summaryKey, extended, matchesAlert, logging);

                        if (element1 == null)
                        {
                            element1 = new Story.Element(lot.ObjectId);
                        }
                        else if (element2 == null)
                        {
                            element2 = new Story.Element(lot.ObjectId);
                        }
                    }
                    else
                    {
                        newParameters.Add(obj);

                        string str = obj as string;
                        if (str != null)
                        {
                            if ((!foundSim) && (foundLot))
                            {
                                AddSummary(manager, str, summaryKey, extended, matchesAlert, logging);
                            }
                        }
                    }
                }
            }

            if (((logging & StoryLogging.Notice) == StoryLogging.Notice) && ((!foundSim) || (matchesAlert)))
            {
                string text = storyKey;
                if (format == PerformFormat.Yes)
                {
                    object[] parameters = newParameters.ToArray();

                    string uniqueKey = manager.UnlocalizedName + storyKey;

                    List<AvailableStory> stories;
                    if (!mValidStories.TryGetValue(uniqueKey, out stories))
                    {
                        stories = new List<AvailableStory>();

                        mValidStories.Add(uniqueKey, stories);

                        for (int i = 0; i <= 9; i++)
                        {
                            AvailableStory story = new AvailableStory(manager, storyKey + i.ToString(), parameters);
                            if (!story.IsValid) continue;

                            stories.Add(story);
                        }
                    }

                    if (stories.Count > 0)
                    {
                        AvailableStory set = RandomUtil.GetRandomObjectFromList(stories);

                        text = set.Localize(manager, firstGender, secondGender, parameters);

                        IncStat("Story: " + uniqueKey);
                    }
                    else
                    {
                        text = manager.Localize(storyKey + "0", false, parameters);

                        IncStat("Missing: " + text, Common.DebugLevel.High);
                    }
                }

                if (!string.IsNullOrEmpty(text))
                {
                    return NewStory(new Story(manager, text, element1, element2));
                }
            }

            return new Story(manager, null, null);
        }

        public static string FullName (SimDescription sim)
        {
            if (sim == null) return null;

            return sim.FullName;
        }

        public class Story
        {
            public class Element
            {
                private ObjectGuid mGuid = ObjectGuid.InvalidObjectGuid;
                private ulong mSimID = 0;

                public Element()
                { }
                public Element(ObjectGuid guid)
                {
                    mGuid = guid;
                }
                public Element(SimDescription sim)
                {
                    if (sim != null)
                    {
                        mSimID = sim.SimDescriptionId;
                    }
                }

                public bool IsSim()
                {
                    return (mSimID != 0);
                }

                public bool IsSim(ulong sim)
                {
                    return (mSimID == sim);
                }

                public ObjectGuid GetGuid()
                {
                    if (mSimID != 0)
                    {
                        SimDescription sim = ManagerSim.Find(mSimID);
                        if ((sim != null) && (sim.CreatedSim != null))
                        {
                            return sim.CreatedSim.ObjectId;
                        }
                    }

                    return mGuid;
                }
            }

            StoryProgressionObject mManager = null;

            string mText = null;

            public Element mID1 = new Element ();
            public Element mID2 = new Element ();
            public string mOverrideImage = null;
            public ProductVersion mOverrideVersion = ProductVersion.BaseGame;

            public int mDelay = 0;

            public bool mShowNoImage = false;

            public Story(StoryProgressionObject manager, string text, Element id1)
            {
                mText = text;
                mID1 = id1;
                mManager = manager;
                mShowNoImage = (mID1 == null);
            }

            public Story(StoryProgressionObject manager, string text, Element id1, Element id2)
            {
                mText = text;
                mID1 = id1;
                mID2 = id2;
                mManager = manager;
                mOverrideImage = null;
                mShowNoImage = ((mID1 == null) && (mID2 == null));
            }

            public StoryProgressionObject Manager
            {
                get { return mManager; }
            }

            public int Delay
            {
                get { return mDelay; }
                set { mDelay = value; }

            }

            public void SetDelay(int min, int max)
            {
                mDelay = RandomUtil.GetInt(min, max);
            }

            public void Invalidate()
            {
                mText = null;
            }

            public override string ToString()
            {
                return mText;
            }

            public bool UsesSim(ulong sim)
            {
                if (mManager is ManagerDeath) return false;

                if ((mID1 != null) && (mID1.IsSim (sim))) return true;
                if ((mID2 != null) && (mID2.IsSim (sim))) return true;

                return false;
            }

            public bool Show()
            {
                if ((mText == null) || (mText == ""))
                {
                    if (Manager != null)
                    {
                        Manager.Stories.AddSuccess("No Text " + Manager.UnlocalizedName);
                    }
                    return false;
                }

                int sims = 0;

                ObjectGuid id1 = ObjectGuid.InvalidObjectGuid;
                if (mID1 != null)
                {
                    id1 = mID1.GetGuid();

                    if (mID1.IsSim())
                    {
                        sims++;
                    }
                }

                ObjectGuid id2 = ObjectGuid.InvalidObjectGuid;
                if (mID2 != null)
                {
                    id2 = mID2.GetGuid();

                    if (mID2.IsSim())
                    {
                        sims++;
                    }
                }

                StyledNotification.NotificationStyle style = StyledNotification.NotificationStyle.kGameMessagePositive;
                if (sims == 2)
                {
                    style = StyledNotification.NotificationStyle.kSimTalking;
                }

                if ((!mShowNoImage) && (id1 == ObjectGuid.InvalidObjectGuid) && (id2 == ObjectGuid.InvalidObjectGuid))
                {
                    if (Manager != null)
                    {
                        Manager.Stories.AddSuccess("No Image " + Manager.UnlocalizedName);
                    }
                    return false;
                }
                else if ((id1 != ObjectGuid.InvalidObjectGuid) && (id2 != ObjectGuid.InvalidObjectGuid))
                {
                    mOverrideImage = null;
                }

                if (id2 == ObjectGuid.InvalidObjectGuid)
                {
                    id2 = id1;
                    id1 = ObjectGuid.InvalidObjectGuid;
                }

                if (sims == 0)
                {
                    style = StyledNotification.NotificationStyle.kSystemMessage;
                }

                Common.Notify(mText, id1, id2, style, mOverrideImage, mOverrideVersion);
                return true;
            }
        }

        public Story NewStory(Story story)
        {
            mStories.Add(story);

            return story;
        }

        public void Purge(StoryProgressionObject manager)
        {
            if (manager is SimPersonality)
            {
                manager = Personalities;
            }

            int index = 0;
            while (index < mStories.Count)
            {
                Story story = mStories[index];

                if (story.Manager == manager)
                {
                    mStories.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public class ShowWholeHouseholdOption : BooleanManagerOptionItem<ManagerStory>
        {
            public ShowWholeHouseholdOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowWholeHousehold";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class ShowSummaryOption : IntegerManagerOptionItem<ManagerStory>
        {
            public ShowSummaryOption()
                : base(20)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowStorySummary";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class FullStoryLogOption : BooleanManagerOptionItem<ManagerStory>
        {
            public FullStoryLogOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "FullStoryLog";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class ClearStoryLogOption : BooleanManagerOptionItem<ManagerStory>, INotPersistableOption
        {
            public ClearStoryLogOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ClearStoryLog";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override string GetUIValue(bool pure)
            {
                return "";
            }

            protected override bool PrivatePerform()
            {
                if (!AcceptCancelDialog.Show(Localize("Prompt", new object[] { Manager.GetValue<DumpStoryLogOption,List<string>> ().Count })))
                {
                    return false;
                }

                Manager.GetValue<DumpStoryLogOption, List<string>>().Clear();

                return base.PrivatePerform();
            }
        }

        public class MaxStoryLogOption : IntegerManagerOptionItem<ManagerStory>
        {
            public MaxStoryLogOption()
                : base(10000)
            { }

            public override string GetTitlePrefix()
            {
                return "MaxStoryLog";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class DumpStoryLogOption : GenericManagerOptionItem<ManagerStory, List<string>>, IGenericAddOption<string>
        {
            public DumpStoryLogOption()
                : base(new List<string>(), new List<string>())
            { }

            public override string GetTitlePrefix()
            {
                return "DumpStoryLog";
            }

            public override string GetUIValue(bool pure)
            {
                return EAText.GetNumberString(Value.Count);
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public string AddValue(string value)
            {
                if ((Manager.GetValue<MaxStoryLogOption, int>() > 0) && (Value.Count > Manager.GetValue<MaxStoryLogOption, int>()))
                {
                    Value.RemoveAt(0);
                }

                Value.Add(value);
                Persist();

                return value;
            }

            protected override bool PrivatePerform()
            {
                StringBuilder builder = new StringBuilder();

                foreach (string story in Value)
                {
                    builder.Append(story + Common.NewLine);
                }

                Common.WriteLog(builder.ToString());

                SimpleMessageDialog.Show(Name, Localize("Success"));
                return true;
            }

            public override string GetStoreKey()
            {
                // Legacy compatibility
                return GetTitlePrefix();
            }

            public void Clear()
            {
                Value.Clear();

                Persist();
            }
        }

        protected class DebugOption : DebugLevelOption<ManagerStory>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class SpeedOption : SpeedBaseOption<ManagerStory>
        {
            public SpeedOption()
                : base(10, false)
            { }
        }

        protected class DumpStatsOption : DumpStatsBaseOption<ManagerStory>
        {
            public DumpStatsOption()
                : base(20)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerStory>
        {
            public DumpScoringOption()
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerStory>
        {
            public TicksPassedOption()
            { }
        }

        public class MaxDumpLengthOption : IntegerManagerOptionItem<ManagerStory>, IDebuggingOption
        {
            public MaxDumpLengthOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "MaxDumpLength";
            }
        }

        public class DisallowByStoryOption : MultiListedManagerOptionItem<ManagerStory, string>
        {
            public DisallowByStoryOption()
                : base (new List<string>())
            { }

            public override string GetTitlePrefix()
            {
                return "DisallowByStory";
            }

            public override string ValuePrefix
            {
                get { return "Disallowed"; }
            }

            protected override bool PersistCreate(ref string defValue, string value)
            {
                defValue = value;
                return true;
            }

            protected override List<IGenericValueOption<string>> GetAllOptions()
            {
                List<IScenarioOptionItem> options = new List<IScenarioOptionItem>();
                Manager.Main.GetOptions(options, false, null);

                List<IGenericValueOption<string>> results = new List<IGenericValueOption<string>>();

                Dictionary<string, bool> lookup = new Dictionary<string, bool>();

                List<string> prefixes = Manager.Main.GetStoryPrefixes();

                foreach (IScenarioOptionItem option in options)
                {
                    Scenario scenario = option.GetScenario();
                    if (scenario == null) continue;

                    List<string> localPrefixes = prefixes;

                    bool checkLocalization = true;

                    Manager manager = option.GetManager() as Manager;
                    if (manager != null)
                    {
                        localPrefixes = new List<string>();
                        manager.GetStoryPrefixes(localPrefixes);

                        checkLocalization = false;
                    }

                    AddStories(scenario, localPrefixes, checkLocalization, results, lookup);
                }

                List<Scenario> scenarios = Common.DerivativeSearch.Find<Scenario>();

                foreach (Scenario scenario in scenarios)
                {
                    AddStories(scenario, prefixes, true, results, lookup);
                }

                foreach (IFormattedStoryScenario formattedStory in Common.DerivativeSearch.Find<IFormattedStoryScenario>())
                {
                    // Handled by previous loop
                    if (formattedStory is Scenario) continue;

                    AddStories(formattedStory, results, lookup);
                }

                return results;
            }

            protected void AddStories(IFormattedStoryScenario formattedStory, List<IGenericValueOption<string>> results, Dictionary<string, bool> lookup)
            {
                string key = formattedStory.GetFormattedStoryManager().UnlocalizedName + ":" + formattedStory.UnlocalizedName;

                if (lookup.ContainsKey(key)) return;
                lookup.Add(key, true);

                results.Add(new ListItem(this, key));
            }
            protected void AddStories(Scenario scenario, List<string> prefixes, bool checkLocalization, List<IGenericValueOption<string>> results, Dictionary<string, bool> lookup)
            {
                IFormattedStoryScenario formattedScenario = scenario as IFormattedStoryScenario;
                if (formattedScenario != null)
                {
                    AddStories(formattedScenario, results, lookup);
                }
                else
                {
                    List<string> stories = scenario.GetStoryPrefixes();

                    foreach (string prefix in prefixes)
                    {
                        foreach (string story in stories)
                        {
                            if (string.IsNullOrEmpty(story)) return;

                            string key = prefix + ":" + story;

                            if (lookup.ContainsKey(key)) continue;
                            lookup.Add(key, true);

                            string result;
                            if ((!checkLocalization) || (Common.Localize(key, false, new object[0], out result)))
                            {
                                results.Add(new ListItem(this, key));
                            }
                        }
                    }
                }
            }

            public class ListItem : BaseListItem<DisallowByStoryOption>
            {
                public ListItem(DisallowByStoryOption option, string story)
                    : base (option, story)
                { }

                public override string Name
                {
                    get
                    {
                        string[] names = Value.Split(':');

                        string value = null;
                        if (!Common.Localize(Value, false, new object[0], out value))
                        {
                            value = names[1];
                        }

                        string managerName = Common.Localize(names[0] + ":MenuName");

                        value = value.Replace(managerName, "").Trim();

                        return managerName + ": " + value;
                    }
                }
            }
        }

        public class ShowNonPushableStoryOption : BooleanManagerOptionItem<ManagerStory>
        {
            public ShowNonPushableStoryOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowNonPushStory";
            }
        }

        public class SummaryUpdates : AlertLevelOption<ManagerStory>
        {
            public SummaryUpdates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "SummaryStories";
            }
        }
    }
}

