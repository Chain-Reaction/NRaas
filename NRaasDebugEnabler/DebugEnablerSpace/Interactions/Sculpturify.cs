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
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
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
    public class Sculpturify : IAddInteractionPair, IAdjustInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            list.Add(new InteractionObjectPair(Singleton, obj));
        }

        public bool AdjustInteraction(InteractionObjectPair pair)
        {
            if (pair.InteractionDefinition is Definition) return false;

            if (pair.InteractionDefinition is GameObject.DEBUG_Sculpturify.Definition) return true;

            return false;
        }

        [DoesntRequireTuning]
        private sealed class Definition : GameObject.DEBUG_Sculpturify.Definition
        {
            public Definition()
            { }
            public Definition(string menuText, SculptureComponent.SculptureMaterial material, string[] menuPath)
                : base(menuText, material, menuPath)
            {}

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, GameObject target, List<InteractionObjectPair> results)
            {
                string[] menuPath = new string[] { GameObject.DEBUG_Sculpturify.LocalizeString("Sculpturify", new object[0x0]) + Localization.Ellipsis };
                foreach (SculptureComponent.SculptureMaterial material in Enum.GetValues(typeof(SculptureComponent.SculptureMaterial)))
                {
                    if (((material != SculptureComponent.SculptureMaterial.None) && (material != SculptureComponent.SculptureMaterial.Metal)) && (material != SculptureComponent.SculptureMaterial.Topiary))
                    {
                        InteractionObjectPair item = new InteractionObjectPair(new Definition(SculptingSkill.GetLocalizedMaterialName(material), material, menuPath), target);
                        results.Add(item);
                    }
                }
            }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                if (target is Sim)
                {
                    if (string.IsNullOrEmpty(MenuText))
                    {
                        return base.GetInteractionName(actor, target, iop);
                    }
                }
                return MenuText;
            }
        }
    }
}