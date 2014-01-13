using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class SimOnFireScenario : SimBuffScenario
    {
        public SimOnFireScenario()
        { }
        protected SimOnFireScenario(SimOnFireScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SimOnFire";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(BuffNames buff)
        {
            return ((buff == BuffNames.OnFire) || (buff == BuffNames.Torched));
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim createdSim = Sim.CreatedSim;
            if (createdSim == null) return false;

            if (createdSim.LotCurrent == null) return false;

            if (createdSim.LotCurrent.IsCommunityLot)
            {
                if (createdSim.LotCurrent != LotManager.ActiveLot)
                {
                    if (Sims != null)
                    {
                        Sims.Reset(Sim);
                    }

                    if (createdSim.Motives != null)
                    {
                        createdSim.Motives.ForceSetMax(CommodityKind.Temperature);
                    }

                    if ((createdSim != null) && (createdSim.BuffManager != null))
                    {
                        createdSim.BuffManager.RemoveElement(BuffNames.OnFire);
                        createdSim.BuffManager.RemoveElement(BuffNames.Torched);
                    }

                    IncStat("Sim Reset");
                }
                else
                {
                    IncStat("Active Lot");
                }

                return true;
            }

            Add(frame, new ExtinguishScenario(createdSim.LotCurrent), ScenarioResult.Start);
            return true;
        }

        public override Scenario Clone()
        {
            return new SimOnFireScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSituation, SimOnFireScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SimOnFire";
            }
        }
    }
}
