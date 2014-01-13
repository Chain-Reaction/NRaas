using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class NoHomeScenario : DualSimScenario 
    {
        public NoHomeScenario (SimDescription sim, SimDescription target)
            : base (sim, target)
        { }
        protected NoHomeScenario(NoHomeScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "NoHome";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueReportChance
        {
            get { return 50; }
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

        protected override bool CommonAllow(SimDescription sim)
        {
            if (GetOption<Option>().IsPreviousNoHome(sim))
            {
                IncStat("Previous");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Household == Target.Household)
            {
                IncStat("Same");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            GetOption<Option> ().AddPreviousNoHome (Sim);
            GetOption<Option> ().AddPreviousNoHome (Target);
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Households;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new NoHomeScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerLot>, IDebuggingOption
        {
            Dictionary<ulong,SimID> mPreviousNoHome = new Dictionary<ulong,SimID>();

            public Option()
                : base(true)
            { }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                if (fullUpdate)
                {
                    mPreviousNoHome.Clear();
                }

 	            base.PrivateUpdate(fullUpdate, initialPass);
            }

            public override string GetTitlePrefix()
            {
                return "ShowNoHome";
            }

            public bool IsPreviousNoHome (SimDescription sim)
            {
                return mPreviousNoHome.ContainsKey(sim.SimDescriptionId);
            }

            public void AddPreviousNoHome (SimDescription sim)
            {
                if (IsPreviousNoHome (sim)) return;

                mPreviousNoHome.Add(sim.SimDescriptionId, new SimID(sim));
            }
        }
    }
}
