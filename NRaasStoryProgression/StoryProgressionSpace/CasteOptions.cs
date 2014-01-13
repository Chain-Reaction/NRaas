using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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
            }

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
                }
                else
                {
                    if (!SimTypes.MatchesAny(sim, mTypes, true)) return false;
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

