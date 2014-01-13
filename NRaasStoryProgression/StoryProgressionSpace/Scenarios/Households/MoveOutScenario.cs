using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios;
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
    public abstract class MoveOutScenario : SimScenario
    {
        SimDescription mStay = null;

        protected MoveOutScenario (SimDescription sim, SimDescription stay)
            : base (sim)
        {
            mStay = stay;
        }
        protected MoveOutScenario(MoveOutScenario scenario)
            : base (scenario)
        {
            mStay = scenario.mStay;
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected abstract HouseholdBreakdown.ChildrenMove ChildMove
        {
            get;
        }

        protected abstract MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going);

        protected abstract ScoredMoveInScenario GetMoveInScenario(List<SimDescription> going);

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Households.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if ((mStay != null) && (sim.Household != mStay.Household))
            {
                IncStat("Diff Home");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            HouseholdBreakdown breakdown = new HouseholdBreakdown(Manager, this, UnlocalizedName, Sim, ChildMove, false);

            if (!breakdown.SimGoing)
            {
                IncStat("Staying");
                return false;
            }
            else if ((breakdown.NoneStaying) && (Sim.LotHome != null))
            {
                IncStat("None Staying");
                return false;
            }
            else if (breakdown.NoneGoing)
            {
                IncStat("None Going");
                return false;
            }
            else if ((mStay != null) && (breakdown.Going.Contains(mStay)))
            {
                IncStat("Both Going");
                return false;
            }

            if (Households.AllowSoloMove (Sim))
            {
                Add(frame, GetMoveInLotScenario(breakdown.Going), ScenarioResult.Failure);
            }

            Add(frame, GetMoveInScenario(breakdown.Going), ScenarioResult.Failure);
            return false;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Households;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
