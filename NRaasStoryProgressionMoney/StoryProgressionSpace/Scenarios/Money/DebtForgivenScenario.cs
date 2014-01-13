using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class DebtForgivenScenario : HouseholdScenario
    {
        public DebtForgivenScenario(Household house)
            : base (house)
        { }
        protected DebtForgivenScenario(DebtForgivenScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "DebtForgiven";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Money.AdjustFunds(House, "PayDebt", -House.FamilyFunds);

            SetValue<DebtOption,int>(House, 0);
            return true;
        }

        protected override bool Push()
        {
            SimDescription head = SimTypes.HeadOfFamily(House);

            return Situations.PushToRabbitHole(this, head, RabbitHoleType.CityHall, false, false);
        }

        public override Scenario Clone()
        {
            return new DebtForgivenScenario(this);
        }
    }
}
