using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Interactions;
using Sims3.UI;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Appliances;

namespace HaveCoffeeWithMe
{
    public class DrinkCoffee
    {
        public class Drink : Eat
        {
            // Fields
            new public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                CommonMethods.PayForCoffee(base.Actor, base.Actor.LotCurrent);
                return base.Run();
            }

            new public class Definition : InteractionDefinition<Sim, IEdible, Drink>
            {

                public override string GetInteractionName(Sim a, IEdible target, InteractionObjectPair interaction)
                {
                    string str;
                    if (a.HasTrait(unchecked((TraitNames)(-5175480303478029536L))))
                    {
                        str = "DrinkInteractionName_Good";
                    }
                    else if (a.HasTrait(unchecked((TraitNames)(-5175480303478029664L))))
                    {
                        str = "DrinkInteractionName_Evil";
                    }
                    if (a.SimDescription.TraitManager.HasElement(TraitNames.Daredevil))
                    {
                        str = "DrinkInteractionName_Daredevil";
                    }
                    else
                    {
                        str = "DrinkInteractionName";
                    }
                    return HotBeverageMachine.LocalizeString(str, new object[] { target.FoodName });
                }

                public override bool Test(Sim a, IEdible target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous && (a.HoursUntilWakeupTime < 12))
                    {
                        return false;
                    }
                    if (isAutonomous && a.HasBuffsToPreventSleepiness())
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
    }
}
