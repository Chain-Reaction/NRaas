using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class OnlyOneMayLeadScenario : PromotionBaseScenario<int>
    {
        static PromotionDataSet sData = new PromotionDataSet();

        SimDescription mLoser;

        public OnlyOneMayLeadScenario(SimDescription sim)
            : base (sim)
        { }
        protected OnlyOneMayLeadScenario(OnlyOneMayLeadScenario scenario)
            : base (scenario)
        {
            mLoser = scenario.mLoser;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "OnlyOneMayLead";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected List<SimDescription> GetContenders(Career newCareer)
        {
            List<SimDescription> sims = new List<SimDescription>();

            foreach (SimDescription sim in Manager.Careers.Employed)
            {
                if (sim == newCareer.OwnerDescription) continue;

                Career career = sim.Occupation as Career;
                if (career == null) continue;

                if (career.CareerLoc != newCareer.CareerLoc) continue;

                if (career.Guid != newCareer.Guid) continue;

                if (career.CareerLevel != newCareer.CareerLevel) continue;

                if (career.CurLevelBranchName != newCareer.CurLevelBranchName) continue;

                sims.Add(sim);
            }

            return sims;
        }

        protected static float GetBossRelationship(SimDescription sim)
        {
            if (sim == null) return float.MinValue;

            if (sim.Occupation == null) return float.MinValue;

            SimDescription boss = sim.Occupation.Boss;

            Relationship relation = Relationship.Get(sim, boss, false);
            if (relation == null) return 0;

            return relation.CurrentLTRLiking;
        }

        protected static int Populate(XmlDbRow row)
        {
            int totalAllowed = row.GetInt("TotalAllowed");
            if (totalAllowed <= 0)
            {
                totalAllowed = 1;
            }

            return totalAllowed;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int totalAllowed = sData.GetDataForLevel(Sim);
            if (totalAllowed <= 0) return false;

            Career career = Sim.Occupation as Career;
            List<SimDescription> contenders = GetContenders(career);

            if (contenders.Count >= totalAllowed)
            {
                AddStat("Contenders", contenders.Count);
                AddStat("Allowed", totalAllowed);

                int score = AddScoring("New Sim", ScoringLookup.GetScore ("Ambition", Sim));

                List<SimDescription> losers = new List<SimDescription>();

                bool allowActive = GetValue<AllowActiveOption, bool>();

                foreach (SimDescription contender in contenders)
                {
                    if (!SimTypes.IsSelectable(Sim))
                    {
                        if ((!allowActive) && (SimTypes.IsSelectable(contender)))
                        {
                            IncStat("Active Denied");
                            continue;
                        }
                    }

                    bool checkOther = false;

                    int contenderScore = AddScoring("Contender", ScoringLookup.GetScore ("Ambition", contender));
                    if (contenderScore < score)
                    {
                        if ((SimTypes.IsSelectable(Sim)) && (SimTypes.IsSelectable(contender)))
                        {
                            IncStat("Both Selectable");
                            checkOther = true;
                        }
                    }
                    else if (contenderScore == score)
                    {
                        IncStat("Equal Score");
                        checkOther = true;
                    }
                    else
                    {
                        continue;
                    }

                    if (checkOther)
                    {
                        int contenderRelation = GetBossRelationship(contender).CompareTo(GetBossRelationship(Sim));

                        if (contenderRelation > 0)
                        {
                            IncStat("Boss Relation");
                            continue;
                        }
                        else if ((contenderRelation == 0) && (contender.Occupation.PayPerHourOrStipend > career.PayPerHourOrStipend))
                        {
                            IncStat("Pay Worse");
                            continue;
                        }
                    }

                    losers.Add(contender);
                }

                if (losers.Count == 0)
                {
                    if ((!allowActive) && (SimTypes.IsSelectable(Sim)))
                    {
                        losers.AddRange(contenders);
                    }
                    else
                    {
                        losers.Add(Sim);
                    }
                }

                if (losers.Count == 0) return false;

                mLoser = RandomUtil.GetRandomObjectFromList(losers);

                mLoser.Occupation.DemoteSim();

                Career loserCareer = mLoser.Occupation as Career;
                if (loserCareer != null)
                {
                    loserCareer.GiveRaise(false);
                }
                return true;
            }

            return false;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mLoser != Sim)
            {
                name += "Success";

                parameters = new object[] { Sim, mLoser };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new OnlyOneMayLeadScenario(this);
        }

        public class AllowActiveOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public AllowActiveOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "OnlyOneMayLeadActive";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class Option : BooleanManagerOptionItem<ManagerCareer>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "OnlyOneMayLead";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new OnlyOneMayLeadScenario(s.Sim), ScenarioResult.Failure);
            }

            public override bool Install(ManagerCareer main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    PromotedScenario.OnOnlyOneMayLeadScenario += OnRun;
                    DemotedScenario.OnOnlyOneMayLeadScenario += OnRun;

                    sData.Parse("NRaas.StoryProgression.OnlyOneMayLead", Populate);
                }

                return true;
            }
        }
    }
}
