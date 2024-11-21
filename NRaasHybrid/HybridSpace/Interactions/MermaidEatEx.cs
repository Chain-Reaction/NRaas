using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class MermaidEatEx : OccultMermaid.MermaidEat, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;
        
        public void OnPreLoad()
        {
            Tunings.Inject<Fish, OccultMermaid.MermaidEat.Definition, Definition>(false);

            sOldSingleton = OccultMermaid.MermaidEat.Singleton;
            OccultMermaid.MermaidEat.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Fish, OccultMermaid.MermaidEat.Definition>(Singleton);
        }

        public new class Definition : OccultMermaid.MermaidEat.Definition, IScubaDivingInteractionDefinition
        {
            public Definition()
            { }

            public Definition(IFishContainer container)
            {
                base.FishContainer = container;
            }

            public override string GetInteractionName(Sim actor, Fish target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Fish target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!OccultTypeHelper.HasType(a, OccultTypes.Mermaid)) return false;

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
