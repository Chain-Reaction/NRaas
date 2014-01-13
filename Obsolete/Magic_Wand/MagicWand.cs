using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Decorations.Mimics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace Sims3.Gameplay.Objects.KolipokiMod
{
    public class MagicWand : SculptureTableCowPlant, ICarryable, IHasRouteRadius, IGameObject, IScriptObject, IScriptLogic, IHasScriptProxy, IObjectUI, IExportableContent
    {
        private bool CanAddToInventory(Inventory inventory)
        {
            Sim owner = inventory.Owner as Sim;
            return ((owner != null) && owner.SimDescription.ChildOrAbove);
        }

        public override void OnStartup()
        {
            base.OnStartup();

            AddComponent<ItemComponent>(new object[] { ItemComponent.SimInventoryItem });
            ItemComp.CanAddToInventoryDelegate = CanAddToInventory;
            RemoveAllInteractions();
            AddInteraction(PickUp.Singleton);
        }

        public string CarryModelName
        {
            get
            {
                return "MagicWand";
            }
        }

        public float CarryRouteToObjectRadius
        {
            get
            {
                return 0.7f;
            }
        }

        public delegate bool OnPerform();

        public static bool PerformAnimation(Sim actor, OnPerform onPerform)
        {
            IGameObject obj2 = actor.Inventory.SetInUse(typeof(MagicWand), TestFunction, typeof(MagicWand));

            actor.CarryStateMachine = StateMachineClient.Acquire(actor.Proxy.ObjectId, "wand");
            actor.CarryStateMachine.SetActor("x", actor);
            actor.CarryStateMachine.EnterState("x", "Cast Spell - Practice");
            Simulator.Sleep(0x23);
            bool result = onPerform();
            Simulator.Sleep(0x41);
            actor.Inventory.SetNotInUse(obj2);

            return result;
        }

        public static bool TestFunction(IGameObject obj, object customData)
        {
            return true;
        }

        private sealed class PickUp : Interaction<Sim, MagicWand>
        {
            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                if (!CarrySystem.PickUp(base.Actor, base.Target))
                {
                    return false;
                }
                if (!CarrySystem.PutInSimInventory(base.Actor))
                {
                    return false;
                }
                return true;
            }

            private sealed class Definition : InteractionDefinition<Sim, MagicWand, PickUp>
            {
                public override string GetInteractionName(Sim a, MagicWand target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(a.IsFemale, "MagicWand.PickUp:MenuName", new object[0]);
                }

                public override bool Test(Sim a, MagicWand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return !target.InUse;
                }
            }
        }
    }
}

