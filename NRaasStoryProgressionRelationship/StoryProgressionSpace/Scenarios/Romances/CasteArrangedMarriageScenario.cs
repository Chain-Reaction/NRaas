using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public class CasteArrangedMarriageScenario : RelationshipScenario
    {
        public CasteArrangedMarriageScenario()
            : base(1)
        { }
        protected CasteArrangedMarriageScenario(CasteArrangedMarriageScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "CasteArrangedMarriage";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool TestOpposing
        {
            get { return true; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        public override bool IsRomantic
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (SimTypes.IsDead(sim))
            {
                IncStat("Dead");
                return false;
            }
            else if (sim.IsMarried)
            {
                IncStat("Already Married");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (GetValue<ArrangedMarriageOption, ulong>(sim) != 0)
            {
                IncStat("Already Arranged");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Partner != null)
            {
                Relationship relation = ManagerSim.GetRelationship(sim, sim.Partner);
                if (relation != null)
                {
                    if (relation.LTR.CurrentLTR == LongTermRelationshipTypes.Fiancee)
                    {
                        IncStat("Fiance");
                        return false;
                    }
                }
            }
            
            if (!HasAnyValue<ArrangedMarriageCasteOption,CasteOptions>(sim))
            {
                IncStat("Caste Fail");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (!Romances.Allow(this, Sim, target))
            {
                return false;
            }
            if (MarriageBaseScenario.TestForChildBlock(Sim, target))
            {
                IncStat("Child Are Married");
                return false;
            }
            else
            {
                SimData targetData = GetData(target);

                if (targetData.HasAnyValue<ArrangedMarriageCasteOption, CasteOptions>())
                {
                    if (!targetData.Contains<ArrangedMarriageCasteOption>(GetData(Sim).Castes))
                    {
                        IncStat("Target Caste Fail");
                        return false;
                    }
                }
            }

            return base.TargetAllow(target);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.TeensAndAdults;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            SimData simData = GetData(sim);

            List<SimDescription> results = new List<SimDescription>();

            foreach (SimDescription target in Sims.TeensAndAdults)
            {
                if (simData.Contains<ArrangedMarriageCasteOption>(GetData(target).Castes))
                {
                    results.Add(target);
                }
            }

            return results;
        }

        protected override bool TargetSort(SimDescription sim, ref List<SimDescription> targets)
        {
            SimScoringList scoring = new SimScoringList("NewFlirt");

            foreach (SimDescription target in targets)
            {
                scoring.Add(this, "NewFlirt", target, sim);
            }

            targets = scoring.GetBestByPercent(100);
            return true;
        }

        protected void HandleDowry(SimDescription a, SimDescription b)
        {
            int min = GetValue<ArrangedMarriageDowryMinOption, int>(a);
            int max = GetValue<ArrangedMarriageDowryMaxOption, int>(a);

            if (max < min)
            {
                max = min;
            }
          
            int dowry = RandomUtil.GetInt(min, max);

            if (GetValue<DowryOnlyToPoorerOption, bool>())
            {
                if (dowry > 0)
                {
                    if (GetValue<NetWorthOption, int>(a.Household) < GetValue<NetWorthOption, int>(b.Household))
                    {
                        IncStat("Dowry Poorer");
                        return;
                    }
                }
                else
                {
                    if (GetValue<NetWorthOption, int>(a.Household) > GetValue<NetWorthOption, int>(b.Household))
                    {
                        IncStat("Dowry Poorer");
                        return;
                    }
                }
            }

            Money.AdjustFunds(a, "ArrangedDowry", -dowry);
            Money.AdjustFunds(b, "ArrangedDowry", dowry);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SetValue<ArrangedMarriageOption, ulong>(Sim, Target.SimDescriptionId);
            SetValue<ArrangedMarriageOption, ulong>(Target, Sim.SimDescriptionId);

            HandleDowry(Sim, Target);
            HandleDowry(Target, Sim);

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new CasteArrangedMarriageScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerRomance, CasteArrangedMarriageScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CasteArrangedMarriage";
            }
        }

        public class DowryOnlyToPoorerOption : BooleanScenarioOptionItem<ManagerRomance, CasteArrangedMarriageScenario>
        {
            public DowryOnlyToPoorerOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ArrangedDowryOnlyToPoorerHousehold";
            }
        }
    }
}
