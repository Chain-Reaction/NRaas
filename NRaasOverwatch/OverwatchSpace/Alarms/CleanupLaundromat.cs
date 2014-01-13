using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class CleanupLaundromat : AlarmOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanupLaundromat";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupLaundromat;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupLaundromat = value;
            }
        }

        protected bool Allow(Lot lot)
        {
            if (lot.IsResidentialLot)
            {
                return false;
            }
            else if (LotManager.ActiveLot == lot)
            {
                return false;
            }
            else if (Household.ActiveHousehold == null)
            {
                return false;
            }
            else if (Occupation.DoesLotHaveAnyActiveJobs(lot))
            {
                Overwatch.Log("Active Job");
                return false;
            }
            else
            {
                foreach (Sim sim in Households.AllSims(Household.ActiveHousehold))
                {
                    if (sim.LotCurrent == lot)
                    {
                        Overwatch.Log("Active Sim");
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Cleanup Laundromat");

                foreach (Lot lot in LotManager.AllLots)
                {
                    if (!Allow(lot)) continue;

                    foreach (GameObject obj in lot.GetObjects<GameObject>())
                    {
                        WashingMachine washing = obj as WashingMachine;
                        if (washing != null)
                        {
                            if (washing.mWashState != WashingMachine.WashState.Empty)
                            {
                                Overwatch.Log("Washer Emptied");

                                washing.SetObjectToReset();
                                washing.RemoveClothes();
                            }
                        }
                        else
                        {
                            Dryer dryer = obj as Dryer;
                            if (dryer != null)
                            {
                                if (dryer.CurDryerState != Dryer.DryerState.Empty)
                                {
                                    Overwatch.Log("Dryer Emptied");

                                    dryer.ForceDryerDone();
                                    dryer.TakeClothes(false);
                                    dryer.SetGeometryState("empty");
                                }
                            }
                            else
                            {
                                Clothesline line = obj as Clothesline;
                                if (line != null)
                                {
                                    if (line.CurClothesState != Dryer.DryerState.Empty)
                                    {
                                        Overwatch.Log("Line Emptied");

                                        if (line.mDripFX == null)
                                        {
                                            line.StartDrying();
                                        }
                                        line.ForceClothesDry();
                                        line.ClothesTaken();
                                    }
                                }
                            }
                        }
                    }
                }

                if (prompt)
                {
                    Overwatch.AlarmNotify(Common.Localize("CleanupLaundromat:Complete"));
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
