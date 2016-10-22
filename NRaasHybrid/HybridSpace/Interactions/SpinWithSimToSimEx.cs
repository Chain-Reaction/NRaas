using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
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
    public class SpinWithSimToSimEx : SkatingRink.SpinWithSimToSim, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, SkatingRink.SpinWithSimToSim.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, SkatingRink.SpinWithSimToSim.Definition>(Singleton);
        }        

        public new class Definition : SkatingRink.SpinWithSimToSim.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SpinWithSimToSimEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a == target)
                {
                    return false;
                }
                if (SkateHelper.CalculateIfActorIsOccultSkaterEx(a) || SkateHelper.CalculateIfActorIsOccultSkaterEx(target))
                {
                    return false;
                }
                SkatingRink.Skate currentInteraction = a.CurrentInteraction as SkatingRink.Skate;
                SkatingRink.Skate skate2 = target.CurrentInteraction as SkatingRink.Skate;
                if ((currentInteraction == null) || (skate2 == null))
                {
                    return false;
                }
                return (((currentInteraction.Target == skate2.Target) && currentInteraction.CanCouplesSpin(true)) && skate2.CanCouplesSpin(false));
            }
        }
    }
}