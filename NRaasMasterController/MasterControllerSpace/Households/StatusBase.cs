using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public abstract class StatusBase : HouseholdFromList
    {

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected static void GetTaxes(Household me, out int owed, out int savings, out int vacationHome)
        {
            owed = (int)(me.ComputeNetWorthOfObjectsInHousehold(true) * Mailbox.kPercentageOfWealthBilled);

            vacationHome = 0;

            savings = 0;

            if (me.RealEstateManager != null)
            {
                int valueOfAllVacationHomes = me.RealEstateManager.GetValueOfAllVacationHomes();

                vacationHome = (int)Math.Round((double)(valueOfAllVacationHomes * RealEstateManager.kPercentageOfVacationHomeValueBilled));

                owed += vacationHome;
            }

            if (me.LotHome != null)
            {
                Dictionary<int, List<float>> dictionary = new Dictionary<int, List<float>>();
                foreach (IReduceBills bills in me.LotHome.GetObjects<IReduceBills>())
                {
                    List<float> list;
                    int key = bills.ReductionArrayIndex();
                    float item = bills.PercentageReduction();
                    if (dictionary.TryGetValue(key, out list))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        List<float> list2 = new List<float>();
                        list2.Add((float)bills.MaxNumberContributions());
                        list2.Add(item);
                        dictionary.Add(key, list2);
                    }
                }

                foreach (KeyValuePair<int, List<float>> pair in dictionary)
                {
                    int num5 = (int)pair.Value[0];
                    pair.Value.RemoveAt(0);
                    pair.Value.Sort();
                    int count = pair.Value.Count;
                    num5 = Math.Min(num5, count);
                    float num7 = 0f;
                    for (int i = 1; i <= num5; i++)
                    {
                        num7 += pair.Value[count - i];
                    }
                    int amount = (int)(owed * num7);

                    owed -= amount;

                    savings += amount;
                }
            }
        }

        public string GetDetails(IMiniSimDescription mini)
        {
            SimDescription sim = mini as SimDescription;
            if (sim != null)
            {
                return GetDetails(sim.LotHome, sim.Household);
            }
            else
            {
                return null;
            }
        }

        public static string GetDetails(Lot lot, Household me)
        {
            string msg = null;
            if (me != null)
            {
                msg += Common.Localize("StatusHouse:HouseName", false, new object[] { me.Name });
            }

            if (lot != null)
            {
                msg += Common.Localize("StatusHouse:LotName", false, new object[] { lot.Name });
            }

            if (me != null)
            {
                msg += Common.Localize("StatusHouse:Funds", false, new object[] { me.FamilyFunds });

                if (me.RealEstateManager != null)
                {
                    int realEstate = 0;
                    foreach (PropertyData data in me.RealEstateManager.AllProperties)
                    {
                        realEstate += data.TotalValue;
                    }

                    msg += Common.Localize("StatusHouse:RealEstate", false, new object[] { realEstate });
                }


                if (lot != null)
                {
                    int taxes, savings, vacationHome;
                    GetTaxes(me, out taxes, out savings, out vacationHome);
                    msg += Common.Localize("StatusHouse:Taxes", false, new object[] { (taxes - vacationHome) + savings, vacationHome, savings, taxes });
                }
            }

            if (lot == null)
            {
                msg += Common.Localize("StatusHouse:Homeless");
            }
            else
            {
                msg += Common.Localize("StatusHouse:Address", false, new object[] { lot.Address, Lots.GetUnfurnishedCost(lot), lot.Cost - lot.GetUnfurnishedCost(), lot.Cost });

                int iFridges = 0, iCribs = 0, iSingleBeds = 0, iDoubleBeds = 0;

                List<IGameObject> lotObjects = new List<IGameObject>(lot.GetObjects<IGameObject>());
                foreach (IGameObject obj in lotObjects)
                {
                    if (obj is Sims3.Gameplay.Objects.Appliances.Fridge)
                    {
                        iFridges++;
                    }
                    else if (obj is ICrib)
                    {
                        iCribs++;
                    }
                    else if (obj is IBedDouble)
                    {
                        iDoubleBeds++;
                    }
                    else if (obj is IBedSingle)
                    {
                        iSingleBeds++;
                    }
                }

                msg += Common.Localize("StatusHouse:Objects", false, new object[] { iFridges, iCribs, iDoubleBeds, iSingleBeds });
            }

            if ((me != null) && (!SimTypes.IsService(me)))
            {
                int count = 0;
                string occupants = null;

                foreach (SimDescription sim in CommonSpace.Helpers.Households.All(me))
                {
                    occupants += Common.NewLine + sim.FullName;

                    count++;
                    if (count >= 24) break;
                }

                msg += Common.Localize("StatusHouse:Occupants", false, new object[] { occupants });
            }

            return msg;
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            string msg = GetDetails(lot, me);
            if (msg != null)
            {
                SimpleMessageDialog.Show(Name, msg);
            }
            return OptionResult.SuccessClose;
        }
    }
}
