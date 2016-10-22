using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class SpinEx : SkatingRink.Spin, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, SkatingRink.Spin.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, SkatingRink.Spin.Definition>(Singleton);
        }

        public new class Definition : SkatingRink.Spin.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SpinEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool SkatableObjectTest(Sim a, ISkatableObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.IsActorUsingMe(a))
                {
                    return false;
                }
                SkateEx currentInteraction = a.CurrentInteraction as SkateEx;
                return ((currentInteraction != null) && currentInteraction.CanSingleSpin(ref greyedOutTooltipCallback));
            }

            public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;                
            }
        }
    }
}
