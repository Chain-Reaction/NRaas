using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class PayForFreeObjects : Common.IAddInteraction
    {
        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        public static void OnAdded(uint stackNumber, InventoryEvent inventoryEvent, IGameObject obj)
        {
            try
            {
                switch (inventoryEvent)
                {
                    case InventoryEvent.kStackAddedTo:
                    case InventoryEvent.kStackCreated:
                        break;
                    default:
                        return;
                }

                Inventory inventory = Inventories.ParentInventory(obj);
                if (inventory == null) return;

                Sim sim = inventory.Owner as Sim;
                if (sim == null) return;

                Tracer tracer = new Tracer();
                tracer.Perform();

                if (tracer.mPurchase)
                {              
                    ManagerMoney.AdjustFundsTask.Perform(sim.SimDescription, "BuyItem", -obj.Value);
                }
            }
            catch (Exception e)
            {
                Common.Exception(obj, e);
            }
        }

        public class Tracer : StackTracer
        {
            public bool mPurchase;

            public Tracer()
            {
                AddTest(typeof(BandInstrumentSituation.JoinJammingSession), "Boolean TryToJoinJam", OnPurchase);
            }

            public bool OnPurchase(StackTrace stack, StackFrame frame)
            {
                mPurchase = true;
                return true;
            }
        }

        public class CustomInjector : Common.InteractionInjector<Sim>
        {
            public CustomInjector()
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                Sim sim = obj as Sim;
                if (sim != null)
                {
                    if (sim.Inventory != null)
                    {
                        sim.Inventory.EventCallback += OnAdded;
                    }
                    return true;
                }

                return false;
            }
        }
    }
}
