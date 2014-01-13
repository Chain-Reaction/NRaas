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
    public class OfferStallionAsStudEx : EquestrianCenter.OfferStallionAsStud, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<EquestrianCenter, EquestrianCenter.OfferStallionAsStud.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<EquestrianCenter, EquestrianCenter.OfferStallionAsStud.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public new class Definition : EquestrianCenter.OfferStallionAsStud.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new OfferStallionAsStudEx();
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
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.OfferStallionAsStud.LocalizeString(a.IsFemale, "NeedHorse", new object[] { a }));
                        return false;
                    }
                }

                Sim container = posture.Container as Sim;
                if (!container.IsSelectable)
                {
                    return false;
                }

                if (container.SimDescription.IsFemale)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.OfferStallionAsStud.LocalizeString(a.IsFemale, "NeedMaleHorse", new object[] { a }));
                    return false;
                }
                else if (container.SimDescription.Child)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.OfferStallionAsStud.LocalizeString(a.IsFemale, "NeedAdultHorse", new object[] { a }));
                    return false;
                }
                else if (container.SimDescription.Elder)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.OfferStallionAsStud.LocalizeString(container.IsFemale, "ElderPetsTooOldForBreed", new object[] { container }));
                    return false;
                }

                /*
                if (container.SimDescription.IsUnicorn)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.OfferStallionAsStud.LocalizeString(a.IsFemale, "UnicornCannotBeOffered", new object[] { a }));
                    return false;
                }
                */

                Motives motives = container.Motives;
                if (motives.HasMotive(CommodityKind.StallionOffered) && (motives.GetValue(CommodityKind.StallionOffered) >= EquestrianCenter.kOfferStallionMotiveThreshold))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.OfferStallionAsStud.LocalizeString(a.IsFemale, "Cooldown", new object[] { a, container }));
                    return false;
                }
                return true;
            }
        }
    }
}
