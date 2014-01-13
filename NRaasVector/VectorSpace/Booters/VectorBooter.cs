using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.VectorSpace.Booters
{
    public class VectorBooter : BooterHelper.ByRowListingBooter
    {
        static Dictionary<string, Data> sVectors = new Dictionary<string, Data>();

        public VectorBooter(string reference)
            : base("Vectors", "VectorsFile", "File", reference, false)
        { }

        public static IEnumerable<Data> Vectors
        {
            get { return sVectors.Values; }
        }

        public static bool HasVectors
        {
            get { return (sVectors.Count > 0); }
        }

        public static List<Item> GetVectorItems(SimDescription testSubject)
        {
            List<Item> results = new List<Item>();

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                bool isFemale = false;
                if (testSubject != null)
                {
                    if (!vector.CanInfect(testSubject, testSubject)) continue;

                    isFemale = testSubject.IsFemale;
                }

                results.Add(new VectorBooter.Item(vector, isFemale));
            }

            return results;
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            string guid = row.GetString("GUID");
            if (string.IsNullOrEmpty(guid))
            {
                BooterLogger.AddError("Invalid GUID: " + guid);
                return;
            }
            else if (sVectors.ContainsKey(guid))
            {
                BooterLogger.AddError("Duplicate GUID: " + guid);
                return;
            }

            sVectors.Add(guid, new Data(guid, file, row));
        }

        public class VectorStageTable : BooterHelper.DataBootTable
        {
            List<Stage> mStages = new List<Stage>();

            public VectorStageTable(BooterHelper.DataBootFile file, string name)
                : base(file, name)
            {
                if (IsValid)
                {
                    Load(LoadStage);
                }
            }

            protected void LoadStage(BooterHelper.BootFile file, XmlDbRow row)
            {
                Type type = row.GetClassType("FullClassName");
                if (type == null)
                {
                    BooterLogger.AddError("Invalid FullClassName: " + row.GetString("FullClassName"));
                    return;
                }

                Stage stage = null;

                try
                {
                    stage = type.GetConstructor(new Type[] { typeof(XmlDbRow) }).Invoke(new object[] { row }) as Stage;
                }
                catch (Exception e)
                {
                    BooterLogger.AddError("Contructor Fail: " + row.GetString("FullClassName"));

                    Common.Exception(file + Common.NewLine + row.GetString("FullClassName") + " Fail", e);
                }

                if (stage != null)
                {
                    mStages.Add(stage);
                }
            }

            public List<Stage> Stages
            {
                get { return mStages; }
            }
        }

        public static Data GetVector(string guid)
        {
            Data vector;
            if (!sVectors.TryGetValue(guid, out vector)) return null;

            return vector;
        }

        public class SymptomChance
        {
            List<SymptomBooter.Data> mSymptoms = new List<SymptomBooter.Data>();

            float mChance;

            public SymptomChance(XmlDbRow row, int index)
            {
                mChance = row.GetFloat("SymptomChance" + index);

                List<string> symptoms = row.GetStringList("Symptoms" + index, ',');

                foreach (string symptomName in symptoms)
                {
                    SymptomBooter.Data symptom = SymptomBooter.GetSymptom(symptomName);
                    if (symptom == null)
                    {
                        BooterLogger.AddError("Unknown Symptom: " + symptomName);
                    }
                    else
                    {
                        mSymptoms.Add(symptom);
                    }
                }
            }

            public void Perform(Sim sim, DiseaseVector vector)
            {
                if (!RandomUtil.RandomChance01(mChance)) return;

                foreach (SymptomBooter.Data symptom in mSymptoms)
                {
                    symptom.Perform(sim, vector);

                    ScoringLog.sLog.IncStat(symptom.Guid + " Symptom");
                }
            }

            public void GetSymptoms(Dictionary<string, SymptomBooter.Data> symptoms)
            {
                foreach (SymptomBooter.Data symptom in mSymptoms)
                {
                    if (symptoms.ContainsKey(symptom.Guid)) continue;

                    symptoms.Add(symptom.Guid, symptom);
                }
            }

            public override string ToString()
            {
                string result = "  Chance: " + mChance;

                foreach (SymptomBooter.Data symptom in mSymptoms)
                {
                    result += Common.NewLine + "   " + symptom.Guid;
                }

                return result;
            }
        }

        public class Mutable
        {
            public readonly string mCounter;

            public readonly int mMinimum;
            public readonly int mMaximum;

            public Mutable(string counter, int min, int max)
            {
                mCounter = counter;
                mMinimum = min;
                mMaximum = max;
            }

            public override string ToString()
            {
                return mCounter + ": " + mMinimum + " to " + mMaximum;
            }
        }

        public abstract class Stage
        {
            string mName;

            VectorControl.StageType mType;

            float mOutdoorInfectionRate;
            float mRoomInfectionRate;
            float mSocialInfectionRate;
            float mWoohooInfectionRate;
            float mFightInfectionRate;

            float mMutationRate;

            bool mCanInoculate = true;

            int mBadRelationshipDelta;
            int mGoodRelationshipDelta;

            string mSymptomScoring;
            int mSymptomScoringMinimum;

            string mStoryUnknown;
            string mStoryIdentified;

            List<SymptomChance> mSymptoms = new List<SymptomChance>();

            List<ResistanceBooter.Data> mResistances = new List<ResistanceBooter.Data>();

            List<Mutable> mMutables = new List<Mutable>();

            public Stage(XmlDbRow row)
            {
                if (BooterLogger.Exists(row, "Name", "Stage"))
                {
                    mName = row.GetString("Name");
                }

                if (!row.TryGetEnum<VectorControl.StageType>("Type", out mType, VectorControl.StageType.None))
                {
                    BooterLogger.AddError(Name + " Invalid Type: " + row.GetString("Type"));
                }

                float infectionRate = row.GetFloat("InfectionRate", 0);

                mOutdoorInfectionRate = row.GetFloat("OutdoorInfectionRate", infectionRate);
                if (mOutdoorInfectionRate > 1)
                {
                    mOutdoorInfectionRate = 1;
                }

                mRoomInfectionRate = row.GetFloat("RoomInfectionRate", mOutdoorInfectionRate * 1.5f);
                if (mRoomInfectionRate > 1)
                {
                    mRoomInfectionRate = 1;
                }

                mSocialInfectionRate = row.GetFloat("SocialInfectionRate", mRoomInfectionRate * 1.5f);
                if (mSocialInfectionRate > 1)
                {
                    mSocialInfectionRate = 1;
                }
                mFightInfectionRate = row.GetFloat("FightInfectionRate", mSocialInfectionRate);
                if (mFightInfectionRate > 1)
                {
                    mFightInfectionRate = 1;
                }

                mWoohooInfectionRate = row.GetFloat("WoohooInfectionRate", mSocialInfectionRate * 1.5f);
                if (mWoohooInfectionRate > 1)
                {
                    mWoohooInfectionRate = 1;
                }

                mMutationRate = row.GetFloat("MutationRate", 0);

                mGoodRelationshipDelta = row.GetInt("GoodRelationshipDelta", 0);
                mBadRelationshipDelta = row.GetInt("BadRelationshipDelta", 0);

                mSymptomScoring = row.GetString("SymptomScoring");
                mSymptomScoringMinimum = row.GetInt("SymptomScoringMinimum", 0);

                mStoryUnknown = row.GetString("StoryUnknown");
                mStoryIdentified = row.GetString("StoryIdentified");

                if (row.Exists("CanInoculate")) // Default is True
                {
                    mCanInoculate = row.GetBool("CanInoculate");
                }
                else if (row.Exists("CanInnoculate"))
                {
                    mCanInoculate = row.GetBool("CanInnoculate");
                }

                int numSymptoms = row.GetInt("NumSymptoms", 0) + 1;

                for (int i = 1; i <= numSymptoms; i++)
                {
                    if (i == numSymptoms)
                    {
                        if (row.Exists("SymptomChance" + i))
                        {
                            BooterLogger.AddError(Name + " More Symptoms than NumSymptoms");
                        }
                        break;
                    }

                    mSymptoms.Add(new SymptomChance(row, i));
                }

                int numResistances = row.GetInt("NumResistances", 0) + 1;

                for (int i = 1; i <= numResistances; i++)
                {
                    if (i == numResistances)
                    {
                        if (row.Exists("Resistance" + i))
                        {
                            BooterLogger.AddError(Name + " More Resistance than NumResistances");
                        }
                        break;
                    }

                    ResistanceBooter.Data resistance = ResistanceBooter.GetResistance(row.GetString("Resistance" + i));
                    if (resistance == null)
                    {
                        BooterLogger.AddError(Name + " Unknown Resistance " + i + ": " + row.GetString("Resistance" + i));
                    }
                    else
                    {
                        mResistances.Add(resistance);
                    }
                }

                int numMutables = row.GetInt("NumMutables", 0) + 1;

                for (int i = 1; i <= numMutables; i++)
                {
                    if (i == numMutables)
                    {
                        if (row.Exists("Mutable" + i))
                        {
                            BooterLogger.AddError(Name + " More Mutable than NumMutables");
                        }
                        break;
                    }

                    List<string> mutable = row.GetStringList("Mutable" + i, ',');
                    if (mutable.Count != 3)
                    {
                        BooterLogger.AddError(Name + " Unknown Mutable " + i + ": " + row.GetString("Mutable" + i));
                        continue;
                    }

                    int minimum;
                    if (!int.TryParse(mutable[1], out minimum))
                    {
                        BooterLogger.AddError(Name + " Mutable " + i + " Invalid Minimum: " + mutable[1]);
                        continue;
                    }

                    int maximum;
                    if (!int.TryParse(mutable[2], out maximum))
                    {
                        BooterLogger.AddError(Name + " Mutable " + i + " Invalid Maximum: " + mutable[2]);
                        continue;
                    }

                    mMutables.Add(new Mutable(mutable[0], minimum, maximum));
                }
            }

            public void GetSymptoms(Dictionary<string, SymptomBooter.Data> symptoms)
            {
                foreach (SymptomChance chance in mSymptoms)
                {
                    chance.GetSymptoms(symptoms);
                }
            }

            public void GetMutables(Dictionary<string, bool> mutables)
            {
                foreach (Mutable mutable in mMutables)
                {
                    mutables[mutable.mCounter] = true;
                }
            }

            public void GetResistances(Dictionary<string, ResistanceBooter.Data> resistances)
            {
                foreach (ResistanceBooter.Data resistance in mResistances)
                {
                    if (resistances.ContainsKey(resistance.Guid)) continue;

                    resistances.Add(resistance.Guid, resistance);
                }
            }

            public bool HasType(VectorControl.StageType type)
            {
                return ((mType & type) == type);
            }

            public string GetStory(bool isIdentified)
            {
                if (isIdentified)
                {
                    if (!string.IsNullOrEmpty(mStoryIdentified))
                    {
                        return mStoryIdentified;
                    }
                }

                return mStoryUnknown;
            }

            public float MutationRate
            {
                get { return mMutationRate; }
            }

            public int GoodRelationshipDelta
            {
                get { return mGoodRelationshipDelta; }
            }

            public int BadRelationshipDelta
            {
                get { return mBadRelationshipDelta; }
            }

            public string Name
            {
                get { return mName; }
            }

            public VectorControl.StageType Type
            {
                get { return mType; }
            }

            public bool CanInoculate
            {
                get
                {
                    return mCanInoculate;
                }
            }

            public float GetInfectionRate(VectorControl.Virulence type)
            {
                switch (type)
                {
                    case VectorControl.Virulence.Outdoors:
                        return mOutdoorInfectionRate;
                    case VectorControl.Virulence.Room:
                        return mRoomInfectionRate;
                    case VectorControl.Virulence.Social:
                        return mSocialInfectionRate;
                    case VectorControl.Virulence.Woohoo:
                        return mWoohooInfectionRate;
                    case VectorControl.Virulence.Fight:
                        return mFightInfectionRate;
                }

                return 0;
            }

            public virtual int GetDuration(DiseaseVector vector)
            {
                return 0;
            }

            public virtual void GetSettings(List<string> settings)
            { }

            public abstract int GetNextStage(SimDescription sim, DiseaseVector vector);

            public abstract bool ValidateStages(Dictionary<string,int> stages);

            public virtual void Symptomize(Sim sim, DiseaseVector vector)
            {
                if ((string.IsNullOrEmpty(mSymptomScoring)) ||
                    (ScoringLookup.GetScore(mSymptomScoring, sim.SimDescription) >= mSymptomScoringMinimum))
                {
                    foreach (SymptomChance chance in mSymptoms)
                    {
                        chance.Perform(sim, vector);
                    }
                }
            }

            public virtual void Resist(Event e, DiseaseVector vector)
            {
                foreach (ResistanceBooter.Data resistance in mResistances)
                {
                    resistance.Perform(e, vector);
                }
            }

            public virtual DiseaseVector.Variant Mutate (string guid, DiseaseVector.Variant strain)
            {
                if (!RandomUtil.RandomChance01(mMutationRate)) return strain;

                if (mMutables.Count == 0) return strain;

                if (!strain.Mutate(RandomUtil.GetRandomObjectFromList(mMutables))) return strain;

                return strain;
            }

            public override string ToString()
            {
                string result = Name;
                result += Common.NewLine + " Class: " + GetType().Name;
                result += Common.NewLine + " Type: " + mType;
                result += Common.NewLine + " Outdoor Infection Rate: " + mOutdoorInfectionRate;
                result += Common.NewLine + " Room Infection Rate: " + mRoomInfectionRate;
                result += Common.NewLine + " Social Infection Rate: " + mSocialInfectionRate;
                result += Common.NewLine + " Fight Infection Rate: " + mFightInfectionRate;
                result += Common.NewLine + " Woohoo Infection Rate: " + mWoohooInfectionRate;
                result += Common.NewLine + " Mutation Rate: " + mMutationRate;
                result += Common.NewLine + " Can Inoculate: " + mCanInoculate;
                result += Common.NewLine + " Bad Relationship Delta: " + mBadRelationshipDelta;
                result += Common.NewLine + " Good Relationship Delta: " + mGoodRelationshipDelta;
                result += Common.NewLine + " Story Unknonwn: " + mStoryUnknown;
                result += Common.NewLine + " Story Identified: " + mStoryIdentified;

                result += Common.NewLine + " Mutables:";
                foreach (Mutable mutable in mMutables)
                {
                    result += Common.NewLine + "  " + mutable;
                }

                result += Common.NewLine + " Symptom Scoring: " + mSymptomScoring;
                result += Common.NewLine + " Symptom Scoring Minimum: " + mSymptomScoringMinimum;

                result += Common.NewLine + " Symptoms:";
                foreach (SymptomChance chance in mSymptoms)
                {
                    result += Common.NewLine + chance;
                }

                result += Common.NewLine + " Resistances:";
                foreach (ResistanceBooter.Data resistance in mResistances)
                {
                    result += Common.NewLine + "  " + resistance.Guid + " [" + resistance.Delta + "]";
                }

                return result;
            }
        }

        public abstract class Test
        {
            public abstract void GetEvents(Dictionary<EventTypeId,bool> events);

            public abstract bool IsSuccess(Event e);
        }

        public class Data
        {
            string mGuid;

            int mInitialStrength;

            List<Stage> mStages;

            string mInfectionScoring;

            int mInfectionMinimum;

            int mInoculationRating;
            int mInoculationCost;

            int mResistanceCost;

            bool mCanOutbreak;

            int mHighProtectionCost;
            int mLowProtectionCost;

            int mMinimumStrainDifference;

            float mStrainMutationRate = -1;

            List<string> mCustomSettings = null;

            List<InstigatorBooter.Data> mInstigators = new List<InstigatorBooter.Data>();

            // On Demand
            Dictionary<string, bool> mMutables = null;

            public Data(string guid, BooterHelper.BootFile file, XmlDbRow row)
            {
                mGuid = guid;

                if (BooterLogger.Exists(row, "InitialStrength", guid))
                {
                    mInitialStrength = row.GetInt("InitialStrength", 0);
                }

                mInfectionScoring = row.GetString("InfectionScoring");
                if (string.IsNullOrEmpty(mInfectionScoring))
                {
                    BooterLogger.AddError(Guid + " Missing InfectionScoring: " + mInfectionScoring);
                }
                else if (ScoringLookup.GetScoring(mInfectionScoring) == null)
                {
                    BooterLogger.AddError(Guid + " Missing InfectionScoring: " + mInfectionScoring);
                }

                mInfectionMinimum = row.GetInt("InfectionMinimum", 0);

                VectorStageTable table = new VectorStageTable(file as BooterHelper.DataBootFile, guid);
                if (!table.IsValid)
                {
                    BooterLogger.AddError(Guid + " Missing Stages: " + guid);
                }
                else
                {
                    Dictionary<string, int> namesToStages = new Dictionary<string, int>();

                    for (int i = 0; i < table.Stages.Count; i++)
                    {
                        if (namesToStages.ContainsKey(table.Stages[i].Name))
                        {
                            BooterLogger.AddError(Guid + " Stage " + i + ": Duplicate Name " + table.Stages[i].Name);
                            continue;
                        }

                        namesToStages.Add(table.Stages[i].Name, i);
                    }

                    for (int i = 0; i < table.Stages.Count; i++)
                    {
                        BooterLogger.AddTrace("Stage " + i);

                        table.Stages[i].ValidateStages(namesToStages);
                    }
                }

                mStages = table.Stages;

                mInoculationRating = row.GetInt("InoculationRating", 10);
                mInoculationCost = row.GetInt("InoculationCost", 0);
                mResistanceCost = row.GetInt("ResistanceCost", 0);

                mCanOutbreak = row.GetBool("CanOutbreak");

                mHighProtectionCost = row.GetInt("HighProtectionCost", 0);
                mLowProtectionCost = row.GetInt("LowProtectionCost", 0);

                mMinimumStrainDifference = row.GetInt("MinimumStrainDifference", 0);

                mStrainMutationRate = row.GetInt("StrainMutationRate", -1);

                int numInstigators = row.GetInt("NumInstigators", 0) + 1;

                for (int i = 1; i <= numInstigators; i++)
                {
                    if (i == numInstigators)
                    {
                        if (row.Exists("Instigator" + i))
                        {
                            BooterLogger.AddError(Guid + " More Instigator then NumInstigators");
                        }
                        break;
                    }

                    mInstigators.Add(InstigatorBooter.GetInstigator(row.GetString("Instigator" + i)));
                }
            }

            public string Guid
            {
                get { return mGuid; }
            }

            public IEnumerable<string> CustomSettings
            {
                get 
                {
                    if (mCustomSettings == null)
                    {
                        mCustomSettings = new List<string>();

                        foreach (Stage stage in mStages)
                        {
                            stage.GetSettings(mCustomSettings);
                        }
                    }
                    return mCustomSettings; 
                }
            }

            public string GetLocalizedName(bool isFemale)
            {
                return Common.Localize("VectorName:" + Guid, isFemale);
            }

            public Dictionary<string, bool> AllMutables
            {
                get 
                {
                    if (mMutables == null)
                    {
                        mMutables = new Dictionary<string, bool>();

                        foreach (Stage stage in mStages)
                        {
                            stage.GetMutables(mMutables);
                        }
                    }
                    return mMutables;
                }
            }

            public bool HasVirulence(int stage, VectorControl.Virulence virulence)
            {
                return (GetInfectionRate(stage, virulence) > 0);
            }

            public bool IsEnabled
            {
                get
                {
                    return Vector.Settings.IsEnabled(Guid);
                }
            }

            public float StrainMutationRate
            {
                get
                {
                    if (mStrainMutationRate < 0)
                    {
                        foreach (Stage stage in mStages)
                        {
                            if (mStrainMutationRate < stage.MutationRate)
                            {
                                mStrainMutationRate = stage.MutationRate;
                            }
                        }
                    }

                    return mStrainMutationRate;
                }
            }

            public int MinimumStrainDifference
            {
                get 
                {
                    if (mMinimumStrainDifference < 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return mMinimumStrainDifference + Vector.Settings.mStrainReinfectionDifferenceV2;
                    }
                } 
            }

            public bool HasInstigators
            {
                get { return (mInstigators.Count > 0); }
            }

            public bool CanOutbreak
            {
                get { return mCanOutbreak; }
            }

            public int InitialStrength
            {
                get { return mInitialStrength; }
            }

            public bool CanInoculate(int stage)
            {
                if (InoculationCost <= 0) return false;

                return mStages[stage].CanInoculate;
            }

            public int HighProtectionCost
            {
                get { return mHighProtectionCost; }
            }

            public int LowProtectionCost
            {
                get { return mLowProtectionCost; }
            }

            public long InoculationStrain
            {
                get { return mInoculationRating + Vector.Settings.GetCurrentStrain(this).Strain; }
            }

            public int InoculationCost
            {
                get { return mInoculationCost; }
            }

            public bool CanBoostResistance
            {
                get { return (mResistanceCost > 0); }
            }

            public int ResistanceCost
            {
                get { return mResistanceCost; }
            }

            public int NumStages
            {
                get { return mStages.Count; }
            }

            public string GetStageName(int stage)
            {
                return mStages[stage].Name;
            }

            public int GetRelationshipDelta(int stage, bool positive)
            {
                if (positive)
                {
                    return mStages[stage].GoodRelationshipDelta;
                }
                else
                {
                    return mStages[stage].BadRelationshipDelta;
                }
            }

            public string GetStory(int stage, bool identified)
            {
                return mStages[stage].GetStory(identified);
            }

            public float GetInfectionRate(int stage, VectorControl.Virulence type)
            {
                return mStages[stage].GetInfectionRate(type);
            }

            public bool CanInfect(SimDescription sim, SimDescription source)
            {
                if (SimTypes.IsDead(sim)) return false;

                if (sim.HasTrait(TraitNames.Simmunity)) return false;

                if ((SimTypes.IsOccult(sim, Sims3.UI.Hud.OccultTypes.ImaginaryFriend)) || 
                    (SimTypes.IsOccult(sim, Sims3.UI.Hud.OccultTypes.Genie)))
                {
                    if (SimTypes.IsService(sim)) return false;
                }

                return (ScoringLookup.GetScore(mInfectionScoring, sim, source) >= mInfectionMinimum);
            }

            public void GetResistances(Dictionary<string,ResistanceBooter.Data> resistances)
            {
                foreach (Stage stage in mStages)
                {
                    stage.GetResistances(resistances);
                }
            }

            public void RegisterInstigators()
            {
                foreach (InstigatorBooter.Data instigator in mInstigators)
                {
                    instigator.Register(this);
                }
            }

            public void Resist(Event e, DiseaseVector vector)
            {
                mStages[vector.Stage].Resist(e, vector);
            }

            public void Symptomize(Sim sim, DiseaseVector vector)
            {
                if (sim == null) return;

                mStages[vector.Stage].Symptomize(sim, vector);
            }

            public DiseaseVector.Variant Mutate(DiseaseVector.Variant strain, int stage)
            {
                return mStages[stage].Mutate(Guid, strain);
            }

            public VectorControl.StageType InitialStage(DiseaseVector vector, out int nextCheck)
            {
                return RunStage(vector, 0, out nextCheck);
            }

            public VectorControl.StageType AdjustStage(DiseaseVector vector, SimDescription sim, out int stage, out int nextCheck)
            {
                stage = mStages[vector.Stage].GetNextStage(sim, vector);

                VectorControl.StageType result = VectorControl.StageType.Resisted;
                if ((stage < 0) || (stage >= mStages.Count))
                {
                    nextCheck = 0;
                }
                else
                {
                    result = RunStage(vector, stage, out nextCheck);
                }

                int localStage = stage;

                if (Common.kDebugging)
                {
                    Common.DebugNotify(vector.IsIgnored ? "" : vector.UnlocalizedName + Common.NewLine + "Stage: " + GetStageName(localStage) + Common.NewLine + sim.FullName, sim.CreatedSim);
                }

                return result;
            }

            protected VectorControl.StageType RunStage(DiseaseVector vector, int stage, out int nextCheck)
            {
                Stage data = mStages[stage];

                nextCheck = data.GetDuration(vector);
                if (nextCheck < 0) nextCheck = 0;

                return data.Type;
            }

            public string GetResearch(bool isFemale)
            {
                string result = GetLocalizedName(isFemale);

                result += Common.NewLine + Common.Localize("VectorDescription:" + Guid) + Common.NewLine;

                Dictionary<string, SymptomBooter.Data> symptoms = new Dictionary<string, SymptomBooter.Data>();

                foreach (Stage stage in mStages)
                {
                    stage.GetSymptoms(symptoms);
                }

                if (symptoms.Count > 0)
                {
                    result += Common.NewLine + Common.Localize("Symptoms:Title");

                    foreach (SymptomBooter.Data symptom in symptoms.Values)
                    {
                        result += Common.NewLine + " " + symptom.GetLocalizedName(isFemale);
                    }
                }

                Dictionary<string, ResistanceBooter.Data> resistances = new Dictionary<string, ResistanceBooter.Data>();
                GetResistances(resistances);

                bool first = true;
                foreach (ResistanceBooter.Data resistance in resistances.Values)
                {
                    if (resistance.Delta <= 0) continue;

                    if (first)
                    {
                        first = false;
                        result += Common.NewLine + Common.Localize("Resistances:Title");
                    }

                    result += Common.NewLine + " " + resistance.GetLocalizedName(isFemale);
                }

                first = true;
                foreach (ResistanceBooter.Data resistance in resistances.Values)
                {
                    if (resistance.Delta >= 0) continue;

                    if (first)
                    {
                        first = false;
                        result += Common.NewLine + Common.Localize("Inflamatory:Title");
                    }

                    result += Common.NewLine + " " + resistance.GetLocalizedName(isFemale);
                }

                first = true;
                foreach (VectorControl.Virulence virulence in Enum.GetValues(typeof(VectorControl.Virulence)))
                {
                    if (virulence == VectorControl.Virulence.Inert) continue;

                    float highestRate = 0;

                    foreach (Stage stage in mStages)
                    {
                        float rate = stage.GetInfectionRate(virulence);
                        if (highestRate < rate)
                        {
                            highestRate = rate;
                        }
                    }

                    if (highestRate > 0)
                    {
                        if (first)
                        {
                            first = false;
                            result += Common.NewLine + Common.Localize("Research:Contagious", isFemale);
                        }

                        result += Common.NewLine + Common.Localize("Research:" + virulence, isFemale, new object[] { (int)(highestRate * 100) });
                    }
                }

                bool canInoculate = false;

                foreach (Stage stage in mStages)
                {
                    if (stage.CanInoculate)
                    {
                        canInoculate = (mInoculationCost > 0);
                        break;
                    }
                }

                result += Common.NewLine + Common.NewLine + Common.Localize("Research:Summary", isFemale);
                result += Common.NewLine + Common.Localize("Research:CanInoculate", isFemale, new object[] { Common.Localize("YesNo:" + canInoculate) });
                if (canInoculate)
                {
                    result += Common.NewLine + Common.Localize("Research:Cost", isFemale, new object[] { InoculationCost });
                    result += Common.NewLine + Common.Localize("Research:InoculateDuration", isFemale, new object[] { InoculationStrain });
                }
                result += Common.NewLine + Common.Localize("Research:CanBuyResistance", isFemale, new object[] { Common.Localize("YesNo:" + CanBoostResistance) });
                if (CanBoostResistance)
                {
                    result += Common.NewLine + Common.Localize("Research:Cost", isFemale, new object[] { ResistanceCost });
                }

                result += Common.NewLine + Common.Localize("Research:Protection", isFemale, new object[] { Common.Localize("YesNo:" + ((mHighProtectionCost > 0) || (mLowProtectionCost > 0))) });
                if (mHighProtectionCost > 0) 
                {
                    result += Common.NewLine + Common.Localize("Research:HighProtection", isFemale, new object[] { mHighProtectionCost });                    
                }
                
                if (mLowProtectionCost > 0)
                {
                    result += Common.NewLine + Common.Localize("Research:LowProtection", isFemale, new object[] { mLowProtectionCost });
                }

                return result;
            }

            public override string ToString()
            {
                string msg = "Vector: " + mGuid;

                msg += Common.NewLine + " Initial Strength: " + mInitialStrength;
                msg += Common.NewLine + " Infection Scoring: " + mInfectionScoring;
                msg += Common.NewLine + " Infection Minimum: " + mInfectionMinimum;
                msg += Common.NewLine + " Inoculation Cost: " + mInoculationCost;
                msg += Common.NewLine + " Inoculation Rating: " + mInoculationRating;
                msg += Common.NewLine + " Resistance Cost: " + mResistanceCost;
                msg += Common.NewLine + " High Protection Cost: " + mHighProtectionCost;
                msg += Common.NewLine + " Low Protection Cost: " + mLowProtectionCost;
                msg += Common.NewLine + " Min Strain Difference: " + mMinimumStrainDifference;

                foreach (string setting in CustomSettings)
                {
                    msg += Common.NewLine + " Setting: " + setting + " [" + Vector.Settings.IsSet(setting) + "]";
                }

                foreach (Stage stage in mStages)
                {
                    msg += Common.NewLine + stage;
                }

                return msg;
            }
        }

        public class Item : ValueSettingOption<VectorBooter.Data>
        {
            public Item(DiseaseVector vector, bool isFemale)
                : base(vector.Data, vector.GetLocalizedName(isFemale), vector.IsIdentified ? 1 : 0)
            { }
            public Item(VectorBooter.Data vector, bool isFemale, int cost, bool booster)
                : base(vector, GetName(vector, isFemale, booster), cost)
            { }
            public Item(VectorBooter.Data vector, bool isFemale)
                : base(vector, vector.GetLocalizedName(isFemale), (int)Vector.Settings.GetCurrentStrain(vector).Strain)
            { }

            protected static string GetName(VectorBooter.Data vector, bool isFemale, bool booster)
            {
                string name = vector.GetLocalizedName(isFemale);

                if (booster)
                {
                    return Common.Localize("GetInoculation:Booster", isFemale, new object[] { name });
                }
                else
                {
                    return name;
                }
            }

            public override bool UsingCount
            {
                get { return false; }
            }
        }
    }
}
