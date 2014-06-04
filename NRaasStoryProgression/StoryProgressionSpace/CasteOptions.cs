using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    [Persistable]
    public class CasteOptions : GenericOptionBase, IPersistence
    {
        ulong mID;

        [Persistable(false)]
        CasteFilter mFilter;

        public CasteOptions() // Required for persistence
        { }
        public CasteOptions(ulong id)
        {
            mID = id;
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
            get { return null;  }
        }

        public ulong ID
        {
            get { return mID; }
        }

        public string DefaultName
        {
            get { return GetValue<CasteDefaultNameOption, string>(); }
        }

        public override string Name
        {
            get { return GetValue<CasteNameOption,string>(); }
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public int Priority
        {
            get
            {
                return Filter.mPriority;
            }
        }

        protected CasteFilter Filter
        {
            get
            {
                if (mFilter == null)
                {
                    mFilter = new CasteFilter(this);
                }

                return mFilter;
            }
        }

        public bool Matches(SimDescription sim, int netWorth)
        {
            return Filter.Matches(sim, netWorth);
        }

        public void InvalidateFilter()
        {
            mFilter = null;
        }

        public void Import(Persistence.Lookup settings)
        {
            mID = settings.GetULong("ID", 0);

            base.Import(StoryProgression.Main, mID.ToString(), settings);
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("ID", mID);

            base.Export(StoryProgression.Main, mID.ToString(), settings);
        }

        public R GetValue<T, R>()
            where T : GenericOptionItem<R>, IReadCasteLevelOption, new()
        {
            return GetInternalValue<T, R>();
        }

        public bool SetValue<T, R>(R value)
            where T : GenericOptionItem<R>, IWriteCasteLevelOption, new()
        {
            return SetInternalValue<T, R>(value);
        }

        public TType AddValue<T, TType>(TType value)
            where T : OptionItem, IGenericAddOption<TType>, IWriteCasteLevelOption, new()
        {
            return AddInternalValue<T, TType>(value);
        }

        public void RemoveValue<T, TType>(TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IWriteCasteLevelOption, new()
        {
            RemoveInternalValue<T, TType>(value);
        }

        public bool HasValue<T, TType>(TType value)
            where T : OptionItem, IGenericHasOption<TType>, IReadCasteLevelOption, new()
        {
            return HasInternalValue<T, TType>(value);
        }

        public override void UpdateInheritors(IGenericLevelOption option)
        {
            base.UpdateInheritors(option);

            InvalidateFilter();

            bool updateAll = ((option is ISimCasteOption) || (GetValue<CasteApplyToHouseOption,bool>()));

            Dictionary<Household, bool> houses = new Dictionary<Household, bool>();

            foreach (SimDescription sim in StoryProgression.Main.Sims.All)
            {
                SimData data = StoryProgression.Main.GetData(sim);

                if (!updateAll)
                {
                    if (!data.Contains(this)) continue;
                }
                else
                {
                    data.InvalidateCache();
                }

                data.Uncache(option);

                if (!data.IsValidOption(option))
                {
                    data.RemoveOption(option);
                }

                if (sim == SimTypes.HeadOfFamily(sim.Household))
                {
                    if (!houses.ContainsKey(sim.Household))
                    {
                        houses.Add(sim.Household, true);

                        HouseholdOptions options = StoryProgression.Main.GetHouseOptions(sim.Household);
                        if (options != null)
                        {
                            if (updateAll)
                            {
                                options.InvalidateCache();
                            }

                            options.Uncache(option);

                            options.UpdateInheritors(option);
                        }
                    }
                }
            }
        }

        public override bool IsValidOption(IGenericLevelOption option)
        {
            if (option is INotCasteLevelOption) return false;

            if (option is IReadCasteLevelOption) return true;

            return (option is IReadSimLevelOption);
        }

        protected override void GetParentOptions(List<ParentItem> options, DefaultingLevel level)
        { }

        public static int SortByPriority(CasteOptions l, CasteOptions r)
        {
            return l.Priority.CompareTo(r.Priority);
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder("<CasteOptions>");

            text.AddXML("ID", mID);
            text += base.ToString();

            text += Common.NewLine + "</CasteOptions>";

            return text.ToString();
        }

        public class CasteFilter
        {
            static CASAgeGenderFlags sAgeGenderMask = CASAgeGenderFlags.AgeMask | CASAgeGenderFlags.GenderMask;

            public readonly bool mAutomatic;

            CASAgeGenderFlags mAgeGender;

            Dictionary<CASAgeGenderFlags,bool> mSpecies;

            List<SimType> mTypes;
            bool mMatchAll;

            Dictionary<string, bool> mPersonalityLeaders;
            Dictionary<string, bool> mPersonalityMembers;

            Dictionary<TraitNames, bool> mRequiredTraits;
            Dictionary<TraitNames, bool> mDeniedTraits;

            int mMinNetWorth;
            int mMaxNetWorth;

            IScoringMethod<SimDescription, SimScoringParameters> mScoring;

            public readonly int mPriority;

            List<Zodiac> mZodiac;

            List<OccupationNames> mCareer;
            Dictionary<OccupationNames, List<int>> mCareerLevel;

            List<SkillNames> mSkill;
            Dictionary<SkillNames, List<int>> mSkillLevel;

            List<AcademicDegreeNames> mDegree;

            public CasteFilter(CasteOptions options)
            {
                mPriority = options.GetValue<CastePriorityOption, int>();

                mAutomatic = options.GetValue<CasteAutoOption, bool>();

                foreach(CASAgeGenderFlags flag in options.GetValue<CasteAgeOption, List<CASAgeGenderFlags>>())
                {
                    mAgeGender |= flag;
                }

                foreach(CASAgeGenderFlags flag in options.GetValue<CasteGenderOption, List<CASAgeGenderFlags>>())
                {
                    mAgeGender |= flag;
                }

                mSpecies = new Dictionary<CASAgeGenderFlags, bool>();

                foreach(CASAgeGenderFlags flag in options.GetValue<CasteSpeciesOption, List<CASAgeGenderFlags>>())
                {
                    mSpecies.Add(flag, true);
                }

                mTypes = new List<SimType>(options.GetValue<CasteTypeOption, List<SimType>>());

                mMatchAll = options.GetValue<CasteMatchAllTypeOption, bool>();

                mMinNetWorth = options.GetValue<CasteFundsMinOption, int>();
                mMaxNetWorth = options.GetValue<CasteFundsMaxOption, int>();

                string scoring = options.GetValue<CasteScoringOption, string>();
                if (!string.IsNullOrEmpty(scoring))
                {
                    mScoring = ScoringLookup.GetScoring(scoring) as IScoringMethod<SimDescription, SimScoringParameters>;
                }

                mPersonalityLeaders = new Dictionary<string, bool>();

                foreach (string personality in options.GetValue<CastePersonalityLeaderOption, List<string>>())
                {
                    mPersonalityLeaders[personality] = true;
                }

                mPersonalityMembers = new Dictionary<string, bool>();

                foreach(string personality in options.GetValue<CastePersonalityMemberOption,List<string>>())
                {
                    mPersonalityMembers[personality] = true;
                }

                mRequiredTraits = new Dictionary<TraitNames, bool>();

                foreach (TraitNames trait in options.GetValue<CasteTraitRequireOption, List<TraitNames>>())
                {
                    mRequiredTraits[trait] = true;
                }

                mDeniedTraits = new Dictionary<TraitNames, bool>();

                foreach (TraitNames trait in options.GetValue<CasteTraitDenyOption, List<TraitNames>>())
                {
                    mDeniedTraits[trait] = true;
                }

                // I need to refactor this but she works for now
                mSkill = new List<SkillNames>();

                foreach (string skill in options.GetValue<CasteSkillFilterOption, List<string>>())
                {
                    // we aren't storing this as skillnames cause twallan didn't for some reason.
                    // I assume changing it will wipe existing DisallowSkill settings
                    SkillNames skillName;
                    if (ParserFunctions.TryParseEnum<SkillNames>(skill, out skillName, SkillNames.None))
                    {
                        mSkill.Add(skillName);
                    }
                }

                mSkillLevel = new Dictionary<SkillNames, List<int>>();
                foreach (string skillLevel in options.GetValue<CasteSkillLevelFilterOption, List<string>>())
                {
                    string[] split = skillLevel.Split('-');
                    if (split.Length == 2)
                    {
                        SkillNames skillName;
                        if (ParserFunctions.TryParseEnum<SkillNames>(split[0], out skillName, SkillNames.None))
                        {
                            if (!mSkillLevel.ContainsKey(skillName))
                            {
                                mSkillLevel.Add(skillName, new List<int>());
                            }

                            int result;
                            if(int.TryParse(split[1], out result))
                            {
                                if (skillName != SkillNames.None && result > 0)
                                {
                                    mSkillLevel[skillName].Add(result);
                                }
                            }
                        }
                    }
                }

                mCareer = new List<OccupationNames>();

                foreach (OccupationNames career in options.GetValue<CasteCareerFilterOption, List<OccupationNames>>())
                {
                    mCareer.Add(career);
                }

                mCareerLevel = new Dictionary<OccupationNames, List<int>>();

                foreach (string careerLevel in options.GetValue<CasteCareerLevelFilterOption, List<string>>())
                {
                    string[] split = careerLevel.Split('-');
                    if (split.Length == 2)
                    {
                        OccupationNames careerName;
                        if (ParserFunctions.TryParseEnum<OccupationNames>(split[0], out careerName, OccupationNames.Undefined))
                        {
                            if (!mCareerLevel.ContainsKey(careerName))
                            {
                                mCareerLevel.Add(careerName, new List<int>());
                            }

                            int result;
                            if (int.TryParse(split[1], out result))
                            {
                                if (careerName != OccupationNames.Undefined && result > 0)
                                {
                                    mCareerLevel[careerName].Add(result);
                                }
                            }
                        }
                    }
                }

                mZodiac = new List<Zodiac>();

                foreach (Zodiac zodiac in options.GetValue<CasteZodiacFilterOption, List<Zodiac>>())
                {
                    mZodiac.Add(zodiac);
                }

                mDegree = new List<AcademicDegreeNames>();

                foreach (AcademicDegreeNames degree in options.GetValue<CasteDegreeFilterOption, List<AcademicDegreeNames>>())
                {
                    mDegree.Add(degree);
                }
            }

            public bool HasAnyDegree(SimDescription sim, List<AcademicDegreeNames> degrees)
            {
                if (sim.CareerManager == null || sim.CareerManager.DegreeManager == null)
                {
                    return false;
                }

                foreach (AcademicDegreeNames degree in degrees)
                {
                    if (sim.CareerManager.DegreeManager.HasCompletedDegree(degree)) return true;
                }

                return false;
            }

            public bool HasAllDegrees(SimDescription sim, List<AcademicDegreeNames> degrees)
            {
                if (sim.CareerManager == null || sim.CareerManager.DegreeManager == null)
                {
                    return false;
                }

                foreach (AcademicDegreeNames degree in degrees)
                {
                    if (!sim.CareerManager.DegreeManager.HasCompletedDegree(degree)) return false;
                }

                return true;
            }

            public bool HasAnyCareer(SimDescription sim, List<OccupationNames> careers)
            {
                if (sim.CareerManager == null || sim.CareerManager.Occupation == null)
                {
                    return false;
                }

                if (careers.Contains(sim.CareerManager.Occupation.Guid)) return true;

                return false;
            }

            public bool HasAllCareers(SimDescription sim, List<OccupationNames> careers)
            {
                // here for consistency but admittedly rather silly :)
                // will only loop once (or twice) so doesn't really matter
                if (sim.CareerManager == null || sim.CareerManager.Occupation == null)
                {
                    return false;
                }

                foreach (OccupationNames career in careers)
                {
                    if (sim.CareerManager.Occupation.Guid != career) return false;
                }

                return true;
            }

            public bool HasAnyCareerOfLevel(SimDescription sim, Dictionary<OccupationNames, List<int>> careers)
            {
                if (careers.Count == 0)
                {                    
                    return false;
                }

                if (!HasAnyCareer(sim, new List<OccupationNames>(careers.Keys))) return false;

                foreach (KeyValuePair<OccupationNames, List<int>> entry in careers)
                {
                    if (entry.Value.Count == 0)
                    {                        
                        continue;
                    }                   

                    if (entry.Value.Contains(sim.CareerManager.Occupation.Level)) return true;                    
                }

                return false;
            }

            public bool HasAllCareersOfLevel(SimDescription sim, Dictionary<OccupationNames, List<int>> careers)
            {
                if (careers.Count == 0)
                {
                    return false;
                }

                if (!HasAllCareers(sim, new List<OccupationNames>(careers.Keys))) return false;

                foreach (KeyValuePair<OccupationNames, List<int>> entry in careers)
                {
                    if (entry.Value.Count == 0)
                    {
                        continue;
                    }

                    if (!entry.Value.Contains(sim.CareerManager.Occupation.Level)) return false;
                }

                return true;
            }

            public bool HasAllSkills(SimDescription sim, List<SkillNames> skills)
            {
                if (sim.SkillManager == null || skills.Count == 0)
                {
                    return false;
                }

                foreach (SkillNames skill in skills)
                {
                    if (sim.SkillManager.GetElement(skill) == null || sim.SkillManager.GetElement(skill).SkillLevel == 0) return false;
                }

                return true;
            }

            public bool HasAnySkill(SimDescription sim, List<SkillNames> skills)
            {
                if (sim.SkillManager == null || skills.Count == 0)
                {
                    return false;
                }

                foreach (SkillNames skill in skills)
                {
                    if (sim.SkillManager.GetElement(skill) != null && sim.SkillManager.GetElement(skill).SkillLevel > 0) return true;
                }

                return false;
            }

            public bool HasAnySkillOfLevel(SimDescription sim, Dictionary<SkillNames, List<int>> skills)
            {
                if (skills.Count == 0)
                {
                    return false;
                }

                if (!HasAnySkill(sim, new List<SkillNames>(skills.Keys))) return false;

                foreach (KeyValuePair<SkillNames, List<int>> entry in skills)
                {
                    if (entry.Value.Count == 0)
                    {
                        continue;
                    }

                    if (entry.Value.Contains(sim.SkillManager.GetElement(entry.Key).SkillLevel)) return true;                    
                }

                return false;
            }

            public bool HasAllSkillsOfLevel(SimDescription sim, Dictionary<SkillNames, List<int>> skills)
            {
                if (skills.Count == 0)
                {
                    return false;
                }

                if (!HasAllSkills(sim, new List<SkillNames>(skills.Keys))) return false;

                foreach (KeyValuePair<SkillNames, List<int>> entry in skills)
                {
                    if (entry.Value.Count == 0)
                    {
                        continue;
                    }

                    if (!entry.Value.Contains(sim.SkillManager.GetElement(entry.Key).SkillLevel)) return false;
                }

                return true;
            }

            public bool HasAnyZodiac(SimDescription sim, List<Zodiac> zodiacs)
            {
                if (zodiacs.Count == 0)
                {
                    return false;
                }

                if (zodiacs.Contains(sim.Zodiac)) return true;

                return false;
            }

            public bool HasAllZodiacs(SimDescription sim, List<Zodiac> zodiacs)
            {
                // same case as the careers
                if (zodiacs.Count == 0)
                {
                    return false;
                }

                foreach (Zodiac zodiac in zodiacs)
                {
                    if (sim.Zodiac != zodiac) return false;
                }

                return true;
            }

            // the caste cache seems to be invalided too rapidly for my taste, something to look into...
            public bool Matches(SimDescription sim, int netWorth)
            {
                CASAgeGenderFlags ageGender = sim.mSimFlags & sAgeGenderMask;
                if ((mAgeGender & ageGender) != ageGender) return false;

                if (mSpecies.Count > 0)
                {
                    if (!mSpecies.ContainsKey(sim.Species)) return false;
                }

                if (mMatchAll)
                {
                    if (!SimTypes.MatchesAll(sim, mTypes)) return false;
                    if (mCareer.Count > 0)
                    {
                        if (!HasAllCareers(sim, mCareer)) return false;
                    }
                    if (mCareerLevel.Count > 0)
                    {
                        if (!HasAllCareersOfLevel(sim, mCareerLevel)) return false;
                    }
                    if (mSkill.Count > 0)
                    {
                        if (!HasAllSkills(sim, mSkill)) return false;
                    }
                    if (mSkillLevel.Count > 0)
                    {
                        if (!HasAllSkillsOfLevel(sim, mSkillLevel)) return false;
                    }
                    if (mZodiac.Count > 0)
                    {
                        if (!HasAllZodiacs(sim, mZodiac)) return false;
                    }
                }
                else
                {
                    if (!SimTypes.MatchesAny(sim, mTypes, true)) return false;
                    if (mCareer.Count > 0)
                    {
                        if (!HasAnyCareer(sim, mCareer)) return false;
                    }
                    if (mCareerLevel.Count > 0)
                    {
                        if (!HasAnyCareerOfLevel(sim, mCareerLevel)) return false;
                    }
                    if (mSkill.Count > 0)
                    {
                        if (!HasAnySkill(sim, mSkill)) return false;
                    }
                    if (mSkillLevel.Count > 0)
                    {
                        if (!HasAnySkillOfLevel(sim, mSkillLevel)) return false;
                    }
                    if (mZodiac.Count > 0)
                    {
                        if (!HasAnyZodiac(sim, mZodiac)) return false;
                    }
                }

                if (netWorth < mMinNetWorth) return false;

                if (netWorth > mMaxNetWorth) return false;

                if (mScoring != null)
                {
                    if (mScoring.Score(new SimScoringParameters(sim)) <= 0) return false;
                }

                if (mPersonalityLeaders.Count > 0)
                {
                    bool found = false;
                    foreach (SimPersonality personality in StoryProgression.Main.Personalities.GetClanLeadership(sim))
                    {
                        if (mPersonalityLeaders.ContainsKey(personality.UnlocalizedName))
                        {
                            found = true;
                        }
                    }

                    if (!found) return false;
                }

                if (mPersonalityMembers.Count > 0)
                {
                    bool found = false;
                    foreach (SimPersonality personality in StoryProgression.Main.Personalities.GetClanMembership(sim, false))
                    {
                        if (mPersonalityMembers.ContainsKey(personality.UnlocalizedName))
                        {
                            found = true;
                        }
                    }

                    if (!found) return false;
                }

                if (mRequiredTraits.Count > 0)
                {
                    bool found = false;

                    foreach (Trait trait in sim.TraitManager.List)
                    {
                        if (mRequiredTraits.ContainsKey(trait.Guid))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found) return false;
                }

                if (mDeniedTraits.Count > 0)
                {
                    foreach (Trait trait in sim.TraitManager.List)
                    {
                        if (mDeniedTraits.ContainsKey(trait.Guid))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}

