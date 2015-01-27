using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Core;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Reflection;
using System.Text;

namespace NRaas.TempestSpace.Interactions
{
    public class TurnOnEx : Sprinkler.TurnOn, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sprinkler, Sprinkler.TurnOn.Definition>(Singleton);
        }

        public override bool Run()
        {
            base.Run();
            SprinklerEx.RemoveEAAlarms(base.Target);
            return true;
        }

        [DoesntRequireTuning]
        public new class Definition : Sprinkler.TurnOn.Definition
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Sprinkler target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TurnOnEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Sprinkler target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return ((!target.TurnedOn && !target.OnOffChanging) && !target.IsBeingUpgraded);                
            }            
        }
    }
}