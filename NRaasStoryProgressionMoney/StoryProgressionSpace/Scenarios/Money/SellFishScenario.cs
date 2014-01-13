using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class SellFishScenario : SellObjectsScenario
    {
        public SellFishScenario()
            : base ()
        { }
        protected SellFishScenario(SellFishScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellFish";
            }
            else
            {
                return "SellStuff";
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override List<GameObject> GetInventory(SimDescription sim)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (Fish fish in Inventories.InventoryFindAll<Fish>(sim))
            {
                if (!fish.CanBeSold()) continue;

                list.Add(fish);
            }

            return list;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool success = false;

            List<IFishContainer> bowls = new List<IFishContainer>();

            foreach(Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                bowls.AddRange(lot.GetObjects<IFishContainer>());
            }

            List<GameObject> inventory = GetInventory(Sim);

            Dictionary<FishType,Fish> perfect = new Dictionary<FishType,Fish>();
            foreach (Fish fish in inventory)
            {
                if (fish == null) continue;

                if (fish.GetQuality() == Sims3.Gameplay.Objects.Quality.Perfect)
                {
                    if (perfect.ContainsKey(fish.Type)) continue;

                    perfect.Add(fish.Type, fish);
                }
            }

            foreach (Fish fish in inventory)
            {
                if (fish == null) continue;

                if (fish.GetQuality() == Sims3.Gameplay.Objects.Quality.Perfect)
                {
                    bool moved = false;

                    foreach (IFishContainer obj in bowls)
                    {
                        FishBowlBase bowl = obj as FishBowlBase;
                        if (bowl != null)
                        {
                            if (bowl.HasFish()) continue;

                            Inventories.ParentInventory(fish).TryToRemove(fish);

                            bowl.SetFishInBowl(fish);

                            bowl.StartFishFx();
                            bowl.FishFed();

                            EventTracker.SendEvent(EventTypeId.kPutFishInFishbowl, Sim.CreatedSim, bowl);

                            IncStat("Stored in Bowl");
                            moved = true;
                            break;
                        }
                        else
                        {
                            FishTank tank = obj as FishTank;
                            if (tank != null)
                            {
                                if (tank.Inventory == null) continue;

                                if (tank.Inventory.IsFull()) continue;

                                tank.Inventory.TryToMove(fish);

                                IncStat("Stored in Tank");
                                moved = true;
                                break;
                            }
                        }
                    }

                    if (perfect[fish.Type] == fish)
                    {
                        moved = true;

                        IncStat("Perfect Retained");
                    }

                    if (moved) continue;
                }

                int value = Money.Sell(Sim, fish);
                mFunds += value;

                AddStat("Sold", value);
                success = true;
            }

            return success;
        }

        public override Scenario Clone()
        {
            return new SellFishScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellFishScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellInventoryFish";
            }
        }
    }
}
