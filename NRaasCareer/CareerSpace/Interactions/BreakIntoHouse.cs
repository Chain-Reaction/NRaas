using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;

namespace NRaas.CareerSpace.Interactions
{
    public class BreakIntoHouse : Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Lot, PrivateEyeInteractions.BreakIntoHouse.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Lot>(Singleton);
        }

        public class Definition : PrivateEyeInteractions.BreakIntoHouse.Definition
        {
            public override string GetInteractionName(Sim a, Lot target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(PrivateEyeInteractions.BreakIntoHouse.Singleton, target));
            }

            public override bool Test(Sim a, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.IsCommunityLot)
                {
                    return false;
                }
                else if (a.IsGreetedOnLot(target))
                {
                    return false;
                }

                OmniCareer omni = a.Occupation as OmniCareer;
                if (omni == null) return false;

                return omni.CanBreakIntoHouses();
            }
        }
    }
}
