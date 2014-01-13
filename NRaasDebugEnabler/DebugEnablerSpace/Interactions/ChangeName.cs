using NRaas.CommonSpace.Interactions;
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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class ChangeName : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if ((obj is Lot) || (obj is RabbitHole))
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                if (Target is Lot)
                {
                    Lot target = Target as Lot;

                    string text = StringInputDialog.Show(Common.Localize("ChangeName:Lot"), Common.Localize("ChangeName:Prompt"), target.Name);
                    if (string.IsNullOrEmpty(text)) return false;

                    target.Name = text;
                }
                else if (Target is RabbitHole)
                {
                    RabbitHole hole = Target as RabbitHole;

                    string str = StringInputDialog.Show(Common.Localize("ChangeName:Rabbithole"), Common.Localize("ChangeName:Prompt"), Target.CatalogName);
                    if (string.IsNullOrEmpty(str)) return false;

                    hole.SetOverrideNameString(str);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<ChangeName>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("ChangeName:MenuName");
            }
        }
    }
}