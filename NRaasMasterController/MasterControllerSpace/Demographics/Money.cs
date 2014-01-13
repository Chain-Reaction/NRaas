using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class Money : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "SimsByMoney";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            int totalHouses = 0, totalResidents = 0;

            List<FamilyFunds.Clumper> funds = new List<FamilyFunds.Clumper>();
            List<NetWorth.Clumper> worth = new List<NetWorth.Clumper>();

            Dictionary<Household, bool> houses = new Dictionary<Household, bool>();
            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (member.Household == null) continue;

                houses[member.Household] = true;
            }

            foreach (Household house in houses.Keys)
            {
                if (house.IsSpecialHousehold) continue;

                totalHouses++;

                totalResidents += CommonSpace.Helpers.Households.NumSimsIncludingPregnancy(house);

                funds.Add(FamilyFunds.New(house));
                worth.Add(NetWorth.New(house));
            }

            string fundBody = FamilyFunds.ToString("SimsByMoney:Element", funds, MasterController.Settings.mByMoneyIntervals);

            string worthBody = NetWorth.ToString("SimsByMoney:Element", worth, MasterController.Settings.mByMoneyIntervals);

            return Common.Localize("SimsByMoney:Body", false, new object[] { totalHouses, totalResidents, fundBody, worthBody });
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize("SimsByMoney:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }
    }
}
