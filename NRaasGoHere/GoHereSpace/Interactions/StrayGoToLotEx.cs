using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.PetSystems;
using Sims3.SimIFace;
using Sims3.UI;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    class StrayGoToLotEx : StrayGoToLot, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {     
            sOldSingleton = Singleton as InteractionDefinition;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Lot, StrayGoToLot.Definition>(Singleton as InteractionDefinition);
        }

        public void MakeStrayGoHomeEx(Sim sim, float x)
        {
            // still not sure why this fires immediately. My theory is it's something to do with
            // changes in high LOD simulation or maybe instantiation is slow...
            // either way, stop yanking the animal out of the world                       
            this.OnGoToLotSuccessEx(sim, x);
        }

        public void OnGoToLotSuccessEx(Sim sim, float x)
        {            
            base.OnGoToLotSuccess(sim, x);
        }

        public override bool Run()
        {           
            StrayRouteToLot entry = StrayRouteToLot.Singleton.CreateInstanceWithCallbacks(base.Target, base.Actor, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, false, null, new Callback(this.OnGoToLotSuccessEx), new Callback(this.MakeStrayGoHomeEx)) as StrayRouteToLot;
            base.Actor.InteractionQueue.AddNext(entry);               

            return true;
        }

        private new class Definition : StrayGoToLot.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new StrayGoToLotEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }            

            public override bool Test(Sim actor, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return actor.SimDescription.IsStray;
            }
        }
    }
}
