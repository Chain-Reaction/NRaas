using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class StockAny : DebugEnablerInteraction<FishTank>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is FishTank)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                List<Item> allChoices = new List<Item>();

                foreach (KeyValuePair<FishType,FishData> pair in Fish.sFishData)
                {
                    allChoices.Add(new Item(pair.Key, pair.Value));
                }

                CommonSelection<Item>.Results choices = new CommonSelection<Item>(Common.Localize("StockAny:MenuName"), allChoices).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return true;

                int maxInventoryCapacity = Target.Inventory.mMaxInventoryCapacity;

                try
                {
                    Target.Inventory.mMaxInventoryCapacity = 0;

                    foreach (Item choice in choices)
                    {
                        Fish fish = Fish.MakeFish(choice.Value, false);
                        if (fish == null) continue;

                        Target.Add(fish);
                    }
                }
                finally
                {
                    Target.Inventory.mMaxInventoryCapacity = maxInventoryCapacity;
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<StockAny>
        {
            public override string GetInteractionName(IActor a, FishTank target, InteractionObjectPair interaction)
            {
                return Common.Localize("StockAny:MenuName");
            }
        }

        public class Item : ValueSettingOption<FishType>
        {
            public Item(FishType type, FishData data)
                : base(type, Localization.LocalizeString(data.StringKeyName, new object[0x0]), 0)
            { }
        }
    }
}