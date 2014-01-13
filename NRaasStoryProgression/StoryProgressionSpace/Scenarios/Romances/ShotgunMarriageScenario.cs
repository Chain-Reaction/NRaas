using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public class ShotgunMarriageScenario : MarriageScenario 
    {
        public ShotgunMarriageScenario(SimDescription sim, SimDescription target)
            : base (sim, target)
        { }
        protected ShotgunMarriageScenario(ShotgunMarriageScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "ShotgunMarriage";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Partner == null) return null;

            List<SimDescription> list = new List<SimDescription>();
            list.Add(sim.Partner);
            return list;
        }

        protected override bool Score()
        {
            if (GetValue<AlwaysMarryOption,bool>()) return true;

            return base.Score();
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((!Sim.IsPregnant) && (!Target.IsPregnant))
            {
                IncStat("Not Pregnant");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Romances;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new ShotgunMarriageScenario(this);
        }

        public class AlwaysMarryOption : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public AlwaysMarryOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AlwaysMarryUnexpected";
            }
        }
    }
}
