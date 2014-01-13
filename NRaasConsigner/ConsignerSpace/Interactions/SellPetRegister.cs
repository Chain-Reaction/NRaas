using NRaas.CommonSpace.Helpers;
using NRaas.ConsignerSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ConsignerSpace.Interactions
{
    public class SellPetRegister : Interaction<Sim, ShoppingRegister>, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<ShoppingRegister, ShoppingRegister.Buy.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP5)) return;

            if (GameUtils.IsInstalled(ProductVersion.EP2))
            {
                interactions.Add<ConsignmentRegister>(Singleton);
            }
            else
            {
                interactions.Add<GeneralStoreRegister>(Singleton);
            }

            interactions.Add<PetstoreRegister>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                Sim simInRole = Target.CurrentRole.SimInRole;
                InteractionInstance instance = SellPetSimInteraction.Singleton.CreateInstance(simInRole, Actor, Actor.InheritedPriority(), Autonomous, CancellableByPlayer);
                
                Actor.InteractionQueue.PushAsContinuation(instance, true);
                return true;
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

        public class Definition : InteractionDefinition<Sim, ShoppingRegister, SellPetRegister>
        {
            public override string GetInteractionName(Sim actor, ShoppingRegister target, InteractionObjectPair iop)
            {
                return Common.Localize("SellPet:MenuName", actor.IsFemale);
            }

            public override bool Test(Sim a, ShoppingRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!target.InteractionTestAvailability()) return false;

                    return (Households.NumPets(a.Household) > 0);
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