using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class AdoptPet : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "AdoptPet";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP5)) return false;

            return base.Allow(parameters);
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.LotHome == null) return false;

            return base.PrivateAllow(me);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!UIUtils.IsOkayToStartModalDialog())
            {
                Common.Notify(Common.Localize("Pause:Failure"));
                return OptionResult.Failure;
            }

            return base.Run(parameters);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            SimDescription pet = PetAdoption.ShowAdoptPetPicker(true);
            if (pet == null)
            {
                Common.Notify(Common.Localize("AdoptPet:Failure"));
                return false;
            }

            string titleText = Common.LocalizeEAString(pet.IsFemale, "Gameplay/Actors/Sim/StrayPets:AdoptStrayNameTitle");
            string promptText = Common.LocalizeEAString(pet.IsFemale, "Gameplay/Actors/Sim/StrayPets:AdoptStrayNameDescription");
            pet.FirstName = StringInputDialog.Show(titleText, promptText, pet.FirstName, 256, StringInputDialog.Validation.SimNameText);

            pet.LastName = me.LastName;

            PetAdoption.GetPetOutOfPool(pet);

            if (pet.Household != null)
            {
                pet.Household.Remove(pet, false);
            }

            me.Household.Add(pet);

            pet.IsNeverSelectable = false;
            pet.WasAdopted = true;

            Relationships.CheckAddHumanParentFlagOnAdoption(me, pet);

            if (me.Partner != null)
            {
                Relationships.CheckAddHumanParentFlagOnAdoption(me.Partner, pet);
            }

            Instantiation.Perform(pet, null);
            return true;
        }
    }
}
