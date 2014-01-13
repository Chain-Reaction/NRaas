using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Interactions
{
    public class TeleportToCommunityLotEx : RoutineMachine.TeleportToCommunityLot, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<RoutineMachine>(PrivacyInteraction.Singleton);
            interactions.Replace<RoutineMachine, RoutineMachine.TeleportToCommunityLot.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<RoutineMachine, RoutineMachine.TeleportToCommunityLot.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                return TeleportBaseEx.Run(this);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : RoutineMachine.TeleportToCommunityLot.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TeleportToCommunityLotEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, RoutineMachine target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}


