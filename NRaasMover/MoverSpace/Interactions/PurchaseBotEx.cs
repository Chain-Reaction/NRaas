using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous.Shopping;
using Sims3.SimIFace;
using Sims3.UI;
using System;

namespace NRaas.MoverSpace.Interactions
{
    public class PurchaseBotEx : ServoBotPedestal.PurchaseBot, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, ServoBotPedestal.PurchaseBot.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, ServoBotPedestal.PurchaseBot.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public new class Definition : ServoBotPedestal.PurchaseBot.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new PurchaseBotEx();
                result.Init(ref parameters);
                return result;
            }

            public override bool Test(Sim a, ServoBotPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return CustomPurchaseTest(target, a, isAutonomous, ref greyedOutTooltipCallback);
            }

            protected static bool CustomPurchaseTest(ServoBotPedestal ths, Sim Actor, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                /*
                if (!Actor.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.None | CASAgeGenderFlags.Human))
                {
                    string localizedString = LocalizeString(Actor.IsFemale, "HouseholdFull", new object[0x0]);
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(localizedString);
                    return false;
                }
                */
                return BaseCustomPurchaseTest(ths, Actor, isAutonomous, ref greyedOutTooltipCallback);
            }

            protected static bool BaseCustomPurchaseTest(ShoppingPedestal ths, Sim Actor, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (ths.CurrentObject() == null)
                {
                    return false;
                }
                if (Actor.IsSelectable && isAutonomous)
                {
                    return false;
                }
                if (Actor.FamilyFunds < ths.ObjectCost)
                {
                    string localizedString = ShoppingPedestal.LocalizeString(Actor.IsFemale, "LackTheSimoleans", new object[0x0]);
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(localizedString);
                    return false;
                }
                return true;
            }
        }
    }
}
