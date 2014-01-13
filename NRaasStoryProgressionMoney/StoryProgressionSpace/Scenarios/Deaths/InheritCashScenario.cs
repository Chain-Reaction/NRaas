using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class InheritCashScenario : SimScenario, IFormattedStoryScenario
    {
        Dictionary<SimDescription, int> mInheritance = null;

        public InheritCashScenario(SimDescription sim)
            : base (sim)
        {
            mInheritance = new Dictionary<SimDescription, int>();
        }
        protected InheritCashScenario(InheritCashScenario scenario)
            : base (scenario)
        {
            mInheritance = new Dictionary<SimDescription,int>(scenario.mInheritance);
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Inheritance";
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Deaths;
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Genealogy == null)
            {
                IncStat("No Genealogy");
                return false;
            }

            Household house = GetData<StoredNetWorthSimData>(sim).Household;
            if (house == null)
            {
                IncStat("No House");
                return false;
            }
            else if (SimTypes.IsSpecial(house))
            {
                IncStat("Special");
                return false;
            }
            else
            {
                int count = HouseholdsEx.NumHumans(house);
                if ((count > 1) || ((count == 1) && (house.SimDescriptions[0] != sim))) // Humans
                {
                    IncStat("People Remain");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Household house = GetData<StoredNetWorthSimData>(Sim).Household;

            int iTotalWorth = GetData<StoredNetWorthSimData>(Sim).NetWorth;
            iTotalWorth -= GetValue<DebtOption,int>(house);

            SetValue<DebtOption,int>(house, 0);

            if (iTotalWorth <= 0)
            {
                IncStat("Inherit Cash: Poor");
                return false;
            }

            Dictionary<SimDescription, float> inheritors = Deaths.GetInheritors(Sim, GetValue<InheritorsOption,ManagerDeath.Inheritors>(), true);

            float fTotal = 0f;
            foreach (KeyValuePair<SimDescription, float> fraction in inheritors)
            {
                fTotal += fraction.Value;
            }

            AddStat("Worth", iTotalWorth);
            AddStat("Inheritors", inheritors.Count);
            AddStat("Split", fTotal);

            bool simMatches = (Deaths.MatchesAlertLevel(Sim)) || (Money.MatchesAlertLevel(Sim));

            foreach (KeyValuePair<SimDescription, float> inheritor in inheritors)
            {
                int iInheritance = (int)(iTotalWorth * (inheritor.Value / fTotal));

                if (iInheritance <= 0) continue;

                SimDescription child = inheritor.Key;
                if (child.Household == null) continue;

                Money.AdjustFunds(child, "Inheritance", iInheritance);

                if ((simMatches) || (Deaths.MatchesAlertLevel(child)) || (Money.MatchesAlertLevel(child)))
                {
                    mInheritance.Add(child, iInheritance);
                }
            }

            return (mInheritance.Count > 0);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Deaths;
            }

            if (mInheritance.Count > 5) return null;

            foreach (KeyValuePair<SimDescription, int> child in mInheritance)
            {
                base.PrintStory(manager, name, new object[] { child.Key, Sim, child.Value }, extended, logging);
            }

            return null;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mInheritance.Count <= 5) return null;

            if (manager == null)
            {
                manager = Deaths;
            }

            text = manager.Localize(GetTitlePrefix(PrefixType.Summary) + "Summary", Sim.IsFemale, new object[] { Sim });

            List<object> paramList = new List<object>();

            foreach (KeyValuePair<SimDescription, int> child in mInheritance)
            {
                text += manager.Localize(GetTitlePrefix(PrefixType.Summary) + "Element", child.Key.IsFemale, new object[] { child.Key, child.Value });

                paramList.Add(child.Key);
            }

            return base.PrintFormattedStory(manager, text, summaryKey, paramList.ToArray(), extended, logging);
        }

        public override Scenario Clone()
        {
            return new InheritCashScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerDeath>
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerDeath main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    DiedScenario.OnInheritCashScenario += new UpdateDelegate(OnRun);
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "InheritCash";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new InheritCashScenario(s.Sim), ScenarioResult.Start);
            }
        }

        public class InheritorsOption : EnumManagerOptionItem<ManagerDeath, ManagerDeath.Inheritors>
        {
            public InheritorsOption()
                : base(ManagerDeath.Inheritors.Children | ManagerDeath.Inheritors.Friends | ManagerDeath.Inheritors.Relatives)
            { }

            protected override int NumSelectable
            {
                get { return 0; }
            }

            public override string GetTitlePrefix()
            {
                return "Inheritors";
            }

            protected override string GetLocalizationValueKey()
            {
                return "Inheritors";
            }

            protected override string GetLocalizationUIKey()
            {
                return null;
            }

            protected override ManagerDeath.Inheritors Convert(int value)
            {
                return (ManagerDeath.Inheritors)value;
            }

            protected override ManagerDeath.Inheritors Combine(ManagerDeath.Inheritors original, ManagerDeath.Inheritors add, out bool same)
            {
                ManagerDeath.Inheritors result = original | add;

                same = (result == original);

                return result;
            }
        }
    }
}
