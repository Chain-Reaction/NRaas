using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.SelectorSpace.Interactions
{
    public class RabbitholeInteraction : ImmediateInteraction<Sim, Lot>, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Lot>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                Definition definition = InteractionDefinition as Definition;

                List<InteractionObjectPair> interactions = definition.Hole.GetAllInteractionsForActor(Actor);
                if (interactions == null) return false;

                InteractionDefinitionOptionList.Perform(Actor, definition.Hit, interactions, true);
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private class Definition : ImmediateInteractionDefinition<Sim, Lot, RabbitholeInteraction>
        {
            RabbitHole mHole;

            GameObjectHit mHit;

            public Definition()
            { }
            public Definition(RabbitHole hole)
            {
                mHole = hole;
            }

            public RabbitHole Hole
            {
                get { return mHole; }
            }

            public GameObjectHit Hit
            {
                get { return mHit; }
            }

            public override string GetInteractionName(Sim a, Lot target, InteractionObjectPair interaction)
            {
                if (mHole == null) return null;

                return mHole.GetLocalizedName();
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Lot target, List<InteractionObjectPair> results)
            {
                foreach (RabbitHole hole in target.GetObjects<RabbitHole>())
                {
                    results.Add(new InteractionObjectPair(new Definition(hole), target));
                }
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                mHit = parameters.Hit;

                return base.Test(ref parameters, ref greyedOutTooltipCallback);
            }

            public override bool Test(Sim actor, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}