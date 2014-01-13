using NRaas.CommonSpace.Helpers;
using NRaas.ConsignerSpace.Helpers;
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
    public class SellPetSimInteraction : ShoppingRegister.BuyItemsWithRegister, Common.IPreLoad
    {
        public readonly static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<ShoppingRegister, ShoppingRegister.BuyItemsWithRegister.BuyItemsDefinition, Definition>(false);
        }

        public override void PostAnimation()
        {
            try
            {
                PetSale.DisplayDialog(Actor.SimDescription, null, false);
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

        public class Definition : ShoppingRegister.BuyItemsWithRegister.BuyItemsDefinition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.Localize("SellPet:MenuName", actor.IsFemale);
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SellPetSimInteraction();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}