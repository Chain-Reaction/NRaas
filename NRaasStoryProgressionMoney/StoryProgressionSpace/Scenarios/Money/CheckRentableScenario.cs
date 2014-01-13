using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
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
using Sims3.Gameplay.RealEstate;
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
    public class CheckRentableScenario : LotScenario
    {
        public static bool sImmediateUpdate = true;

        public CheckRentableScenario()
        { }
        protected CheckRentableScenario(CheckRentableScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CheckRentable";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            if (sImmediateUpdate)
            {
                sImmediateUpdate = false;
                return true;
            }

            return base.Allow(fullUpdate, initialPass);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Household rentalOwner = null;

            if (Lot.ResidentialLotSubType == ResidentialLotSubType.kEP10_PrivateLot)
            {
                foreach (Household house in Household.GetHouseholdsLivingInWorld())
                {
                    if (house.RealEstateManager == null) continue;

                    PropertyData data = house.RealEstateManager.FindProperty(Lot);
                    if (data == null) continue;

                    if (data.PropertyType == RealEstatePropertyType.PrivateLot) continue;

                    rentalOwner = house;
                    break;
                }
            }
            else if (!RentalHelper.IsRentable(Money, Lot))
            {
                rentalOwner = Money.GetDeedOwner(Lot);
            }

            if (rentalOwner == null) return false;

            SimDescription headOfFamily = SimTypes.HeadOfFamily(rentalOwner);
            if ((headOfFamily != null) && (headOfFamily.CreatedSim != null))
            {
                RentalHelper.SellRentalLot(Money, headOfFamily.CreatedSim, Lot);

                IncStat("Sold");
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new CheckRentableScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, CheckRentableScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CheckRentable";
            }
        }
    }
}
