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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class DebtSaleScenario : SellObjectsScenario
    {
        public DebtSaleScenario()
        { }
        protected DebtSaleScenario(DebtSaleScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "DebtSale";
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
            return Sims.Adults;
        }

        protected override List<GameObject> GetInventory(SimDescription sim)
        {
            List<GameObject> results = new List<GameObject>();

            foreach (GameObject obj in Inventories.InventoryFindAll<GameObject>(sim))
            {
                if (obj.Value == 0) continue;

                if (obj is Vehicle)
                {
                    CarOwnable ownable = obj as CarOwnable;
                    if (ownable != null)
                    {
                        if (ownable.GeneratedOwnableForNpc) continue;

                        if (ownable is CarMotiveMobile) continue;

                        if (ownable is SirenEnabledVehicle) continue;
                    }

                    NameComponent name = obj.GetComponent<NameComponent>();
                    if ((name == null) || (string.IsNullOrEmpty(name.Name)))
                    {
                        results.Add(obj);
                    }
                }
            }

            return results;
        }

        protected override bool Allow()
        {
            if (GetValue<RatioOption,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (AddStat("Ratio", GetValue<NetRatioOption,int>(sim.Household)) < GetValue<RatioOption,int>()) 
            {
                IncStat("Gated");
                return false;
            }

            return base.Allow(sim);
        }

        public override Scenario Clone()
        {
            return new DebtSaleScenario(this);
        }

        public class RatioOption : IntegerScenarioOptionItem<ManagerMoney, DebtSaleScenario>, ManagerMoney.IDebtOption
        {
            public RatioOption()
                : base(35)
            { }

            public override string GetTitlePrefix()
            {
                return "DebtSaleRatio";
            }
        }
    }
}
