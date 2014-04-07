using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerCaste : Manager
    {
        static Dictionary<string, bool> sCheckedCastes = new Dictionary<string, bool>();

        public ManagerCaste(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Castes";
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (!settings.GetValue<AllowCasteOption, bool>())
            {
                stats.IncStat("Allow: Caste Denied");
                return false;
            }

            return true;
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerCaste>(this).Perform(initial);
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (initialPass)
            {
                InitializeCastes();
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public static void AddDefaultCaste(string value)
        {
            sCheckedCastes[value] = true;
        }

        public delegate void OnInitialize();

        public static event OnInitialize OnInitializeCastes;

        protected void InitializeCastes()
        {
            bool teenCreated;

            int currentVersion = GetValue<Version,int>();

            sCheckedCastes.Clear();

            CasteOptions teenOptions = Options.GetNewCasteOptions("Teenagers", Common.Localize("Caste:Teen"), out teenCreated);
            if (teenCreated)
            {
                teenOptions.SetValue<CasteAutoOption, bool>(true);
                teenOptions.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                teenOptions.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Teen);

                teenOptions.SetValue<DisallowCareerOption, List<OccupationNames>>(null);
            }

            if (teenOptions != null)
            {
                if (currentVersion < 1)
                {
                    teenOptions.AddValue<DisallowCareerOption, OccupationNames>(OccupationNames.Firefighter);
                    teenOptions.AddValue<DisallowCareerOption, OccupationNames>(OccupationNames.GhostHunter);
                    teenOptions.AddValue<DisallowCareerOption, OccupationNames>(OccupationNames.InteriorDesigner);
                    teenOptions.AddValue<DisallowCareerOption, OccupationNames>(OccupationNames.Stylist);
                    teenOptions.AddValue<DisallowCareerOption, OccupationNames>(OccupationNames.AcademicCareer);
                }

                if (currentVersion < 2)
                {
                    teenOptions.SetValue<AllowAdoptionOption, bool>(false);
                }
            }

            bool youngAdultCreated;
            CasteOptions youngAdultOptions = Options.GetNewCasteOptions("YoungAdults", Common.Localize("Caste:YoungAdult"), out youngAdultCreated);
            if (youngAdultCreated)
            {
                youngAdultOptions.SetValue<CasteAutoOption, bool>(true);
                youngAdultOptions.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                youngAdultOptions.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.YoungAdult);
                youngAdultOptions.SetValue<AllowMoveSoloOption, bool>(true);
            }

            if (youngAdultOptions != null)
            {
                if (currentVersion < 1)
                {
                    if (!GameUtils.IsUniversityWorld())
                    {
                        youngAdultOptions.AddValue<DisallowCareerOption, OccupationNames>(OccupationNames.AcademicCareer);
                    }
                }
            }

            bool adultCreated;
            CasteOptions adultOptions = Options.GetNewCasteOptions("Adults", Common.Localize("Caste:Adult"), out adultCreated);
            if (adultCreated)
            {
                adultOptions.SetValue<CasteAutoOption, bool>(true);
                adultOptions.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                adultOptions.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Adult);
                adultOptions.SetValue<AllowMoveSoloOption, bool>(true);
            }

            if (adultOptions != null)
            {
                if (currentVersion < 1)
                {
                    if (!GameUtils.IsUniversityWorld())
                    {
                        adultOptions.AddValue<DisallowCareerOption, OccupationNames>(OccupationNames.AcademicCareer);
                    }
                }
            }

            bool elderCreated;
            CasteOptions elderOptions = Options.GetNewCasteOptions("Elders", Common.Localize("Caste:Elder"), out elderCreated);
            if (elderCreated)
            {
                elderOptions.SetValue<CasteAutoOption, bool>(true);
                elderOptions.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                elderOptions.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Elder);
                elderOptions.SetValue<AllowMoveSoloOption, bool>(true);
            }

            if (elderOptions != null)
            {
                if (currentVersion < 1)
                {
                    if (!GameUtils.IsUniversityWorld())
                    {
                        elderOptions.AddValue<DisallowCareerOption, OccupationNames>(OccupationNames.AcademicCareer);
                    }
                }
            }

            bool maleCreated;
            CasteOptions maleOptions = Options.GetNewCasteOptions("Males", Common.Localize("Caste:Male"), out maleCreated);
            if (maleCreated)
            {
                maleOptions.SetValue<CasteAutoOption, bool>(true);
                maleOptions.SetValue<CasteGenderOption, List<CASAgeGenderFlags>>(null);
                maleOptions.AddValue<CasteGenderOption, CASAgeGenderFlags>(CASAgeGenderFlags.Male);

                maleOptions.AddValue<DisallowCastePregnancyOption, CasteOptions>(maleOptions);
                maleOptions.SetValue<AllowCanBePregnantOption, bool>(false);
            }

            bool femaleCreated;
            CasteOptions femaleOptions = Options.GetNewCasteOptions("Females", Common.Localize("Caste:Female"), out femaleCreated);
            if (femaleCreated)
            {
                femaleOptions.SetValue<CasteAutoOption, bool>(true);
                femaleOptions.SetValue<CasteGenderOption, List<CASAgeGenderFlags>>(null);
                femaleOptions.AddValue<CasteGenderOption, CASAgeGenderFlags>(CASAgeGenderFlags.Female);

                femaleOptions.AddValue<DisallowCastePregnancyOption, CasteOptions>(femaleOptions);
            }

            if (maleCreated)
            {
                maleOptions.AddValue<DisallowCasteCanBePregnantOption, CasteOptions>(femaleOptions);
            }

            // No Service Flirt, Friends, Personality, Pregnancies
            bool created;
            CasteOptions options = Options.GetNewCasteOptions("ServicePopulation", Common.Localize("Caste:Service"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.Service);

                options.SetValue<AllowRomanceOption, bool>(false);
                options.SetValue<AllowFriendshipOption, bool>(false);
                options.SetValue<AllowMarriageOption, bool>(false);
                options.SetValue<AllowPersonalityOption, bool>(false);
                options.SetValue<AllowPregnancyParticipationOption, bool>(false);
                options.SetValue<AllowPushAtDayOption, bool>(false);
                options.SetValue<AllowPushAtNightOption, bool>(false);
                options.SetValue<AllowStoryOption, bool>(false);
                options.SetValue<AllowPushWorkOption, bool>(false);
            }

            // No Homeless Pregnancies
            options = Options.GetNewCasteOptions("Homeless", Common.Localize("Caste:Homeless"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.Townie);

                options.SetValue<AllowPregnancyParticipationOption, bool>(false);
            }

            // No Tourist Flirt, Friends, Pregnancies
            options = Options.GetNewCasteOptions("Tourists", Common.Localize("Caste:Tourist"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.Tourist);

                options.SetValue<AllowRomanceOption, bool>(false);
                options.SetValue<AllowFriendshipOption, bool>(false);
                options.SetValue<AllowPregnancyParticipationOption, bool>(false);
                options.SetValue<AllowPushWorkOption, bool>(false);
            }

            if (teenCreated)
            {
                // No Teen-Adult Flirting, No Teen Pregnancy
                teenOptions.SetValue<AllowPregnancyParticipationOption, bool>(false);
                teenOptions.SetValue<AllowMarriageOption, bool>(false);
                teenOptions.AddValue<DisallowCastePregnancyOption, CasteOptions>(youngAdultOptions);
                teenOptions.AddValue<DisallowCastePregnancyOption, CasteOptions>(adultOptions);
                teenOptions.AddValue<DisallowCastePregnancyOption, CasteOptions>(elderOptions);
                teenOptions.AddValue<DisallowCasteFlirtOption, CasteOptions>(youngAdultOptions);
                teenOptions.AddValue<DisallowCasteFlirtOption, CasteOptions>(adultOptions);
                teenOptions.AddValue<DisallowCasteFlirtOption, CasteOptions>(elderOptions);
            }

            if (youngAdultCreated)
            {
                if (currentVersion < 3)
                {
                    // Handled by Teen Caste
                    youngAdultOptions.RemoveValue<DisallowCastePregnancyOption, CasteOptions>(teenOptions);
                    youngAdultOptions.RemoveValue<DisallowCasteFlirtOption, CasteOptions>(teenOptions);
                }
            }

            if (adultCreated)
            {
                if (currentVersion < 3)
                {
                    // Handled by Teen Caste
                    adultOptions.RemoveValue<DisallowCastePregnancyOption, CasteOptions>(teenOptions);
                    adultOptions.RemoveValue<DisallowCasteFlirtOption, CasteOptions>(teenOptions);
                }
            }

            if (elderCreated)
            {
                if (currentVersion < 3)
                {
                    // Handled by Teen Caste
                    elderOptions.RemoveValue<DisallowCastePregnancyOption, CasteOptions>(teenOptions);
                    elderOptions.RemoveValue<DisallowCasteFlirtOption, CasteOptions>(teenOptions);
                }
            }

            // No Elder-Female Pregnancies
            options = Options.GetNewCasteOptions("ElderFemales", Common.Localize("Caste:ElderFemale"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Elder);
                options.SetValue<CasteGenderOption, List<CASAgeGenderFlags>>(null);
                options.AddValue<CasteGenderOption, CASAgeGenderFlags>(CASAgeGenderFlags.Female);

                options.SetValue<AllowPregnancyParticipationOption, bool>(false);
            }

            // No Simbot Pregnancies
            options = Options.GetNewCasteOptions("Simbots", Common.Localize("Caste:Simbot"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.SimBot);

                options.SetValue<AllowPregnancyParticipationOption, bool>(false);
                options.SetValue<AllowAgingOption, bool>(false);
            }

            // No Robot Pregnancies
            options = Options.GetNewCasteOptions("Plumbots", SimTypes.GetLocalizedName(SimType.Plumbot), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.Plumbot);
                options.AddValue<AllowCastePregnancyOption, CasteOptions>(options);
                options.SetValue<AllowAgingOption, bool>(false);
            }
            else if(options != null)            
            {
                if (currentVersion < 4)
                {
                    options.RemoveValue<AllowCasteFlirtOption, CasteOptions>(options);
                }
            }

            // No Vampire Aging
            options = Options.GetNewCasteOptions("Vampires", Common.Localize("Caste:Vampire"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.Vampire);
                options.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.YoungAdult);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Adult);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Elder);

                options.SetValue<AllowAgingOption, bool>(false);
            }

            // No Mummy Aging
            options = Options.GetNewCasteOptions("Mummies", Common.Localize("Caste:Mummy"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.Mummy);
                options.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.YoungAdult);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Adult);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Elder);

                options.SetValue<AllowAgingOption, bool>(false);
            }

            // No Ghost Aging
            options = Options.GetNewCasteOptions("Ghosts", Common.Localize("Caste:Ghost"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.PlayableGhost);
                options.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.YoungAdult);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Adult);
                options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Elder);

                options.SetValue<AllowAgingOption, bool>(false);
            }

            // No Horse Pushes
            options = Options.GetNewCasteOptions("Horses", Common.Localize("Caste:Horse"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteSpeciesOption, List<CASAgeGenderFlags>>(null);
                options.AddValue<CasteSpeciesOption, CASAgeGenderFlags>(CASAgeGenderFlags.Horse);

                options.SetValue<AllowPushAtDayOption, bool>(false);
                options.SetValue<AllowPushAtNightOption, bool>(false);
            }

            // Don't Push Actives
            options = Options.GetNewCasteOptions("ActiveFamily", Common.Localize("Caste:ActiveFamily"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.ActiveFamily);

                options.SetValue<AllowPushAtDayOption, bool>(false);
                options.SetValue<AllowPushAtNightOption, bool>(false);
                options.SetValue<AllowPushWorkOption, bool>(false);
                options.SetValue<AllowSummaryOption, bool>(false);
                options.SetValue<AllowPushPartyOption, bool>(false);
                options.SetValue<AllowAdoptionOption, bool>(false);
            }

            // Setting this just to be safe
            if (options != null)
            {
                if (currentVersion < 5)
                {
                    options.SetValue<AllowMoveFamilyOption, bool>(false);
                    options.SetValue<AllowMoveSoloOption, bool>(false);
                }
            }

            // For simplicity
            options = Options.GetNewCasteOptions("NonActiveFamily", Common.Localize("Caste:NonActiveFamily"), out created);
            if (created)
            {
                options.SetValue<CasteAutoOption, bool>(true);
                options.SetValue<CasteTypeOption, List<SimType>>(null);
                options.AddValue<CasteTypeOption, SimType>(SimType.NonActiveFamily);

                if (GameUtils.IsUniversityWorld())
                {
                    options.SetValue<AllowMarriageOption, bool>(false);
                    options.SetValue<AllowPregnancyParticipationOption, bool>(false);
                }
            }
            else if (options != null)
            {
                if (currentVersion < 1)
                {
                    if (GameUtils.IsUniversityWorld())
                    {
                        options.SetValue<AllowMarriageOption, bool>(false);
                        options.SetValue<AllowPregnancyParticipationOption, bool>(false);
                    }
                }
            }

            if (OnInitializeCastes != null)
            {
                OnInitializeCastes();
            }

            foreach (CasteOptions option in new List<CasteOptions>(Options.AllCastes))
            {
                string defaultName = option.DefaultName;
                if (string.IsNullOrEmpty(defaultName)) continue;

                if (!sCheckedCastes.ContainsKey(defaultName))
                {
                    Options.RemoveCaste(option);
                }
            }

            GetOption<CreatedCastesOption>().RemoveMissing(sCheckedCastes);

            Options.InvalidateCache();

            SetValue<Version, int>(5);
        }

        public List<CasteOptions> GetMatching(SimDescription sim, int netWorth)
        {
            List<CasteOptions> results = new List<CasteOptions>();

            SimData simData = GetData(sim);

            if (simData.GetValue<AllowCasteOption,bool>())
            {
                List<ulong> manualCastes = simData.GetValue<ManualCasteOption, List<ulong>>();

                foreach (CasteOptions caste in Options.AllCastes)
                {
                    if (manualCastes.Contains(caste.ID))
                    {
                        results.Add(caste);
                    }
                    else if (caste.GetValue<CasteAutoOption, bool>())
                    {
                        if (caste.Matches(sim, netWorth))
                        {
                            results.Add(caste);
                        }
                    }
                }

                results.Sort(CasteOptions.SortByPriority);
            }

            return results;
        }

        public class CreatedCastesOption : MultiListedManagerOptionItem<ManagerCaste, string>, IDebuggingOption
        {
            public CreatedCastesOption()
                : base(new string[0])
            {}

            public override string GetTitlePrefix()
            {
                return "CreatedCastes";
            }

            protected override bool PersistCreate(ref string defValue, string value)
            {
                defValue = value;
                return true;
            }

            public void RemoveMissing(Dictionary<string, bool> checkedCastes)
            {
                for(int i=Value.Count-1; i>=0 ; i--)
                {
                    if (!checkedCastes.ContainsKey(Value[i]))
                    {
                        Value.RemoveAt(i);
                    }
                }

                Persist();
            }

            protected override List<IGenericValueOption<string>> GetAllOptions()
            {
                List<IGenericValueOption<string>> results = new List<IGenericValueOption<string>>();

                foreach (CasteOptions option in Manager.Options.AllCastes)
                {
                    if (string.IsNullOrEmpty(option.DefaultName)) continue;

                    results.Add(new Item(this, option.DefaultName));
                }

                return results;
            }

            public class Item : BaseListItem<CreatedCastesOption>
            {
                public Item(CreatedCastesOption option, string value)
                    : base(option, value)
                { }
            }
        }


        public class Version : IntegerManagerOptionItem<Main>, IDebuggingOption
        {
            public Version()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "CasteVersion";
            }
        }

        public class Updates : AlertLevelOption<ManagerCaste>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        protected class DebugOption : DebugLevelOption<ManagerCaste>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class SpeedOption : SpeedBaseOption<ManagerCaste>
        {
            public SpeedOption()
                : base(500, false)
            { }
        }

        protected class DumpStatsOption : DumpStatsBaseOption<ManagerCaste>
        {
            public DumpStatsOption()
                : base(5)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerCaste>
        {
            public DumpScoringOption()
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerCaste>
        {
            public TicksPassedOption()
            { }
        }

        public class ResetCastesOption : BooleanManagerOptionItem<ManagerCaste>, IDebuggingOption
        {
            public ResetCastesOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ResetCastes";
            }

            public override string GetUIValue(bool pure)
            {
                return null;
            }

            protected override bool PrivatePerform()
            {
                foreach (CasteOptions caste in new List<CasteOptions>(Manager.Options.AllCastes))
                {
                    Manager.Options.RemoveCaste(caste);
                }

                Manager.SetValue<CreatedCastesOption, List<string>>(new List<string>());

                Manager.InitializeCastes();
                return true;
            }
        }
    }
}
