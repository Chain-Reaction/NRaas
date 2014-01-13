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
    public class HouseChangedScenario : HouseChangedBaseScenario
    {
        public HouseChangedScenario()
        { }
        protected HouseChangedScenario(HouseChangedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HouseChanged";
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (OldHouse != null)
            {
                foreach (SimDescription sim in CommonSpace.Helpers.Households.All(OldHouse))
                {
                    foreach (ICarpoolSchedulingSimData data in GetData(sim).GetList<ICarpoolSchedulingSimData>())
                    {
                        data.Reset();
                    }
                }
            }

            if (NewHouse != null)
            {
                foreach (SimDescription sim in CommonSpace.Helpers.Households.All(NewHouse))
                {
                    Add(frame, new UpdateCareerScenario(sim, false), ScenarioResult.Start);
                }
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new HouseChangedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, HouseChangedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CareerHouseChanged";
            }
        }
    }
}
