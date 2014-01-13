using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class SellDuplicateInventoryScenario : SellObjectsScenario
    {
        public SellDuplicateInventoryScenario()
            : base ()
        { }
        protected SellDuplicateInventoryScenario(SellDuplicateInventoryScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellDuplicateInventory";
            }
            else
            {
                return "SellStuff";
            }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 100; }
        }

        protected override bool ShouldReport
        {
            get { return RandomUtil.CoinFlip(); }
        }

        protected override int PushChance
        {
            get { return 25; }
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

        protected static int SortByValue<T>(T a, T b)
            where T : class, IGameObject
        {
            if (a.Value > b.Value) return -1;

            if (a.Value < b.Value) return 1;

            return 0;
        }

        protected static void AddToList<T>(SimDescription sim, List<GameObject> objs, ItemTestFunction testFunc)
            where T : GameObject
        {
            bool first = true;

            List<T> results = Inventories.InventoryFindAll<T>(sim);

            results.Sort(new Comparison<T>(SortByValue));

            foreach (T obj in results)
            {
                if (testFunc != null)
                {
                    if (!testFunc(obj, null)) continue;
                }

                if (!first)
                {
                    objs.Add(obj as GameObject);
                }
                else
                {
                    first = false;
                }
            }
        }

        protected static bool OnTestVehicle(IGameObject obj, object customData)
        {
            CarOwnable car = obj as CarOwnable;
            if ((car != null) && (car.GeneratedOwnableForNpc)) return false;

            return true;
        }

        protected static bool OnTestCane(IGameObject obj, object customData)
        {
            Cane cane = obj as Cane;
            if ((cane != null) && (cane.UsingCane)) return false;

            return true;
        }

        protected override List<GameObject> GetInventory(SimDescription sim)
        {
            List<GameObject> list = new List<GameObject>();

            AddToList<Guitar>(sim, list, null);
            AddToList<Computer>(sim, list, null);
            AddToList<Stereo>(sim, list, null);
            AddToList<StuffedAnimal>(sim, list, null);
            AddToList<PlayCatchObject>(sim, list, null);
            AddToList<PicnicBasket>(sim, list, null);
            AddToList<FirePit>(sim, list, null);
            AddToList<Vehicle>(sim, list, OnTestVehicle);
            AddToList<Cane>(sim, list, OnTestCane);
            AddToList<Umbrella>(sim, list, null);

            return list;
        }

        public override Scenario Clone()
        {
            return new SellDuplicateInventoryScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellDuplicateInventoryScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellDuplicateInventory";
            }
        }
    }
}
