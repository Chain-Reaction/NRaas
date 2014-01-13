using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class Lots : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "Lots";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            int iHouseholds = 0, iAllLots = 0, iNonEmptyLots = 0, iCommercial = 0, iResidential = 0;
            int iFridges = 0, iCribs = 0, iSingleBeds = 0, iDoubleBeds = 0;
            int iNoFridge = 0, iNoCrib = 0, iNoBed = 0;

            int iUnfurnishedPrice = 0, iFurniturePrice = 0, iFamilyFunds = 0, iResidents = 0;

            Dictionary<Lot, bool> lots = new Dictionary<Lot, bool>();

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (member.LotHome == null) continue;

                lots[member.LotHome] = true;
            }

            foreach (Lot lot in LotManager.Lots)
            {
                if (lot.IsWorldLot) continue;

                iAllLots++;

                if (lot.LotType == LotType.Commercial)
                {
                    iCommercial++;
                }
                else
                {
                    iResidential++;
                }

                if (lot.Household == null)
                {
                    lots[lot] = true;
                }
            }

            foreach (Lot lot in lots.Keys)
            {
                if (lot.IsWorldLot) continue;

                if (lot.Household != null)
                {
                    iHouseholds++;

                    iFamilyFunds += lot.Household.FamilyFunds;

                    iResidents += CommonSpace.Helpers.Households.NumSimsIncludingPregnancy(lot.Household);
                }

                iUnfurnishedPrice += CommonSpace.Helpers.Lots.GetUnfurnishedCost(lot);

                iFurniturePrice += lot.CalculateFurnitureWorth();

                bool bFridge = false, bCrib = false, bDoubleBed = false, bSingleBed = false;

                List<IGameObject> lotObjects = new List<IGameObject>(lot.GetObjects<IGameObject>());
                foreach (IGameObject obj in lotObjects)
                {
                    if (obj is Sims3.Gameplay.Objects.Appliances.Fridge)
                    {
                        iFridges++;
                        bFridge = true;
                    }
                    else if (obj is Sims3.Gameplay.Objects.Beds.Crib)
                    {
                        iCribs++;
                        bCrib = true;
                    }
                    else if (obj is Sims3.Gameplay.Objects.Beds.BedDouble)
                    {
                        iDoubleBeds++;
                        bDoubleBed = true;
                    }
                    else if (obj is Sims3.Gameplay.Objects.Beds.BedSingle)
                    {
                        iSingleBeds++;
                        bSingleBed = true;
                    }
                }

                if ((lot.IsResidentialLot) && (lotObjects.Count > 2))
                {
                    iNonEmptyLots++;

                    if (!bFridge)
                    {
                        iNoFridge++;
                    }
                    if (!bCrib)
                    {
                        iNoCrib++;
                    }
                    if ((!bDoubleBed) && (!bSingleBed))
                    {
                        iNoBed++;
                    }
                }
            }

            List<object> objects = new List<object>();

            objects.Add(iAllLots);
            objects.Add(iResidential);
            objects.Add(iCommercial);

            objects.Add(iHouseholds);
            objects.Add(iNonEmptyLots - iHouseholds);
            objects.Add(iResidential - iNonEmptyLots);
            if (iNonEmptyLots == 0)
            {
                objects.Add(0);
            }
            else
            {
                objects.Add(iResidents / iNonEmptyLots);
            }

            objects.Add(iUnfurnishedPrice);
            if (iNonEmptyLots == 0)
            {
                objects.Add("");
            }
            else
            {
                objects.Add(iUnfurnishedPrice / iNonEmptyLots);
            }

            objects.Add(iFurniturePrice);
            if (iNonEmptyLots == 0)
            {
                objects.Add("");
            }
            else
            {
                objects.Add(iFurniturePrice / iNonEmptyLots);
            }

            objects.Add(iFamilyFunds);
            if (iHouseholds == 0)
            {
                objects.Add("");
            }
            else
            {
                objects.Add(iFamilyFunds / iHouseholds);
            }

            objects.Add(iUnfurnishedPrice + iFurniturePrice + iFamilyFunds);
            objects.Add(iFridges);
            objects.Add(iCribs);
            objects.Add(iSingleBeds);
            objects.Add(iDoubleBeds);
            objects.Add(iNoFridge);
            objects.Add(iNoCrib);
            objects.Add(iNoBed);

            return Common.Localize("Lots:Body", false, objects.ToArray());
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize ("Lots:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }
    }
}
