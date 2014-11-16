using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Appliances;

namespace HaveCoffeeWithMe
{
    class DrinkCoffeeInteraction
    {
        public class DrinkCoffee : Interaction<Sim, BarTray>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();
           
            private const string sLocalizationKey = "HaveCoffeeWithMe:";

            // Methods
            public override ThumbnailKey GetIconKey()
            {
                ResourceKey key = base.Target.GetResourceKey();
                foreach (Slot slot in base.Target.GetContainmentSlots())
                {                    
                    HotBeverageMachine.Cup cup = base.Target.GetContainedObject(slot) as HotBeverageMachine.Cup;
                    if (cup != null)
                    {
                        key = cup.GetResourceKey();
                        break;
                    }
                }               

                return new ThumbnailKey(key, ThumbnailSize.Medium);
            }

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString(sLocalizationKey + name, parameters);
            }

            public override bool Run()
            {
                Route r = base.Actor.CreateRoute();
                r.SetOption(Route.RouteOption.DoLineOfSightCheckUserOverride, true);
                r.PlanToPointRadialRange(base.Target.Position, 0.5f, 3f, Vector3.UnitZ, 360f, RouteDistancePreference.PreferNearestToRouteOrigin, RouteOrientationPreference.TowardsObject);
                if (!base.Actor.DoRoute(r))
                {
                    base.Actor.AddExitReason(ExitReason.RouteFailed);
                    return false;
                }
                if (!base.Target.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), BarTray.kTimeToWaitInLine))
                {
                    return false;
                }
                List<Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup> randomList = new List<Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup>();
                foreach (Slot slot in base.Target.GetContainmentSlots())
                {
                    Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup containedObject = base.Target.GetContainedObject(slot) as Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup;
                    if ((containedObject != null) && !containedObject.InUse)
                    {
                        randomList.Add(containedObject);
                    }
                }
                if (randomList.Count == 0)
                {
                    return false;
                }
                Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup randomObjectFromList = RandomUtil.GetRandomObjectFromList<Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup>(randomList);
                if ((randomObjectFromList != null) && CarrySystem.PickUp(base.Actor, randomObjectFromList))
                {                 
                    //Pay for the coffee
                    CommonMethods.PayForCoffee(base.Actor, base.Target.LotCurrent);

                    InteractionInstance instance = EatHeldFood.Singleton.CreateInstance(randomObjectFromList, base.Actor, base.Actor.InheritedPriority(), false, true);
                    return Actor.InteractionQueue.PushAsContinuation(instance, false);

                }
                return false;
            }       

            // Nested Types
            private sealed class Definition : InteractionDefinition<Sim, BarTray, DrinkCoffee>
            {
                // Methods
                public override string GetInteractionName(Sim a, BarTray target, InteractionObjectPair interaction)
                {
                    return HaveCoffeeWithMe.DrinkCoffeeInteraction.DrinkCoffee.LocalizeString("DrinkCoffee", new object[0]);
                }

                public override bool Test(Sim a, BarTray target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;                   
                }
            }
        }


    }
}
