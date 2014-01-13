using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class ScanRoom : Interaction<Sim, Sim>, Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public ScanRoom()
        { }

        public override bool Run()
        {
            try
            {
                List<SimDescription> sims = new List<SimDescription>();

                foreach (Sim sim in Actor.LotCurrent.GetObjects<Sim>())
                {
                    if (sim == Actor) continue;

                    if (sim.RoomId != Actor.RoomId) continue;

                    sims.Add(sim.SimDescription);
                }

                SimSelection selection = new SimSelection(Common.Localize("ScanRoom:MenuName"), Actor.SimDescription, sims, SimSelection.Type.ScanRoom, -1000);
                SimDescription choice = selection.SelectSingle();

                if (selection.IsEmpty)
                {
                    Common.Notify(Common.Localize("ScanRoom:Failure"));
                    return false;
                }

                if (choice != null)
                {
                    if (Actor.IsHuman)
                    {
                        Actor.InteractionQueue.PushAsContinuation(new SocialInteractionA.Definition("Flirt", null, null, false), choice.CreatedSim, true);
                    }
                    else
                    {
                        Actor.InteractionQueue.PushAsContinuation(new SocialInteractionA.Definition("Pet Socialize", null, null, false), choice.CreatedSim, true);
                    }
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

        public class Definition : InteractionDefinition<Sim, Sim, ScanRoom>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.Localize("ScanRoom:MenuName", actor.IsFemale);
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor.SimDescription.ChildOrBelow) return false;

                if (isAutonomous) return false;

                if (actor != target) return false;

                if (actor.LotCurrent == null) return false;

                if (actor.LotCurrent.IsWorldLot) return false;

                return true;
            }
        }
    }
}
