using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.ScoringMethods;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public class SimScenarioFilter : IUpdateManagerOption
    {
        enum StandardFilter
        {
            None,
            Partner,
            AnyFlirt,
            ExistingFlirt,
            ExistingOrAnyFlirt,
            ExistingFriend,
            ExistingEnemy,
            Partnered,
            Me,
            Nemesis
        }

        enum ThirdPartyFilter
        {
            None,
            Partner,
            Parents,
            Siblings,
            Children,
            Romantic,
            Friend,
            Enemy
        }

        public enum RelationshipLevel : int
        {
            Maximum = 100,
            Minimum = -100,
            Friend = 40,
            Disliked = -20,
            Enemy = -20,
            Neutral = 0
        }

        StoryProgressionObject mManager;

        bool mEnabled = false;

        string mScoring;
        int mScoringMinimum;
        int mScoringMaximum;

        int mRelationshipMinimum;
        int mRelationshipMaximum;

        bool mDisallowRelated = false;

        CASAgeGenderFlags mAgeGender = CASAgeGenderFlags.None;
        List<CASAgeGenderFlags> mSpecies = new List<CASAgeGenderFlags>();

        List<string> mUserAgeGenders = new List<string>();
        List<ValueTestLoadStore> mValueTestLoads = new List<ValueTestLoadStore>();

        List<AgeGenderOption> mAgeGenderOptions = null;
        List<IValueTestOption> mValueTestOptions = null;

        string mClan;

        bool mClanLeader;
        bool mClanMembers;

        StandardFilter mStandardFilter = StandardFilter.None;
        bool mAllowAffair = true;

        int mStandardGate = 0;
        bool mStandardIgnoreBusy = false;
        bool mStandardDisallowPartner = false;
        bool mAllowOpposing = true;

        ThirdPartyFilter mThirdPartyFilter = ThirdPartyFilter.None;

        Common.MethodStore mCustomTest = null;

        public SimScenarioFilter()
        { }

        public override string ToString()
        {
            string text = "-- SimScenarioFilter --";

            text += Common.NewLine + "Enabled=" + mEnabled;
            text += Common.NewLine + "Scoring=" + mScoring;
            text += Common.NewLine + "ScoringMinimum=" + mScoringMinimum;
            text += Common.NewLine + "ScoringMaximum=" + mScoringMaximum;
            text += Common.NewLine + "RelationshipMinimum=" + mRelationshipMinimum;
            text += Common.NewLine + "RelationshipMaximum=" + mRelationshipMaximum;
            text += Common.NewLine + "DisallowRelated=" + mDisallowRelated;
            text += Common.NewLine + "AllowOpposing=" + mAllowOpposing;
            text += Common.NewLine + "AgeGender=" + mAgeGender;

            foreach (CASAgeGenderFlags species in mSpecies)
            {
                text += "," + species;
            }

            if (mAgeGenderOptions != null)
            {
                foreach (AgeGenderOption ageGender in mAgeGenderOptions)
                {
                    text += Common.NewLine + "UserAgeGender=" + ageGender;
                }
            }

            if (mValueTestOptions != null)
            {
                foreach (IValueTestOption testValue in mValueTestOptions)
                {
                    text += Common.NewLine + "ValueTest=" + testValue;
                }
            }

            text += Common.NewLine + "Clan=" + mClan;
            text += Common.NewLine + "ClanLeader=" + mClanLeader;
            text += Common.NewLine + "ClanMembers=" + mClanMembers;

            text += Common.NewLine + "AllowAffair=" + mAllowAffair;
            text += Common.NewLine + "StandardFilter=" + mStandardFilter;
            text += Common.NewLine + "StandardGate=" + mStandardGate;
            text += Common.NewLine + "StandardIgnoreBusy=" + mStandardIgnoreBusy;
            text += Common.NewLine + "StandardDisallowPartner=" + mStandardDisallowPartner;

            text += Common.NewLine + "ThirdPartyFilter=" + mThirdPartyFilter;

            text += Common.NewLine + "CustomTest=" + mCustomTest;

            text += Common.NewLine + "-- End SimScenarioFilter --";

            return text;
        }

        public bool AllowAffair
        {
            get { return mAllowAffair; }
        }

        public int ScoringMinimum
        {
            get { return mScoringMinimum; }
        }

        public void SetDisallowPartner(bool value)
        {
            mStandardDisallowPartner = value;
        }

        public bool Enabled
        {
            get { return mEnabled; }
        }

        public string Clan
        {
            get { return mClan; }
        }

        public void UpdateManager(StoryProgressionObject manager)
        {
            mManager = manager;

            mAgeGenderOptions = new List<AgeGenderOption>();

            foreach (string name in mUserAgeGenders)
            {
                AgeGenderOption option = manager.GetOption<AgeGenderOption>(name);
                if (option == null) continue;

                mAgeGenderOptions.Add(option);
            }

            StoryProgressionObject valueTestManager = manager;

            if (!string.IsNullOrEmpty(mClan))
            {
                valueTestManager = manager.Personalities.GetPersonality(mClan);
            }

            List<IValueTestOption> values = new List<IValueTestOption>();

            if (valueTestManager != null)
            {
                foreach(ValueTestLoadStore store in mValueTestLoads)
                {
                    IntegerOption intOption = manager.GetOption<IntegerOption>(store.mName);
                    if (intOption != null)
                    {
                        values.Add(new IntegerOption.ValueTest(intOption, store.mMinimum, store.mMaximum));
                    }
                    else
                    {
                        BooleanOption boolOption = manager.GetOption<BooleanOption>(store.mName);
                        if (boolOption != null)
                        {
                            values.Add(new BooleanOption.ValueTest(boolOption, store.mMatch));
                        }
                    }
                }
            }

            mValueTestOptions = values;
        }

        protected void Collect(SimDescription sim)
        {
            if (string.IsNullOrEmpty(mScoring)) return;

            SimListedScoringMethod scoring = ScoringLookup.GetScoring(mScoring) as SimListedScoringMethod;
            if (scoring == null) return;

            scoring.Collect(sim);
        }

        public ICollection<SimDescription> Filter(Parameters parameters, string name, SimDescription sim)
        {
            return Filter(parameters, name, sim, null);
        }
        public ICollection<SimDescription> Filter(Parameters parameters, string name, SimDescription sim, ICollection<SimDescription> potentials)
        {
            if (!mEnabled)
            {
                parameters.IncStat(name + " Disabled");

                if (parameters.mDefaultAll)
                {
                    return parameters.mManager.Sims.All;
                }
                else
                {
                    return potentials;
                }
            }

            Collect(sim);

            if ((sim != null) && (potentials == null))
            {
                switch (mStandardFilter)
                {
                    case StandardFilter.Me:
                        potentials = new List<SimDescription>();
                        potentials.Add(sim);
                        break;
                    case StandardFilter.Partner:
                        potentials = new List<SimDescription>();
                        if (sim.Partner != null)
                        {
                            potentials.Add(sim.Partner);
                        }
                        break;
                    case StandardFilter.AnyFlirt:
                        potentials = parameters.mManager.Flirts.FindAnyFor(parameters, sim, mAllowAffair, false);
                        break;
                    case StandardFilter.ExistingFriend:
                        potentials = parameters.mManager.Friends.FindExistingFriendFor(parameters, sim, mStandardGate, mStandardIgnoreBusy);
                        break;
                    case StandardFilter.Partnered:
                        potentials = parameters.mManager.Romances.FindPartneredFor(sim);
                        break;
                    case StandardFilter.ExistingFlirt:
                        potentials = parameters.mManager.Flirts.FindExistingFor(parameters, sim, mStandardDisallowPartner);
                        break;
                    case StandardFilter.ExistingOrAnyFlirt:
                        potentials = parameters.mManager.Flirts.FindExistingFor(parameters, sim, mStandardDisallowPartner);
                        if ((potentials == null) || (potentials.Count == 0))
                        {
                            potentials = parameters.mManager.Flirts.FindAnyFor(parameters, sim, mAllowAffair, false);
                        }
                        break;
                    case StandardFilter.ExistingEnemy:
                        potentials = parameters.mManager.Friends.FindExistingEnemyFor(parameters, sim, mStandardGate, mStandardIgnoreBusy);
                        break;
                    case StandardFilter.Nemesis:
                        potentials = parameters.mManager.Friends.FindNemesisFor(parameters, sim, mStandardIgnoreBusy);
                        break;
                }

                if (potentials != null)
                {
                    parameters.AddStat(name + " " + mStandardFilter.ToString(), potentials.Count);
                }
            }

            SimPersonality clan = parameters.mManager as SimPersonality;

            if (!string.IsNullOrEmpty(mClan))
            {
                clan = parameters.mManager.Personalities.GetPersonality(mClan);
                if (clan == null)
                {
                    parameters.IncStat(mClan + " Missing");
                    return new List<SimDescription>();
                }
            }

            if (clan != null)
            {
                if (potentials == null)
                {
                    if (mClanMembers)
                    {
                        potentials = clan.GetClanMembers(mClanLeader);

                        parameters.mIsFriendly = true;
                    }
                    else if (mClanLeader)
                    {
                        potentials = clan.MeAsList;

                        parameters.mIsFriendly = true;
                    }

                    if (potentials != null)
                    {
                        parameters.AddStat(name + " Clan", potentials.Count);
                    }
                }
                else if ((mClanLeader) || (mClanMembers))
                {
                    List<SimDescription> clanPotentials = new List<SimDescription>();

                    foreach (SimDescription potential in potentials)
                    {
                        if (clan.Me == potential)
                        {
                            if (!mClanLeader)
                            {
                                parameters.IncStat(name + " Leader Denied");
                                continue;
                            }
                        }
                        else if (clan.IsMember(potential))
                        {
                            if (!mClanMembers)
                            {
                                parameters.IncStat(name + " Member Denied");
                                continue;
                            }
                        }
                        else
                        {
                            parameters.IncStat(name + " Non-clan Denied");
                            continue;
                        }

                        clanPotentials.Add(potential);
                    }

                    potentials = clanPotentials;
                }
            }

            if (potentials == null)
            {
                potentials = parameters.mManager.Sims.All;

                parameters.AddStat(name + " All", potentials.Count);
            }

            parameters.AddStat(name + " Potentials", potentials.Count);

            ScoringList<SimDescription> list = new ScoringList<SimDescription>();

            foreach (SimDescription potential in potentials)
            {
                int score = 0;
                if (!Test(parameters, name, sim, potential, true, out score)) continue;

                list.Add(potential, score);

                parameters.mManager.Main.Sleep("SimScenarioFilter:Filter");
            }

            List<SimDescription> results = list.GetBestByPercent(100);

            parameters.AddStat(name + " Results", results.Count);

            if (mThirdPartyFilter != ThirdPartyFilter.None)
            {
                Dictionary<SimDescription, bool> lookup = new Dictionary<SimDescription, bool>();

                foreach (SimDescription result in results)
                {
                    switch (mThirdPartyFilter)
                    {
                        case ThirdPartyFilter.Romantic:
                            foreach (Relationship relation in Relationship.Get(result))
                            {
                                if (!relation.AreRomantic()) continue;

                                SimDescription other = relation.GetOtherSimDescription(result);
                                if (other == null) continue;

                                lookup[other] = true;
                            }
                            break;
                        case ThirdPartyFilter.Friend:
                            foreach (Relationship relation in Relationship.Get(result))
                            {
                                if (!relation.AreFriends()) continue;

                                SimDescription other = relation.GetOtherSimDescription(result);
                                if (other == null) continue;

                                lookup[other] = true;
                            }
                            break;
                        case ThirdPartyFilter.Enemy:
                            foreach (Relationship relation in Relationship.Get(result))
                            {
                                if (!relation.AreEnemies()) continue;

                                SimDescription other = relation.GetOtherSimDescription(result);
                                if (other == null) continue;

                                lookup[other] = true;
                            }
                            break;
                        case ThirdPartyFilter.Partner:
                            if (result.Partner == null) continue;

                            lookup[result.Partner] = true;
                            break;
                        case ThirdPartyFilter.Parents:
                            foreach (SimDescription parent in Relationships.GetParents(result))
                            {
                                lookup[parent] = true;
                            }
                            break;
                        case ThirdPartyFilter.Siblings:
                            foreach (SimDescription sibling in Relationships.GetSiblings(result))
                            {
                                lookup[sibling] = true;
                            }
                            break;
                        case ThirdPartyFilter.Children:
                            foreach (SimDescription child in Relationships.GetChildren(result))
                            {
                                lookup[child] = true;
                            }
                            break;
                    }

                    parameters.mManager.Main.Sleep("SimScenarioFilter:Filter");
                }

                results.Clear();
                results.AddRange(lookup.Keys);

                parameters.AddStat(name + " " + mThirdPartyFilter, results.Count);
            }

            return results;
        }

        public bool Test(Parameters parameters, string name, SimDescription sim, SimDescription potential)
        {
            int score = 0;
            return Test(parameters, name, sim, potential, true, out score);
        }
        public bool Test(Parameters parameters, string name, SimDescription sim, SimDescription potential, bool testScore, out int score)
        {
            score = 0;

            if (SimTypes.IsDead(potential))
            {
                parameters.IncStat(name + " Dead Fail");
                return false;
            }

            if (!parameters.CustomTestAllow (potential))
            {
                parameters.IncStat(name + " Custom Fail");
                return false;
            }

            if ((mCustomTest.Valid) && (!mCustomTest.Invoke<bool>(new object[] { parameters, sim, potential })))
            {
                parameters.IncStat(name + " Custom Test Fail");
                return false;
            }

            parameters.IncStat("Testing " + potential.FullName, Common.DebugLevel.Logging);

            CASAgeGenderFlags age = (mAgeGender & CASAgeGenderFlags.AgeMask);

            if ((age == CASAgeGenderFlags.None) && (potential.ToddlerOrBelow))
            {
                parameters.IncStat(name + " Too Young");
                return false;
            }
            else if ((age != CASAgeGenderFlags.None) && ((mAgeGender & potential.Age) != potential.Age))
            {
                parameters.IncStat(name + " Age Fail");
                return false;
            }
            else if (((mAgeGender & CASAgeGenderFlags.GenderMask) != CASAgeGenderFlags.None) && ((mAgeGender & potential.Gender) != potential.Gender))
            {
                parameters.IncStat(name + " Gender Fail");
                return false;
            }
            else if (!mSpecies.Contains(potential.Species))
            {
                parameters.IncStat(name + " Species Fail");
                return false;
            }

            if ((!mAllowAffair) && (parameters.mIsRomantic) && (sim != null))
            {
                if (((potential.Partner != null) || (sim.Partner != null)) && (potential.Partner != sim))
                {
                    parameters.IncStat(name + " Affair Denied");
                    return false;
                }
            }

            if ((sim != null) && (sim != potential))
            {
                if ((mStandardDisallowPartner) && (sim.Partner == potential))
                {
                    parameters.IncStat(name + " Partner Fail");
                    return false;
                }

                if ((!mAllowOpposing) && (StoryProgression.Main.Personalities.IsOpposing(parameters, sim, potential, false)))
                {
                    parameters.IncStat(name + " Clan Opposing Fail");
                    return false;
                }

                if (!parameters.mIsFriendly)
                {
                    if (StoryProgression.Main.Personalities.IsFriendly(parameters, sim, potential))
                    {
                        parameters.IncStat(name + " Clan Friendly Denied");
                        return false;
                    }
                }

                int liking = 0;

                Relationship relation = Relationship.Get(sim, potential, false);
                if (relation != null)
                {
                    liking = (int)relation.LTR.Liking;
                }

                if (liking < mRelationshipMinimum)
                {
                    parameters.IncStat(name + " Relationship Underscore");
                    return false;
                }
                else if (liking > mRelationshipMaximum)
                {
                    parameters.IncStat(name + " Relationship Overscore");
                    return false;
                }
            }

            foreach (IValueTestOption value in mValueTestOptions)
            {
                if (!value.Satisfies())
                {
                    parameters.IncStat(name + " " + value.ToString() + " Value Fail");
                    return false;
                }
            }

            foreach (AgeGenderOption ageGender in mAgeGenderOptions)
            {
                if (!ageGender.Satisfies(potential.Age, potential.Gender, potential.Species))
                {
                    parameters.IncStat(name + " " + ageGender.Name + " User Fail");
                    return false;
                }
            }

            if (mDisallowRelated)
            {
                if (parameters.mManager.Flirts.IsCloselyRelated(sim, potential))
                {
                    parameters.IncStat(name + " Closely Related");
                    return false;
                }
            }

            if ((testScore) && (Score(potential, sim, parameters.mAbsoluteScoring, out score)))
            {
                int scoringMinimum = mScoringMinimum + parameters.mScoreDelta;
                int scoringMaximum = mScoringMaximum + parameters.mScoreDelta;

                if (score < scoringMinimum)
                {
                    parameters.AddScoring(name + " Under Scoring Fail", score);
                    return false;
                }
                else if (score > scoringMaximum)
                {
                    parameters.AddScoring(name + " Over Scoring Fail", score);
                    return false;
                }

                parameters.AddStat(name + " Score Success", score);
                return true;
            }
            else
            {
                parameters.IncStat(name + " No Scoring");
                return true;
            }
        }

        public bool Score(SimDescription potential, SimDescription other, bool absolute, out int score)
        {
            return Score(mScoring, potential, other, absolute, out score);
        }
        protected bool Score(string scoringName, SimDescription potential, SimDescription other, bool absolute, out int score)
        {
            if (string.IsNullOrEmpty(scoringName))
            {
                score = 0;
                return false;
            }
            else
            {
                IListedScoringMethod scoring = ScoringLookup.GetScoring(scoringName);
                if (scoring == null)
                {
                    score = 0;
                    return false;
                }

                score = scoring.IScore(new ManagerScoringParameters(mManager, potential, other, absolute));
                return true;
            }
        }

        public bool Parse(XmlDbRow row, StoryProgressionObject manager, IUpdateManager updater, string prefix, bool errorIfNone, ref string error)
        {
            mDisallowRelated = row.GetBool(prefix + "DisallowRelated");

            if (row.Exists(prefix + "StandardFilter"))
            {
                mEnabled = true;

                if (!ParserFunctions.TryParseEnum<StandardFilter>(row.GetString(prefix + "StandardFilter"), out mStandardFilter, StandardFilter.None))
                {
                    error = prefix + "StandardFilter invalid";
                    return false;
                }
            }

            string customTest = row.GetString(prefix + "CustomTest");

            mCustomTest = new Common.MethodStore(customTest, new Type[] { typeof(Parameters), typeof(SimDescription), typeof(SimDescription) });
            if ((mCustomTest == null) && (!string.IsNullOrEmpty(customTest)))
            {
                error = prefix + "CustomTest Invalid";
                return false;
            }

            switch (mStandardFilter)
            {
                case StandardFilter.ExistingFriend:
                case StandardFilter.ExistingEnemy:
                case StandardFilter.Nemesis:
                    if (!row.Exists(prefix + "StandardIgnoreBusy"))
                    {
                        error = prefix + "StandardIgnoreBusy missing";
                        return false;
                    }

                    mStandardIgnoreBusy = row.GetBool(prefix + "StandardIgnoreBusy");
                    break;
            }

            switch (mStandardFilter)
            {
                case StandardFilter.ExistingFriend:
                case StandardFilter.ExistingEnemy:
                    RelationshipLevel standardLevel;
                    if (ParserFunctions.TryParseEnum<RelationshipLevel>(row.GetString(prefix + "StandardGate"), out standardLevel, RelationshipLevel.Neutral))
                    {
                        mStandardGate = (int)standardLevel;
                    }
                    else
                    {
                        RelationshipLevel defGate = RelationshipLevel.Neutral;
                        if (mStandardFilter == StandardFilter.ExistingFriend)
                        {
                            defGate = RelationshipLevel.Friend;
                        }
                        else
                        {
                            defGate = RelationshipLevel.Enemy;
                        }
                        mStandardGate = row.GetInt(prefix + "StandardGate", (int)defGate);
                    }

                    break;
                case StandardFilter.ExistingFlirt:
                case StandardFilter.ExistingOrAnyFlirt:
                    if (!row.Exists(prefix + "StandardDisallowPartner"))
                    {
                        error = prefix + "StandardDisallowPartner missing";
                        return false;
                    }

                    break;
            }

            // The default for DisallowPartner can be altered by the calling system, don't overwrite it
            if (row.Exists(prefix + "StandardDisallowPartner"))
            {
                mStandardDisallowPartner = row.GetBool(prefix + "StandardDisallowPartner");
            }

            switch (mStandardFilter)
            {
                case StandardFilter.AnyFlirt:
                case StandardFilter.ExistingOrAnyFlirt:
                    if (!row.Exists(prefix + "AllowAffair"))
                    {
                        error = prefix + "AllowAffair missing";
                        return false;
                    }

                    break;
            }

            if (row.Exists(prefix + "AllowOpposing"))
            {
                mAllowOpposing = row.GetBool(prefix + "AllowOpposing");
            }
            else
            {
                mAllowOpposing = true;
            }

            if (row.Exists("AllowAffair"))
            {
                error = prefix + "AllowAffair misdefined";
                return false;
            }
            else if (row.Exists(prefix + "AllowAffair"))
            {
                mAllowAffair = row.GetBool(prefix + "AllowAffair");
            }
            else
            {
                mAllowAffair = false;
            }

            if (row.Exists(prefix + "ThirdPartyFilter"))
            {
                mEnabled = true;

                if (!ParserFunctions.TryParseEnum<ThirdPartyFilter>(row.GetString(prefix + "ThirdPartyFilter"), out mThirdPartyFilter, ThirdPartyFilter.None))
                {
                    error = prefix + "ThirdPartyFilter invalid";
                    return false;
                }
            }

            mScoring = row.GetString(prefix + "Scoring");

            if (!string.IsNullOrEmpty(mScoring))
            {
                mEnabled = true;

                if (ScoringLookup.GetScoring(mScoring) == null)
                {
                    error = mScoring + " missing";
                    return false;
                }

                if ((!row.Exists(prefix + "ScoringMinimum")) && (!row.Exists(prefix + "ScoringMaximum")))
                {
                    error = prefix + "ScoringMinimum missing";
                    return false;
                }
            }

            mScoringMaximum = row.GetInt(prefix + "ScoringMaximum", int.MaxValue);
            mScoringMinimum = row.GetInt(prefix + "ScoringMinimum", int.MinValue);

            if (mScoringMinimum > mScoringMaximum)
            {
                int scoring = mScoringMinimum;
                mScoringMinimum = mScoringMaximum;
                mScoringMaximum = scoring;
            }

            if ((row.Exists(prefix + "RelationshipMinimum")) || (row.Exists(prefix + "RelationshipMaximum")))
            {
                mEnabled = true;
            }

            RelationshipLevel relationLevel;
            if (ParserFunctions.TryParseEnum<RelationshipLevel>(row.GetString(prefix + "RelationshipMaximum"), out relationLevel, RelationshipLevel.Neutral))
            {
                mRelationshipMaximum = (int)relationLevel;
            }
            else
            {
                mRelationshipMaximum = row.GetInt(prefix + "RelationshipMaximum", 101);
            }

            if (ParserFunctions.TryParseEnum<RelationshipLevel>(row.GetString(prefix + "RelationshipMinimum"), out relationLevel, RelationshipLevel.Neutral))
            {
                mRelationshipMinimum = (int)relationLevel;
            }
            else
            {
                mRelationshipMinimum = row.GetInt(prefix + "RelationshipMinimum", -101);
            }

            if (mRelationshipMinimum > mRelationshipMaximum)
            {
                int scoring = mRelationshipMinimum;
                mRelationshipMinimum = mRelationshipMaximum;
                mRelationshipMaximum = scoring;
            }

            mClan = row.GetString(prefix + "Clan");
            if (!string.IsNullOrEmpty(mClan))
            {
                mEnabled = true;
            }

            if (row.Exists(prefix + "ClanLeader"))
            {
                mEnabled = true;
            }

            mClanLeader = row.GetBool(prefix + "ClanLeader");

            if (row.Exists(prefix + "ClanMembers"))
            {
                mEnabled = true;
            }

            mClanMembers = row.GetBool(prefix + "ClanMembers");

            string ageGender = row.GetString(prefix + "AgeGender");
            if (!string.IsNullOrEmpty(ageGender))
            {
                mEnabled = true;

                if (!ParserFunctions.TryParseEnum<CASAgeGenderFlags>(ageGender, out mAgeGender, CASAgeGenderFlags.None))
                {
                    error = "Unknown AgeGender " + ageGender;
                    return false;
                }
            }

            StringToSpeciesList converter = new StringToSpeciesList();
            mSpecies = converter.Convert(row.GetString(prefix + "Species"));
            if (mSpecies == null)
            {
                error = converter.mError;
                return false;
            }

            if (mSpecies.Count == 0)
            {
                mSpecies.Add(CASAgeGenderFlags.Human);
            }

            for (int i = 0; i < 10; i++)
            {
                string key = prefix + "UserAgeGender" + i;
                if (!row.Exists(key)) break;

                mEnabled = true;

                string name = row.GetString(key);

                AgeGenderOption option = manager.GetOption<AgeGenderOption>(name);
                if (option == null)
                {
                    error = prefix + "UserAgeGender" + i + " " + name + " missing";
                    return false;
                }

                mUserAgeGenders.Add(name);
            }

            for (int i = 0; i < 10; i++)
            {
                string key = prefix + "ValueTest" + i;
                if (!row.Exists(key)) break;

                mEnabled = true;

                string name = row.GetString(key);

                int min = row.GetInt(key + "Minimum", int.MinValue);
                int max = row.GetInt(key + "Maximum", int.MaxValue);

                bool match = true;
                if (row.Exists(key + "Match"))
                {
                    match = row.GetBool(key + "Match");
                }

                mValueTestLoads.Add(new ValueTestLoadStore(name, min, max, match));
            }

            if ((!mEnabled) && (errorIfNone))
            {
                error = prefix + " Filter missing";
                return false;
            }

            updater.AddUpdater(this);
            return true;
        }

        public class Parameters : IScoringGenerator
        {
            readonly Common.IStatGenerator mStats;

            public readonly ManagerProgressionBase mManager;

            public bool mIsFriendly;

            public readonly bool mIsRomantic;

            public readonly bool mDefaultAll;

            public readonly int mScoreDelta;

            public readonly bool mAbsoluteScoring;

            readonly AllowFunc mTestAllow;

            readonly string mName;

            public delegate bool AllowFunc(SimDescription sim);

            public Parameters(ManagerProgressionBase manager)
                : this(manager, false, 0, null)
            { }
            public Parameters(ManagerProgressionBase manager, bool absolute, AllowFunc testAllow)
                : this(manager, absolute, 0, testAllow)
            { }
            public Parameters(ManagerProgressionBase manager, bool absolute, int scoreDelta)
                : this(manager, absolute, scoreDelta, null)
            { }
            public Parameters(ManagerProgressionBase manager, bool absolute, int scoreDelta, AllowFunc testAllow)
            {
                mTestAllow = testAllow;

                mStats = manager.Scoring;

                mScoreDelta = scoreDelta;

                mManager = manager;

                mIsFriendly = true;

                mIsRomantic = false;

                mDefaultAll = false;

                mAbsoluteScoring = absolute;

                mName = manager.UnlocalizedName;
            }
            public Parameters(Scenario scenario, bool defaultAll)
            {
                mDefaultAll = defaultAll;

                mStats = scenario;

                mManager = scenario.Manager;

                mName = mManager.UnlocalizedName + " " + scenario.UnlocalizedName;

                IFriendlyScenario friendly = scenario as IFriendlyScenario;
                if (friendly != null)
                {
                    mIsFriendly = friendly.IsFriendly;
                }

                IRomanticScenario romantic = scenario as IRomanticScenario;
                if (romantic != null)
                {
                    mIsRomantic = romantic.IsRomantic;
                }

                mAbsoluteScoring = false;
            }

            public Common.DebugLevel DebuggingLevel
            {
                get
                {
                    return mStats.DebuggingLevel;
                }
            }

            public bool CustomTestAllow(SimDescription sim)
            {
                if (mTestAllow == null) return true;

                return mTestAllow(sim);
            }

            public int AddScoring(string scoring, SimDescription sim)
            {
                int score = ScoringLookup.GetScore(scoring, sim);

                if (!Common.kDebugging) return score; 

                return AddScoring(mName + " " + scoring, score);
            }
            public int AddScoring(string scoring, SimDescription sim, Common.DebugLevel minLevel)
            {
                int score = ScoringLookup.GetScore(scoring, sim);

                if (!Common.kDebugging) return score;

                return AddScoring(mName + " " + scoring, score, minLevel);
            }

            public int AddScoring(string scoring, SimDescription sim, SimDescription other)
            {
                int score = ScoringLookup.GetScore(scoring, sim, other);

                if (!Common.kDebugging) return score;

                return AddScoring(mName + " " + scoring, score);
            }
            public int AddScoring(string scoring, SimDescription sim, SimDescription other, Common.DebugLevel minLevel)
            {
                int score = ScoringLookup.GetScore(scoring, sim, other);

                if (!Common.kDebugging) return score;

                return AddScoring(mName + " " + scoring, score, minLevel);
            }

            public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim)
            {
                int score = ScoringLookup.GetScore(scoring, option, type, sim);

                if (!Common.kDebugging) return score;

                return AddScoring(mName + " " + scoring, score);
            }
            public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, Common.DebugLevel minLevel)
            {
                int score = ScoringLookup.GetScore(scoring, option, type, sim);

                if (!Common.kDebugging) return score;

                return AddScoring(mName + " " + scoring, score, minLevel);
            }

            public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, SimDescription other)
            {
                int score = ScoringLookup.GetScore(scoring, option, type, sim, other);

                if (!Common.kDebugging) return score;

                return AddScoring(mName + " " + scoring, score);
            }
            public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, SimDescription other, Common.DebugLevel minLevel)
            {
                int score = ScoringLookup.GetScore(scoring, option, type, sim, other);

                if (!Common.kDebugging) return score;

                return AddScoring(mName + " " + scoring, score, minLevel);
            }

            public int AddScoring(string stat, int score)
            {
                if (!Common.kDebugging) return score; 

                return mStats.AddScoring(mName + " " + stat, score);
            }
            public int AddScoring(string stat, int score, Common.DebugLevel minLevel)
            {
                if (!Common.kDebugging) return score; 

                return mStats.AddScoring(mName + " " + stat, score, minLevel);
            }

            public int AddStat(string stat, int val)
            {
                if (!Common.kDebugging) return val; 

                return mStats.AddStat(mName + " " + stat, val);
            }
            public int AddStat(string stat, int val, Common.DebugLevel minLevel)
            {
                if (!Common.kDebugging) return val;

                return mStats.AddStat(mName + " " + stat, val, minLevel);
            }

            public float AddStat(string stat, float val)
            {
                if (!Common.kDebugging) return val; 

                return mStats.AddStat(mName + " " + stat, val);
            }
            public float AddStat(string stat, float val, Common.DebugLevel minLevel)
            {                
                if (!Common.kDebugging) return val;

                return mStats.AddStat(mName + " " + stat, val, minLevel);
            }

            public void IncStat(string stat)
            {
                if (!Common.kDebugging) return;

                mStats.IncStat(mName + " " + stat);
            }
            public void IncStat(string stat, Common.DebugLevel minLevel)
            {
                if (!Common.kDebugging) return;

                mStats.IncStat(mName + " " + stat, minLevel);
            }
        }

        public class ValueTestLoadStore
        {
            public readonly string mName;

            public readonly int mMinimum;
            public readonly int mMaximum;

            public readonly bool mMatch;

            public ValueTestLoadStore(string name, int min, int max, bool match)
            {
                mName = name;
                mMinimum = min;
                mMaximum = max;
                mMatch = match;
            }
        }

        public interface IValueTestOption
        {
            bool Satisfies();
        }
    }
}
