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
using Sims3.Gameplay.Skills;
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
    public class BreedMareEx : EquestrianCenter.BreedMare, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<EquestrianCenter, EquestrianCenter.BreedMare.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<EquestrianCenter, EquestrianCenter.BreedMare.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool InRabbitHole()
        {
            try
            {
                SimDescription male;
                int num;
                if (!PetAdoption.BreedMare(Actor, Target, Horse.FullName, out male, out num))
                {
                    return false;
                }

                CancellableByPlayer = false;

                Pregnancies.Start(Horse, male, false);

                Actor.ModifyFunds(-num);

                EventTracker.SendEvent(new WooHooEvent(EventTypeId.kWooHooed, Horse, null, Target));

                StartStages();
                BeginCommodityUpdates();
                
                bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));

                AssignHorseAndPosture(Horse, RabbitHole.SimRabbitHoleState.Leading);
                
                EndCommodityUpdates(succeeded);

                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : EquestrianCenter.BreedMare.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new BreedMareEx();
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
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BreedMare.LocalizeString(a.IsFemale, "NeedHorse", new object[] { a }));
                        return false;
                    }
                }
                Sim container = posture.Container as Sim;
                if (!container.IsSelectable)
                {
                    return false;
                }

                if (!container.SimDescription.IsFemale)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BreedMare.LocalizeString(a.IsFemale, "NeedFemaleHorse", new object[] { a }));
                    return false;
                }
                if (container.SimDescription.Child)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BreedMare.LocalizeString(a.IsFemale, "NeedAdultHorse", new object[] { a }));
                    return false;
                }
                if (container.SimDescription.Elder)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BreedMare.LocalizeString(container.IsFemale, "ElderPetsTooOldForBreed", new object[] { container }));
                    return false;
                }

                /* Removed
                if (container.SimDescription.IsUnicorn)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BreedMare.LocalizeString(a.IsFemale, "UnicornCannotBeBreed", new object[] { a }));
                    return false;
                }
                if (!container.Household.CanAddSpeciesToHousehold(container.SimDescription.Species, 0x1, true))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BreedMare.LocalizeString(container.IsFemale, "TooManyHorsesInHousehold", new object[] { container }));
                    return false;
                }
                */

                if (container.SimDescription.IsPregnant)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(EquestrianCenter.BreedMare.LocalizeString(container.IsFemale, "AlreadyPregnant", new object[] { container }));
                    return false;
                }
                return true;
            }
        }
    }
}
