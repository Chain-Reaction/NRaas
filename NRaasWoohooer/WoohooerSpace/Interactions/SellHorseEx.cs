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
    public class SellHorseEx : EquestrianCenter.SellHorse, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<EquestrianCenter, EquestrianCenter.SellHorse.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<EquestrianCenter, EquestrianCenter.SellHorse.Definition, Definition>(true);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public new class Definition : EquestrianCenter.SellHorse.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new SellHorseEx();
                result.Init(ref parameters);
                return result;
            }
            
            public override bool Test(Sim a, EquestrianCenter target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.IsActorUsingMe(a))
                {
                    return false;
                }

                Posture posture = a.Posture as RidingPosture;
                if (posture == null)
                {
                    posture = a.Posture as LeadingHorsePosture;
                    if (posture == null)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.SellHorse.LocalizeString(a.IsFemale, "SimNeedsToBeRidingAHorse", new object[] { a }));
                        return false;
                    }
                }

                Sim container = posture.Container as Sim;
                if (!container.IsSelectable)
                {
                    return false;
                }

                SimDescription simDescription = container.SimDescription;
                if (simDescription.Child)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/RabbitHoles/EquestrianCenter/OfferStallionAsStud:NeedAdultHorse", new object[] { a }));
                    return false;
                }

                /*
                if (simDescription.IsUnicorn)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.SellHorse.LocalizeString(a.IsFemale, "UnicornCannotBeSold", new object[0x0]));
                    return false;
                }
                */

                SocialWorkerPetPutUp instance = SocialWorkerPetPutUp.Instance;
                Lot lotHome = container.LotHome;
                if ((((instance != null) && (lotHome != null)) && (instance.IsServiceRequested(lotHome) || instance.IsAnySimAssignedToLot(lotHome))) && (instance.PetToPutUp == simDescription))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
