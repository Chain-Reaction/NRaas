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
    public class SoloSpinEx : SkatingRink.SoloSpin, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, SkatingRink.SoloSpin.Definition, Definition>(false);

            sOldSingleton = SkatingRink.SoloSpin.Singleton as InteractionDefinition;
            SkatingRink.SoloSpin.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, SkatingRink.SoloSpin.Definition>(SkatingRink.SoloSpin.Singleton as InteractionDefinition);
        }

        public new class Definition : SkatingRink.SoloSpin.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SoloSpinEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a != target)
                {
                    return false;
                }
                if (SkateHelper.CalculateIfActorIsOccultSkaterEx(a))
                {
                    return false;
                }
                SkatingRink.Skate currentInteraction = a.CurrentInteraction as SkatingRink.Skate;
                return ((currentInteraction != null) && currentInteraction.CanSingleSpin(ref greyedOutTooltipCallback));
            }
        }
    }
}