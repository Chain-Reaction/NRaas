using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public abstract class PurchaseVehicleScenario<T> : PurchaseObjectScenario<T>
        where T : class, IOwnableVehicle
    {
        int mExtraFunds = 0;

        int mMinimum = 0;

        public PurchaseVehicleScenario()
        { }
        protected PurchaseVehicleScenario(PurchaseVehicleScenario<T> scenario)
            : base (scenario)
        {
            mMinimum = scenario.mMinimum;
            mExtraFunds = scenario.mExtraFunds;
        }

        protected override int Minimum
        {
            get
            {
                return mMinimum;
            }
        }

        protected override int Maximum
        {
            get 
            {
                return int.MaxValue;
            }
        }

        protected override int Funds
        {
            get
            {
                return (base.Funds + mExtraFunds);
            }
        }

        protected override int ContinueChance
        {
            get { return 20; }
        }

        protected override BuildBuyProduct.eBuyCategory Category
        {
            get { return BuildBuyProduct.eBuyCategory.kBuyCategoryVehicles; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.TeensAndAdults;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if ((sim.CreatedSim.LotCurrent == null) || (sim.CreatedSim.LotCurrent.IsWorldLot))
            {
                IncStat("In Transit");
                return false;
            }
            else if (!Households.AllowGuardian(sim))
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.GetPreferredVehicle() != null)
            {
                IncStat("Preferred");
                return false;
            }
            else
            {
                bool found = false;
                List<IOwnableVehicle> vehicles = Inventories.InventoryDuoFindAll<IOwnableVehicle, Vehicle>(sim);

                IOwnableVehicle choice = sim.CreatedSim.GetOwnedAndUsableVehicle(sim.CreatedSim.LotCurrent) as IOwnableVehicle;
                if (choice != null)
                {
                    vehicles.Add (choice);
                }

                IOwnableVehicle reserved = sim.CreatedSim.GetReservedVehicle() as IOwnableVehicle;
                if (reserved != null)
                {
                    if (!vehicles.Contains(reserved))
                    {
                        vehicles.Add(reserved);
                    }
                }

                IOwnableVehicle preferred = sim.CreatedSim.GetPreferredVehicle();
                if (preferred != null)
                {
                    if (!vehicles.Contains(preferred))
                    {
                        vehicles.Add(preferred);
                    }
                }

                AddStat("Vehicles", vehicles.Count);

                foreach(IOwnableVehicle existing in vehicles)
                {
                    CarOwnable car = existing as CarOwnable;
                    if ((car != null) && (car.GeneratedOwnableForNpc)) continue;

                    T vehicle = existing as T;
                    if ((vehicle != null) && (!Test(vehicle)))
                    {
                        IncStat("Has Other Type");
                        return false;
                    }

                    found = true;
                }

                if (found)
                {
                    if (AddScoring("ReplaceVehicle", sim) <= 0)
                    {
                        IncStat("Replace Score Fail");
                        return false;
                    }
                }
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<IOwnableVehicle> existing = new List<IOwnableVehicle>();

            foreach (IOwnableVehicle obj in Inventories.InventoryDuoFindAll<IOwnableVehicle, Vehicle>(Sim))
            {
                CarOwnable car = obj as CarOwnable;
                if ((car != null) && (car.GeneratedOwnableForNpc)) continue;

                Vehicle vehicle = obj as Vehicle;
                if (vehicle == null) continue;

                NameComponent name = vehicle.GetComponent<NameComponent>();
                if ((name == null) || (string.IsNullOrEmpty(name.Name)))
                {
                    existing.Add(obj);
                }
            }

            mExtraFunds = 0;
            mMinimum = 0;

            foreach (IOwnableVehicle vehicle in existing)
            {
                GameObject obj = vehicle as GameObject;

                mExtraFunds += obj.Value;

                if (mMinimum < obj.Value)
                {
                    mMinimum = obj.Value;
                }
            }

            AddStat("Extra", mExtraFunds);

            if (existing.Count > 0)
            {
                mMinimum += 2500;
            }

            if (!base.PrivateUpdate(frame)) return false;

            if (Object != null)
            {
                Sim.SetPreferredVehicle(Object);
            }

            foreach (IOwnableVehicle vehicle in existing)
            {
                Money.Sell(Sim, vehicle as GameObject);
            }

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            ManagerStory.Story story = base.PrintStory(manager, name, parameters, extended, logging);
            if (story == null) return null;

            story.mID2 = story.mID1;
            story.mID1 = new ManagerStory.Story.Element(Object.ObjectId);

            return story;
        }
    }
}
