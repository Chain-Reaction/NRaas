using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Interactions
{
    public class TravelBackToHomeWorldEx : Computer.TravelBackToHomeWorld, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Computer, Computer.TravelBackToHomeWorld.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Computer, Computer.TravelBackToHomeWorld.Definition>(Singleton);
        }

        public new class Definition : Computer.TravelBackToHomeWorld.Definition
        {
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TravelBackToHomeWorldEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!target.IsComputerUsable(a, true, false, isAutonomous)) return false;

                    return Helpers.TravelUtilEx.CanSimTriggerTravelToHomeWorld(a, ref greyedOutTooltipCallback);
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
