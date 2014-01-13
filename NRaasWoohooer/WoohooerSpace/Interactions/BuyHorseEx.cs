using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class BuyHorseEx : EquestrianCenter.BuyHorse, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<EquestrianCenter, EquestrianCenter.BuyHorse.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<EquestrianCenter, EquestrianCenter.BuyHorse.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public new class Definition : EquestrianCenter.BuyHorse.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new BuyHorseEx();
                result.Init(ref parameters);
                return result;
            }
            
            public override bool Test(Sim a, EquestrianCenter target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                PetPool pool;
                if (!PetPoolManager.TryGetPetPool(PetPoolType.BuySellHorse, out pool, false))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BuyHorse.LocalizeString(a.IsFemale, "PoolEmpty", new object[0x0]));
                    return false;
                }

                /*
                if (!a.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Horse, 0x1, true))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BuyHorse.LocalizeString(a.IsFemale, "TooManyPetsInHousehold", new object[0x0]));
                    return false;
                }
                */

                Lot lotHome = a.LotHome;
                if ((lotHome != null) && lotHome.HasVirtualResidentialSlots)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(PetAdoption.LocalizeString(a.IsFemale, "CannotAdoptHorse", new object[0]));
                    return false;
                }

                return true;
            }
        }
    }
}
