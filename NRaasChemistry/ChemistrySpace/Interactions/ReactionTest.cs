using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Interactions
{
    public class ReactionTest : Interaction<Sim, Sim>, Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public override bool Run()
        {
            Definition def = base.InteractionDefinition as Definition;
            base.Actor.PlayReaction(def.playType, ReactionSpeed.Immediate);

            return true;
        }

        public class Definition : InteractionDefinition<Sim, Sim, ReactionTest>
        {
            public ReactionTypes playType;

            public Definition()
            {
            }

            public Definition(ReactionTypes type)
            {
                playType = type;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                foreach (ReactionTypes @type in Enum.GetValues(typeof(ReactionTypes)))
                {
                    results.Add(new InteractionObjectPair(new Definition(@type), target));
                }
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Reaction Test - " + playType.ToString();
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { "Reaction Test" + Localization.Ellipsis };
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return Chemistry.Settings.Debugging;
            }
        }
    }
}
