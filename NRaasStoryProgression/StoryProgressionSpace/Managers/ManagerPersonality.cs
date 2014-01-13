using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerPersonality : Manager
    {
        public enum LawfulnessType
        {
            Lawful,
            Unlawful,
            Neutral,
            Undefined,
        }

        protected List<SimPersonality> mPersonalities = new List<SimPersonality>();

        public ManagerPersonality(Main manager)
            : base(manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Personalities";
        }

        public bool HasPersonalities
        {
            get
            {
                return (mPersonalities.Count > 0);
            }
        }

        public IEnumerable<SimPersonality> AllPersonalities
        {
            get
            {
                return mPersonalities;
            }
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerPersonality>(this).Perform(initial);
        }

        public override void Startup(PersistentOptionBase options)
        {
            base.Startup(options);

            foreach (SimPersonality personality in PersonalityLookup.Personalities)
            {
                if (personality.Install (Main))
                {
                    mPersonalities.Add(personality);

                    personality.Startup(options);
                }
            }
        }

        public override void GetStoryPrefixes(List<string> prefixes)
        {
            foreach (SimPersonality personality in mPersonalities)
            {
                prefixes.Add(personality.UnlocalizedName);
            }
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (initialPass)
            {
                foreach (SimDescription sim in Sims.All)
                {
                    List<SimPersonality> clans = GetClanMembership(sim, false);

                    foreach (SimPersonality clan in clans)
                    {
                        if (clan.Me == null)
                        {
                            GetData(sim).RemoveClan(clan);

                            IncStat("Leader Reset");
                        }
                        else
                        {
                            clan.AddToClan(this, sim, false);
                        }
                    }
                }
            }

            if (fullUpdate)
            {
                foreach (SimDescription sim in Sims.All)
                {
                    List<SimPersonality> clans = GetClanMembership(sim, true);
                    if (clans == null) continue;

                    foreach (SimPersonality clan in clans)
                    {
                        clan.CheckRetention(sim);
                    }
                }
            }
            
            foreach (SimPersonality personality in mPersonalities)
            {
                personality.Update(fullUpdate, initialPass);
            }
            
            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public override void Shutdown()
        {
            base.Shutdown();

            foreach (SimPersonality personality in mPersonalities)
            {
                personality.Shutdown();
            }

            mPersonalities = null;
        }

        public void CheckRetention(SimDescription sim)
        {
            foreach (SimPersonality personality in mPersonalities)
            {
                personality.CheckRetention(sim);
            }
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (sim.LotHome == null)
            {
                stats.IncStat("Allow: Homeless");
                return false;
            }

            if (!settings.GetValue<AllowPersonalityOption, bool>())
            {
                stats.IncStat("Allow: User Denied");
                return false;
            }

            SimPersonality personality = stats as SimPersonality;
            if (personality == null)
            {
                IHasPersonality hasPersonality = stats as IHasPersonality;
                if (hasPersonality != null)
                {
                    personality = hasPersonality.Personality;
                }
                else
                {
                    Scenario scenario = stats as Scenario;
                    if (scenario != null)
                    {
                        personality = scenario.Manager as SimPersonality;
                    }
                }
            }

            if (personality == null)
            {
                Common.DebugStackLog(stats.GetType().ToString());
                //stats.IncStat("Allow: Incorrect Manager");
                //return false;
            }
            else if (settings.HasValue<DisallowPersonalityOption, SimPersonality>(personality))
            {
                stats.IncStat("Allow: Personality Denied");
                return false;
            }

            return true;
        }

        public bool Allow(IHasPersonality stats, Sim sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IHasPersonality stats, Sim sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }
        public bool Allow(IHasPersonality stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IHasPersonality stats, SimDescription sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            foreach (SimPersonality personality in mPersonalities)
            {
                personality.RemoveSim(sim);
            }
        }

        public override void GetOptions<TOption>(List<TOption> options, bool vbUI, AllowOptionDelegate<TOption> allow)
        {
            base.GetOptions(options, vbUI, allow);

            foreach (SimPersonality personality in mPersonalities)
            {
                if (vbUI)
                {
                    TOption listing = new Main.MasterListingOption(personality) as TOption;
                    if (listing != null)
                    {
                        options.Add(listing);
                    }
                }
                else
                {
                    personality.GetOptions(options, vbUI, allow);
                }
            }
        }

        public SimPersonality GetPersonality(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            SimPersonality personality = PersonalityLookup.GetPersonality(name);
            if (personality == null) return null;

            if (!personality.IsInstalled) return null;

            return personality;
        }

        public bool IsFriendly(Common.IStatGenerator stats, SimDescription simA, SimDescription simB)
        {
            List<SimPersonality> clans = GetClanMembership(simA, true);

            foreach (SimPersonality clan in clans)
            {
                if (clan.IsFriendly(simA, simB))
                {
                    stats.IncStat("Clan Friendly " + clan.UnlocalizedName);
                    return true;
                }
            }

            return false;
        }

        public LawfulnessType GetLawfulness(SimDescription sim)
        {
            int score = AddScoring("Lawfulness", sim);

            if (score > 0) return LawfulnessType.Lawful;

            if (score < 0) return LawfulnessType.Unlawful;

            bool unlawful = false;
            bool lawful = false;

            foreach (SimPersonality clan in GetClanMembership(sim, true))
            {
                switch (clan.Lawfulness)
                {
                    case LawfulnessType.Lawful:
                        lawful = true;
                        break;
                    case LawfulnessType.Unlawful:
                        unlawful = true;
                        break;
                }
            }

            if (unlawful)
            {
                return LawfulnessType.Unlawful;
            }
            else if (lawful)
            {
                return LawfulnessType.Lawful;
            }

            return LawfulnessType.Neutral;
        }

        public bool IsOpposing(Common.IStatGenerator stats, SimDescription simA, SimDescription simB, bool testSplit)
        {
            if ((testSplit) && (!GetValue<BrokenHomeScenario.SplitOpposingClanOption, bool>()))
            {
                return false;
            }

            LawfulnessType lawfulA = GetLawfulness(simA);
            LawfulnessType lawfulB = GetLawfulness(simB);

            switch (lawfulA)
            {
                case LawfulnessType.Lawful:
                    if (lawfulB == LawfulnessType.Unlawful)
                    {
                        stats.IncStat("Lawful Fail");
                        return true;
                    }
                    break;
                case LawfulnessType.Unlawful:
                    if (lawfulB == LawfulnessType.Lawful)
                    {
                        stats.IncStat("Lawful Fail");
                        return true;
                    }
                    break;
            }

            List<SimPersonality> clansA = GetClanMembership(simA, true);

            List<SimPersonality> clansB = GetClanMembership(simB, true);

            foreach (SimPersonality clanA in clansA)
            {
                foreach (SimPersonality clanB in clansB)
                {
                    if (clanA.IsOpposing(clanB))
                    {
                        stats.IncStat("Opposing: " + clanA.UnlocalizedName + ", " + clanB.UnlocalizedName);
                        return true;
                    }
                }
            }

            return false;
        }

        public SimDescription GetClanLeader(StoryProgressionObject manager)
        {
            SimPersonality clan = manager as SimPersonality;
            if (clan == null) return null;

            return clan.Me;
        }

        public List<SimPersonality> GetClanMembership(SimDescription sim, bool includeLeadership)
        {
            List<SimPersonality> personalities = new List<SimPersonality>();

            if (sim != null)
            {
                if (includeLeadership)
                {
                    personalities.AddRange(GetClanLeadership(sim));
                }

                SimData simData = GetData(sim);

                List<string> clans = new List<string>(simData.Clans);
                foreach (string clan in clans)
                {
                    SimPersonality personality = GetPersonality(clan);
                    if (personality == null)
                    {
                        simData.RemoveClan(clan);
                    }
                    else
                    {
                        personalities.Add(personality);
                    }
                }
            }

            return personalities;
        }

        public List<SimPersonality> GetClanLeadership(SimDescription sim)
        {
            List<SimPersonality> clans = new List<SimPersonality>();

            if (sim != null)
            {
                foreach (SimPersonality personality in mPersonalities)
                {
                    if (personality.Me == sim)
                    {
                        clans.Add(personality);
                    }
                }
            }

            return clans;
        }

        public List<SimDescription> GetClanAlliesFor(SimDescription sim)
        {
            Dictionary<SimDescription, bool> list = new Dictionary<SimDescription, bool>();

            List<SimPersonality> clans = GetClanMembership(sim, true);

            foreach (SimPersonality clan in clans)
            {
                foreach (SimDescription ally in clan.GetAlliesFor(sim))
                {
                    list[ally] = true;
                }
            }

            return new List<SimDescription> (list.Keys);
        }

        public class Updates : AlertLevelOption<ManagerPersonality>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                if (AcceptCancelDialog.Show(Localize("PersonalityPrompt", new object[] { DisplayValue })))
                {
                    foreach (SimPersonality personality in Manager.mPersonalities)
                    {
                        personality.SetAlertLevel(Value);
                    }
                }

                return true;
            }
        }

        protected class DebugOption : DebugLevelOption<ManagerPersonality>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class DumpStatsOption : DumpStatsBaseOption<ManagerPersonality>
        {
            public DumpStatsOption()
                : base(1)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerPersonality>
        {
            public DumpScoringOption()
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerPersonality>
        {
            public TicksPassedOption()
            { }
        }

        protected class SpeedOption : SpeedBaseOption<ManagerPersonality>
        {
            public SpeedOption()
                : base (1000, false)
            { }
        }

        public class IncreasedChancePerCycleOption : IntegerManagerOptionItem<ManagerPersonality>
        {
            public IncreasedChancePerCycleOption()
                : base(15)
            { }

            public override string GetTitlePrefix()
            {
                return "IncreasedChancePerCycle";
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                foreach (SimPersonality personality in Manager.mPersonalities)
                {
                    personality.SetIncreasedChangePerCycle(Value);
                }

                return true;
            }
        }

        public class MinCelebrityOption : IntegerManagerOptionItem<ManagerPersonality>
        {
            public MinCelebrityOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "MinCelebrityPerScenario";
            }
        }
    }
}
