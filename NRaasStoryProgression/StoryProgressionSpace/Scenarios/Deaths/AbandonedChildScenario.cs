using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class AbandonedChildScenario : HouseholdScenario
    {
        public AbandonedChildScenario(Household house)
            : base (house)
        { }
        protected AbandonedChildScenario(AbandonedChildScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "AbandonedChild";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow(Household house)
        {
            if (SimTypes.IsService(house))
            {
                IncStat("Service");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Deaths;
            }

            bool child = false;
            foreach (SimDescription sim in HouseholdsEx.All(House))
            {
                if (!Households.AllowGuardian(sim))
                {
                    child = true;
                    break;
                }
            }

            if (child)
            {
                name = "AbandonedChild";
            }
            else
            {
                name = "ChildMoved";
            }

            if (parameters == null)
            {
                SimDescription head = SimTypes.HeadOfFamily(House);
                if (head == null) return null;

                parameters = new object[] { head };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new AbandonedChildScenario(this);
        }
    }
}
