using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class InspectionFailScenario : HouseholdScenario
    {
        public InspectionFailScenario(Household house)
            : base(house)
        { }
        protected InspectionFailScenario(InspectionFailScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "InspectionFail";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected static ManagerLot.CheckResult OnLotPriceCheck(Common.IStatGenerator stats, Lot lot, int currentLotCost, int availableFunds)
        {
            if (availableFunds < StoryProgression.Main.Lots.GetLotCost(lot))
            {
                stats.IncStat("Find Lot: Too expensive");
                return ManagerLot.CheckResult.Failure;
            }

            return ManagerLot.CheckResult.Success;
        }

        protected override bool Allow(Household house)
        {
            if (Lots.FindLot(this, HouseholdsEx.All(house), 0, ManagerLot.FindLotFlags.Inspect, OnLotPriceCheck) == null)
            {
                IncStat("No Homes");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SetValue<InspectedOption,bool>(House, true);

            if ((Lots.MatchesAlertLevel(House.AllSimDescriptions)) && (GetValue<PromptOption, bool>()))
            {
                if (AcceptCancelDialog.Show(Common.Localize("InspectionFail:Prompt", false, new object[] { House.Name, House.LotHome.Address })))
                {
                    if (CameraController.IsMapViewModeEnabled())
                    {
                        Sims3.Gameplay.Core.Camera.ToggleMapView();
                    }

                    Camera.FocusOnLot(House.LotHome.LotId, 0f);
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new InspectionFailScenario(this);
        }

        public class PromptOption : BooleanManagerOptionItem<ManagerLot>, ManagerLot.IInspectionOption
        {
            public PromptOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "PromptOnInspectionFail";
            }
        }
    }
}
