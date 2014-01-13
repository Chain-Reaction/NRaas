using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
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
    public class MoveFromFamilyInventory : DebugEnablerInteraction<Sim>
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is Sim)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                List<Item> items = new List<Item>();
                foreach (GameObject obj in Inventories.QuickFind<GameObject>(Actor.Household.SharedFamilyInventory.Inventory))
                {
                    items.Add(new Item(obj));
                }

                CommonSelection<Item>.Results choices = new CommonSelection<Item>(GetInteractionName(), items).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return false;

                foreach (Item item in choices)
                {
                    Inventory inventory = Inventories.ParentInventory(item.Value);

                    if (!inventory.TryToRemove(item.Value)) continue;

                    uint stackNumber = Actor.Inventory.FindValidStack(item.Value);
                    Actor.Inventory.AddInternal(item.Value, stackNumber, false);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<MoveFromFamilyInventory>
        {
            public override string GetInteractionName(IActor a, Sim target, InteractionObjectPair interaction)
            {
                return Common.Localize("MoveFromFamilyInventory:MenuName");
            }
        }

        public class Item : ValueSettingOption<GameObject>
        {
            public Item(GameObject obj)
            {
                mValue = obj;

                mThumbnail = mValue.GetThumbnailKey();
            }

            public override string Name
            {
                get { return mValue.CatalogName; }
            }
        }
    }
}