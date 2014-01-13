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
using Sims3.Gameplay.DreamsAndPromises;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
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
            int autoToastDreams = GetValue<AutoToastDreamsOption, int>();
            if (autoToastDreams > 0)
            {
                if ((OldHouse != null) && (NewHouse != null))
                {
                    foreach (SimDescription sim in CommonSpace.Helpers.Households.All(NewHouse))
                    {
                        if (sim.CreatedSim == null) continue;

                        DreamsAndPromisesManager dnpManager = sim.CreatedSim.DreamsAndPromisesManager;
                        if (dnpManager == null) continue;

                        if (dnpManager.mDreamNodes == null) continue;

                        int numPromised = dnpManager.NumPromisedDreams();

                        if (sim.LifetimeWishNode != null)
                        {
                            numPromised--;
                        }

                        if (numPromised > 0) continue;

                        IncStat("Dropping Dreams");

                        int remainingToastDreams = autoToastDreams;
                        while ((remainingToastDreams > 0) &&
                               (dnpManager.mDreamNodes.Count > 0))
                        {
                            dnpManager.CancelDream(dnpManager.mDreamNodes[0]);

                            remainingToastDreams--;

                            IncStat("Dropped Starting Dream");
                        }
                    }
                }
            }

            UpdateAgingScenario updateAging = new UpdateAgingScenario();
            updateAging.Manager = Sims;

            updateAging.Perform(CommonSpace.Helpers.Households.All(NewHouse), frame);
            updateAging.Perform(CommonSpace.Helpers.Households.All(OldHouse), frame);
            return false;
        }

        public override Scenario Clone()
        {
            return new HouseChangedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim, HouseChangedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SimHouseChanged";
            }
        }

        public class AutoToastDreamsOption : IntegerManagerOptionItem<ManagerSim>
        {
            public AutoToastDreamsOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "RemoveStartingDreams";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
