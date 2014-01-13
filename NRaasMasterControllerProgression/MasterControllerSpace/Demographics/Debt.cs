extern alias SP;

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
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class Debt : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "SimsByDebt";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            int totalHouses = 0, totalResidents = 0;

            List<ProgressionDebt.Clumper> debt = new List<ProgressionDebt.Clumper>();
            List<ProgressionNetWorth.Clumper> worth = new List<ProgressionNetWorth.Clumper>();

            Dictionary<Household, bool> houses = new Dictionary<Household, bool>();
            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (member.Household == null) continue;

                houses[member.Household] = true;
            }

            foreach(Household house in houses.Keys)
            {
                if (house.IsSpecialHousehold) continue;

                totalHouses++;

                totalResidents += CommonSpace.Helpers.Households.NumSimsIncludingPregnancy(house);

                debt.Add(ProgressionDebt.New(house));
                worth.Add(ProgressionNetWorth.New(house));
            }

            string debtBody = ProgressionDebt.ToString("SimsByMoney:Element", debt, MasterController.Settings.mByMoneyIntervals);

            string worthBody = ProgressionNetWorth.ToString("SimsByMoney:Element", worth, MasterController.Settings.mByMoneyIntervals);

            return MasterController.Localize("SimsByDebt:Body", false, new object[] { totalHouses, totalResidents, debtBody, worthBody });
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize("SimsByDebt:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }
    }
}
