using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class CallPurchaseLocationHomeEx : Phone.CallPurchaseLocationHome, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Phone, Phone.CallPurchaseLocationHome.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallPurchaseLocationHome.Definition>(Singleton);
        }

        public override Phone.Call.ConversationBehavior OnCallConnected()
        {
            try
            {
                List<Lot> purchaseableLots = RentalHelper.GetPurchaseableLots();
                if (purchaseableLots.Count != 0x0)
                {
                    mChosenLot = RealEstateManager.ChooseLot(purchaseableLots, Actor, Localization.LocalizeString("Gameplay/Locations/LocationHomeDeed:PurchaseLot", new object[0x0]), true);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return Phone.Call.ConversationBehavior.JustHangUp;
        }

        public override void OnCallFinished()
        {
            try
            {
                if (mChosenLot != null)
                {
                    RentalHelper.PurchaseRentalLot(StoryProgression.Main.Money, Actor, mChosenLot);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public new class Definition : Phone.Call.CallDefinition<CallPurchaseLocationHomeEx>
        {
            public override string[] GetPath(bool isFemale)
            {
                if (GameUtils.IsInstalled(ProductVersion.EP9))
                {
                    return new string[] { (Localization.LocalizeString("Gameplay/Objects/Electronics/SmartPhone:RealEstateAndTravel", new object[0x0]) + Localization.Ellipsis) };
                }
                else
                {
                    return new string[] { (Localization.LocalizeString("Gameplay/Core/Lot:RealEstateInteractionsPath", new object[0x0]) + Localization.Ellipsis) };
                }
            }

            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                if (Common.IsAwayFromHomeworld())
                {
                    return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
                }
                else
                {
                    return Common.Localize("PurchaseRentalLotPhone:MenuName", actor.IsFemale);
                }
            }

            public override bool Test(Sim a, Phone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }

                    if (Common.IsAwayFromHomeworld())
                    {
                        return TravelUtil.PurchaseLocationHomeInteractionTest(a);
                    }
                    else
                    {
                        return true;
                    }
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

