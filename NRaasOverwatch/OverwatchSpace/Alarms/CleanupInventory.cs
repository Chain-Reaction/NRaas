using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
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
    public class CleanupInventory : AlarmOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanupInventory";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupInventory;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupInventory = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Cleanup Inventory");

                Inventories.CheckInventories(Overwatch.Log, Overwatch.Log, true);
                /*
                foreach (GameObject obj in Sims3.Gameplay.Queries.GetObjects<GameObject>())
                {
                    if (obj.Inventory == null) continue;

                    if (obj is Sims3.Gameplay.Objects.Decorations.FishBowlBase)
                    {
                        Sims3.Gameplay.Objects.Decorations.FishBowlBase bowl = obj as Sims3.Gameplay.Objects.Decorations.FishBowlBase;

                        if ((bowl.FishInBowl != null) && (Simulator.GetProxy(bowl.FishInBowl.ObjectId) == null))
                        {
                            bowl.BowlEmptied();

                            Overwatch.Log("Removed Bad Fish");
                        }
                    }

                    if ((obj.InInventory) && (Inventories.ParentInventory(obj) == null))
                    {
                        if (obj is Sims3.Gameplay.Objects.Vehicles.CarOwnable)
                        {
                            Sims3.Gameplay.Objects.Vehicles.CarOwnable vehicle = obj as Sims3.Gameplay.Objects.Vehicles.CarOwnable;

                            vehicle.AddComponent<ItemComponent>(new object[] { false, new List<Type>(new Type[] { typeof(Sim) }) });
                            vehicle.ItemComp.CanAddToInventoryDelegate = new ItemComponent.CanAddToInventoryCallback(vehicle.CanAddToInventory);

                            Overwatch.Log(vehicle.CatalogName);
                        }
                        else if (obj is UniqueObject)
                        {
                            Overwatch.Log("Corrupt Unique Object: " + obj.ToTooltipString());

                            (obj as UniqueObject).mClaimantId = 0;
                        }
                        else
                        {
                            try
                            {
                                Overwatch.Log("Bad Inventory: " + obj.CatalogName);

                                if ((obj.LotCurrent != null) && (obj.LotCurrent.Household != null))
                                {
                                    obj.LotCurrent.ModifyFunds(obj.Value);
                                }

                                obj.Destroy();
                                obj.Dispose();
                            }
                            catch (Exception e)
                            {
                                Common.Exception(obj, e);
                            }
                        }
                    }
                }*/
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
