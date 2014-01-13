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
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Tutorial;
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
    public class CallPurchaseLocationHomeEx : Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Phone, Phone.CallPurchaseLocationHome.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Phone.CallPurchaseLocationHome.Singleton;
            Phone.CallPurchaseLocationHome.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallPurchaseLocationHome.Definition>(Phone.CallPurchaseLocationHome.Singleton);
        }

        public class Definition : Phone.CallPurchaseLocationHome.Definition
        {
            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public static bool CanPurchaseLocationHome(Sim sim, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!GameUtils.IsOnVacation())
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not On Vacation");
                    return false;
                }                
                else if (!sim.Household.RealEstateManager.HasBeenFixedupInHomeworld)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not HasBeenFixedupInHomeworld");
                    return false;                    
                }
                else if (sim.VisaManager.GetVisaLevel(GameUtils.GetCurrentWorld()) < RealEstateManager.kVisaLevelRequiredToPurchaseVacationHome)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("GetVisaLevel <= " + RealEstateManager.kVisaLevelRequiredToPurchaseVacationHome);
                    return false;                    
                }

                return true;
            }

            public static bool PurchaseLocationHomeInteractionTest(Sim actor, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!CanPurchaseLocationHome(actor, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                if (RealEstateManager.GetPurchaseableLots().Count == 0x0)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Purchaseable Lots");
                    return false;
                }

                return true;
            }

            public override bool Test(Sim a, Phone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (Common.kDebugging)
                {
                    if (!PurchaseLocationHomeInteractionTest(a, ref greyedOutTooltipCallback)) return false;
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
