using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class GoToRabbithole : Interaction<Sim, Sim>, Common.IAddInteraction
    {
        static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                List<Item> choices = new List<Item>();

                foreach(RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
                {
                    choices.Add(new Item(hole));
                }

                Item choice = new CommonSelection<Item>(Common.Localize("GoToRabbithole:MenuName", Target.IsFemale), choices).SelectSingle();
                if (choice == null) return false;

                List<InteractionObjectPair> interactions = choice.Value.GetAllInteractionsForActor(Target);
                if (interactions == null) return false;

                for (int i = interactions.Count - 1; i >= 0; i--)
                {
                    if (interactions[i].InteractionDefinition is IImmediateInteractionDefinition)
                    {
                        interactions.RemoveAt(i);
                    }
                }

                if (InteractionDefinitionOptionList.Perform(Target, GameObjectHit.NoHit, interactions) == OptionResult.Failure)
                {
                    InteractionInstance interaction = VisitCommunityLotEx.Singleton.CreateInstance(choice.Value.LotCurrent, Target, GetPriority(), Autonomous, CancellableByPlayer);

                    Target.InteractionQueue.Add(interaction);
                }

                return true;
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

        public class Item : ValueSettingOption<RabbitHole>
        {
            public Item(RabbitHole hole)
                : base(hole, hole.GetLocalizedName(), 0, hole.GetThumbnailKey())
            { }
        }

        private class Definition : SoloSimInteractionDefinition<GoToRabbithole>
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.Localize("GoToRabbithole:MenuName", target.IsFemale);
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor != target) return false;

                return base.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
