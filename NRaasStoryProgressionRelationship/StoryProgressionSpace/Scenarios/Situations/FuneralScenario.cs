using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class FuneralScenario : GatheringScenario
    {
        SimDescription mDeadSim = null;

        protected FuneralScenario(SimDescription host, SimDescription deadSim)
            : base(host, host.LotHome)
        {
            mDeadSim = deadSim;
        }
        protected FuneralScenario(FuneralScenario scenario)
            : base (scenario)
        { 
            mDeadSim = scenario.mDeadSim;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "FuneralGathering";
        }

        protected override OutfitCategories PartyAttire
        {
            get { return OutfitCategories.Formalwear; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        public static FuneralScenario Create(ManagerSituation manager, SimDescription deadSim)
        {
            SimDescription host = null;

            List<SimDescription> hosts = new List<SimDescription>();
            foreach (SimDescription parent in Relationships.GetParents(deadSim))
            {
                hosts.Add(parent);
            }

            foreach (SimDescription child in Relationships.GetChildren(deadSim))
            {
                hosts.Add(child);
            }

            foreach (SimDescription sibling in Relationships.GetSiblings(deadSim))
            {
                hosts.Add(sibling);
            }

            foreach (Relationship relation in Relationship.GetRelationships(deadSim))
            {
                if (relation.LTR.Liking <= 50) continue;

                hosts.Add(relation.GetOtherSimDescription(deadSim));
            }

            foreach (SimDescription sim in hosts)
            {
                if (sim == null) continue;

                if (sim.LotHome == null) continue;

                if (manager.IsBusy(manager, sim, true)) continue;

                host = sim;
                break;
            }

            if (host == null) return null;

            return new FuneralScenario(host, deadSim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!SimTypes.IsDead(sim))
            {
                IncStat("Not Dead");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((!Relationships.IsCloselyRelated(mDeadSim, Target, false)) || (ManagerSim.GetLTR(mDeadSim, Target) < 50))
            {
                IncStat("Low LTR");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Guests.Count < TargetMinimum) return false;

            new FuneralSituation(Lot, mDeadSim, Sim.CreatedSim, Guests, PartyAttire, SimClock.CurrentTime());
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, mDeadSim, Lot };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new FuneralScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGatheringOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerSituation main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    DiedScenario.OnFuneralScenario += new UpdateDelegate(OnRun);
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "FuneralGathering";
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, FuneralScenario.Create(scenario.Situations, s.Sim), ScenarioResult.Start);
            }
        }
    }
}
