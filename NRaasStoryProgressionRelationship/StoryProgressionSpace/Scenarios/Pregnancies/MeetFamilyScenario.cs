using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class MeetFamilyScenario : DualSimScenario
    {
        public enum FamilyLevel : int
        {
            None = 0x00,
            Parents = 0x01,
            Siblings = 0x02,
            Grandparents = 0x04,
            Other = 0x08,
        }

        public MeetFamilyScenario(SimDescription sim)
            : base(sim)
        { }
        protected MeetFamilyScenario(MeetFamilyScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "MeetFamily";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 100; }
        }

        protected override int TargetContinueChance
        {
            get { return 100; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (HasValue<LevelOption, FamilyLevel>(FamilyLevel.Other))
            {
                return Sims.All;
            }

            List<SimDescription> sims = new List<SimDescription>();

            if (HasValue<LevelOption, FamilyLevel>(FamilyLevel.Parents))
            {
                sims.AddRange(Relationships.GetParents(sim));
            }
            
            if (HasValue<LevelOption, FamilyLevel>(FamilyLevel.Siblings))
            {
                sims.AddRange(Relationships.GetSiblings(sim));
            }

            if (HasValue<LevelOption, FamilyLevel>(FamilyLevel.Grandparents))
            {
                foreach (SimDescription parent in Relationships.GetParents(sim))
                {
                    sims.AddRange(Relationships.GetParents(parent));
                }
            }

            return sims;
        }

        protected override bool Allow()
        {
            if (GetValue<OptionV2,int>() <= 0) return false;

            return base.Allow();
        }      

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.Baby)
            {
                IncStat("Not Baby");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (HasValue<LevelOption, FamilyLevel>(FamilyLevel.Other))
            {
                if (!Relationships.IsCloselyRelated(Sim, target, false))
                {
                    IncStat("Not Related");
                    return false;
                }
            }

            foreach (SimDescription parent in Relationships.GetParents(Sim))
            {
                if (ManagerSim.GetLTR(Sim, Target) < 0)
                {
                    IncStat("Parent Relation Fail");
                    return false;
                }
            }

            return base.TargetAllow(target);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Relationship relation = ManagerSim.GetRelationship(Sim, Target);
            if (relation == null)
            {
                IncStat("No Relation");
                return false;
            }
            
            relation.MakeAcquaintances();

            int liking = GetValue<OptionV2,int>();
            if (relation.LTR.Liking < liking)
            {
                relation.LTR.SetLiking (liking);
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new MeetFamilyScenario(this);
        }

        public class OptionV2 : IntegerManagerOptionItem<ManagerPregnancy>
        {
            public OptionV2()
                : base(0)
            { }

            public override bool Install(ManagerPregnancy main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    BirthScenario.OnBirthScenario += OnInstall;
                }
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "MeetFamily";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public static void OnInstall(Scenario scenario, ScenarioFrame frame)
            {
                BirthScenario s = scenario as BirthScenario;
                if (s == null) return;

                foreach (SimDescription baby in s.Babies)
                {
                    scenario.Add(frame, new MeetFamilyScenario(baby), ScenarioResult.Start);
                }
            }
        }

        public class LevelOption : MultiEnumManagerOptionItem<ManagerPregnancy, FamilyLevel>
        {
            public LevelOption()
                : base (new FamilyLevel[] { FamilyLevel.Parents, FamilyLevel.Siblings, FamilyLevel.Grandparents, FamilyLevel.Other })
            { }

            public override string GetTitlePrefix()
            {
                return "FamilyLevel";
            }

            protected override bool Allow(FamilyLevel value)
            {
                if (value == FamilyLevel.None) return false;

                return base.Allow(value);
            }
        }
    }
}
