using NRaas.CommonSpace.Dialogs;
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
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class GoToVenueWith : Interaction<Sim, Sim>, Common.IAddInteraction
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
                GoToVenue.Item choice = GoToVenue.GetChoices(Actor, Common.Localize("GoToVenueWith:MenuName", Actor.IsFemale));
                if (choice == null) return false;

                InteractionInstance interaction = null;
                if (choice.Value.IsCommunityLot)
                {
                    interaction = VisitCommunityLotWith.VisitWithSingleton.CreateInstance(choice.Value, Actor, GetPriority(), Autonomous, CancellableByPlayer);
                }
                else
                {
                    interaction = VisitLotWith.VisitWithSingleton.CreateInstance(choice.Value, Actor, GetPriority(), Autonomous, CancellableByPlayer);
                }

                if (Actor == Target)
                {
                    interaction.SelectedObjects = SelectedObjects;
                }
                else
                {
                    interaction.SelectedObjects = new List<object>();
                    interaction.SelectedObjects.Add(Target);
                }

                Actor.InteractionQueue.PushAsContinuation(interaction, true);
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

        private class Definition : SoloSimInteractionDefinition<GoToVenueWith>
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.Localize("GoToVenueWith:MenuName", target.IsFemale);
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                Sim actor = parameters.Actor as Sim;
                Sim target = parameters.Target as Sim;

                if ((actor.IsInGroupingSituation()) || (actor != target))
                {
                    listObjs = null;
                    headers = null;
                    NumSelectableRows = 0x0;
                }
                else
                {
                    NumSelectableRows = -1;
                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, Terrain.GoHereWith.GetValidFollowers(parameters.Actor), false);
                }
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor.SimDescription.ToddlerOrBelow) return false;

                if (target.SimDescription.ToddlerOrBelow) return false;

                return base.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
