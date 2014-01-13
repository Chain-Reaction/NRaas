using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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
    public class InterloperMoveScenario : HouseholdScenario
    {
        public InterloperMoveScenario()
        { }
        protected InterloperMoveScenario(InterloperMoveScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "InterloperMove";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (!Households.Allow(this, house, 0))
            {
                IncStat("User Denied");
                return false;
            }
            else if (SimTypes.HeadOfFamily(House) == null)
            {
                IncStat("No Head");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            ScoringList<SimDescription> scoring = new ScoringList<SimDescription>();

            SimDescription head = SimTypes.HeadOfFamily(House);

            foreach (SimDescription sim in House.AllSimDescriptions)
            {
                if ((sim == head) || (sim.Partner == head)) continue;

                // Don't move sims that can't move out
                if (!Households.AllowSoloMove(sim)) continue;

                // Don't move sims related to the head of family
                if (Flirts.IsCloselyRelated(sim, head)) continue;

                // Don't move sims that don't have partners
                if (sim.Partner == null) continue;

                if (!House.AllSimDescriptions.Contains(sim.Partner)) continue;

                if (Flirts.IsCloselyRelated(sim.Partner, head)) continue;

                scoring.Add(sim, AddScoring("FindOwnHome", sim.Partner, sim));
            }

            ICollection<SimDescription> best = scoring.GetBestByPercent(100);
            if ((best == null) || (best.Count == 0))
            {
                IncStat("No Choices");
                return false;
            }
            else
            {
                foreach (SimDescription sim in best)
                {
                    HouseholdBreakdown breakdown = new HouseholdBreakdown(Manager, this, UnlocalizedName, sim, HouseholdBreakdown.ChildrenMove.Scoring, false);

                    Add(frame, new StandardMoveInLotScenario(breakdown, 0), ScenarioResult.Failure);
                    Add(frame, new PostScenario(sim), ScenarioResult.Success);
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new InterloperMoveScenario(this);
        }

        public class PostScenario : SimScenario
        {
            public PostScenario(SimDescription sim)
                : base(sim)
            { }
            protected PostScenario(PostScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "InterloperMovePost";
                }
                else
                {
                    return "InterloperMove";
                }
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
                return null;
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return true;
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (parameters == null)
                {
                    parameters = new object[] { Sim, Sim.Partner };
                }

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new PostScenario(this);
            }
        }

        public class Option : BooleanScenarioOptionItem<ManagerHousehold, InterloperMoveScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "InterloperMove";
            }
        }
    }
}
